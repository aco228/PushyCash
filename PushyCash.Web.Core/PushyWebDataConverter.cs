using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Web.Core
{
	public class PushyWebDataConverter
	{

		public static byte[] ObjToBytes(object obj)
		{
			BinaryFormatter bf = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}
		
		public static T BytesToObj<T>(byte[] arrBytes)
		{
			try
			{
				using (var memStream = new MemoryStream())
				{
					var binForm = new BinaryFormatter();
					memStream.Write(arrBytes, 0, arrBytes.Length);
					memStream.Seek(0, SeekOrigin.Begin);
					return (T)binForm.Deserialize(memStream);
				}
			}
			catch(Exception e)
			{
				return default(T);
			}
		}
		
	}
}
