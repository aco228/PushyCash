using Direct.Core;
using PushyCash.Afflow.Models;
using PushyCash.Direct;
using PushyCash.TrafficNetworks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	public class TrafficSourceUpdateStatsWorker : WorkerBase
	{
		public TrafficSourceUpdateStatsWorker() : base(primaryKey: "traffic", secondaryKey: "update_stats", timeout: PushyGlobal.InitialConfiguration.SecondsForTrafficSourceUpdateStats_WithNoActiveCampaign * 1000, startImmediatly:true)
		{
		}

		public override void OnLoop()
		{
			try
			{

				if (PushyContext.CampaignManager.ActiveCampaigns.Count == 0)
					this.Timeout = PushyGlobal.InitialConfiguration.SecondsForTrafficSourceUpdateStats_WithNoActiveCampaign * 1000;
				else
					this.Timeout = PushyGlobal.InitialConfiguration.SecondsForTrafficSourceUpdateStats_WithActiveCampaign * 1000;

				foreach (var n in PushyContext.TrafficNetworkManager.Networks)
				{
					System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + "-----------------------------> " + n.Value.Name);
					var trafficSource = n.Value;
					bool thereWereSomeChanges = false;
					bool thereWereNullValues = false;

					int trafficSourceClicks = 0, trafficSourceConversions = 0;
					double trafficSourcePayout = 0.0, trafficSourceSpent = 0.0;

					Parallel.ForEach(n.Value.GetCampaignTodayStats(), (stat) =>
					{
						if(stat == null)
						{
							thereWereNullValues = true;
							return;
						}

						if (!n.Value.CampaignIDMap.ContainsKey(stat.CampaignID))
							return;

						var campaign = trafficSource.CampaignIDMap[stat.CampaignID];
						
						var afflowStats = new AfflowCampaignStat() { };
						if(stat.Clicks > 10)
							afflowStats = PushyContext.AfflowManager.GetCampaignStats(trafficSource.AfflowMap.NID.ToString(), campaign.ID, campaign.Country, campaign.Device);

            if (afflowStats == null)
              return;

						int statConversions = (trafficSource.HasConversionsReport ? stat.Conversions : afflowStats.Leads);
						if (stat.Conversions == 0 && afflowStats.Leads > 0)
							statConversions = afflowStats.Leads;
						if (statConversions > 0 && afflowStats.Leads == 0)
							statConversions = stat.Conversions;

						double statPayout = (afflowStats != null ? afflowStats.Revenue : (trafficSource.HasConversionsReport ? stat.Payout : 0.0));

						if (stat.Clicks == 0 && statConversions == 0 && statPayout == 0.0 && stat.Spent == 0.0) // nothing to write on
							return;

						thereWereSomeChanges = true;
						campaign.Stats.BeginTransactionUpdate();
						campaign.Stats.Clicks = stat.Clicks;
						int campaignTempConversions = campaign.Stats.Conversions;
            if(statConversions > 0)
            {
              int a = 0;
            }
						campaign.Stats.Conversions = statConversions;
						campaign.Stats.Payout = statPayout;
						campaign.Stats.Costs = stat.Spent;						
						campaign.Stats.CommitTransactionUpdate();

						// auto starting and checking if everything is ok
						if (statConversions > campaignTempConversions)
							campaign.OnConversion();
						campaign.CheckAutoRunner();

						trafficSourceClicks += stat.Clicks;
						trafficSourceConversions += statConversions;
						trafficSourcePayout += statPayout;
						trafficSourceSpent += stat.Spent;
					});
					
					if(thereWereSomeChanges && !thereWereNullValues)
					{
						// ignorisi vrijednosti koje su manje, jer wtf

						trafficSource.Clicks = trafficSourceClicks;
						trafficSource.Conversions = trafficSourceConversions;
						trafficSource.Spent = trafficSourceSpent;
						trafficSource.Payout = trafficSourcePayout;

						//if (trafficSourceClicks >= trafficSource.Clicks)
						//	trafficSource.Clicks = trafficSourceClicks;

						//if(trafficSourceConversions >= trafficSource.Conversions)
						//	trafficSource.Conversions = trafficSourceConversions;

						//if(trafficSourceSpent >= trafficSource.Spent)
						//	trafficSource.Spent = trafficSourceSpent;

						//if(trafficSourcePayout >= trafficSource.Payout)
						//	trafficSource.Payout = trafficSourcePayout;
					}
				}
				System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + "-----------------------------> " + "Done");
			}
			catch (Exception e)
			{
				PLogger.Error("Fatal on traffic stats update:: " + e.ToString());
			}
		}
	}
}
