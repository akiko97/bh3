namespace MoleMole
{
	public class IdleMPPeer : MPPeer
	{
		public static IdleMPPeer IDLE_PEER = new IdleMPPeer();

		public override int channelSequenceCapacity
		{
			get
			{
				return 0;
			}
		}

		public override int peerID
		{
			get
			{
				return 0;
			}
		}

		public override int reliableChannel
		{
			get
			{
				return 0;
			}
		}

		public override PeerState state
		{
			get
			{
				return PeerState.Unitialized;
			}
		}

		public override int stateUpdateChannel
		{
			get
			{
				return 0;
			}
		}

		public override int totalPeerCount
		{
			get
			{
				return 0;
			}
		}

		public override void Connect(string ipAddress, int serverPort)
		{
		}

		public override void Core()
		{
		}

		public override void GetPeerStats(out ulong sendTotal, out ulong recvTotal, out ulong sendCount, out ulong recvCount)
		{
			sendTotal = 0uL;
			recvTotal = 0uL;
			sendCount = 0uL;
			recvCount = 0uL;
		}

		public override void GetPeerStats2(out string stat2)
		{
			stat2 = string.Empty;
		}

		public override void Init()
		{
		}

		public override void Listen(int serverPort = 0, string ipAddress = null)
		{
		}

		public override void SendByChannel(byte[] data, int len, int channel, int channelSequence)
		{
		}

		public override void ServerReady()
		{
		}

		public override void Shutdown()
		{
		}

		public override void StopListen()
		{
		}
	}
}
