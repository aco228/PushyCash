using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Slack
{
	public class SlackManager
	{
		private SlackApi _api = null;
		public SlackCampaign Campaign = null;
		public SlackGeneral General = null;
		public SlackInterestingOffers Offers = null;
		public SlackReport Report = null;

		public SlackManager()
		{
			this._api = new SlackApi();
			this.Campaign = new SlackCampaign(this._api);
			this.General = new SlackGeneral(this._api);
			this.Offers = new SlackInterestingOffers(this._api);
			this.Report = new SlackReport(this._api);
		}
				

	}
}
