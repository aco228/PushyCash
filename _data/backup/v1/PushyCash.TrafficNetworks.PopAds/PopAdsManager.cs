using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushyCash.TrafficNetworks.Core;

namespace PushyCash.TrafficNetworks.PopAds
{
	public class PopAdsManager : Core.TrafficSourceBase
	{
		public PopAdsManager() 
			: base("popads", new PopAdsAfflowMap(), new PopAdsApi())
		{
		}

		public override TrafficCampaignData ConvertJson(dynamic json)
		{
			throw new NotImplementedException();
		}
	}
}
