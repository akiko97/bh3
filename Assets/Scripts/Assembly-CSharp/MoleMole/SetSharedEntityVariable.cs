using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Group")]
	public class SetSharedEntityVariable : Action
	{
		public class SetVariable
		{
			public enum VariableType
			{
				Bool = 0,
				Int = 1,
				Float = 2
			}

			public string variableName;

			public VariableType variableType;

			public bool boolValue;

			public int intValue;

			public float floatValue;
		}

		public SharedEntity targetEntity;

		public List<SetVariable> SetVariableList;

		public override void OnAwake()
		{
			base.OnAwake();
		}

		public override TaskStatus OnUpdate()
		{
			if (targetEntity.Value != null)
			{
				BehaviorTree component = targetEntity.Value.GetComponent<BehaviorTree>();
				for (int i = 0; i < SetVariableList.Count; i++)
				{
					switch (SetVariableList[i].variableType)
					{
					case SetVariable.VariableType.Bool:
						component.SetVariableValue(SetVariableList[i].variableName, SetVariableList[i].boolValue);
						break;
					case SetVariable.VariableType.Int:
						component.SetVariableValue(SetVariableList[i].variableName, SetVariableList[i].intValue);
						break;
					case SetVariable.VariableType.Float:
						component.SetVariableValue(SetVariableList[i].variableName, SetVariableList[i].floatValue);
						break;
					}
				}
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}
