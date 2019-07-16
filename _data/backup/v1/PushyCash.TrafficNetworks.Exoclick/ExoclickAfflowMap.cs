using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.TrafficNetworks.Exoclick
{
	public class ExoclickAfflowMap : Core.TrafficSourceAfflowMapBase
	{
		public override int NID { get => 9803; } // 14406
		public override string DefaultOptionsData
		{
			get => "{'entry_popup':1,'exit_popup':0,'vibrate':1,'sound':1,'redirect_time':10,'url':'win.ningoffer.club','rcid':'{campaign_id}','kw1':'{src_hostname}','kw2':'{site_id}','kw3':'{zone_id}','kw4':'','kw5':'','s4':'{conversions_tracking}','offer_geo_arr':['{COUNTRY}']}".Replace('"', '\'');
		}
	}
}
