using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskDescription("Sets the SharedQuaternion variable to the specified object. Returns Success.")]
	[TaskCategory("Basic/SharedVariable")]
	public class SetSharedQuaternion : Action
	{
		[Tooltip("The value to set the SharedQuaternion to")]
		public SharedQuaternion targetValue;

		[Tooltip("The SharedQuaternion to set")]
		[RequiredField]
		public SharedQuaternion targetVariable;

		public override TaskStatus OnUpdate()
		{
			targetVariable.Value = targetValue.Value;
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetValue = Quaternion.identity;
			targetVariable = Quaternion.identity;
		}
	}
}
