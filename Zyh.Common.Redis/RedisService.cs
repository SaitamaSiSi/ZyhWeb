using StackExchange.Redis;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Zyh.Common.Redis
{
    /// <summary>
    /// Redis帮助类
    /// </summary>
    public class RedisService : IDisposable
    {
        /// <summary>
        /// Redis连接类
        /// </summary>
        private ConnectionMultiplexer _redis;

        /// <summary>
        /// Redis数据对象类
        /// </summary>
        private readonly IDatabase _db;

        /// <summary>
        /// Redis消息订阅类
        /// </summary>
        private readonly ISubscriber _subscriber;

        /// <summary>
        /// Redis红锁工厂
        /// </summary>
        private readonly RedLockFactory _lockFactory;

        /// <summary>
        /// 超时时间
        /// </summary>
        private readonly TimeSpan _defaultExpiryTime = TimeSpan.FromSeconds(30);

        /// <summary>
        /// 等待时间
        /// </summary>
        private readonly TimeSpan _defaultWaitTime = TimeSpan.FromSeconds(10);

        /// <summary>
        /// 重试时间
        /// </summary>
        private readonly TimeSpan _defaultRetryTime = TimeSpan.FromMilliseconds(200);

        #region 公共操作

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="pwd"></param>
        public RedisService(string ip, int port, string pwd)
        {
            try
            {
                var config = new ConfigurationOptions
                {
                    EndPoints = { $"{ip}:{port}" }, // Redis 地址和端口
                    Password = pwd, // Redis 密码
                    AbortOnConnectFail = false,       // 连接失败时不终止
                    ConnectTimeout = 5000             // 连接超时时间（毫秒）
                };
                _redis = ConnectionMultiplexer.Connect(config);
                _db = _redis.GetDatabase();
                _subscriber = _redis.GetSubscriber();

                List<RedLockEndPoint> redLockEndPoints = new List<RedLockEndPoint>()
                {
                    new RedLockEndPoint()
                    {
                        EndPoint = new DnsEndPoint(ip, port),
                        Password = pwd
                    }
                };
                _lockFactory = RedLockFactory.Create(redLockEndPoints);
            }
            catch (RedisConnectionException ex)
            {
                Console.WriteLine($"认证失败: {ex.Message}");
                throw ex;
            }
        }

        public void Test()
        {
            // TODO 使用带续期的锁
        }

        /// <summary>
        /// 尝试获取分布式锁并执行操作
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<bool> ExecuteWithLockAsync(string resourceName, Func<Task> action)
        {
            await using var redLock = await _lockFactory.CreateLockAsync(
                resourceName,
                _defaultExpiryTime,
                _defaultWaitTime,
                _defaultRetryTime);

            // 获取锁成功
            if (redLock.IsAcquired)
            {
                await action();
                return true;
            }

            // 获取锁失败
            return false;
        }

        /// <summary>
        /// 带返回值的锁操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceName"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<T> ExecuteWithLockAsync<T>(string resourceName, Func<Task<T>> func)
        {
            await using var redLock = await _lockFactory.CreateLockAsync(
                resourceName,
                _defaultExpiryTime,
                _defaultWaitTime,
                _defaultRetryTime);

            // 获取锁成功
            if (redLock.IsAcquired)
            {
                return await func();
            }

            // 获取锁失败
            return default;
        }

        /// <summary>
        /// 显式锁管理版本
        /// </summary>
        public async Task<bool> TryAcquireLockAsync(string resourceName, TimeSpan expiryTime)
        {
            var redLock = await _lockFactory.CreateLockAsync(
                resourceName,
                expiryTime);

            return redLock.IsAcquired;
        }

        /// <summary>
        /// 删除键，不能直接删除分组
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool DelKey(RedisKey key)
        {
            return _db.KeyDelete(key);
        }

        /// <summary>
        /// SCAN命令查找某个分组下的所有键
        /// </summary>
        /// <param name="pattern">格式group1:subgroup1:*</param>
        /// <param name="pageSize"></param>
        public List<RedisKey> ScanKeysManual(string pattern, int pageSize = 1000)
        {
            List<RedisKey> findKeys = new List<RedisKey>();
            IDatabase db = _redis.GetDatabase();
            long cursor = 0;
            do
            {
                // 执行 SCAN 命令
                RedisResult result = db.Execute("SCAN", cursor.ToString(), "MATCH", pattern, "COUNT", pageSize);

                // 解析响应（RedisResult 类型）
                var response = (RedisResult[])result;
                // 新游标
                cursor = long.Parse((string)response[0]);
                var keys = (RedisKey[])response[1];
                // 当前页的键
                if (keys != null)
                {
                    foreach (var key in keys)
                    {
                        findKeys.Add(key);
                    }
                }
            } while (cursor != 0);
            return findKeys;
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry">TimeSpan.FromSeconds(60)</param>
        public void SetOverTime(RedisKey key, TimeSpan expiry)
        {
            _db.KeyExpire(key, expiry);
        }

        /// <summary>
        /// 加载订阅，Literal-channel，Pattern-channel:*
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="action"></param>
        public void LoadSubscribe(RedisChannel channel, Action<RedisChannel, RedisValue> action)
        {
            if (_subscriber.SubscribedEndpoint(channel) == null)
            {
                _subscriber.Subscribe(channel, action);
            }
        }

        /// <summary>
        /// 卸载订阅，Literal和Pattern不要混用
        /// </summary>
        /// <param name="channel"></param>
        public void UnloadSubscribe(RedisChannel channel)
        {
            _subscriber.Unsubscribe(channel);
        }

        /// <summary>
        /// 发送订阅消息
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        public void SendPublish(RedisChannel channel, RedisValue message)
        {
            // 发布消息
            ISubscriber sub = _redis.GetSubscriber();
            sub.Publish(channel, message);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _redis?.Close();
            _redis?.Dispose();
            _lockFactory?.Dispose();
        }

        #endregion

        #region 值类型

        /// <summary>
        /// 设置值，可以设置 "Test1:Test2" 的值，value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(RedisKey key, RedisValue value)
        {
            return _db.StringSet(key, value);
        }

        /// <summary>
        /// 增加计数，如果不是数字会抛异常
        /// </summary>
        /// <param name="key"></param>
        public long IncrValue(RedisKey key, long value = 1)
        {
            return _db.StringIncrement(key, value);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RedisValue GetValue(RedisKey key)
        {
            return _db.StringGet(key);
        }

        #endregion

        #region 值列表类型

        /// <summary>
        /// 添加List数据 row value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        public void AddList(RedisKey key, RedisValue[] values)
        {
            // 向列表添加元素
            foreach (RedisValue value in values)
            {
                _db.ListRightPush(key, value);
            }
        }

        /// <summary>
        /// 获取List数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RedisValue[] GetList(RedisKey key)
        {
            return _db.ListRange(key);
        }

        #endregion

        #region 哈希列表类型

        /// <summary>
        /// 设置单个Hash字段 row key value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        public void SetHash(RedisKey key, HashEntry[] values)
        {
            _db.HashSet(key, values);
        }

        /// <summary>
        /// 增加计数，如果不是数字会抛异常
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public long IncrHash(RedisKey key, RedisValue field, long value = 1)
        {
            return _db.HashIncrement(key, field, value);
        }

        /// <summary>
        /// 获取单个Hash字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public RedisValue GetHash(RedisKey key, RedisValue field)
        {
            return _db.HashGet(key, field);
        }

        /// <summary>
        /// 删除单个Hash字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool DelHash(RedisKey key, RedisValue field)
        {
            return _db.HashDelete(key, field);
        }

        /// <summary>
        /// 获取所有Hash字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public HashEntry[] GetAllHash(string key)
        {
            return _db.HashGetAll(key);
        }

        #endregion

        #region 带分数的值列表

        /// <summary>
        /// 添加带层级的成员, row value score
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <param name="score"></param>
        public void SetSortedSet(RedisKey key, RedisValue member, double score)
        {
            _db.SortedSetAdd(key, member, score);
        }

        /// <summary>
        /// 增加计数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double IncrSrotedSet(RedisKey key, RedisValue member, double value = 1)
        {
            return _db.SortedSetIncrement(key, member, value);
        }

        #endregion

        #region 流类型

        /// <summary>
        /// 向流中添加消息，row id value(json)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="streamPairs"></param>
        public void AddStream(RedisKey key, NameValueEntry[] streamPairs)
        {
            _db.StreamAdd(key, streamPairs);
        }

        #endregion
    }
}
