using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MoleMole
{
	public class MiClient : MiClientInterface
	{
		private string host_;

		private ushort port_;

		private Socket socket_;

		private ManualResetEvent timeout_event_;

		private int timeout_ms_;

		private Thread client_producer_thread_;

		private Queue<NetPacketV1> recv_queue_;

		private bool connected_before_;

		private Action disconnect_callback_;

		private Action<NetPacketV1> doCmd_callback_;

		private int keepalive_time_ms_;

		private DateTime last_keepalive_time_;

		private NetPacketV1 keepalive_packet_;

		private byte[] left_buf_;

		private int left_buf_len_;

		private EndPoint _remoteEndPoint;

		public string Host
		{
			get
			{
				return host_;
			}
		}

		public ushort Port
		{
			get
			{
				return port_;
			}
		}

		public MiClient()
		{
			host_ = string.Empty;
			port_ = 0;
			socket_ = null;
			timeout_event_ = new ManualResetEvent(false);
			timeout_ms_ = 0;
			client_producer_thread_ = null;
			recv_queue_ = new Queue<NetPacketV1>();
			connected_before_ = false;
			disconnect_callback_ = null;
			doCmd_callback_ = null;
			keepalive_time_ms_ = 0;
			last_keepalive_time_ = TimeUtil.Now;
			keepalive_packet_ = null;
			left_buf_ = new byte[262144];
			left_buf_len_ = 0;
		}

		~MiClient()
		{
			disconnect();
		}

		public bool connect(string host, ushort port, int timeout_ms = 2000)
		{
			try
			{
				if (isConnected())
				{
					return false;
				}
				host_ = host;
				port_ = port;
				timeout_ms_ = timeout_ms;
				IPAddress[] hostAddresses = Dns.GetHostAddresses(host_);
				if (hostAddresses.Length == 0)
				{
					return false;
				}
				socket_ = null;
				_remoteEndPoint = null;
				for (int i = 0; i < hostAddresses.Length; i++)
				{
					IPAddress iPAddress = GetIPAddress(hostAddresses[i]);
					IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, port_);
					Socket socket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					socket.NoDelay = true;
					socket.Blocking = true;
					socket.SendTimeout = timeout_ms_;
					socket.ReceiveTimeout = timeout_ms_;
					socket.ReceiveBufferSize = 524288;
					timeout_event_.Reset();
					socket.BeginConnect(iPAddress, port_, connectCallback, socket);
					timeout_event_.WaitOne(timeout_ms, false);
					if (socket.Connected)
					{
						socket_ = socket;
						_remoteEndPoint = iPEndPoint;
						break;
					}
				}
				if (socket_ == null)
				{
					return false;
				}
				startClientThread();
				connected_before_ = true;
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		private void connectCallback(IAsyncResult asyncresult)
		{
			try
			{
				Socket socket = asyncresult.AsyncState as Socket;
				socket.EndConnect(asyncresult);
			}
			catch (SystemException)
			{
			}
			finally
			{
			}
			timeout_event_.Set();
		}

		public void disconnect()
		{
			connected_before_ = false;
			if (isConnected())
			{
				if (socket_ != null)
				{
					socket_.Close();
				}
				socket_ = null;
				timeout_event_.Set();
			}
		}

		public bool isConnected()
		{
			if (socket_ == null)
			{
				return false;
			}
			bool flag = true;
			try
			{
				if (_remoteEndPoint == null)
				{
					flag = false;
				}
			}
			catch (SystemException)
			{
				flag = false;
			}
			if (!flag && connected_before_)
			{
				connected_before_ = false;
				if (disconnect_callback_ != null)
				{
					disconnect_callback_();
				}
			}
			return flag;
		}

		public bool send(NetPacketV1 packet)
		{
			if (packet == null)
			{
				return false;
			}
			if (!isConnected())
			{
				return false;
			}
			MemoryStream ms = new MemoryStream();
			if (!packet.serialize(ref ms))
			{
				return false;
			}
			try
			{
				SocketError error = SocketError.Success;
				int num = socket_.Send(ms.GetBuffer(), 0, (int)ms.Length, SocketFlags.None, out error);
				if (error != SocketError.Success)
				{
					return false;
				}
				if (num != ms.Length)
				{
					return false;
				}
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		private List<NetPacketV1> recvPacketList()
		{
			if (!isConnected())
			{
				return null;
			}
			List<NetPacketV1> list = new List<NetPacketV1>();
			try
			{
				byte[] array = new byte[262144];
				SocketError error = SocketError.Success;
				int num = socket_.Receive(array, 0, array.Length, SocketFlags.None, out error);
				switch (error)
				{
				case SocketError.IOPending:
				case SocketError.WouldBlock:
				case SocketError.TimedOut:
					return list;
				case SocketError.Success:
					if (num != 0)
					{
						byte[] array2 = new byte[262144];
						Array.Copy(left_buf_, 0, array2, 0, left_buf_len_);
						Array.Copy(array, 0, array2, left_buf_len_, num);
						int num2 = num + left_buf_len_;
						left_buf_len_ = 0;
						NetPacketV1 netPacketV;
						for (int num3 = num2; num3 > 0; list.Add(netPacketV), num3 -= netPacketV.getPacketLen())
						{
							byte[] buf = new byte[num3];
							Array.Copy(array2, num2 - num3, buf, 0, num3);
							netPacketV = new NetPacketV1();
							switch (netPacketV.deserialize(ref buf))
							{
							case PacketStatus.PACKET_NOT_COMPLETE:
								left_buf_len_ = num3;
								Array.Copy(buf, 0, left_buf_, 0, left_buf_len_);
								break;
							case PacketStatus.PACKET_CORRECT:
								continue;
							}
							break;
						}
						break;
					}
					goto default;
				default:
					socket_.Close();
					socket_ = null;
					return list;
				}
			}
			catch (SystemException)
			{
				if (!isConnected())
				{
				}
			}
			return list;
		}

		private void clientProducerThreadHandler()
		{
			while (isConnected())
			{
				try
				{
					List<NetPacketV1> list = recvPacketList();
					if (list != null && list.Count != 0)
					{
						foreach (NetPacketV1 item in list)
						{
							lock (recv_queue_)
							{
								recv_queue_.Enqueue(item);
							}
						}
						timeout_event_.Set();
					}
					keepalive();
				}
				catch (SystemException)
				{
				}
			}
			disconnect();
		}

		private void clientConsumerThreadHandler()
		{
			while (isConnected() || 0 < recv_queue_.Count)
			{
				NetPacketV1 netPacketV = recv(-1);
				if (netPacketV != null)
				{
					doCmd_callback_(netPacketV);
				}
			}
		}

		public NetPacketV1 recv(int timeout_ms = 0)
		{
			lock (recv_queue_)
			{
				if (0 < recv_queue_.Count)
				{
					return recv_queue_.Dequeue();
				}
			}
			if (!isConnected())
			{
				return null;
			}
			if (timeout_ms == 0)
			{
				return null;
			}
			timeout_event_.Reset();
			timeout_event_.WaitOne(timeout_ms, false);
			lock (recv_queue_)
			{
				if (recv_queue_.Count == 0)
				{
					return null;
				}
				return recv_queue_.Dequeue();
			}
		}

		private bool isClientThreadRun()
		{
			return client_producer_thread_ != null && client_producer_thread_.IsAlive;
		}

		private bool startClientThread()
		{
			if (isClientThreadRun())
			{
				return false;
			}
			client_producer_thread_ = new Thread(clientProducerThreadHandler);
			client_producer_thread_.Start();
			return true;
		}

		public void setDisconnectCallback(Action callback)
		{
			disconnect_callback_ = callback;
		}

		public void setCmdCallBack(Action<NetPacketV1> callback)
		{
			doCmd_callback_ = callback;
		}

		public bool setKeepalive(int time_ms, NetPacketV1 packet)
		{
			if (time_ms <= 0 || packet == null)
			{
				return false;
			}
			keepalive_time_ms_ = time_ms;
			keepalive_packet_ = packet;
			return true;
		}

		private void keepalive()
		{
			if (keepalive_time_ms_ != 0 && keepalive_packet_ != null && (TimeUtil.Now - last_keepalive_time_).TotalMilliseconds >= (double)keepalive_time_ms_)
			{
				send(keepalive_packet_);
				last_keepalive_time_ = TimeUtil.Now;
			}
		}

		public static bool IsIPV6()
		{
			return false;
		}

		public static IPAddress GetIPAddress(IPAddress ip)
		{
			return ip;
		}
	}
}
