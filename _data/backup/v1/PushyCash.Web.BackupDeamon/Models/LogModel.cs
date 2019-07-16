using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PushyCash.Web.BackupDeamon.Models
{
	public class LogModel
	{
		public DateTime Created { get; set; } = DateTime.Now;
		public string Text { get; set; } = string.Empty;
	}
}