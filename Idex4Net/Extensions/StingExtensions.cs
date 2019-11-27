using System;

namespace Idex4Net.Extensions
{
    public static class StingExtensions
    {
        public static byte[] FromHexString(this string value)
        {
            var bytes = new byte[value.Length / 2];
            for (var i = 0; i < value.Length; i += 2)
                bytes[i >> 1] = Convert.ToByte(value.Substring(i, 2), 16);
            return bytes;
        }
    }
}