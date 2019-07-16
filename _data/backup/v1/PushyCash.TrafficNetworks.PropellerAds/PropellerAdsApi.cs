using PushyCash.TrafficNetworks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.PropellerAds
{
	public class PropellerAdsApi : TrafficNetworkApiBase
	{
		public DateTime ApiKeyExpires = DateTime.Now.AddSeconds(-30);

		public PropellerAdsApi() 
			: base("Authorization", "")
		{

		}

		private void TryToLogin()
		{
			if (this.ApiKeyExpires >= DateTime.Now)
				return;

			this.TOKEN = string.Empty;
			dynamic json = this.Post("https://ssp-api.propellerads.com/v5/adv/login?username=mediamonkeys&password=52JEL72Cs:1^6WN");
			if (json == null)
				return;

			this.TOKEN = "Bearer " + json.api_token;
			this.ApiKeyExpires.AddMinutes(10);
		}
		public override IEnumerable<TrafficCampaign> GetCampaigns()
		{
			this.TryToLogin();
			dynamic json = this.Get("https://ssp-api.propellerads.com/v5/adv/campaigns?page_size=1000&is_archived=0");
			if (json == null)
				yield break;
			
			foreach(var row in json.result)
			{
				TrafficCampaignData data = this.Manager.ConvertJson(row);
				if (data == null)
					continue;

				PropellerAdsCampaign campaign = new PropellerAdsCampaign(data);

				yield return campaign;
			}

			yield break;
		}
		public override bool StartCampaign(string campaignID)
		{
			return this.Put("https://ssp-api.propellerads.com/v5/adv/campaigns/play", "{ \"campaign_ids\": [" + campaignID + "] }") != null;
		}
		public override void StartCampaign(TrafficCampaign campaign)
		{
			for(; ; )
			{
				dynamic result = this.Put("https://ssp-api.propellerads.com/v5/adv/campaigns/play", "{ \"campaign_ids\": ["+campaign.ID+"] }");
				if (result != null)
					return;
				System.Threading.Thread.Sleep(250);
			}
		}
		public override bool StopCampaign(TrafficCampaign campaign)
		{
			for (; ; )
			{
				dynamic result = this.Put("https://ssp-api.propellerads.com/v5/adv/campaigns/stop", "{ \"campaign_ids\": [" + campaign.ID + "] }");
				if (result != null)
					return true;
				System.Threading.Thread.Sleep(250);
			}
		}
		public override bool StopCampaign(string campaignID)
		{
			for (; ; )
			{
				dynamic result = this.Put("https://ssp-api.propellerads.com/v5/adv/campaigns/stop", "{ \"campaign_ids\": [" + campaignID + "] }");
				if (result != null)
					return true;
				System.Threading.Thread.Sleep(250);
			}
		}
		public override bool Bid(TrafficCampaign campaign)
		{
			return true;
		}
		public override TrafficSourceTodayStats GetTodayStats()
		{
			this.TryToLogin();
			DateTime today = DateTime.Today; DateTime tomorrow = today.AddDays(1);
			dynamic json = this.Get($"https://ssp-api.propellerads.com/v5/adv/statistics/campaigns?date_from={today.Year}-{today.Month}-{today.Day}&date_to={tomorrow.Year}-{tomorrow.Month}-{tomorrow.Day}&page_size=10000");
			if (json == null)
				return null;

			TrafficSourceTodayStats result = new TrafficSourceTodayStats();
			foreach (dynamic row in json.result)
			{
				result.Conversions += int.Parse(row.conversions.ToString());
				result.Redirects += int.Parse(row.impressions.ToString());
				result.Spent += double.Parse(row.money.ToString());
			}

			return result;
		}
		public override IEnumerable<TrafficSourceTodayStats> GetCampaignTodayStats()
		{
			this.TryToLogin();
			DateTime today = DateTime.Today; DateTime tomorrow = today.AddDays(1);
			dynamic json = this.Get($"https://ssp-api.propellerads.com/v5/adv/statistics/campaigns?date_from={today.Year}-{today.Month}-{today.Day}&date_to={tomorrow.Year}-{tomorrow.Month}-{tomorrow.Day}&page_size=10000");
			if (json == null)
				yield break;

			TrafficSourceTodayStats result = new TrafficSourceTodayStats();
			foreach (dynamic row in json.result)
				yield return new TrafficSourceTodayStats()
				{
					CampaignID = row.campaign_id.ToString(),
					Conversions = int.Parse(row.conversions.ToString()),
					Redirects = int.Parse(row.impressions.ToString()),
					Spent = double.Parse(row.money.ToString())
				};

			yield break;
		}
		public override TrafficCurrentStats GetCurrentStats(TrafficCampaign campaign)
		{
			this.TryToLogin();
			DateTime today = DateTime.Today; DateTime tomorrow = today.AddDays(2);
			dynamic json = this.Get($"https://ssp-api.propellerads.com/v5/adv/statistics/campaigns?date_from={today.Year}-{today.Month}-{today.Day}&date_to={tomorrow.Year}-{tomorrow.Month}-{tomorrow.Day}&page_size=10000&campaign_id[]=" + campaign.ID);
			if (json == null)
				return null;

			if (json.result.ToString().Equals("[]"))
				return new TrafficCurrentStats();

			TrafficCurrentStats result = new TrafficCurrentStats()
			{
				Clicks = int.Parse(json.result[0].impressions.ToString()),
				Conversions = int.Parse(json.result[0].conversions.ToString()),
				Payout = 0.0,
				Spent = double.Parse(json.result[0].money.ToString())
			};
			return result;
		}
		public override TrafficCampaignData ReloadCampaign(TrafficCampaign campaign)
		{
			this.TryToLogin();
			dynamic json = this.Get("https://ssp-api.propellerads.com/v5/adv/campaigns?page_size=1000&id[]=" + campaign.ID);
			if (json == null)
				return null;

			foreach (dynamic row in json.result)
				return this.Manager.ConvertJson(row);

			return null;
		}
		public override void ResumeTargets(TrafficCampaign campaign, List<TrafficTargetsInfo> tagets)
		{
			this.TryToLogin();
			string zones = "";
			foreach (var t in tagets) zones += (!zones.Equals("") ? "," : "") + "\"" + t.Name + "\"";
			this.Put($"https://ssp-api.propellerads.com/v5/adv/campaigns/{campaign.ID}/targeting/exclude/zone", "{ \"zone\": [ "+ zones +" ] }");
		}
		public override void StopTargets(TrafficCampaign campaign, List<TrafficTargetsInfo> tagets)
		{
			this.TryToLogin();
			string zones = "";
			foreach (var t in tagets) zones += (!zones.Equals("") ? "," : "") + "\"" + t.Name + "\"";
			this.Put($"https://ssp-api.propellerads.com/v5/adv/campaigns/{campaign.ID}/targeting/include/zone", "{ \"zone\": [ " + zones + " ] }");
		}


		// network specific

		public TrafficSourceTodayStats GetTodayStatsFiltered(List<TrafficCampaign> listOfCampaigns)
		{
			this.TryToLogin();
			DateTime today = DateTime.Today; DateTime tomorrow = today.AddDays(1);
			string campaignIds = "";
			foreach (var c in listOfCampaigns) campaignIds += "&campaign_id[]=" + c.ID;

			dynamic json = this.Get($"https://ssp-api.propellerads.com/v5/adv/statistics/campaigns?date_from={today.Year}-{today.Month}-{today.Day}&date_to={tomorrow.Year}-{tomorrow.Month}-{tomorrow.Day}&page_size=10000" + campaignIds);
			if (json == null)
				return null;

			TrafficSourceTodayStats result = new TrafficSourceTodayStats();
			foreach (dynamic row in json.result)
			{
				result.Conversions += int.Parse(row.conversions.ToString());
				result.Redirects += int.Parse(row.impressions.ToString());
				result.Spent += double.Parse(row.money.ToString());
			}

			return result;
		}

		public IEnumerable<TrafficSourceTodayStats> GetCampaignTodayStatsFiltered(List<TrafficCampaign> listOfCampaigns)
		{
			this.TryToLogin();
			DateTime today = DateTime.Today; DateTime tomorrow = today.AddDays(1);
			string campaignIds = "";
			foreach (var c in listOfCampaigns) campaignIds += "&campaign_id[]=" + c.ID;

			dynamic json = this.Get($"https://ssp-api.propellerads.com/v5/adv/statistics/campaigns?date_from={today.Year}-{today.Month}-{today.Day}&date_to={tomorrow.Year}-{tomorrow.Month}-{tomorrow.Day}&page_size=10000" + campaignIds);
			if (json == null)
				yield break;

			TrafficSourceTodayStats result = new TrafficSourceTodayStats();
			foreach (dynamic row in json.result)
				yield return new TrafficSourceTodayStats()
				{
					CampaignID = row.campaign_id.ToString(),
					Conversions = int.Parse(row.conversions.ToString()),
					Redirects = int.Parse(row.impressions.ToString()),
					Spent = double.Parse(row.money.ToString())
				};

			yield break;
		}


	}
}
