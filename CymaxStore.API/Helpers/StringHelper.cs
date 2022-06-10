using System.Xml.Serialization;

namespace CymaxStore.API.Helpers
{
    public class StringHelper
    {
        public static T FromXml<T>(string content)
        {
            try
            {
                using (var stringReader = new StringReader(content))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    var obj = serializer.Deserialize(stringReader);
                    return (T)obj;
                }
            }
            catch (Exception)
            {
            }

            return default(T);
        }
    }
}
