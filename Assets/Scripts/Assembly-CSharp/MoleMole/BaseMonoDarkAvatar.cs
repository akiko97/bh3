using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseMonoDarkAvatar : BaseMonoMonster
	{
		[AnimationCallback]
		protected void RunOnRightFoot()
		{
		}

		[AnimationCallback]
		protected void RunOnLeftFoot()
		{
		}

		[AnimationCallback]
		protected void ClearSkillTriggers()
		{
		}

		[AnimationCallback]
		private void TriggerCameraPullFar(float time)
		{
		}

		[AnimationCallback]
		private void TriggerCameraPushNear(float time)
		{
		}

		[AnimationCallback]
		private void TriggerCameraPullFurther(float time)
		{
		}

		[AnimationCallback]
		private void TimeSlowTrigger(float time)
		{
		}

		[AnimationCallback]
		public void SetLevelComboTimerState(int state)
		{
		}

		[AnimationCallback]
		private void TriggerTintCamera(float duration)
		{
		}

		public override void SetTrigger(string name)
		{
			name = ConvertTrigger(name);
			if (name != null)
			{
				base.SetTrigger(name);
			}
		}

		public override void ResetTrigger(string name)
		{
			name = ConvertTrigger(name);
			if (name != null)
			{
				base.ResetTrigger(name);
			}
		}

		private string ConvertTrigger(string name)
		{
			switch (name)
			{
			case "HitTrigger":
				name = "TriggerHit";
				break;
			case "ThrowBlowTrigger":
				name = "TriggerKnockDown";
				break;
			case "ATKTrigger":
				name = "TriggerAttack";
				break;
			case "ThrowDownTrigger":
			case "ThrowTrigger":
				name = null;
				break;
			}
			return name;
		}

		public virtual void TriggerAppear()
		{
			SetTrigger("TriggerSwitchIn");
		}

		protected override void PostInit()
		{
			base.PostInit();
			object value;
			config.DynamicArguments.TryGetValue("MuteAttachWeapon", out value);
			if (value == null || !(bool)value)
			{
				int weaponID = (int)config.DynamicArguments["WeaponID"];
				string avatarType = (string)config.DynamicArguments["AvatarType"];
				WeaponData.WeaponModelAndEffectAttach(weaponID, avatarType, this);
			}
			object value2;
			config.DynamicArguments.TryGetValue("MuteDarkAvatarShader", out value2);
			if (value2 == null || !(bool)value2)
			{
				ConfigBaseRenderingData renderingDataConfig = RenderingData.GetRenderingDataConfig<ConfigBaseRenderingData>("Basic_DarkAvatar");
				for (int i = 0; i < _matListForSpecailState.Count; i++)
				{
					RenderingData.ApplyRenderingData(renderingDataConfig, _matListForSpecailState[i].material);
				}
			}
			TriggerAppear();
		}

		public override void BeHit(int frameHalt, AttackResult.AnimatorHitEffect hitEffect, AttackResult.AnimatorHitEffectAux hitEffectAux, KillEffect killEffect, BeHitEffect beHitEffect, float aniDamageRatio, Vector3 hitForward, float retreatVelocity)
		{
			if (hitEffect == AttackResult.AnimatorHitEffect.ThrowBlow || hitEffect == AttackResult.AnimatorHitEffect.ThrowDown)
			{
				hitEffect = AttackResult.AnimatorHitEffect.KnockDown;
				retreatVelocity *= 0.1f;
			}
			base.BeHit(frameHalt, hitEffect, hitEffectAux, killEffect, beHitEffect, aniDamageRatio, hitForward, retreatVelocity);
		}

		public override void SetEliteShader()
		{
		}
	}
}
