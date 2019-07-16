using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Web.Core.Models.Campaign
{
	[Serializable()]
	public class PushyNetworkCampaignUpdateWebModel
	{
		public string LinkName { get; set; } = string.Empty;
		public string PushyNetworkCampaignName { get; set; } = string.Empty;

		public bool IsStoped { get; set; } = false;
		public bool IsPaused { get; set; } = false;

		public int Conversions { get; set; } = 0;
		public int Clicks { get; set; } = 0;
		public double Payout { get; set; } = 0.0;
		public double Cost { get; set; } = 0.0;
		public double Revenue { get => this.Payout - this.Cost; }
		public double? LastBid { get; set; } = null;
	}
}
