using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PushyCash.Links
{
	public class LinkManager
	{
		private static object countryVerticalLink_lockObj = new object();
		public Dictionary<string, List<Link>> CountryVerticalLinkMapCache { get; protected set; } = new Dictionary<string, List<Link>>(); /* ("{0}_{1}_{2}", country, vertical, device) */
		public Dictionary<string, Link> Links { get; protected set; } = new Dictionary<string, Link>(); //* _US.T-MOBILE.mainstream.android *//
		public Link this[string key] { get { return this.Links.ContainsKey(key) ? this.Links[key] : null; } }
		public Action OnLoadFinished = null;

		public LinkManager()
		{ }

		public void Update()
		{
			new Thread(() =>
			{
				FileInfo info = PushyExternalStorage.GetFileInfo(@"afflow\links");
				List<Afflow.Models.AfflowLink> links = null;

				if (info.Exists)
					links = PushyExternalStorage.ReadFromBinaryFile<List<Afflow.Models.AfflowLink>>(info.FullName);

				if (links == null || links.Count == 0)
				{
					links = PushyContext.AfflowManager.GetLinkList();
					PushyExternalStorage.WriteToBinaryFile<List<Afflow.Models.AfflowLink>>(info.FullName, links);
				}

				foreach (Afflow.Models.AfflowLink link in links)
				{
					if (!this.Links.ContainsKey(link.Key))
						this.Links.Add(link.Key, new Link(link));
					else
						this.Links[link.Key].Add(link);
				}

				this.OnLoadFinished?.Invoke();
				PushyContext.OnLoadFinished("LinkManager");
			}).Start();
		}

		public List<Link> GetLinks(string country, string vertical, string device)
		{
			string key = string.Format("{0}_{1}_{2}", country, vertical, device);
			if (this.CountryVerticalLinkMapCache.ContainsKey(key))
				return CountryVerticalLinkMapCache[key];

			List<Link> result = new List<Link>();
			foreach (KeyValuePair<string, Link> l in Links)
				if (l.Value.Country.Equals(country) && l.Value.Vertical.Equals(vertical) && l.Value.Device.Equals(device))
					lock (countryVerticalLink_lockObj)
						result.Add(l.Value);

			if (result.Count > 0)
				lock (countryVerticalLink_lockObj)
					this.CountryVerticalLinkMapCache.Add(key, result);

			return result;
		}

		public List<LinkDisplay> GetDisplayData()
		{
			List<LinkDisplay> result = new List<LinkDisplay>();
			List<PushyCash.Web.Core.Models.LiveFeedTableWebModel> resultForSend = new List<Web.Core.Models.LiveFeedTableWebModel>();

			lock (countryVerticalLink_lockObj)
				foreach (KeyValuePair<string, List<Link>> dict in PushyContext.LinkManager.CountryVerticalLinkMapCache)
					foreach (Link link in dict.Value)
					{
						if (link.CurrentConversions == 0 && link.CurrentPayout == 0.0)
							continue;

						if (link.TimeoutAfterCampaign.HasValue && (link.TimeoutAfterCampaign > DateTime.Now)) // iskuliraj one linkove koje su u timeout-u zbog prethodnog pustanja kampanja
							continue;

						LinkDisplay display = new LinkDisplay(link);

						display.OneMinuteConversions = link.OfferConversions.OneMinuteConversions.ToString();
						display.OneMinuteAvaragePayout = Math.Round(link.OfferConversions.AvarageOneMinutePayout, 2).ToString();
						display.TwoMinuteConversions = link.OfferConversions.TwoMinuteConversions.ToString();
						display.TwoMinuteAvaragePayout = Math.Round(link.OfferConversions.AvarageTwoMinutePayout, 2).ToString();
						display.ThreeMinuteConversions = link.OfferConversions.ThreeMinuteConversions.ToString();
						display.ThreeMinuteAvaragePayout = Math.Round(link.OfferConversions.AvarageThreeMinutePayout, 2).ToString();

						display.TotalConversions = link.TotalConversions.ToString();
						display.TotalPayout = link.TotalPayout.ToString();
						display.Timeout = string.Format("{0}m", link.TrackMinutes);
						display.LastConversions = string.Format("{0}s", ((int)(DateTime.Now - link.LastConversion).TotalSeconds).ToString());
						display.SetConversions(link.CurrentConversions);
						
						var entryForSend = display.Copy();
						entryForSend.HasActiveCampaing = link.HasActiveCampaign;
						entryForSend.IsLocked = link.IsLocked;
						resultForSend.Add(entryForSend);

						result.Add(display);
					}

			result = result.OrderBy(x => x.GetConversions() * -1).Take(5).ToList();
			resultForSend = resultForSend.OrderBy(x => x.Conversions * -1).Take(15).ToList();
			Web.Core.PushyWebSender.Send<List<Web.Core.Models.LiveFeedTableWebModel>>("LiveFeedTable", resultForSend);

			return result.Take(10).ToList();
		}

		// SUMMARY: Get all data prepared for printing into console application
		public List<LinkDisplayOld> GetDisplayDataOld()
		{
			List<LinkDisplayOld> result = new List<LinkDisplayOld>();
			List<PushyCash.Web.Core.Models.LiveFeedTableWebModelOld> resultForSend = new List<Web.Core.Models.LiveFeedTableWebModelOld>();

			lock (countryVerticalLink_lockObj)
				foreach (KeyValuePair<string, List<Link>> dict in PushyContext.LinkManager.CountryVerticalLinkMapCache)
					foreach (Link link in dict.Value)
					{
						if (link.CurrentConversions == 0 && link.CurrentPayout == 0.0)
							continue;

						LinkDisplayOld display = new LinkDisplayOld(link);

						display.Conversions = string.Format("{0}/{1}", link.CurrentConversions, link.TrackConversions);
						display.Payout = string.Format("{0}/{1}", link.CurrentPayout, link.TrackPayout);
						display.AvaragePayout = Math.Round(link.AvaragePayout, 2).ToString();
						display.TotalConversions = link.TotalConversions.ToString();
						display.TotalPayout = link.TotalPayout.ToString();
						display.Timeout = string.Format("{0}m", link.TrackMinutes);
						display.LastConversions = string.Format("{0}s", ((int)(DateTime.Now - link.LastConversion).TotalSeconds).ToString());

						// Prepare procentige
						//int conversionsForProcentige = (link.CurrentConversions > link.TrackConversions ? link.TrackConversions : link.CurrentConversions);
						//double payoutForProcentige = (link.CurrentPayout > link.TrackPayout ? link.TrackPayout : link.CurrentPayout);
						//double procentigeForConversions = (conversionsForProcentige * 1.0) / (link.TrackConversions * 1.0) * 100.0,
						//			 procentigeForPayout = (payoutForProcentige * 1.0) / (link.TrackPayout * 1.0) * 100.0;
						//double procentige  = Math.Round((procentigeForConversions + procentigeForPayout) / 2, 2);
						double procentige = Math.Round(((link.CurrentConversions * 1.0) / (link.TrackConversions * 1.0)) * 100, 2);
						if (procentige > 100)
							procentige = 100;

						display.SetProcentige(procentige);
						display.Procentige = string.Format("{0}%", procentige);

						var entryForSend = display.Copy();
						entryForSend.HasActiveCampaing = link.HasActiveCampaign;
						entryForSend.IsLocked = link.IsLocked;
						resultForSend.Add(entryForSend);

						result.Add(display);
					}

			result = result.OrderBy(x => x.GetProcentige() * -1).Take(10).ToList();
			resultForSend = resultForSend.OrderBy(x => x.Procentige * -1).Take(15).ToList();
			Web.Core.PushyWebSender.Send<List<Web.Core.Models.LiveFeedTableWebModelOld>>("LiveFeedTable", resultForSend);

			return result.Take(10).ToList();
		}

		#region # Mapping #

		public void MapDataWithSnapshot()
		{
			if (PushyContext.SnapshotManager == null)
				return;

			foreach (KeyValuePair<string, Links.Link> link in this.Links)
			{
				link.Value.MapSnapshotOffers(PushyContext.SnapshotManager.GetOffersDictonary(link.Value.Vertical, link.Value.Country, link.Value.AfflowMobileOperator));
			}
		}

		public void MapDataWithTrafficCampaigns()
		{
			foreach (KeyValuePair<string, Link> link in this.Links)
				foreach (TrafficNetworks.Core.TrafficSourceBase trafficSource in PushyContext.TrafficNetworkManager.NetworksList)
					if (trafficSource[link.Value.Name] != null)
					{
						if (!link.Value.AfflowLinkMap.ContainsKey(trafficSource.Name))
							continue;
						TrafficNetworks.Core.TrafficCampaign camp = trafficSource[link.Value.Name];
						camp.Afflowlink = link.Value.AfflowLinkMap[trafficSource.Name];
						link.Value.TrafficCampaigns.Add(camp);
					}
		}

		#endregion

	}
}
