using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.Models.Campaigns
{
	/*
		This model should provide us informations about where traffic is going
		(is it going to offer we specified, or afflow is sending it on some random offers)
		If that is the case, we should close that PushyNetwork campaing
		https://api.monetizer.co/data/report.php?nid=6647&rcid=Global&order=clicks&direction=desc&keyword=carrier_offer&start_ts=1530828000&end_ts=1530914399&tz=Europe/Berlin
	*/

	public class AfflowCampaignMobileOperatorOfferMap
	{
		public string Keyword { get; set; } = string.Empty;
		public string Country { get; set; } = string.Empty;
		public string MobileOperator { get; set; } = string.Empty;
		public string Offer { get; set; } = string.Empty;
		public int Clicks { get; set; } = 0;

		public void Prepare()
		{
			string[] split = this.Keyword.Split(',');
			this.Offer = split[1].Trim();

			split = split[0].Split(' ');
			this.Country = split[0].Trim();
			this.MobileOperator = split[1].Trim();
		}


	}
}
