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
		public Core.TrafficSourceBase Manager { get; set; } = null;
		public string TOKEN { get; protected set; } = string.Empty;
		public string HeaderInformation { get; protected set; } = string.Empty;

		public TrafficNetworkApiBase(string headerInformations, string token)
		{
			this.TOKEN = token;
			this.HeaderInformation = headerInformations;
		}

		public virtual List<TrafficCampaign> GetCampaignsList()
		{
			List<TrafficCampaign> result = new List<TrafficCampaign>();
			foreach (TrafficCampaign c in this.GetCampaigns())
				result.Add(c);
			return result;
		}

		public abstract IEnumerable<TrafficCampaign> GetCampaigns();
		public abstract TrafficCurrentStats GetCurrentStats(TrafficCampaign campaign);
		public abstract TrafficCampaignData ReloadCampaign(TrafficCampaign campaign);
		public abstract bool StartCampaign(string campaignID);
		public abstract void StartCampaign(TrafficCampaign campaign);
		public abstract bool StopCampaign(string campaignID);
		public abstract bool StopCampaign(TrafficCampaign campaign);
		public abstract bool Bid(TrafficCampaign campaign);
		public abstract void StopTargets(TrafficCampaign campaign, List<TrafficTargetsInfo> tagets);
		public abstract void ResumeTargets(TrafficCampaign campaign, List<TrafficTargetsInfo> tagets);
		public abstract TrafficSourceTodayStats GetTodayStats();
		public abstract IEnumerable<TrafficSourceTodayStats> GetCampaignTodayStats();

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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return null;
			}
		}
	}
}
