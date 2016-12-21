using System.IO;
using System.Runtime.Serialization;

namespace VitalChoice.Infrastructure.RabbitMQ.Base
{
    public static class ObjectSerializer
    {
#if !NETSTANDARD1_5
        public static object DeserializeFrom(Stream stream)
        {
            NetDataContractSerializer serializer = new NetDataContractSerializer();
            return serializer.Deserialize(stream);
        }

        public static void SerializeTo(Stream stream, object obj)
        {
            NetDataContractSerializer serializer = new NetDataContractSerializer();
            serializer.Serialize(stream, obj);
        }
#else
        public static object DeserializeFrom(Stream stream)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (var reader = new StreamReader(stream))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return serializer.Deserialize(jsonReader);
                }
            }
        }

        public static void SerializeTo(Stream stream, object obj)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (var writer = new StreamWriter(stream))
            {
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    serializer.Serialize(jsonWriter, obj);
                }
            }
        }
#endif

#if !NETSTANDARD1_5
        public static object Deserialize(byte[] data)
        {
            NetDataContractSerializer serializer = new NetDataContractSerializer();
            using (var memory = new MemoryStream(data))
            {
                return serializer.Deserialize(memory);
            }
        }

        public static byte[] Serialize(object obj)
        {
            NetDataContractSerializer serializer = new NetDataContractSerializer();
            using (var memory = new MemoryStream())
            {
                serializer.Serialize(memory, obj);
                return memory.ToArray();
            }
        }
#else
        public static object Deserialize(byte[] data)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (var memory = new MemoryStream(data))
            {
                using (var reader = new StreamReader(memory))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        return serializer.Deserialize(jsonReader);
                    }
                }
            }
        }

        public static byte[] Serialize(object obj)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (var memory = new MemoryStream())
            {
                using (var writer = new StreamWriter(memory))
                {
                    using (var jsonWriter = new JsonTextWriter(writer))
                    {
                        serializer.Serialize(jsonWriter, obj);
                    }
                }
                return memory.ToArray();
            }
        }
#endif
    }
}