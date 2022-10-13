using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WishList.Business.Tools
{
    public static class Crypto
    {
        public static string DigestString(string str, string salt)
        {
            var hashFunc = SHA256.Create();
            var data = Encoding.UTF8.GetBytes(str + salt);
            var hashedPassword = hashFunc.ComputeHash(data);
            return Convert.ToBase64String(hashedPassword);
        }

        public static string GenerateSalt()
        {
            var random = new Random();
            var salt = new byte[32];
            random.NextBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public static bool VerifyString(string validStr, string salt, string str)
        {
            var hashedPassword = DigestString(str, salt);
            return validStr.Equals(hashedPassword);
        }
    }
}
