using Direct.Core;
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
		public TrafficSourceUpdateStatsWorker() : base(primaryKey: "traffic", secondaryKey: "update_stats", timeout: 20 * 1000, startImmediatly:true)
		{
		}

		public override void OnLoop()
		{
			try
			{
				foreach (var n in PushyContext.TrafficNetworkManager.Networks)
				{
					System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + "-----------------------------> " + n.Value.Name);
					var trafficSource = n.Value;
					int trafficSourceClicks = 0, trafficSourceConversions = 0;
					double trafficSourcePayout = 0.0, trafficSourceSpent = 0.0;

					Parallel.ForEach(n.Value.GetCampaignTodayStats(), (stat) =>
					{
						if (!n.Value.CampaignIDMap.ContainsKey(stat.CampaignID))
							return;

						var campaign = trafficSource.CampaignIDMap[stat.CampaignID];
						var afflowStats = PushyContext.AfflowManager.GetCampaignStats(trafficSource.AfflowMap.NID.ToString(), campaign.ID, campaign.Country, campaign.Device);
						int statConversions = (trafficSource.HasConversionsReport ? stat.Conversions : afflowStats.Leads);
						double statPayout = (afflowStats != null ? afflowStats.Revenue : (trafficSource.HasConversionsReport ? stat.Payout : 0.0));

						trafficSourceClicks += stat.Redirects;
						trafficSourceConversions += statConversions;
						trafficSourcePayout += statPayout;
						trafficSourceSpent += stat.Spent;

						if (campaign.CurrentStats != null)
						{
							campaign.CurrentStats.Clicks = campaign.InitialStats.Clicks == 0 ? stat.Redirects : stat.Redirects - campaign.InitialStats.Clicks;
							campaign.CurrentStats.Conversions = campaign.InitialStats.Conversions == 0 ? statConversions : statConversions - campaign.InitialStats.Conversions;
							campaign.CurrentStats.Payout = campaign.InitialStats.Payout == 0 ? statPayout : statPayout - campaign.InitialStats.Payout;
							campaign.CurrentStats.Spent = campaign.InitialStats.Spent == 0 ? stat.Spent : stat.Spent - campaign.InitialStats.Spent;
						}
						else
							campaign.InitialStats = new TrafficCurrentStats()
							{
								Clicks = stat.Redirects,
								Payout = statPayout,
								Spent = stat.Spent,
								Conversions = statConversions
							};

						if (campaign.LastDirectPushyNetworkCampaign == null)
							return;

						bool hasChanges = false;

						int clicks = campaign.LastDirectPushyNetworkCampaign.InitialClicks == 0 ? stat.Redirects : campaign.LastDirectPushyNetworkCampaign.InitialClicks - stat.Redirects;
						int conversions = campaign.LastDirectPushyNetworkCampaign.InitialConversions == 0 ? statConversions : campaign.LastDirectPushyNetworkCampaign.InitialConversions - statConversions;
						double payout = campaign.LastDirectPushyNetworkCampaign.InitialPayout == 0 ? statPayout : campaign.LastDirectPushyNetworkCampaign.InitialPayout - statPayout;
						double costs = campaign.LastDirectPushyNetworkCampaign.InitialCost == 0 ? stat.Spent : campaign.LastDirectPushyNetworkCampaign.InitialCost - stat.Spent;

						if (campaign.LastDirectPushyNetworkCampaign.Clicks != clicks) { campaign.LastDirectPushyNetworkCampaign.Clicks = clicks; hasChanges = true; }
						if (campaign.LastDirectPushyNetworkCampaign.Conversions < conversions) { campaign.LastDirectPushyNetworkCampaign.Conversions = conversions; hasChanges = true; }
						if (campaign.LastDirectPushyNetworkCampaign.Cost < costs) { campaign.LastDirectPushyNetworkCampaign.Cost = costs; hasChanges = true; }
						if (campaign.LastDirectPushyNetworkCampaign.Payout < payout) { campaign.LastDirectPushyNetworkCampaign.Payout = payout; hasChanges = true; }

						if (!hasChanges)
							return;

						campaign.LastDirectPushyNetworkCampaign.ROI = (payout == 0) ? -100.0 : Math.Round((payout - costs) / costs * 100);
						PushyCashDirect.Instance.Update(campaign.LastDirectPushyNetworkCampaign);
					});
					
					trafficSource.Clicks = trafficSourceClicks;
					trafficSource.Conversions = trafficSourceConversions;
					trafficSource.Spent = trafficSourceSpent;
					trafficSource.Payout = trafficSourcePayout;

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
