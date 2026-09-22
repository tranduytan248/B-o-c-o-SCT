using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TSFramework.Core.Members.Encode
{
    public class AesEncryptor
    {
        private static readonly int KeySize = 256;
        private static readonly int BlockSize = 128;

        /// <summary>
        ///     Encrypt byte data with password
        /// </summary>
        /// <param name="bytesToBeEncrypted">Byte data to be encrypted</param>
        /// <param name="passwordBytes">Byte data to encrypt</param>
        /// <returns></returns>
        private static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = {1, 2, 3, 4, 5, 6, 7, 8};

            using (var ms = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        /// <summary>
        ///     Decrypt data with password
        /// </summary>
        /// <param name="bytesToBeDecrypted">Byte data to be decrypted</param>
        /// <param name="passwordBytes">Byte data to decrypt</param>
        /// <returns></returns>
        private static byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = {1, 2, 3, 4, 5, 6, 7, 8};

            using (var ms = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        /// <summary>
        ///     Encrypt text data with password
        /// </summary>
        /// <param name="input">String data to be encrypted</param>
        /// <param name="password">String password to encrypt</param>
        /// <returns></returns>
        public static string EncryptText(string input, string password)
        {
            // Get the bytes of the string
            var bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var bytesEncrypted = Encrypt(bytesToBeEncrypted, passwordBytes);

            var result = Convert.ToBase64String(bytesEncrypted);

            return result;
        }

        /// <summary>
        ///     Decrypt text data by password
        /// </summary>
        /// <param name="input">String data to be decrypted</param>
        /// <param name="password">String password to decrypt</param>
        /// <returns></returns>
        public static string DecryptText(string input, string password)
        {
            // Get the bytes of the string
            var bytesToBeDecrypted = Convert.FromBase64String(input);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var bytesDecrypted = Decrypt(bytesToBeDecrypted, passwordBytes);

            var result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }

        /// <summary>
        ///     Encrypt content file with password
        /// </summary>
        /// <param name="filePath">Path of file want to encrypt</param>
        /// <param name="password">Password to encrypt</param>
        /// <param name="outFilePath">Path of output file encrypted</param>
        public static void EncryptFile(string filePath, string password, string outFilePath)
        {
            var bytesToBeEncrypted = File.ReadAllBytes(filePath);
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var bytesEncrypted = Encrypt(bytesToBeEncrypted, passwordBytes);

            File.WriteAllBytes(outFilePath, bytesEncrypted);
        }

        /// <summary>
        ///     Decrypt content file by password
        /// </summary>
        /// <param name="filePath">Path of file want to decrypt</param>
        /// <param name="password">Password to decrypt</param>
        /// <param name="outFilePath">Path of output file decrypted</param>
        public static void DecryptFile(string filePath, string password, string outFilePath)
        {
            var bytesToBeDecrypted = File.ReadAllBytes(filePath);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var bytesDecrypted = Decrypt(bytesToBeDecrypted, passwordBytes);

            File.WriteAllBytes(outFilePath, bytesDecrypted);
        }
    }
}