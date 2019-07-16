using Direct.Core;
using PushyCash.Afflow.Models;
using PushyCash.Direct;
using PushyCash.TrafficNetworks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.ConsoleApp.StatsCreator
{
	class Program
	{
		public static string DAY = "30";
		public static string MONTH = "6";
		public static string YEAR = "2018";

		static void Main(string[] args)
		{
			PushyContext.AfflowManager = new Afflow.AfflowManager();
			PushyContext.TrafficNetworkManager = new TrafficNetworkManager();
			PushyContext.TrafficNetworkManager.OnLoadedAction = OnFinishLoading;
			PushyContext.TrafficNetworkManager.Update();
		}

		static void OnFinishLoading()
		{
			DateTime startDate = DateTime.Parse(string.Format("{0}-{1}-{2} 00:00:00.000", YEAR, MONTH, DAY));
			DateTime endDate = DateTime.Parse(string.Format("{0}-{1}-{2} 23:59:59.999", YEAR, MONTH, DAY));

			Console.WriteLine("Starting to gather data");

			foreach (var trafficSource in PushyContext.TrafficNetworkManager.NetworksList)
			{
				Console.WriteLine($"Collecting data for {trafficSource.Name}");

				Dictionary<string, AfflowLeagueStats> afflowCampaigns = new Dictionary<string, AfflowLeagueStats>();
				foreach (var afflow_stat in PushyContext.AfflowManager.GetLeagueStats(trafficSource.AfflowMap.NID.ToString(), trafficSource.Timezone.ToString(), startDate, endDate))
					if (!afflowCampaigns.ContainsKey(afflow_stat.Keyword))
						afflowCampaigns.Add(afflow_stat.Keyword, afflow_stat);

				int index = 0, count = afflowCampaigns.Count;

				foreach(var stat in trafficSource.API.GetStats(startDate, endDate))
				{
					index++;
					if (!afflowCampaigns.ContainsKey(stat.CampaignID))
						continue;
					var afflowStat = afflowCampaigns[stat.CampaignID];

					PushyContext.AfflowManager.SetSpending(trafficSource.AfflowMap.NID, stat.CampaignID, startDate, stat.Spent);
					int? trafficCampaignID = PushyCashDirect.Instance.LoadInt(
						$"SELECT TOP 5 TrafficNetworkCampaignID FROM PushyCash.core.TrafficNetworkCampaign WHERE CampaignID='{stat.CampaignID}' AND TrafficNetworkID={trafficSource.DirectModelID} ");

					if (!trafficCampaignID.HasValue)
						continue;

					int? trafficNetworkCampaignStatID = PushyCashDirect.Instance.LoadInt(
						$"SELECT TOP 5 * FROM PushyCash.core.TrafficCampaignDayStat WHERE TrafficNetworkCampaignID={trafficCampaignID.Value} AND Day='{YEAR}-{MONTH}-{DAY} 00:00:00.0000000'");
					
					double roi = afflowStat.Revenue == 0 || stat.Spent == 0 ? -100 : (afflowStat.Revenue - stat.Spent) / stat.Spent * 100;
					double revenue = afflowStat.Revenue - stat.Spent;
					if (trafficNetworkCampaignStatID.HasValue)
						PushyCashDirect.Instance.Execute(
							"UPDATE [].TrafficCampaignDayStat SET Clicks={0}, Conversions={1}, Payout={2}, Costs={3}, Revenue={4}, ROI={5} WHERE TrafficCampaignDayStatID={6}",
							afflowStat.Clicks, afflowStat.Leads, afflowStat.Revenue, stat.Spent, revenue, roi, trafficNetworkCampaignStatID.Value);
					else
						PushyCashDirect.Instance.Execute(
							"INSERT INTO [].TrafficCampaignDayStat (TrafficNetworkCampaignID, TrafficNetworkID, Day, Clicks, Conversions, Payout, Costs, Revenue, ROI)", 
							trafficCampaignID.Value, trafficSource.DirectModelID, $"{startDate.Year}-{startDate.Month}-{startDate.Day} 00:00:00.0000000", 
							afflowStat.Clicks, afflowStat.Leads, afflowStat.Revenue, stat.Spent, revenue, roi);

					Console.WriteLine($"   -> {index}/{count} {stat.CampaignID} ");
					TryOptimization(trafficSource, stat.CampaignID);
					int a = 0;
				}				
			}
		}
		
		static void TryOptimization(TrafficNetworks.Core.TrafficSourceBase trafficSource, string campaignID)
		{
			if (trafficSource.Name.Equals("zeropark"))
				return;

			Dictionary<string, AfflowTrafficTargetReport> targets = new Dictionary<string, AfflowTrafficTargetReport>();
			foreach(var target in  PushyContext.AfflowManager.GetTrafficTargetInfo(trafficSource.AfflowMap.NID, campaignID))
				if (!targets.ContainsKey(target.Keyword))
					targets.Add(target.Keyword, target);

			List<TrafficTargeStatInfo> t = trafficSource.API.GetTargetsInfo(campaignID, DateTime.Today.AddDays(-7), DateTime.Now);
			List<TrafficTargetsInfo> targetsForStopage = new List<TrafficTargetsInfo>();

			foreach(var networkTargetStat in t)
			{
				string reasonForClosing = string.Empty;
				if (networkTargetStat.Clicks == 0 || !targets.ContainsKey(networkTargetStat.Name))
					continue;

				var afflowTargetStat = targets[networkTargetStat.Name];
				if (afflowTargetStat.Conversions == 0 && ((afflowTargetStat.Clicks > 1500 && networkTargetStat.Spent > 0.30) || networkTargetStat.Spent >= 0.80))
					reasonForClosing = "There is no conversions and spendings are more than .8c, or more than 1500c";

				double rev = afflowTargetStat.Revenue - networkTargetStat.Spent;
				if (afflowTargetStat.Conversions > 0 && rev <= -1.5)
					reasonForClosing = "There are conversions but profit is less than 1.5";

				if (string.IsNullOrEmpty(reasonForClosing))
					continue;

				DirectContainer dc = PushyCashDirect.Instance.LoadContainer(
					string.Format(@"SELECT c.TrafficNetworkCampaignID, tnb.TrafficNetworkTargetBlacklistID FROM PushyCash.core.TrafficNetworkCampaign AS c 
						LEFT OUTER JOIN  PushyCash.core.TrafficNetworkTargetBlacklist AS tnb ON tnb.TrafficNetworkCampaignID=c.TrafficNetworkCampaignID
						WHERE (c.CampaignID='{0}' AND tnb.TrafficNetworkTargetBlacklistID IS NULL) OR 
						(c.CampaignID='{0}' AND tnb.Name='{1}')", campaignID, networkTargetStat.Name));

				int trafficNetworkCampaignID = dc.GetInt("TrafficNetworkCampaignID").Value;
				int? trafficNetworkTargetBlacklist = dc.GetInt("TrafficNetworkTargetBlacklistID");
				if (trafficNetworkTargetBlacklist.HasValue)
					continue;

				PushyCashDirect.Instance.Execute("INSERT INTO [].TrafficNetworkTargetBlacklist (TrafficNetworkCampaignID, Name, Reason)", trafficNetworkCampaignID, networkTargetStat.Name, reasonForClosing);
				targetsForStopage.Add(new TrafficTargetsInfo() { Name = networkTargetStat.Name });
			}
		}

	}
}

