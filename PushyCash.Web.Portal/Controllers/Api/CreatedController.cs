using PushyCash.Web.Core.Models;
using PushyCash.Web.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PushyCash.Web.Portal.Controllers.Api
{
  public class CreatedController : ApiController
  {
    public PushyCampaignViewModel Get()
    {
      var model = new PushyCampaignViewModel();

      foreach (var c in PushyPortalApplication.PushyCampaigns.Values)
      {
        Campaign campaign = new Campaign();
        campaign.LinkName = c.Campaign.LinkName;
        campaign.LinkAvaragePayout = c.Campaign.LinkAvaragePayout;
        campaign.NumberOfConversions = c.Campaign.NumberOfConversions;
        campaign.OffersActivated = c.Campaign.OffersActivated;
        campaign.RecomendedBudget = c.Campaign.RecomendedBudget;
        campaign.RecomendedClicksForConversion = c.Campaign.RecomendedClicksForConversion;
        campaign.TrackingMinutes = c.Campaign.TrackingMinutes;

        foreach (var n in c.NetworkCampaigns.Values)
        {
          Network network = new Network();
          network.NetworkName = n.Campaign.NetworkName;
          network.ShowLogs = false;
          NetworkData data = new NetworkData();

          NetworkViewModel vm = new NetworkViewModel(n.Campaign);
          data.Network = vm;

          foreach (var l in n.Logs)
            data.Logs.Add(l);

          network.Data = data;
          campaign.Networks.Add(network);
        }
        model.Campaigns.Add(campaign);
      }
      return model;
    }
  }
}
