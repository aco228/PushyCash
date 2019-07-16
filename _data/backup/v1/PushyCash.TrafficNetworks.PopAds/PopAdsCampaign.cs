using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushyCash.TrafficNetworks.Core;

namespace PushyCash.TrafficNetworks.PopAds
{
	[Serializable()]
	public class PopAdsCampaign : TrafficCampaign
	{
		public PopAdsCampaign(TrafficCampaignData data) : base(data) { }
	}
}
