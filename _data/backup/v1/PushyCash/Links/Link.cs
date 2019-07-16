using PushyCash.Afflow.Models;
using PushyCash.Campaigns;
using PushyCash.Direct.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Links
{
	public class Link
	{
		private static readonly object conversion_lockObj = new object();
		public static readonly object offerSnapshotMap_lockObj = new object();

		private Dictionary<string, Snapshots.SnapshotOffer> OfferSnapshotMap { get; set; } = new Dictionary<string, Snapshots.SnapshotOffer>();
		private DirectLink _directLink = null;
		private DateTime? _timeoutAfterCampaign = null;

		public Campaigns.PushyCampaign PushyCampaign { get; protected set; } = null;
		public LinkOffersConversion OfferConversions { get; protected set; } = null;

		public Dictionary<string, Afflow.Models.AfflowLink> AfflowLinkMap { get; protected set; } = new Dictionary<string, Afflow.Models.AfflowLink>();
		public List<TrafficNetworks.Core.TrafficCampaign> TrafficCampaigns { get; set; } = new List<TrafficNetworks.Core.TrafficCampaign>();
		public DirectLink DirectLink
		{
			get
			{
				if (this._directLink != null)
					return this._directLink;
				this._directLink = DirectLink.LoadOrInsert(this.Name, this.Country, this.MobileOperator, this.Vertical, this.Device);
				return this._directLink;
			}
		}
		
		public string Name { get => string.Format("{0}.{1}.{2}.{3}", this.Country, this.AfflowMobileOperator, this.Vertical, this.Device); }
		public string FullName { get => string.Format("{0}.{1}.{2}.{3}", this.Country, this.MobileOperator, this.Vertical, this.Device); }
		public string Country { get; protected set; } = "";
		public string Vertical { get; protected set; } = "";
		public string MobileOperator { get; protected set; } = "";
		public string AfflowMobileOperator { get; protected set; } = "";
		public string Device { get; protected set; } = "";
		public bool IsLocked { get => this._timeoutAfterCampaign.HasValue && DateTime.Now > this._timeoutAfterCampaign; }
		public bool HasActiveCampaign { get => this.PushyCampaign != null; }

		public int TrackConversions = 2;
		public int TrackMinutes = 1;
		public double TrackPayout = 0.1;

		public int TotalConversions { get; protected set; } = 0;												
		public double TotalPayout { get; protected set; } = 0;                          
		public int CurrentConversions { get => this.OfferConversions.CurrentConversions; }
		public double CurrentPayout { get => this.OfferConversions.CurrentPayout; }
		public double AvaragePayout { get => this.OfferConversions.AvaragePayout; }
		public DateTime LastConversion { get => this.OfferConversions.LastConversion; }
		private DateTime? TimeoutAfterCampaign { get => this._timeoutAfterCampaign; set { this._timeoutAfterCampaign = value; this.SaveCachedTimeout();  } } 
		
		#region # Construction #

		public Link(Afflow.Models.AfflowLink link) { this.OnLinkInsertion(link); }
		public void Add(Afflow.Models.AfflowLink link) { this.OnLinkInsertion(link); }
		private void OnLinkInsertion(Afflow.Models.AfflowLink link)
		{
			if (string.IsNullOrEmpty(this.Country)) this.Country = link.Country;
			if (string.IsNullOrEmpty(this.Vertical)) this.Vertical = link.vertical;
			if (string.IsNullOrEmpty(this.MobileOperator)) this.MobileOperator = link.MobileOperator;
			if (string.IsNullOrEmpty(this.AfflowMobileOperator)) this.AfflowMobileOperator = link.AfflowMobileOperator;
			if (string.IsNullOrEmpty(this.Device)) this.Device = link.Device;

			if (this.Device.ToLower() == "android") this.Device = "Android";
			else if (this.Device.ToLower() == "ios") this.Device = "iOS";
			
			this.OfferConversions = new LinkOffersConversion(this);
			this.AfflowLinkMap.Add(link.TrafficNetwork, link);
		}

		private void LoadCachedTimeout()
		{
			string location = string.Format(@"D:\Projects\AkoProjects\dot.net\PushyCash\_storage\afflow\link_cache\{0}.dat", this.Name);
			if (!File.Exists(location))
				return;
			this.TimeoutAfterCampaign = DateTime.Parse(File.ReadAllText(location));
		}

		private void SaveCachedTimeout()
		{
			string location = string.Format(@"D:\Projects\AkoProjects\dot.net\PushyCash\_storage\afflow\link_cache\{0}.dat", this.Name);
			File.WriteAllText(location, this._timeoutAfterCampaign.Value.ToString());
		}

		#endregion

		#region # Snapshot offers #

		public bool ContainsOffer(string offerID)
		{
			lock (offerSnapshotMap_lockObj)
				return this.OfferSnapshotMap.ContainsKey(offerID);
		}
		public void AddSnapshotOffer(Snapshots.SnapshotOffer offer)
		{
			lock (offerSnapshotMap_lockObj)
			{
				if (!this.OfferSnapshotMap.ContainsKey(offer.OfferID))
					this.OfferSnapshotMap.Add(offer.OfferID, offer);
			}
		}
		public void RemoveSnapshotOffer(Snapshots.SnapshotOffer offer)
		{
			lock (offerSnapshotMap_lockObj)
			{
				if (this.OfferSnapshotMap.ContainsKey(offer.OfferID))
					this.OfferSnapshotMap.Remove(offer.OfferID);
			}
		}
		public void MapSnapshotOffers(Dictionary<string, Snapshots.SnapshotOffer> offers)
		{
			lock (Link.conversion_lockObj)
			{
				if (this.OfferSnapshotMap.Count == 0)
				{
					foreach (KeyValuePair<string, Snapshots.SnapshotOffer> offer in offers)
					{
						offer.Value.MapTest.Add(this.Name);
						offer.Value.OfferIsMapped = true;
						this.OfferSnapshotMap.Add(offer.Key, offer.Value);
					}
					return;
				}

				// check if new offers are added
				foreach (KeyValuePair<string, Snapshots.SnapshotOffer> offer in offers)
					if (!this.OfferSnapshotMap.ContainsKey(offer.Key))
					{
						offer.Value.OfferIsMapped = true;
						this.OfferSnapshotMap.Add(offer.Key, offer.Value);
					}

				// remove offerse that dont exists anymore
				//TODO: Collection is modified.. wtf?

				int limit = 50, index = 0;
				for (; ; )
					try
					{
						foreach (KeyValuePair<string, Snapshots.SnapshotOffer> offer in this.OfferSnapshotMap)
							if (!offers.ContainsKey(offer.Key))
								this.OfferSnapshotMap.Remove(offer.Key);
						break;
					}
					catch (Exception e)
					{
						index++;
						if (index == limit)
						{
							int a = 0;
						}
						System.Threading.Thread.Sleep(500);
					}
			}
		}

		#endregion

		#region # Conversions #
		
		public void AddConversion(string offerID, double payout)
		{
			this.TotalConversions++; this.TotalPayout += payout;
			this.OfferConversions.Add(offerID, payout);
			PConsole.AddTrackedConvertion(payout);
		}
		public void OnGarbageCollection()
		{
			this.OfferConversions.GarbageCollection();
			this.CheckIfLinkShouldStart();
		}
		public void CheckIfLinkShouldStart()
		{
			LinkOffersConversionResult linkStartResult = this.OfferConversions.CheckIfLinkShouldStart();
			if (!linkStartResult.ShoulLinkStart)
				return;

			if (this.PushyCampaign != null || (TimeoutAfterCampaign.HasValue && DateTime.Now < TimeoutAfterCampaign))
			{
				this.OfferConversions.Clear();
				return;
			}

			#region # Check if Link should not start Campaign because of initial configuration #

			if (PushyGlobal.InitialConfiguration.DontStartNewCampaigns)
			{
				PLogger.Debug($"Link {this.FullName} will not start because DontStartNewCampaigns=true");
				this.OfferConversions.Clear();
				return;
			}

			if (PushyGlobal.InitialConfiguration.MaximumNumberOfCampaigns.HasValue && PushyGlobal.InitialConfiguration.TotalNumberOfActivatedCampaigns >= PushyGlobal.InitialConfiguration.MaximumNumberOfCampaigns.Value)
			{
				PLogger.Debug($"Link {this.FullName} will not start because MaximumNumberOfCampaigns");
				this.OfferConversions.Clear();
				return;
			}

			if (PushyGlobal.InitialConfiguration.MaximumNumberOfActiveCampaigns.HasValue && PushyContext.CampaignManager.ActiveCampaigns.Count >= PushyGlobal.InitialConfiguration.MaximumNumberOfActiveCampaigns.Value)
			{
				PLogger.Debug($"Link {this.FullName} will not start because MaximumNumberOfActiveCampaigns");
				this.OfferConversions.Clear();
				return;
			}

			if (PushyGlobal.InitialConfiguration.IgnoredCountries.Contains(this.Country))
			{
				PLogger.Debug($"Link {this.FullName} will not start because IgnoredCountries");
				this.OfferConversions.Clear();
				return;
			}

			#endregion
			
			this.TimeoutAfterCampaign = DateTime.Now.AddMinutes(60);

			if(PushyGlobal.CurrentRevenue <= (PushyGlobal.DailyBudget * -1))
			{
				PLogger.Error($"Link {this.FullName} will not start because daily budget is wasted");
				this.OfferConversions.Clear();
				return;
			}
			
			// recomendation based on tracked conversions
			AfflowOffersMetrics offerMetrics = PushyContext.AfflowManager.GetOfferMetrics(this.Country, this.Vertical, this.Device, this.OfferConversions.Offers);

			offerMetrics.LinkRecomendedBudget = (this.AvaragePayout + (this.AvaragePayout * 0.2)) * linkStartResult.Multiplier;
			if (offerMetrics.LinkRecomendedBudget > PushyGlobal.InitialConfiguration.MaximumBudget)
				offerMetrics.LinkRecomendedBudget = PushyGlobal.InitialConfiguration.MaximumBudget;
			offerMetrics.LinkRecomendedClicksWithoutConversion = offerMetrics.RecomendedClicksWithoutConversion;
			if (offerMetrics.LinkRecomendedClicksWithoutConversion < PushyGlobal.InitialConfiguration.MinimumClicksForConversions)
				offerMetrics.LinkRecomendedClicksWithoutConversion = PushyGlobal.InitialConfiguration.MinimumClicksForConversions;

			if (offerMetrics.LinkRecomendedClicksWithoutConversion > PushyGlobal.InitialConfiguration.MaximumClicksForConversions)
				offerMetrics.LinkRecomendedClicksWithoutConversion = PushyGlobal.InitialConfiguration.MaximumClicksForConversions;

			if (this.TrafficCampaigns.Count != 0)
			{
				this.PushyCampaign = new Campaigns.PushyCampaign(this, offerMetrics, linkStartResult, this.AvaragePayout);
				PushyGlobal.InitialConfiguration.TotalNumberOfActivatedCampaigns++;
			}
			else
				PLogger.OfferDoesNotHaveTrafficNetworks(this);

			this.OfferConversions.Clear();
		}
		public void CloseCampaign()
		{
			this.OfferConversions.Clear();
			this.TimeoutAfterCampaign = DateTime.Now.AddHours(4);
			this.PushyCampaign = null;
		}

		#endregion

	}
}
