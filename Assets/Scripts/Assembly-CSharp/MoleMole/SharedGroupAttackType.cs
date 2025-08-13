using BehaviorDesigner.Runtime;
using MoleMole.Config;

namespace MoleMole
{
	public class SharedGroupAttackType : SharedVariable<ConfigGroupAIMinionOld.AttackType>
	{
		public override string ToString()
		{
			return mValue.ToString();
		}

		public static implicit operator SharedGroupAttackType(ConfigGroupAIMinionOld.AttackType value)
		{
			SharedGroupAttackType sharedGroupAttackType = new SharedGroupAttackType();
			sharedGroupAttackType.mValue = value;
			return sharedGroupAttackType;
		}
	}
}
