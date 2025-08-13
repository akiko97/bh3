using System.Collections.Generic;
using MoleMole.Config;
using MoleMole.MPProtocol;
using proto;

namespace MoleMole
{
	public class LevelIdentity : BaseAbilityEntityIdentiy
	{
		private MPLevelActor _mpLevelActor;

		private int _readyPeerCount = 1;

		public override IdentityRemoteMode remoteMode
		{
			get
			{
				return IdentityRemoteMode.Mute;
			}
		}

		public override void Init()
		{
			Singleton<MPLevelManager>.Instance.levelIdentity = this;
			_mpLevelActor = (MPLevelActor)Singleton<LevelManager>.Instance.levelActor;
			_mpLevelActor.SetupIdentity(this);
			_abilityEntity = _mpLevelActor.levelEntity;
			_abilityActor = _mpLevelActor;
			base.Init();
		}

		public override void OnAuthorityStart()
		{
		}

		public override void OnRemoteStart()
		{
			_mpLevelActor.witchTimeLevelBuff.muteUpdateDuration = true;
			_mpLevelActor.stopWorldLevelBuff.muteUpdateDuration = true;
		}

		protected override void OnAuthorityReliablePacket(MPRecvPacketContainer pc)
		{
			base.OnAuthorityReliablePacket(pc);
			if (pc.packet is Packet_Level_PeerStageReady)
			{
				OnAuthority_PeerStageReady(pc);
			}
			else if (pc.packet is Packet_Level_RequestLevelBuff)
			{
				OnAuthorityLevelBuff_Request(pc);
			}
		}

		protected override void OnRemoteReliablePacket(MPRecvPacketContainer pc)
		{
			base.OnRemoteReliablePacket(pc);
			if (pc.packet is Packet_Level_CreateStageFullData)
			{
				On_Level_CreateStageFullData(pc.As<Packet_Level_CreateStageFullData>());
			}
			else if (pc.packet is Packet_Level_PeerStageReady)
			{
				OnRemote_PeerStageReady(pc);
			}
			else if (pc.packet is Packet_Level_ResultLevelBuff)
			{
				OnRemote_Result_LevelBuff(pc);
			}
		}

		public void DebugCreateStageWithFullDataSync(MPSendPacketContainer pc)
		{
			if (isOwner)
			{
				DebugCreateStageWithFullDataSync_Impl(pc.ReadAs<Packet_Level_CreateStageFullData>());
				Singleton<MPManager>.Instance.SendReliableToOthers(runtimeID, pc);
			}
		}

		private void On_Level_CreateStageFullData(Packet_Level_CreateStageFullData packet)
		{
			DebugCreateStageWithFullDataSync_Impl(packet);
		}

		private void DebugCreateStageWithFullDataSync_Impl(Packet_Level_CreateStageFullData fullData)
		{
			Singleton<LevelScoreManager>.Instance.SetDevLevelBeginIntent("Lua/Levels/Common/Level 0.lua", LevelActor.Mode.NetworkedMP, 10, 10, null);
			Singleton<LevelScoreManager>.Instance.memberList = new List<AvatarDataItem>();
			for (int i = 0; i < fullData.AvatarsLength; i++)
			{
				MoleMole.MPProtocol.MPAvatarDataItem avatars = fullData.GetAvatars(i);
				AvatarDataItem avatarDataItem = new AvatarDataItem(avatars.AvatarID, avatars.Level, avatars.Star);
				ConfigAvatar avatarConfig = AvatarData.GetAvatarConfig(avatarDataItem.AvatarRegistryKey);
				WeaponDataItem dummyFirstWeaponDataByRole = Singleton<StorageModule>.Instance.GetDummyFirstWeaponDataByRole(avatarConfig.CommonArguments.RoleName, 1);
				avatarDataItem.equipsMap[(EquipmentSlot)1] = dummyFirstWeaponDataByRole;
				Singleton<LevelScoreManager>.Instance.memberList.Add(avatarDataItem);
			}
			Singleton<LevelManager>.Instance.levelActor.SuddenLevelStart();
			Singleton<LevelManager>.Instance.levelActor.levelMode = LevelActor.Mode.NetworkedMP;
			Singleton<MPLevelManager>.Instance.mpMode = fullData.MpMode;
			if (fullData.MpMode == MPMode.Normal)
			{
				Singleton<LevelManager>.Instance.gameMode = new NetworkedMP_Default_GameMode();
			}
			else if (fullData.MpMode == MPMode.PvP_ReceiveNoSend)
			{
				Singleton<LevelManager>.Instance.gameMode = new NetworkedMP_PvPTest_GameMode();
			}
			else if (fullData.MpMode == MPMode.PvP_SendNoReceive)
			{
				Singleton<LevelManager>.Instance.gameMode = new NetworkedMP_PvPTest_GameMode();
			}
			Singleton<StageManager>.Instance.CreateStage(fullData.StageData.StageName, new List<string> { "Born" }, null);
		}

		private void OnAuthority_PeerStageReady(MPRecvPacketContainer recvPc)
		{
			Packet_Level_PeerStageReady packet_Level_PeerStageReady = recvPc.As<Packet_Level_PeerStageReady>();
			_readyPeerCount++;
			if (_readyPeerCount == Singleton<MPManager>.Instance.peer.totalPeerCount)
			{
				MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Level_PeerStageReady>();
				Packet_Level_PeerStageReady.StartPacket_Level_PeerStageReady(pc.builder);
				Packet_Level_PeerStageReady.AddState(pc.builder, PingPongEnum.Response);
				pc.Finish(Packet_Level_PeerStageReady.EndPacket_Level_PeerStageReady(pc.builder));
				Singleton<MPManager>.Instance.SendReliableToOthers(562036737u, pc);
				Singleton<EventManager>.Instance.FireEvent(new EvtStageReady
				{
					isBorn = true
				});
			}
		}

		private void OnRemote_PeerStageReady(MPRecvPacketContainer recvPc)
		{
			Packet_Level_PeerStageReady packet_Level_PeerStageReady = recvPc.As<Packet_Level_PeerStageReady>();
			Singleton<EventManager>.Instance.FireEvent(new EvtStageReady
			{
				isBorn = true
			});
		}

		private void OnAuthorityLevelBuff_Request(MPRecvPacketContainer pc)
		{
			Packet_Level_RequestLevelBuff packet_Level_RequestLevelBuff = pc.As<Packet_Level_RequestLevelBuff>();
			_mpLevelActor.MPRequestStartLevelBuff((LevelBuffType)packet_Level_RequestLevelBuff.LevelBuffType, (LevelBuffSide)packet_Level_RequestLevelBuff.Side, packet_Level_RequestLevelBuff.OwnerRuntimeID, packet_Level_RequestLevelBuff.AllowRefresh, packet_Level_RequestLevelBuff.EnteringSlow, packet_Level_RequestLevelBuff.NotStartEffect, packet_Level_RequestLevelBuff.Duration, packet_Level_RequestLevelBuff.InstancedAbilityID, packet_Level_RequestLevelBuff.ActionLocalID);
		}

		private void OnRemote_Result_LevelBuff(MPRecvPacketContainer pc)
		{
			Packet_Level_ResultLevelBuff packet_Level_ResultLevelBuff = pc.As<Packet_Level_ResultLevelBuff>();
			_mpLevelActor.MPResponseHandleLevelBuff(packet_Level_ResultLevelBuff.Action, (LevelBuffType)packet_Level_ResultLevelBuff.LevelBuffType, packet_Level_ResultLevelBuff.EnteringSlow, packet_Level_ResultLevelBuff.NotStartEffect, packet_Level_ResultLevelBuff.OwnerRuntimeID, (LevelBuffSide)packet_Level_ResultLevelBuff.Side, packet_Level_ResultLevelBuff.InstancedAbilityID, packet_Level_ResultLevelBuff.ActionLocalID);
		}
	}
}
