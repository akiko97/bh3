namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	public class MultiCompareSharedFloat : Conditional
	{
		public LogicGroup logicGroup;

		public override TaskStatus OnUpdate()
		{
			if (logicGroup.Result())
			{
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}

		public override void OnReset()
		{
		}
	}
}
