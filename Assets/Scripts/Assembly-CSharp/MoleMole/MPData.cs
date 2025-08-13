using System;
using System.Collections.Generic;
using MoleMole.Config;
using MoleMole.MPProtocol;

namespace MoleMole
{
	public static class MPData
	{
		public static ConfigMPArguments AVATAR_DEFAULT_MP_SETTINGS = new ConfigMPArguments
		{
			SyncSendInterval = 1f / 15f,
			RemoteMode = IdentityRemoteMode.Mute,
			MuteSyncAnimatorTags = new string[3] { "AVATAR_HITSUB", "AVATAR_DIESUB", "AVATAR_THROW" }
		};

		public static ConfigMPArguments MONSTER_DEFAULT_MP_SETTINGS = new ConfigMPArguments
		{
			SyncSendInterval = 0.1f,
			RemoteMode = IdentityRemoteMode.SendAndReceive,
			MuteSyncAnimatorTags = new string[3] { "MONSTER_HITSUB", "MONSTER_DIESUB", "MONSTER_THROWSUB" }
		};

		public static HashSet<Type> ReplicatedEventTypes = new HashSet<Type>
		{
			typeof(EvtHittingOther),
			typeof(EvtBeingHit),
			typeof(EvtAttackLanded),
			typeof(EvtEvadeSuccess)
		};

		public static HashSet<Type> ReplicatedEventWireTypes = new HashSet<Type>
		{
			typeof(Packet_Event_EvtHittingOther),
			typeof(Packet_Event_EvtBeingHit),
			typeof(Packet_Event_EvtAttackLanded),
			typeof(Packet_Event_EvtEvadeSuccess)
		};
	}
}
