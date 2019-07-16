using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.PropellerAds
{
	public class PropellerAdsAfflowMap : Core.TrafficSourceAfflowMapBase
	{
		public override int NID { get => 10729; }
		public override string DefaultOptionsData
		{
			get => "{'entry_popup':1,'exit_popup':1,'vibrate':1,'sound':1,'redirect_time':10,'url':'win.ningoffer.club','rcid':'{campaignId}','kw1':'','kw2':'','kw3':'{zoneid}','kw4':'','kw5':'','s4':'${SUBID}','offer_geo_arr':['{COUNTRY}']}".Replace('"', '\'');
		}
	}
}
