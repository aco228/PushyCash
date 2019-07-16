using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushyCash.TrafficNetworks.Core;

namespace PushyCash.TrafficNetworks.PropellerAds
{
	public class PropellerAdsManager : TrafficSourceBase
	{
		public PropellerAdsManager() : 
			base("propellerads", new PropellerAdsAfflowMap(), new PropellerAdsApi())
		{ }

		public override bool SupportsAutoBid => false;
		public override bool HasPayoutReport => false;

		public override int OptimizationTemporaryLimit => 50;
		public override int OptimizationStopageLimit => 300;
		public override TrafficSourceTimeZone Timezone => TrafficSourceTimeZone.London;

		public override TrafficCampaignData ConvertJson(dynamic json)
		{
			string name = json.name;
			if (name[0] != '_')
				return null;

			if (json.is_archived.ToString().Equals("1")) // deleted campaign
				return null;
			
			TrafficNameAnalyser analyser = TrafficNameAnalyser.Anylise(name);
			TrafficCampaignData data = new TrafficCampaignData(json.id.ToString(), "propellerads", name, analyser);
			data.Status = this.GetStatusByJson(json.status.ToString());
			data.Namespace = typeof(PropellerAdsCampaign).AssemblyQualifiedName.Replace(", Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", string.Empty);
			data.IsCampaignActive = data.Status.Equals("Working");
			return data;
		}

		// SUMMARY: Helper method for getting right status name from API json
		private string GetStatusByJson(string input)
		{
			switch(input)
			{
				case "1": return "Draft";
				case "2": return "Moderation pending";
				case "3": return "Rejected";
				case "4": return "Ready";
				case "5": return "Test run";
				case "6": return "Working";
				case "7": return "Paused";
				case "8": return "Stopped";
				case "9": return "Completed";
				default: return "";

			}
		}
		
		protected override TrafficSourceTodayStats GetTodayStatsImplementation()
		{
			if(this.Campaigns == null || this.Campaigns.Count == 0)
				return base.GetTodayStatsImplementation();
			return (this.API as PropellerAdsApi).GetTodayStatsFiltered(this.CampaignsList);	
		}

	}
}
