using System.Security.Cryptography;

namespace DotNetOpen.Common.Helpers
{
    public class RandomHelper
    {
        public static string GenerateCode(int strLength = 8)
        {
            var str = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            return GenerateRandomKeys(str, strLength);
        }

        public static string GenerateDigits(int strLength = 6)
        {
            var str = "0123456789";
            return GenerateRandomKeys(str, strLength);
        }
        public static byte[] GenerateBytes(int strLength)
        {
            var data = new byte[strLength]; 
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(data);
            } 
            return data;
        }

        private static string GenerateRandomKeys(string str, int strLength)
        {
            var data = new byte[strLength];
            var chArray = new char[strLength];
            var length = str.Length;
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(data);
            }
            for (int i = 0; i < strLength; i++)
            {
                chArray[i] = str[data[i] % length];
            }
            return new string(chArray);
        }
    }
}
