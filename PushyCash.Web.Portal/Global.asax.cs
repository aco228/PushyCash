using Direct.Core;
using PushyCash.Direct;
using PushyCash.Web.Core.Models;
using PushyCash.Web.Portal.Hubs;
using PushyCash.Web.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PushyCash.Web.Portal
{
	public class PushyPortalApplication : System.Web.HttpApplication
	{
		public static DateTime? LastDeamonCall = DateTime.Now;
		public static bool IsDeamonLive = false;
		public static MainInformationsWebModel MainInformationsWebModel = new MainInformationsWebModel();
		public static List<LiveFeedTableWebModel> LiveFeedTableWebModel = new List<LiveFeedTableWebModel>();
		public static List<DebugWebModel> DebugWebModel = new List<DebugWebModel>();
		public static List<string> DeamonCommands = new List<string>();

		public static Dictionary<string, PushyCampaignModel> PushyCampaigns = new Dictionary<string, PushyCampaignModel>();
		
		protected void Application_Start()
		{
      GlobalConfiguration.Configure(WebApiConfig.Register);
      AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			PushyContext.TrafficNetworkManager = new TrafficNetworkManager();

      //dummy();

			new Thread(() =>
			{
				for(; ;)
				{
					if(LastDeamonCall.HasValue && (DateTime.Now - LastDeamonCall.Value).TotalSeconds >= 20)
					{
						IsDeamonLive = false;
						PushyCampaigns.Clear();
						MainHub.Current.OnDeamonDown();
					}
					if(!IsDeamonLive && LastDeamonCall.HasValue && (DateTime.Now - LastDeamonCall.Value).TotalSeconds < 15)
					{
						IsDeamonLive = true;
						MainHub.Current.OnDeamonIsLive();
					}
					Thread.Sleep(1000);
				}
			}).Start();
		}
		
		private static void dummy()
		{
			PushyCampaignModel model = new PushyCampaignModel();
			model.Campaign = new PushyCampaignWebModel()
			{
				LinkName = "_AT.3.mainstream.adult",
				OffersActivated = "fe9f06,961571,d48908,806790",
				NumberOfConversions = 10, TrackingMinutes = 2,
				AllowedDeficit = 4.25, RecomendedClicksForConversion = 825,
				LinkAvaragePayout = 0.85
			};
			
			model.NetworkCampaigns.Add("zeropark", new PushyNetworkCampaignModel() {
				Campaign = new PushyNetworkCampaignWebModel()
				{
					LinkName = "_AT.3.mainstream.adult", Name = "zeropark",
					NetworkName = "zeropark", Budget = 4.25, ToleratedDeficit = 1.2,
					ClicksForConversion = 900, Started = DateTime.Now, IsPaused = false, IsStoped = false,
					Ended = null,

					Conversions = 1, Clicks = 8, Payout = 2.2, Cost = 1.2, LastBid = 0.00001
				} });

			
			model.NetworkCampaigns.Add("popads", new PushyNetworkCampaignModel() {
				Campaign = new PushyNetworkCampaignWebModel()
				{
					LinkName = "_AT.3.mainstream.adult", Name = "popads",
					NetworkName = "popads", Budget = 8.25, ToleratedDeficit = 2.2,
					ClicksForConversion = 425, Started = DateTime.Now, IsPaused = true, IsStoped = false,
					Ended = null,

					Conversions = 1, Clicks = 8, Payout = 2.2, Cost = 1.2, LastBid = 0.00001
				} });
			
			model.NetworkCampaigns.Add("exoclick", new PushyNetworkCampaignModel() {
				Campaign = new PushyNetworkCampaignWebModel()
				{
					LinkName = "_AT.3.mainstream.adult", Name = "exoclick",
					NetworkName = "exoclick", Budget = 8.25, ToleratedDeficit = 2.2,
					ClicksForConversion = 425, Started = DateTime.Now, IsPaused = false, IsStoped = true,
					Ended = null,

					Conversions = 1, Clicks = 8, Payout = 2.2, Cost = 1.2, LastBid = 0.00001
				} });

			PushyCampaigns.Add("1", model);



			PushyCampaignModel model2 = new PushyCampaignModel();
			model2.Campaign = new PushyCampaignWebModel()
			{
				LinkName = "_DE.o2.adult.android",
				OffersActivated = "fe9f06,961571,d48908,806790",
				NumberOfConversions = 10,
				TrackingMinutes = 2,
				AllowedDeficit = 4.25,
				RecomendedClicksForConversion = 825,
				LinkAvaragePayout = 0.85
			};


			model2.NetworkCampaigns.Add("zeropark", new PushyNetworkCampaignModel()
			{
				Campaign = new PushyNetworkCampaignWebModel()
				{
					LinkName = "_DE.o2.adult.android",
					Name = "zeropark",
					NetworkName = "zeropark",
					Budget = 4.25,
					ToleratedDeficit = 1.2,
					ClicksForConversion = 900,
					Started = DateTime.Now,
					IsPaused = false,
					IsStoped = false,
					Ended = DateTime.Now,

					Conversions = 1,
					Clicks = 8,
					Payout = 2.2,
					Cost = 1.2,
					LastBid = 0.00001
				}
			});

			PushyCampaigns.Add("2", model2);
		}

	}
}
