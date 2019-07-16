using PushyCash.Afflow.Models;
using PushyCash.Afflow.Models.Campaigns;
using PushyCash.Afflow.Models.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.API
{
	internal class AfflowApi : AfflowApiBase
	{
		private static int LastLiveFeedID = 0;
		private static int LastConversionID = 0;

		public AfflowApi(AfflowManager manager): base(manager)
		{ }

		public void PreformLogin()
		{
			string url = "https://api.monetizer.co/auth.php";
			dynamic json =  this.Post(url, "&email=nsp@monkeyhou.se&pass=k6NpUaTFOC");
			if (json == null || json.ToString().Equals("[]"))
				return;

			AfflowApi.TOKEN = json.api_key;
			int a = 0;
		}
		
		// SUMMARY: Save last feed id, so we will ignore old data when collection starts
		public void SetLiveFeedLastID()
		{
			string url = "https://api.monetizer.co/data/partner-lead-update.php?limit=1";
			dynamic json = this.Request(url);
			if (json == null || json.ToString().Equals("[]"))
				return;
			foreach (dynamic row in json)
				LastLiveFeedID = row.lead_id;
		}

		public IEnumerable<AfflowLiveFeed> GetLiveFeed()
		{
			string url = "https://api.monetizer.co/data/partner-lead-update.php?limit=35";
			dynamic json = this.Request(url);

			if (json == null || json.ToString().Equals("[]"))
				yield break;

			int maxValueForIteration = 0;

			foreach (dynamic row in json)
			{
				AfflowLiveFeed entry = AfflowLiveFeed.Get(row);

				int lead_id = row.lead_id;
				if (maxValueForIteration == 0 || lead_id > maxValueForIteration)
					maxValueForIteration = lead_id;
				if (LastLiveFeedID != 0 && LastLiveFeedID >= lead_id)
					continue;

				//if (entry != null && this._manager.AvaliableCountries.Contains(entry.Country))
				if (entry == null)
					continue;

				yield return entry;
			}

			LastLiveFeedID = maxValueForIteration;
			yield break;
		}

		public IEnumerable<AfflowLink> GetLinkData()
		{
			int page = 1;
			for(; ; )
			{
				string url = string.Format("https://api.monetizer.co/data/partner-link.php?page={0}&bbr=1&filtered=0", page);
				page++;

				dynamic json = this.Request(url);
				if (json == null || json.ToString().Equals("[]"))
					yield break;

				foreach (dynamic row in json)
				{
					string name = row.name;
					if (name[0] != '_')
						continue;
					AfflowLink entry = AfflowLink.Get(row);
					if (entry == null)
						continue;

					entry.AfflowMobileOperator = this._manager.MobileOperatorMap.GetName(entry.Country, entry.MobileOperator);
					yield return entry;
				}
			}
			yield break;
		}

		public void WhitelistLink(Afflow.Models.AfflowLink link, string offers)
		{
			string data = "";
			data += "&disable_bbr=" + link.disable_bbr;
			data += "&disable_popunder=" + link.disable_popunder;
			data += "&safe_url=null";
			data += "&safe_page_rcid={rcid}";
			data += "&whitelist_data=" + link.whitelist_data;
			data += "&options_data=" + link.options_data;
			data += "&offer_whitelist_data=" + offers;
			data += "&vertical=" + link.vertical;
			data += "&name=" + link.name;
			data += "&bbr_rcid={rcid}";
			data += "&popunder_rcid={rcid}";
			data += "&desktop_redirect=" + link.desktop_redirect;
			data += "&hash=" + link.hash;
			string url = "https://api.monetizer.co/data/partner-link.php";
			this.Post(url, data);
		}

		public IEnumerable<AfflowSnapshot> GetSnapshotData(string vertical)
		{
			string url = "https://api.monetizer.co/data/partner-splitter-report.php?fast=1&vertical={0}&order=clicks&direction=desc&keyword=carrier_offer&date_range=three&tz=Europe/Belgrade";
			dynamic json = this.Request(string.Format(url, vertical));
			if (json == null || json.ToString().Equals("[]"))
				yield break;

			foreach (dynamic row in json)
			{
				AfflowSnapshot data = AfflowSnapshot.Get(row, vertical);
				if (data != null || this._manager.AvaliableCountries.Contains(data.Country))
					yield return data;
			}

			yield break;
		}

		public AfflowRevenue GetTodayRevenue()
		{
			string date = string.Format("{0}-{1}-{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
			string url = string.Format("https://api.monetizer.co/data/earnings.php?type=NikolaSpasojevic&start_date={0}&end_date={0}&tz=Europe/Berlin", date);
			dynamic json = this.Request(url);
			if (json == null || json.ToString().Equals("[]"))
				return null;

			AfflowRevenue result = new AfflowRevenue();
			result.Ravenue = double.Parse(json.First.amount.ToString()) * -1;
			return result;

		}

		public IEnumerable<AfflowConversion> GetFirstConversions(int count)
		{
			string url = "https://api.monetizer.co/data/partner-lead-update.php?owner=1&limit=25";
			dynamic json = this.Request(url);

			if (json == null || json.ToString().Equals("[]"))
				yield break;

			int index = 0;
			foreach (dynamic row in json)
			{
				if (index == count)
					break;

				AfflowConversion entry = AfflowConversion.Get(row);
				if (entry != null)
				{
					index++;
					yield return entry;
				}
			}

			yield break;
		}

		public IEnumerable<AfflowConversion> GetConversions()
		{
			string url = "https://api.monetizer.co/data/partner-lead-update.php?owner=1&limit=25";
			dynamic json = this.Request(url);

			if (json == null || json.ToString().Equals("[]"))
				yield break;

			int maxValueForIteration = 0;
			foreach (dynamic row in json)
			{

				int lead_id = row.lead_id;
				if(LastConversionID == 0)
				{
					LastConversionID = lead_id;
					yield break;
				}

				if (maxValueForIteration == 0 || lead_id > maxValueForIteration)
					maxValueForIteration = lead_id;
				if (LastConversionID >= lead_id)
					continue;
				
				AfflowConversion entry = AfflowConversion.Get(row);
				if (entry == null)
					continue;

				yield return entry;
			}

			LastConversionID = maxValueForIteration;
			yield break;
		}

		public void InsertLink(string data)
		{
			string url = "https://api.monetizer.co/data/partner-link.php";
			this.Post(url, data);
		}

		public IEnumerable<AfflowLeagueStats> GetLeagueStats(string nid, DateTime startDate, DateTime endDate)
		{
			string url = string.Format("https://api.monetizer.co/data/report.php?nid={0}&start_ts={1}&end_ts={2}&tz=Europe/Belgrade", nid, this.DateToUnix(startDate), this.DateToUnix(endDate));
			dynamic json = this.Request(url, tolerateNull:true);
			if (json == null || json.ToString().Equals("[]"))
				yield break;

			foreach (dynamic row in json)
				yield return new AfflowLeagueStats()
				{
					Keyword = row.keyword,
					Clicks = row.clicks,
					Revenue = row.revenue,
					OfferClicks = row.offer_clicks,
					Leads = row.leads
				};

			yield break;
		}
		
		public AfflowCampaignStat GetCampaignStats(string nid, string campaignID, string expectedCountry, string expectedOS, DateTime startDate, DateTime endDate)
		{
			string url = string.Format("https://api.monetizer.co/data/report.php?nid={0}&rcid={1}&order=clicks&direction=desc&keyword=geo_os&start_ts={2}&end_ts={3}&tz=Europe/Belgrade", nid, campaignID, this.DateToUnix(startDate), this.DateToUnix(endDate));
			dynamic json = this.Request(url, tolerateNull:true);
			if (json == null)
				return null;

			string expectedKeyword = string.Format("{0}, {1}", expectedCountry, expectedOS).ToLower();
			foreach (dynamic row in json)
				if(row.keyword.ToString().ToLower().Equals(expectedKeyword))
					return new AfflowCampaignStat()
					{
						Keyword = row.keyword,
						Clicks = row.clicks,
						Revenue = row.revenue,
						OfferClicks = row.offer_clicks,
						Leads = row.leads,
						CR = row.cr
					};

			return null;
		}

		public IEnumerable<AfflowOfferMobileOperatorStats> GetOfferMobileOperatorCampaignStats(string nid, string campaignID, string expectedCountry, string expectedMNO, DateTime startDate, DateTime endDate)
		{
			string url = string.Format("https://api.monetizer.co/data/report.php?nid={0}&rcid={1}&order=clicks&direction=desc&keyword=carrier_offer&start_ts={2}&end_ts={3}&tz=Europe/Belgrade", nid, campaignID, this.DateToUnix(startDate), this.DateToUnix(endDate));
			dynamic json = this.Request(url, tolerateNull: true);
			if (json == null)
				yield break;

			string expectedKeyword = string.Format("{0} {1}", expectedCountry, expectedMNO).ToLower();
			foreach (dynamic row in json)
			{
				if (!row.keyword.ToString().ToLower().Contains(expectedKeyword))
					continue;
				yield return new AfflowOfferMobileOperatorStats()
				{
					OfferID = row.keyword.ToString().Split(',')[1].Trim(),
					Clicks = row.clicks,
					Conversions = row.leads, 
					Revenue = row.revenue
				};
			}
			
			yield break;
		}

		public IEnumerable<AfflowOfferStats> GetOfferStats(List<string> offers, int rid)
		{
			string url = string.Format("https://api.monetizer.co/data/partner-splitter-report.php?fast=1&rid={0}&order=clicks&direction=desc&keyword=offer&date_range=today&tz=Europe/Belgrade", rid);
			dynamic json = this.Request(url);
			if (json == null || json.ToString().Equals("[]"))
				yield break;

			foreach(dynamic row in json)
			{
				string keyword = row.keyword.ToString();
				if (offers.Contains(keyword) || offers.Contains(keyword.Replace("#", string.Empty)))
					yield return new AfflowOfferStats()
					{
						Keyword = row.keyword,
						Clicks = row.offer_clicks,
						Conversions = row.leads,
						Revenue = row.revenue,
						CR = row.cr,
						OfferEPC = row.offer_epc
					};
			}

			yield break;
		}

		public IEnumerable<AfflowTrafficTargetReport> GetTrafficTargetInfo(int nid, string campaignID)
		{
			DateTime start = DateTime.Now.AddDays(-7);
			DateTime end = DateTime.Now;
			string url = string.Format("https://api.monetizer.co/data/report.php?nid={0}&rcid={1}&order=clicks&direction=desc&keyword=ts&start_ts={2}&end_ts={3}&tz=Europe/Belgrade", nid, campaignID, this.DateToUnix(start), this.DateToUnix(end));
			dynamic json = this.Request(url, true);
			if (json == null || json.ToString().Equals("[]"))
				yield break;

			foreach (dynamic row in json)
				yield return new AfflowTrafficTargetReport()
				{
					Keyword = row.keyword,
					Clicks = row.clicks,
					Conversions = row.leads,
					BotClicks = row.bot_clicks,
					Revenue = row.revenue
				};
			yield break;
		}

		public IEnumerable<AfflowCampaignMobileOperatorOfferMap> GetCampaignMobileOperatorMap(int nid, string campaignID)
		{
			DateTime start = DateTime.Today.AddDays(-1);
			DateTime endDate = DateTime.Now;
			string url = string.Format("https://api.monetizer.co/data/report.php?nid={0}&rcid={1}&order=clicks&direction=desc&keyword=carrier_offer&start_ts={2}&end_ts={3}&tz=Europe/Belgrade", nid, campaignID, this.DateToUnix(start), this.DateToUnix(endDate));
			dynamic json = this.Request(url);
			if (json == null)
				yield break;

			foreach (dynamic row in json)
			{
				var entry = new AfflowCampaignMobileOperatorOfferMap()
				{
					Keyword = row.keyword,
					Clicks = row.clicks
				};
				entry.Prepare();
				yield return entry;
			}

			yield break;
		}
		public void SetSpending(int nid, string campaignID, DateTime date, double amount)
		{
			string url = string.Format("https://api.monetizer.co/data/partner-spend.php?action=set");
			string dateString = string.Format("{0}-{1}-{2}", date.Year, date.Month, date.Day);
			this.Post(url, string.Format("&nid={0}&rcid={1}&date={2}&amount={3}", nid, campaignID, dateString, amount));
		}
	}
}
