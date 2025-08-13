using BehaviorDesigner.Runtime;

namespace MoleMole
{
	public class SharedAttackType : SharedVariable<AttackType>
	{
		public override string ToString()
		{
			return mValue.ToString();
		}

		public static implicit operator SharedAttackType(AttackType value)
		{
			SharedAttackType sharedAttackType = new SharedAttackType();
			sharedAttackType.mValue = value;
			return sharedAttackType;
		}
	}
}
