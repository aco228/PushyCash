using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.Models
{
	public class AfflowLeagueStats
	{
		public string Keyword { get; set; } = string.Empty;
		public int Clicks { get; set; } = 0;
		public int FilteredClicks { get; set; } = 0;
		public int OfferClicks { get; set; } = 0;
		public int Leads { get; set; } = 0;
		public double Revenue { get; set; } = 0.0;
	}
}
