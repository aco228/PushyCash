using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Core
{
	public class TrafficTargetsInfo
	{
		public string Name { get; set; } = string.Empty;
		public int Clicks { get; set; } = 0;
		public int Conversions { get; set; } = 0;
		public double BotRisk { get; set; } = 0;

		public static List<string> LoadCache(TrafficCampaign campaign)
		{
			string location = @"D:\Projects\AkoProjects\dot.net\PushyCash\_storage\traffic_networks\optimization_cache\" + campaign.TrafficNetworkName + @"\" + campaign.ID + ".txt";
			if (!File.Exists(location))
				return new List<string>();

			return File.ReadAllLines(location).ToList();
		}

		public static void SaveCache(TrafficCampaign campaign)
		{
			string location = @"D:\Projects\AkoProjects\dot.net\PushyCash\_storage\traffic_networks\optimization_cache\" + campaign.TrafficNetworkName + @"\" + campaign.ID + ".txt";
			File.WriteAllLines(location, campaign.AlreadyStopedTargets);
		}

		

	}
}
