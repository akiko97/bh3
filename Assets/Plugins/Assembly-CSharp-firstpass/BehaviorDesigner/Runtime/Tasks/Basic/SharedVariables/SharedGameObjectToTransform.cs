using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskDescription("Gets the Transform from the GameObject. Returns Success.")]
	[TaskCategory("Basic/SharedVariable")]
	public class SharedGameObjectToTransform : Action
	{
		[Tooltip("The GameObject to get the Transform of")]
		public SharedGameObject sharedGameObject;

		[Tooltip("The Transform to set")]
		[RequiredField]
		public SharedTransform sharedTransform;

		public override TaskStatus OnUpdate()
		{
			if (sharedGameObject.Value == null)
			{
				return TaskStatus.Failure;
			}
			sharedTransform.Value = sharedGameObject.Value.GetComponent<Transform>();
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			sharedGameObject = null;
			sharedTransform = null;
		}
	}
}
