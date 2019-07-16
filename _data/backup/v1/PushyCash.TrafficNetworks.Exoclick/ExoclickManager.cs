using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushyCash.TrafficNetworks.Core;

namespace PushyCash.TrafficNetworks.Exoclick
{
	public class ExoclickManager : Core.TrafficSourceBase
	{
		public ExoclickManager() 
			: base("exoclick", new ExoclickAfflowMap(), new ExoclickApi())
		{ }

		public override TrafficCampaignData ConvertJson(dynamic json)
		{
			throw new NotImplementedException();
		}
	}
}
