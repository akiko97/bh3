namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	public class CompareSharedString : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedString variable;

		[Tooltip("The variable to compare to")]
		public SharedString compareTo;

		public override TaskStatus OnUpdate()
		{
			if (!string.IsNullOrEmpty(variable.Value))
			{
				return (!variable.Value.Equals(compareTo.Value)) ? TaskStatus.Failure : TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		public override void OnReset()
		{
			variable = string.Empty;
			compareTo = string.Empty;
		}
	}
}
