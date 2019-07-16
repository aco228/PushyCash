using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushyCash.TrafficNetworks.Core;

namespace PushyCash.TrafficNetworks.Zeropark
{
	public class ZeroparkManager : Core.TrafficSourceBase
	{
		public ZeroparkManager() 
			: base("zeropark", new ZeroparkAfflowMap(), new ZeroparkApi())
		{ }

		public override int OptimizationTemporaryLimit => 15;
		public override int OptimizationStopageLimit => 100;

		public override TrafficCampaignData ConvertJson(dynamic json)
		{
			dynamic details = json.details;
			string name = details.name.ToString();
			if (name[0] != '_')
				return null;

			TrafficNameAnalyser analyser = TrafficNameAnalyser.Anylise(name);
			TrafficCampaignData data = new TrafficCampaignData(details.id.ToString(), "zeropark", name, analyser);
			data.Status = details.state.state.ToString();
			data.Namespace = typeof(ZeroparkCampaign).AssemblyQualifiedName.Replace(", Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", string.Empty);
			data.IsCampaignActive = details.state.state.ToString().ToLower().Equals("active");

			switch (json.visibility.finalVisibility.ToString().ToLower())
			{
				case "processing": data.Visibility = 0; break; 
				case "very_high": data.Visibility = 10; break;
				case "high": data.Visibility = 8; break;
				case "medium": data.Visibility = 5; break;
				case "low": data.Visibility = 3; break;
				case "very_low": data.Visibility = 1; break;
			}
			
			return data;
		}
	}
}
