namespace MoleMole
{
	public class RuntimeIDManager
	{
		private const uint PEER_MASK = 3758096384u;

		private const uint CATEGORY_MASK = 520093696u;

		private const uint IS_SYNCED_MASK = 8388608u;

		private const uint SEQUENCE_MASK = 8388607u;

		public const int PEER_SHIFT = 29;

		public const int CATEGORY_SHIFT = 24;

		public const int IS_SYNCED_SHIFT = 23;

		public const int SEQUENCE_SHIFT = 0;

		private const ushort MAX_PEER = 8;

		private const ushort MAX_CATEGORY = 32;

		private const uint MAX_SQUENCE = 8388608u;

		public const ushort MANAGER_CATE = 1;

		public const ushort CAMERA_CATE = 2;

		public const ushort AVATAR_CATE = 3;

		public const ushort MONSTER_CATE = 4;

		public const ushort EFFECT_CATE = 5;

		public const ushort DYNAMICOBJECT_CATE = 6;

		public const ushort PROPOBJECT_CATE = 7;

		public const int CATEGORY_COUNT = 7;

		public const uint LEVEL_RUNTIMEID = 562036737u;

		public const uint AVATAR_RESERVERED_SEQ_ID = 10u;

		private uint _networkedNextSeqID = 20u;

		private uint _localNextSeqID = 20u;

		private uint _peerID;

		private RuntimeIDManager()
		{
			_peerID = 1u;
		}

		public uint GetFixedAvatarRuntimeIDForPeer(int peerID)
		{
			return (uint)((peerID << 29) | 0x3000000 | 0x800000 | (int)(10L + (long)peerID));
		}

		public void SetupPeerID(int peerID)
		{
			_peerID = (uint)peerID;
		}

		public void InitAtAwake()
		{
		}

		public uint GetNextRuntimeID(ushort category)
		{
			_networkedNextSeqID++;
			return (_peerID << 29) | (uint)(category << 24) | 0x800000 | _networkedNextSeqID;
		}

		public uint GetNextNonSyncedRuntimeID(ushort category)
		{
			_localNextSeqID++;
			return (_peerID << 29) | (uint)(category << 24) | _localNextSeqID;
		}

		public ushort ParseCategory(uint runtimeID)
		{
			return (ushort)((runtimeID & 0x1F000000) >> 24);
		}

		public uint ParseSequenceID(uint runtimeID)
		{
			return runtimeID & 0x7FFFFF;
		}

		public uint ParsePeerID(uint runtimeID)
		{
			return (runtimeID & 0xE0000000u) >> 29;
		}

		public bool IsSyncedRuntimeID(uint runtimeID)
		{
			return (runtimeID & 0x800000) != 0;
		}
	}
}
