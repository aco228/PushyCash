using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Core
{
	public class TrafficSourceWorkerThread
	{
		public TrafficSourceBase TrafficSource { get; protected set; } = null;

		public TrafficSourceWorkerThread(TrafficSourceBase trafficSource)
		{
			this.TrafficSource = trafficSource;
		}

		public void LoadInitialStats()
		{
			foreach (var stat in this.TrafficSource.GetCampaignTodayStats())
			{
				if (!this.TrafficSource.Campaigns.ContainsKey(stat.CampaignID))
					continue;

				this.TrafficSource[stat.CampaignID].InitialStats = new TrafficCurrentStats()
				{
					Clicks = stat.Redirects,
					Payout = stat.Payout,
					Conversions = stat.Conversions
				};
			}
		}

		public void Call()
		{
			
		}

	}
}
