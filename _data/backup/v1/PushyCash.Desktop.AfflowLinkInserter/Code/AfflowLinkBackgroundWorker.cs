using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PushyCash.Desktop.AfflowLinkInserter.Code
{
	public class AfflowLinkBackgroundWorker
	{
		public AfflowLinkBackgroundWorker()
		{
			new Thread(() =>
			{
				List<string> result = new List<string>();
				MainForm.AfflowDataLinks = MainForm.AfflowManager.GetLinkList();
				foreach (Afflow.Models.AfflowLink link in MainForm.AfflowDataLinks)
					result.Add(link.FullName);
				MainForm.AfflowLinks = result;
				MessageBox.Show("Afflowlinks loaded");
			}).Start();
		}
		
	}
}
