using System;
using System.IO;
using System.Xml.Serialization;

namespace CookingAI
{
    public class MyStorage
    {
        public static void storeXML<T>(T recipes, string file)
        {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                FileStream stream;
                stream = new FileStream(file, FileMode.Create);
                serializer.Serialize(stream, recipes);
                stream.Close();
        }

        public static T readXML<T>(string file)
        {
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    XmlSerializer xmlSer = new XmlSerializer(typeof(T));
                    return (T)xmlSer.Deserialize(sr);
                }

            }
            catch
            {
                return default(T);
            }
        }
    }
}