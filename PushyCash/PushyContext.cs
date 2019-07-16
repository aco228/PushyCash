using PushyCash.Direct;
using PushyCash.Direct.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash
{
	public static class PushyContext
	{
		public static Dictionary<string, Dictionary<string, Workers.WorkerBase>> Threads { get; set; } = null; // Primary Key _ Secondary Key _ Thread
		

		public static Afflow.AfflowManager AfflowManager = null;
		public static Snapshots.SnapshotManager SnapshotManager = null;
		public static Links.LinkManager LinkManager = null;
		public static Sockets.Core.PushyDistributionServer SocketServer = null;
		public static Slack.SlackManager Slack = null;
		public static TrafficNetworkManager TrafficNetworkManager = null;
		public static Campaigns.PushyCampaignManager CampaignManager = null;

		public static Action OnInitializationFinishedAction = null;
		public static Action OnEntireLoadProcessIsFinished = null;

		public static void Init()
		{
			AfflowManager = new Afflow.AfflowManager();
			AfflowManager.OnServerFatal = OnServerApiErrorsCallback;
			SnapshotManager = new Snapshots.SnapshotManager();
			LinkManager = new Links.LinkManager();
			Slack = new PushyCash.Slack.SlackManager();
			Slack.OnFatal = OnSlackException;
			TrafficNetworkManager = new TrafficNetworkManager();
			SocketServer = new Sockets.Core.PushyDistributionServer();
			CampaignManager = new Campaigns.PushyCampaignManager();
			
			Console.Title = "PushyCash | Server";
			PConsole.Current = new PConsole();
			PushyGlobal.ApplicationStarted = DateTime.Now;
			
			Console.WriteLine("Application started. Loading...");
			_totalNumberOfExpectedLoads++; SnapshotManager.Update();
			_totalNumberOfExpectedLoads++; LinkManager.Update();
			_totalNumberOfExpectedLoads++; TrafficNetworkManager.Update();

			Threads = new Dictionary<string, Dictionary<string, Workers.WorkerBase>>();
			new Workers.AfflowLiveFeedWorker();
			new Workers.AfflowLoginWorker();
			//new Workers.AfflowSnapshotUpdateWorker();
			//new Workers.AfflowConversionsFeedWorker();
			new Workers.GarbageCollectorLinkWorker();
			new Workers.GarbageCollectorSnapshotOfferWorker();
			new Workers.ConsoleDisplayDataWorker();
			new Workers.ActiveCampaignsWorker();
			new Workers.TrafficSourceUpdateStatsWorker();
			new Workers.SingleThreadWorkers.SingleThreadWorker();
			Workers.WorkerBase.ControlExecutionTime();

			PushyCashDirect.Instance.OnFatalAction = OnDatabaseException;
			OnInitializationFinishedAction?.Invoke();
		}

		#region # Initial loads # 

		private static int _totalNumberOfExpectedLoads = 0;
		private static int _currentNumberOfLoads = 0;
		private static bool _initialLoadIsFinished = false;

		public static void OnLoadFinished(string nameOfProcess)
		{
			Console.WriteLine("FinishedLoading = " + nameOfProcess);
			if (_initialLoadIsFinished)
				return;

			_currentNumberOfLoads++;
			if (_currentNumberOfLoads < _totalNumberOfExpectedLoads)
				return;

			_initialLoadIsFinished = true;
			PreformMapping();

			Console.WriteLine("Loading finish and mapping");
			
			OnEntireLoadProcessIsFinished?.Invoke();
		}

		public static void StartCollecting()
		{
			AfflowManager.BeforeCollectionsStarts();

			foreach (var workerKeys in Threads)
				foreach (var worker in workerKeys.Value)
					worker.Value.Start();

			PConsole.ReadyForPrinting = true;
			Console.SetBufferSize(1200, 800);
			
			Console.Clear();
			PLogger.Debug("PushyDeamon started");
		}

		#endregion

		#region # Mapping #

		private static void PreformMapping()
		{
			if(LinkManager != null)
			{
				LinkManager.MapDataWithSnapshot();
				LinkManager.MapDataWithTrafficCampaigns();
			}
		}

		#endregion

		// SUMMARY: Callback function when server exception happens in any of modules
		public static void OnServerApiErrorsCallback(string threadKey, string text)
		{
			if (PushyContext.Threads.ContainsKey(threadKey))
				foreach (var thread in PushyContext.Threads[threadKey])
					foreach (var tt in PushyContext.Threads[threadKey].Values)
						tt.WaitUntilDate = DateTime.Now.AddMinutes(5);
			
			PLogger.Error(text);
		}
		
		public static void OnDatabaseException(string exception)
		{
			PLogger.Error(exception);
		}

		public static void OnSlackException(string exception)
		{

		}

	}
}
