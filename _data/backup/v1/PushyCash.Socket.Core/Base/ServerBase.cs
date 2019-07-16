using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PushyCash.Sockets.Core.Base
{
	public abstract class ServerBase
	{
		private object LockObj = new object();
		private byte[] _buffer = new byte[25600];
		protected List<Socket> ClientSockets { get; set; } = new List<Socket>();
		protected Socket ServerSocket { get; set; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		// Overridable actions
		public Action OnNewClientConnection = null;
		public Action OnClientDisconection = null;
		public Action OnDataReceived = null;
		
		public ServerBase()
		{
			SetupServer();
		}

		// SUMMARY: Start server listening
		protected virtual void SetupServer()
		{
			Console.WriteLine("Setting up server..");
			ServerSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
			ServerSocket.Listen(1);
			ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
		}
		
		// SUMMARY: On client connected
		protected virtual void AcceptCallback(IAsyncResult AR)
		{
			Socket socket = ServerSocket.EndAccept(AR);
			try
			{
				lock (this.LockObj)
					ClientSockets.Add(socket);
				Console.WriteLine("Client connected");
				this.OnNewClientConnection?.Invoke();

				socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
				ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
			}
			catch (Exception e)
			{
				lock (this.LockObj)
					ClientSockets.Remove(socket);
				OnClientDisconection?.Invoke();
			}
		}

		// SUMMARY: On receive message from client
		protected virtual void ReceiveCallback(IAsyncResult AR)
		{
			Socket socket = (Socket)AR.AsyncState;
			try
			{
				int received = socket.EndReceive(AR);
				byte[] dataBuf = new byte[received];
				Array.Copy(_buffer, dataBuf, received);

				socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
			}
			catch (Exception e)
			{
				lock (this.LockObj)
					ClientSockets.Remove(socket);
				OnClientDisconection?.Invoke();
			}
		}

		// SUMMARY: Send model to client
		public void SendModel(PushyDistributionModel model)
		{
			if (ClientSockets.Count == 0)
				return;

			lock (this.LockObj)
			{
				foreach (Socket socket in this.ClientSockets)
					try
					{
						byte[] data = PushyDistributionModelManager.GetBytesFromObj(model);
						socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
					}
					catch (Exception e) { }
			}
		}

		// SUMMARY: On data sent to client
		private void SendCallback(IAsyncResult AR)
		{
			Socket socket = (Socket)AR.AsyncState;
			socket.EndSend(AR);
		}
	}
}
