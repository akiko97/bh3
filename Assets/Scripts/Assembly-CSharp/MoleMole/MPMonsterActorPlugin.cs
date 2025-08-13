using System;
using FlatBuffers;
using MoleMole.MPProtocol;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class MPMonsterActorPlugin : BaseMPAbilityActorPlugin
	{
		protected MonsterActor _actor;

		protected MonsterIdentity _identity;

		public MPMonsterActorPlugin(BaseActor actor)
		{
			_actor = (MonsterActor)actor;
		}

		public void SetupIdentity(MonsterIdentity identity)
		{
			_identity = identity;
			Setup(_actor, identity);
		}

		public override void OnAdded()
		{
			_actor.appliedAbilities.Add(Tuple.Create(AbilityData.GetAbilityConfig("MPTest_HitToBleedAlt"), AbilityData.EMPTY_OVERRIDE_MAP));
			if (Singleton<MPManager>.Instance.isMaster)
			{
				MonsterActor actor = _actor;
				actor.onPostInitialized = (Action)Delegate.Combine(actor.onPostInitialized, new Action(OnInitializeDoneReplicate));
			}
		}

		private void OnInitializeDoneReplicate()
		{
			MPSendPacketContainer initPc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Monster_MonsterCreation>();
			StringOffset monsterNameOffset = initPc.builder.CreateString(_actor.monster.MonsterName);
			StringOffset monsterTypeOffset = initPc.builder.CreateString(_actor.monster.TypeName);
			Packet_Monster_MonsterCreation.StartPacket_Monster_MonsterCreation(initPc.builder);
			Packet_Monster_MonsterCreation.AddMonsterName(initPc.builder, monsterNameOffset);
			Packet_Monster_MonsterCreation.AddMonsterType(initPc.builder, monsterTypeOffset);
			Packet_Monster_MonsterCreation.AddLevel(initPc.builder, _actor.level);
			Packet_Monster_MonsterCreation.AddIsElite(initPc.builder, _actor.isElite);
			Packet_Monster_MonsterCreation.AddUniqueMonsterID(initPc.builder, _actor.uniqueMonsterID);
			Vector3 xZPosition = _actor.monster.XZPosition;
			Packet_Monster_MonsterCreation.AddInitPos(initPc.builder, MPVector2_XZ.CreateMPVector2_XZ(initPc.builder, xZPosition.x, xZPosition.z));
			initPc.Finish(Packet_Monster_MonsterCreation.EndPacket_Monster_MonsterCreation(initPc.builder));
			Singleton<MPManager>.Instance.InstantiateMPIdentity<MonsterIdentity>(_actor.runtimeID, initPc);
		}

		protected override bool OnRemoteReplicatedEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnRemoteBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		private bool OnRemoteBeingHit(EvtBeingHit evt)
		{
			if (evt.attackData.rejected)
			{
				return false;
			}
			if (evt.attackData.hitCollision == null)
			{
				_actor.AmendHitCollision(evt.attackData);
			}
			evt.attackData.resolveStep = AttackData.AttackDataStep.FinalResolved;
			float num = (float)_actor.HP - evt.resolvedDamage;
			if (num <= 0f)
			{
				num = 0f;
			}
			DelegateUtils.UpdateField(ref _actor.HP, num, num - (float)_actor.HP, _actor.onHPChanged);
			_actor.FireAttackDataEffects(evt.attackData);
			_actor.AbilityBeingHit(evt);
			_actor.BeingHit(evt.attackData, evt.beHitEffect);
			return true;
		}
	}
}
