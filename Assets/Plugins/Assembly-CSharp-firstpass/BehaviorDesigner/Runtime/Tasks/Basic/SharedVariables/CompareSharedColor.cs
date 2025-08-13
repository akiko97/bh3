using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	[TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
	[TaskCategory("Basic/SharedVariable")]
	public class CompareSharedColor : Conditional
	{
		[Tooltip("The first variable to compare")]
		public SharedColor variable;

		[Tooltip("The variable to compare to")]
		public SharedColor compareTo;

		public override TaskStatus OnUpdate()
		{
			return (!variable.Value.Equals(compareTo.Value)) ? TaskStatus.Failure : TaskStatus.Success;
		}

		public override void OnReset()
		{
			variable = Color.black;
			compareTo = Color.black;
		}
	}
}
