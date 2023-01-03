using CCMLibrary.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class handle with serialization and deserialization of message passing via TCP
    /// </summary>
    internal class PackageHandler
    {
        private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public static string GetStringFromBytes(ref byte[] buffer)
        {
            char[] chars = Encoding.Default.GetChars(buffer);
            int index_null = Array.IndexOf(chars, '\0');
            if (index_null == -1 || index_null == chars.Length - 1)
            {
                return Encoding.Default.GetString(buffer);
            }
            StringBuilder stringBuilder = new StringBuilder();
            string value = "";
            foreach (char c in chars)
            {
                if (c == '\0')
                {
                    continue;
                }
                stringBuilder.Append(c);
            }
            return stringBuilder.ToString();
        }

        public static byte[] SerializeObject(Enum message, object? classObject)
        {
            if (classObject == null)
            {
                classObject = 0;
            }

            Package package = new Package
            {
                MessageType = message.GetType(),
                ClassType = classObject.GetType(),
                MessageBytes = message,
                ClassBytes = classObject,

            };
            string jsonString = JsonConvert.SerializeObject(package, _serializerSettings);

            byte[] content = Encoding.Default.GetBytes(jsonString);
            byte[] size = BitConverter.GetBytes((Int32)content.Length);
            byte[] combined = size.Concat(content).ToArray();

            return combined;
        }

        public static (Enum, object) DeserializeObject(ref MemoryStream memoryStream)
        {
            Package? package=null;
            memoryStream.Position = 0;
            using (StreamReader sr = new StreamReader(memoryStream, Encoding.Default))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                try
                {
                    JsonSerializer serializer = new JsonSerializer();
                    package = serializer.Deserialize<Package>(reader);
                }
                catch (Exception)
                {
                    memoryStream.Position = 0;

                    StringBuilder sb = new StringBuilder();
                    Int32 nc;
                    Char c;
                    using (StreamReader rdr = new StreamReader(memoryStream, Encoding.Default))
                    {
                        while ((nc = rdr.Read()) != -1)
                        {
                            c = (Char)nc;
                            if (c != '\0') sb.Append(c);
                        }
                    }
                    string json = sb.ToString();
                    package = JsonConvert.DeserializeObject<Package>(json);
                }
            }
            
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.TypeNameHandling = TypeNameHandling.All;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                object p = serializer.Deserialize(new JTokenReader(package.ClassBytes), package.ClassType);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(Enum, object? p)' doesn't match target type '(Enum, object)'.
                return (Enum.ToObject(package.MessageType, package.MessageBytes), p);
#pragma warning restore CS8619 // Nullability of reference types in value of type '(Enum, object? p)' doesn't match target type '(Enum, object)'.
            }
            catch (Exception ex)
            {
                throw new ClusterSystemException(ex.Message);
            }
        }

        public static (Enum, object) DeserializeObject(string json)
        {
            Package? package;
            try
            {
                package = JsonConvert.DeserializeObject<Package>(json);
                JsonSerializer serializer = new JsonSerializer();
                serializer.TypeNameHandling= TypeNameHandling.All;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                object p = serializer.Deserialize(new JTokenReader(package.ClassBytes), package.ClassType);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(Enum, object? p)' doesn't match target type '(Enum, object)'.
                return (Enum.ToObject(package.MessageType, package.MessageBytes), p);
#pragma warning restore CS8619 // Nullability of reference types in value of type '(Enum, object? p)' doesn't match target type '(Enum, object)'.
            }
            catch (Exception ex)
            {
                throw new ClusterSystemException(ex.Message);
            }
        }
    }
}
