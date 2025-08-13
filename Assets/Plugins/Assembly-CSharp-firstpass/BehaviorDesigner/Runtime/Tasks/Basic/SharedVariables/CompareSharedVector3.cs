using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	[TaskCategory("Basic/SharedVariable")]
	public class CompareSharedVector3 : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedVector3 variable;

		[Tooltip("The variable to compare to")]
		public SharedVector3 compareTo;

		public override TaskStatus OnUpdate()
		{
			return (!variable.Value.Equals(compareTo.Value)) ? TaskStatus.Failure : TaskStatus.Success;
		}

		public override void OnReset()
		{
			variable = Vector3.zero;
			compareTo = Vector3.zero;
		}
	}
}
