using Microsoft.AspNet.SignalR;
using PushyCash.Web.Core.Models;
using PushyCash.Web.Portal.Hubs;
using PushyCash.Web.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PushyCash.Web.Portal.Controllers
{
	public class TestsController : Controller
	{

    public ActionResult OnPushyCampaignStart()
    {
      PushyCampaignWebModel data = new PushyCampaignWebModel();
      data.LinkName = "Campaign1";
      data.OffersActivated = "offer1 offer2";
      if (!PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
        PushyPortalApplication.PushyCampaigns.Add(data.LinkName, new Models.PushyCampaignModel() { Campaign = data });
      MainHub.Current.OnPushyCampaignStart(data);
      return new HttpStatusCodeResult(200);
    }
    public ActionResult OnPushyCampaignUpdate()
    {
      PushyCampaignWebModel data = PushyPortalApplication.PushyCampaigns["Campaign1"].Campaign;
      data.TrackingMinutes += 3;
      if (data == null)
        return new HttpStatusCodeResult(500);
      if (PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
        PushyPortalApplication.PushyCampaigns[data.LinkName].Campaign = data;
      MainHub.Current.OnPushyCampaignUpdate(data);
      return new HttpStatusCodeResult(200);
    }
    public ActionResult OnPushyCampaignStop()
    {
      PushyCampaignWebModel data = PushyPortalApplication.PushyCampaigns["Campaign1"].Campaign;
      if (data == null)
        return new HttpStatusCodeResult(500);
      if (PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
        PushyPortalApplication.PushyCampaigns.Remove(data.LinkName);
      MainHub.Current.OnPushyCampaignStop(data);
      return new HttpStatusCodeResult(200);
    }

    public ActionResult OnPushyNetworkCampaignStart()
    {
      PushyNetworkCampaignWebModel data = new PushyNetworkCampaignWebModel();
      data.LinkName = "Campaign1";
      data.Name = "Zero";
      data.NetworkName = "Zero";
      data.Conversions = 5;
      data.Budget = 20.0;
      data.ToleratedDeficit = 15;
      
      if (data == null || !PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
        return new HttpStatusCodeResult(500);

      PushyCampaignModel model = PushyPortalApplication.PushyCampaigns[data.LinkName];
      if (!model.NetworkCampaigns.ContainsKey(data.Name))
        model.NetworkCampaigns.Add(data.Name, new PushyNetworkCampaignModel() { Campaign = data });

      MainHub.Current.OnPushyNetworkCampaignStart(data);
      return new HttpStatusCodeResult(200);
    }

    public ActionResult OnPushyNetworkCampaignStart2()
    {
      PushyNetworkCampaignWebModel data = new PushyNetworkCampaignWebModel();
      data.LinkName = "Campaign1";
      data.Name = "Network";
      data.NetworkName = "Network";
      data.Conversions = 6;
      data.Budget = 20.0;
      data.ToleratedDeficit = 15;

      if (data == null || !PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
        return new HttpStatusCodeResult(500);

      PushyCampaignModel model = PushyPortalApplication.PushyCampaigns[data.LinkName];
      if (!model.NetworkCampaigns.ContainsKey(data.Name))
        model.NetworkCampaigns.Add(data.Name, new PushyNetworkCampaignModel() { Campaign = data });

      MainHub.Current.OnPushyNetworkCampaignStart(data);
      return new HttpStatusCodeResult(200);
    }
    public ActionResult OnPushyNetworkCampaignUpdate()
    {
      PushyNetworkCampaignWebModel data = PushyPortalApplication.PushyCampaigns["Campaign1"].NetworkCampaigns["Zero"].Campaign;
      data.Conversions++;
      
      if (data == null || !PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
        return new HttpStatusCodeResult(500);

      PushyCampaignModel model = PushyPortalApplication.PushyCampaigns[data.LinkName];
      if (model.NetworkCampaigns.ContainsKey(data.Name))
        model.NetworkCampaigns[data.Name].Campaign = data;

      MainHub.Current.OnPushyNetworkCampaignUpdate(data);
      return new HttpStatusCodeResult(200);
    }
    public ActionResult OnPushyNetworkCampaignUpdate2()
    {
      PushyNetworkCampaignWebModel data = PushyPortalApplication.PushyCampaigns["Campaign1"].NetworkCampaigns["Network"].Campaign;
      data.Conversions = 7;

      if (data == null || !PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
        return new HttpStatusCodeResult(500);

      PushyCampaignModel model = PushyPortalApplication.PushyCampaigns[data.LinkName];
      if (model.NetworkCampaigns.ContainsKey(data.Name))
        model.NetworkCampaigns[data.Name].Campaign = data;

      MainHub.Current.OnPushyNetworkCampaignUpdate(data);
      return new HttpStatusCodeResult(200);
    }
    public ActionResult OnPushyNetworkCampaignLog()
    {
      PushyNetworkCampaignLog data = new PushyNetworkCampaignLog();
      data.LinkName = "Campaign1";
      data.PushyNetworkCampaignName = "Zero";
      data.Created = DateTime.Now;
      data.Text = "Text";

      if (data == null || !PushyPortalApplication.PushyCampaigns.ContainsKey(data.LinkName))
        return new HttpStatusCodeResult(500);

      PushyCampaignModel model = PushyPortalApplication.PushyCampaigns[data.LinkName];
      if (model.NetworkCampaigns.ContainsKey(data.PushyNetworkCampaignName))
        model.NetworkCampaigns[data.PushyNetworkCampaignName].Logs.Insert(0, data);

      MainHub.Current.OnPushyNetworkCampaignLog(data);
      return new HttpStatusCodeResult(200);
    }

    public ActionResult Test2()
    {
      var list = PushyPortalApplication.PushyCampaigns.Keys.ToList();

      int a = 0;

      return View();
    }
    

	}
}