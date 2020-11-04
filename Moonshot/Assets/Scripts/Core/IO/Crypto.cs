using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace Core.IO {
    public static class Crypto {

        public static string Encrypt(byte[] data, byte[] des64Key, byte[] iv) {
            if (data == null || des64Key == null || iv == null) {
                return null;
            }

            SymmetricAlgorithm algo = DES.Create();
            ICryptoTransform transform = algo.CreateEncryptor(des64Key, iv);
            byte[] output = transform.TransformFinalBlock(data, 0, data.Length);

            return Convert.ToBase64String(output);
        }

        public static T Decrypt<T>(string base64Data, byte[] des64Key, byte[] iv) where T : class, new() {
            if (string.IsNullOrEmpty(base64Data) || des64Key == null || iv == null) {
                return null;
            }
            
            SymmetricAlgorithm algo = DES.Create();
            ICryptoTransform transform = algo.CreateDecryptor(des64Key, iv);
            byte[] input = Convert.FromBase64String(base64Data);
            byte[] output = transform.TransformFinalBlock(input, 0, input.Length);

            using (MemoryStream m = new MemoryStream(output)) {
                BinaryFormatter b = new BinaryFormatter();
                return b.Deserialize(m) as T;
            }
        }
    }
}
