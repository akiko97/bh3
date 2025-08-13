using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	[TaskCategory("Basic/SharedVariable")]
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	public class CompareSharedLayerMask : Conditional
	{
		[BehaviorDesigner.Runtime.Tasks.Tooltip("The first varible to compare")]
		public SharedLayerMask variable;

		[BehaviorDesigner.Runtime.Tasks.Tooltip("The variable to compare to")]
		public SharedLayerMask compareTo;

		public override TaskStatus OnUpdate()
		{
			return ((int)variable.Value != (int)compareTo.Value) ? TaskStatus.Failure : TaskStatus.Success;
		}

		public override void OnReset()
		{
			variable = default(LayerMask);
			compareTo = default(LayerMask);
		}
	}
}
