using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Direct.Models.TrafficNetworks
{
	public class DirectTrafficNetworkCampaign : PushyDirectModelBase
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

		/// 
		// TARGET STOPAGE

		public static void AddBlackListValue(int TrafficNetworkCampaignID, string value, string reason)
		{
			int? valueID = PushyCashDirect.Instance.LoadInt("SELECT TrafficNetworkTargetBlacklistID FROM [].TrafficNetworkTargetBlacklist WHERE TrafficNetworkCampaignID={0} AND Name={1}", TrafficNetworkCampaignID, value);
			if (valueID.HasValue && valueID.Value > 0)
				return;
			PushyCashDirect.Instance.Execute("INSERT INTO [].TrafficNetworkTargetBlacklist (TrafficNetworkCampaignID, Name, Reason)", TrafficNetworkCampaignID, value, reason);
		}

		/// 
		// TARGET TEMPORARY STOPAGE (Add)

		public static void AddTemporaryBlackListValue(int TrafficNetworkCampaignID, string value, string reason)
		{
			int? valueID = PushyCashDirect.Instance.LoadInt("SELECT TrafficNetworkTargetTemporaryBlacklistID FROM [].TrafficNetworkTargetTemporaryBlacklist WHERE TrafficNetworkCampaignID={0} AND Name={1}", TrafficNetworkCampaignID, value);
			if (valueID.HasValue && valueID.Value > 0)
				return;
			PushyCashDirect.Instance.Execute("INSERT INTO [].TrafficNetworkTargetTemporaryBlacklist (TrafficNetworkCampaignID, Name, Reason)", TrafficNetworkCampaignID, value, reason);
		}

		/// 
		// TARGET TEMPORARY STOPAGE (Remove)
		
		public static void RemoveTemporaryBlackListValue(int TrafficNetworkCampaignID, string value)
		{
			PushyCashDirect.Instance.Execute("DELETE FROM [].TrafficNetworkTargetTemporaryBlacklist WHERE TrafficNetworkCampaignID={0} AND Name={1}", TrafficNetworkCampaignID, value);
		}

	}
}
