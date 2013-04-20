using System;
using System.Security.Cryptography;

namespace Catpic.Utils.OAuth
{
    public static class OAuthHelper
    {
        public static string Hash(string value)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(value);
            data = x.ComputeHash(data);
            string ret = "";
            for (int i = 0; i < data.Length; i++)
                ret += data[i].ToString("x2");
            return ret; 
        }

        public static string GetCacheKey(string appId, string ownerId, string serviceId)
        {
            return Hash(String.Format("{0}:{1}:{2}", appId, ownerId, serviceId));
        }
    }
}
