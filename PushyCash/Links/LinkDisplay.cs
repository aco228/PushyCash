using PushyCash.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Links
{
	[Serializable()]
	public class LinkDisplay
	{
		private int _conversions = 0;
		public int GetConversions() { return this._conversions; }
		public void SetConversions(int conversions) { this._conversions = conversions; }

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
		public string Timeout { get; set; } = "";
		public string LastConversions { get; set; } = "";
		public string CampaignStatus { get; set; } = "";

		public LinkDisplay() { }
		public LinkDisplay(Link link)
		{
			this.Country = link.Country;
			this.Vertical = link.Vertical;
			this.MobileOperator = link.AfflowMobileOperator;
			this.OS = link.Device;
		}
		
		public LiveFeedTableWebModel Copy()
		{
			return new LiveFeedTableWebModel()
			{
				Country = this.Country,
				OneMinuteConversions = this.OneMinuteConversions,
				OneMinuteAvaragePayout = this.OneMinuteAvaragePayout,
				TwoMinuteConversions = this.TwoMinuteConversions,
				TwoMinuteAvaragePayout = this.TwoMinuteAvaragePayout,
				ThreeMinuteConversions = this.ThreeMinuteConversions,
				ThreeMinuteAvaragePayout = this.ThreeMinuteAvaragePayout,
				Vertical = this.Vertical,
				MobileOperator = this.MobileOperator,
				OS = this.OS,
				TotalConversions = this.TotalConversions,
				TotalPayout = this.TotalPayout,
				LastConversions = this.LastConversions,
				Conversions = this._conversions
			};
		}


	}
}
