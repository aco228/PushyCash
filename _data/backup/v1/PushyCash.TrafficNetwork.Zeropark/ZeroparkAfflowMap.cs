using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Zeropark
{
	public class ZeroparkAfflowMap : Core.TrafficSourceAfflowMapBase
	{
		public override int NID { get => 9803; }
		public override string DefaultOptionsData
		{
			get => "{'entry_popup':1,'exit_popup':1,'vibrate':1,'sound':1,'redirect_time':10,'url':'win.ningoffer.club','rcid':'{long_campaign_id}','kw1':'{keyword}','kw2':'{match}','kw3':'{target}','kw4':'','kw5':'','s4':'{cid}','offer_geo_arr':['{COUNTRY}']}".Replace('"', '\'');
		}
	}
}
