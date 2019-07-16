using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Web.Core.Models
{
	[Serializable()]
	public class PushyNetworkCampaignWebModel
	{
		public string LinkName { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;

		public string NetworkName { get; set; } = string.Empty;
		public double Budget { get; set; } = 0.0;
		public double ToleratedDeficit { get; set; } = 0.0;
		public int ClicksForConversion { get; set; } = 0;
		public DateTime Started { get; set; } = DateTime.Now;
		public bool IsPaused { get; set; } = false;
		public bool IsStoped { get; set; } = false;
		public DateTime? Ended { get; set; } = null;
    public string StartedTime { get => Started.ToString("HH:mm"); }
    public string EndedTime { get => Ended.HasValue ? Ended.Value.ToString("HH:mm") : string.Empty; }

    // on update
    public int Conversions { get; set; } = 0;
		public int Clicks { get; set; } = 0;
		public double Payout { get; set; } = 0.0;
		public double Cost { get; set; } = 0.0;
		public double Revenue { get => this.Payout - this.Cost; }
		public double? LastBid { get; set; } = null;
	}
}
