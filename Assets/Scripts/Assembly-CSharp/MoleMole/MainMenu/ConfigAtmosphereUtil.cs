using UnityEngine;

namespace MoleMole.MainMenu
{
	public static class ConfigAtmosphereUtil
	{
		public static int ChooseRandomly(int[] rates)
		{
			int num = 0;
			foreach (int num2 in rates)
			{
				num += num2;
			}
			int num3 = Random.Range(0, num);
			num = 0;
			for (int j = 0; j < rates.Length; j++)
			{
				num += rates[j];
				if (num > num3)
				{
					return j;
				}
			}
			return Random.Range(0, rates.Length);
		}
	}
}
