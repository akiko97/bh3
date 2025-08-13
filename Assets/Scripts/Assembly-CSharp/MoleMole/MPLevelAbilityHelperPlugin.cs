using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public class MPLevelAbilityHelperPlugin : BaseActorPlugin
	{
		private class PendingApplyModifierEntry
		{
			public ApplyLevelBuff config;

			public uint ownerID;

			public int instancedAbilityID;

			public uint otherTargetID;
		}

		private class EffectiveAttachModifier
		{
			public ApplyLevelBuff config;

			public uint ownerID;

			public int instancedAbilityID;

			public List<SubModifierLocater> attachedModifiers = new List<SubModifierLocater>();
		}

		private struct SubModifierLocater
		{
			public uint modifierOwnerID;

			public int modifierLocalID;
		}

		private enum State
		{
			WaitingForStart = 0,
			WaitingForEnd = 1
		}

		private List<PendingApplyModifierEntry> _pendingApplyLevelBuffs;

		private EffectiveAttachModifier _curAttachedEntry;

		private State _state;

		public MPLevelAbilityHelperPlugin(MPLevelActor mpLevelActor)
		{
			_pendingApplyLevelBuffs = new List<PendingApplyModifierEntry>();
		}

		public override bool OnEvent(BaseEvent evt)
		{
			if (evt is EvtLevelBuffState)
			{
				return OnLevelBuffStateChange((EvtLevelBuffState)evt);
			}
			return false;
		}

		private bool OnLevelBuffStateChange(EvtLevelBuffState evt)
		{
			if (evt.state == LevelBuffState.Start)
			{
				AttachCurrent(evt.sourceId);
				_state = State.WaitingForEnd;
			}
			else if (evt.state == LevelBuffState.Stop)
			{
				DetachCurrent();
				_state = State.WaitingForStart;
			}
			else if (evt.state == LevelBuffState.Switch)
			{
				DetachCurrent();
				AttachCurrent(evt.sourceId);
			}
			return false;
		}

		private void AttachCurrent(uint sourceID)
		{
			for (int i = 0; i < _pendingApplyLevelBuffs.Count; i++)
			{
				PendingApplyModifierEntry pendingApplyModifierEntry = _pendingApplyLevelBuffs[i];
				if (pendingApplyModifierEntry.ownerID == sourceID)
				{
					_curAttachedEntry = AttachLevelBuffModifier(_pendingApplyLevelBuffs[i]);
					break;
				}
			}
			_pendingApplyLevelBuffs.Clear();
		}

		private void DetachCurrent()
		{
			if (_curAttachedEntry != null)
			{
				DetachLevelBuffModifiers(_curAttachedEntry);
				_curAttachedEntry = null;
			}
		}

		private EffectiveAttachModifier AttachLevelBuffModifier(PendingApplyModifierEntry entry)
		{
			BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(entry.ownerID);
			if (actor == null)
			{
				return null;
			}
			ActorAbility instancedAbilityByID = actor.mpAbilityPlugin.GetInstancedAbilityByID(entry.instancedAbilityID);
			if (instancedAbilityByID == null)
			{
				return null;
			}
			BaseAbilityActor actor2 = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(entry.otherTargetID);
			EffectiveAttachModifier effectiveAttachModifier = new EffectiveAttachModifier();
			effectiveAttachModifier.ownerID = entry.ownerID;
			effectiveAttachModifier.config = entry.config;
			effectiveAttachModifier.instancedAbilityID = entry.instancedAbilityID;
			AttachModifier[] attachModifiers = entry.config.AttachModifiers;
			foreach (AttachModifier attachModifier in attachModifiers)
			{
				BaseAbilityActor outTarget;
				BaseAbilityActor[] outTargetLs;
				bool needHandleTargetOnNull;
				actor.mpAbilityPlugin.ExternalResolveTarget(attachModifier.Target, attachModifier.TargetOption, instancedAbilityByID, actor2, out outTarget, out outTargetLs, out needHandleTargetOnNull);
				ConfigAbilityModifier configAbilityModifier = instancedAbilityByID.config.Modifiers[attachModifier.ModifierName];
				int localID = configAbilityModifier.localID;
				if (outTarget != null)
				{
					BaseAbilityEntityIdentiy identity = Singleton<MPManager>.Instance.GetIdentity<BaseAbilityEntityIdentiy>(outTarget.runtimeID);
					identity.Command_TryApplyModifier(entry.ownerID, entry.instancedAbilityID, localID);
					effectiveAttachModifier.attachedModifiers.Add(new SubModifierLocater
					{
						modifierOwnerID = outTarget.runtimeID,
						modifierLocalID = localID
					});
				}
				else
				{
					if (outTargetLs == null)
					{
						continue;
					}
					for (int j = 0; j < outTargetLs.Length; j++)
					{
						if (outTargetLs[j] != null)
						{
							BaseAbilityEntityIdentiy identity2 = Singleton<MPManager>.Instance.GetIdentity<BaseAbilityEntityIdentiy>(outTargetLs[j].runtimeID);
							identity2.Command_TryApplyModifier(entry.ownerID, entry.instancedAbilityID, localID);
							effectiveAttachModifier.attachedModifiers.Add(new SubModifierLocater
							{
								modifierOwnerID = outTargetLs[j].runtimeID,
								modifierLocalID = localID
							});
						}
					}
				}
			}
			return effectiveAttachModifier;
		}

		private void DetachLevelBuffModifiers(EffectiveAttachModifier entry)
		{
			for (int i = 0; i < entry.attachedModifiers.Count; i++)
			{
				BaseAbilityEntityIdentiy baseAbilityEntityIdentiy = Singleton<MPManager>.Instance.TryGetIdentity<BaseAbilityEntityIdentiy>(entry.attachedModifiers[i].modifierOwnerID);
				if (baseAbilityEntityIdentiy != null)
				{
					baseAbilityEntityIdentiy.Command_TryRemoveModifier(entry.ownerID, entry.instancedAbilityID, entry.attachedModifiers[i].modifierLocalID);
				}
			}
		}

		public void AttachPendingModifiersToNextLevelBuff(ApplyLevelBuff config, uint ownerID, int instancedAbilityID, uint otherTargetID)
		{
			_pendingApplyLevelBuffs.Add(new PendingApplyModifierEntry
			{
				config = config,
				ownerID = ownerID,
				instancedAbilityID = instancedAbilityID,
				otherTargetID = otherTargetID
			});
		}
	}
}
