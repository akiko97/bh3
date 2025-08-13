namespace MoleMole.Config
{
	public class ConfigGroupAIMinionParamOld
	{
		public enum ParamType
		{
			Float = 0,
			Int = 1,
			Bool = 2,
			AttackType = 3,
			MoveType = 4,
			None = 5
		}

		public string Name;

		public ParamType Type = ParamType.None;

		public float FloatValue;

		public int IntValue;

		public bool BoolValue;

		public ConfigGroupAIMinionOld.AttackType AttackTypeValue;

		public ConfigGroupAIMinionOld.MoveType MoveTypeValue;

		public bool Interruption;

		public bool TriggerAttack;

		public float TriggerAttackDelay;
	}
}
