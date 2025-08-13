using System;
using FlatBuffers;
using MoleMole.Config;
using MoleMole.MPProtocol;

namespace MoleMole
{
	public abstract class BaseAbilityEntityIdentiy : BaseMPIdentity
	{
		protected BaseMonoAbilityEntity _abilityEntity;

		protected BaseAbilityActor _abilityActor;

		protected MPActorAbilityPlugin _mpAbilityPlugin;

		public override IdentityRemoteMode remoteMode
		{
			get
			{
				return _abilityEntity.commonConfig.MPArguments.RemoteMode;
			}
		}

		public override void Init()
		{
			_mpAbilityPlugin = _abilityActor.GetPluginAs<ActorAbilityPlugin, MPActorAbilityPlugin>();
		}

		public sealed override void OnReliablePacket(MPRecvPacketContainer pc)
		{
			if (base.isAuthority)
			{
				OnAuthorityReliablePacket(pc);
			}
			else
			{
				OnRemoteReliablePacket(pc);
			}
		}

		public sealed override void OnStateUpdatePacket(MPRecvPacketContainer pc)
		{
			if (base.isAuthority)
			{
				OnAuthorityStateUpdate(pc);
			}
			else
			{
				OnRemoteStateUpdate(pc);
			}
		}

		protected virtual void OnAuthorityReliablePacket(MPRecvPacketContainer pc)
		{
			if (pc.packet is Packet_Ability_InvocationTable)
			{
				OnAbilityInvokeTable(pc);
			}
		}

		protected virtual void OnRemoteReliablePacket(MPRecvPacketContainer pc)
		{
			if (pc.packet is Packet_Ability_InvocationTable)
			{
				OnAbilityInvokeTable(pc);
			}
		}

		protected virtual void OnAuthorityStateUpdate(MPRecvPacketContainer pc)
		{
		}

		protected virtual void OnRemoteStateUpdate(MPRecvPacketContainer pc)
		{
		}

		public override void OnAuthorityStart()
		{
			base.OnAuthorityStart();
			BaseAbilityActor abilityActor = _abilityActor;
			abilityActor.onJustKilled = (Action<uint, string, KillEffect>)Delegate.Combine(abilityActor.onJustKilled, new Action<uint, string, KillEffect>(OnAuthorityJustKilled));
		}

		private void OnAuthorityJustKilled(uint killerID, string animEventID, KillEffect killEffect)
		{
			MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Entity_Kill>();
			StringOffset animEventIDOffset = MPMiscs.CreateString(pc.builder, animEventID);
			pc.Finish(Packet_Entity_Kill.CreatePacket_Entity_Kill(pc.builder, killerID, animEventIDOffset, (byte)killEffect));
			Singleton<MPManager>.Instance.SendReliableToOthers(runtimeID, pc);
		}

		private void OnAbilityInvokeTable(MPRecvPacketContainer pc)
		{
			_mpAbilityPlugin.HandleInvokes(pc.As<Packet_Ability_InvocationTable>(), pc.fromPeerID);
		}

		public void Command_TryApplyModifier(uint casterID, int instancedAbilityID, int modifierLocalID)
		{
			if (base.isAuthority)
			{
				_mpAbilityPlugin.MPTryApplyModifierByID(casterID, instancedAbilityID, modifierLocalID);
				return;
			}
			RecordInvokeEntryContext entry;
			_mpAbilityPlugin.StartRecordInvokeEntry(instancedAbilityID, 0, casterID, 255, out entry);
			Offset<MetaArg_Command_ModifierChangeRequest> offset = MetaArg_Command_ModifierChangeRequest.CreateMetaArg_Command_ModifierChangeRequest(entry.builder, ModifierAction.Added, (byte)modifierLocalID);
			entry.Finish(offset, AbilityInvokeArgument.MetaArg_Command_ModifierChangeRequest);
		}

		public void Command_TryRemoveModifier(uint casterID, int instancedAbilityID, int modifierLocalID)
		{
			if (base.isAuthority)
			{
				_mpAbilityPlugin.MPTryRemoveModifierByID(casterID, instancedAbilityID, modifierLocalID);
				return;
			}
			RecordInvokeEntryContext entry;
			_mpAbilityPlugin.StartRecordInvokeEntry(instancedAbilityID, 0, casterID, 255, out entry);
			Offset<MetaArg_Command_ModifierChangeRequest> offset = MetaArg_Command_ModifierChangeRequest.CreateMetaArg_Command_ModifierChangeRequest(entry.builder, ModifierAction.Removed, (byte)modifierLocalID);
			entry.Finish(offset, AbilityInvokeArgument.MetaArg_Command_ModifierChangeRequest);
		}
	}
}
