using System;
using System.Security.Cryptography;
namespace CrypticWytch
{
    public static class AesGcmHelper
    {
        private const int KeySize = 32;      
        private const int NonceSize = 12;    
        private const int TagSize = 16;      

        /// <summary>
        /// Шифрует данные с помощью AES-256-GCM
        /// </summary>
        /// <param name="plainText">Исходный текст (UTF-8)</param>
        /// <param name="key">Ключ (32 байта)</param>
        /// <returns>Base64-строка в формате: Nonce + Tag + Ciphertext</returns>
        public static string Encrypt(string plainText, byte[] key)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Текст не может быть пустым", nameof(plainText));

            if (key == null || key.Length != KeySize)
                throw new ArgumentException($"Ключ должен быть {KeySize} байт", nameof(key));

            byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            byte[] nonce = new byte[NonceSize];
            RandomNumberGenerator.Fill(nonce);

            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[TagSize];

            using var aes = new AesGcm(key, TagSize);
            aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

            byte[] result = new byte[NonceSize + TagSize + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
            Buffer.BlockCopy(cipherBytes, 0, result, NonceSize + TagSize, cipherBytes.Length);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Расшифровывает данные с помощью AES-256-GCM
        /// </summary>
        /// <param name="cipherBase64">Base64-строка (Nonce + Tag + Ciphertext)</param>
        /// <param name="key">Ключ (32 байта)</param>
        /// <returns>Расшифрованный текст (UTF-8)</returns>
        public static string Decrypt(string cipherBase64, byte[] key)
        {
            if (string.IsNullOrEmpty(cipherBase64))
                throw new ArgumentException("Шифротекст не может быть пустым", nameof(cipherBase64));

            if (key == null || key.Length != KeySize)
                throw new ArgumentException($"Ключ должен быть {KeySize} байт", nameof(key));

            byte[] combined = Convert.FromBase64String(cipherBase64);

            
            if (combined.Length < NonceSize + TagSize)
                throw new ArgumentException("Данные слишком короткие для корректного шифротекста");

            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];
            byte[] cipherBytes = new byte[combined.Length - NonceSize - TagSize];

            Buffer.BlockCopy(combined, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(combined, NonceSize, tag, 0, TagSize);
            Buffer.BlockCopy(combined, NonceSize + TagSize, cipherBytes, 0, cipherBytes.Length);

            byte[] plainBytes = new byte[cipherBytes.Length];

            using var aes = new AesGcm(key,TagSize);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

            return System.Text.Encoding.UTF8.GetString(plainBytes);
        }
    }
}