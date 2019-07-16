using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.PopAds
{
	public class PopAdsAfflowMap : Core.TrafficSourceAfflowMapBase
	{
		public override int NID { get => 11629; }
		public override string DefaultOptionsData
		{
			get => "{'entry_popup':1,'exit_popup':0,'vibrate':1,'sound':1,'redirect_time':10,'url':'win.ningoffer.club','rcid':'campaign_name','kw1':'[WEBSITEID]','kw2':'[KEYWORD]','kw3':'[CATEGORYID]','kw4':'[BID]','kw5':'[QUALITY]','s4':'','offer_geo_arr':['{COUNTRY}']}".Replace('"', '\'');
		}
	}
}
