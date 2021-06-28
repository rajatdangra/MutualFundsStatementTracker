using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Mutual_Funds_Statement_Tracker.Models
{
    public class AESEncryption
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        const string key = "PasswordPassword";
        /* Android encryption */
        public static string DecryptAndroid(string textToDecrypt)
        {
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                rijndaelCipher.KeySize = 0x80;
                rijndaelCipher.BlockSize = 0x80;
                byte[] encryptedData = Convert.FromBase64String(textToDecrypt.Replace(' ', '+'));
                byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
                byte[] keyBytes = new byte[0x10];
                int len = pwdBytes.Length;
                if (len > keyBytes.Length)
                {
                    len = keyBytes.Length;
                }
                Array.Copy(pwdBytes, keyBytes, len);
                rijndaelCipher.Key = keyBytes;
                rijndaelCipher.IV = keyBytes;
                byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                return Encoding.UTF8.GetString(plainText);
            }
            catch (Exception ex)
            {
                logger.Error("AESEncryption|DecryptAndroid:"+ex.Message);
                return null;
            }
        }

        public static string EncryptAndroid(string textToEncrypt)
        {
            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                rijndaelCipher.KeySize = 0x80;
                rijndaelCipher.BlockSize = 0x80;
                byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
                byte[] keyBytes = new byte[0x10];
                int len = pwdBytes.Length;
                if (len > keyBytes.Length)
                {
                    len = keyBytes.Length;
                }
                Array.Copy(pwdBytes, keyBytes, len);
                rijndaelCipher.Key = keyBytes;
                rijndaelCipher.IV = keyBytes;
                ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
                return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
            }
            catch (Exception ex)
            {
                logger.Error("AESEncryption|EncryptAndroid:" + ex.Message);

                return null;
            }
        }

        /*Android encyption */

        /// <summary>
        /// Encrpyts the sourceString, returns this result as an Aes encrpyted, BASE64 encoded string
        /// </summary>
        /// <param name="plainSourceStringToEncrypt">a plain, Framework string (ASCII, null terminated)</param>
        /// <param name="key">The pass phrase.</param>
        /// <returns>
        /// returns an Aes encrypted, BASE64 encoded string
        /// </returns>
        public static string EncryptIOS(string plainSourceStringToEncrypt)
        {
            try
            {
                //Set up the encryption objects
                using (AesCryptoServiceProvider acsp = GetProvider(Encoding.Default.GetBytes(key)))
                {
                    byte[] sourceBytes = Encoding.ASCII.GetBytes(plainSourceStringToEncrypt);
                    ICryptoTransform ictE = acsp.CreateEncryptor();

                    //Set up stream to contain the encryption
                    MemoryStream msS = new MemoryStream();

                    //Perform the encrpytion, storing output into the stream
                    CryptoStream csS = new CryptoStream(msS, ictE, CryptoStreamMode.Write);
                    csS.Write(sourceBytes, 0, sourceBytes.Length);
                    csS.FlushFinalBlock();

                    //sourceBytes are now encrypted as an array of secure bytes
                    byte[] encryptedBytes = msS.ToArray(); //.ToArray() is important, don't mess with the buffer

                    //return the encrypted bytes as a BASE64 encoded string
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
            catch (Exception ex)
            {
                logger.Error("AESEncryption|EncryptIOS:" + ex.Message);

                return null;
            }
        }


        /// <summary>
        /// Decrypts a BASE64 encoded string of encrypted data, returns a plain string
        /// </summary>
        /// <param name="base64StringToDecrypt">an Aes encrypted AND base64 encoded string</param>
        /// <param name="passphrase">The passphrase.</param>
        /// <returns>returns a plain string</returns>
        public static string DecryptIOS(string base64StringToDecrypt)
        {
            try
            {
                //Set up the encryption objects
                using (AesCryptoServiceProvider acsp = GetProvider(Encoding.Default.GetBytes(key)))
                {
                    base64StringToDecrypt = base64StringToDecrypt.Replace(' ', '+');
                    byte[] RawBytes = Convert.FromBase64String(base64StringToDecrypt);
                    ICryptoTransform ictD = acsp.CreateDecryptor();

                    //RawBytes now contains original byte array, still in Encrypted state

                    //Decrypt into stream
                    MemoryStream msD = new MemoryStream(RawBytes, 0, RawBytes.Length);
                    CryptoStream csD = new CryptoStream(msD, ictD, CryptoStreamMode.Read);
                    //csD now contains original byte array, fully decrypted

                    //return the content of msD as a regular string
                    return new StreamReader(csD).ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                logger.Error("AESEncryption|DecryptIOS:" + ex.Message);
                return "";
            }
        }

        private static AesCryptoServiceProvider GetProvider(byte[] key)
        {
            try
            {
                AesCryptoServiceProvider result = new AesCryptoServiceProvider();
                result.BlockSize = 128;
                result.KeySize = 128;
                result.Mode = CipherMode.CBC;
                result.Padding = PaddingMode.PKCS7;

                result.GenerateIV();
                result.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                byte[] RealKey = GetKey(key, result);
                result.Key = RealKey;
                // result.IV = RealKey;
                return result;
            }
            catch (Exception ex)
            {
               
                   logger.Error("AESEncryption|AesCryptoServiceProvider:"+ex.Message);
                return null;
            }
        }

        private static byte[] GetKey(byte[] suggestedKey, SymmetricAlgorithm p)
        {
            try
            {
                byte[] kRaw = suggestedKey;
                List<byte> kList = new List<byte>();

                for (int i = 0; i < p.LegalKeySizes[0].MinSize; i += 8)
                {
                    kList.Add(kRaw[i / 8 % kRaw.Length]);
                }
                byte[] k = kList.ToArray();
                return k;
            }
            catch (Exception ex)
            {

                logger.Error("AESEncryption|GetKey:" + ex.Message);

                return null;
            }
        }
    }
}