﻿#region Copyright
/*Copyright (c) 2016 Javus Software (Pty) Ltd

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
#endregion
using System;
using System.Text;
using PCLCrypto;

namespace Parq.Security
{
    public static class CryptoUtilities
    {
        const int IVSize = 16;

        public static byte[] GetAES256KeyMaterial()
        {
            return WinRTCrypto.CryptographicBuffer.GenerateRandom(32);
        }

        public static byte[] Get256BitSalt()
        {
            return WinRTCrypto.CryptographicBuffer.GenerateRandom(32);
        }

        public static byte[] Encrypt(byte[] plainText, byte[] keyMaterial)
        {
            var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider
                .OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);

            var key = provider.CreateSymmetricKey(keyMaterial);
            var IV = WinRTCrypto.CryptographicBuffer.GenerateRandom(IVSize);
            var cipher = WinRTCrypto.CryptographicEngine.Encrypt(key, plainText, IV);

            var cipherText = new byte[IV.Length + cipher.Length];

            IV.CopyTo(cipherText, 0);
            cipher.CopyTo(cipherText, IV.Length);

            return cipherText;
        }

        public static byte[] Decrypt(byte[] cipherText, byte[] keyMaterial)
        {
            var provider = WinRTCrypto.SymmetricKeyAlgorithmProvider
                .OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);

            var key = provider.CreateSymmetricKey(keyMaterial);

            byte[] IV = new byte[IVSize];
            Array.Copy(cipherText, IV, IV.Length);

            byte[] cipher = new byte[cipherText.Length - IVSize];
            Array.Copy(cipherText, IVSize, cipher, 0, cipher.Length);

            return WinRTCrypto.CryptographicEngine.Decrypt(key, cipher, IV);
        }

        public static byte[] GetHash(byte[] data, byte[] salt)
        {
            byte[] saltedData = new byte[data.Length + salt.Length];
            Array.Copy(salt, saltedData, salt.Length);
            Array.Copy(data, 0, saltedData, salt.Length, data.Length);

            var sha = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
            return sha.HashData(saltedData);
        }

        public static string ByteArrayToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        public static byte[] StringToByteArray(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }
    }
}
