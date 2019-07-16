using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Web.Core.Models
{
	[Serializable()]
	public class LiveFeedTableWebModel
	{
		public int Conversions = 0;

		public string Country { get; set; } = "";

		public string OneMinuteConversions { get; set; } = "";
		public string OneMinuteAvaragePayout { get; set; } = "";

		public string TwoMinuteConversions { get; set; } = "";
		public string TwoMinuteAvaragePayout { get; set; } = "";

		public string ThreeMinuteConversions { get; set; } = "";
		public string ThreeMinuteAvaragePayout { get; set; } = "";

		public string Vertical { get; set; } = "";
		public string MobileOperator { get; set; } = "";
		public string OS { get; set; } = "";
		public string TotalConversions { get; set; } = "";
		public string TotalPayout { get; set; } = "";
		public string LastConversions { get; set; } = "";

		public bool IsLocked { get; set; } = false;
		public bool HasActiveCampaing { get; set; } = false;
	}
}
