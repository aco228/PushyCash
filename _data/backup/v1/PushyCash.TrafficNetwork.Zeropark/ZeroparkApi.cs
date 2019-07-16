using PushyCash.TrafficNetworks.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Zeropark
{
	public class ZeroparkApi : TrafficNetworkApiBase
	{
		public ZeroparkApi()
			: base("api-token", "AAABY/OP6qaael5fcIsRr0raugvfjNofnx/ZaDRrWhRskhyOiMkOsc9PM3Nh1+M4NNlfKDGw9w5bMKczYNeFEQ==")
		{
		}
		
		public override IEnumerable<TrafficCampaign> GetCampaigns()
		{
			string url = "https://panel.zeropark.com/api/stats/campaign/all?oauth_token=AAABY/OP6qaael5fcIsRr0raugvfjNofnx/ZaDRrWhRskhyOiMkOsc9PM3Nh1+M4NNlfKDGw9w5bMKczYNeFEQ==&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1528806323&oauth_nonce=4sm5f2&oauth_version=1.0&oauth_signature=99vZQw/9Ml+lbqNMHQnXOVvJBQw=&interval=TODAY&page=0&limit=1000&sortColumn=SPENT&sortOrder=DESC";
			dynamic result = this.Get(url);

			try { result = result.elements; }
			catch (Exception e) { yield break; }

			foreach (dynamic row in result)
			{
				TrafficCampaignData data = this.Manager.ConvertJson(row);
				if (data == null)
					continue;

				ZeroparkCampaign camp = new ZeroparkCampaign(data);
				switch (data.Status)
				{
					case "ACTIVE": camp.Status = TrafficCampaignStatus.Active; break;
					case "PAUSED": camp.Status = TrafficCampaignStatus.Paused; break;
				}
				
				yield return camp;
			}

			yield break;
		}
		public override bool StartCampaign(string campaignID)
		{
			string url = string.Format("https://panel.zeropark.com/api/campaign/{0}/resume", campaignID);
			return this.Post(url) != null;
		}
		public override void StartCampaign(TrafficCampaign campaign)
		{
			ZeroparkCampaign camp = campaign as ZeroparkCampaign;
			if (camp == null)
				return;

			string url = string.Format("https://panel.zeropark.com/api/campaign/{0}/resume", camp.ID);
			this.Post(url);
		}
		public override bool StopCampaign(TrafficCampaign campaign)
		{
			for (; ; )
			{
				dynamic result = this.Post(string.Format("https://panel.zeropark.com/api/campaign/{0}/pause", campaign.ID));
				if (result != null)
					return true;
				System.Threading.Thread.Sleep(250);
			}
		}
		public override bool StopCampaign(string campaignID)
		{
			for (; ; )
			{
				dynamic result = this.Post(string.Format("https://panel.zeropark.com/api/campaign/{0}/pause", campaignID));
				if (result != null)
					return true;
				System.Threading.Thread.Sleep(250);
			}
		}
		public override TrafficCurrentStats GetCurrentStats(TrafficCampaign campaign)
		{
			ZeroparkCampaign camp = campaign as ZeroparkCampaign;
			if (camp == null)
				return null;

			string url = string.Format("https://panel.zeropark.com/api/stats/campaign/interval/{0}?interval=TODAY", camp.ID);
			dynamic json = this.Get(url);
			if (json == null || json.ToString().Equals("[]"))
				return null;

			return new TrafficCurrentStats()
			{
				Spent = json.summary.spent,
				Payout = json.summary.payout,
				Clicks = json.summary.redirects,
				Conversions = json.summary.conversions
			};
		}
		public override bool Bid(TrafficCampaign campaign)
		{
			ZeroparkCampaign camp = campaign as ZeroparkCampaign;
			if (camp == null)
				return false;

			string url = string.Format("https://panel.zeropark.com/api/campaign/{0}/bid?bid={1}", camp.ID, Math.Round(camp.BidValue, 4).ToString());
			return this.Post(url) != null;
		}
		public override TrafficCampaignData ReloadCampaign(TrafficCampaign campaign)
		{
			string url = string.Format("https://panel.zeropark.com/api/stats/campaign/all?oauth_token=AAABY/OP6qaael5fcIsRr0raugvfjNofnx/ZaDRrWhRskhyOiMkOsc9PM3Nh1+M4NNlfKDGw9w5bMKczYNeFEQ==&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1528806323&oauth_nonce=4sm5f2&oauth_version=1.0&oauth_signature=99vZQw/9Ml+lbqNMHQnXOVvJBQw=&interval=TODAY&page=0&limit=1000&sortColumn=SPENT&sortOrder=DESC&campaignNameOrId={0}", campaign.ID);
			dynamic result = this.Get(url);
			try { result = result.elements; }
			catch (Exception e) { return null; }

			foreach (dynamic row in result)
				return this.Manager.ConvertJson(row);

			return null;
		}
		public override void StopTargets(TrafficCampaign campaign, List<TrafficTargetsInfo> tagets)
		{
			ZeroparkCampaign camp = campaign as ZeroparkCampaign;
			if (camp == null || tagets == null || tagets.Count == 0)
				return;

			string targets = "";
			foreach (var t in tagets)
				targets = (targets != "" ? "," : "") + t.Name;

			string url = string.Format("https://panel.zeropark.com/api/campaign/{0}/targets/pause?hash={1}", camp.ID, targets);
			this.Post(url);
		}
		public override void ResumeTargets(TrafficCampaign campaign, List<TrafficTargetsInfo> tagets)
		{
			ZeroparkCampaign camp = campaign as ZeroparkCampaign;
			if (camp == null || tagets == null || tagets.Count == 0)
				return;

			string targets = "";
			foreach (var t in tagets)
				targets = (targets != "" ? "," : "") + t.Name;

			string url = string.Format("https://panel.zeropark.com/api/campaign/{0}/targets/resume?hash={1}", camp.ID, targets);
			this.Post(url);
		}
		public override TrafficSourceTodayStats GetTodayStats()
		{
			string url = "https://panel.zeropark.com/api/stats/campaign/all?interval=TODAY&page=0&limit=10000&sortColumn=SPENT&sortOrder=DESC";
			dynamic json = this.Get(url), stats;
			try { stats = json.summary; } catch (Exception e) { return null; }

			TrafficSourceTodayStats result = new TrafficSourceTodayStats();
			result.Spent = json.summary.spent;
			result.Redirects = json.summary.redirects;
			result.Payout = json.summary.payout;
			result.Conversions = json.summary.conversions;
			return result;
		}
		public override IEnumerable<TrafficSourceTodayStats> GetCampaignTodayStats()
		{
			string url = "https://panel.zeropark.com/api/stats/campaign/all?interval=TODAY&page=0&limit=10000&sortColumn=SPENT&sortOrder=DESC";
			dynamic json = this.Get(url);
			if (json == null)
				yield break;

			foreach(dynamic row in json.elements)
			{
				string name = row.details.name.ToString();
				if (name[0] != '_')
					continue;

				yield return new TrafficSourceTodayStats()
				{
					CampaignID = row.details.id.ToString(),
					Redirects = row.stats.redirects,
					Spent = row.stats.spent,
					Conversions = row.stats.conversions
				};
			}

			yield break;
		}

	}
}
