namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskDescription("Sets the SharedInt variable to the specified object. Returns Success.")]
	[TaskCategory("Basic/SharedVariable")]
	public class SetSharedInt : Action
	{
		[Tooltip("The value to set the SharedInt to")]
		public SharedInt targetValue;

		[Tooltip("The SharedInt to set")]
		[RequiredField]
		public SharedInt targetVariable;

		public override TaskStatus OnUpdate()
		{
			targetVariable.Value = targetValue.Value;
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetValue = 0;
			targetVariable = 0;
		}
	}
}
