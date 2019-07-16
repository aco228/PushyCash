using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.Models.Stats
{
	[Serializable()]
	public class AfflowInitialCampaignData
	{
		public int PushyNetworkCampaignID { get; set; } = -1;
		public string CampaignID { get; set; } = string.Empty;
		public string NID { get; set; } = string.Empty;
		public string Country { get; set; } = string.Empty;
		public string Device { get; set; } = string.Empty;
		public string MobileOperator { get; set; } = string.Empty;

		public int Clicks
		{
			get
			{
				int result = 0;
				foreach (var o in Offers) result += o.Clicks;
				return result;
			}
		}
		public int Conversions
		{
			get
			{
				int result = 0;
				foreach (var o in Offers) result += o.Conversions;
				return result;
			}
		}
		public double Revenue
		{
			get
			{
				double result = 0;
				foreach (var o in Offers) result += o.Revenue;
				return result;
			}
		}

		public AfflowCampaignStat CampaignStats { get; set; } = null;
		public List<AfflowOfferMobileOperatorStats> Offers { get; set; } = null;
	}
}
