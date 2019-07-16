using Newtonsoft.Json;
using PushyCash;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCashHelper
{

	class MNO
	{
		public string Afflow = "";
		public string Zeropark = "";
	}

	class Program
	{
		//static PushyCashDirect DB = PushyCashDirect.Instance;
		static Dictionary<string, List<MNO>> MAP = new Dictionary<string, List<MNO>>();
		static string Link = "https://win.ningoffer.club/?utm_medium={HASH}&utm_campaign={campaignid}&3={zoneid}&cid=${SUBID}";

		static string GetMnoName(string country, string name)
		{
			if (!MAP.ContainsKey(country))
			{
				int a = 0;
				return string.Empty;
			}
			foreach (var m in MAP[country])
				if (m.Afflow.Equals(name))
					return m.Zeropark;

			return "";
		}

		static void Main(string[] args)
		{
			PushyContext.AfflowManager = new PushyCash.Afflow.AfflowManager();
			string[] lines = File.ReadAllLines(@"D:\Projects\AkoProjects\dot.net\PushyCash\_data\networks\propellerads\mobileoperator.afflow.map.txt");
			foreach (string line in lines)
			{
				string[] split = line.Split('#');
				if (!MAP.ContainsKey(split[0]))
					MAP.Add(split[0], new List<MNO>() { new MNO() { Afflow = split[1], Zeropark = split[2] } });
				else
					MAP[split[0]].Add(new MNO() { Afflow = split[1], Zeropark = split[2] });
			}


			string result = "";
			string nonFoundMobileOperators = "";

			foreach (PushyCash.Afflow.Models.AfflowLink link in PushyContext.AfflowManager.GetLinks())
			{
				if (link.vertical.Equals("adult"))
					continue;
				if (!link.TrafficNetwork.Equals("propellerads"))
					continue;

				bool isWifi = link.AfflowMobileOperator.Contains("WiFi");
				string mmno = isWifi ? "" : GetMnoName(link.Country, link.AfflowMobileOperator);
				if (string.IsNullOrEmpty(mmno))
				{
					int wtf_aa = 0;
					nonFoundMobileOperators += link.Country + ", " + link.AfflowMobileOperator + Environment.NewLine;
				}

				string name = link.FullName.Replace(".propellerads", string.Empty);
				string device = link.Device.Equals("android") ? "\"android\"" : "\"ios\"";
				string destination = Link.Replace("{HASH}", link.hash);
				string connection = isWifi ? "other" : "mobile";
				string mobileIsp =  (string.IsNullOrEmpty(mmno) ? "" : "\"" + mmno + "\"" );

				string json2 = "{" + string.Format("\"name\":\"{0}\",", name)
					+ string.Format("\"country\":\"{0}\",", link.Country)
					+ string.Format("\"device\":[{0}],", device)
					+ string.Format("\"destination\":\"{0}\",", destination)
					+ string.Format("\"connection\":\"{0}\",", connection)
					+ string.Format("\"mobileIsp\":[{0}]", mobileIsp) + "}";


				result += (result != "" ? "," : "" ) + json2;
			}

			result = "[" + result + "]";
			Console.ReadKey();
		}
	}
}
