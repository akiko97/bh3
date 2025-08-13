using System;
using MoleMole.Config;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace MoleMole
{
	public class Mono_BOSS_020 : BaseMonoBoss
	{
		[Header("Wing Trails")]
		public SmoothTrail[] wingTrails;

		[Header("Show Wing Trail Skill IDs")]
		public string[] ShowWingTrailSkillIDs;

		protected override void PostInit()
		{
			base.PostInit();
			onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(onCurrentSkillIDChanged, new Action<string, string>(ShowWingTrailBySkillID));
		}

		private void ShowWingTrailBySkillID(string from, string to)
		{
			if (Miscs.ArrayContains(ShowWingTrailSkillIDs, to))
			{
				for (int i = 0; i < wingTrails.Length; i++)
				{
					wingTrails[i].Emit = true;
				}
			}
			else
			{
				for (int j = 0; j < wingTrails.Length; j++)
				{
					wingTrails[j].Emit = false;
				}
			}
		}

		public override void BeHit(int frameHalt, AttackResult.AnimatorHitEffect hitEffect, AttackResult.AnimatorHitEffectAux hitEffectAux, KillEffect killEffect, BeHitEffect beHitEffect, float aniDamageRatio, Vector3 hitForward, float retreatVelocity)
		{
			if (hitEffect > AttackResult.AnimatorHitEffect.Light)
			{
				SetOverrideSteerFaceDirectionFrame(-hitForward);
			}
			base.BeHit(frameHalt, hitEffect, hitEffectAux, killEffect, beHitEffect, aniDamageRatio, hitForward, retreatVelocity);
		}
	}
}
