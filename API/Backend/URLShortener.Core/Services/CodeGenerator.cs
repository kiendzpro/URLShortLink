using System;
using System.Security.Cryptography;
using System.Text;

namespace URLShortener.Core.Services
{
    public static class CodeGenerator
    {
        private static readonly char[] CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        
        /// <summary>
        /// Tạo mã ngẫu nhiên với độ dài nhất định
        /// </summary>
        /// <param name="length">Độ dài của mã, mặc định là 6</param>
        /// <returns>Mã ngẫu nhiên</returns>
        public static string GenerateCode(int length = 6)
        {
            var result = new StringBuilder(length);
            var random = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            
            random.GetBytes(bytes);
            
            for (int i = 0; i < length; i++)
            {
                result.Append(CHARS[bytes[i] % CHARS.Length]);
            }
            
            return result.ToString();
        }
        
        /// <summary>
        /// Tạo mã ngẫu nhiên dựa trên URL gốc
        /// </summary>
        /// <param name="url">URL gốc cần tạo mã</param>
        /// <param name="length">Độ dài của mã, mặc định là 6</param>
        /// <returns>Mã dựa trên URL</returns>
        public static string GenerateCodeFromUrl(string url, int length = 6)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(url + Guid.NewGuid().ToString()));
            
            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(CHARS[hashBytes[i] % CHARS.Length]);
            }
            
            return result.ToString();
        }
    }
} 