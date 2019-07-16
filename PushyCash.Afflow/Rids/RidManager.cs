using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.Rids
{
	public class RidManager
	{
		//public List<Rid> Rids { get; protected set; } = new List<Rid>();
		private Dictionary<string, int> Rids { get; set; } = new Dictionary<string, int>(); // [country_vertical] / int rid

		public RidManager()
		{
			string filePath = @"D:\Projects\AkoProjects\dot.net\PushyCash\_storage\afflow\rids.afflow_data.json";
			if (!File.Exists(filePath))
				return;

			string text = File.ReadAllText(filePath);
			dynamic json = JsonConvert.DeserializeObject(text);
			foreach(dynamic row in json)
			{
				string key = string.Format("{0}_{1}", row.country, row.vertical);
				if (!Rids.ContainsKey(key))
					Rids.Add(key, int.Parse(row.rid.ToString()));
			}
		}

		public int? GetRid(string country, string vertical)
		{
			string key = string.Format("{0}_{1}", country, vertical);
			if (!Rids.ContainsKey(key))
				return null;
			return Rids[key];
		}

	}
}
