using Direct.Core;
using PushyCash.Afflow.Models;
using PushyCash.Direct;
using PushyCash.Direct.Models;
using PushyCash.Links;
using PushyCash.TrafficNetworks.Core;
using PushyCash.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Campaigns
{
	public class PushyCampaign
	{		
		private DateTime? LastConversion { get; set; } = null;
		
		public Links.Link Link { get; protected set; } = null;
		public AfflowOffersMetrics Metrics { get; protected set; } = null;
		public DirectPushyCampaign DirectPushyCampaign { get; protected set; } = null;
		public Dictionary<string, PushyNetworkCampaign> PushyNetworkCampaigns { get; protected set; } = new Dictionary<string, PushyNetworkCampaign>(); // nid_id * id = keyword
		public DateTime Created { get; protected set; } = DateTime.Now;
		public PushyCampaignWebModel WebModel = null;
		public bool AllCampaignsAreInactive
		{
			get
			{
				foreach (var c in this.PushyNetworkCampaigns)
					if (!c.Value.IsStoped)
						return false;
				return true;
			}
		}
		public string NetworkNames
		{
			get
			{
				string result = "";
				foreach (var n in this.PushyNetworkCampaigns) result += n.Value.TrafficCampaign.TrafficNetworkName + ",";
				return result.Substring(0, result.Length - 1);
			}
		}
		public List<string> OffersActivated { get; protected set; } = new List<string>();
		public string OffersWhitelisted { get; protected set; } = string.Empty;
		public double AllowedDeficit { get; protected set; } = 0.0;

		public double CurrentCost
		{
			get
			{
				double result = 0.0;
				foreach (var pnc in this.PushyNetworkCampaigns) result += pnc.Value.Spent;
				return result;
			}
		}
		public double CurrentRevenue
		{
			get
			{
				double result = 0.0;
				foreach (var pnc in this.PushyNetworkCampaigns) result += pnc.Value.Payout;
				return result;
			}
		}
		public int CurrentConversions
		{
			get
			{
				int result = 0;
				foreach (var pnc in this.PushyNetworkCampaigns) result += pnc.Value.Conversions;
				return result;
			}
		}
		public int CurrentClicks
		{
			get
			{
				int result = 0;
				foreach (var pnc in this.PushyNetworkCampaigns) result += pnc.Value.Clicks;
				return result;
			}
		}
		
		public PushyCampaign(Links.Link link, Afflow.Models.AfflowOffersMetrics metrics, LinkOffersConversionResult linkConversionsResult, double avaragePayout)
		{
			this.AllowedDeficit = avaragePayout + (avaragePayout * 0.2);

			PLogger.Debug($"Link '{link.FullName}' is starting campaign");
			this.DirectPushyCampaign = Direct.Models.DirectPushyCampaign.Insert(
				link.DirectLink.LinkID, link.FullName, 
				link.OfferConversions.GetWhitelistedOffersForSlackReport(), 
				link.TrackMinutes, metrics.LinkRecomendedBudget, metrics.LinkRecomendedClicksWithoutConversion, link.AvaragePayout);
			this.OffersWhitelisted = link.OfferConversions.GetWhitelistedOffersForSlackReport();
			this.OffersActivated = link.OfferConversions.Offers;
			
			this.Link = link;
			this.Metrics = metrics;

			this.WebModel = new PushyCampaignWebModel()
			{
				LinkName = this.Link.FullName,
				OffersActivated = this.Link.OfferConversions.GetWhitelistedOffersForSlackReport(),
				NumberOfConversions = linkConversionsResult.NumberOfConversions,
				TrackingMinutes = linkConversionsResult.Minutes,
				AllowedDeficit = Math.Round(this.AllowedDeficit),
				RecomendedClicksForConversion = this.Metrics.LinkRecomendedClicksWithoutConversion,
				LinkAvaragePayout = Math.Round(avaragePayout, 2)
			};
			Web.Core.PushyWebSender.Send<PushyCampaignWebModel>("OnPushyCampaignStart", this.WebModel);

			foreach (TrafficCampaign tc in Link.TrafficCampaigns)
			{
				this.PushyNetworkCampaigns.Add(string.Format("{0}_{1}", tc.Afflowlink.nid, tc.ID), new PushyNetworkCampaign(this, tc));
				this.Link.OfferConversions.WhitelistOffersInAfflow(tc.TrafficNetworkName);
			}

			PLogger.OfferStarting(link.Name);
			PushyContext.Slack.Campaign.Start(link.Country, link.MobileOperator, link.Vertical, link.Device, this.OffersWhitelisted, this.NetworkNames);

			PushyContext.CampaignManager.AddCampaign(this);
			this.Created = DateTime.Now;
		}
		
		public void Stop()
		{
			foreach (var c in this.PushyNetworkCampaigns)
				c.Value.Stop("Stop called from PushyCampaign");
		}
		public void Update()
		{
			int closedCampaigns = 0;
			foreach (var tn in this.PushyNetworkCampaigns)
			{
				tn.Value.Update();
				if (tn.Value.IsStoped)
					closedCampaigns++;
			}

			if (this.CurrentCost > PushyCampaignManager.DailyBudget)
			{
				foreach (var tn in this.PushyNetworkCampaigns)
					tn.Value.Stop("Daily budget is exceded");
				PLogger.Debug($"Link {this.Link.FullName} is stoping campaign because daily budget is wasted. CurrentCost:{Math.Round(PushyCampaignManager.CurrentCost, 2)}, this.CurrentCost:{Math.Round(this.CurrentCost, 2)}, DailyBudget:{Math.Round(PushyCampaignManager.DailyBudget, 2)}");
			}

			if (this.PushyNetworkCampaigns.Count == closedCampaigns)
				this.CloseCampaign();
		}
		public void CloseCampaign()
		{
			if(this.DirectPushyCampaign != null)
			{
				this.DirectPushyCampaign.Conversions = this.CurrentConversions;
				this.DirectPushyCampaign.Clicks = this.CurrentClicks;
				this.DirectPushyCampaign.Cost = this.CurrentCost;
				this.DirectPushyCampaign.Payout = this.CurrentRevenue;
				this.DirectPushyCampaign.AvaragePayout = this.CurrentConversions == 0 ? 0.0 : Math.Round(this.CurrentRevenue / this.CurrentConversions, 2);
				this.DirectPushyCampaign.Ended = DateTime.Now;
				PushyCashDirect.Instance.Update(this.DirectPushyCampaign);
			}

			List<string> networkReports = new List<string>();
			//foreach (var n in this.PushyNetworkCampaigns)
			//	networkReports.Add(string.Format("{0} {1}conv / {2}payout", n.Value.TrafficCampaign.TrafficNetworkName, n.Value.Conversions, n.Value.Payout));
			
			PushyContext.CampaignManager.RemoveCampaign(this);
			Web.Core.PushyWebSender.Send<PushyCampaignWebModel>("OnPushyCampaignStop", this.WebModel);
			PLogger.Debug($"Link '{this.Link.FullName}' campaign is closed");

			PushyContext.Slack.Campaign.End(Link.Country, Link.MobileOperator, Link.Vertical, Link.Device, this.OffersWhitelisted, networkReports.ToArray());
			this.Link.CloseCampaign();
		}
	}
}
