using System.Collections.Generic;
using System.Diagnostics;
using FullInspector;
using MoleMole.Config;
using UniRx;

namespace MoleMole
{
	public abstract class BaseActorActionContext
	{
		private enum OwnedPredicateState
		{
			Attach = 0,
			Detach = 1
		}

		[InspectorCollapsedFoldout]
		public BaseAbilityMixin[] instancedMixins;

		private List<Tuple<BaseMonoAbilityEntity, string, OwnedPredicateState>> _ownedPredicates;

		private List<int> _attachedPatternIndices;

		private List<BaseMonoAbilityEntity> _materialGroupPushed;

		private List<Tuple<BaseMonoAbilityEntity, string>> _animEventMasked;

		private List<ActorModifier> _modifiersAttached;

		private List<Tuple<BaseAbilityActor, AbilityState>> _attachedStateImmunes;

		private List<Tuple<BaseAbilityActor, ConfigBuffDebuffResistance>> _attachedResistanceBuffDebuffs;

		private List<Tuple<BaseMonoAbilityEntity, bool>> _attachedIsGhost;

		private List<BaseMonoAbilityEntity> _attachedNoCollisions;

		private List<Tuple<BaseMonoAbilityEntity, bool>> _attachedAllowSelected;

		private List<Tuple<AvatarActor, bool>> _attachAllowSwitchOther;

		private List<Tuple<AvatarActor, bool>> _attachMuteOtherQTE;

		private List<BaseAbilityActor> _attachedImmuneDebuff;

		private List<Tuple<BaseMonoAbilityEntity, float>> _attachedOpacity;

		private List<Tuple<BaseMonoAbilityEntity, string>> _attachedEffectOverrides;

		private List<int> _attachedStageTintsIndices;

		public BaseActorActionContext()
		{
		}

		public BaseAbilityMixin GetInstancedMixin(int mixinLocalID)
		{
			for (int i = 0; i < instancedMixins.Length; i++)
			{
				if (instancedMixins[i].mixinLocalID == mixinLocalID)
				{
					return instancedMixins[i];
				}
			}
			return null;
		}

		public abstract string GetDebugContextName();

		public abstract BaseAbilityActor GetDebugOwner();

		[Conditional("NG_HSOD_DEBUG")]
		[Conditional("UNITY_EDITOR")]
		protected void DebugLogContext(string format, params object[] args)
		{
		}

		private void CheckInit<T>(ref List<T> ls)
		{
			if (ls == null)
			{
				ls = new List<T>();
			}
		}

		public void AttachEffectPatternIndex(int patternIx)
		{
			CheckInit(ref _attachedPatternIndices);
			_attachedPatternIndices.Add(patternIx);
		}

		public void AttachStageTint(AttachStageTint tintConfig)
		{
			CheckInit(ref _attachedStageTintsIndices);
			int item = Singleton<StageManager>.Instance.GetPerpStage().PushRenderingData(tintConfig.RenderDataName, tintConfig.TransitDuration);
			_attachedStageTintsIndices.Add(item);
		}

		public void AttachAnimEventPredicate(BaseMonoAbilityEntity target, string predicate)
		{
			CheckInit(ref _ownedPredicates);
			_ownedPredicates.Add(Tuple.Create(target, predicate, OwnedPredicateState.Attach));
		}

		public void DetachAnimEventPredicate(BaseMonoAbilityEntity target, string predicate)
		{
			CheckInit(ref _ownedPredicates);
			_ownedPredicates.Add(Tuple.Create(target, predicate, OwnedPredicateState.Detach));
		}

		public void AttachPushMaterialGroup(BaseMonoAbilityEntity target)
		{
			CheckInit(ref _materialGroupPushed);
			_materialGroupPushed.Add(target);
		}

		public void AttachMaskedAnimEventID(BaseMonoAbilityEntity target, string animEventID)
		{
			CheckInit(ref _animEventMasked);
			_animEventMasked.Add(Tuple.Create(target, animEventID));
		}

		public void AttachModifier(ActorModifier modifier)
		{
			CheckInit(ref _modifiersAttached);
			_modifiersAttached.Add(modifier);
		}

		public void AttachImmuneAbilityState(BaseAbilityActor target, AbilityState state)
		{
			CheckInit(ref _attachedStateImmunes);
			_attachedStateImmunes.Add(Tuple.Create(target, state));
		}

		public void AttachImmuneDebuff(BaseAbilityActor target)
		{
			CheckInit(ref _attachedImmuneDebuff);
			_attachedImmuneDebuff.Add(target);
		}

		public void AttachBuffDebuffResistance(BaseAbilityActor target, AttachBuffDebuffResistance resistance)
		{
			CheckInit(ref _attachedResistanceBuffDebuffs);
			ConfigBuffDebuffResistance configBuffDebuffResistance = new ConfigBuffDebuffResistance(resistance.ResistanceBuffDebuffs, resistance.ResistanceRatio, resistance.ResistanceDurationRatio);
			_attachedResistanceBuffDebuffs.Add(Tuple.Create(target, configBuffDebuffResistance));
			target.AddBuffDebuffResistance(configBuffDebuffResistance);
		}

		public void AttachIsGhost(BaseMonoAbilityEntity target, bool isGhost)
		{
			target.SetCountedIsGhost(isGhost);
			CheckInit(ref _attachedIsGhost);
			_attachedIsGhost.Add(Tuple.Create(target, isGhost));
		}

		public void AttachAllowSwitchOther(BaseMonoAbilityEntity target, bool allowSwitchOther)
		{
			if (Singleton<LevelManager>.Instance.levelActor.levelMode != LevelActor.Mode.Single)
			{
				return;
			}
			CheckInit(ref _attachAllowSwitchOther);
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(target.GetRuntimeID());
			if (actor != null)
			{
				actor.AllowOtherSwitchIn = allowSwitchOther;
				if (allowSwitchOther)
				{
					actor.ResetSwitchInTimer();
				}
				_attachAllowSwitchOther.Add(Tuple.Create(actor, allowSwitchOther));
			}
		}

		public void AttachMuteOtherQTE(BaseMonoAbilityEntity target, bool muteOtherQTE)
		{
			CheckInit(ref _attachMuteOtherQTE);
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(target.GetRuntimeID());
			if (actor != null)
			{
				actor.MuteOtherQTE = muteOtherQTE;
				_attachMuteOtherQTE.Add(Tuple.Create(actor, muteOtherQTE));
			}
		}

		public void AttachNoCollision(BaseMonoAbilityEntity target)
		{
			target.PushNoCollision();
			CheckInit(ref _attachedNoCollisions);
			_attachedNoCollisions.Add(target);
		}

		public void AttachAllowSelected(BaseMonoAbilityEntity target, bool allowSelected)
		{
			target.SetCountedDenySelect(!allowSelected);
			CheckInit(ref _attachedAllowSelected);
			_attachedAllowSelected.Add(Tuple.Create(target, allowSelected));
		}

		public void AttachOpacity(BaseMonoAbilityEntity target, float opacity)
		{
			PropObjectActor actor = Singleton<EventManager>.Instance.GetActor<PropObjectActor>(target.GetRuntimeID());
			if (actor != null)
			{
				actor.SetPorpObjectOpacity(opacity);
				CheckInit(ref _attachedOpacity);
				_attachedOpacity.Add(Tuple.Create(target, opacity));
			}
		}

		public void AttachEffectOverride(BaseMonoAbilityEntity target, string key)
		{
			CheckInit(ref _attachedEffectOverrides);
			_attachedEffectOverrides.Add(Tuple.Create(target, key));
		}

		protected void AttachToActor(BaseAbilityActor actor)
		{
		}

		protected void DetachFromActor(BaseAbilityActor actor)
		{
			if (_ownedPredicates != null)
			{
				for (int i = 0; i < _ownedPredicates.Count; i++)
				{
					Tuple<BaseMonoAbilityEntity, string, OwnedPredicateState> tuple = _ownedPredicates[i];
					if (tuple.Item1 != null)
					{
						if (tuple.Item3 == OwnedPredicateState.Attach)
						{
							tuple.Item1.RemoveAnimEventPredicate(tuple.Item2);
						}
						else
						{
							tuple.Item1.AddAnimEventPredicate(tuple.Item2);
						}
					}
				}
				_ownedPredicates.Clear();
			}
			if (_attachedPatternIndices != null)
			{
				for (int j = 0; j < _attachedPatternIndices.Count; j++)
				{
					actor.entity.DetachEffect(_attachedPatternIndices[j]);
				}
				_attachedPatternIndices.Clear();
			}
			if (_materialGroupPushed != null)
			{
				for (int k = 0; k < _materialGroupPushed.Count; k++)
				{
					if (_materialGroupPushed[k] != null)
					{
						_materialGroupPushed[k].PopMaterialGroup();
					}
				}
				_materialGroupPushed.Clear();
			}
			if (_animEventMasked != null)
			{
				for (int l = 0; l < _animEventMasked.Count; l++)
				{
					if (_animEventMasked[l].Item1 != null)
					{
						_animEventMasked[l].Item1.UnmaskAnimEvent(_animEventMasked[l].Item2);
					}
				}
				_animEventMasked.Clear();
			}
			if (_modifiersAttached != null)
			{
				for (int m = 0; m < _modifiersAttached.Count; m++)
				{
					ActorModifier actorModifier = _modifiersAttached[m];
					if (actorModifier.owner != null)
					{
						bool flag = actorModifier.owner.abilityPlugin.TryRemoveModifier(actorModifier);
					}
				}
				_modifiersAttached.Clear();
			}
			if (_attachedStateImmunes != null)
			{
				for (int n = 0; n < _attachedStateImmunes.Count; n++)
				{
					Tuple<BaseAbilityActor, AbilityState> tuple2 = _attachedStateImmunes[n];
					if (tuple2.Item1 != null)
					{
						tuple2.Item1.SetAbilityStateImmune(tuple2.Item2, false);
					}
				}
				_attachedStateImmunes.Clear();
			}
			if (_attachedImmuneDebuff != null)
			{
				for (int num = 0; num < _attachedImmuneDebuff.Count; num++)
				{
					BaseAbilityActor baseAbilityActor = _attachedImmuneDebuff[num];
					if (_attachedImmuneDebuff[num] != null)
					{
						baseAbilityActor.SetImmuneDebuff(false);
					}
				}
				_attachedImmuneDebuff.Clear();
			}
			if (_attachedResistanceBuffDebuffs != null)
			{
				for (int num2 = 0; num2 < _attachedResistanceBuffDebuffs.Count; num2++)
				{
					BaseAbilityActor item = _attachedResistanceBuffDebuffs[num2].Item1;
					if (item != null)
					{
						item.RemoveBuffDebuffResistance(_attachedResistanceBuffDebuffs[num2].Item2);
					}
				}
				_attachedResistanceBuffDebuffs.Clear();
			}
			if (_attachedIsGhost != null)
			{
				for (int num3 = 0; num3 < _attachedIsGhost.Count; num3++)
				{
					Tuple<BaseMonoAbilityEntity, bool> tuple3 = _attachedIsGhost[num3];
					if (!(tuple3.Item1 == null))
					{
						tuple3.Item1.SetCountedIsGhost(!tuple3.Item2);
					}
				}
				_attachedIsGhost.Clear();
			}
			if (_attachedAllowSelected != null)
			{
				for (int num4 = 0; num4 < _attachedAllowSelected.Count; num4++)
				{
					Tuple<BaseMonoAbilityEntity, bool> tuple4 = _attachedAllowSelected[num4];
					if (!(tuple4.Item1 == null))
					{
						tuple4.Item1.SetCountedDenySelect(tuple4.Item2);
					}
				}
				_attachedAllowSelected.Clear();
			}
			if (_attachAllowSwitchOther != null)
			{
				for (int num5 = 0; num5 < _attachAllowSwitchOther.Count; num5++)
				{
					Tuple<AvatarActor, bool> tuple5 = _attachAllowSwitchOther[num5];
					if (tuple5.Item1 != null)
					{
						tuple5.Item1.SetAllowOtherCanSwitchIn(false);
					}
				}
				_attachAllowSwitchOther.Clear();
			}
			if (_attachMuteOtherQTE != null)
			{
				for (int num6 = 0; num6 < _attachMuteOtherQTE.Count; num6++)
				{
					Tuple<AvatarActor, bool> tuple6 = _attachMuteOtherQTE[num6];
					if (tuple6.Item1 != null)
					{
						tuple6.Item1.MuteOtherQTE = false;
					}
				}
				_attachMuteOtherQTE.Clear();
			}
			if (_attachedOpacity != null)
			{
				for (int num7 = 0; num7 < _attachedOpacity.Count; num7++)
				{
					Tuple<BaseMonoAbilityEntity, float> tuple7 = _attachedOpacity[num7];
					if (!(tuple7.Item1 == null))
					{
						PropObjectActor actor2 = Singleton<EventManager>.Instance.GetActor<PropObjectActor>(tuple7.Item1.GetRuntimeID());
						if (actor2 != null)
						{
							actor2.SetPorpObjectOpacity(actor2.Opacity);
						}
					}
				}
				_attachedOpacity.Clear();
			}
			if (_attachedEffectOverrides != null)
			{
				for (int num8 = 0; num8 < _attachedEffectOverrides.Count; num8++)
				{
					Tuple<BaseMonoAbilityEntity, string> tuple8 = _attachedEffectOverrides[num8];
					if (!(tuple8.Item1 == null))
					{
						tuple8.Item1.RemoveEffectOverride(tuple8.Item2);
					}
				}
				_attachedEffectOverrides.Clear();
			}
			if (_attachedStageTintsIndices != null)
			{
				for (int num9 = 0; num9 < _attachedStageTintsIndices.Count; num9++)
				{
					int stackIx = _attachedStageTintsIndices[num9];
					Singleton<StageManager>.Instance.GetPerpStage().PopRenderingData(stackIx);
				}
				_attachedStageTintsIndices.Clear();
			}
			if (_attachedNoCollisions == null)
			{
				return;
			}
			for (int num10 = 0; num10 < _attachedNoCollisions.Count; num10++)
			{
				if (!(_attachedNoCollisions[num10] == null))
				{
					_attachedNoCollisions[num10].PopNoCollision();
				}
			}
			_attachedNoCollisions.Clear();
		}
	}
}
