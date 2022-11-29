using CCMLibrary.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    internal class PackageHandler
    {
        private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public static string GetStringFromBytes(ref byte[] buffer)
        {
            /*
           string value = "";
           value += Encoding.UTF8.GetString(buffer);
           while(value.Length < buffer.Length)
           {
               int len = value.Length;
               value += Encoding.UTF8.GetString(buffer, len+1, buffer.Length);
           }
           return value;*/
            /*
            int index_null = Array.IndexOf(buffer, 0);
            if (index_null == -1 || index_null == buffer.Length-1)
            {
                return Encoding.Default.GetString(buffer);
            }*/


            
            char[] chars = Encoding.Default.GetChars(buffer);
            int index_null = Array.IndexOf(chars, '\0');
            if (index_null == -1 || index_null == chars.Length - 1)
            {
                return Encoding.Default.GetString(buffer);
            }
            string value = "";
            foreach (char c in chars)
            {
                if (c == '\0')
                {
                    continue;
                }
                value += c;
            }
            return value;
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

        public static (Enum, object) DeserializeObject(byte[] byteArr)
        {
            string json = Encoding.Default.GetString(byteArr);
            //json = json.Trim(new char[] { '\uFEFF', '\u200B' });
            Package? package;
            try
            {
                package = JsonConvert.DeserializeObject<Package>(json, _serializerSettings);
                JsonSerializer serializer = new JsonSerializer();
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
            //string json = Encoding.UTF8.GetString(byteArr);
            //json = json.Trim(new char[] { '\uFEFF', '\u200B' });
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
