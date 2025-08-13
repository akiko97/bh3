using System;
using UnityEngine;

namespace MoleMole
{
	public class MonoMei : BaseMonoAvatar
	{
		[Header("Normalized time in Born to stop the born shader effect")]
		public float BornFXNormalizedTimeStop = 0.5f;

		private bool _isAppearFXing;

		public override void TriggerSkill(int skillNum)
		{
			if (skillNum == 1)
			{
				SetLocomotionBool("EvadeBackward", !GetActiveControlData().hasSteer);
			}
			base.TriggerSkill(skillNum);
		}

		protected override void PostInit()
		{
			base.PostInit();
			onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Combine(onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(CheckBornAnimatorState));
		}

		protected override void Update()
		{
			base.Update();
			if (_isAppearFXing && GetCurrentNormalizedTime() > BornFXNormalizedTimeStop)
			{
				_isAppearFXing = false;
				SetShaderData(E_ShaderData.AppearMei, false);
				onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Remove(onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(CheckBornAnimatorState));
			}
		}

		private void CheckBornAnimatorState(AnimatorStateInfo fromState, AnimatorStateInfo toState)
		{
			if (toState.tagHash == AvatarData.AVATAR_APPEAR_TAG)
			{
				SetShaderData(E_ShaderData.AppearMei, true);
				_isAppearFXing = true;
			}
			else if (fromState.tagHash == AvatarData.AVATAR_APPEAR_TAG && _isAppearFXing)
			{
				_isAppearFXing = false;
				SetShaderData(E_ShaderData.AppearMei, false);
				onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Remove(onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(CheckBornAnimatorState));
			}
		}

		public void SetBodyVisible(int show)
		{
			bool flag = show != 0;
			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].enabled = flag;
			}
		}
	}
}
