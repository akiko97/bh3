using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	[TaskCategory("Basic/SharedVariable")]
	public class CompareSharedRect : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedRect variable;

		[Tooltip("The variable to compare to")]
		public SharedRect compareTo;

		public override TaskStatus OnUpdate()
		{
			return (!variable.Value.Equals(compareTo.Value)) ? TaskStatus.Failure : TaskStatus.Success;
		}

		public override void OnReset()
		{
			variable = default(Rect);
			compareTo = default(Rect);
		}
	}
}
