using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Workers.SingleThreadWorkers
{
	public abstract class SingleThreadOperationBase
	{
		private bool Executing { get; set; } = false;
		private int DelayInSeconds { get; set; }
		public DateTime NextCall { get; protected set; }
		public bool ShouldBeCalled { get => !Executing && (DateTime.Now > this.NextCall); }

		public SingleThreadOperationBase(int delayInSeconds, bool executeImmediately = false)
		{
			this.DelayInSeconds = delayInSeconds;

			if (!executeImmediately)
				this.NextCall = DateTime.Now.AddSeconds(this.DelayInSeconds);
			else
				this.NextCall = DateTime.Now.AddSeconds(-1);
		}

		protected abstract void OnCall();

		public void Call()
		{
			this.Executing = true;
			try
			{
				this.OnCall();
			}
			catch (Exception e)
			{
				int a = 0;
			}

			this.Executing = false;
			this.NextCall = DateTime.Now.AddSeconds(this.DelayInSeconds);
		}
		
	}
}
