using PushyCash.Web.Core.Models;
using PushyCash.Web.Core.Models.Campaign;
using PushyCash.Web.Portal.Hubs;
using PushyCash.Web.Portal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PushyCash.Web.Portal.Controllers
{
	public class DeamonCallbackController : PushyController
	{

		public ActionResult MainInformations()
		{
			MainInformationsWebModel data = this.GetData<MainInformationsWebModel>(Request);
			if (data == null)
				return new HttpStatusCodeResult(500);

			PushyPortalApplication.MainInformationsWebModel = data;
			MainHub.Current.OnMainInformations(data);
			return new HttpStatusCodeResult(200);
		}
		public ActionResult LiveFeedTable()
		{
			List<LiveFeedTableWebModel> list = this.GetData< List<LiveFeedTableWebModel>>(Request);
			if(list == null)
				return new HttpStatusCodeResult(500);

			PushyPortalApplication.LiveFeedTableWebModel = list;
			MainHub.Current.OnLiveFeedData(list);
			return new HttpStatusCodeResult(200);
		}
		public ActionResult Debug()
		{
			DebugWebModel data = this.GetData<DebugWebModel>(Request);
			if (data == null)
				return new HttpStatusCodeResult(500);

			PushyPortalApplication.DebugWebModel.Insert(0, data);
			if (PushyPortalApplication.DebugWebModel.Count > 200)
				PushyPortalApplication.DebugWebModel = PushyPortalApplication.DebugWebModel.Take(200).ToList();

			MainHub.Current.OnDebug(data);
			return new HttpStatusCodeResult(200);
		}

		/// PushyCampaigns
		/// 

		public ActionResult OnPushyCampaignStart()
		{
			PushyCampaignWebModel data = this.GetData<PushyCampaignWebModel>(Request);
			if (data == null)
				return new HttpStatusCodeResult(500);
			if (!PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
				PushyPortalApplication.PushyCampaigns.Add(data.LinkName, new Models.PushyCampaignModel() { Campaign = data });
			MainHub.Current.OnPushyCampaignStart(data);
			return new HttpStatusCodeResult(200);
		}
		public ActionResult OnPushyCampaignUpdate()
		{
			PushyCampaignWebModel data = this.GetData<PushyCampaignWebModel>(Request);
			if (data == null)
				return new HttpStatusCodeResult(500);
			if (PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
				PushyPortalApplication.PushyCampaigns[data.LinkName].Campaign = data;
			MainHub.Current.OnPushyCampaignStop(data);
			return new HttpStatusCodeResult(200);
		}
		public ActionResult OnPushyCampaignStop()
		{
			PushyCampaignWebModel data = this.GetData<PushyCampaignWebModel>(Request);
			if (data == null)
				return new HttpStatusCodeResult(500);
			if (PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
				PushyPortalApplication.PushyCampaigns.Remove(data.LinkName);
			MainHub.Current.OnPushyCampaignStop(data);
			return new HttpStatusCodeResult(200);
		}

		public ActionResult OnPushyNetworkCampaignStart()
		{
			PushyNetworkCampaignWebModel data = this.GetData<PushyNetworkCampaignWebModel>(Request);
			if (data == null || !PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
				return new HttpStatusCodeResult(500);

			PushyCampaignModel model = PushyPortalApplication.PushyCampaigns[data.LinkName];
			if (!model.NetworkCampaigns.ContainsKey(data.Name))
				model.NetworkCampaigns.Add(data.Name, new PushyNetworkCampaignModel() { Campaign = data });
			else
			{
				model.NetworkCampaigns[data.Name].Campaign = data;
				MainHub.Current.OnPushyNetworkCampaignUpdate(model.NetworkCampaigns[data.Name].Campaign);
			}

			MainHub.Current.OnPushyNetworkCampaignStart(data);
			return new HttpStatusCodeResult(200);
		}
		public ActionResult OnPushyNetworkCampaignUpdate()
		{
			PushyNetworkCampaignUpdateWebModel data = this.GetData<PushyNetworkCampaignUpdateWebModel>(Request);
			if (data == null || !PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
				return new HttpStatusCodeResult(500);

			PushyCampaignModel model = PushyPortalApplication.PushyCampaigns[data.LinkName];
			if (model.NetworkCampaigns.ContainsKey(data.PushyNetworkCampaignName))
			{
				model.NetworkCampaigns[data.PushyNetworkCampaignName].Campaign.Clicks = data.Clicks;
				model.NetworkCampaigns[data.PushyNetworkCampaignName].Campaign.Conversions = data.Conversions;
				model.NetworkCampaigns[data.PushyNetworkCampaignName].Campaign.Payout = data.Payout;
				model.NetworkCampaigns[data.PushyNetworkCampaignName].Campaign.Cost = data.Cost;
				model.NetworkCampaigns[data.PushyNetworkCampaignName].Campaign.IsPaused = data.IsPaused;
				model.NetworkCampaigns[data.PushyNetworkCampaignName].Campaign.IsStoped = data.IsStoped;
				model.NetworkCampaigns[data.PushyNetworkCampaignName].Campaign.LastBid = data.LastBid;
			}
			else
				return new HttpStatusCodeResult(500);
			
			MainHub.Current.OnPushyNetworkCampaignUpdate(model.NetworkCampaigns[data.PushyNetworkCampaignName].Campaign);
			return new HttpStatusCodeResult(200);
		}
		public ActionResult OnPushyNetworkCampaignLog()
		{
			PushyNetworkCampaignLog data = this.GetData<PushyNetworkCampaignLog>(Request);
			if (data == null || !PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
				return new HttpStatusCodeResult(500);

			PushyCampaignModel model = PushyPortalApplication.PushyCampaigns[data.LinkName];
			if (model.NetworkCampaigns.ContainsKey(data.PushyNetworkCampaignName))
				model.NetworkCampaigns[data.PushyNetworkCampaignName].Logs.Insert(0, data);
			else if (!model.NetworkCampaigns.ContainsKey(data.PushyNetworkCampaignName))
			{
				model.NetworkCampaigns.Add(data.PushyNetworkCampaignName, new PushyNetworkCampaignModel() { Campaign = new PushyNetworkCampaignWebModel() });
				MainHub.Current.OnPushyNetworkCampaignStart(model.NetworkCampaigns[data.PushyNetworkCampaignName].Campaign);
				model.NetworkCampaigns[data.PushyNetworkCampaignName].Logs.Insert(0, data);
			}

			MainHub.Current.OnPushyNetworkCampaignLog(data);
			return new HttpStatusCodeResult(200);
		}

		/// Data manipulation
		/// 

		public T GetData<T>(HttpRequestBase request)
		{
			PushyPortalApplication.LastDeamonCall = DateTime.Now;
			MemoryStream ms = new MemoryStream();
			request.InputStream.CopyTo(ms);
			byte[] data = ms.ToArray();
			return PushyCash.Web.Core.PushyWebDataConverter.BytesToObj<T>(data);
		}

	}
}