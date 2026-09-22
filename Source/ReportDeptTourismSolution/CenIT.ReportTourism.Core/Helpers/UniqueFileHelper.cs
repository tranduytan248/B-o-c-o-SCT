using System;
using System.Security.Cryptography;
using System.Text;

namespace CenIT.ReportTourism.Core.Helpers
{
    public class UniqueFileHelper
    {
        public static string GenUniqueKey(int iSize = 32)
        {
            var chars = new char[62];
            var a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            chars = a.ToCharArray();

            var crypto = new RNGCryptoServiceProvider();

            var data = new byte[iSize];
            crypto.GetNonZeroBytes(data);

            var result = new StringBuilder(iSize);

            foreach (var b in data)
                result.Append(chars[b % (chars.Length - 1)]);

            return Convert.ToString(result);
        }
    }
}