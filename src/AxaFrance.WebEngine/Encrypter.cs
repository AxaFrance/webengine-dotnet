// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: Huaxing YUAN, at: 2022-8-1 10:19
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AxaFrance.WebEngine.Web")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("AxaFrance.WebEngine.MobileApp")]
namespace AxaFrance.WebEngine
{
    /// <summary>
    /// A simple encrypter helps you to avoid storing sensitive test data 
    /// </summary>
    public static class Encrypter
    {

        /// <summary>
        /// Encrypt original text with the default encryption key and store the encrypted data into base64 string
        /// </summary>
        /// <param name="original">Plain text to be encrypted.</param>
        /// <returns>Encrypted data converted into Base64 string format.</returns>
        public static string Encrypt(string original)
        {
            var aes = Aes.Create();
            byte[] encryptionKey = Encoding.Unicode.GetBytes(Settings.Instance.encryptionKey);
            byte[] iv = new byte[16];

            Array.Resize(ref encryptionKey, 32);
            Array.Copy(encryptionKey, iv, 16);

            aes.Key = encryptionKey;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var encrypter = aes.CreateEncryptor(encryptionKey, iv);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encrypter, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(original);
                    }
                    var encrypted = msEncrypt.ToArray();
                    return Convert.ToBase64String(encrypted);
                }
            }
        }

        /// <summary>
        /// Decrypt data with the default encryption key from a base64 string
        /// </summary>
        /// <param name="encryptedBase64">Encrypted data using Base64 Format</param>
        /// <returns>Decrypted data</returns>
        internal static string Decrypt(string encryptedBase64)
        {
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                byte[] iv = new byte[16];
                byte[] encryptionKey = Encoding.Unicode.GetBytes(Settings.Instance.encryptionKey);
                Array.Resize(ref encryptionKey, 32);
                Array.Copy(encryptionKey, iv, 16);

                aesAlg.Key = encryptionKey;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv);

                byte[] content = Convert.FromBase64String(encryptedBase64);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(content))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
