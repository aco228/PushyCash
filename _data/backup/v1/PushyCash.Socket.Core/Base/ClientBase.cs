using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Sockets.Core.Base
{
	public abstract class ClientBase
	{
		private byte[] _buffer = new byte[25600];
		private bool _clientWasConnected = false;
		protected Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		// Overridable actions
		public Action<PushyDistributionModel> OnMessageReceived = null;
		public Action OnConnection = null;
		public Action OnDisconnection = null;

		public ClientBase()
		{
			LoopConnect();
		}

		public void LoopConnect()
		{
			Console.WriteLine("Trying to connect...");
			if (_clientWasConnected)
			{
				ClientSocket.Shutdown(SocketShutdown.Both);
				ClientSocket.Disconnect(true);
				ClientSocket.Close();
				ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				this.OnDisconnection?.Invoke();
			}

			while (!ClientSocket.Connected)
			{
				try { ClientSocket.Connect(new IPEndPoint(IPAddress.Parse("192.168.38.138"), 100)); }
				catch (Exception e) { }
				System.Threading.Thread.Sleep(1000);
			}

			this.OnConnection?.Invoke();
			_clientWasConnected = true;
			Console.WriteLine("Connected..");

			ClientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
		}

		private void ReceiveCallback(IAsyncResult AR)
		{
			try
			{
				int received = ClientSocket.EndReceive(AR);
				System.Diagnostics.Debug.WriteLine("received: " + received);
				

				byte[] dataBuf = new byte[received];
				Array.Copy(_buffer, dataBuf, received);

				PushyDistributionModel modelReceived = PushyDistributionModelManager.ConvertToModel(dataBuf);
				if (modelReceived != null)
					OnMessageReceived?.Invoke(modelReceived);

				ClientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
			}
			catch (Exception e)
			{
				if (!this.ClientSocket.Connected)
					this.LoopConnect();
				else
					ClientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
			}
		}

	}
}
