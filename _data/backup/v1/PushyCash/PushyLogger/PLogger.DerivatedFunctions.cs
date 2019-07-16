using PushyCash.PushyLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash
{
	public static partial class PLogger
	{
	
		public static void InterestingOffer(string country, string mobileOperator, string vertical, string device, 
			double avarigePayout, string offerID, double minutesPassed, int numberOfTransatcions, int minutes)
		{
			lock (lockObj)
			{
				string text = string.Format("{0}.{1}.{2}.{3} ({4}) = {5}", country, mobileOperator, vertical, device, avarigePayout, offerID);
				Logs.Insert(0, new PushyLog(PushyLogType.OFR, text));
				PushyContext.Slack.Offers.New(country, mobileOperator, vertical, device, avarigePayout, offerID, minutesPassed, numberOfTransatcions, minutes);
				PConsole.PrintLogs();
			}
		}


		public static void OfferDoesNotHaveTrafficNetworks(Links.Link link)
		{
			lock (lockObj)
			{
				PushyContext.Slack.Campaign.NoTrafficNetwork(link.Country, link.MobileOperator, link.Vertical, link.Device);
				string text = string.Format("Link '{0}' is ready but does not have TS", link.Name);
				Logs.Insert(0, new PushyLog(PushyLogType.CSN, text));
				PConsole.PrintLogs();
			}
		}

		public static void OfferStarting(string text)
		{
			lock (lockObj)
			{
				text = string.Format("Link '{0}' ready for start", text);
				Logs.Insert(0, new PushyLog(PushyLogType.CST, text));
				PConsole.PrintLogs();
			}
		}

		public static void OfferEnding(string text)
		{
			lock (lockObj)
			{
				Logs.Insert(0, new PushyLog(PushyLogType.CEN, text));
				PConsole.PrintLogs();
			}
		}
		

	}
}
