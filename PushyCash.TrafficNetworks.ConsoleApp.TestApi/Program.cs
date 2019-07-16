using PushyCash.TrafficNetworks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.ConsoleApp.TestApi
{
	class Program
	{
		static string NAME_OF_TRAFFIC_NETWORK = "propellerads";

		static void Main(string[] args)
		{
			var breakpoint = 0;
			PushyContext.TrafficNetworkManager = new TrafficNetworkManager();
			if (!PushyContext.TrafficNetworkManager.Networks.ContainsKey(NAME_OF_TRAFFIC_NETWORK))
				throw new Exception("Name does not exists");

			var network = PushyContext.TrafficNetworkManager[NAME_OF_TRAFFIC_NETWORK];

			TrafficCampaign campaign = null;
			foreach(var c in network.API.GetCampaigns()) { campaign = c; break; }

			if(campaign == null)
				throw new Exception("Not any campaign is loaded");
			
			campaign.MapWithTrafficSource(network);
			string campaignName = campaign.FullName;
			string id = campaign.ID;

			//breakpoint = 0;

			//campaign.Start();
			//breakpoint = 0;

			//campaign.Stop();
			//breakpoint = 0;

			//var todayStats = network.GetTodayStats();
			//breakpoint = 0;

			//var campaignStats = campaign.GetCurrentStats();
			//breakpoint = 0;

			//var compaignReload = campaign.Reload();
			//breakpoint = 0;

			//List<TrafficTargetsInfo> testTargets = new List<TrafficTargetsInfo>() { new TrafficTargetsInfo() { Name = "1" }, new TrafficTargetsInfo() { Name = "2" } };
			//network.API.ResumeTargets(campaign, testTargets);
			//breakpoint = 0;
			//network.API.StopTargets(campaign, testTargets);
			//breakpoint = 0;
		}
	}
}
