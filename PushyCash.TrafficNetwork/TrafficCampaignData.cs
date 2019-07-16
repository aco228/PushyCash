using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Core
{
	[Serializable()]
	public class TrafficCampaignData
	{
		public string Namespace { get; set; } = string.Empty; // will be used for external storage

		public string ID { get; protected set; } = string.Empty;
		public int? DirectModelID { get; set; } = null;
		public string TrafficNetworkName { get; protected set; } = string.Empty;
		public string FullName { get; protected set; } = string.Empty;
		public string Country { get; set; } = string.Empty;
		public string MobileOperator { get; set; } = string.Empty;
		public string Vertical { get; set; } = string.Empty;
		public string Device { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public int Visibility { get; set; } = 0;
		public bool IsCampaignActive { get; set; } = false;

		public TrafficCampaignData(string id, string trafficNetworkName, string fullName, TrafficNameAnalyser analyser)
		{
			this.ID = id;
			this.TrafficNetworkName = trafficNetworkName;
			this.FullName = fullName.Replace("_", string.Empty);
			this.Country = analyser.Country;
			this.MobileOperator = analyser.MobileOperator;
			this.Vertical = analyser.Vertical;
			this.Device = analyser.Device;
		}

	}
}
