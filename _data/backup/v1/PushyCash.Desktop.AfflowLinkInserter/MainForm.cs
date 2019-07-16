using PushyCash.Afflow.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PushyCash.Desktop.AfflowLinkInserter
{
	public partial class MainForm : Form
	{
		public static Afflow.AfflowManager AfflowManager = null;
		public static string[] AvaliableDevices = new string[] { "Android", "iOS" };
		public static List<AfflowLink> AfflowDataLinks = new List<AfflowLink>();
		public static List<string> NetworkNames = new List<string>();
		public static List<string> AfflowLinks = null;

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			AfflowManager = new Afflow.AfflowManager();
			new Code.AfflowLinkBackgroundWorker();
			PushyContext.TrafficNetworkManager = new TrafficNetworkManager();
			foreach (TrafficNetworks.Core.TrafficSourceBase ts in PushyContext.TrafficNetworkManager.NetworksList)
				NetworkNames.Add(ts.Name);
			
			this.comboBoxNetworks.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxNetworks.Items.AddRange(NetworkNames.ToArray());
			this.comboBoxNetworks.SelectedIndex = 0;

			this.textBox.Text = ExampleForm.ExampleText;
		}

		private void btnExample_Click(object sender, EventArgs e)
		{
			(new ExampleForm()).Show();
		}

		private void btnSubmit_Click(object sender, EventArgs e)
		{
			this.textBoxLogs.Text = "";
			if(AfflowLinks == null || AfflowLinks.Count == 0)
			{
				MessageBox.Show("Afflow reference links are not loaded. Please wait until it is finished");
				return;
			}

			if(string.IsNullOrEmpty(this.textBox.Text))
			{
				MessageBox.Show("Write something idiot");
				return;
			}

			string[] Data = this.textBox.Text.Split('\n');
			string networkName = NetworkNames[this.comboBoxNetworks.SelectedIndex];
			TrafficNetworks.Core.TrafficSourceAfflowMapBase afflowMap = PushyContext.TrafficNetworkManager[networkName].AfflowMap;

			for (int i = 0; i < Data.Length; i++)
			{
				string d = Data[i];
				string[] mainSplit = d.Split('#');
				string country = mainSplit[0].Substring(0, 3).Replace(" ", string.Empty);
				string vertical = mainSplit[0].Substring(3, mainSplit[0].Length - 3).Trim();
				string[] operatorsSplit = mainSplit[1].Trim().Split(',');

				foreach (string o in operatorsSplit)
				{
					if (string.IsNullOrEmpty(o))
						continue;

					string realOperatorName = AfflowManager.MobileOperatorMap.GetName(country, o);
					if (string.IsNullOrEmpty(realOperatorName))
						continue;

					foreach (string device in AvaliableDevices)
					{
						string name = string.Format("_{0}.{1}.{2}.{3}.{4}", country, o.Trim(), vertical, device.ToLower(), networkName);
						if(AfflowLinks.Contains(name))
						{
							this.textBoxLogs.Text = " -- EXISTS -- " + name + Environment.NewLine + this.textBoxLogs.Text;
							continue;
						}
						
						string postData =
										"&rb_url=null"
									+ "&safe_page_rcid={rcid}"
									+ "&disable_bbr=0"
									+ "&disable_popunder=0"
									+ "&whitelist_data={'geo':['{COUNTRY}'],'os':['{DEVICE}'],'carrier':[{'geo':'{COUNTRY}','carrier':'{COUNTRY} {OPERATOR}'}],'city':null}"
									+ "&options_data=" + afflowMap.DefaultOptionsData
									+ "&vertical={VERTICAL}"
									+ "&adult=" + (vertical.Contains("adult") ? "1" : "0")
									+ "&allow_aggressive=1"
									+ "&name={NAME}"
									+ "&nid=" + afflowMap.NID // league
									+ "&bbr_rcid={rcid}"
									+ "&desktop_redirect=1"
									+ "&popunder_rcid={rcid}";
						postData = postData.Replace("'", "\"")
									.Replace("{COUNTRY}", country)
									.Replace("{DEVICE}", device)
									.Replace("{OPERATOR}", realOperatorName)
									.Replace("{VERTICAL}", vertical)
									.Replace("{NAME}", name);
						
						AfflowManager.InsertLink(postData);
						this.textBoxLogs.Text = ">> INSERTED >> " + name + Environment.NewLine + this.textBoxLogs.Text;
					}
				}
			}
		}

		private void btnAfflowUrls_Click(object sender, EventArgs e)
		{
			if (MainForm.AfflowDataLinks == null)
				return;
			(new Forms.GetAfflowLinksForm()).Show();
		}
	}
}
