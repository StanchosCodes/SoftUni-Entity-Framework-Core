using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.Utilities
{
    public class XmlHelper
    {
        public T Deserialize<T>(string inputXml, string rootName)
        {
            // 1 - Initializing a root element for the serializer
            // 2 - Initializing a serializer by giving him the given type of the instance and the root element
            // 3 - Initializing a string or stream reader to read the input xml so we can give it to the deserialize method
            // 4 - Calling the Deserialize method be giving him the already made stream with the input xml
            // 4 - Casting the result of the deserialize method to the given dto type because otherwise it returns object

            XmlRootAttribute root = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), root);

            StringReader reader = new StringReader(inputXml);

            T deserializedDtos = (T)xmlSerializer.Deserialize(reader);

            return deserializedDtos;
        }

        public string Serialize<T>(T obj, string rootName)
        {
            StringBuilder result = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringWriter writer = new StringWriter(result); // same as stringReader, writes in the given stringBuilder and is a stream (the using is because we want after we stop using the writer to automaticaly close it to not waste memory)

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty); // to remove the unwanted namespaces from the created document (xmlns:xsi="www.w3.org/2001/XMLSchema-instance" xmlns:xsd="www.w3.org/2001/XMLSchema")

            xmlSerializer.Serialize(writer, obj, namespaces); // Takes stream and the object we want to serialize

            return result.ToString().TrimEnd();
        }
    }
}
