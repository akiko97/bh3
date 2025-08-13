namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
	public class FloatCompareGroup
	{
		public enum CompareType
		{
			MoreThan = 0,
			LessThan = 1,
			Equal = 2
		}

		public SharedFloat variable;

		public SharedFloat compareTo;

		public CompareType compareType;

		public bool Result()
		{
			switch (compareType)
			{
			case CompareType.Equal:
				return (variable.Value == compareTo.Value) ? true : false;
			case CompareType.LessThan:
				return (variable.Value < compareTo.Value) ? true : false;
			case CompareType.MoreThan:
				return (variable.Value > compareTo.Value) ? true : false;
			default:
				return false;
			}
		}
	}
}
