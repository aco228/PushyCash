
function LiveManager()
{
	this.init = function () { }
	this.onTest = function (data) { console.log(data); }
  this.onMainInformationReceived = function (data) { console.log(data); }
  this.onPushyCampaignStart = function (data) { return data };
  this.onLiveFeedData = function (data) { return data;}
  this.onPushyNetworkCampaignStart = function (data) { return data; }
  this.onPushyNetworkCampaignUpdate = function (data) { return data; }
  this.onPushyNetworkCampaignLog = function (data) { return data; }
  this.onPushyCampaignUpdate = function (data) { return data; }
  this.onPushyCampaignStop = function (data) { return data; }
  this.onDebug = function (data) { return data; }
  this.deamonIsLive = function (data) { return data; }
  this.deamonDown = function (data) { return data; }
}