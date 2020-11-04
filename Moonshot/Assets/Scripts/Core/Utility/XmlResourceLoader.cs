using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Core.Utility {
    public static class XmlResourceLoader {
        public static T LoadFile<T>(string resourceSubPath) where T : class {            
            string persistentPath = Path.Combine(Application.persistentDataPath, resourceSubPath);
            TextAsset textAss = Resources.Load<TextAsset>(resourceSubPath);

            if (textAss == null) {
                throw new InvalidOperationException("Unable to load text asset: " + resourceSubPath);
            }

            if (!File.Exists(persistentPath)) {
                DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(persistentPath));
                if (!dir.Exists) {
                    dir.Create();
                }

                using (StreamWriter stream = new StreamWriter(persistentPath)) {
                    stream.Write(textAss.text);
                }
            }

            return Deserialize<T>(persistentPath);
        }

        /// <summary>
        /// Generic XML serializer; serializes any XML serializable type to disk
        /// </summary>
        /// <typeparam name="T">Any serializable class type</typeparam>
        /// <param name="obj">The XML serializable object to serialize</param>
        /// <param name="output">The path to save the file to</param>
        /// <returns>True if serialization succeded</returns>
        public static void Serialize<T>(T obj, string output) where T : class {
            using (TextWriter stream = new StreamWriter(output)) {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                ser.Serialize(stream, obj);
            }
        }

        /// <summary>
        /// Generic XML deserializer; deserializes any object from disk into an XML serializable type
        /// </summary>
        /// <typeparam name="T">Any serializable class type to convert to</typeparam>
        /// <param name="input">File path to read</param>
        /// <returns>Serialized object</returns>
        public static T Deserialize<T>(string input) where T : class {
            using (TextReader stream = new StreamReader(input)) {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                return ser.Deserialize(stream) as T;
            }
        }
    }
}
