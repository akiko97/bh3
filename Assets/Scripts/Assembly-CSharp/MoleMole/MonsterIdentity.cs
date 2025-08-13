using MoleMole.Config;
using MoleMole.MPProtocol;

namespace MoleMole
{
	public class MonsterIdentity : BaseAnimatorEntityIdentity
	{
		protected BaseMonoMonster _monster;

		protected MonsterActor _monsterActor;

		public override void PreInitReplicateRemote(MPRecvPacketContainer pc)
		{
			Packet_Monster_MonsterCreation packet_Monster_MonsterCreation = pc.As<Packet_Monster_MonsterCreation>();
			Singleton<MonsterManager>.Instance.CreateMonster(packet_Monster_MonsterCreation.MonsterName, packet_Monster_MonsterCreation.MonsterType, packet_Monster_MonsterCreation.Level, true, MPMiscs.Convert(packet_Monster_MonsterCreation.InitPos), pc.runtimeID, packet_Monster_MonsterCreation.IsElite, packet_Monster_MonsterCreation.UniqueMonsterID);
		}

		public override void Init()
		{
			_monster = Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(runtimeID);
			_monsterActor = Singleton<MPEventManager>.Instance.GetActor<MonsterActor>(runtimeID);
			_animatorEntity = _monster;
			_abilityEntity = _monster;
			_abilityActor = _monsterActor;
			_monsterActor.GetPlugin<MPMonsterActorPlugin>().SetupIdentity(this);
			_monsterActor.GetPluginAs<ActorAbilityPlugin, MPActorAbilityPlugin>().SetupIdentity(this);
			base.Init();
		}

		protected override void AuthoritySendTransformSyncCore()
		{
			if (_monster.IsActive())
			{
				base.AuthoritySendTransformSyncCore();
			}
		}

		public override void OnRemoteStart()
		{
			base.OnRemoteStart();
			_monster.destroyMode = BaseMonoMonster.DestroyMode.DeactivateOnly;
		}

		public override void OnRemoval()
		{
			base.OnRemoval();
			_monster.destroyMode = BaseMonoMonster.DestroyMode.SetToBeRemoved;
			_monster.SetDestroy();
		}

		protected override void OnRemoteReliablePacket(MPRecvPacketContainer pc)
		{
			base.OnRemoteReliablePacket(pc);
			if (pc.packet is Packet_Entity_Kill)
			{
				OnRemoteKill(pc.As<Packet_Entity_Kill>());
			}
		}

		private void OnRemoteKill(Packet_Entity_Kill packet)
		{
			_monsterActor.Kill(packet.KillerID, packet.AnimEventID, (KillEffect)packet.KillEffect);
		}

		protected override void OnRemoteEntityAnimatorStateChanged(Packet_Entity_AnimatorStateChange packet)
		{
			if (_monster.IsActive())
			{
				base.OnRemoteEntityAnimatorStateChanged(packet);
			}
		}

		protected override void OnRemoteTransformSync(Packet_Entity_TransformSync packet)
		{
			if (_monster.IsActive() && !_monster.IsRetreating() && !_monster.IsFrameHalting())
			{
				base.OnRemoteTransformSync(packet);
			}
		}

		protected override void OnRemoteEntityAnimatorParameterChanged(Packet_Entity_AnimatorParameterChange packet)
		{
			if (_monster.IsActive())
			{
				base.OnRemoteEntityAnimatorParameterChanged(packet);
			}
		}
	}
}
