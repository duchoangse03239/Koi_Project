using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoiManagement.Common
{
    public static class CommonFunction
    {
        static Random rand = new Random();

        private const string Alphabet ="abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GenerateString(int size)
        {
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = Alphabet[rand.Next(Alphabet.Length)];
            }
            return new string(chars);
        }
    }
}