using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models.TrafficNetworks
{
	public class DirectTrafficNetwork : PushyDirectModelBase
	{
		public DirectTrafficNetwork() : base("TrafficNetwork") { }

		public int TrafficNetworkID { get; set; } = -1;
		public int NID { get; set; } = -1;
		public string Name { get; set; } = string.Empty;
		public double DailyBudget { get; set; } = 20.0;

		public static DirectTrafficNetwork Load(string name, int nid)
		{
			DirectTrafficNetwork result = PushyCashDirect.Instance.LoadContainer("SELECT * FROM PushyCash.core.TrafficNetwork WHERE Name={0}", name).Convert<DirectTrafficNetwork>();
			if (result != null)
				return result;

			result.TrafficNetworkID = PushyCashDirect.Instance.Execute("INSERT INTO [].TrafficNetwork (Nid, Name)", nid, name).Value;
			result.Name = name;
			result.NID = nid;
			return result;
		}

	}
}
