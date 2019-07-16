using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.PushyConsole
{
	internal abstract class PConsoleControlBase
	{
		public static object lockObj = new object();
		private string WhiteLine { get; set; } = string.Empty;

		public int Top { get; protected set; } = 0;
		public int Left { get; protected set; } = 0;
		public int Width { get; protected set; } = 0;
		public int CurrentLineCount { get; protected set; } = 0;
		public int LastLineCount { get; protected set; } = 0;
	
		public PConsoleControlBase(int top, int left, int width)
		{
			this.Top = top;
			this.Left = left;
			this.Width = width;
			this.WhiteLine = new StringBuilder().Append(' ', this.Width).ToString();
		}

		public void OnNewIteration()
		{
			this.LastLineCount = this.CurrentLineCount;
			this.CurrentLineCount = Top;
		}

		public void Clear()
		{
			if (LastLineCount == 0)
				return;

			if (LastLineCount > this.CurrentLineCount)
				for (int i = 0; i < LastLineCount - CurrentLineCount; i++)
					Console.WriteLine(WhiteLine);

		}
		
		protected void WriteLine()
		{
			lock(lockObj)
			{
				Console.SetCursorPosition(Left, CurrentLineCount);
				Console.WriteLine(WhiteLine);
				this.CurrentLineCount++;
			}
		}

		protected void WriteLine(string text)
		{
			lock(lockObj)
			{
				Console.SetCursorPosition(Left, CurrentLineCount);
				Console.WriteLine(this.TrimString(text, Width));
				this.CurrentLineCount++;
			}
		}

		protected void SetCursorPosition(int left, int top)
		{
			lock(lockObj)
			{
				Console.SetCursorPosition(left, top);
			}
		}

		protected void Write(string text)
		{
			lock(lockObj)
			{
				Console.Write(text);
			}
		}

		protected string TrimString(string input, int expectedCount)
		{
			if (input.Length > expectedCount)
				input = input.Substring(0, expectedCount);
			else
				input += new StringBuilder().Append(' ', (expectedCount - input.Length)).ToString();
			return input;
		}



	}
}
