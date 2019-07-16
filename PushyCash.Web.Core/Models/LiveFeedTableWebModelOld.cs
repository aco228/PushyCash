using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Web.Core.Models
{
	[Serializable()]
	public class LiveFeedTableWebModelOld
	{
		public string Country { get; set; } = "";
		public string Conversions { get; set; } = "";
		public string Payout { get; set; } = "";
		public string AvaragePayout { get; set; } = "";
		public double Procentige { get; set; } = 0.0;

		public string Vertical { get; set; } = "";
		public string MobileOperator { get; set; } = "";
		public string OS { get; set; } = "";
		public string TotalConversions { get; set; } = "";
		public string TotalPayout { get; set; } = "";
		public string Timeout { get; set; } = "";
		public string LastConversions { get; set; } = "";

		public bool IsLocked { get; set; } = false;
		public bool HasActiveCampaing { get; set; } = false;
	}
}
