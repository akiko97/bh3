using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Basic/SharedVariable")]
	public class SharedFloatLogic : Action
	{
		public enum LogicType
		{
			Add = 0,
			Multiple = 1
		}

		public SharedFloat BaseValue;

		public LogicType Logic;

		public SharedFloat ParamValue;

		public override TaskStatus OnUpdate()
		{
			switch (Logic)
			{
			case LogicType.Add:
				BaseValue.Value += ParamValue.Value;
				break;
			case LogicType.Multiple:
				BaseValue.Value *= ParamValue.Value;
				break;
			}
			return TaskStatus.Success;
		}
	}
}
