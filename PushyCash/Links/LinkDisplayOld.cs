using PushyCash.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Links
{
	[Serializable()]
	public class LinkDisplayOld
	{
		private double _procentige = 0.0;
		public double GetProcentige() { return this._procentige; }
		public void SetProcentige(double procentige) { this._procentige = procentige; }

		public string Country { get; set; } = "";
		public string Conversions { get; set; } = "";
		public string Payout { get; set; } = "";
		public string AvaragePayout { get; set; } = "";
		public string Procentige { get; set; } = "";
		
		public string Vertical { get; set; } = "";
		public string MobileOperator { get; set; } = "";
		public string OS { get; set; } = "";
		public string TotalConversions { get; set; } = "";
		public string TotalPayout { get; set; } = "";
		public string Timeout { get; set; } = "";
		public string LastConversions { get; set; } = "";
		public string CampaignStatus { get; set; } = "";

		public LinkDisplayOld() { }
		public LinkDisplayOld(Link link)
		{
			this.Country = link.Country;
			this.Vertical = link.Vertical;
			this.MobileOperator = link.AfflowMobileOperator;
			this.OS = link.Device;
		}


		public LiveFeedTableWebModelOld Copy()
		{
			return new LiveFeedTableWebModelOld()
			{
				Country = this.Country,
				Conversions = this.Conversions,
				Payout = this.Payout,
				Procentige = this.GetProcentige(),
				AvaragePayout = this.AvaragePayout,
				Vertical = this.Vertical,
				MobileOperator = this.MobileOperator,
				OS = this.OS,
				TotalConversions = this.TotalConversions,
				TotalPayout = this.TotalPayout,
				Timeout = this.Timeout,
				LastConversions = this.LastConversions
			};
		}

	}
}
