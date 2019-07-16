using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models
{
	public class DirectPushyNetworkCampaign : DirectModel
	{
		public DirectPushyNetworkCampaign() : base("PushyNetworkCampaign") { }

		public int PushyNetworkCampaignID { get; set; } = -1;
		public int PushyCampaignID { get; set; } = -1;
		public int LinkID { get; set; } = -1;
		public int TrafficNetworkID { get; set; } = -1;
		public int TrafficNetworkCampaignID { get; set; } = -1;
		public string TrafficNetworkName { get; set; } = string.Empty;
		public string LinkName { get; set; } = string.Empty;
		public string CampaignID { get; set; } = string.Empty;

		public int Conversions { get; set; } = 0;
		public int Clicks { get; set; } = 0;
		public double Payout { get; set; } = 0.0;
		public double Cost { get; set; } = 0.0;

		public int InitialConversions { get; set; } = 0;
		public int InitialClicks { get; set; } = 0;
		public double InitialPayout { get; set; } = 0.0;
		public double InitialCost { get; set; } = 0.0;

		public double? LastBid { get; set; } = 0.0;
		public double Budget { get; set; } = 0.0;
		public int ClicksForConversion { get; set; } = 0;
		public string ReasonForClosing { get; set; } = string.Empty;
		public bool WasProfitable { get; set; } = false;
		public bool HadConversions { get; set; } = false;
		public bool HadClicks { get; set; } = false;
		public bool WasFinished { get; set; } = false;
		public double ROI { get; set; } = -100.0;
		public DateTime Started { get; set; } = DateTime.Now;
		public DateTime? Ended { get; set; } = null;
		
		public void Log(string text)
		{
			PushyCashDirect.Instance.Execute("INSERT INTO [].PushyNetworkCampaignLog (PushyNetworkCampaignID, Text)", this.PushyNetworkCampaignID, text);
		}
		
		public static DirectPushyNetworkCampaign Load(int id)
		{
			return PushyCashDirect.Instance.LoadContainer("SELECT * FROM [].PushyNetworkCampaign WHERE PushyNetworkCampaignID={0}", id).Convert<DirectPushyNetworkCampaign>();
		}

	}
}
