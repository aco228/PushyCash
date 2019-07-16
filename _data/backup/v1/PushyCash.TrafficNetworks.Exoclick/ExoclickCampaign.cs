using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushyCash.TrafficNetworks.Core;

namespace PushyCash.TrafficNetworks.Exoclick
{
	[Serializable()]
	public class ExoclickCampaign : TrafficCampaign
	{
		public ExoclickCampaign(TrafficCampaignData data) : base(data) { }
	}
}
