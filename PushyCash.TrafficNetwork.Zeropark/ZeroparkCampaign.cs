using PushyCash.TrafficNetworks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Zeropark
{
	public enum ZeroparkVisibility { processing , very_high , high , medium , low , very_low }

	[Serializable()]
	public class ZeroparkCampaign : TrafficCampaign
	{
		public double MaximumBidAllowed = 0.0350;
		public ZeroparkCampaign(TrafficCampaignData data) : base(data) { }
		public ZeroparkVisibility ZeroparkVisibility
		{
			get
			{
				switch (this.Data.Visibility)
				{
					case 1:
						return ZeroparkVisibility.very_low;
					case 3:
						return ZeroparkVisibility.low;
					case 5:
						return ZeroparkVisibility.medium;
					case 8:
						return ZeroparkVisibility.high;
					case 10:
						return ZeroparkVisibility.very_high;
					default:
						return ZeroparkVisibility.processing;
				}
			}
		}

		public override void BeforeAutoBiddingStarts()
		{
			double? defaultBidValue = 0.0020;
			this.Bid(defaultBidValue.Value);
			this.Reload();
		}
		public override bool AutoBid()
		{
			this.Reload();

			// bid is still processing
			if(this.ZeroparkVisibility == ZeroparkVisibility.processing)
				return false;

			double nextBid = this.BidValue.Value;

			if (this.ZeroparkVisibility == ZeroparkVisibility.very_high)
				nextBid -= 0.0003;
			else if (this.ZeroparkVisibility == ZeroparkVisibility.high)
				nextBid -= 0.0001;
			else if (this.ZeroparkVisibility == ZeroparkVisibility.medium)
				nextBid += 0.0003;
			else if (this.ZeroparkVisibility == ZeroparkVisibility.low)
				nextBid += 0.0003;
			else if (this.ZeroparkVisibility == ZeroparkVisibility.very_low)
				nextBid += 0.0004;
			
			if (nextBid <= 0.0)
				nextBid = 0.0001;
			if (nextBid >= MaximumBidAllowed) // maximum BID, check this out again!!!
				nextBid = MaximumBidAllowed;
			
			if (this.Bid(nextBid))
				return true;
			return false;
		}

	}
}
