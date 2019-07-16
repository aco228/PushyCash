using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushyCash.TrafficNetworks.Core;

namespace PushyCash.TrafficNetworks.Exoclick
{
	public class ExoclickApi : TrafficNetworkApiBase
	{
		public ExoclickApi() 
			: base("header", "token")
		{ }
		
		public override IEnumerable<TrafficCampaign> GetCampaigns()
		{
			yield break;
		}
		

		public override void StartCampaign(TrafficCampaign campaign)
		{
		}

		public override bool StopCampaign(TrafficCampaign campaign)
		{
			return false;
		}

		public override bool Bid(TrafficCampaign campaign)
		{
			throw new NotImplementedException();
		}

		public override TrafficCampaignData ReloadCampaign(TrafficCampaign campaign)
		{
			throw new NotImplementedException();
		}

		public override void StopTargets(TrafficCampaign campaign, List<TrafficTargetsInfo> tagets)
		{
			throw new NotImplementedException();
		}

		public override void ResumeTargets(TrafficCampaign campaign, List<TrafficTargetsInfo> tagets)
		{
			throw new NotImplementedException();
		}

		public override TrafficSourceTodayStats GetTodayStats()
		{
			return null;
		}

		public override TrafficCurrentStats GetCurrentStats(TrafficCampaign campaign)
		{
			throw new NotImplementedException();
		}

		public override bool StopCampaign(string campaignID)
		{
			throw new NotImplementedException();
		}

		public override bool StartCampaign(string campaignID)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<TrafficSourceTodayStats> GetCampaignTodayStats()
		{
			yield break;
		}
	}
}
