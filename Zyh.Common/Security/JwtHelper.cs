//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/12/12 16:07:38</date>
//------------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Zyh.Common.Security
{
    public class JwtHelper
    {
        // 密钥，用于签名Token
        private static byte[]? _rootKey = null;
        private static int accessTokenMins = 10;
        private static int refreshTokenMins = 24 * 60;

        public static bool Init()
        {
            if (_rootKey == null)
            {
                using (var rng = new RNGCryptoServiceProvider())
                {
                    _rootKey = new byte[256];
                    rng.GetBytes(_rootKey);
                }
            }
            return true;
        }

        /// <summary>
        /// 生成AccessToken
        /// </summary>
        /// <param name="username">这里测试用的是用户信息，可以传入其他信息</param>
        /// <returns></returns>
        public static string GenerateAccessToken(string username)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            //------------生成AccessToken----------------------------------
            // token中的claims用于储存自定义信息，如登录之后的用户id等
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,username),
                // new Claim(ClaimTypes.Role,"admin")
            };
            var key = new SymmetricSecurityKey(_rootKey);
            //生成Token两种方式
            //方式一
            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Issuer = issuer,
            //    Audience = "testClient",
            //    NotBefore = DateTime.Now, // 预设值就是 DateTime.Now
            //    IssuedAt = DateTime.Now, // 预设值就是 DateTime.Now
            //    Subject = new ClaimsIdentity(claims),
            //    Expires = DateTime.Now.AddMinutes(30),
            //    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            //};
            //var securityToken = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            //var serializeToken = jwtSecurityTokenHandler.WriteToken(securityToken);
            //方式二
            var token = new JwtSecurityToken(
                issuer: "WebServer",                    // 发布者
                audience: "WebClient",                // 接收者
                notBefore: DateTime.Now,                                                          // token签发时间
                expires: DateTime.Now.AddMinutes(accessTokenMins),                                             // token过期时间
                claims: claims,                                                                   // 该token内存储的自定义字段信息
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)    // 用于签发token的秘钥算法
            );
            return jwtSecurityTokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 生成RefreshToken
        /// </summary>
        /// <returns></returns>
        public static string GenerateRefreshToken()
        {
            //var refClaims = new[]
            //{
            //   new Claim("role","refresh")
            //};
            var key = new SymmetricSecurityKey(_rootKey);
            var refreshToken = new JwtSecurityToken(
                issuer: "WebServer",                    // 发布者
                audience: "WebClient",                // 接收者
                notBefore: DateTime.Now,                                                          // token签发时间
                expires: DateTime.Now.AddMinutes(refreshTokenMins),                                             // token过期时间
                                                                                                                // claims: refClaims,                                                                   // 该token内存储的自定义字段信息
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)    // 用于签发token的秘钥算法
            );

            // 返回成功信息，写出token
            return new JwtSecurityTokenHandler().WriteToken(refreshToken);
        }

        /// <summary>
        /// 刷新accessToken
        /// </summary>
        /// <param name="accessToken">过期的accessToken</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string RefreshToken(string accessToken)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            bool isCan = jwtSecurityTokenHandler.CanReadToken(accessToken);//验证Token格式
            if (!isCan)
                throw new Exception("传入访问令牌格式错误");
            //var jwtToken = jwtSecurityTokenHandler.ReadJwtToken(refreshtoken);//转换类型为token，不用这一行
            var validateParameter = new TokenValidationParameters()//验证参数
            {
                ValidateAudience = true,
                // 验证发布者
                ValidateIssuer = true,
                // 验证过期时间
                ValidateLifetime = false,
                // 验证秘钥
                ValidateIssuerSigningKey = true,
                // 读配置Issure
                ValidIssuer = "WebServer",
                // 读配置Audience
                ValidAudience = "WebClient",
                // 设置生成token的秘钥
                IssuerSigningKey = new SymmetricSecurityKey(_rootKey)
            };

            //验证传入的过期的AccessToken
            SecurityToken validatedToken = null;
            try
            {
                jwtSecurityTokenHandler.ValidateToken(accessToken, validateParameter, out validatedToken);//微软提供的验证方法。那个out传出的参数，类型是是个抽象类，记得转换
            }
            catch (SecurityTokenException)
            {
                throw new Exception("传入AccessToken被修改");
            }
            // 获取SecurityKey
            var key = new SymmetricSecurityKey(_rootKey);
            var jwtToken = validatedToken as JwtSecurityToken;//转换一下
            var accClaims = jwtToken.Claims;
            var access_Token = new JwtSecurityToken(
                    issuer: "WebServer",                    // 发布者
                    audience: "WebClient",                // 接收者
                    notBefore: DateTime.Now,                                                          // token签发时间
                    expires: DateTime.Now.AddMinutes(accessTokenMins),                                             // token过期时间
                    claims: accClaims,                                                                   // 该token内存储的自定义字段信息
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)    // 用于签发token的秘钥算法
                );
            // 返回成功信息，写出token
            return new JwtSecurityTokenHandler().WriteToken(access_Token);
        }

        public static bool CheckToken(string authorization, string username)
        {
            try
            {
                string token = authorization.Substring("Bearer ".Length).Trim();
                var jwtToken = new JwtSecurityToken(token);
                if (jwtToken == null || jwtToken.Payload == null)
                {
                    return false;
                }

                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(username) && !string.Equals(jwtToken.Payload.Sub, username, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
