using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models.TrafficNetworks
{
	public class DirectTrafficCampaignDayStat : PushyDirectModelBase
	{
		public DirectTrafficCampaignDayStat() : base("TrafficCampaignDayStat") { }

		public int TrafficCampaignDayStatID { get; set; } = -1;
		public int TrafficNetworkCampaignID { get; set; } = 0;
		public int TrafficNetworkID { get; set; } = 0;
		public DateTime Day { get; set; }

		public int Clicks { get; set; } = 0;
		public int Conversions { get; set; } = 0;
		public double Payout { get; set; } = 0.0;
		public double Costs { get; set; } = 0.0;
		public double Revenue => this.Payout - this.Costs;
		public double ROI => this.Payout == 0 || this.Costs == 0 ? -100.0 : (((this.Payout - this.Costs) / this.Costs) * 100);
		public int NumberOfTries { get; set; } = 0;

		public bool IsRunning { get; set; } = false;
		public bool AutoRunning { get; set; } = false;
		public bool WasProfitable => this.Revenue > 0;
		public bool HasConversions => this.Conversions > 0;
		public bool HasClicks => this.Clicks > 0;

		public DateTime? Started { get; set; } = null;
		public DateTime? Ended { get; set; } = null;
		
		public void Log(string text)
		{
			PushyCashDirect.Instance.Execute("INSERT INTO [].TrafficCampaignDayStatLog (TrafficCampaignDayStatID, Text)", this.TrafficCampaignDayStatID, text);
		}

		public override void Update()
		{
			this.GetDatabase().Update(this);
		}

	}
}
