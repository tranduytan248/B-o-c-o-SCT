using System.Security.Cryptography;
using System.Text;
using TSFramework.Core.Utils.Crypto;

namespace TSFramework.Core.Utils
{
    public class UPasswordHash
    {
        public static string GenerateHashPassword(string password)
        {
            ICryptoService cryptoService = new Pbkdf2Crypto();
            return cryptoService.Compute(password);
        }

        public static string GenerateSalt(string password)
        {
            ICryptoService cryptoService = new Pbkdf2Crypto();
            return cryptoService.GenerateSalt(32);
        }

        public static string GenerateCryptoPassword(string password, string salt)
        {
            var algorithm = new SHA1CryptoServiceProvider(); //or use SHA256.Create();
            var hashData = algorithm.ComputeHash(Encoding.UTF8.GetBytes(password + salt));

            var sb = new StringBuilder();
            foreach (var b in hashData)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static bool IsCompare(string cryptoPassword1, string cryptoPassword2)
        {
            ICryptoService cryptoService = new Pbkdf2Crypto();
            return cryptoService.Compare(cryptoPassword1, cryptoPassword2);
        }
    }
}