namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Sets the SharedGameObject variable to the specified object. Returns Success.")]
	public class SetSharedGameObject : Action
	{
		[Tooltip("The value to set the SharedGameObject to. If null the variable will be set to the current GameObject")]
		public SharedGameObject targetValue;

		[Tooltip("The SharedGameObject to set")]
		[RequiredField]
		public SharedGameObject targetVariable;

		public override TaskStatus OnUpdate()
		{
			targetVariable.Value = ((!(targetValue.Value != null)) ? gameObject : targetValue.Value);
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetValue = null;
			targetVariable = null;
		}
	}
}
