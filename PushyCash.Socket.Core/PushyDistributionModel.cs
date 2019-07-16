using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Sockets.Core
{
	[Serializable()]
	public class PushyDistributionModel
	{

		public string Type { get; set; } = "";
		public byte[] Data { get; set; } = null;

		public PushyDistributionModel(string type, object data)
		{
			Type = type;
			Data = PushyDistributionModelManager.GetBytesFromObj(data);
		}

	}
}
