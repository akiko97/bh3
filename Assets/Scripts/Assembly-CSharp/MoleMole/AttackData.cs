using MoleMole.Config;

namespace MoleMole
{
	public class AttackData : AttackResult
	{
		public enum AttackDataStep
		{
			AttackerResolved = 0,
			AttackeeResolved = 1,
			FinalResolved = 2
		}

		public float attackerAniDamageRatio;

		public EntityClass attackerClass;

		public EntityNature attackerNature;

		public ushort attackerCategory;

		public float attackerCritChance;

		public float attackerCritDamageRatio;

		public int attackerLevel;

		public float attackerShieldDamageRatio;

		public float attackerShieldDamageDelta;

		public float attackerAttackPercentage;

		public float attackerAttackValue;

		public float addedAttackRatio;

		public float addedDamageRatio;

		public float attackerAddedAttackValue;

		public float attackerAddedAllDamageReduceRatio;

		public float attackerNormalDamage;

		public float attackerNormalDamagePercentage;

		public float addedAttackerNormalDamageRatio;

		public float attackerFireDamage;

		public float attackerFireDamagePercentage;

		public float addedAttackerFireDamageRatio;

		public float attackerThunderDamage;

		public float attackerThunderDamagePercentage;

		public float addedAttackerThunderDamageRatio;

		public float attackerIceDamage;

		public float attackerIceDamagePercentage;

		public float addedAttackerIceDamageRatio;

		public float attackerAlienDamage;

		public float attackerAlienDamagePercentage;

		public float addedAttackerAlienDamageRatio;

		public float attackeeAniDefenceRatio;

		public EntityNature attackeeNature;

		public EntityClass attackeeClass;

		public float attackeeAddedDamageTakeRatio;

		public int noBreakFrameHaltAdd;

		public float natureDamageRatio;

		public AttackDataStep resolveStep;

		public bool IsFinalResolved()
		{
			return resolveStep == AttackDataStep.FinalResolved;
		}

		public void Reject(RejectType rejectType)
		{
			resolveStep = AttackDataStep.FinalResolved;
			rejectState = rejectType;
		}

		public AttackData Clone()
		{
			return (AttackData)MemberwiseClone();
		}
	}
}
