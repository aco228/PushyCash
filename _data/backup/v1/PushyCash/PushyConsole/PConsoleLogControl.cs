using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.PushyConsole
{
	internal class PConsoleLogControl : PConsoleControlBase
	{
		private static int Top = 2, Left = 113, Width = 70;

		public PConsoleLogControl() : base(Top, Left, Width)
		{
		}

		public void PrintLogs()
		{
			if ((Left + Width) > Console.BufferWidth)
				return;

			foreach(PushyLogger.PushyLog log in PLogger.Logs)
			{
				lock(PConsoleControlBase.lockObj)
				{
					Console.SetCursorPosition(Left, CurrentLineCount);
					Console.Write(log.Created + " ");

					if (log.Type == PLogger.PushyLogType.ERR) Console.ForegroundColor = ConsoleColor.Red;
					if (log.Type == PLogger.PushyLogType.CEN) Console.ForegroundColor = ConsoleColor.DarkGreen;
					if (log.Type == PLogger.PushyLogType.WRN) Console.ForegroundColor = ConsoleColor.Yellow;
					if (log.Type == PLogger.PushyLogType.OFR) Console.ForegroundColor = ConsoleColor.Cyan;
					if (log.Type == PLogger.PushyLogType.DBG) Console.ForegroundColor = ConsoleColor.White;
					if (log.Type == PLogger.PushyLogType.CST  || log.Type == PLogger.PushyLogType.CSN)
						Console.ForegroundColor = ConsoleColor.Green;
					Console.Write(string.Format("[{0}] ", log.Type.ToString()));
					Console.ResetColor();

					if(log.Type == PLogger.PushyLogType.CST) Console.ForegroundColor = ConsoleColor.Green;
					
					Console.Write(this.TrimString(log.Text, 100));
					Console.ResetColor();
					Console.Write(Environment.NewLine);
					this.CurrentLineCount++;
				}
			}
		}
		
	}
}
