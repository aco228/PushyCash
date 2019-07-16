using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using PushyCash.Web.BackupDeamon.App_Start;
using PushyCash.Web.BackupDeamon.Models;
using System.Threading;
using PushyCash.Direct;
using Direct.Core;

namespace PushyCash.Web.BackupDeamon
{
	public class BackupWebApplication : System.Web.HttpApplication
	{
		public static bool IsDeamonLive = false;
		public static DateTime? LastDeamonCall = null;
		public static List<LogModel> Logs = new List<LogModel>();
		
		protected void Application_Start()
		{
			GlobalConfiguration.Configure(WebApiConfig.Register);
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			PushyContext.TrafficNetworkManager = new TrafficNetworkManager();
			Log("Application started");

			new Thread(() =>
			{
				for (; ; )
				{
					if (LastDeamonCall.HasValue && IsDeamonLive && (DateTime.Now - LastDeamonCall.Value).TotalSeconds >= 90)
					{
						Log("Deamon is not active anymore");
						IsDeamonLive = false;
						OnDeamonTimeout();
					}
					if (!IsDeamonLive && LastDeamonCall.HasValue && (DateTime.Now - LastDeamonCall.Value).TotalSeconds < 15)
					{
						Log("Deamon is ACTIVE");
						IsDeamonLive = true;
					}
					Thread.Sleep(1000);
				}
			}).Start();
		}


		private static void OnDeamonTimeout()
		{
			PushyCashDirect db = PushyCashDirect.Instance;
			DirectContainer dc = db.LoadContainer("SELECT * FROM [].ActiveCampaigns");
			if (dc == null || dc.RowsCount == 0)
				return;

			foreach (DirectContainerRow row in dc.Rows)
			{
				if (!PushyContext.TrafficNetworkManager.Networks.ContainsKey(row.GetString("TrafficNetwork")))
				{
					Log($"There is no {row.GetString("TrafficNetwork")}.{row.GetString("CampaignID")}");
					continue;
				}

				Log($"Turning off {row.GetString("TrafficNetwork")}.{row.GetString("CampaignID")}");
				PushyContext.TrafficNetworkManager.Networks[row.GetString("TrafficNetwork")].API.StopCampaign(row.GetString("CampaignID"));
				db.Execute("DELETE FROM [].ActiveCampaigns WHERE ActiveCampaignsID={0}", row.GetString("ActiveCampaignsID"));
			}
		}

		public static void Log(string text)
		{
			PushyCashDirect.Instance.Execute("INSERT INTO [].Log (Level, Text)", "BACKUP_DEAMON", text);
			Logs.Insert(0, new LogModel() { Text = text });
			if (Logs.Count > 100)
				for (int i = 100; i < Logs.Count - 1; i++)
					Logs.RemoveAt(i);
		}
	}
}
