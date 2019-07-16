using PushyCash.Direct;
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
		private static object lockObj = new object();
		public enum PushyLogType { DBG, OFR, WRN, ERR, CST, CSN, CEN };
		public static List<PushyLog> Logs = new List<PushyLog>();

		public static void Debug(string text)
		{
			lock (lockObj)
			{
				Web.Core.PushyWebSender.Send<Web.Core.Models.DebugWebModel>("Debug", new Web.Core.Models.DebugWebModel() { Level = "Debug", Text = text });
				PushyContext.Slack.General.New(string.Format("[DBG]: {0}", text));
				Logs.Insert(0, new PushyLog(PushyLogType.DBG, text));
				PushyCashDirect.Instance.Execute("INSERT INTO [].Log (Level, Text)", "DEBUG", text.Replace("'", string.Empty));
				PConsole.PrintLogs();
			}
		}
		public static void Warn(string text)
		{
			lock (lockObj)
			{
				Web.Core.PushyWebSender.Send<Web.Core.Models.DebugWebModel>("Warn", new Web.Core.Models.DebugWebModel() { Level = "Debug", Text = text });
				PushyContext.Slack.General.New(string.Format("[WARN]: {0}", text));
				Logs.Insert(0, new PushyLog(PushyLogType.WRN, text));
				PushyCashDirect.Instance.Execute("INSERT INTO [].Log (Level, Text)", "WARN", text.Replace("'", string.Empty));
				PConsole.PrintLogs();
			}
		}
		public static void Error(string text)
		{
			lock (lockObj)
			{
				Web.Core.PushyWebSender.Send<Web.Core.Models.DebugWebModel>("Error", new Web.Core.Models.DebugWebModel() { Level = "Debug", Text = text });
				PushyContext.Slack.General.New(string.Format("[*ERROR*]: {0}", text));
				PushyCashDirect.Instance.Execute("INSERT INTO [].Log (Level, Text)", "ERROR", text.Replace("'", string.Empty));
				Logs.Insert(0, new PushyLog(PushyLogType.ERR, text));
				PConsole.PrintLogs();
			}
		}
		
		public static void PreformCleanup()
		{
			int maxNumber = 18;
			lock(lockObj)
			{
				if(Logs.Count > maxNumber)
					for (int i = maxNumber - 1; i < Logs.Count; i++)
						Logs.RemoveAt(i);
			}
		}

	}
}
