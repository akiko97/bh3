namespace MoleMole.Config
{
	public class ConfigEntityAttackEffect : IndexedConfig<ConfigEntityAttackEffect>
	{
		public string EffectPattern;

		public string SwitchName;

		public bool MuteAttackEffect;

		public AttackEffectTriggerAt AttackEffectTriggerPos;

		public override int CompareTo(ConfigEntityAttackEffect other)
		{
			if (other == null)
			{
				return 1;
			}
			int num = IndexedConfig.Compare(EffectPattern, other.EffectPattern);
			if (num != 0)
			{
				return num;
			}
			num = IndexedConfig.Compare(SwitchName, other.SwitchName);
			if (num != 0)
			{
				return num;
			}
			num = MuteAttackEffect.CompareTo(other.MuteAttackEffect);
			if (num != 0)
			{
				return num;
			}
			return AttackEffectTriggerPos.CompareTo(other.AttackEffectTriggerPos);
		}

		public override int ContentHash()
		{
			int lastHash = 0;
			HashUtils.ContentHashOnto(EffectPattern, ref lastHash);
			HashUtils.ContentHashOnto(SwitchName, ref lastHash);
			HashUtils.ContentHashOnto(MuteAttackEffect, ref lastHash);
			HashUtils.ContentHashOnto((int)AttackEffectTriggerPos, ref lastHash);
			return 0;
		}
	}
}
