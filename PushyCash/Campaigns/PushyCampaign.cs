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
		private DateTime? ClosingTimeout { get; set; } = null;
		
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
		
		public double AvaragePayout { get => this.DirectPushyCampaign.AvaragePayout; }
		public double AllowedDeficit { get => this.DirectPushyCampaign.AllowedDeficit; }
		public string OffersWhitelisted { get => this.DirectPushyCampaign.OffersActivated; }
		public int RecomendedClicksForConversion { get => this.DirectPushyCampaign.RecomendedClicksForConversion; }

		public int Conversions
		{
			get
			{
				int result = 0;
				foreach (var n in this.PushyNetworkCampaigns) result += n.Value.Conversions;
				return result;
			}
		}
		public int Clicks
		{
			get
			{
				int result = 0;
				foreach (var n in this.PushyNetworkCampaigns) result += n.Value.Clicks;
				return result;
			}
		}
		public double Payout
		{
			get
			{
				double result = 0;
				foreach (var n in this.PushyNetworkCampaigns) result += n.Value.Payout;
				return result;
			}
		}
		public double Spent
		{
			get
			{
				double result = 0;
				foreach (var n in this.PushyNetworkCampaigns) result += n.Value.Spent;
				return result;
			}
		}
		public double EarnedMoney
		{
			get
			{
				double result = 0;
				foreach (var n in this.PushyNetworkCampaigns) result += n.Value.EarnedMoney;
				return result;
			}
		}
		public double AvarageROI
		{
			get
			{
				double result = 0;
				foreach (var n in this.PushyNetworkCampaigns) result += n.Value.ROI;
				return result / this.PushyNetworkCampaigns.Count;
			}
		}

		public PushyCampaign(Links.Link link, Afflow.Models.AfflowOffersMetrics metrics, LinkOffersConversionResult linkConversionsResult, double avaragePayout)
		{
			this.Link = link;
			this.Metrics = metrics;

			this.DirectPushyCampaign = new DirectPushyCampaign()
			{
				LinkID = link.DirectLink.LinkID,
				LinkName = link.FullName,
				AvaragePayout = avaragePayout,
				OffersActivated = link.OfferConversions.GetWhitelistedOffersForSlackReport(),
				TrackingMinutes = linkConversionsResult.Minutes,
				TrackingConversions = linkConversionsResult.NumberOfConversions,
				RecomendedClicksForConversion = metrics.RecomendedClicksWithoutConversion,
				AllowedDeficit = avaragePayout + (avaragePayout * 0.8)
			};
			PushyCashDirect.Instance.Insert(this.DirectPushyCampaign);
			this.OffersActivated = link.OfferConversions.Offers;
			
			PLogger.Debug($"Link {link.FullName} is starting campaign");
			
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
				try
				{
					this.PushyNetworkCampaigns.Add(string.Format("{0}_{1}", tc.Afflowlink.nid, tc.ID), new PushyNetworkCampaign(this, tc));
					this.Link.OfferConversions.WhitelistOffersInAfflow(tc.TrafficNetworkName);
				}
				catch (Exception e)
				{
					PLogger.Error("Error starting netwwork campaign:: " + e.ToString());
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
			
			if (this.PushyNetworkCampaigns.Count == closedCampaigns)
			{
				if(this.ClosingTimeout == null)
				{
					PLogger.Debug("Campaign is closing '"+this.Link.Name+"', Adding 10min closing timeout");
					this.ClosingTimeout = DateTime.Now.AddMinutes(10);
					return;
				}

				if(this.ClosingTimeout.HasValue && (this.ClosingTimeout.Value > DateTime.Now))
					this.CloseCampaign();
			}
		}
		public void CloseCampaign()
		{
			if(this.DirectPushyCampaign != null)
			{
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
