using MoleMole.Config;
using MoleMole.MPProtocol;

namespace MoleMole
{
	public class AvatarIdentity : BaseAnimatorEntityIdentity
	{
		protected BaseMonoAvatar _avatar;

		protected AvatarActor _avatarActor;

		private IdentityRemoteMode _remoteMode;

		public override IdentityRemoteMode remoteMode
		{
			get
			{
				return _remoteMode;
			}
		}

		public override void Init()
		{
			_avatar = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID);
			_avatarActor = Singleton<MPEventManager>.Instance.GetActor<AvatarActor>(runtimeID);
			_animatorEntity = _avatar;
			_abilityEntity = _avatar;
			_abilityActor = _avatarActor;
			_avatarActor.GetPlugin<MPAvatarActorPlugin>().SetupIdentity(this);
			_avatarActor.GetPluginAs<ActorAbilityPlugin, MPActorAbilityPlugin>().SetupIdentity(this);
			switch (Singleton<MPLevelManager>.Instance.mpMode)
			{
			case MPMode.Normal:
				_remoteMode = IdentityRemoteMode.Mute;
				break;
			case MPMode.PvP_ReceiveNoSend:
				_remoteMode = IdentityRemoteMode.ReceiveAndNoSend;
				_avatar.SetAttackSelectMethod(AvatarAttackTargetSelectPattern.PvPSelectRemoteAvatar);
				break;
			case MPMode.PvP_SendNoReceive:
				_remoteMode = IdentityRemoteMode.SendAndNoReceive;
				_avatar.SetAttackSelectMethod(AvatarAttackTargetSelectPattern.PvPSelectRemoteAvatar);
				break;
			}
			base.Init();
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
			_avatarActor.Kill(packet.KillerID, packet.AnimEventID, (KillEffect)packet.KillEffect);
		}
	}
}
