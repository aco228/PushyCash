using PushyCash.TrafficNetworks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.PropellerAds
{
	[Serializable()]
	class PropellerAdsCampaign : TrafficCampaign
	{
		public PropellerAdsCampaign(TrafficCampaignData data) : base(data) { }
	}
}
