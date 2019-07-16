using Direct.Core;
using Newtonsoft.Json;
using PushyCash.Afflow.Models;
using PushyCash.Afflow.Models.Stats;
using PushyCash.Direct;
using PushyCash.Direct.Models;
using PushyCash.Links;
using PushyCash.TrafficNetworks.Core;
using PushyCash.TrafficNetworks.Zeropark;
using PushyCash.Web.Core;
using PushyCash.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.ConsoleApp.Tests
{

	class Program
	{

		static void Main(string[] args)
		{
			PushyContext.AfflowManager = new Afflow.AfflowManager();

			var initialStats = PushyContext.AfflowManager.GetInitialCampaignData(10729.ToString(), "1258407", "US", "T-MOBILE", "Android");
			var currentStats = PushyContext.AfflowManager.GetCampaignStats(10729.ToString(), "1258407", "US", "Android");
			foreach (var cmno in PushyContext.AfflowManager.GetCampaignMobileOperatorMap(10729, "1258407"))
			{
				int a = 0;
			}


			loadsCount++;
			PushyContext.TrafficNetworkManager = new TrafficNetworkManager();
			PushyContext.TrafficNetworkManager.OnLoadedAction = OnFinish;
			PushyContext.TrafficNetworkManager.Update();
			
			//loadsCount++;
			//PushyContext.SnapshotManager = new Snapshots.SnapshotManager();
			//PushyContext.SnapshotManager.OnLoadFinished = OnFinish;
			//PushyContext.SnapshotManager.Update();
			
			//loadsCount++;
			//PushyContext.LinkManager = new LinkManager();
			//PushyContext.LinkManager.OnLoadFinished = OnFinish;
			//PushyContext.LinkManager.Update();



			Console.ReadKey();
		}

		static int loads = 0, loadsCount = 0;
		static void OnFinish()
		{
			loads++;
			if (loads != loadsCount) return;
			
			var propellerAds = PushyContext.TrafficNetworkManager["propellerads"];
			var usCampaign = propellerAds["US.T-MOBILE.mainstream.Android"];
			//var stats = usCampaign.GetCurrentStats();
			int a = 0;
		}
		

	}
}
