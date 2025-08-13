using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Basic/SharedVariable")]
	public class SetSharedLayerMask : Action
	{
		[BehaviorDesigner.Runtime.Tasks.Tooltip("The value to set the LayerMask to")]
		public SharedLayerMask targetValue;

		[RequiredField]
		[BehaviorDesigner.Runtime.Tasks.Tooltip("The LayerMask to set")]
		public SharedLayerMask targetVariable;

		public override TaskStatus OnUpdate()
		{
			targetVariable.Value = targetValue.Value;
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetValue = default(LayerMask);
			targetVariable = default(LayerMask);
		}
	}
}
