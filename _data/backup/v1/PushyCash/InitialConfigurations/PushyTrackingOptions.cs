using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.InitialConfigurations.TrackingOptions
{
	public class PushyTrackingOption { public int Minutes = 0, MaximumConversions = 0, MinimumConversions = 0; public double Multiplier = 0.0;  }

	public class PushyTrackingOptions
	{
		public List<PushyTrackingOption> Options { get; protected set; } = new List<PushyTrackingOption>();

		public PushyTrackingOptions()
		{
			dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(@"D:\Projects\AkoProjects\dot.net\PushyCash\_storage\tracking_options.json"));
			if (json == null)
				throw new Exception("WTF? Json is not converted");

			foreach(dynamic row in json)
				Options.Add(new PushyTrackingOption() { Minutes = row.Minutes, MaximumConversions = row.Max, MinimumConversions = row.Min, Multiplier = row.Multiplier });

			int a = 0;
		}


	}
}
