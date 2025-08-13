using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AbilityAvatarAutoUltraModeMixin : BaseAbilityMixin
	{
		private AvatarAutoUltraModeMixin config;

		private bool _isUltraMode;

		public AbilityAvatarAutoUltraModeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
			: base(instancedAbility, instancedModifier, config)
		{
			this.config = (AvatarAutoUltraModeMixin)config;
		}

		public override void Core()
		{
			base.Core();
			bool flag = Singleton<AvatarManager>.Instance.IsLocalAvatar(entity.GetRuntimeID());
			if (!_isUltraMode && (float)actor.SP / (float)actor.maxSP >= config.AutoUltraSPRatio && flag)
			{
				actor.abilityPlugin.HandleActionTargetDispatch(config.BeginActions, instancedAbility, instancedModifier, actor, null);
				_isUltraMode = true;
			}
			if (_isUltraMode && flag)
			{
				float num = config.CostSPSpeed * Time.deltaTime;
				DelegateUtils.UpdateField(ref actor.SP, (float)actor.SP - num, 0f - num, actor.onSPChanged);
				if ((float)actor.SP / (float)actor.maxSP < config.EndUltarSPRatio)
				{
					actor.abilityPlugin.HandleActionTargetDispatch(config.EndActions, instancedAbility, instancedModifier, actor, null);
					_isUltraMode = false;
				}
			}
		}
	}
}
