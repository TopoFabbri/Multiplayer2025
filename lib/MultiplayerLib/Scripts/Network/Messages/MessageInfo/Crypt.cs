using System;
using System.IO;
using System.Security.Cryptography;
using Multiplayer.Utils;

namespace Multiplayer.Network.Messages.MessageInfo
{
    public static class Crypt
    {
        private const int KeySize = 256; // Can be 128, 192, or 256 bits
        private const int BlockSize = 128; // AES block size is 128 bits
        private static byte[] _key;
        private static byte[] _iv;

        public static void GenerateOperations(uint seed)
        {
            // Instead of generating operations, we'll generate a cryptographic key
            using RNGCryptoServiceProvider rng = new();
            _key = new byte[KeySize / 8];
            _iv = new byte[BlockSize / 8];
            
            // Use the seed to generate deterministic key and IV
            using Rfc2898DeriveBytes deriveBytes = new(
                seed.ToString(), 
                new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, // Salt
                1000);
            
            _key = deriveBytes.GetBytes(KeySize / 8);
            _iv = deriveBytes.GetBytes(BlockSize / 8);
        }

        public static byte[] Encrypt(byte[] data)
        {
            if (data == null || data.Length == 0)
                return data;

            using Aes aes = Aes.Create();
            
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using MemoryStream memoryStream = new();
            
            // First byte for IsCrypted flag
            memoryStream.WriteByte(1);

            using (CryptoStream cryptoStream = new(
                       memoryStream,
                       aes.CreateEncryptor(),
                       CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();
            }

            return memoryStream.ToArray();
        }

        public static byte[] Decrypt(byte[] data)
        {
            if (data == null || data.Length <= 1)
                return data;

            try
            {
                // Skip the first byte (IsCrypted flag)
                byte[] encryptedData = new byte[data.Length - 1];
                Array.Copy(data, 1, encryptedData, 0, data.Length - 1);

                using Aes aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using MemoryStream memoryStream = new();
                
                using (CryptoStream cryptoStream = new(
                           memoryStream,
                           aes.CreateDecryptor(),
                           CryptoStreamMode.Write))
                {
                    cryptoStream.Write(encryptedData, 0, encryptedData.Length);
                    cryptoStream.FlushFinalBlock();
                }

                return memoryStream.ToArray();
            }
            catch (CryptographicException)
            {
                // Handle decryption errors
                return data;
            }
        }

        public static bool IsCrypted(byte[] data)
        {
            return data is { Length: > 0 } && data[0] == 1;
        }
    }
}