using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace SuperGrowtopiaMaker
{
	public class DataHandler
	{
		public static void SaveData(object array, string path)
		{
			using (Stream stream = File.Open(path, FileMode.Create))
			{
				new BinaryFormatter().Serialize(stream, array);
			}
		}

        public static void SaveData(object array, Stream ownStream)
        {
            using (Stream stream = ownStream)
            {
                new BinaryFormatter().Serialize(stream, array);
            }
        }

		public static object LoadData(string path)
		{
			object result;
			using (Stream stream = File.Open(path, FileMode.Open))
			{
				result = new BinaryFormatter().Deserialize(stream);
			}
			return result;
        }

        public static object LoadData(Stream ownStream)
        {
            object result;
            using (Stream stream = ownStream)
            {
                result = new BinaryFormatter().Deserialize(stream);
            }
            return result;
        }
	}
}
