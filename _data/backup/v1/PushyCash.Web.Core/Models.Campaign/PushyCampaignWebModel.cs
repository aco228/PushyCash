using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Web.Core.Models
{
	[Serializable()]
	public class PushyCampaignWebModel
	{

		public string LinkName { get; set; } = string.Empty;
		public string OffersActivated { get; set; } = string.Empty;
		public int NumberOfConversions { get; set; } = 0;
		public int TrackingMinutes { get; set; } = 0;
		public double AllowedDeficit { get; set; } = 0.0;
		public int RecomendedClicksForConversion { get; set; } = 0;
		public double LinkAvaragePayout { get; set; } = 0.0;
	}
}
