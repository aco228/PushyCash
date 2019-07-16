using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models
{
	public class DirectPushyCampaign : DirectModel
	{
		public DirectPushyCampaign() : base("PushyCampaign") { }

		public int PushyCampaignID { get; set; } = -1;
		public int LinkID { get; set; } = -1;
		public string LinkName { get; set; } = string.Empty;
		public int Conversions { get; set; } = 0;
		public int Clicks { get; set; } = 0;
		public double Payout { get; set; } = 0.0;
		public double Cost { get; set; } = 0.0;
		public double AvaragePayout { get; set; } = 0.0;
		public string OffersActivated { get; set; } = string.Empty;
		public int TrackingMinutes { get; set; } = 0;
		public double RecomendedBudget { get; set; } = 0.0;
		public int RecomendedClicksForConversion { get; set; } = 0;
		public double LinkAvaragePayout { get; set;  } = 0.0;
		public DateTime Started { get; set; } = DateTime.Now;
		public DateTime? Ended { get; set; } = null;

		public static DirectPushyCampaign Insert(int linkID, string linkName, string offersActivated, int trackingMinutes, double recomendedBudget, int recomendedConversionsForConversion, double linkAvaragePayout)
		{
			DirectPushyCampaign result = new DirectPushyCampaign();
			result.LinkID = linkID;
			int? id = PushyCashDirect.Instance.Execute("INSERT INTO [].PushyCampaign (LinkID, LinkName, OffersActivated, TrackingMinutes, RecomendedBudget, RecomendedClicksForConversion, LinkAvaragePayout)", 
				linkID, linkName, offersActivated, trackingMinutes, recomendedBudget, recomendedConversionsForConversion, linkAvaragePayout);
			if (!id.HasValue)
				return null;

			result.PushyCampaignID = id.Value;
			result.LinkName = linkName;
			result.LinkID = linkID;
			result.OffersActivated = offersActivated;
			result.TrackingMinutes = trackingMinutes;
			result.RecomendedBudget = recomendedBudget;
			result.RecomendedClicksForConversion = recomendedConversionsForConversion;
			result.LinkAvaragePayout = linkAvaragePayout;
			return result;
		}

	}
}
