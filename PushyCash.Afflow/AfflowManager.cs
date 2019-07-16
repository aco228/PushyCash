using PushyCash.Afflow.API;
using PushyCash.Afflow.MobileOperators;
using PushyCash.Afflow.Models;
using PushyCash.Afflow.Models.Campaigns;
using PushyCash.Afflow.Models.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow
{
	public class AfflowManager
	{
		private AfflowApi _api = null;
		public List<string> AvaliableCountries { get; set; } = new List<string>();
		public AfflowMobileOperatorManager MobileOperatorMap { get; protected set; } = null;
		public Rids.RidManager RidManager { get; protected set; } = null;
		public TrafficCosts.TrafficCostsManager TrafficCostManager { get; protected set; } = null;
		public AfflowTimezones Timezone { get; set; } = new AfflowTimezones();
		public Action<string, string> OnServerFatal = null;
		
		public AfflowManager()
		{
			this._api = new AfflowApi(this);
			this.MobileOperatorMap = new AfflowMobileOperatorManager();
			this.RidManager = new Rids.RidManager();
			this.TrafficCostManager = new TrafficCosts.TrafficCostsManager();
			//this.PreformLogin();
		}

		public void BeforeCollectionsStarts()
		{
			this.PreformLogin();
			this._api.SetLiveFeedLastID();
		}
		
		/// LOGIN
		/// 
		public void PreformLogin() { this._api.PreformLogin(); }

		/// LIVE FEED
		/// 
		public IEnumerable<AfflowLiveFeed> GetLiveFeed() { return this._api.GetLiveFeed(); }
		public List<AfflowLiveFeed> GetLiveFeedList()
		{
			List<AfflowLiveFeed> result = new List<AfflowLiveFeed>();
			foreach (AfflowLiveFeed entry in this.GetLiveFeed()) result.Add(entry);
			return result;
		}

		/// SNAPSHOT DATA
		/// 
		public IEnumerable<AfflowSnapshot> GetSnapshotData(string vertical) { return this._api.GetSnapshotData(vertical); }
		public List<AfflowSnapshot> GetSnapshotDataList(string vertical)
		{
			List<AfflowSnapshot> result = new List<AfflowSnapshot>();
			foreach (AfflowSnapshot entry in this.GetSnapshotData(vertical)) result.Add(entry);
			return result;
		}

		/// LINKS
		/// 
		public IEnumerable<AfflowLink> GetLinks() { return this._api.GetLinkData(); }
		public List<AfflowLink> GetLinkList()
		{
			List<AfflowLink> result = new List<AfflowLink>();
			foreach (AfflowLink entry in this.GetLinks()) result.Add(entry);
			return result;
		}
		public void InsertLink(string data) { this._api.InsertLink(data); }
		public void WhitelistLink(Afflow.Models.AfflowLink link, string offers) { this._api.WhitelistLink(link, offers); }

		/// CONVERSIONS
		/// 
		public IEnumerable<AfflowConversion> GetFirstConversions(int count) { return this._api.GetFirstConversions(count); }
		public IEnumerable<AfflowConversion> GetConversions() { return this._api.GetConversions(); }
		public List<AfflowConversion> GetConversionsList()
		{
			List<AfflowConversion> result = new List<AfflowConversion>();
			foreach (AfflowConversion entry in this.GetConversions()) result.Add(entry);
			return result;
		}

		/// TODAY REVENUE
		/// 
		public AfflowRevenue GetTodayRevenue() { return this._api.GetTodayRevenue(); }
		public List<AfflowLeagueStats> GetLeagueStatsList(string nid, string timezoneCity = "", DateTime? startDate = null, DateTime? endDate = null)
		{
			List<AfflowLeagueStats> result = new List<AfflowLeagueStats>();
			this.Timezone.City = timezoneCity;
			foreach (var entry in this.GetLeagueStats(nid, timezoneCity, startDate, endDate)) result.Add(entry);
			return result;
		}
		public IEnumerable<AfflowLeagueStats> GetLeagueStats(string nid, string timezoneCity = "", DateTime? startDate = null, DateTime? endDate = null)
		{
			if (!startDate.HasValue) startDate = DateTime.Today.AddDays(-1);
			if (!endDate.HasValue) endDate = DateTime.Today.AddDays(1);
			this.Timezone.City = timezoneCity;
			return this._api.GetLeagueStats(nid, startDate.Value, endDate.Value);
		}

		/// STATS
		///
		public AfflowCampaignStat GetCampaignStats(string nid, string campaignID, string expectedCountry, string expectedOS, DateTime? startDate = null, DateTime? endDate = null)
		{
			if (!startDate.HasValue) startDate = DateTime.Today;
			if (!endDate.HasValue) endDate = DateTime.Now.AddDays(1);
			return this._api.GetCampaignStats(nid, campaignID, expectedCountry, expectedOS, startDate.Value, endDate.Value);
		}
		public AfflowOffersMetrics GetOfferMetrics(string country, string vertical, string device, List<string> offers)
		{
			double? cost = this.TrafficCostManager.GetPrice(vertical, device, country);
			AfflowOffersMetrics result = new AfflowOffersMetrics(cost.Value);
			foreach (AfflowOfferStats s in this.GetOfferStats(country, vertical, offers))
				result.Add(s);
			return result;
		}
		public List<AfflowOfferStats> GetOfferStatsList(string country, string vertical, List<string> offers)
		{
			List<AfflowOfferStats> result = new List<AfflowOfferStats>();
			foreach (var o in this.GetOfferStats(country, vertical, offers))
				result.Add(o);
			return result;
		}
		public IEnumerable<AfflowOfferStats> GetOfferStats(string country, string vertical, List<string> offers)
		{
			if (this.RidManager == null) return null;
			int? rid = this.RidManager.GetRid(country, vertical);
			if (!rid.HasValue) return null;
			return this._api.GetOfferStats(offers, rid.Value);
		}
		public AfflowInitialCampaignData GetInitialCampaignData(string nid, string campaignID, string expectedCountry, string expectedMNO, string expectedDevice)
		{
			AfflowInitialCampaignData result = new AfflowInitialCampaignData();
			result.NID = nid;
			result.CampaignID = campaignID;
			result.Country = expectedCountry;
			result.Device = expectedDevice;
			result.MobileOperator = expectedMNO;
			result.CampaignStats = this.GetCampaignStats(nid, campaignID, expectedCountry, expectedDevice);
			result.Offers = new List<AfflowOfferMobileOperatorStats>();

			DateTime startDate = DateTime.Now.AddDays(-30);
			DateTime endDate = DateTime.Now.AddMinutes(-1);
			foreach (var entry in this._api.GetOfferMobileOperatorCampaignStats(nid, campaignID, expectedCountry, expectedMNO, startDate, endDate))
				result.Offers.Add(entry);			
			return result;
		}
		public bool CheckIfFilteredOfferIsReceivingTraffic(int nid, string campaignID, List<string> offers)
		{
			int totalClicks = 0, offerClicks = 0;
			foreach(var o in this.GetCampaignMobileOperatorMap(nid, campaignID))
			{
				totalClicks += o.Clicks;
				if (offers.Contains(o.Offer) || offers.Contains(o.Offer.Replace("#",string.Empty)))
					offerClicks += o.Clicks;
			}
			
			double procentige = (offerClicks * 1.0) / (totalClicks * 1.0) * 100.0;
			return (procentige < 80) ? false : true;
		}
		public IEnumerable<AfflowCampaignMobileOperatorOfferMap> GetCampaignMobileOperatorMap(int nid, string campaignID) => this._api.GetCampaignMobileOperatorMap(nid, campaignID);
		public void SetSpending(int nid, string campaignID, DateTime date, double amount) => this._api.SetSpending(nid, campaignID, date, amount);
		
		///	TARGETS STATS
		///	
		public List<AfflowTrafficTargetReport> GetTrafficTargetInfoList(int nid, string campaignID)
		{
			List<AfflowTrafficTargetReport> result = new List<AfflowTrafficTargetReport>();
			foreach (var r in this.GetTrafficTargetInfo(nid, campaignID)) result.Add(r);
			return result;
		}
		public IEnumerable<AfflowTrafficTargetReport> GetTrafficTargetInfo(int nid, string campaignID)
		{
			return this._api.GetTrafficTargetInfo(nid, campaignID);
		}
	}
}
