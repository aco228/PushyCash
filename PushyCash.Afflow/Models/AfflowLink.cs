using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Afflow.Models
{
	[Serializable()]
	public class AfflowLink
	{
		public string Key { get { return string.Format("_{0}.{1}.{2}.{3}", this.Country, this.AfflowMobileOperator, this.vertical, this.Device); } }
		public string FullName { get => string.Format("_{0}.{1}.{2}.{3}.{4}", this.Country, this.MobileOperator, this.vertical , this.Device, this.TrafficNetwork); }
		
		public string Country { get; protected set; } = "";
		public string MobileOperator { get; protected set; } = "";
		public string AfflowMobileOperator { get; internal set; } = "";
		public string Device { get; protected set; } = "";
		public string TrafficNetwork { get; protected set; } = "";

		// Afflow data
		public string disable_bbr { get; protected set; } = "";
		public string disable_popunder { get; protected set; } = "";
		public string safe_url { get; protected set; } = "";
		public string safe_page_rcid { get; protected set; } = "";
		public string whitelist_data { get; protected set; } = "";
		public string options_data { get; protected set; } = "";
		public string offer_whitelist_data = "";
		public string vertical { get; protected set; } = "";
		public string name { get; protected set; } = "";
		public string bbr_rcid { get; protected set; } = "";
		public string popunder_rcid { get; protected set; } = "";
		public string desktop_redirect { get; protected set; } = "";
		public string hash { get; protected set; } = "";
		public string nid { get; protected set; } = "";
		
		public static AfflowLink Get(dynamic row)
		{
			AfflowLink result = new AfflowLink();
			string name = row.name; // name should be like this: _IT.vodafone.mainstream.ios.zeropark
			if (name[0] != '_')
				return null;

			string[] split = name.Split('.');
			if (split.Length != 5)
				return null;

			result.Country = split[0].Replace("_", string.Empty);
			result.MobileOperator = split[1];
			result.Device = split[3];
			result.TrafficNetwork = split[4];
			
			result.disable_bbr = row.disable_bbr;
			result.disable_popunder = row.disable_popunder;
			result.safe_url = row.safe_url;
			result.safe_page_rcid = row.safe_page_rcid;
			result.whitelist_data = row.whitelist_data;
			result.options_data = row.options_data.ToString();
			if(result.options_data.Contains("\"offer_geo_arr\":[\"{IT}\"]")) // fucking fix
				result.options_data = result.options_data.Replace("\"offer_geo_arr\":[\"{IT}\"]", string.Format("\"offer_geo_arr\":[\"{0}\"]", result.Country));
			result.offer_whitelist_data = row.offer_whitelist_data;
			result.vertical = row.vertical;
			result.name = row.name;
			result.bbr_rcid = row.bbr_rcid;
			result.popunder_rcid = row.popunder_rcid;
			result.desktop_redirect = row.desktop_redirect;
			result.hash = row.hash;
			result.nid = row.nid;

			return result;
		}

	}
}
