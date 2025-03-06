using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

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
            }
            catch (RedisConnectionException ex)
            {
                Console.WriteLine($"认证失败: {ex.Message}");
                throw ex;
            }
        }

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
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RedisValue GetValue(RedisKey key)
        {
            return _db.StringGet(key);
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
                var dicResult = result.ToDictionary().FirstOrDefault();
                // 新游标
                cursor = long.Parse(dicResult.Key);
                //var response = (RedisResult[])result;
                //cursor = long.Parse((string)response[0]);
                var keys = (RedisKey[])dicResult.Value;
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
        /// 向流中添加消息，row id value(json)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="streamPairs"></param>
        public void AddStream(RedisKey key, NameValueEntry[] streamPairs)
        {
            _db.StreamAdd(key, streamPairs);
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
        }
    }
}
