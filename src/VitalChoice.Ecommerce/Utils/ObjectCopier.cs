//using System;
//using System.IO;
//using System.Runtime.Serialization;
//#if NET451
//using System.Runtime.Serialization.Formatters.Binary;
//#endif

//namespace VitalChoice.Infrastructure.Utils
//{
//	public static class ObjectCopier
//	{
//		public static T Clone<T>(T source)
//		{
//#if NET451
//			if (!typeof(T).IsSerializable)
//			{
//				throw new ArgumentException("The type must be serializable.", nameof(source));
//			}

//			if (ReferenceEquals(source, null))
//			{
//				return default(T);
//			}

//			IFormatter formatter = new BinaryFormatter();
//			Stream stream = new MemoryStream();
//			using (stream)
//			{
//				formatter.Serialize(stream, source);
//				stream.Seek(0, SeekOrigin.Begin);
//				return (T)formatter.Deserialize(stream);
//			}
//#else
//			return default(T);
//#endif
//		}
//	}
//}
