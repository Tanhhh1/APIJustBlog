using Application.Interfaces.Security;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security
{
    public class AesEncryptionService : IEncryptionService
    {
        private readonly byte[] key;

        public AesEncryptionService(IConfiguration configuration)
        {
            var section = configuration.GetSection("AesEncryption") ?? throw new ArgumentException("AES_KEY not found in environment variables");
            key = Convert.FromBase64String(section["Key"]);
        }
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            byte[] data = Encoding.UTF8.GetBytes(plainText);

            byte[] nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            byte[] cipher = new byte[data.Length];
            byte[] tag = new byte[16];

            var aes = new AesGcm(key);
            aes.Encrypt(nonce, data, cipher, tag);

            byte[] result = new byte[nonce.Length + cipher.Length + tag.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(cipher, 0, result, nonce.Length, cipher.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length + cipher.Length, tag.Length);

            return Convert.ToBase64String(result);
        }
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            byte[] fullData = Convert.FromBase64String(cipherText);

            byte[] nonce = new byte[12];
            byte[] tag = new byte[16];
            byte[] cipher = new byte[fullData.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(fullData, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(fullData, nonce.Length, cipher, 0, cipher.Length);
            Buffer.BlockCopy(fullData, nonce.Length + cipher.Length, tag, 0, tag.Length);

            byte[] plain = new byte[cipher.Length];

            try
            {
                using var aesGcm = new AesGcm(key);
                aesGcm.Decrypt(nonce, cipher, tag, plain);
            }
            catch (CryptographicException)
            {
                throw new InvalidOperationException("Decryption failed: data may have been tampered with.");
            }

            return Encoding.UTF8.GetString(plainText);
        }
    }
}
