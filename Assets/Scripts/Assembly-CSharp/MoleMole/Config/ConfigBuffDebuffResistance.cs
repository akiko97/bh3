using System.Collections.Generic;

namespace MoleMole.Config
{
	public class ConfigBuffDebuffResistance
	{
		public List<AbilityState> ResistanceBuffDebuffs = new List<AbilityState>();

		public float ResistanceRatio;

		public float DurationRatio;

		public ConfigBuffDebuffResistance(AbilityState[] abilityStates, float resistRatio, float durationRatio)
		{
			for (int i = 0; i < abilityStates.Length; i++)
			{
				ResistanceBuffDebuffs.Add(abilityStates[i]);
			}
			ResistanceRatio = resistRatio;
			DurationRatio = durationRatio;
		}
	}
}
