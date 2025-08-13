using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityAvatarQTEMixin : BaseAbilityMixin
	{
		public class QTETarget
		{
			private const float QTE_CHECK_DELAY = 0.3f;

			public MonsterActor monsterActor;

			public float checkDelayTime;

			public QTETarget(MonsterActor actor)
			{
				monsterActor = actor;
				checkDelayTime = 0.3f;
			}

			public void Core()
			{
				if (checkDelayTime > 0f)
				{
					checkDelayTime -= Time.deltaTime;
				}
			}

			public bool CanCheck()
			{
				return !(checkDelayTime > 0f);
			}
		}

		private AvatarQTEMixin config;

		private AvatarActor _avatarActor;

		private List<QTETarget> _qteTargetList = new List<QTETarget>();

		private EntityTimer _qteMaxTimer;

		private EntityTimer _delayQteTimer;

		public AbilityAvatarQTEMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarQTEMixin)config;
			_avatarActor = actor as AvatarActor;
			_qteMaxTimer = new EntityTimer(instancedAbility.Evaluate(this.config.QTEMaxTimeSpan));
			_qteMaxTimer.Reset(false);
			_delayQteTimer = new EntityTimer(instancedAbility.Evaluate(this.config.DelayQTETimeSpan));
			_delayQteTimer.Reset(false);
		}

		public override void OnAdded()
		{
			if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
			{
				BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
				baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
				Singleton<EventManager>.Instance.RegisterEventListener<EvtBuffAdd>(actor.runtimeID);
				Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackLanded>(actor.runtimeID);
			}
		}

		public override void OnRemoved()
		{
			if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
			{
				Singleton<EventManager>.Instance.RemoveEventListener<EvtBuffAdd>(actor.runtimeID);
				Singleton<EventManager>.Instance.RemoveEventListener<EvtAttackLanded>(actor.runtimeID);
				BaseMonoAbilityEntity baseMonoAbilityEntity = entity;
				baseMonoAbilityEntity.onCurrentSkillIDChanged = (Action<string, string>)Delegate.Remove(baseMonoAbilityEntity.onCurrentSkillIDChanged, new Action<string, string>(SkillIDChangedCallback));
			}
			actor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ModifierName);
		}

		public override void Core()
		{
			bool flag = true;
			for (int i = 0; i < config.Conditions.Length; i++)
			{
				QTECondition qteCondition = config.Conditions[i];
				if (!CheckMaintainCondition(qteCondition))
				{
					flag = false;
					break;
				}
			}
			if (_qteMaxTimer.isActive)
			{
				if (!_qteMaxTimer.isTimeUp)
				{
					_qteMaxTimer.Core(1f);
				}
				else
				{
					flag = false;
					_qteMaxTimer.Reset(false);
					_delayQteTimer.Reset(true);
				}
			}
			if (flag)
			{
				return;
			}
			if (!_avatarActor.IsSwitchInCD() && (bool)_avatarActor.isAlive)
			{
				if (_delayQteTimer.isActive)
				{
					if (!_delayQteTimer.isTimeUp)
					{
						_delayQteTimer.Core(1f);
						return;
					}
					_qteTargetList.Clear();
					_avatarActor.DisableQTEAttack();
				}
			}
			else
			{
				_qteTargetList.Clear();
				_avatarActor.DisableQTEAttack();
			}
		}

		private bool CheckMaintainCondition(QTECondition qteCondition)
		{
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(_avatarActor.runtimeID))
			{
				return false;
			}
			AvatarActor avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
			if (avatarActor.MuteOtherQTE)
			{
				return false;
			}
			for (int i = 0; i < _qteTargetList.Count; i++)
			{
				QTETarget qTETarget = _qteTargetList[i];
				qTETarget.Core();
				if (qTETarget.CanCheck())
				{
					MonsterActor monsterActor = qTETarget.monsterActor;
					if (monsterActor == null || !monsterActor.isAlive || !QTERangeCheck(monsterActor, qteCondition.QTERange))
					{
						continue;
					}
					if (qteCondition.QTEType == QTEConditionType.QTEAnimationTag)
					{
						for (int j = 0; j < qteCondition.QTEValues.Length; j++)
						{
							MonsterData.MonsterTagGroup tagGroup = (MonsterData.MonsterTagGroup)(int)Enum.Parse(typeof(MonsterData.MonsterTagGroup), qteCondition.QTEValues[j]);
							if (monsterActor.monster.IsAnimatorInTag(tagGroup))
							{
								return true;
							}
						}
					}
					else
					{
						if (qteCondition.QTEType != QTEConditionType.QTEBuffTag)
						{
							continue;
						}
						for (int k = 0; k < qteCondition.QTEValues.Length; k++)
						{
							AbilityState abilityState = (AbilityState)(int)Enum.Parse(typeof(AbilityState), qteCondition.QTEValues[k]);
							if ((monsterActor.abilityState & abilityState) != AbilityState.None)
							{
								return true;
							}
						}
					}
					continue;
				}
				return true;
			}
			return false;
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtAttackLanded)
			{
				return OnAttackLanded((EvtAttackLanded)evt);
			}
			if (evt is EvtBuffAdd)
			{
				return OnBuffAdd((EvtBuffAdd)evt);
			}
			return false;
		}

		private bool OnAttackLanded(EvtAttackLanded evt)
		{
			bool flag = true;
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackeeID);
			if (baseAbilityActor == null || !(baseAbilityActor is MonsterActor) || Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID))
			{
				return false;
			}
			AvatarActor avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
			if (avatarActor.MuteOtherQTE)
			{
				return false;
			}
			MonsterActor qteTarget = baseAbilityActor as MonsterActor;
			QTECondition[] triggerConditions = config.TriggerConditions;
			foreach (QTECondition qTECondition in triggerConditions)
			{
				if (qTECondition.QTEType == QTEConditionType.QTEAnimationTag)
				{
					flag &= QTERangeCheck(qteTarget, qTECondition.QTERange);
					bool flag2 = false;
					for (int j = 0; j < qTECondition.QTEValues.Length; j++)
					{
						flag2 |= evt.attackResult.hitEffect == (AttackResult.AnimatorHitEffect)(int)Enum.Parse(typeof(AttackResult.AnimatorHitEffect), qTECondition.QTEValues[j]);
					}
					flag = flag && flag2;
				}
				else if (qTECondition.QTEType == QTEConditionType.QTEBuffTag)
				{
					flag = false;
					break;
				}
			}
			if (flag && !_avatarActor.IsSwitchInCD() && !_avatarActor.AllowOtherSwitchIn)
			{
				AddQTETarget(qteTarget);
				_avatarActor.EnableQTEAttack(config.QTEName);
				_qteMaxTimer.Reset(true);
			}
			return false;
		}

		private bool OnBuffAdd(EvtBuffAdd evt)
		{
			bool flag = true;
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID);
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID) || baseAbilityActor == null)
			{
				return false;
			}
			AvatarActor avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
			if (avatarActor.MuteOtherQTE)
			{
				return false;
			}
			MonsterActor qteTarget = baseAbilityActor as MonsterActor;
			QTECondition[] triggerConditions = config.TriggerConditions;
			foreach (QTECondition qTECondition in triggerConditions)
			{
				if (qTECondition.QTEType == QTEConditionType.QTEBuffTag)
				{
					flag &= QTERangeCheck(qteTarget, qTECondition.QTERange);
					bool flag2 = false;
					for (int j = 0; j < qTECondition.QTEValues.Length; j++)
					{
						flag2 |= evt.abilityState == (AbilityState)(int)Enum.Parse(typeof(AbilityState), qTECondition.QTEValues[j]);
					}
					flag = flag && flag2;
				}
				else if (qTECondition.QTEType == QTEConditionType.QTEAnimationTag)
				{
					flag = false;
					break;
				}
			}
			if (flag && !_avatarActor.IsSwitchInCD() && !_avatarActor.AllowOtherSwitchIn)
			{
				AddQTETarget(qteTarget);
				_avatarActor.EnableQTEAttack(config.QTEName);
				_qteMaxTimer.Reset(true);
			}
			return false;
		}

		private void AddQTETarget(MonsterActor actor)
		{
			bool flag = true;
			foreach (QTETarget qteTarget in _qteTargetList)
			{
				if (qteTarget.monsterActor == actor)
				{
					flag = false;
				}
			}
			if (flag)
			{
				_qteTargetList.Add(new QTETarget(actor));
			}
		}

		private bool QTERangeCheck(MonsterActor qteTarget, float targetRange)
		{
			if (qteTarget != null && Vector3.Distance(Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition, qteTarget.entity.XZPosition) < targetRange)
			{
				return true;
			}
			return false;
		}

		private bool QTEAbilityStateCheck(MonsterActor qteTarget, AbilityState targetAbilityState)
		{
			if (qteTarget != null)
			{
				return qteTarget.abilityState == targetAbilityState;
			}
			return false;
		}

		private bool QTEHitEffectCheck(EvtAttackLanded evt, AttackResult.AnimatorHitEffect targetHitEffect)
		{
			if (evt != null && evt.attackResult.hitEffect == targetHitEffect)
			{
				return true;
			}
			return false;
		}

		private bool QTEAnimationTagCheck(MonsterActor qteTarget, MonsterData.MonsterTagGroup animationTag)
		{
			if (qteTarget != null && qteTarget.monster.IsAnimatorInTag(animationTag))
			{
				return true;
			}
			return false;
		}

		private void AddModifier(ActorAbility instancedAbility, string modifierName)
		{
			if (_avatarActor.CanSwitchInQTE())
			{
				actor.abilityPlugin.ApplyModifier(instancedAbility, modifierName);
			}
		}

		private void RemoveModifier(ActorAbility instancedAbility, string modifierName)
		{
			actor.abilityPlugin.TryRemoveModifier(instancedAbility, modifierName);
		}

		private void SkillIDChangedCallback(string from, string to)
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < config.SkillIDs.Length; i++)
			{
				if (config.SkillIDs[i] == from)
				{
					flag = true;
				}
				if (config.SkillIDs[i] == to)
				{
					flag2 = true;
				}
			}
			if (!actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, actor, null))
			{
				RemoveModifier(instancedAbility, config.ModifierName);
			}
			else if (!flag && flag2)
			{
				AddModifier(instancedAbility, config.ModifierName);
				Singleton<EventManager>.Instance.FireEvent(new EvtQTEFire(actor.runtimeID, config.QTEName));
				_qteTargetList.Clear();
				_avatarActor.DisableQTEAttack();
			}
		}
	}
}
