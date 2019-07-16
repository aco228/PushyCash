using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.Models
{

	/*
		Get campaign stats (from yesterday to today)
		https://api.monetizer.co/data/report.php?nid={0}&rcid={1}&order=clicks&direction=desc&keyword=geo_os&start_ts={2}&end_ts={3}&tz=Europe/Belgrade
	*/

	[Serializable()]
	public class AfflowCampaignStat
	{
		public string Keyword { get; set; } = string.Empty;
		public int Clicks { get; set; } = 0;
		public int FilteredClicks { get; set; } = 0;
		public int OfferClicks { get; set; } = 0;
		public int Leads { get; set; } = 0;
		public double Revenue { get; set; } = 0.0;
		public double CR { get; set; } = 0.0;
	}
}
