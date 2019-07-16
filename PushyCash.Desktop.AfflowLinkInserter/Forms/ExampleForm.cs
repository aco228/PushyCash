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
	public partial class ExampleForm : Form
	{
		public ExampleForm()
		{
			InitializeComponent();
		}

		public static string ExampleText = @"IT mainstream #  tim, wifi, wind, 3, vodafone
SA mainstream #  wifi, zain, mobily, jawal
CA mainstream #  wifi
FR adult #  wifi, sfr, orange, bouygues
AE mainstream #  wifi, etisalat, du
CA aggressive #   wifi, bell, rogers, videotron, telus
AU mainstream #  wifi, telstra, vodafone, optus
RU mainstream #  tele2, mts, wifi, beeline, megafon
IQ mainstream #  wifi, asia, zain
US aggressive #   wifi, mobile, sprint, verizon, att
IT aggressive #   wind, tim, 3, wifi, vodafone
TH adult #  ais, dtac, TRUEMOVE, wifi
ES mainstream #  wifi, vodafone, orange, movistar
DE aggressive #   wifi, o2, telekom, vodafone
EG mainstream #  wifi, vodafone, orange, etisalat
GR aggressive #   wifi, cosmote, vodafone, wind
MX adult #  wifi, telcel, movistar, att
GB mainstream #  wifi, o2, 3, orange, lycamobile, vodafone, t-mobile
FR aggressive #   mobile, wifi, BOUYGUES, nrj, sfr, orange
MY mainstream #  maxis, wifi, digi, celcom, mobile
IR mainstream #  mtn, ir-mci, wifi, rightel
IN aggressive #   jio, airtel, wifi, idea, vodafone, cellone
JO mainstream #  zain, wifi, UMNIAH, orange
US adult #  wifi, t-mobile, att, sprint, verizon
AE adult #  wifi
JP aggressive #   wifi, docomo, au, softbank
BR aggressive #   wifi, vivo, tim
MA mainstream #  iam, wifi, inwi";

		private void ExampleForm_Load(object sender, EventArgs e)
		{
			this.textBox.Text = ExampleText;
		}
	}
}
