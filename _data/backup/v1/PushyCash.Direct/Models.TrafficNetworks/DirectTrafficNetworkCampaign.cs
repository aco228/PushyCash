using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models.TrafficNetworks
{
	public class DirectTrafficNetworkCampaign : DirectModel
	{
		public DirectTrafficNetworkCampaign() : base("TrafficNetworkCampaign") { }

		public int TrafficNetworkCampaignID { get; set; } = -1;
		public int TrafficNetworkID { get; set; } = -1;
		public string FullName { get; set; } = string.Empty;
		public List<string> InitialBlacklist = null;

		public static int? Load(int trafficNetworkID, string name, string id)
		{
			var result = PushyCashDirect.Instance.LoadInt("SELECT TrafficNetworkCampaignID FROM [].TrafficNetworkCampaign WHERE TrafficNetworkID={0} AND FullName={1} AND CampaignID={2}", trafficNetworkID, name, id);
			if (result != null)
				return result;

			return PushyCashDirect.Instance.Execute("INSERT INTO [].TrafficNetworkCampaign (TrafficNetworkID, FullName, CampaignID)", trafficNetworkID, name, id);
		}

		public static List<string> LoadBlacklist(int TrafficNetworkCampaignID)
		{
			return PushyCashDirect.Instance.LoadContainer("SELECT Name FROM [].TrafficNetworkTargetBlacklist WHERE TrafficNetworkCampaignID={0}", TrafficNetworkCampaignID).ConvertToStringList();
		}

		public static void AddNewBlacklistValues(int TrafficNetworkCampaignID, List<string> newList)
		{
			foreach (string name in newList)
				PushyCashDirect.Instance.Execute("INSERT INTO [].TrafficNetworkTargetBlacklist (TrafficNetworkCampaignID, Name)", TrafficNetworkCampaignID, name);
		}

	}
}
