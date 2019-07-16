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
		public int CorrectedClicks { get; set; } = 0; // klikovi koji se dobijaju kada se od trenutnik klikova oduzmu prethodni (koristi se za temporary targets stop)
		public int Conversions { get; set; } = 0;
		public double BotRisk { get; set; } = 0;
		public string Reason { get; set; } = string.Empty;

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
