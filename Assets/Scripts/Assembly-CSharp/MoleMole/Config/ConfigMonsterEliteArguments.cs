using System;
using UnityEngine;

namespace MoleMole.Config
{
	public class ConfigMonsterEliteArguments
	{
		public float HPRatio = 1f;

		public float DefenseRatio = 1f;

		public float AttackRatio = 1f;

		public float DebuffResistanceRatio;

		public string HexColorElite1 = "#FFCC00B9";

		[NonSerialized]
		public Color EliteColor1;

		public float EliteEmissionScaler1 = 1f;

		public float EliteNormalDisplacement1 = 0.02f;

		public string HexColorElite2 = "#FFD84449";

		[NonSerialized]
		public Color EliteColor2;

		public float EliteEmissionScaler2 = 1f;

		public float EliteNormalDisplacement2 = 0.04f;
	}
}
