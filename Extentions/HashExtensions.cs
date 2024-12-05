using System;
using System.Collections.Generic;

namespace ApiPyrus.Extentions
{
    internal static class HashExtensions
    {
        internal static string ToHexString(this IList<byte> hash, bool lowerCase = false)
        {
            if (hash == null)
                throw new ArgumentNullException(nameof(hash));

            var chArrayLength = hash.Count * 2;

            var chArray = new char[chArrayLength];
            for (int i = 0, index = 0; i < chArrayLength; i += 2, index++)
            {
                var b = hash[index];
                chArray[i] = GetHexValue(b / 16, lowerCase);
                chArray[i + 1] = GetHexValue(b % 16, lowerCase);
            }

            return new string(chArray);
        }

        private static char GetHexValue(int i, bool lowerCase)
        {
            var startChar = lowerCase ? 'a' : 'A';
            return i < 10
                ? (char)(i + '0')
                : (char)(i - 10 + startChar);
        }
    }
}
