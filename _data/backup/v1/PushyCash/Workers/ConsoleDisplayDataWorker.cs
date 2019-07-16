using PushyCash.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	public class ConsoleDisplayDataWorker : WorkerBase
	{
		public ConsoleDisplayDataWorker() : base("console", "display", 1000)
		{ }
		
		public override void OnLoop()
		{
			//List<LinkDisplayOld> linkDisplayList = PushyContext.LinkManager.GetDisplayDataOld();
			List<LinkDisplay> linkDisplayList = PushyContext.LinkManager.GetDisplayData();

			Console.Clear();
			PConsole.PrintHeader();
			PConsole.PrintTableAndCampaigns(linkDisplayList);
			PConsole.PrintLogs();


		}
		
	}
}
