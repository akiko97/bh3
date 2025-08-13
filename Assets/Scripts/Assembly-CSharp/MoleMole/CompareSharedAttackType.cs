using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	public class CompareSharedAttackType : Conditional
	{
		[Tooltip("The first varible to compare")]
		public SharedAttackType variable;

		[Tooltip("The variable to compare to")]
		public SharedAttackType compareTo;

		public override TaskStatus OnUpdate()
		{
			return (variable.Value != compareTo.Value) ? TaskStatus.Failure : TaskStatus.Success;
		}

		public override void OnReset()
		{
			variable = AttackType.NearAttack;
			compareTo = AttackType.NearAttack;
		}
	}
}
