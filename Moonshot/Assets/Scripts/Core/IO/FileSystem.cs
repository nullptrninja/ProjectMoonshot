using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Core.Utility;
using UnityEngine;

namespace Core.IO {
    /// <summary>
    /// Facilitates functions to allow easy read/write access to files stored in the device's persistent data store.
    /// Part of the methods in this namespace wraps the already existing XmlResourceLoader() which will automatically
    /// copy over requested data files to persistent storage.
    /// </summary>
    public static class FileSystem {
        public static T LoadTextResource<T>(string subPathToFile) where T : class, new() {
            return XmlResourceLoader.LoadFile<T>(subPathToFile);
        }

        public static T LoadResource<T>(string subPathToFile) where T : class, new() {
            return XmlResourceLoader.Deserialize<T>(subPathToFile);
        }

        public static void SaveResource<T>(T obj, string subPathToFile) where T : class, new() {
            XmlResourceLoader.Serialize<T>(obj, subPathToFile);
        }

        public static void SaveEncrypted<T>(T obj, string subPathToFile, byte[] des64Key) where T : class, new() {
            if (obj == null) {
                throw new ArgumentNullException("Object cannot be null");
            }

            if (!FileSystem.IsSerializable(typeof(T))) {
                throw new ArgumentException("Type is not serializable");
            }

            if (string.IsNullOrEmpty(subPathToFile)) {
                throw new ArgumentNullException("File path cannot be null or empty");
            }

            if (des64Key == null || des64Key.Length != 8) {
                throw new ArgumentException("Key was null or invalid length");
            }

            // Convert object to encryptable bytes
            byte[] data = FileSystem.SerializeToBytes(obj);
            if (data == null) {
                throw new InvalidOperationException("Could not convert the object into byte stream");
            }

            // Encrypt the byte stream to a base64 string
            string outData = Crypto.Encrypt(data, des64Key, des64Key);

            // Write encrypted string to disk
            string filePath = Path.Combine(Application.persistentDataPath, subPathToFile);            
            StreamWriter stream = File.CreateText(filePath);
            stream.Write(outData);
            stream.Flush();
            stream.Close();
        }

        public static T LoadEncrypted<T>(string subPathToFile, byte[] des64Key) where T : class, new() {
            if (!FileSystem.IsSerializable(typeof(T))) {
                throw new ArgumentException("Type is not serializable");
            }

            string filePath = Path.Combine(Application.persistentDataPath, subPathToFile);
            using (StreamReader stream = new StreamReader(filePath)) {
                string encryptedData = stream.ReadToEnd();
                return Crypto.Decrypt<T>(encryptedData, des64Key, des64Key);
            }
        }

        private static bool IsSerializable(Type t) {
            return t.IsSerializable; // && typeof(ISerializable).IsAssignableFrom(t);
        }

        private static byte[] SerializeToBytes(object o) {
            if (o == null) {
                return null;
            }

            BinaryFormatter b = new BinaryFormatter();
            using (MemoryStream m = new MemoryStream()) {
                b.Serialize(m, o);
                return m.ToArray();
            }
        }
    }
}
