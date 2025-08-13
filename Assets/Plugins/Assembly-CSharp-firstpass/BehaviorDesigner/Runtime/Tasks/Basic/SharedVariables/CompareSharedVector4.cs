using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	[TaskCategory("Basic/SharedVariable")]
	public class CompareSharedVector4 : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedVector4 variable;

		[Tooltip("The variable to compare to")]
		public SharedVector4 compareTo;

		public override TaskStatus OnUpdate()
		{
			return (!variable.Value.Equals(compareTo.Value)) ? TaskStatus.Failure : TaskStatus.Success;
		}

		public override void OnReset()
		{
			variable = Vector4.zero;
			compareTo = Vector4.zero;
		}
	}
}
