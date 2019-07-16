using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Core
{
	public class TrafficCampaignStartMetrics
	{
		public bool ShouldCampaignRun { get; protected set; } = true;
		public string Explanation { get; protected set; } = string.Empty;
	}
}
