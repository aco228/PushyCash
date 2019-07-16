using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Core
{
	public abstract class TrafficNetworkApiBase
	{
		protected object lock_obj = new object();
		public Core.TrafficSourceBase Manager { get; set; } = null;
		public string TOKEN { get; protected set; } = string.Empty;
		public string HeaderInformation { get; protected set; } = string.Empty;

		public TrafficNetworkApiBase(string headerInformations, string token)
		{
			this.TOKEN = token;
			this.HeaderInformation = headerInformations;
		}
		
		/// CAMPAIGN LISTS
		public abstract IEnumerable<TrafficCampaign> GetCampaigns();
		public virtual List<TrafficCampaign> GetCampaignsList()
		{
			List<TrafficCampaign> result = new List<TrafficCampaign>();
			foreach (TrafficCampaign c in this.GetCampaigns())
				result.Add(c);
			return result;
		}

		/// CAMPAIGNS
		public abstract bool StartCampaign(string campaignID);
		public abstract void StartCampaign(TrafficCampaign campaign);
		public abstract bool StopCampaign(string campaignID);
		public abstract bool StopCampaign(TrafficCampaign campaign);
		public abstract TrafficCampaignData ReloadCampaign(TrafficCampaign campaign);
		public abstract TrafficCurrentStats GetCurrentStats(TrafficCampaign campaign);

		/// BIDS
		public abstract bool Bid(TrafficCampaign campaign);

		/// TARGETS BLACLIST
		public virtual void StopTargets(TrafficCampaign campaign, List<TrafficTargetsInfo> tagets) => StopTargets(campaign.ID, tagets);
		public virtual void ResumeTargets(TrafficCampaign campaign, List<TrafficTargetsInfo> tagets) => ResumeTargets(campaign.ID, tagets);
		public virtual void StopTarget(string campaignID, string targetName) => this.StopTargets(campaignID, new List<TrafficTargetsInfo>() { new TrafficTargetsInfo() { Name = targetName } });
		public virtual void ResumeTarget(string campaignID, string targetName) => this.ResumeTargets(campaignID, new List<TrafficTargetsInfo>(){ new TrafficTargetsInfo(){ Name = targetName }});

		public abstract List<TrafficTargeStatInfo> GetTargetsInfo(string campaignID, DateTime startDate, DateTime endDate);
		public abstract void StopTargets(string campaignID, List<TrafficTargetsInfo> tagets);
		public abstract void ResumeTargets(string campaignID, List<TrafficTargetsInfo> tagets);

		/// STATS
		public abstract TrafficSourceTodayStats GetTodayStats();
		public abstract IEnumerable<TrafficSourceTodayStats> GetCampaignTodayStats();
		public abstract IEnumerable<TrafficSourceTodayStats> GetStats(DateTime? startDate, DateTime? endDate);
		
		/// 
		//	WEB METHODS

		protected dynamic Get(string requestUrl)
		{
			try
			{
				var webRequest = System.Net.WebRequest.Create(requestUrl);
				if (webRequest == null)
					return null;

				webRequest.Method = "GET";
				webRequest.Timeout = 12000;
				webRequest.ContentType = "application/json";
				if(!string.IsNullOrEmpty(HeaderInformation))
					webRequest.Headers.Add(HeaderInformation, TOKEN);

				lock(this.lock_obj)
				{
					using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
					{
						using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
						{
							var jsonResponse = sr.ReadToEnd();
							dynamic json = JsonConvert.DeserializeObject(jsonResponse);

							//Console.WriteLine(String.Format("Response: {0}", jsonResponse));
							return json;
						}
					}
				}
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		protected dynamic Post(string requestUrl, string data = "")
		{
			try
			{
				var webRequest = System.Net.WebRequest.Create(requestUrl);
				if (webRequest == null)
					return null;

				webRequest.Method = "POST";
				webRequest.Timeout = 12000;
				webRequest.ContentType = "application/json";
				webRequest.ContentType = "application/x-www-form-urlencoded";
				if (!string.IsNullOrEmpty(HeaderInformation) && !string.IsNullOrEmpty(this.TOKEN))
					webRequest.Headers.Add(HeaderInformation, TOKEN);

				if(!string.IsNullOrEmpty(data))
				{
					string postData = data;
					ASCIIEncoding encoding = new ASCIIEncoding();
					byte[] byte1 = encoding.GetBytes(postData);
					webRequest.ContentLength = byte1.Length;
					Stream newStream = webRequest.GetRequestStream();
					newStream.Write(byte1, 0, byte1.Length);
				}

				lock(this.lock_obj)
				{
					using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
					using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
					{
						var jsonResponse = sr.ReadToEnd();
						if (jsonResponse == "[]")
							return null;

						dynamic json = JsonConvert.DeserializeObject(jsonResponse);

						//Console.WriteLine(String.Format("Response: {0}", jsonResponse));
						return json;
					}
				}
			}
			catch (Exception ex)
			{
				//Console.WriteLine(ex.ToString());
				return null;
			}
		}
		protected dynamic Put(string requestUrl, string data)
		{
			try
			{
				var webRequest = System.Net.WebRequest.Create(requestUrl);
				if (webRequest == null)
					return null;

				webRequest.Method = "PUT";
				webRequest.Timeout = 12000;
				webRequest.ContentType = "application/json";
				webRequest.ContentType = "application/json";
				if (!string.IsNullOrEmpty(HeaderInformation) && !string.IsNullOrEmpty(this.TOKEN))
					webRequest.Headers.Add(HeaderInformation, TOKEN);

				string postData = data;
				ASCIIEncoding encoding = new ASCIIEncoding();
				byte[] byte1 = encoding.GetBytes(postData);
				webRequest.ContentLength = byte1.Length;
				Stream newStream = webRequest.GetRequestStream();
				newStream.Write(byte1, 0, byte1.Length);
				
				lock(this.lock_obj)
				{
					using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
					using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
					{
						var jsonResponse = sr.ReadToEnd();
						if (jsonResponse == "[]")
							return null;

						dynamic json = JsonConvert.DeserializeObject(jsonResponse);

						//Console.WriteLine(String.Format("Response: {0}", jsonResponse));
						return json;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return null;
			}
		}
		
		protected long DateToUnix(DateTime foo)
		{
			return ((DateTimeOffset)foo).ToUnixTimeSeconds();
		}
	}
}
