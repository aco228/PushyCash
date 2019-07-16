using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.TrafficCosts
{
	public class TrafficPhazeResult
	{
		public int TestingPhaze = 0;
		public int OptimazingPhaze = 0;
	}


	public class TrafficCostsManager
	{
		private static string Path = @"D:\Projects\AkoProjects\dot.net\PushyCash\_storage\traffic_costs\";
		public Dictionary<string, Dictionary<string, Dictionary<string, double>>> Costs = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
		public Dictionary<string, Dictionary<string, Dictionary<string, int>>> Volume = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
		// [vertical][android/ios][country]

		public TrafficCostsManager()
		{
			List<string> verticals = new List<string>() { "adult", "mainstream", "aggressive" };
			List<string> devices = new List<string>() { "Android", "iOS" };

			foreach (string v in verticals)
				foreach (string d in devices)
					this.Load(v, d);
		}

		private void Load(string vertical, string device)
		{
			string filePath = string.Format(@"{0}\{1}.json", Path, string.Format("traffic_cost.{0}.{1}", device.ToLower(), vertical));
			if (!File.Exists(filePath))
				return;
			
			if (!Costs.ContainsKey(vertical)) Costs.Add(vertical, new Dictionary<string, Dictionary<string, double>>());
			if (!Costs[vertical].ContainsKey(device)) Costs[vertical].Add(device, new Dictionary<string, double>());
			if (!Volume.ContainsKey(vertical)) Volume.Add(vertical, new Dictionary<string, Dictionary<string, int>>());
			if (!Volume[vertical].ContainsKey(device)) Volume[vertical].Add(device, new Dictionary<string, int>());

			dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(filePath));
			foreach (dynamic row in json)
			{
				if (!Costs[vertical][device].ContainsKey(row.countryCode.ToString()))
					Costs[vertical][device].Add(row.countryCode.ToString(), Math.Round(Double.Parse(row.avgCost.ToString()), 4));
				if (!Volume[vertical][device].ContainsKey(row.countryCode.ToString()))
					Volume[vertical][device].Add(row.countryCode.ToString(), int.Parse(row.available.ToString()));
			}
		}

		public double? GetPrice(string vertical, string device, string country)
		{
			if (!Costs.ContainsKey(vertical)) return null;
			if (!Costs[vertical].ContainsKey(device)) return null;
			if (!Costs[vertical][device].ContainsKey(country)) return null;
			return Costs[vertical][device][country];
		}

		public TrafficPhazeResult GetTrafficType(string vertical, string device, string country)
		{
			if (!Volume.ContainsKey(vertical)) return null;
			if (!Volume[vertical].ContainsKey(device)) return null;
			if (!Volume[vertical][device].ContainsKey(country)) return null;

			if (Volume[vertical][device][country] > 50000000)
				return new TrafficPhazeResult() { TestingPhaze = 2500, OptimazingPhaze = 3500 };
			else if (Volume[vertical][device][country] < 50000000 && Volume[vertical][device][country] > 1000000)
				return new TrafficPhazeResult() { TestingPhaze = 1500, OptimazingPhaze = 2500 };
			else if (Volume[vertical][device][country] < 1000000)
				return new TrafficPhazeResult() { TestingPhaze = 500, OptimazingPhaze = 1500 };

			return null;
		}

	}
}
