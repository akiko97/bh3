using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityBronyaArsenalMixin : BaseAbilityMixin
	{
		private class CannonInfo
		{
			public Vector3 localPosition;

			public Vector3 localForward;

			public Vector3 position;

			public Vector3 forward;

			public float delay;

			public List<MonoEffect> chargeEffects;

			public List<MonoEffect> cannonEffects;

			public Vector3 targetPositon = Vector3.zero;

			public float fireTimer;
		}

		private enum ArsenalState
		{
			None = 0,
			Charge = 1,
			Fire = 2,
			Disappear = 3
		}

		private BronyaArsenalMixin config;

		private BaseMonoMonster _monster;

		private List<CannonInfo> _cannonList;

		private int _chargeIndex;

		private float _Timer;

		private float _shakeTimer;

		private ArsenalState _state;

		public AbilityBronyaArsenalMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (BronyaArsenalMixin)config;
			_monster = entity as BaseMonoMonster;
			_cannonList = new List<CannonInfo>();
			_chargeIndex = 0;
			_Timer = 0f;
			_state = ArsenalState.None;
		}

		private void InitCannonInfo()
		{
			MonoEffect monoEffect = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(config.PositionsEffect, entity.XZPosition, entity.transform.forward, Vector3.one, entity)[0];
			int num = Mathf.Min(config.DelayList.Length, monoEffect.transform.childCount);
			for (int i = 0; i < num; i++)
			{
				CannonInfo cannonInfo = new CannonInfo();
				cannonInfo.localPosition = monoEffect.transform.GetChild(i).localPosition;
				CannonInfo item = cannonInfo;
				_cannonList.Add(item);
			}
			_cannonList.Sort((CannonInfo x, CannonInfo y) => (int)(x.localPosition.magnitude - y.localPosition.magnitude));
			for (int num2 = 0; num2 < _cannonList.Count; num2++)
			{
				float angle = 22f - Mathf.Clamp(_cannonList[num2].localPosition.y, 2f, 6f);
				_cannonList[num2].delay = config.DelayList[num2];
				_cannonList[num2].localForward = Quaternion.AngleAxis(angle, Vector3.right) * Vector3.forward;
				_cannonList[num2].fireTimer = Random.Range(config.FireIntervial, config.FireIntervial * 2f);
			}
			monoEffect.SetDestroy();
		}

		public override void OnAbilityTriggered(EvtAbilityStart evt)
		{
			_Timer = 0f;
			ClearCannons();
			InitCannonInfo();
			_state = ArsenalState.Charge;
			_chargeIndex = 0;
		}

		public override void Core()
		{
			base.Core();
			_Timer += Time.deltaTime * entity.TimeScale;
			if (_state == ArsenalState.None)
			{
				return;
			}
			if (_state == ArsenalState.Charge)
			{
				if (_chargeIndex < _cannonList.Count)
				{
					CannonInfo cannonInfo = _cannonList[_chargeIndex];
					if (_Timer >= cannonInfo.delay)
					{
						Vector3 vector = Quaternion.FromToRotation(Vector3.forward, entity.transform.forward) * cannonInfo.localForward;
						Vector3 vector2 = (cannonInfo.position = entity.XZPosition + Quaternion.FromToRotation(Vector3.forward, entity.transform.forward) * cannonInfo.localPosition);
						cannonInfo.forward = vector.normalized;
						cannonInfo.cannonEffects = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(config.CannonEffects, vector2, vector, Vector3.one, entity);
						UpdateEffects(cannonInfo.cannonEffects, vector2, vector);
						cannonInfo.chargeEffects = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(config.ChargeEffects, vector2, vector, Vector3.one, entity);
						UpdateEffects(cannonInfo.chargeEffects, vector2, vector);
						Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(config.HintEffects, GetTargetPosition(cannonInfo), Vector3.forward, Vector3.one, entity);
						_chargeIndex++;
					}
				}
				if (_Timer > config.ChargeTime)
				{
					_state = ArsenalState.Fire;
					_Timer = 0f;
					_shakeTimer = config.FireIntervial;
				}
			}
			else if (_state == ArsenalState.Fire)
			{
				for (int i = 0; i < _cannonList.Count; i++)
				{
					CannonInfo cannonInfo2 = _cannonList[i];
					cannonInfo2.fireTimer -= Time.deltaTime * entity.TimeScale;
					if (!(cannonInfo2.fireTimer > 0f))
					{
						List<MonoEffect> effects = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(config.ShootEffects, cannonInfo2.position, cannonInfo2.forward, Vector3.one, entity);
						UpdateEffects(effects, cannonInfo2.position, cannonInfo2.forward);
						Vector2 vector3 = Random.insideUnitCircle * config.ExplodeRadius;
						Vector3 vector4 = new Vector3(vector3.x, 0f, vector3.y);
						Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(config.ExplodeEffects, GetTargetPosition(cannonInfo2) + vector4, Vector3.forward, Vector3.one, entity);
						CreateExplode(GetTargetPosition(cannonInfo2));
						cannonInfo2.fireTimer = config.FireIntervial;
					}
				}
				_shakeTimer -= Time.deltaTime * entity.TimeScale;
				if (_shakeTimer <= 0f)
				{
					ConfigMonsterAnimEvent configMonsterAnimEvent = SharedAnimEventData.ResolveAnimEvent(_monster.config, config.ShakeAnimEventID);
					if (configMonsterAnimEvent.CameraShake != null)
					{
						AttackPattern.ActCameraShake(configMonsterAnimEvent.CameraShake);
					}
					_shakeTimer = config.FireIntervial;
				}
				if (_Timer > config.FireTime)
				{
					_state = ArsenalState.Disappear;
					_Timer = 0f;
				}
			}
			else if (_Timer > config.ClearTime)
			{
				_state = ArsenalState.None;
				ClearCannons();
			}
		}

		private void CreateExplode(Vector3 pos)
		{
			if (config.ExplodeAnimEventID == null)
			{
				return;
			}
			List<CollisionResult> list = CollisionDetectPattern.CylinderCollisionDetectBySphere(pos, pos, config.ExplodeRadius, config.ExplodeRadius, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(actor.runtimeID, MixinTargetting.Enemy));
			for (int i = 0; i < list.Count; i++)
			{
				CollisionResult collisionResult = list[i];
				BaseMonoEntity collisionResultEntity = AttackPattern.GetCollisionResultEntity(collisionResult.entity);
				if (!(collisionResultEntity == null))
				{
					Singleton<EventManager>.Instance.FireEvent(new EvtHittingOther(actor.runtimeID, collisionResultEntity.GetRuntimeID(), config.ExplodeAnimEventID, collisionResult.hitPoint, collisionResult.hitForward));
				}
			}
		}

		private void UpdateEffects(List<MonoEffect> _effects, Vector3 _pos, Vector3 _dir)
		{
			if (_effects == null)
			{
				return;
			}
			foreach (MonoEffect _effect in _effects)
			{
				_effect.transform.position = _pos;
				_effect.transform.forward = _dir;
			}
		}

		private void DestroyEffects(List<MonoEffect> _effects)
		{
			if (_effects == null)
			{
				return;
			}
			foreach (MonoEffect _effect in _effects)
			{
				_effect.SetDestroyImmediately();
			}
			_effects.Clear();
		}

		private void ClearCannons()
		{
			foreach (CannonInfo cannon in _cannonList)
			{
				DestroyEffects(cannon.chargeEffects);
				DestroyEffects(cannon.cannonEffects);
			}
			_cannonList.Clear();
		}

		private Vector3 GetTargetPosition(CannonInfo cannon)
		{
			float num = Mathf.Abs(cannon.position.y / cannon.forward.y);
			return cannon.position + cannon.forward * num;
		}
	}
}
