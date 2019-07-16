using Direct.Core;
using PushyCash.Afflow.TrafficCosts;
using PushyCash.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Campaigns
{
	public class NetworkCampaignMetrics
	{
		private PushyNetworkCampaign _campaign = null;

		public bool ShouldCampaignRun { get; protected set; } = true;
		public string Explanation { get; protected set; } = string.Empty;
		public double Multiplier { get; protected set; } = 1.5;
		

		public NetworkCampaignMetrics(PushyNetworkCampaign campaign)
		{
			// TODO: Implement this in a better way

			//DirectContainer dc = PushyCashDirect.Instance.LoadContainer(
			//	@"SELECT Clicks, Conversions, ROI FROM [].PushyNetworkCampaign 
			//		WHERE TrafficNetworkName={0} AND CampaignID={1} AND WasFinished=1 AND Clicks>350",
			//	campaign.TrafficCampaign.TrafficNetworkName, campaign.CampaignID);
			
			//switch (dc.RowsCount)
			//{
			//	/// TESTING PHASE
			//	///

			//	case 0:
			//		this.Multiplier = 6;
			//		return;

			//	case 1:
			//		if (dc.GetDouble("ROI", 0).Value < -95)
			//		{
			//			this.CampaignShoulNotRun("Campaign runed one time and ROI is above -95%");
			//			return;
			//		}
			//		this.Multiplier = 4;
			//		return;

			//	case 2:
			//		if (dc.GetDouble("ROI", 0).Value < -75)
			//		{
			//			this.CampaignShoulNotRun("Campaign runed TWO time and ROI is above -75%");
			//			return;
			//		}
			//		this.Multiplier = 3.5;
			//		return;

			//	case 3:
			//		if (dc.GetDouble("ROI", 0).Value < -25)
			//		{
			//			this.CampaignShoulNotRun("Campaign runed THREE time and ROI is above -25%");
			//			return;
			//		}
			//		this.Multiplier = 2;
			//		return;

			//	case 4:
			//		if (dc.GetDouble("ROI", 0).Value < -0.2)
			//		{
			//			this.CampaignShoulNotRun("Campaign runed FOUR time and ROI is above -0.2%");
			//			return;
			//		}
			//		this.Multiplier = 1.2;
			//		return;

			//	default:
			//		break;
			//}

			//// check last 2 iteration
			//int lastIndex = dc.RowsCount - 1;
			//int firstIndex = lastIndex - 2;
			//if (firstIndex <= 5) firstIndex = 5;

			//double rois = 0.0;
			//int count = 0;

			//for(int i = firstIndex; i <= lastIndex; i++)
			//{
			//	count++;
			//	rois += dc.GetDouble("ROI", i).Value;
			//}

			//if (rois / count < 0)
			//	this.CampaignShoulNotRun("Last two iteration ROI was bellow 0%");
		}
		
		public void CampaignShoulNotRun(string explanation)
		{
			this.ShouldCampaignRun = false;
			this.Explanation = explanation;
		}


	}
}
