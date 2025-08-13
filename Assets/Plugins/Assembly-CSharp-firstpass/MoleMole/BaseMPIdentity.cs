namespace MoleMole
{
	public abstract class BaseMPIdentity
	{
		public uint runtimeID;

		public bool isOwner;

		public bool isAuthority
		{
			get
			{
				return isOwner;
			}
		}

		public int authorityPeerID
		{
			get
			{
				return GetPeerID(runtimeID);
			}
		}

		public abstract IdentityRemoteMode remoteMode { get; }

		public static int GetPeerID(uint runtimeID)
		{
			return (int)(runtimeID >> 29);
		}

		public static uint PackRuntimeID(int peerID, uint rest)
		{
			return (uint)(peerID << 29) | rest;
		}

		public int GetPeerID()
		{
			return GetPeerID(runtimeID);
		}

		public virtual void PreInitReplicateRemote(MPRecvPacketContainer pc)
		{
		}

		public virtual void Init()
		{
		}

		public virtual void OnRemoval()
		{
		}

		public virtual void OnReliablePacket(MPRecvPacketContainer pc)
		{
		}

		public virtual void OnStateUpdatePacket(MPRecvPacketContainer pc)
		{
		}

		public virtual void Core()
		{
		}

		public virtual void OnAuthorityStart()
		{
		}

		public virtual void OnRemoteStart()
		{
		}
	}
}
