using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.ConsoleApp.BackupDeamon
{
	class Program
	{
		static void Main(string[] args)
		{
			Process[] pList = Process.GetProcesses();
			foreach (Process p in pList)
				if (p.ProcessName.Contains("PushyCash.ConsoleApp.Deamon"))
				{
					File.WriteAllText(@"D:\Projects\AkoProjects\dot.net\PushyCash\PushyCash.ConsoleApp.Deamon\bin\Debug\Storage\_backupDeamon.txt", DateTime.Now.ToString());
					Environment.Exit(0);
					return;
				}

			PushyContext.TrafficNetworkManager = new TrafficNetworkManager();
			PushyContext.AfflowManager = new Afflow.AfflowManager();

			PushyContext.TrafficNetworkManager.Update();
			PushyContext.TrafficNetworkManager.OnLoadedAction = OnTrafficManagerLoaded;
			Console.ReadKey();
		}

		static void OnTrafficManagerLoaded()
		{
			bool needForUpdate = false;
			foreach (var ts in PushyContext.TrafficNetworkManager.Networks)
				foreach (var tsc in ts.Value.CampaignsList)
					if (tsc.Data.IsCampaignActive)
					{
						Console.WriteLine("Campaign " + tsc.FullName + " is stoped");
						tsc.Stop();
						needForUpdate = true;
					}

			if (needForUpdate)
				PushyContext.TrafficNetworkManager.SaveStorageTrafficData();

			if (File.Exists(PushyExternalStorage.StoragePath + "run.txt"))
			{
				Process.Start(@"D:\Projects\AkoProjects\dot.net\PushyCash\PushyCash.ConsoleApp.Deamon\bin\Debug\PushyCash.ConsoleApp.Deamon.exe");
				File.Delete(PushyExternalStorage.StoragePath + "run.txt");
			}

			File.WriteAllText(@"D:\Projects\AkoProjects\dot.net\PushyCash\PushyCash.ConsoleApp.Deamon\bin\Debug\Storage\_backupDeamon.txt", DateTime.Now.ToString());
			Environment.Exit(0);
		}

	}
}
