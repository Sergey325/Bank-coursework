using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

namespace Курсовая
{
    static class DataSerialiser
    {
        public static void BinarySerialise(object data, string file_path)
        {
            using (FileStream file = new FileStream(file_path, FileMode.OpenOrCreate))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, data);
            }
        }
        public static object BinaryDeserialise(string file_path)
        {
            using (FileStream file = new FileStream(file_path, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(file);
            }

        }

        /*Поля, которые определяются в конструкторе, будут перезаписаны, т.к. при десериализации
         * экзмемпляры создаются заново. Также нельзя сериализировать класс, в котором отсутствует
         * беспараметрический конструктор
        */
        public static void XMLSerialise(Type data_type, object data, string file_path)
        {
            using (TextWriter writer = new StreamWriter(file_path))
            {
                XmlSerializer xmlSerialise = new XmlSerializer(data_type);
                xmlSerialise.Serialize(writer, data);
            }
        }
        public static object XMLDeserialise(Type data_type, string file_path)
        {
            using (TextReader reader = new StreamReader(file_path))
            {
                XmlSerializer xmlSerialise = new XmlSerializer(data_type);
                return xmlSerialise.Deserialize(reader);
            }
        }

        public static void JsonSerialise(Type data_type, object data, string file_path)
        {
            if (File.Exists(file_path)) File.Delete(file_path);
            using (FileStream fs = new FileStream(file_path, FileMode.OpenOrCreate))
            {
                var jsonF = new DataContractJsonSerializer(data_type);
                jsonF.WriteObject(fs, data);
            }
        }
        public static object JsonDeserialise(Type data_type, string file_path)
        {
            using (FileStream fs = new FileStream(file_path, FileMode.OpenOrCreate))
            {
                var jsonF = new DataContractJsonSerializer(data_type);
                return jsonF.ReadObject(fs);
            }
        }
    }
}
