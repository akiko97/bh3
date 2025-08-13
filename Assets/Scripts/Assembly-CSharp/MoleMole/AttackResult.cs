using System;
using System.Text;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class AttackResult
	{
		public enum ElementType
		{
			Plain = 0,
			Fire = 1,
			Thunder = 2,
			Ice = 3,
			Alien = 4
		}

		[Flags]
		public enum AttackCategoryTag
		{
			None = 0,
			Normal = 1,
			Branch = 2,
			Charge = 4,
			Ultra = 8,
			Weapon = 0x10,
			SwitchIn = 0x20,
			QTE = 0x40,
			Evade = 0x80,
			Defend = 0x100
		}

		public enum ActorHitType
		{
			Melee = 0,
			Ranged = 1,
			Ailment = 2
		}

		[Flags]
		public enum ActorHitFlag
		{
			None = 0,
			ShieldBroken = 1,
			Count = 1
		}

		public enum ActorHitLevel
		{
			Normal = 0,
			Mute = 1,
			Critical = 2
		}

		public enum AnimatorHitEffect
		{
			Mute = 0,
			Light = 1,
			Normal = 2,
			FaceAttacker = 3,
			ThrowUp = 4,
			ThrowDown = 5,
			KnockDown = 6,
			ThrowUpBlow = 7,
			ThrowBlow = 8,
			ThrowAirBlow = 9
		}

		public enum AnimatorHitEffectAux
		{
			Normal = 0,
			HitLeft = 1,
			HitRight = 2,
			HitUpper = 3,
			HitCentered = 4,
			HitBack = 5,
			HitOnLeft = 6,
			HitOnRight = 7
		}

		public enum HitEffectPattern
		{
			Mute = 0,
			OnlyAttack = 1,
			OnlyBeHit = 2,
			Normal = 3
		}

		public class HitCollsion
		{
			public Vector3 hitDir;

			public Vector3 hitPoint;
		}

		public enum RejectType
		{
			Normal = 0,
			RejectAll = 1,
			RejectButShowAttackEffect = 2
		}

		public float damage;

		public float plainDamage;

		public float fireDamage;

		public float thunderDamage;

		public float iceDamage;

		public float alienDamage;

		public float aniDamageRatio;

		public float retreatVelocity;

		public int frameHalt;

		public bool isAnimEventAttack = true;

		public bool isInComboCount = true;

		public AttackCategoryTag attackCategoryTag;

		public ActorHitType hitType;

		public ActorHitFlag hitFlag;

		public ActorHitLevel hitLevel;

		public AnimatorHitEffect hitEffect;

		public AnimatorHitEffectAux hitEffectAux;

		public HitEffectPattern hitEffectPattern;

		public KillEffect killEffect;

		public HitCollsion hitCollision;

		public ConfigEntityAttackEffect attackEffectPattern;

		public ConfigEntityAttackEffect beHitEffectPattern;

		public ConfigEntityCameraShake attackCameraShake;

		public bool isFromBullet;

		public bool noTriggerEvadeAndDefend;

		public RejectType rejectState;

		public bool rejected
		{
			get
			{
				return rejectState > RejectType.Normal;
			}
		}

		public void AddHitFlag(ActorHitFlag flag)
		{
			hitFlag |= flag;
		}

		public bool ContainHitFlag(ActorHitFlag flag)
		{
			return (hitFlag & flag) != 0;
		}

		public float GetTotalDamage()
		{
			return damage + plainDamage + fireDamage + thunderDamage + iceDamage + alienDamage;
		}

		public float GetElementalDamage()
		{
			return plainDamage + fireDamage + thunderDamage + iceDamage + alienDamage;
		}

		public string GetDebugOutput()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("\n--");
			stringBuilder.AppendFormat("伤害: {0}\n", damage);
			if (plainDamage > 0f)
			{
				stringBuilder.AppendFormat("元素伤害(无属性): {0}\n", plainDamage);
			}
			if (fireDamage > 0f)
			{
				stringBuilder.AppendFormat("元素伤害(火属性): {0}\n", fireDamage);
			}
			if (thunderDamage > 0f)
			{
				stringBuilder.AppendFormat("元素伤害(雷属性): {0}\n", thunderDamage);
			}
			if (iceDamage > 0f)
			{
				stringBuilder.AppendFormat("元素伤害(冰属性): {0}\n", iceDamage);
			}
			if (alienDamage > 0f)
			{
				stringBuilder.AppendFormat("元素伤害(异能属性): {0}\n", alienDamage);
			}
			if (GetTotalDamage() > damage)
			{
				stringBuilder.AppendFormat("总伤害: {0}\n", GetTotalDamage());
			}
			if (retreatVelocity > 0f)
			{
				stringBuilder.AppendFormat("击退: {0}\n", retreatVelocity);
			}
			if (hitLevel != ActorHitLevel.Normal)
			{
				stringBuilder.AppendFormat("攻击等级: {0}\n", hitLevel);
			}
			if (hitEffect != AnimatorHitEffect.Normal)
			{
				stringBuilder.AppendFormat("特殊 HitEffect: {0}\n", hitEffect);
			}
			if (hitEffectAux != AnimatorHitEffectAux.Normal)
			{
				stringBuilder.AppendFormat("特殊 HitEffectAux: {0}\n", hitEffectAux);
			}
			if (killEffect != KillEffect.KillNow)
			{
				stringBuilder.AppendFormat("特殊 KillEffect: {0}\n", killEffect);
			}
			if (!isAnimEventAttack)
			{
				stringBuilder.AppendFormat("!(内在攻击)\n");
			}
			if (!isInComboCount)
			{
				stringBuilder.AppendFormat("!(不算Combo)\n");
			}
			if (rejected)
			{
				stringBuilder.AppendFormat("!(无效)\n");
			}
			stringBuilder.AppendLine("--\n");
			return stringBuilder.ToString();
		}
	}
}
