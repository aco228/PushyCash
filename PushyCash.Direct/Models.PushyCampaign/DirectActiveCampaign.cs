using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models.PushyCampaign
{
	public class DirectActiveCampaign
	{

		public static void Insert(string campaignID, string trafficNetwork)
		{
			PushyCashDirect.Instance.Execute("INSERT INTO [].ActiveCampaigns (TrafficNetwork, CampaignID)", trafficNetwork, campaignID);
		}
		
		public static void Delete(string campaignID, string trafficNetwork)
		{
			PushyCashDirect.Instance.Execute("DELETE FROM [].ActiveCampaigns WHERE TrafficNetwork={0} AND CampaignID={1}", trafficNetwork, campaignID);
		}
		
	}
}
