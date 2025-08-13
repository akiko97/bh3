using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class Mono_BOSS_040 : BaseMonoDarkAvatar
	{
		public override void Awake()
		{
			base.Awake();
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
