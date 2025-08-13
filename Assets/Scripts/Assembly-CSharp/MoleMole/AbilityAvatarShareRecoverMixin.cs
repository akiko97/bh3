using System;
using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public class AbilityAvatarShareRecoverMixin : BaseAbilityMixin
	{
		private AvatarShareRecoverMixin config;

		public AbilityAvatarShareRecoverMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarShareRecoverMixin)config;
		}

		public override void OnAdded()
		{
			if (config.ShareHP)
			{
				BaseAbilityActor baseAbilityActor = actor;
				baseAbilityActor.onHPChanged = (Action<float, float, float>)Delegate.Combine(baseAbilityActor.onHPChanged, new Action<float, float, float>(ShareHPCallback));
			}
			if (config.ShareSP)
			{
				BaseAbilityActor baseAbilityActor2 = actor;
				baseAbilityActor2.onSPChanged = (Action<float, float, float>)Delegate.Combine(baseAbilityActor2.onSPChanged, new Action<float, float, float>(ShareSPCallback));
			}
		}

		public override void OnRemoved()
		{
			if (config.ShareHP)
			{
				BaseAbilityActor baseAbilityActor = actor;
				baseAbilityActor.onHPChanged = (Action<float, float, float>)Delegate.Remove(baseAbilityActor.onHPChanged, new Action<float, float, float>(ShareHPCallback));
			}
			if (config.ShareSP)
			{
				BaseAbilityActor baseAbilityActor2 = actor;
				baseAbilityActor2.onSPChanged = (Action<float, float, float>)Delegate.Remove(baseAbilityActor2.onSPChanged, new Action<float, float, float>(ShareSPCallback));
			}
		}

		private void ShareHPCallback(float from, float to, float amount)
		{
			if (amount <= 0f || !actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, null, null))
			{
				return;
			}
			float amount2 = instancedAbility.Evaluate(config.ShareHPRatio) * amount;
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			for (int i = 0; i < allPlayerAvatars.Count; i++)
			{
				BaseMonoAvatar baseMonoAvatar = allPlayerAvatars[i];
				if (!(baseMonoAvatar == null) && baseMonoAvatar.IsAlive() && baseMonoAvatar.GetRuntimeID() != actor.runtimeID)
				{
					AvatarActor avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(baseMonoAvatar.GetRuntimeID());
					avatarActor.HealHP(amount2);
				}
			}
		}

		private void ShareSPCallback(float from, float to, float amount)
		{
			if (amount <= 0f || !actor.abilityPlugin.EvaluateAbilityPredicate(config.Predicates, instancedAbility, instancedModifier, null, null))
			{
				return;
			}
			float amount2 = instancedAbility.Evaluate(config.ShareSPRatio) * amount;
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			for (int i = 0; i < allPlayerAvatars.Count; i++)
			{
				BaseMonoAvatar baseMonoAvatar = allPlayerAvatars[i];
				if (!(baseMonoAvatar == null) && baseMonoAvatar.IsAlive() && baseMonoAvatar.GetRuntimeID() != actor.runtimeID)
				{
					AvatarActor avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(baseMonoAvatar.GetRuntimeID());
					avatarActor.HealSP(amount2);
				}
			}
		}
	}
}
