using PushyCash.Web.Core.Models;
using PushyCash.Web.Core.Models.Campaign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Campaigns
{
	public class PushyNetworkCampaignWebModelStatusChanger
	{
		private PushyNetworkCampaign _network = null;
		
		public PushyNetworkCampaignWebModelStatusChanger(PushyNetworkCampaign network)
		{
			this._network = network;
		}

		public void SendReportIfThereAreChanges()
		{
			bool result = false;
			if (this._network.Conversions != this._network.WebModel.Conversions)
				result = true;
			if (this._network.Clicks != this._network.WebModel.Clicks)
				result = true;
			else if (this._network.Payout != this._network.WebModel.Payout)
				result = true;
			else if (this._network.Spent != this._network.WebModel.Cost)
				result = true;
			else if (this._network.IsStoped != this._network.WebModel.IsStoped)
				result = true;
			else if (this._network.TrafficCampaign.BidValue != this._network.WebModel.LastBid)
				result = true;
			else if (this._network.Budget != this._network.WebModel.Budget)
				result = true;

			if (!result)
				return;

			this._network.WebModel.Conversions = this._network.Conversions;
			this._network.WebModel.Clicks = this._network.Clicks;
			this._network.WebModel.Cost = this._network.Spent;
			this._network.WebModel.Payout = this._network.Payout;
			this._network.WebModel.IsStoped = this._network.IsStoped;
			this._network.WebModel.LastBid = this._network.TrafficCampaign.BidValue;

			if(result)
				Web.Core.PushyWebSender.Send<PushyNetworkCampaignUpdateWebModel>("OnPushyNetworkCampaignUpdate", new PushyNetworkCampaignUpdateWebModel()
				{
					LinkName = this._network.PushyCampaign.Link.FullName,
					PushyNetworkCampaignName = this._network.TrafficCampaign.TrafficNetworkName,

					IsStoped = this._network.WebModel.IsStoped,
					IsPaused = this._network.WebModel.IsPaused,
					Conversions = this._network.WebModel.Conversions,
					Clicks = this._network.WebModel.Clicks,
					Payout = Math.Round(this._network.WebModel.Payout, 2),
					Cost = Math.Round(this._network.WebModel.Cost, 2),
					LastBid = this._network.WebModel.LastBid
				});
		}


	}
}
