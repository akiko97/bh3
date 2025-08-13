using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityGlobalSubShieldMixin : BaseAbilityMixin
	{
		private GlobalSubShieldMixin config;

		public static string GLOBAL_SHIELD_KEY = "GlobalShield";

		private DynamicActorValue<float> _globalShieldValue;

		private float _totalShieldValue;

		private float[] _shieldEffectRange;

		private int _shieldEffectPatternIx = -1;

		private MixinEffect _currentMixinEffect;

		public AbilityGlobalSubShieldMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (GlobalSubShieldMixin)config;
		}

		public override void OnAdded()
		{
			_globalShieldValue = instancedAbility.caster.abilityPlugin.CreateOrGetDynamicFloat(GLOBAL_SHIELD_KEY, 0f);
			_globalShieldValue.SubAttach(OnShieldChanged, ref _totalShieldValue);
			if (config.ShieldEffects != null)
			{
				_shieldEffectRange = new float[config.ShieldEffectRanges.Length + 1];
				config.ShieldEffectRanges.CopyTo(_shieldEffectRange, 0);
				_shieldEffectRange[config.ShieldEffectRanges.Length] = 1f;
			}
			UpdateShieldEffect();
		}

		public override void OnRemoved()
		{
			_globalShieldValue.SubDetach(OnShieldChanged);
			RemoveEffect();
		}

		public override bool OnPostEvent(BaseEvent evt)
		{
			if (evt is EvtBeingHit)
			{
				return OnPostBeingHit((EvtBeingHit)evt);
			}
			return false;
		}

		private bool OnPostBeingHit(EvtBeingHit evt)
		{
			if (!evt.attackData.isAnimEventAttack)
			{
				return false;
			}
			if (evt.attackData.rejected)
			{
				return false;
			}
			if (actor.abilityState.ContainsState(AbilityState.Invincible) || actor.abilityState.ContainsState(AbilityState.Undamagable))
			{
				return false;
			}
			if (_globalShieldValue == null || _globalShieldValue.Value <= 0f)
			{
				return false;
			}
			float value = _globalShieldValue.Value;
			float num = evt.attackData.damage * (1f - DamageModelLogic.GetDefenceRatio((float)instancedAbility.caster.defense * instancedAbility.Evaluate(config.ShieldDefenceRatio), evt.attackData.attackerLevel));
			float num2 = value - num;
			bool flag = Mathf.Approximately(evt.attackData.GetTotalDamage() - evt.attackData.damage, 0f);
			if (num2 <= 0f)
			{
				num2 = 0f;
			}
			_globalShieldValue.Pub(num2);
			if (num2 > 0f)
			{
				if (flag)
				{
					evt.attackData.Reject(AttackResult.RejectType.RejectAll);
				}
				else
				{
					evt.attackData.damage = 0f;
					evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Mute;
				}
				if (evt.attackData.hitCollision != null)
				{
					FireMixinEffect(config.ShieldSuccessEffect, entity, evt.attackData.hitCollision.hitPoint, evt.attackData.hitCollision.hitDir);
				}
				else
				{
					FireMixinEffect(config.ShieldSuccessEffect, entity);
				}
				actor.abilityPlugin.HandleActionTargetDispatch(config.ShieldSuccessActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
			}
			else if (value > 0f)
			{
				float num3 = 1f - value / evt.attackData.damage;
				evt.attackData.damage *= num3;
				if (config.ShieldBrokenTimeSlow > 0f)
				{
					Singleton<LevelManager>.Instance.levelActor.TimeSlow(config.ShieldBrokenTimeSlow);
				}
				FireMixinEffect(config.ShieldBrokenEffect, entity);
				actor.abilityPlugin.HandleActionTargetDispatch(config.ShieldBrokenActions, instancedAbility, instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
				evt.attackData.frameHalt = 0;
				evt.attackData.attackerAniDamageRatio = 10f;
				Singleton<EventManager>.Instance.FireEvent(new EvtShieldBroken(actor.runtimeID));
				evt.attackData.AddHitFlag(AttackResult.ActorHitFlag.ShieldBroken);
			}
			return true;
		}

		private void OnShieldChanged(float from, float to)
		{
			UpdateShieldEffect();
		}

		private void UpdateShieldEffect()
		{
			MixinEffect mixinEffect = config.ShieldEffects[0];
			float num = _globalShieldValue.Value / _totalShieldValue;
			if (Mathf.Approximately(num, 0f))
			{
				RemoveEffect();
				return;
			}
			for (int i = 0; i < _shieldEffectRange.Length; i++)
			{
				if (num <= _shieldEffectRange[i])
				{
					mixinEffect = config.ShieldEffects[i];
					break;
				}
			}
			if (_currentMixinEffect != mixinEffect)
			{
				if (_currentMixinEffect != null)
				{
					entity.DetachEffect(_shieldEffectPatternIx);
				}
				_currentMixinEffect = mixinEffect;
				_shieldEffectPatternIx = entity.AttachEffect(mixinEffect.EffectPattern);
			}
		}

		private void RemoveEffect()
		{
			if (_currentMixinEffect != null)
			{
				entity.DetachEffect(_shieldEffectPatternIx);
			}
			_currentMixinEffect = null;
		}
	}
}
