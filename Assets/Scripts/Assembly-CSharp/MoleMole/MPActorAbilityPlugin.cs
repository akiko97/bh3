using System;
using System.Collections.Generic;
using FlatBuffers;
using MoleMole.Config;
using MoleMole.MPProtocol;

namespace MoleMole
{
	public class MPActorAbilityPlugin : ActorAbilityPlugin
	{
		public delegate void MPRemoteActionHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry);

		public delegate void MPAuthorityActionHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context);

		public const int INVOCATION_META_LOCALID = 255;

		private FlatBufferBuilder _invokeTableBuilder = new FlatBufferBuilder(128);

		private List<Offset<AbilityInvokeEntry>> _invokeTableOffsets = new List<Offset<AbilityInvokeEntry>>();

		private static MetaArg_ModifierChange _metaModifierChange = new MetaArg_ModifierChange();

		private static MetaArg_AbilityControl _metaAbilityControl = new MetaArg_AbilityControl();

		private static MetaArg_Command_ModifierChangeRequest _metaModifierReq = new MetaArg_Command_ModifierChangeRequest();

		protected BaseAbilityEntityIdentiy _abilityIdentity;

		public MPActorAbilityPlugin(BaseAbilityActor abilityActor)
			: base(abilityActor)
		{
		}

		public void FireEffect_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
		{
			FireEffectHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
			context.Finish(true);
		}

		public void FireEffect_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
		{
			FireEffectHandler(actionConfig, instancedAbility, instancedModifier, target, null);
		}

		public void ApplyModifier_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
		{
			ApplyModifier applyModifier = (ApplyModifier)actionConfig;
			if (target == _owner)
			{
				_owner.abilityPlugin.ApplyModifier(instancedAbility, applyModifier.ModifierName);
			}
			else if (Singleton<MPEventManager>.Instance.IsIdentityAuthority(target.runtimeID))
			{
				target.abilityPlugin.ApplyModifier(instancedAbility, applyModifier.ModifierName);
			}
			else
			{
				context.Finish(true);
			}
		}

		public void ApplyModifier_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
		{
			ApplyModifier applyModifier = (ApplyModifier)actionConfig;
			if (target != _owner && Singleton<MPEventManager>.Instance.IsIdentityAuthority(target.runtimeID))
			{
				target.abilityPlugin.ApplyModifier(instancedAbility, applyModifier.ModifierName);
			}
		}

		public void AttachEffect_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
		{
			AttachEffectHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
			context.Finish(true);
		}

		public void AttachEffect_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
		{
			AttachEffectHandler(actionConfig, instancedAbility, instancedModifier, target, null);
		}

		public void DamageByAttackValue_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
		{
			DamageByAttackValueHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}

		public void Predicated_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
		{
			PredicatedHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
		}

		private void HealSP_Common(HealSP config, ActorAbility instancedAbility, BaseAbilityActor target)
		{
			float num = instancedAbility.Evaluate(config.Amount);
			if (Singleton<MPEventManager>.Instance.IsIdentityAuthority(target.runtimeID))
			{
				target.HealSP(num);
			}
			if ((bool)target.isAlive && !config.MuteHealEffect && num > 0f)
			{
				target.entity.FireEffect("Ability_HealSP");
			}
		}

		public void HealSP_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
		{
			HealSP_Common((HealSP)actionConfig, instancedAbility, target);
			context.Finish(true);
		}

		public void HealSP_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
		{
			HealSP_Common((HealSP)actionConfig, instancedAbility, target);
		}

		public void AvatarSkillStart_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
		{
			AvatarSkillStartHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
			context.Finish(true);
		}

		public void AvatarSKillStart_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
		{
			AvatarSkillStartHandler(actionConfig, instancedAbility, instancedModifier, target, null);
		}

		public void ActTimeSlow_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
		{
			TimeSlowHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
			context.Finish(true);
		}

		public void ActTimeSlow_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
		{
			TimeSlowHandler(actionConfig, instancedAbility, instancedModifier, target, null);
		}

		public void SetAnimatorBool_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
		{
			SetAnimatorBoolHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
			context.Finish(true);
		}

		public void SetAnimatorBool_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
		{
			SetAnimatorBoolHandler(actionConfig, instancedAbility, instancedModifier, target, null);
		}

		public void TriggerAbility_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
		{
			TriggerAbilityHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
			context.Finish(true);
		}

		public void TriggerAbility_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
		{
			TriggerAbilityHandler(actionConfig, instancedAbility, instancedModifier, target, null);
		}

		public void ApplyLevelBuff_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
		{
			ApplyLevelBuff applyLevelBuff = (ApplyLevelBuff)actionConfig;
			float duration = _Internal_CalculateApplyLevelBuffDuration(applyLevelBuff, instancedAbility, evt);
			uint runtimeID = instancedAbility.caster.runtimeID;
			LevelBuffSide levelBuffSide = ((!applyLevelBuff.UseOverrideCurSide) ? CalculateLevelBuffSide(instancedAbility.caster.runtimeID) : applyLevelBuff.OverrideCurSide);
			MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Level_RequestLevelBuff>();
			Packet_Level_RequestLevelBuff.StartPacket_Level_RequestLevelBuff(pc.builder);
			Packet_Level_RequestLevelBuff.AddLevelBuffType(pc.builder, (byte)applyLevelBuff.LevelBuff);
			Packet_Level_RequestLevelBuff.AddEnteringSlow(pc.builder, applyLevelBuff.EnteringTimeSlow);
			Packet_Level_RequestLevelBuff.AddAllowRefresh(pc.builder, applyLevelBuff.LevelBuffAllowRefresh);
			Packet_Level_RequestLevelBuff.AddSide(pc.builder, (byte)levelBuffSide);
			Packet_Level_RequestLevelBuff.AddOwnerRuntimeID(pc.builder, runtimeID);
			Packet_Level_RequestLevelBuff.AddNotStartEffect(pc.builder, applyLevelBuff.NotStartEffect);
			Packet_Level_RequestLevelBuff.AddDuration(pc.builder, duration);
			Packet_Level_RequestLevelBuff.AddInstancedAbilityID(pc.builder, (byte)instancedAbility.instancedAbilityID);
			Packet_Level_RequestLevelBuff.AddActionLocalID(pc.builder, (byte)applyLevelBuff.localID);
			pc.Finish(Packet_Level_RequestLevelBuff.EndPacket_Level_RequestLevelBuff(pc.builder));
			Singleton<MPManager>.Instance.SendReliableToPeer(562036737u, 1, pc);
			Singleton<MPLevelManager>.Instance.levelActor.GetPlugin<MPLevelAbilityHelperPlugin>().AttachPendingModifiersToNextLevelBuff(applyLevelBuff, _owner.runtimeID, instancedAbility.instancedAbilityID, (target != null) ? target.runtimeID : 0u);
		}

		public void ApplyLevelBuff_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
		{
		}

		public void RemoveModifier_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
		{
			RemoveModifier removeModifier = (RemoveModifier)actionConfig;
			if (target == _owner)
			{
				_owner.abilityPlugin.TryRemoveModifier(instancedAbility, removeModifier.ModifierName);
			}
			else if (Singleton<MPEventManager>.Instance.IsIdentityAuthority(target.runtimeID))
			{
				target.abilityPlugin.TryRemoveModifier(instancedAbility, removeModifier.ModifierName);
			}
			else
			{
				context.Finish(true);
			}
		}

		public void RemoveModifier_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
		{
			RemoveModifier removeModifier = (RemoveModifier)actionConfig;
			if (target != _owner && Singleton<MPEventManager>.Instance.IsIdentityAuthority(target.runtimeID))
			{
				target.abilityPlugin.TryRemoveModifier(instancedAbility, removeModifier.ModifierName);
			}
		}

		public static void STUB_RemoteMute(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, Table argument)
		{
		}

		public static void STUB_AuthorityMute(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
		{
			context.Finish(false);
		}

		private void FlushRecordInvokeEntriesAndSend()
		{
			if (_invokeTableOffsets.Count > 0)
			{
				MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket(typeof(Packet_Ability_InvocationTable), _invokeTableBuilder);
				VectorOffset invokesOffset = Packet_Ability_InvocationTable.CreateInvokesVector(pc.builder, _invokeTableOffsets.ToArray());
				Offset<Packet_Ability_InvocationTable> offset = Packet_Ability_InvocationTable.CreatePacket_Ability_InvocationTable(pc.builder, invokesOffset);
				pc.Finish(offset);
				if (_abilityIdentity.isAuthority)
				{
					Singleton<MPManager>.Instance.SendReliableToOthers(_owner.runtimeID, pc);
				}
				else
				{
					Singleton<MPManager>.Instance.SendReliableToPeer(_owner.runtimeID, _abilityIdentity.GetPeerID(), pc);
				}
				_invokeTableBuilder.Clear();
				_invokeTableOffsets.Clear();
			}
		}

		protected override ActorModifier ApplyModifier(ActorAbility instancedAbility, ConfigAbilityModifier modifierConfig)
		{
			return base.ApplyModifier(instancedAbility, modifierConfig);
		}

		protected override void RemoveModifier(ActorModifier modifier, int index)
		{
			base.RemoveModifier(modifier, index);
		}

		public void StartRecordInvokeEntry(int instancedAbilityID, int instancedModifierID, uint targetRuntimeID, int localID, out RecordInvokeEntryContext entry)
		{
			entry = default(RecordInvokeEntryContext);
			entry.instancedAbilityID = (byte)instancedAbilityID;
			entry.instanceModifierID = (byte)instancedModifierID;
			entry.targetID = ((targetRuntimeID != _owner.runtimeID) ? targetRuntimeID : 0u);
			entry.localID = (byte)localID;
			entry.builder = _invokeTableBuilder;
			entry.outOffsetLs = _invokeTableOffsets;
		}

		protected override void HandleAction(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
		{
			if (_abilityIdentity.isAuthority && EvaluateAbilityPredicate(actionConfig.Predicates, instancedAbility, instancedModifier, target, evt))
			{
				RecordInvokeEntryContext entry;
				StartRecordInvokeEntry(instancedAbility.instancedAbilityID, (instancedModifier != null) ? instancedModifier.instancedModifierID : 0, (target != null) ? target.runtimeID : _owner.runtimeID, actionConfig.localID, out entry);
				actionConfig.MPGetAuthorityHandler(this)(actionConfig, instancedAbility, instancedModifier, target, evt, ref entry);
			}
		}

		public void HandleInvokes(Packet_Ability_InvocationTable table, int fromPeerID)
		{
			int invokesLength = table.InvokesLength;
			for (int i = 0; i < invokesLength; i++)
			{
				AbilityInvokeEntry invokes = table.GetInvokes(i);
				if (invokes.LocalID == byte.MaxValue)
				{
					if (_abilityIdentity.isAuthority)
					{
						MetaAuthorityInvokeHandler(invokes);
					}
					else
					{
						MetaRemoteInvokeHandler(invokes);
					}
					continue;
				}
				uint target = invokes.Target;
				BaseAbilityActor baseAbilityActor = ((target != 0) ? Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(target) : _owner);
				if (baseAbilityActor == null)
				{
					break;
				}
				int instancedModifierID = invokes.InstancedModifierID;
				ActorModifier actorModifier;
				ActorAbility actorAbility;
				if (instancedModifierID != 0)
				{
					actorModifier = GetInstancedModifierByID(instancedModifierID);
					actorAbility = actorModifier.parentAbility;
				}
				else
				{
					actorModifier = null;
					actorAbility = GetInstancedAbilityByID(invokes.InstancedAbilityID);
				}
				BaseActionContainer baseActionContainer = actorAbility.config.InvokeSites[invokes.LocalID];
				if (baseActionContainer is ConfigAbilityAction)
				{
					ConfigAbilityAction configAbilityAction = (ConfigAbilityAction)baseActionContainer;
					configAbilityAction.MPGetRemoteHandler(this)(configAbilityAction, actorAbility, actorModifier, baseAbilityActor, invokes);
					continue;
				}
				BaseAbilityMixin baseAbilityMixin = null;
				if (actorModifier != null)
				{
					baseAbilityMixin = actorModifier.GetInstancedMixin(invokes.LocalID);
				}
				if (baseAbilityMixin == null)
				{
					baseAbilityMixin = actorAbility.GetInstancedMixin(invokes.LocalID);
				}
				baseAbilityMixin.HandleMixinInvokeEntry(invokes, fromPeerID);
			}
		}

		protected override ActorModifier AddModifierOnIndex(ActorAbility instancedAbility, ConfigAbilityModifier modifierConfig, int index)
		{
			if (_abilityIdentity.isAuthority)
			{
				RecordInvokeEntryContext entry;
				StartRecordInvokeEntry(instancedAbility.instancedAbilityID, index + 1, instancedAbility.caster.runtimeID, 255, out entry);
				Offset<MetaArg_ModifierChange> offset = MetaArg_ModifierChange.CreateMetaArg_ModifierChange(entry.builder, ModifierAction.Added, (byte)modifierConfig.localID);
				entry.Finish(offset, AbilityInvokeArgument.MetaArg_ModifierChange);
				return base.AddModifierOnIndex(instancedAbility, modifierConfig, index);
			}
			return null;
		}

		protected override void RemoveModifierOnIndex(ActorModifier modifier, int index)
		{
			if (_abilityIdentity.isAuthority)
			{
				RecordInvokeEntryContext entry;
				StartRecordInvokeEntry(0, modifier.instancedModifierID, 0u, 255, out entry);
				Offset<MetaArg_ModifierChange> offset = MetaArg_ModifierChange.CreateMetaArg_ModifierChange(entry.builder, ModifierAction.Removed, (byte)modifier.config.localID);
				entry.Finish(offset, AbilityInvokeArgument.MetaArg_ModifierChange);
				base.RemoveModifierOnIndex(modifier, index);
			}
		}

		protected override void AddAppliedAbilities()
		{
			if (_abilityIdentity.isAuthority)
			{
				RecordInvokeEntryContext entry;
				StartRecordInvokeEntry(0, 0, 0u, 255, out entry);
				Offset<MetaArg_AbilityControl> offset = MetaArg_AbilityControl.CreateMetaArg_AbilityControl(entry.builder);
				entry.Finish(offset, AbilityInvokeArgument.MetaArg_AbilityControl);
				base.AddAppliedAbilities();
			}
		}

		protected override void RemoveAllAbilities()
		{
			if (_abilityIdentity.isAuthority)
			{
				RecordInvokeEntryContext entry;
				StartRecordInvokeEntry(0, 0, 0u, 255, out entry);
				Offset<MetaArg_AbilityControl> offset = MetaArg_AbilityControl.CreateMetaArg_AbilityControl(entry.builder, AbilityControlType.RemoveAllAbilities);
				entry.Finish(offset, AbilityInvokeArgument.MetaArg_AbilityControl);
				base.RemoveAllAbilities();
			}
		}

		protected override void RemoveAllNonOnDestroyAbilities()
		{
			if (_abilityIdentity.isAuthority)
			{
				RecordInvokeEntryContext entry;
				StartRecordInvokeEntry(0, 0, 0u, 255, out entry);
				Offset<MetaArg_AbilityControl> offset = MetaArg_AbilityControl.CreateMetaArg_AbilityControl(entry.builder, AbilityControlType.RemoveAllNonDestroyAbilities);
				entry.Finish(offset, AbilityInvokeArgument.MetaArg_AbilityControl);
				base.RemoveAllNonOnDestroyAbilities();
			}
		}

		protected override void RemoveAllModifies()
		{
			if (_abilityIdentity.isAuthority)
			{
				base.RemoveAllModifies();
			}
		}

		private void MetaAuthorityInvokeHandler(AbilityInvokeEntry invokeEntry)
		{
			if (invokeEntry.ArgumentType == AbilityInvokeArgument.MetaArg_Command_ModifierChangeRequest)
			{
				MetaAuthorityCommand_ModifierChangeRequestHandler(invokeEntry);
			}
		}

		private void MetaAuthorityCommand_ModifierChangeRequestHandler(AbilityInvokeEntry invokeEntry)
		{
			_metaModifierReq = invokeEntry.GetArgument(_metaModifierReq);
			uint target = invokeEntry.Target;
			uint casterID = ((target != 0) ? target : _owner.runtimeID);
			if (_metaModifierReq.Action == ModifierAction.Added)
			{
				MPTryApplyModifierByID(casterID, invokeEntry.InstancedAbilityID, _metaModifierReq.ModifierLocalID);
			}
			else if (_metaModifierReq.Action == ModifierAction.Removed)
			{
				MPTryRemoveModifierByID(casterID, invokeEntry.InstancedAbilityID, _metaModifierReq.ModifierLocalID);
			}
		}

		private void MetaRemoteInvokeHandler(AbilityInvokeEntry invokeEntry)
		{
			if (invokeEntry.ArgumentType == AbilityInvokeArgument.MetaArg_ModifierChange)
			{
				MetaHandleModifierChange(invokeEntry);
			}
			else if (invokeEntry.ArgumentType == AbilityInvokeArgument.MetaArg_AbilityControl)
			{
				MetaHandlerAbilityControl(invokeEntry);
			}
		}

		private void MetaHandlerAbilityControl(AbilityInvokeEntry invokeEntry)
		{
			_metaAbilityControl = invokeEntry.GetArgument(_metaAbilityControl);
			switch (_metaAbilityControl.Type)
			{
			case AbilityControlType.AddAppliedAbilities:
				base.AddAppliedAbilities();
				break;
			case AbilityControlType.RemoveAllAbilities:
				base.RemoveAllAbilities();
				break;
			case AbilityControlType.RemoveAllNonDestroyAbilities:
				base.RemoveAllNonOnDestroyAbilities();
				break;
			}
		}

		private void MetaHandleModifierChange(AbilityInvokeEntry table)
		{
			_metaModifierChange = table.GetArgument(_metaModifierChange);
			if (_metaModifierChange.Action == ModifierAction.Added)
			{
				ActorAbility actorAbility = null;
				ConfigAbilityModifier configAbilityModifier = null;
				int index = table.InstancedModifierID - 1;
				BaseAbilityActor baseAbilityActor = ((table.Target != 0) ? Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(table.Target) : _owner);
				actorAbility = ((MPActorAbilityPlugin)baseAbilityActor.abilityPlugin).GetInstancedAbilityByID(table.InstancedAbilityID);
				configAbilityModifier = actorAbility.config.ModifierIDMap[_metaModifierChange.ModifierLocalID];
				base.AddModifierOnIndex(actorAbility, configAbilityModifier, index);
			}
			else
			{
				ActorModifier instancedModifierByID = GetInstancedModifierByID(table.InstancedModifierID);
				int index2 = table.InstancedModifierID - 1;
				base.RemoveModifierOnIndex(instancedModifierByID, index2);
			}
		}

		public void SetupIdentity(BaseAbilityEntityIdentiy identity)
		{
			_abilityIdentity = identity;
			base.OnAdded();
		}

		public override void OnAdded()
		{
			MPManager instance = Singleton<MPManager>.Instance;
			instance.OnFrameEnd = (Action)Delegate.Combine(instance.OnFrameEnd, new Action(FlushRecordInvokeEntriesAndSend));
		}

		public override void OnRemoved()
		{
			MPManager instance = Singleton<MPManager>.Instance;
			instance.OnFrameEnd = (Action)Delegate.Remove(instance.OnFrameEnd, new Action(FlushRecordInvokeEntriesAndSend));
			base.OnRemoved();
		}

		public override BaseAbilityMixin CreateInstancedAbilityMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
		{
			BaseAbilityMixin baseAbilityMixin = config.MPCreateInstancedMixin(instancedAbility, instancedModifier);
			if (baseAbilityMixin == null)
			{
				return null;
			}
			baseAbilityMixin.selfIdentity = Singleton<MPManager>.Instance.GetIdentity<BaseAbilityEntityIdentiy>(baseAbilityMixin.actor.runtimeID);
			return baseAbilityMixin;
		}

		public ActorAbility GetInstancedAbilityByID(int appliedAbilityID)
		{
			return _appliedAbilities[appliedAbilityID - 1];
		}

		public ActorModifier GetInstancedModifierByID(int appliedModifierID)
		{
			return _appliedModifiers[appliedModifierID - 1];
		}

		public bool MPTryRemoveModifierByID(uint casterID, int instancedAbilityID, int modifierLocalID)
		{
			for (int i = 0; i < _appliedModifiers.Count; i++)
			{
				if (_appliedModifiers[i] != null)
				{
					ActorModifier actorModifier = _appliedModifiers[i];
					if (actorModifier.parentAbility.caster.runtimeID == casterID && actorModifier.parentAbility.instancedAbilityID == instancedAbilityID && actorModifier.config.localID == modifierLocalID)
					{
						RemoveModifier(actorModifier, i);
						return true;
					}
				}
			}
			return false;
		}

		public bool MPTryApplyModifierByID(uint casterID, int instancedAbilityID, int modifierLocalID)
		{
			BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(casterID);
			if (actor == null)
			{
				return false;
			}
			ActorAbility instancedAbilityByID = actor.mpAbilityPlugin.GetInstancedAbilityByID(instancedAbilityID);
			if (instancedAbilityByID == null)
			{
				return false;
			}
			ConfigAbilityModifier modifierConfig = instancedAbilityByID.config.ModifierIDMap[modifierLocalID];
			ActorModifier actorModifier = ApplyModifier(instancedAbilityByID, modifierConfig);
			return actorModifier != null;
		}

		protected override void HandleActionTargetDispatch(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor other, BaseEvent evt, Func<BaseAbilityActor, bool> targetPredicate)
		{
			if (_abilityIdentity.isAuthority)
			{
				base.HandleActionTargetDispatch(actionConfig, instancedAbility, instancedModifier, other, evt, targetPredicate);
			}
		}

		public void ExternalResolveTarget(AbilityTargetting targetting, TargettingOption option, ActorAbility instancedAbility, BaseAbilityActor other, out BaseAbilityActor outTarget, out BaseAbilityActor[] outTargetLs, out bool needHandleTargetOnNull)
		{
			ResolveTarget(targetting, option, instancedAbility, other, out outTarget, out outTargetLs, out needHandleTargetOnNull);
		}
	}
}
