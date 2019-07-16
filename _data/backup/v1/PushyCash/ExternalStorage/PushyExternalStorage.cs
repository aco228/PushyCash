using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash
{
	public class PushyExternalStorage
	{		

		public static string StoragePath
		{
			get
			{
				return @"D:\Projects\AkoProjects\dot.net\PushyCash\_storage";
			}
		}

		public static FileInfo GetFileInfo(string name)
		{
			string path = string.Format(@"{0}\{1}.dat", StoragePath, name);
			return new FileInfo(path);
		}

		public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
		{
			using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
			{
				try
				{
					var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
					binaryFormatter.Serialize(stream, objectToWrite);
				}
				catch (Exception e)
				{
					PLogger.Error("Could not write binary file :: " + filePath + " :: because :: " + e.ToString());
				}
			}
		}

		public static T ReadFromBinaryFile<T>(string filePath)
		{
			using (Stream stream = File.Open(filePath, FileMode.Open))
			{
				try
				{
					stream.Seek(0, SeekOrigin.Begin);
					var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
					return (T)binaryFormatter.Deserialize(stream);
				}
				catch(Exception e)
				{
					PLogger.Error("Could not read binary file :: " + filePath + " :: because :: " + e.ToString());
					return default(T);
				}
			}
		}

	}
}
