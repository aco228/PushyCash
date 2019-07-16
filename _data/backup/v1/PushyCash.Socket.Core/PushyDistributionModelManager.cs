using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Sockets.Core
{
	public class PushyDistributionModelManager
	{

		public static byte[] GetBytesFromObj(object data)
		{
			var binFormatter = new BinaryFormatter();
			var mStream = new MemoryStream();
			binFormatter.Serialize(mStream, data);
			return mStream.ToArray();
		}

		public static object GetObjFromBytes(byte[] data)
		{
			var mStream = new MemoryStream();
			var binFormatter = new BinaryFormatter();
			mStream.Write(data, 0, data.Length);
			mStream.Position = 0;
			return binFormatter.Deserialize(mStream);
		}
		
		public static PushyDistributionModel ConvertToModel(byte[] data)
		{
			var mStream = new MemoryStream();
			var binFormatter = new BinaryFormatter();
			mStream.Write(data, 0, data.Length);
			mStream.Position = 0;
			return binFormatter.Deserialize(mStream) as PushyDistributionModel;
		}

	}
}
