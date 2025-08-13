namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	public class LessThanSharedFloatOR : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedFloat variable01;

		[Tooltip("The first variable to compare")]
		public SharedFloat variable02;

		[Tooltip("The first variable to compare")]
		public SharedFloat variable03;

		[Tooltip("The variable to compare to")]
		public SharedFloat compareTo;

		public override TaskStatus OnUpdate()
		{
			if (variable01.Value < compareTo.Value || variable02.Value < compareTo.Value || variable03.Value < compareTo.Value)
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		public override void OnReset()
		{
			variable01 = 0f;
			variable02 = 0f;
			variable03 = 0f;
			compareTo = 0f;
		}
	}
}
