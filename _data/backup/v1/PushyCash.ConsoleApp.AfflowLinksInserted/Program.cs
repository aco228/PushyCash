using PushyCash.Afflow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.ConsoleApp.AfflowLinksInserted
{
	class Program
	{

		static string[] Data = new string[]
		{
			"IT mainstream #  tim, wifi, wind, 3, vodafone",
			"SA mainstream #  wifi, zain, mobily, jawal",
			"CA mainstream #  wifi",
			"FR adult #  wifi, sfr, orange, bouygues",
			"AE mainstream #  wifi, etisalat, du",
			"CA aggressive #   wifi, bell, rogers, videotron, telus",
			"AU mainstream #  wifi, telstra, vodafone, optus",
			"RU mainstream #  tele2, mts, wifi, beeline, megafon",
			"IQ mainstream #  wifi, asia, zain",
			"US aggressive #   wifi, mobile, sprint, verizon, att",
			"IT aggressive #   wind, tim, 3, wifi, vodafone",
			"TH adult #  ais, dtac, TRUEMOVE, wifi",
			"ES mainstream #  wifi, vodafone, orange, movistar",
			"DE aggressive #   wifi, o2, telekom, vodafone",
			"EG mainstream #  wifi, vodafone, orange, etisalat",
			"GR aggressive #   wifi, cosmote, vodafone, wind",
			"MX adult #  wifi, telcel, movistar, att",
			"GB mainstream #  wifi, o2, 3, orange, lycamobile, vodafone, t-mobile",
			"FR aggressive #   mobile, wifi, BOUYGUES, nrj, sfr, orange",
			"MY mainstream #  maxis, wifi, digi, celcom, mobile",
			"IR mainstream #  mtn, ir-mci, wifi, rightel",
			"IN aggressive #   jio, airtel, wifi, idea, vodafone, cellone",
			"JO mainstream #  zain, wifi, UMNIAH, orange",
			"US adult #  wifi, t-mobile, att, sprint, verizon",
			"AE adult #  wifi",
			"JP aggressive #   wifi, docomo, au, softbank",
			"BR aggressive #   wifi, vivo, tim",
			"MA mainstream #  iam, wifi, inwi"
		};

		private static string[] Devices = new string[] { "Android", "iOS" };

		static void Main(string[] args)
		{
			string result = "";

			for(int i = 0; i < Data.Length; i++)
			{
				string d = Data[i];
				string[] mainSplit = d.Split('#');
				string country = mainSplit[0].Substring(0, 3).Replace(" ", string.Empty);
				string vertical = mainSplit[0].Substring(3, mainSplit[0].Length - 3).Trim();
				AfflowManager afflowManager = new AfflowManager();
				
				string[] operatorsSplit = mainSplit[1].Trim().Split(',');

				int limit = 10;
				int ii = 0;

				foreach(string o in operatorsSplit)
				{
					if (string.IsNullOrEmpty(o))
						continue;

					string realOperatorName = afflowManager.MobileOperatorMap.GetName(country, o);
					if(string.IsNullOrEmpty(realOperatorName))
					{
						Console.WriteLine("Could not find real name for this Operator");
					}

					foreach(string device in Devices)
					{
						string name = string.Format("_{0}.{1}.{2}.{3}.zeropark", country, o.Trim(), vertical, device.ToLower());
						string postData =
										"&rb_url=null"
									+ "&safe_page_rcid={rcid}"
									+ "&disable_bbr=0"
									+ "&disable_popunder=0"
									+ "&whitelist_data={'geo':['{COUNTRY}'],'os':['{DEVICE}'],'carrier':[{'geo':'{COUNTRY}','carrier':'{COUNTRY} {OPERATOR}'}],'city':null}"
									+ "&options_data={'entry_popup':1,'exit_popup':1,'vibrate':1,'sound':1,'redirect_time':10,'url':'win.ningoffer.club','rcid':'{long_campaign_id}','kw1':'{keyword}','kw2':'{match}','kw3':'{target}','kw4':'','kw5':'','s4':'{cid}','offer_geo_arr':['{IT}']}"
									+ "&vertical={VERTICAL}"
									+ "&name={NAME}"
									+ "&nid=9803" // league
									+ "&bbr_rcid={rcid}"
									+ "&desktop_redirect=1"
									+ "&popunder_rcid={rcid}";
						postData = postData.Replace("'", "\"")
									.Replace("{COUNTRY}", country)
									.Replace("{DEVICE}", device)
									.Replace("{OPERATOR}", realOperatorName)
									.Replace("{VERTICAL}", vertical)
									.Replace("{NAME}", name);

						Request(postData);

						ii++;
						Console.WriteLine("INSERTED " + name);
					}

					Console.WriteLine(string.Format("-----> Finished {0}_{1} {2}/{3}", country, vertical, i, Data.Length));
				}
				
			}


			int a = 0;

		}

		private static string TOKEN = "4cb20a96a3ac100deab3bf486c06a2bedb3a3beb";

		public static string Request(string data)
		{
			string requestUrl = "https://api.monetizer.co/data/partner-link.php";
			try
			{
				var webRequest = System.Net.WebRequest.Create(requestUrl);
				if (webRequest == null)
					return null;

				webRequest.Method = "POST";
				webRequest.Timeout = 12000;
				webRequest.ContentType = "application/json";
				webRequest.Headers.Add("x_afflow_api_token", TOKEN);
				webRequest.ContentType = "application/x-www-form-urlencoded";

				string postData = data;
				ASCIIEncoding encoding = new ASCIIEncoding();
				byte[] byte1 = encoding.GetBytes(postData);
				webRequest.ContentLength = byte1.Length;
				Stream newStream = webRequest.GetRequestStream();
				newStream.Write(byte1, 0, byte1.Length);

				using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
				{
					using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
					{
						var jsonResponse = sr.ReadToEnd();

						//Console.WriteLine(String.Format("Response: {0}", jsonResponse));
						return jsonResponse;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return null;
			}
		}



	}
}
