using Commons;
using Commons.Cache;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WeChatRelated
{
    /// <summary>
    /// 微信的公用方法
    /// </summary>
    public class Common
    {
        /// <summary>
        /// 存储网页授权token
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
       public static bool SetCache_OAuthAccessToken(object value)
        {
            if (string.IsNullOrWhiteSpace(Current.WxOpenId) || value==null)
            {
                return false;
            }
            return CacheService.GetCacheManager().Set(Current.WxOpenId+"_OAuthAccessToken", value, new TimeSpan(11, 50, 59), new TimeSpan(0));
        }
        /// <summary>
        /// 获取网页授权token
        /// </summary>
        /// <returns></returns>
        public static OAuthAccessTokenResult GetCache_OAuthAccessToken()
        {
            if (string.IsNullOrWhiteSpace(Current.WxOpenId))
            {
                return null;
            }
            return CacheService.GetCacheManager().Get<OAuthAccessTokenResult>(Current.WxOpenId + "_OAuthAccessToken");
        }


        /// <summary>
        /// 微信小程序解密算法
        /// </summary>
        /// <param name="encryptedData">加密数据</param>
        /// <param name="iv">初始向量</param>
        /// <param name="sessionKey">从服务端获取的SessionKey</param>
        /// <returns></returns>
        public string Decrypt(string encryptedData, string iv, string sessionKey)
        {
            //创建解密器生成工具实例
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            //设置解密器参数
            aes.Mode = CipherMode.CBC;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            //格式化待处理字符串
            byte[] byte_encryptedData = Convert.FromBase64String(encryptedData);
            byte[] byte_iv = Convert.FromBase64String(iv);
            byte[] byte_sessionKey = Convert.FromBase64String(sessionKey);

            aes.IV = byte_iv;
            aes.Key = byte_sessionKey;
            //根据设置好的数据生成解密器实例
            ICryptoTransform transform = aes.CreateDecryptor();

            //解密
            byte[] final = transform.TransformFinalBlock(byte_encryptedData, 0, byte_encryptedData.Length);
            //生成结果
            string result = Encoding.UTF8.GetString(final);
            return result;
        }
    }
}
