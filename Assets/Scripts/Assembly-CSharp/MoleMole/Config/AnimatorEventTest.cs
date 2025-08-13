using System;
using System.Text;

namespace MoleMole.Config
{
	public class AnimatorEventTest : AnimatorEvent
	{
		public int IntValue;

		public int[] IntArray;

		public float FloatValue;

		public float[] FloatArray;

		public string StringValue;

		public string[] StringArray;

		public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
		{
		}

		public string GetArrayText(Array array)
		{
			StringBuilder stringBuilder = new StringBuilder("[");
			if (array != null)
			{
				int i = 0;
				for (int length = array.Length; i < length; i++)
				{
					if (i != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(array.GetValue(i).ToString());
				}
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}
