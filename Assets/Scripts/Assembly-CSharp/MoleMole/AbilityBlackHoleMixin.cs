using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityBlackHoleMixin : BaseAbilityMixin
	{
		private BlackHoleMixin config;

		private AbilityTriggerField _fieldActor;

		private EntityTimer _blackHoleTimer;

		private List<BaseAbilityActor> _insideActors;

		private Dictionary<BaseAbilityActor, int> _addedVelocityActorsAndIndexDic = new Dictionary<BaseAbilityActor, int>();

		private float _pullVelocity;

		private int _blackHoleEffectIx;

		public AbilityBlackHoleMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (BlackHoleMixin)config;
			_blackHoleTimer = new EntityTimer(instancedAbility.Evaluate(this.config.Duration));
			_blackHoleTimer.SetActive(false);
			_pullVelocity = instancedAbility.Evaluate(this.config.PullVelocity);
			_insideActors = new List<BaseAbilityActor>();
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(actor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldExit>(actor.runtimeID);
		}

		public override void OnRemoved()
		{
			Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldEnter>(actor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldExit>(actor.runtimeID);
			if (_blackHoleTimer.isActive && !_blackHoleTimer.isTimeUp)
			{
				KillBlackHole(false);
			}
		}

		public override void Core()
		{
			if (!_blackHoleTimer.isActive)
			{
				return;
			}
			_blackHoleTimer.Core(1f);
			if (_blackHoleTimer.isTimeUp)
			{
				KillBlackHole(true);
				return;
			}
			for (int i = 0; i < _insideActors.Count; i++)
			{
				BaseAbilityActor baseAbilityActor = _insideActors[i];
				if (baseAbilityActor != null && baseAbilityActor.IsActive())
				{
					if (_pullVelocity > 0f && Miscs.DistancForVec3IgnoreY(baseAbilityActor.entity.XZPosition, _fieldActor.triggerField.transform.position) < 0.3f)
					{
						RemoveAdditiveVelocity(baseAbilityActor);
						continue;
					}
					Vector3 additiveVelocity = _fieldActor.triggerField.transform.position - baseAbilityActor.entity.XZPosition;
					additiveVelocity.y = 0f;
					additiveVelocity.Normalize();
					SetAdditiveVelocity(baseAbilityActor, additiveVelocity);
				}
			}
		}

		private void SetAdditiveVelocity(BaseAbilityActor targetActor, Vector3 additiveVelocity)
		{
			if (targetActor != null)
			{
				if (!_addedVelocityActorsAndIndexDic.ContainsKey(targetActor))
				{
					targetActor.entity.SetHasAdditiveVelocity(true);
					int value = targetActor.entity.AddAdditiveVelocity(additiveVelocity * _pullVelocity);
					_addedVelocityActorsAndIndexDic.Add(targetActor, value);
				}
				else
				{
					int index = _addedVelocityActorsAndIndexDic[targetActor];
					targetActor.entity.SetAdditiveVelocityOfIndex(additiveVelocity * _pullVelocity, index);
				}
			}
		}

		private void RemoveAdditiveVelocity(BaseAbilityActor targetActor)
		{
			if (_addedVelocityActorsAndIndexDic.ContainsKey(targetActor))
			{
				int index = _addedVelocityActorsAndIndexDic[targetActor];
				targetActor.entity.SetAdditiveVelocityOfIndex(Vector3.zero, index);
				targetActor.entity.SetHasAdditiveVelocity(false);
				_addedVelocityActorsAndIndexDic.Remove(targetActor);
			}
		}

		private void KillBlackHole(bool doExplodeHit)
		{
			if (_fieldActor == null || _fieldActor.triggerField == null)
			{
				return;
			}
			List<uint> insideRuntimeIDs = _fieldActor.GetInsideRuntimeIDs();
			Vector3 position = _fieldActor.triggerField.transform.position;
			position.y = 1f;
			for (int i = 0; i < insideRuntimeIDs.Count; i++)
			{
				uint runtimeID = insideRuntimeIDs[i];
				BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(runtimeID);
				if (baseAbilityActor == null || baseAbilityActor.gameObject == null || config.ModifierNames == null || config.ModifierNames.Length <= 0)
				{
					continue;
				}
				int j = 0;
				for (int num = config.ModifierNames.Length; j < num; j++)
				{
					if (baseAbilityActor.abilityPlugin != null)
					{
						baseAbilityActor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ModifierNames[j]);
					}
				}
			}
			if (doExplodeHit && config.ExplodeAnimEventID != null)
			{
				List<CollisionResult> list = CollisionDetectPattern.CylinderCollisionDetectBySphere(_fieldActor.triggerField.XZPosition, _fieldActor.triggerField.XZPosition, instancedAbility.Evaluate(config.Radius), 2f, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(actor.runtimeID, MixinTargetting.Enemy));
				for (int k = 0; k < list.Count; k++)
				{
					CollisionResult collisionResult = list[k];
					BaseMonoEntity collisionResultEntity = AttackPattern.GetCollisionResultEntity(collisionResult.entity);
					if (!(collisionResultEntity == null))
					{
						Singleton<EventManager>.Instance.FireEvent(new EvtHittingOther(actor.runtimeID, collisionResultEntity.GetRuntimeID(), config.ExplodeAnimEventID, collisionResult.hitPoint, collisionResult.hitForward));
					}
				}
			}
			FireMixinEffect(config.DestroyEffect, _fieldActor.triggerField);
			_fieldActor.Kill();
			_blackHoleTimer.timespan = instancedAbility.Evaluate(config.Duration);
			_blackHoleTimer.Reset(false);
			if (config.CreationEffect != null && config.CreationEffect.EffectPattern != null)
			{
				Singleton<EffectManager>.Instance.SetDestroyIndexedEffectPattern(_blackHoleEffectIx);
			}
			foreach (KeyValuePair<BaseAbilityActor, int> item in _addedVelocityActorsAndIndexDic)
			{
				if (item.Key != null && item.Key.entity != null)
				{
					item.Key.entity.SetAdditiveVelocityOfIndex(Vector3.zero, item.Value);
					item.Key.entity.SetHasAdditiveVelocity(false);
				}
			}
			_addedVelocityActorsAndIndexDic.Clear();
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (evt is EvtFieldEnter)
			{
				return ListenFieldEnter((EvtFieldEnter)evt);
			}
			if (evt is EvtFieldExit)
			{
				return ListenFieldExit((EvtFieldExit)evt);
			}
			return false;
		}

		private bool ListenFieldEnter(EvtFieldEnter evt)
		{
			if (_fieldActor == null || _fieldActor.runtimeID != evt.targetID)
			{
				return false;
			}
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
			if (baseAbilityActor == null || baseAbilityActor.gameObject == null)
			{
				return false;
			}
			baseAbilityActor.entity.SetHasAdditiveVelocity(true);
			if (config.ModifierNames != null && config.ModifierNames.Length > 0)
			{
				int i = 0;
				for (int num = config.ModifierNames.Length; i < num; i++)
				{
					baseAbilityActor.abilityPlugin.ApplyModifier(instancedAbility, config.ModifierNames[i]);
				}
			}
			_insideActors.Add(baseAbilityActor);
			return true;
		}

		private bool ListenFieldExit(EvtFieldExit evt)
		{
			if (_fieldActor == null || _fieldActor.runtimeID != evt.targetID)
			{
				return false;
			}
			BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
			if (baseAbilityActor == null || baseAbilityActor.gameObject == null)
			{
				return true;
			}
			if (_insideActors.Contains(baseAbilityActor))
			{
				_insideActors.Remove(baseAbilityActor);
				RemoveAdditiveVelocity(baseAbilityActor);
			}
			if (config.ModifierNames != null && config.ModifierNames.Length > 0)
			{
				int i = 0;
				for (int num = config.ModifierNames.Length; i < num; i++)
				{
					baseAbilityActor.abilityPlugin.TryRemoveModifier(instancedAbility, config.ModifierNames[i]);
				}
			}
			return true;
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			if (_blackHoleTimer.isActive)
			{
				KillBlackHole(true);
			}
			BaseMonoEntity baseMonoEntity = null;
			if (evt.otherID != 0)
			{
				BaseAbilityActor baseAbilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
				if (baseAbilityActor != null && baseAbilityActor.entity != null)
				{
					baseMonoEntity = baseAbilityActor.entity;
				}
			}
			Vector3 initPos;
			if (baseMonoEntity != null)
			{
				initPos = baseMonoEntity.XZPosition + new Vector3(0f, 0.5f, 0f);
			}
			else
			{
				Vector3 origin = entity.XZPosition + new Vector3(0f, 0.5f, 0f);
				initPos = CollisionDetectPattern.GetRaycastPoint(origin, entity.transform.forward, instancedAbility.Evaluate(config.CreationZOffset), 0.2f, 1 << InLevelData.STAGE_COLLIDER_LAYER);
			}
			_fieldActor = Singleton<DynamicObjectManager>.Instance.CreateAbilityTriggerField(initPos, entity.transform.forward, actor, instancedAbility.Evaluate(config.Radius), MixinTargetting.Enemy, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID());
			if (config.CreationEffect != null && config.CreationEffect.EffectPattern != null)
			{
				_blackHoleEffectIx = Singleton<EffectManager>.Instance.CreateIndexedEntityEffectPattern(config.CreationEffect.EffectPattern, _fieldActor.triggerField);
			}
			if (config.ApplyAttackerWitchTimeRatio && evt.TriggerEvent != null)
			{
				EvtEvadeSuccess evtEvadeSuccess = evt.TriggerEvent as EvtEvadeSuccess;
				if (evtEvadeSuccess != null)
				{
					MonsterActor monsterActor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evtEvadeSuccess.attackerID);
					if (monsterActor != null)
					{
						ConfigMonsterAnimEvent configMonsterAnimEvent = SharedAnimEventData.ResolveAnimEvent(monsterActor.config, evtEvadeSuccess.skillID);
						if (configMonsterAnimEvent != null)
						{
							_blackHoleTimer.timespan *= configMonsterAnimEvent.AttackProperty.WitchTimeRatio;
						}
					}
				}
			}
			_blackHoleTimer.Reset(true);
		}
	}
}
