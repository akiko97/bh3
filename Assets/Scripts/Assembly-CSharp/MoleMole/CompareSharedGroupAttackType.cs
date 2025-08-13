using BehaviorDesigner.Runtime.Tasks;
using MoleMole.Config;

namespace MoleMole
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	public class CompareSharedGroupAttackType : Conditional
	{
		[Tooltip("The first varible to compare")]
		public SharedGroupAttackType variable;

		[Tooltip("The variable to compare to")]
		public SharedGroupAttackType compareTo;

		public override TaskStatus OnUpdate()
		{
			return (variable.Value != compareTo.Value) ? TaskStatus.Failure : TaskStatus.Success;
		}

		public override void OnReset()
		{
			variable = ConfigGroupAIMinionOld.AttackType.Free;
			compareTo = ConfigGroupAIMinionOld.AttackType.Free;
		}
	}
}
