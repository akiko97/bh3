using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	public class CompareLogicGroup
	{
		public enum LogicType
		{
			And = 0,
			Or = 1
		}

		public LogicType logicType;

		public List<FloatCompareGroup> compareGroupList;

		public bool Result()
		{
			if (logicType == LogicType.And)
			{
				for (int i = 0; i < compareGroupList.Count; i++)
				{
					if (!compareGroupList[i].Result())
					{
						return false;
					}
				}
				return true;
			}
			if (logicType == LogicType.Or)
			{
				for (int j = 0; j < compareGroupList.Count; j++)
				{
					if (compareGroupList[j].Result())
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}
	}
}
