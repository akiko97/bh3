namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	[TaskCategory("Basic/SharedVariable")]
	public class CompareSharedObject : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedObject variable;

		[Tooltip("The variable to compare to")]
		public SharedObject compareTo;

		public override TaskStatus OnUpdate()
		{
			if (variable.Value == null && compareTo.Value != null)
			{
				return TaskStatus.Failure;
			}
			if (variable.Value == null && compareTo.Value == null)
			{
				return TaskStatus.Success;
			}
			return (!variable.Value.Equals(compareTo.Value)) ? TaskStatus.Failure : TaskStatus.Success;
		}

		public override void OnReset()
		{
			variable = null;
			compareTo = null;
		}
	}
}
