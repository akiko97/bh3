using System;
using System.Text;
using FullInspector;

namespace MoleMole.Config
{
	public class ConfigEntityAttackProperty : IOnLoaded
	{
		public float DamagePercentage;

		public float AddedDamageValue;

		public float NormalDamage;

		public float NormalDamagePercentage;

		public float FireDamage;

		public float FireDamagePercentage;

		public float ThunderDamage;

		public float ThunderDamagePercentage;

		public float IceDamage;

		public float IceDamagePercentage;

		public float AlienDamage;

		public float AlienDamagePercentage;

		public float AniDamageRatio;

		public AttackResult.ActorHitType HitType;

		public AttackResult.AnimatorHitEffect HitEffect = AttackResult.AnimatorHitEffect.Normal;

		public AttackResult.AnimatorHitEffectAux HitEffectAux;

		public KillEffect KillEffect;

		public int FrameHalt;

		public float RetreatVelocity;

		public bool IsAnimEventAttack = true;

		public bool IsInComboCount = true;

		public float SPRecover;

		public float WitchTimeRatio = 1f;

		public bool NoTriggerEvadeAndDefend;

		public int NoBreakFrameHaltAdd = 2;

		public MixinTargetting AttackTargetting = MixinTargetting.Enemy;

		[InspectorNullable]
		public AttackResult.AttackCategoryTag[] CategoryTag;

		[NonSerialized]
		public AttackResult.AttackCategoryTag CategoryTagCombined;

		public void OnLoaded()
		{
			if (CategoryTag != null)
			{
				for (int i = 0; i < CategoryTag.Length; i++)
				{
					CategoryTagCombined |= CategoryTag[i];
				}
			}
		}

		public string GetDebugOutput()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("\n--");
			stringBuilder.AppendFormat("攻击力百分比: {0}\n", DamagePercentage);
			if (AddedDamageValue > 0f)
			{
				stringBuilder.AppendFormat("额外基础伤害: {0}\n", AddedDamageValue);
			}
			if (NormalDamage > 0f)
			{
				stringBuilder.AppendFormat("元素伤害(无属性): {0}\n", NormalDamage);
			}
			if (FireDamage > 0f)
			{
				stringBuilder.AppendFormat("元素伤害(火属性): {0}\n", FireDamage);
			}
			if (ThunderDamage > 0f)
			{
				stringBuilder.AppendFormat("元素伤害(雷属性): {0}\n", ThunderDamage);
			}
			if (AlienDamage > 0f)
			{
				stringBuilder.AppendFormat("元素伤害(异能属性): {0}\n", AlienDamage);
			}
			if (RetreatVelocity > 0f)
			{
				stringBuilder.AppendFormat("击退: {0}\n", RetreatVelocity);
			}
			if (HitEffect != AttackResult.AnimatorHitEffect.Normal)
			{
				stringBuilder.AppendFormat("特殊 HitEffect: {0}\n", HitEffect);
			}
			if (KillEffect != KillEffect.KillNow)
			{
				stringBuilder.AppendFormat("特殊 KillEffect: {0}\n", KillEffect);
			}
			if (!IsAnimEventAttack)
			{
				stringBuilder.AppendFormat("!(内在攻击)\n");
			}
			if (!IsInComboCount)
			{
				stringBuilder.AppendFormat("!(不算Combo)\n");
			}
			stringBuilder.AppendLine("--\n");
			return stringBuilder.ToString();
		}
	}
}
