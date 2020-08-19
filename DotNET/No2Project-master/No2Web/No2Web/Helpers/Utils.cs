using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace No2Web.Helpers
{
    public static class Utils
    {
        public static class HashString
        {
            public static string GetMD5(string text)
            {
                MD5 hash = MD5.Create();

                byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }

                hash.Clear();

                return builder.ToString();
            }
        }
    }
}
