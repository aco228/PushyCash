using Direct.Core;
using Newtonsoft.Json;
using PushyCash.Afflow.Models;
using PushyCash.Afflow.Models.Stats;
using PushyCash.Direct;
using PushyCash.Direct.Models;
using PushyCash.Direct.Models.TrafficNetworks;
using PushyCash.Links;
using PushyCash.TrafficNetworks.Core;
using PushyCash.TrafficNetworks.PropellerAds;
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

			//DirectContainer dc = PushyCashDirect.Instance.LoadContainer(
			//	@"SELECT c.CampaignID FROM PushyCash.core.TrafficCampaignDayStat AS s
			//		LEFT OUTER JOIN PushyCash.core.TrafficNetworkCampaign AS c ON s.TrafficNetworkCampaignID = c.TrafficNetworkCampaignID
			//		WHERE Clicks > 300 ORDER BY Clicks DESC; ");

			//List<string> avaliableCampaigns = new List<string>();
			//foreach (var row in dc.Rows)
			//	if(!avaliableCampaigns.Contains(row.GetString("CampaignID")))
			//		avaliableCampaigns.Add(row.GetString("CampaignID"));

			//string bad = "";
			//string improved = "";
			//string notReacting = "";


			//foreach(string campID in avaliableCampaigns)
			//{
			//	DirectContainer ddc = PushyCashDirect.Instance.LoadContainer(
			//		string.Format(@"SELECT s.Day, s.ROI, s.Clicks, s.Conversions, s.Payout, s.Costs, s.Revenue, c.FullName, tn.Name
			//			FROM PushyCash.core.TrafficCampaignDayStat AS s
			//			LEFT OUTER JOIN PushyCash.core.TrafficNetworkCampaign AS c ON s.TrafficNetworkCampaignID=c.TrafficNetworkCampaignID
			//			LEFT OUTER JOIN PushyCash.core.TrafficNetwork AS tn ON tn.TrafficNetworkID=c.TrafficNetworkID
			//			WHERE c.CampaignID='{0}' AND s.Costs > 0.1 AND s.Clicks>300
			//			ORDER BY s.Day ASC", campID));

			//	if (!ddc.HasValue && ddc.RowsCount < 2)
			//		continue;

			//	double firstElem = ddc.GetDouble("ROI").Value;
			//	bool alwaysBad = true;
			//	double compareElem = firstElem;
			//	bool isAsceding = true;

			//	string improvementResult = "";

			//	foreach(var row in ddc.Rows)
			//	{
			//		double currentRoi = row.GetDouble("ROI").Value;
			//		improvementResult += (improvementResult != "" ? "," : "") + currentRoi;
			//		if (currentRoi != -100) alwaysBad = false;
			//		if (currentRoi > compareElem) isAsceding = false;
			//		compareElem = currentRoi;
			//	}

			//	string result = campID + " - " + ddc.GetString("FullName") + " # " + improvementResult;


			//	if (alwaysBad) notReacting += result + Environment.NewLine;
			//	else if (isAsceding) bad += result + Environment.NewLine;
			//	else improved += result + Environment.NewLine;

			//}

			//int a = 0;

		}
		

	}
}

