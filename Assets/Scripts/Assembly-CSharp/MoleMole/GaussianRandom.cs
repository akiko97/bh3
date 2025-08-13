using UnityEngine;

namespace MoleMole
{
	public static class GaussianRandom
	{
		private static float spare;

		private static bool isSpareReady;

		public static float Val(float mean, float stdDev)
		{
			if (isSpareReady)
			{
				isSpareReady = false;
				return spare * stdDev + mean;
			}
			float num;
			float num2;
			float num3;
			do
			{
				num = Random.value * 2f - 1f;
				num2 = Random.value * 2f - 1f;
				num3 = num * num + num2 * num2;
			}
			while (num3 >= 1f || num3 == 0f);
			float num4 = Mathf.Sqrt(-2f * Mathf.Log(num3) / num3);
			spare = num2 * num4;
			isSpareReady = true;
			return mean + stdDev * num * num4;
		}
	}
}
