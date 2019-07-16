using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PushyCash.Workers
{
	public abstract class WorkerBase
	{
		private bool _preformStop = false;

		public string PrimaryKey { get; private set; } = string.Empty;
		public string SecondaryKey { get; private set; } = string.Empty;
		public int Timeout { get; protected set; } = 0;
		public bool Running { get; protected set; } = false;
		public bool StartImmediatly { get; protected set; } = false;
		public DateTime LastExecution { get; protected set; } = DateTime.Now;
		public DateTime? WaitUntilDate { get; set; } = null;
		public Thread Thead = null;

		public WorkerBase(string primaryKey, string secondaryKey, int timeout, bool startImmediatly = false)
		{
			this.PrimaryKey = primaryKey;
			this.SecondaryKey = secondaryKey;
			this.Timeout = timeout;
			this.StartImmediatly = startImmediatly;

			if (!PushyContext.Threads.ContainsKey(this.PrimaryKey))
				PushyContext.Threads.Add(this.PrimaryKey, new Dictionary<string, WorkerBase>());
			if (!PushyContext.Threads[this.PrimaryKey].ContainsKey(this.SecondaryKey))
				PushyContext.Threads[this.PrimaryKey].Add(this.SecondaryKey, this);
		}

		//
		// Abstract methods
		//

		public abstract void OnLoop();

		//
		// Public methods
		//

		public void Start()
		{
			if (this.Running)
				return;
			this.Running = true;

			this.Thead = new Thread(() =>
			{
				for (; ; )
				{
					if(!this.StartImmediatly)
						Thread.Sleep(this.Timeout);

					if (this.WaitUntilDate.HasValue && this.WaitUntilDate > DateTime.Now)
						continue;

					if (this.WaitUntilDate.HasValue)
						this.WaitUntilDate = null;

					if (this._preformStop)
					{
						this._preformStop = false;
						break;
					}

					try
					{
						this.OnLoop();
					}
					catch (Exception e)
					{
						PLogger.Error($"Worker.{this.PrimaryKey}.{this.SecondaryKey} EXCEPTION:: " + e.ToString());
					}

					this.LastExecution = DateTime.Now;
					
					if (this.StartImmediatly)
						Thread.Sleep(this.Timeout);
				}
			});
			this.Thead.Start();
		}

		public void Stop()
		{
			this._preformStop = true;
		}

		// SUMMARY: Kill locked thread and start it over
		public static void ControlExecutionTime()
		{
			new Thread(() =>
			{
				for (; ; )
				{
					Thread.Sleep(10000);
					try
					{
						foreach (var threadPrimaryKey in PushyContext.Threads)
							foreach (var thread in threadPrimaryKey.Value)
								if (thread.Value.WaitUntilDate.HasValue && (DateTime.Now - thread.Value.LastExecution).TotalMinutes > 5)
								{
									thread.Value.Stop();
									PushyContext.Threads[thread.Value.PrimaryKey][thread.Value.SecondaryKey].Stop();
									PushyContext.Threads[thread.Value.PrimaryKey][thread.Value.SecondaryKey].Thead.Abort();

									//PushyContext.Threads[thread.Value.PrimaryKey][thread.Value.SecondaryKey] =
									//	(WorkerBase)Activator.CreateInstance(thread.Value.GetType(), thread.Value.PrimaryKey, thread.Value.SecondaryKey, thread.Value.Timeout);

									PLogger.Error(string.Format("Worker '{0}' timeout for 5 minute.. whole server will be restarted", thread.Key));
									File.Create(PushyExternalStorage.StoragePath + "run.txt");
									Environment.Exit(0);
								}
					}
					catch (Exception e) { }
				}
			}).Start();
		}


	}
}
