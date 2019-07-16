using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PushyCash.Desktop.AfflowLinkInserter.Forms
{
	public partial class GetAfflowLinksForm : Form
	{
		private string ExoclickPattern = "https://win.ningoffer.club/?utm_medium={HASH}&utm_campaign={campaign_id}&1={src_hostname}&2={site_id}&3={zone_id}&cid={conversions_tracking}";
		private string PopadsPattern = "https://win.ningoffer.club/?utm_medium={HASH}&utm_campaign=campaign_name&1=[WEBSITEID]&2=[KEYWORD]&3=[CATEGORYID]&4=[BID]&5=[QUALITY]";
		private string ZeroparkPattern = "https://win.ningoffer.club/?utm_medium={HASH}&utm_campaign={long_campaign_id}&1={keyword}&2={match}&3={target}&cid={cid}";

		public GetAfflowLinksForm()
		{
			InitializeComponent();
		}

		private void GetAfflowLinksForm_Load(object sender, EventArgs e)
		{
			this.Show();
			foreach(Afflow.Models.AfflowLink link in MainForm.AfflowDataLinks)
			{
				textBox.Text += link.FullName + Environment.NewLine;
				if (link.TrafficNetwork.Equals("zeropark"))
					textBox.Text += ZeroparkPattern.Replace("{HASH}", link.hash);
				if (link.TrafficNetwork.Equals("popads"))
					textBox.Text += PopadsPattern.Replace("{HASH}", link.hash);
				if (link.TrafficNetwork.Equals("exoclick"))
					textBox.Text += ExoclickPattern.Replace("{HASH}", link.hash);
				textBox.Text += "" + Environment.NewLine + Environment.NewLine;
				Application.DoEvents();
			}
		}
	}
}
