using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Authorize.Net.Util
{
#pragma warning disable 1591
    public static class XmlUtility
    {
        public static string GetXml<T>(T entity) //where T: object //MarshalByRefObject //Serializable 
        {
            string xmlString;

            var requestType = typeof (T);
            var serializer = new XmlSerializer(requestType);
            using (var writer = new Utf8StringWriter())
            {
                serializer.Serialize(writer, entity);
                xmlString = writer.ToString();
            }

            return xmlString;
        }

        public static T Create<T>(string xml) //where T: object //MarshalByRefObject
        {
            var entity = default(T);
            //make sure we have not null and not-empty string to de-serialize
            if (null != xml && 0 != xml.Trim().Length)
            {
                var responseType = typeof (T);
                object deSerializedobject;
                var serializer = new XmlSerializer(responseType);
                using (var reader = new StringReader(xml))
                {
                    deSerializedobject = serializer.Deserialize(reader);
                }

                if (deSerializedobject is T)
                {
                    entity = (T) deSerializedobject;
                }
            }

            return entity;
        }
    }

    public sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
#pragma warning disable 1591
}