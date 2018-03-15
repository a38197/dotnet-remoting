using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace SuperSoftware.Shared
{
    public class SerializationAdapter<T>
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));

        public void SerializeToFile(string fileName, T data)
        {
            using (TextWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, data);
            }
            
        }

        public T DerializeFromFile(string fileName)
        {
            using (TextReader reader = new StreamReader(fileName))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }


}
