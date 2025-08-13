using MoleMole.Config;
using MoleMole.MPProtocol;
using UnityEngine;

namespace MoleMole
{
	public class BaseAbilityMixin
	{
		public BaseAbilityActor actor;

		public BaseMonoAbilityEntity entity;

		public ActorAbility instancedAbility;

		public ActorModifier instancedModifier;

		public int mixinLocalID;

		public int instancedMixinID;

		public BaseAbilityEntityIdentiy selfIdentity;

		public BaseAbilityMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
		{
			this.instancedAbility = instancedAbility;
			this.instancedModifier = instancedModifier;
			actor = ((instancedModifier == null) ? instancedAbility.caster : instancedModifier.owner);
			entity = actor.entity;
			mixinLocalID = config.localID;
		}

		public virtual void OnAdded()
		{
		}

		public virtual void OnRemoved()
		{
		}

		public virtual void OnAbilityTriggered(EvtAbilityStart evt)
		{
		}

		public virtual bool OnEvent(BaseEvent evt)
		{
			return false;
		}

		public virtual bool OnPostEvent(BaseEvent evt)
		{
			return false;
		}

		public virtual bool ListenEvent(BaseEvent evt)
		{
			return false;
		}

		public virtual void Core()
		{
		}

		public bool EvaluatePredicate(float lhs, float rhs, MixinPredicate predicate)
		{
			switch (predicate)
			{
			case MixinPredicate.Equal:
				return lhs == rhs;
			case MixinPredicate.Greater:
				return lhs > rhs;
			case MixinPredicate.GreaterOrEqual:
				return lhs >= rhs;
			case MixinPredicate.Lesser:
				return lhs < rhs;
			default:
				return false;
			}
		}

		public void FireMixinEffect(MixinEffect effectConfig, BaseMonoEntity target, bool allowInactiveFire = false)
		{
			if (effectConfig == null || !(target != null))
			{
				return;
			}
			bool flag = false;
			if (allowInactiveFire)
			{
				flag = true;
			}
			else if (entity.gameObject.activeSelf)
			{
				flag = true;
			}
			if (flag)
			{
				if (effectConfig.EffectPattern != null)
				{
					entity.FireEffect(effectConfig.EffectPattern, target.transform.position, target.transform.forward);
				}
				if (effectConfig.AudioPattern != null)
				{
					entity.PlayAudio(effectConfig.AudioPattern, target.transform);
				}
			}
		}

		public void FireMixinEffect(MixinEffect effectConfig, BaseMonoEntity target, Vector3 pos, Vector3 forward, bool allowInactiveFire = false)
		{
			if (effectConfig == null || !(target != null))
			{
				return;
			}
			bool flag = false;
			if (allowInactiveFire)
			{
				flag = true;
			}
			else if (entity.gameObject.activeSelf)
			{
				flag = true;
			}
			if (flag)
			{
				if (effectConfig.EffectPattern != null)
				{
					entity.FireEffect(effectConfig.EffectPattern, pos, forward);
				}
				if (effectConfig.AudioPattern != null)
				{
					entity.PlayAudio(effectConfig.AudioPattern, target.transform);
				}
			}
		}

		public int AttachMixinEffect(MixinEffect effectConfig)
		{
			if (effectConfig.AudioPattern != null)
			{
				entity.PlayAudio(effectConfig.AudioPattern, entity.transform);
			}
			return entity.AttachEffect(effectConfig.EffectPattern);
		}

		public virtual void HandleMixinInvokeEntry(AbilityInvokeEntry invokeEntry, int fromPeerID)
		{
		}

		protected void StartRecordMixinInvokeEntry(out RecordInvokeEntryContext context, uint targetID = 0)
		{
			actor.mpAbilityPlugin.StartRecordInvokeEntry(instancedAbility.instancedAbilityID, (instancedModifier != null) ? instancedModifier.instancedModifierID : 0, (targetID != 0) ? targetID : actor.runtimeID, mixinLocalID, out context);
		}
	}
}
