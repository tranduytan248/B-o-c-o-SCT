using System;
using System.Security.Cryptography;
using System.Text;

namespace TSFramework.Core.Members.Encode
{
    public class RsaEncryptor
    {
        private static readonly bool _optimalAsymmetricEncryptionPadding = false;

        //These keys are of 2048byte
        private static readonly string PublicKey =
            "MjA0OCE8UlNBS2V5VmFsdWU+PE1vZHVsdXM+djFTTVVyYk5SZW50VDEya0FhWXNRMEh3Y2hjWG9nbnFUWGpYd1NXaGR5Qi9aaTQ5VnF4L0lFdWxSaGFhVjdHOUtENWRmY0I4eEZaZGgyNGJ0MHpZbGFNTlFyRVBNNnQzUEdvZXZmMXVCby9wVnhlcWFocEFkWkIwelNJcjhwTk5UOW52czV5WEN1Q00xRFo0UUR3Q3A3b2U2aXc2ZHZ4VEZNWFZJdW9rSkcrdmlFMWhORDhnbGg0dFVsMWVBdThKT3YyR0tyWmhvTmUxK2tnRzNNUmRueEFGTDQyRDl4eWF5NERvcmpGL2ZjYWNNc3dFYkM3MUo2bFNobnR2YnQ1RnY0elY1bkg0aDhqYzhnV1dQVDUvWG16TElLMmlJRDJ6L3NyeGgvbzdMRkRhWVhXMnVwbUt5VUJQR2k0OGJLUVZKT3JjZU9rd3owWE1nTDFJUk4yWnhRPT08L01vZHVsdXM+PEV4cG9uZW50PkFRQUI8L0V4cG9uZW50PjwvUlNBS2V5VmFsdWU+";

        private static readonly string PrivateKey =
            "MjA0OCE8UlNBS2V5VmFsdWU+PE1vZHVsdXM+djFTTVVyYk5SZW50VDEya0FhWXNRMEh3Y2hjWG9nbnFUWGpYd1NXaGR5Qi9aaTQ5VnF4L0lFdWxSaGFhVjdHOUtENWRmY0I4eEZaZGgyNGJ0MHpZbGFNTlFyRVBNNnQzUEdvZXZmMXVCby9wVnhlcWFocEFkWkIwelNJcjhwTk5UOW52czV5WEN1Q00xRFo0UUR3Q3A3b2U2aXc2ZHZ4VEZNWFZJdW9rSkcrdmlFMWhORDhnbGg0dFVsMWVBdThKT3YyR0tyWmhvTmUxK2tnRzNNUmRueEFGTDQyRDl4eWF5NERvcmpGL2ZjYWNNc3dFYkM3MUo2bFNobnR2YnQ1RnY0elY1bkg0aDhqYzhnV1dQVDUvWG16TElLMmlJRDJ6L3NyeGgvbzdMRkRhWVhXMnVwbUt5VUJQR2k0OGJLUVZKT3JjZU9rd3owWE1nTDFJUk4yWnhRPT08L01vZHVsdXM+PEV4cG9uZW50PkFRQUI8L0V4cG9uZW50PjxQPi8yY1VJS2RlMFB1b2RVaDJQQ3krbFU0aWFvVWtOZ0dOOVhHNmhvcll3c1ovbzdwdTJYZjZmS2E5M09OZ1R0NUpqaW5QL3grZG9ibmFiU1hNNFNwRGJlb3JVRGZBKzhYeDIxTHBCT0FtYUtUVWlkejNjMHlQRXBQZ3lOMlpVb3poUWhjejZlUk01cUdQSlgxU29WMjczM3ZUREFtTEVWS0N4eFRZOHVNSWI3OD08L1A+PFE+djhjYlBmcHh5aXZUelhsV2Q5L3hNK3pRUlJRSk4rTDFIYURiNHYxKzU3dExEb3VlcG03ajI0MkJFZ2U4dTNENmJEanZneWhBWFIxV3IwR09KSjBBb1ZPV2FLLzdvZ3NHZjBnM1dzNzVicWtWSmdNTHZETnFxSVVVd0ZqZml3TllONkJnN0dIdGl2S0VGdmJldTEzcGFxVERyTnFuV0ZQaWFQK1lkQ09xVjNzPTwvUT48RFA+eWVSVDF0UTNjWC9kMUlocFhud0lVOEltRm9vVTY5UWl3YWtiUjR1dWVabXNBR001aVJMOG9WaTFzVXpVTHNRczVRSk1kMklvbTFWdFF1YWtwRUZpZUJxcURvbGtOaUp0WTNDUTN0Zkp4T0szV0J1aVNEUjJ6THEwOEZPc0JjTnp0V2plRXIvendrUm9BYnlsZXdXN281Z2dadDJNWHk4WVRnTSsxQkYvODhVPTwvRFA+PERRPlRGOUxYd1JFbW9HWHFJVkF4UjVlblJJYTR0ZVcwRFhHN1pTbzNKMmRFMFhJSHpQRTYzelBxeGlRSlJFRnZSUEI5cVU1NU41N3UxazZzektGRzltV2JhaXZCbVBHN3dJN0JTZEtQQlNleXMzMUNSMC9hQ1NGdmpTNVRkeFdzYktVU0JyTFhuZWxOS2RkcVJPSkljN0ZiTjNPdXlDY2NoVjkzZGlqNnVSbEtzOD08L0RRPjxJbnZlcnNlUT5rSmYwVHZoNDZjTEQ4OElIVVZ0V3hYaDVsYlNUTWw2ZnB5cFhhUU9laUtpTy9XcnZic21waXdBVEhDQ0pERDhYdDFwbTc5K0hrc21sUjlrYktXR2U4WmNqZHJHdUZlZ3NDUGRpT3VGMVN0a283NWtnblJVY0ZTb1hxSzF1YVgvTWsxTEtDbVpZY3djQ0t2VC9OQUZrWVpVdVNqT3pPckVrRk9VNDdML3VDVE09PC9JbnZlcnNlUT48RD5HbTMyZUZLU0pvODYzZFRFbkFtMVlaRVJRdUZYdldWN1BUcHRLMXdrWXMxVmErc0ZSQnpON3Nza1NIdEUxTXBUbytTQmk2WjBWYmJNY3JIT0dGTUFOQ055Nkh5RzZnOU1pRWJzZWpndzQ2MHJnWUZlWkF1K1RiOG5zMUorR2FNcGNkZGNHa2FPUXMxa0JzaURjZlFZTmMwckNoUVQrMjI5bUVmL3VqUDN6Q1IzcUNzdkZjVTRuMkMwZzBYSWhLQ1dHYXRsbW5MOW9FMWN0MzY4aWZYK0JCUVljUExqSE05TTZaSU9pMWtmR3M2bXhaT0V3cm1BWFB0T0ZweW1tNlZjMUM4WGtVUENCVERtWUZTSFpiaHNaT09IZHpaVVlUa2lmN1VzRk40MjdTSDVrMTNpQTVGRGJTb053bW9kQ0ZrWitENGJNQ2JUZWgwVTNvell6M3FnM1E9PTwvRD48L1JTQUtleVZhbHVlPg==";

        public static string Encrypt(string plainText)
        {
            int keySize;
            string publicKeyXml;

            GetKeyFromEncryptionString(PublicKey, out keySize, out publicKeyXml);

            var encrypted = Encrypt(Encoding.UTF8.GetBytes(plainText), keySize, publicKeyXml);

            return Convert.ToBase64String(encrypted);
        }

        private static byte[] Encrypt(byte[] data, int keySize, string publicKeyXml)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "data");
            var maxLength = GetMaxDataLength(keySize);
            if (data.Length > maxLength) throw new ArgumentException($"Maximum data length is {maxLength}", "data");
            if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (string.IsNullOrEmpty(publicKeyXml)) throw new ArgumentException("Key is null or empty", "publicKeyXml");

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicKeyXml);
                return provider.Encrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }

        public static string Decrypt(string encryptedText)
        {
            int keySize;
            string publicAndPrivateKeyXml;

            GetKeyFromEncryptionString(PrivateKey, out keySize, out publicAndPrivateKeyXml);

            var decrypted = Decrypt(Convert.FromBase64String(encryptedText), keySize, publicAndPrivateKeyXml);

            return Encoding.UTF8.GetString(decrypted);
        }

        private static byte[] Decrypt(byte[] data, int keySize, string publicAndPrivateKeyXml)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "data");
            if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (string.IsNullOrEmpty(publicAndPrivateKeyXml))
                throw new ArgumentException("Key is null or empty", "publicAndPrivateKeyXml");

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicAndPrivateKeyXml);
                return provider.Decrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }

        private static int GetMaxDataLength(int keySize)
        {
            if (_optimalAsymmetricEncryptionPadding) return (keySize - 384) / 8 + 7;

            return (keySize - 384) / 8 + 37;
        }

        private static bool IsKeySizeValid(int keySize)
        {
            return keySize >= 384 && keySize <= 16384 && keySize % 8 == 0;
        }

        private static void GetKeyFromEncryptionString(string rawkey, out int keySize, out string xmlKey)
        {
            keySize = 0;
            xmlKey = "";

            if (string.IsNullOrEmpty(rawkey)) return;
            var keyBytes = Convert.FromBase64String(rawkey);
            var stringKey = Encoding.UTF8.GetString(keyBytes);

            if (!stringKey.Contains("!")) return;
            var splittedValues = stringKey.Split(new[] {'!'}, 2);

            keySize = int.Parse(splittedValues[0]);
            xmlKey = splittedValues[1];
        }
    }
}