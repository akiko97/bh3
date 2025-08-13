using System;
using UnityEngine;

namespace MoleMole
{
	public class MonoKiana : BaseMonoAvatar
	{
		public Renderer LeftWeapon;

		public Renderer RightWeapon;

		[Header("When exiting from these skill IDs set weapon visible will be reset.")]
		public string[] ResetWeaponShownSkillIDs;

		[Header("Normalized time in Born to stop the born shader effect")]
		public float BornFXNormalizedStopTime = 0.5f;

		private bool _isAppearFXing;

		protected override void PostInit()
		{
			base.PostInit();
			if (ResetWeaponShownSkillIDs.Length > 0)
			{
				onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(onCurrentSkillIDChanged, new Action<string, string>(ResetWeaponBySkillID));
			}
			onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Combine(onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(CheckBornAnimatorState));
		}

		private void SetWeaponVisible(int show)
		{
			bool flag = show != 0;
			if (LeftWeapon != null)
			{
				LeftWeapon.enabled = flag;
			}
			if (RightWeapon != null)
			{
				RightWeapon.enabled = flag;
			}
		}

		private void ResetWeaponBySkillID(string from, string to)
		{
			if (Miscs.ArrayContains(ResetWeaponShownSkillIDs, from))
			{
				SetWeaponVisible(1);
			}
		}

		protected override void Update()
		{
			base.Update();
			if (_isAppearFXing && GetCurrentNormalizedTime() > BornFXNormalizedStopTime)
			{
				_isAppearFXing = false;
				SetShaderData(E_ShaderData.AppearKiana, false);
				onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Remove(onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(CheckBornAnimatorState));
			}
		}

		private void CheckBornAnimatorState(AnimatorStateInfo fromState, AnimatorStateInfo toState)
		{
			if (toState.tagHash == AvatarData.AVATAR_APPEAR_TAG)
			{
				SetShaderData(E_ShaderData.AppearKiana, true);
				_isAppearFXing = true;
			}
			else if (fromState.tagHash == AvatarData.AVATAR_APPEAR_TAG && _isAppearFXing)
			{
				_isAppearFXing = false;
				SetShaderData(E_ShaderData.AppearKiana, false);
				onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>)Delegate.Remove(onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(CheckBornAnimatorState));
			}
		}
	}
}
