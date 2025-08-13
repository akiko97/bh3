namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	[TaskCategory("Basic/SharedVariable")]
	public class CompareSharedBool : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedBool variable;

		[Tooltip("The variable to compare to")]
		public SharedBool compareTo;

		public override TaskStatus OnUpdate()
		{
			return (!variable.Value.Equals(compareTo.Value)) ? TaskStatus.Failure : TaskStatus.Success;
		}

		public override void OnReset()
		{
			variable = false;
			compareTo = false;
		}
	}
}
