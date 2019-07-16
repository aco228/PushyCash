using PushyCash.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PushyCash.Web.Portal.Models
{
  public class PushyCampaignViewModel
  {
    public List<Campaign> Campaigns { get; set; }

    public PushyCampaignViewModel()
    {
      Campaigns = new List<Campaign>();
    }
  }

  public class Campaign : CampaignViewModel
  {
    public List<Network> Networks { get; set; }

    public Campaign()
    {
      Networks = new List<Network>();
    }
  }

  public class Network
  {
    public string NetworkName { get; set; }
    public NetworkData Data { get; set; }
    public bool ShowLogs { get; set; }
  }

  public class NetworkData
  {
    public List<PushyNetworkCampaignLog> Logs { get; set; }
    public NetworkViewModel Network { get; set; }

    public NetworkData()
    {
      Logs = new List<PushyNetworkCampaignLog>();
    }
  }

  public class CampaignViewModel : PushyCampaignWebModel
  {

  }

  public class NetworkViewModel : PushyNetworkCampaignWebModel
  {
    public NetworkViewModel(PushyNetworkCampaignWebModel pushyNetworkCampaignWebModel)
    {

    }
  }

}