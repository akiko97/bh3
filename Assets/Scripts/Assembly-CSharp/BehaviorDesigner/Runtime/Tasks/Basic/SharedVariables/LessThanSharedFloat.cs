namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	[TaskCategory("Basic/SharedVariable")]
	public class LessThanSharedFloat : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedFloat variable;

		[Tooltip("The variable to compare to")]
		public SharedFloat compareTo;

		public override TaskStatus OnUpdate()
		{
			return (!(variable.Value < compareTo.Value)) ? TaskStatus.Failure : TaskStatus.Success;
		}

		public override void OnReset()
		{
			variable = 0f;
			compareTo = 0f;
		}
	}
}
