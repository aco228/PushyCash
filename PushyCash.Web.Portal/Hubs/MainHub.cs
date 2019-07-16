using Microsoft.AspNet.SignalR;
using PushyCash.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PushyCash.Web.Portal.Hubs
{
	public class MainHub : Hub
  {
    public static MainHub Current { get => new MainHub(GlobalHost.ConnectionManager.GetHubContext<MainHub>()); }
    private IHubContext _context = null;

    public MainHub(IHubContext context)
    {
      this._context = context;
    }
		
		public void OnDeamonDown() => this.Update("deamonDown", $"Deamon is not responding {(int)(DateTime.Now - PushyPortalApplication.LastDeamonCall.Value).Minutes} minutes"); 
		public void OnDeamonIsLive() => this.Update("deamonIsLive", "Demon is live");  
		public void OnMainInformations(MainInformationsWebModel model) => this.Update("onMainInformationReceived", model); 
		public void OnLiveFeedData(List<LiveFeedTableWebModel> model) => this.Update("onLiveFeedData", model); 
		public void OnDebug(DebugWebModel model) { this.Update("onDebug", model); }

		public void OnPushyCampaignStart(PushyCampaignWebModel data) => this.Update("onPushyCampaignStart", data);
		public void OnPushyCampaignUpdate(PushyCampaignWebModel data) => this.Update("onPushyCampaignUpdate", data);
		public void OnPushyCampaignStop(PushyCampaignWebModel data) => this.Update("onPushyCampaignStop", data);

		public void OnPushyNetworkCampaignStart(PushyNetworkCampaignWebModel data) => this.Update("onPushyNetworkCampaignStart", data);
		public void OnPushyNetworkCampaignUpdate(PushyNetworkCampaignWebModel data) => this.Update("onPushyNetworkCampaignUpdate", data);
		public void OnPushyNetworkCampaignLog(PushyNetworkCampaignLog data) => this.Update("onPushyNetworkCampaignLog", data);


		public void Update(string method, object data)
    {
      var sendinData = new
      {
        method = method,
        data = data
      };

      if (this._context != null)
        this._context.Clients.All.update(sendinData);
      else
        this.Clients.All.update(sendinData);

    }

  }
}