using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models
{
	public class DirectPushyCampaign : PushyDirectModelBase
	{
		public DirectPushyCampaign() : base("PushyCampaign") { }

		public int PushyCampaignID { get; set; } = -1;
		public int LinkID { get; set; } = -1;
		public string LinkName { get; set; } = string.Empty;
		public double AvaragePayout { get; set; } = 0.0;
		public string OffersActivated { get; set; } = string.Empty;
		public int TrackingMinutes { get; set; } = 0;
		public int TrackingConversions { get; set; } = 0;
		public int RecomendedClicksForConversion { get; set; } = 0;
		public double AllowedDeficit { get; set; } = 0.0;
		public DateTime Started { get; set; } = DateTime.Now;
		public DateTime? Ended { get; set; } = null;

	}
}
