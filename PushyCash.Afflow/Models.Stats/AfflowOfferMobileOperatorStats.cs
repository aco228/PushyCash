using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.Models.Stats
{

	/*
		Will be used as metric for PushyNetworkCampaign,
		we will collect this data at start of campaign, and 3 hours after campaign ended, so we can have final data, 
		and also we will have data for offers that converts so we can wait for those offers to start again.
	*/

	[Serializable()]
	public class AfflowOfferMobileOperatorStats
	{
		public string OfferID { get; set; } = string.Empty;
		public int Clicks { get; set; } = 0;
		public int Conversions { get; set; } = 0;
		public double Revenue { get; set; } = 0.0;
	}
}
