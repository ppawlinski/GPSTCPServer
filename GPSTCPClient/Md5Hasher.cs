﻿using System;
using System.Text;
using System.Security.Cryptography;
namespace GPSTCPClient
{
    public static class Md5Hasher
    {
        public static string CreateMD5(string input)
        {
            string result = "";
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                result = Convert.ToBase64String(data);
            }
            return result;
        }
    }
}
