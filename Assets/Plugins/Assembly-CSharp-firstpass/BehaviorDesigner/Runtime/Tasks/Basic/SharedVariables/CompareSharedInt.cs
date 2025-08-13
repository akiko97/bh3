namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	[TaskCategory("Basic/SharedVariable")]
	public class CompareSharedInt : Conditional
	{
		public enum CompareType
		{
			MoreThan = 0,
			LessThan = 1,
			Equal = 2
		}

		[Tooltip("The first variable to compare")]
		public SharedInt variable;

		public CompareType compareType = CompareType.Equal;

		[Tooltip("The variable to compare to")]
		public SharedInt compareTo;

		public override TaskStatus OnUpdate()
		{
			switch (compareType)
			{
			case CompareType.Equal:
				return (!variable.Value.Equals(compareTo.Value)) ? TaskStatus.Failure : TaskStatus.Success;
			case CompareType.LessThan:
				return (variable.Value >= compareTo.Value) ? TaskStatus.Failure : TaskStatus.Success;
			case CompareType.MoreThan:
				return (variable.Value <= compareTo.Value) ? TaskStatus.Failure : TaskStatus.Success;
			default:
				return TaskStatus.Failure;
			}
		}

		public override void OnReset()
		{
			variable = 0;
			compareTo = 0;
		}
	}
}
