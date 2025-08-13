using BehaviorDesigner.Runtime.Tasks;
using MoleMole.Config;

namespace MoleMole
{
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	[TaskCategory("Basic/SharedVariable")]
	public class CompareSharedGroupMoveType : Conditional
	{
		[Tooltip("The first varible to compare")]
		public SharedGroupMoveType variable;

		[Tooltip("The variable to compare to")]
		public SharedGroupMoveType compareTo;

		public override TaskStatus OnUpdate()
		{
			return (variable.Value != compareTo.Value) ? TaskStatus.Failure : TaskStatus.Success;
		}

		public override void OnReset()
		{
			variable = ConfigGroupAIMinionOld.MoveType.Free;
			compareTo = ConfigGroupAIMinionOld.MoveType.Free;
		}
	}
}
