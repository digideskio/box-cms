﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Box.Composition
{
    public static class CryptUtil
    {
        private static string key = ConfigurationSettings.AppSettings["ENCRYPT_KEY"] as string;
        private static string iv = ConfigurationSettings.AppSettings["ENCRYPT_IV"] as string;

        private static SymmetricAlgorithm GetAlgorithm()
        {
            SymmetricAlgorithm alg = new RijndaelManaged();
            alg.Key = Convert.FromBase64String(key);
            alg.IV = Convert.FromBase64String(iv);

            return alg;
        }

        public static byte[] EncryptBytes(byte[] file)
        {
            SymmetricAlgorithm alg = GetAlgorithm();
            byte[] encrypted;

            using (var stream = new MemoryStream())
            using (var encrypt = new CryptoStream(stream, alg.CreateEncryptor(), CryptoStreamMode.Write))
            {
                encrypt.Write(file, 0, file.Length);
                encrypt.FlushFinalBlock();
                encrypted = stream.ToArray();
            }

            alg.Clear();
            return encrypted;
        }

        public static byte[] DecryptBytes(byte[] file)
        {
            SymmetricAlgorithm alg = GetAlgorithm();
            byte[] decrypted;

            using (var stream = new MemoryStream(file))
            using (var encrypt = new CryptoStream(stream, alg.CreateDecryptor(), CryptoStreamMode.Write))
            {
                encrypt.Write(file, 0, file.Length);
                encrypt.FlushFinalBlock();
                decrypted = stream.ToArray();
            }
            alg.Clear();
            return decrypted;
        }   
    }
}