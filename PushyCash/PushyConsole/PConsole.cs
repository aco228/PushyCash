using PushyCash.Links;
using PushyCash.PushyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash
{
	public class PConsole
	{
		private static object Transactions_lockObj = new object();
		public static bool ReadyForPrinting = false;
		public static PConsole Current = null;

		public static string Hours(DateTime time) { return string.Format("{0}:{1}", (time.Hour < 10 ? "0" : "") + time.Hour.ToString(), (time.Minute < 10 ? "0" : "") + time.Minute.ToString()); }
		
		private PConsoleHeaderControl HeaderControl = new PConsoleHeaderControl();
		private PConsoleTableControl TableControl = new PConsoleTableControl();
		private PConsoleLogControl LogControl = new PConsoleLogControl();

		public static void PrintHeader()
		{
			if (Current == null || !ReadyForPrinting)
				return;
			Current.HeaderControl.OnNewIteration();
			Current.HeaderControl.PrintHeader();
			//Current.HeaderControl.Clear();
		}
		
		public static void PrintTableAndCampaigns(List<LinkDisplay> list)
		{
			if (Current == null || !ReadyForPrinting)
				return;

			Current.TableControl.OnNewIteration();
			Current.TableControl.PrintHeader();
			Current.TableControl.PrintTableData(list);
			Current.TableControl.PrintCampaigns();
			//Current.TableControl.Clear();
		}

		public static void AddTrackedConvertion(double payout)
		{
			if (Current == null)
				return;

			PushyGlobal.TrackedConversions++;
			PushyGlobal.TrackedPayout += payout;
		}

		public static void AddUntrackedConvertion(double payout)
		{
			if (Current == null)
				return;

			PushyGlobal.UntrackedConversions++;
			PushyGlobal.UntrackedPayout += payout;
		}

		public static void PrintLogs()
		{
			if (Current == null || !ReadyForPrinting)
				return;

			Current.LogControl.OnNewIteration();
			Current.LogControl.PrintLogs();
			//Current.LogControl.Clear();
		}
		
		

	}
}
