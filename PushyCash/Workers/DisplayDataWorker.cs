using PushyCash.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	public class DisplayDataWorker : WorkerBase
	{
		public DisplayDataWorker() : base(1000)
		{ }

		private string _whiteString = (new StringBuilder()).Append(' ', 100).ToString();
		private int _lastNumberOfLines = 0;

		public override void OnLoop()
		{
			List<LinkDisplay> linkDisplayList = PushyContext.LinkManager.GetDisplayData();
			PConsole.PrintTable(linkDisplayList);

			//return;
			//PMLog.Clear();
			//Console.SetCursorPosition(0, 0);

			//PConsole1.PWriteLine(_whiteString);
			//PConsole1.PWriteLine("  Application started   :  " + SharedInformations.ApplicationStarted.ToString());
			//PConsole1.PWriteLine("  Tracked info          :  " + string.Format("{0} / {1}", SharedInformations.TotalTrackedConversions, SharedInformations.TotalTrackedPayout));
			//PConsole1.PWriteLine("  Untracked info        :  " + string.Format("{0} / {1}", SharedInformations.TotalUntrackedConversions, SharedInformations.TotalUntrackedPayout));
			//PConsole1.PWriteLine(_whiteString);

			//PrintRow("_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_");
			//PrintRow("Country", "MNO", "Conversions", "Payout", "Procentige", "Vertical", "OS", "TConversions", "TPayout", "Tmout", "LConv");
			//PrintRow("_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_");

			//List<LinkDisplay> linkDisplayList = PushyContext.LinkManager.GetDisplayData();
			//foreach (Links.LinkDisplay display in linkDisplayList)
			//	PrintRow(display.Country, display.MobileOperator, display.Conversions, display.Payout, display.Procentige, display.Vertical,
			//		display.OS, display.TotalConversions, display.TotalPayout, display.Timeout, display.LastConversions);

			//PrintRow("_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_");

			//if (linkDisplayList.Count > 0 && PushyContext.SocketServer != null)
			//	PushyContext.SocketServer.SendModel(new Sockets.Core.PushyDistributionModel("list_linkDisplay", linkDisplayList));

			//PMLog.Print();

			//int deleteLines = this._lastNumberOfLines - PConsole1.NewLines;
			//if (deleteLines > 0)
			//	for (int i = 0; i < deleteLines; i++)
			//		Console.WriteLine(_whiteString);


			//this._lastNumberOfLines = PConsole1.NewLines;
			//PConsole1.NewLines = 0;
		}

		
		

	}
}
