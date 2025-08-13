using System.Collections.Generic;
using FlatBuffers;
using MoleMole.Config;
using MoleMole.MPProtocol;

namespace MoleMole
{
	public class MPLevelActor : LevelActor
	{
		private const float STUB_REMOTE_DURATION = 10f;

		private LevelIdentity _levelIdentity;

		public void SetupIdentity(LevelIdentity levelIdentity)
		{
			_levelIdentity = levelIdentity;
		}

		protected override void HandleAvatarCreationForStageCreation(EvtStageCreated evt, out bool sendStageReady)
		{
			List<MonoSpawnPoint> list = new List<MonoSpawnPoint>();
			foreach (string avatarSpawnName in evt.avatarSpawnNameList)
			{
				int namedSpawnPointIx = Singleton<StageManager>.Instance.GetStageEnv().GetNamedSpawnPointIx(avatarSpawnName);
				list.Add(Singleton<StageManager>.Instance.GetStageEnv().spawnPoints[namedSpawnPointIx]);
			}
			if (evt.isBorn)
			{
				CreateMPAvatars();
			}
			Singleton<AvatarManager>.Instance.InitAvatarsPos(list);
			Singleton<MonsterManager>.Instance.InitMonstersPos(evt.offset);
			if (!Singleton<MPManager>.Instance.isMaster)
			{
				MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Level_PeerStageReady>();
				Packet_Level_PeerStageReady.StartPacket_Level_PeerStageReady(pc.builder);
				Packet_Level_PeerStageReady.AddState(pc.builder, PingPongEnum.Request);
				pc.Finish(Packet_Level_PeerStageReady.EndPacket_Level_PeerStageReady(pc.builder));
				Singleton<MPManager>.Instance.SendReliableToPeer(562036737u, 1, pc);
			}
			sendStageReady = false;
		}

		private void CreateMPAvatars()
		{
			List<AvatarDataItem> memberList = Singleton<LevelScoreManager>.Instance.memberList;
			for (int i = 0; i < memberList.Count; i++)
			{
				int num = i + 1;
				bool isLocal = num == Singleton<MPManager>.Instance.peerID;
				uint fixedAvatarRuntimeIDForPeer = Singleton<RuntimeIDManager>.Instance.GetFixedAvatarRuntimeIDForPeer(num);
				Singleton<AvatarManager>.Instance.CreateAvatar(memberList[i], isLocal, InLevelData.CREATE_INIT_POS, InLevelData.CREATE_INIT_FORWARD, fixedAvatarRuntimeIDForPeer, true, true);
				Singleton<MPManager>.Instance.RegisterIdentity(fixedAvatarRuntimeIDForPeer, num, new AvatarIdentity());
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(fixedAvatarRuntimeIDForPeer);
				actor.CreateAppliedAbility(AbilityData.GetAbilityConfig("Temp_UnlockSKL02Button"));
				actor.CreateAppliedAbility(AbilityData.GetAbilityConfig("Test_UnlockBranchAttack"));
				actor.PushProperty("Actor_MaxHPDelta", 3000f);
			}
		}

		protected override void InitAdditionalLevelActorPlugins()
		{
			witchTimeLevelBuff = new MPLevelBuffWitchTime(this);
			stopWorldLevelBuff = new LevelBuffStopWorld(this);
			levelBuffs = new BaseLevelBuff[2];
			levelBuffs[0] = witchTimeLevelBuff;
			levelBuffs[1] = stopWorldLevelBuff;
			AddPlugin(new MPLevelAbilityHelperPlugin(this));
		}

		private ApplyLevelBuff LocateApplyLevelBuffConfig(uint ownerID, int instancedAbilityID, int actionLocalID)
		{
			if (instancedAbilityID == 0)
			{
				return null;
			}
			BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(ownerID);
			if (actor == null)
			{
				return null;
			}
			ActorAbility instancedAbilityByID = actor.mpAbilityPlugin.GetInstancedAbilityByID(instancedAbilityID);
			if (instancedAbilityByID == null)
			{
				return null;
			}
			return (ApplyLevelBuff)instancedAbilityByID.config.InvokeSites[actionLocalID];
		}

		private void CreateLevelBuffEffect(LevelBuffType type, uint ownerID, int instancedAbilityID, int actionLocalID)
		{
			ApplyLevelBuff applyLevelBuff = LocateApplyLevelBuffConfig(ownerID, instancedAbilityID, actionLocalID);
			if (applyLevelBuff != null && !string.IsNullOrEmpty(applyLevelBuff.AttachLevelEffectPattern))
			{
				Singleton<EffectManager>.Instance.CreateUniqueIndexedEffectPattern(applyLevelBuff.AttachLevelEffectPattern, type.ToString(), Singleton<LevelManager>.Instance.levelEntity);
			}
		}

		private void DestroyLevelBuffEffect(LevelBuffType type)
		{
			Singleton<EffectManager>.Instance.TrySetDestroyUniqueIndexedEffectPattern(type.ToString());
		}

		public void MPRequestStartLevelBuff(LevelBuffType type, LevelBuffSide side, uint ownerID, bool allowRefresh, bool enteringSlow, bool notStartEffect, float duration, int instancedAbilityID, int actionLocalID)
		{
			BaseLevelBuff baseLevelBuff;
			switch (type)
			{
			case LevelBuffType.WitchTime:
				baseLevelBuff = witchTimeLevelBuff;
				break;
			case LevelBuffType.StopWorld:
				baseLevelBuff = stopWorldLevelBuff;
				break;
			default:
				baseLevelBuff = null;
				break;
			}
			bool useMaxDuration = allowRefresh;
			if (baseLevelBuff.isActive)
			{
				switch (type)
				{
				case LevelBuffType.WitchTime:
				{
					bool flag = witchTimeLevelBuff.Refresh(duration, side, ownerID, enteringSlow, useMaxDuration, notStartEffect);
					if (flag)
					{
						Singleton<EventManager>.Instance.FireEvent(new EvtLevelBuffState(type, LevelBuffState.Switch, witchTimeLevelBuff.levelBuffSide, ownerID));
						DestroyLevelBuffEffect(type);
						CreateLevelBuffEffect(type, ownerID, instancedAbilityID, actionLocalID);
					}
					SendLevelBuffResponse((!flag) ? LevelBuffAction.SameSideExtend : LevelBuffAction.SwitchSide, type, enteringSlow, notStartEffect, ownerID, witchTimeLevelBuff.levelBuffSide, instancedAbilityID, actionLocalID);
					break;
				}
				}
				return;
			}
			switch (type)
			{
			case LevelBuffType.WitchTime:
				witchTimeLevelBuff.ownerID = ownerID;
				witchTimeLevelBuff.Setup(enteringSlow, duration, side, notStartEffect);
				break;
			case LevelBuffType.StopWorld:
				stopWorldLevelBuff.ownerID = ownerID;
				stopWorldLevelBuff.Setup(enteringSlow, duration, ownerID);
				break;
			}
			AddPlugin(baseLevelBuff);
			baseLevelBuff.isActive = true;
			CreateLevelBuffEffect(type, ownerID, instancedAbilityID, actionLocalID);
			SendLevelBuffResponse(LevelBuffAction.Add, type, enteringSlow, notStartEffect, ownerID, baseLevelBuff.levelBuffSide, instancedAbilityID, actionLocalID);
			Singleton<EventManager>.Instance.FireEvent(new EvtLevelBuffState(type, LevelBuffState.Start, side, ownerID));
		}

		public override void StopLevelBuff(BaseLevelBuff buff)
		{
			if (_levelIdentity.isAuthority)
			{
				SendLevelBuffResponse(LevelBuffAction.Remove, buff.levelBuffType, false, false, 0u, LevelBuffSide.FromAvatar);
			}
			DestroyLevelBuffEffect(buff.levelBuffType);
			base.StopLevelBuff(buff);
		}

		private void SendLevelBuffResponse(LevelBuffAction action, LevelBuffType type, bool enteringSlow, bool notStartEffect, uint ownerID, LevelBuffSide side, int instancedAbilityID = 0, int actionLocalID = 0)
		{
			MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Level_ResultLevelBuff>();
			bool enteringSlow2 = enteringSlow;
			bool notStartEffect2 = notStartEffect;
			Offset<Packet_Level_ResultLevelBuff> offset = Packet_Level_ResultLevelBuff.CreatePacket_Level_ResultLevelBuff(pc.builder, action, 0, (byte)side, enteringSlow2, notStartEffect2, ownerID, (byte)instancedAbilityID, (byte)actionLocalID);
			pc.Finish(offset);
			Singleton<MPManager>.Instance.SendReliableToOthers(562036737u, pc);
		}

		public void MPResponseHandleLevelBuff(LevelBuffAction action, LevelBuffType type, bool enteringSlow, bool notStartEffect, uint ownerID, LevelBuffSide side, int instancedAbilityID, int actionLocalID)
		{
			BaseLevelBuff baseLevelBuff;
			switch (type)
			{
			case LevelBuffType.WitchTime:
				baseLevelBuff = witchTimeLevelBuff;
				break;
			case LevelBuffType.StopWorld:
				baseLevelBuff = stopWorldLevelBuff;
				break;
			default:
				baseLevelBuff = null;
				break;
			}
			switch (action)
			{
			case LevelBuffAction.Add:
				switch (type)
				{
				case LevelBuffType.WitchTime:
					witchTimeLevelBuff.Setup(enteringSlow, 10f, side, notStartEffect);
					break;
				case LevelBuffType.StopWorld:
					stopWorldLevelBuff.Setup(enteringSlow, 10f, ownerID);
					break;
				}
				baseLevelBuff.ownerID = ownerID;
				AddPlugin(baseLevelBuff);
				baseLevelBuff.isActive = true;
				CreateLevelBuffEffect(type, ownerID, instancedAbilityID, actionLocalID);
				Singleton<EventManager>.Instance.FireEvent(new EvtLevelBuffState(type, LevelBuffState.Start, witchTimeLevelBuff.levelBuffSide, ownerID));
				break;
			case LevelBuffAction.Remove:
				StopLevelBuff(baseLevelBuff);
				break;
			case LevelBuffAction.SameSideExtend:
				baseLevelBuff.ownerID = ownerID;
				witchTimeLevelBuff.ExtendDuration(10f, enteringSlow, true);
				break;
			case LevelBuffAction.SwitchSide:
				witchTimeLevelBuff.SwitchSide(enteringSlow, 10f, side, ownerID, notStartEffect);
				DestroyLevelBuffEffect(type);
				CreateLevelBuffEffect(type, ownerID, instancedAbilityID, actionLocalID);
				Singleton<EventManager>.Instance.FireEvent(new EvtLevelBuffState(type, LevelBuffState.Switch, witchTimeLevelBuff.levelBuffSide, ownerID));
				break;
			}
		}
	}
}
