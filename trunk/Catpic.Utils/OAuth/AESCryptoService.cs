// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AESCryptoService.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Defines the AESCryptoService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Utils.OAuth
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Crypto services implementation which uses AES
    /// </summary>
    public class AESCryptoService : ICryptoService
    {
        /// <summary>
        /// Salt value
        /// </summary>
        private static readonly byte[] Salt = Encoding.ASCII.GetBytes("o6809942kbH8c5");

        /// <summary>
        /// Shared secret
        /// </summary>
        private readonly string _sharedSecret;

        /// <summary>
        /// Initializes a new instance of the <see cref="AESCryptoService"/> class.
        /// </summary>
        /// <param name="sharedSecret"> Shared secret. </param>
        public AESCryptoService(string sharedSecret)
        {
            this._sharedSecret = sharedSecret;
        }

        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using 
        /// DecryptStringAES().  The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText"> The text to encrypt. </param>
        /// <param name="sharedSecret"> A password used to generate a key for encryption. </param>
        /// <returns> Encrepted string. </returns>
        public static string EncryptStringAES(string plainText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException("plainText");
            }

            if (string.IsNullOrEmpty(sharedSecret))
            {
                throw new ArgumentNullException("sharedSecret");
            }

            string outStr;                       // Encrypted string to return
            RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes(sharedSecret, Salt);

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }

                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText"> The text to decrypt. </param>
        /// <param name="sharedSecret"> A password used to generate a key for decryption. </param>
        /// <returns> The decrypt string aes. </returns>
        public static string DecryptStringAES(string cipherText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException("cipherText");
            }

            if (string.IsNullOrEmpty(sharedSecret))
            {
                throw new ArgumentNullException("sharedSecret");
            }

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            try
            {
                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes(sharedSecret, Salt);

                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (var msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }

            return plaintext;
        }

        /// <summary>
        /// Encrypts data string
        /// </summary>
        /// <param name="data"> Data string. </param>
        /// <returns> Encrypted byte array </returns>
        public byte[] Encrypt(string data)
        {
            var encryptedData = EncryptStringAES(data, this._sharedSecret);
            var byteConverter = new UnicodeEncoding();
            return byteConverter.GetBytes(encryptedData);
        }

        /// <summary>
        /// Decrypts data string
        /// </summary>
        /// <param name="data"> Encrypted data array. </param>
        /// <returns> Target string</returns>
        public string Decrypt(byte[] data)
        {
            var byteConverter = new UnicodeEncoding();
            var encryptedString = byteConverter.GetString(data);
            return DecryptStringAES(encryptedString, this._sharedSecret);
        }

        /// <summary>
        /// Read byte array from stream
        /// </summary>
        /// <param name="s"> Target stream. </param>
        /// <returns> Byte array</returns>
        private static byte[] ReadByteArray(Stream s)
        {
            var rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            var buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}
