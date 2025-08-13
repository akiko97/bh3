namespace MoleMole.Config
{
	public class ConfigEquipmentSkillEntry
	{
		public int EquipmentSkillID;

		public string AbilityName;

		public string AbilityOverride = "Default";

		public bool IsActiveSkill;

		public bool IsInstantTrigger;

		public int SkillCD;

		public float SPCost;

		public float SPNeed;

		public int MaxChargesCount;

		public string ParamSpecial1;

		public ParamMethod ParamMethod1;

		public string ParamSpecial2;

		public ParamMethod ParamMethod2;

		public string ParamSpecial3;

		public ParamMethod ParamMethod3;
	}
}
