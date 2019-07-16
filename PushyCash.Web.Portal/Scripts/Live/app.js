var app = new Vue({
  el: '#app',
  data: {
    liveFeeds: [],
    debug: [],
    showLog: false,
    campaigns: [],
    mainInfo: {},
    deamon: false

  },
  computed: {
    cr() {
      let cr = 0;
      if (this.mainInfo.Clicks == 0) {
        return 0;
      }
      cr = this.mainInfo.Conversions / this.mainInfo.Clicks;
      return Math.round(cr * 100) / 100;
    }
  },
  async created() {
    liveManager = new LiveManager();

    this.mainInfo = MAIN_INFO;
    this.mainInfo.TrackedPayout = Math.round(this.mainInfo.TrackedPayout * 100) / 100;
    this.mainInfo.UntrackedPayout = Math.round(this.mainInfo.UntrackedPayout * 100) / 100;

    this.deamon = DEAMON;

    let debug = await axios.get("/api/debug");
    this.debug = debug.data;

    let response = await axios.get("/api/campaign");
    this.campaigns = response.data.Campaigns;

    liveManager.onMainInformationReceived = data => {
      console.log(data);
      this.mainInfo = data;
      this.mainInfo.TrackedPayout = Math.round(data.TrackedPayout * 100) / 100;
      this.mainInfo.UntrackedPayout = Math.round(data.UntrackedPayout * 100) / 100;
    }
  
    liveManager.onLiveFeedData = (data) => {

      this.liveFeeds = [];
      
      data.forEach(row => {

        let liveFeed = {};

        liveFeed.Country = row.Country;
        liveFeed.MobileOperator = row.MobileOperator.split(" ")[0];
        liveFeed.Vertical = row.Vertical;
        liveFeed.OS = row.OS + ".png";

        liveFeed.Conversions = row.Conversions;
        liveFeed.OneMinuteConversions = row.OneMinuteConversions;
        liveFeed.OneMinuteAvaragePayout = row.OneMinuteAvaragePayout;
        liveFeed.TwoMinuteConversions = row.TwoMinuteConversions;
        liveFeed.TwoMinuteAvaragePayout = row.TwoMinuteAvaragePayout;
        liveFeed.TotalConversions = row.TotalConversions;
        liveFeed.TotalPayout = Math.round(row.TotalPayout * 100) / 100;
        liveFeed.Timeout = row.Timeout;
        liveFeed.LastConversions = row.LastConversions;

        let className = "";

        if (liveFeed.HasActiveCampaing) {
          className = "live-row-green";
        } else if (liveFeed.IsLocked) {
          className = "live-row-red";
        }

        liveFeed.className = className;

        this.liveFeeds.push(liveFeed);
      });
    }

    liveManager.onPushyCampaignStart = data => {

      console.log(data);

      let model = { ...data, Networks: [] };

      if (this.campaigns.findIndex(c => c.LinkName == data.LinkName) == -1) {
        this.campaigns.push(model);
      }

    }

    liveManager.onPushyCampaignUpdate = data => {
      
      console.log("onPushyCampaignUpdate",data);

      let campaign = this.campaigns.find(c => c.LinkName == data.LinkName);
      if (campaign == null) { return; }

      campaign.LinkName = data.LinkName;
      campaign.OffersActivated = data.OffersActivated;
      campaign.NumberOfConversions = data.NumberOfConversions;
      campaign.TrackingMinutes = data.TrackingMinutes;
      campaign.RecomendedBudget = data.RecomendedBudget;
      campaign.RecomendedClicksForConversion = data.RecomendedClicksForConversion;
      campaign.LinkAvaragePayout = data.LinkAvaragePayout;
    }

    liveManager.onPushyCampaignStop = data => {

      console.log(data);

      let index = this.campaigns.findIndex(c => c.LinkName == data.LinkName);
      if (index == -1) { return; }

      this.campaigns.splice(index, 1);
    }

    liveManager.onPushyNetworkCampaignStart = (data) => {
      
      console.log("onPushyNetworkCampaignStart",data);

      let campaign = this.campaigns.find(c => c.LinkName == data.LinkName);
      if (campaign == null) { return }

      let network = { NetworkName: data.NetworkName, Data: {}, ShowLogs: false };
      network.Data.Network = data;
      network.Data.Logs = [];

      if (campaign.Networks.findIndex(n => n.NetworkName == data.NetworkName) != -1) { return };

      campaign.Networks.push(network);
      campaign.NumberOfConversions += data.Conversions;
    }

    liveManager.onPushyNetworkCampaignUpdate = (data) => {


      console.log(data);

      let campaign = this.campaigns.find(c => c.LinkName == data.LinkName);
      if (campaign == null) return;

      let network = campaign.Networks.find(n => n.NetworkName == data.NetworkName);
      if (network == null) return;

      Object.assign(network.Data.Network, data);
      //network = model;

      let conversions = 0;
      campaign.Networks.forEach(n => {
        conversions += n.Data.Network.Conversions;
      });
      
      if (campaign != null) {
        campaign.NumberOfConversions = conversions;
      }
    }

    liveManager.onPushyNetworkCampaignLog = data => {
      console.log(data);

      let campaign = this.campaigns.find(c => c.LinkName == data.LinkName);
      if (campaign == null) return;

      let network = campaign.Networks.find(n => n.NetworkName == data.PushyNetworkCampaignName);
      if (network == null) return;

      network.Data.Logs.push(data);
    }

    liveManager.onDebug = data => {
      console.log(data);
      if (this.debug.length == 200) {
        this.debug.splice(-1, 1);
      }
      this.debug.unshift(data);
    }

    liveManager.deamonIsLive = data => {
      this.deamon = true;
    }

    liveManager.deamonDown = data => {
      this.deamon = false;
    }

  }
})