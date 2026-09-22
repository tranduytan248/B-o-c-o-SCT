using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace TSFramework.Core.Utils
{
    public class EHashMD5
    {
        public static string FromObject(object obj)
        {
            var jsonObject = JsonConvert.SerializeObject(obj);
            if (string.IsNullOrEmpty(jsonObject)) return string.Empty;

            return CalculateMD5Hash(jsonObject);
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }
    }
}