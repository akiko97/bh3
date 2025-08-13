namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class AvatarWeaponOverHeatMixin : ConfigAbilityMixin, IHashable
	{
		public float OverHeatMax;

		public float CoolSpeed;

		public int OverHeatLayer = 2;

		public float OverHeatCoolSpeed;

		public float ToMaxCoolSpeedTime;

		public string OverHeatButtonSkillID;

		public string[] SkillIDs;

		public string[] NoCoolSkillIDs;

		public float[] SkillHeatAdds;

		public string[] ContinuousSkillIDs;

		public float[] ContinuousHeatAddSpeed;

		public DynamicFloat ContinuousHeatSpeedRatio = DynamicFloat.ONE;

		public string IgnorePredicate;

		public ConfigAbilityAction[] OverHeatActions = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] CoolDownActions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(OverHeatMax, ref lastHash);
			HashUtils.ContentHashOnto(CoolSpeed, ref lastHash);
			HashUtils.ContentHashOnto(OverHeatLayer, ref lastHash);
			HashUtils.ContentHashOnto(OverHeatCoolSpeed, ref lastHash);
			HashUtils.ContentHashOnto(ToMaxCoolSpeedTime, ref lastHash);
			HashUtils.ContentHashOnto(OverHeatButtonSkillID, ref lastHash);
			if (SkillIDs != null)
			{
				string[] skillIDs = SkillIDs;
				foreach (string value in skillIDs)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			if (NoCoolSkillIDs != null)
			{
				string[] noCoolSkillIDs = NoCoolSkillIDs;
				foreach (string value2 in noCoolSkillIDs)
				{
					HashUtils.ContentHashOnto(value2, ref lastHash);
				}
			}
			if (SkillHeatAdds != null)
			{
				float[] skillHeatAdds = SkillHeatAdds;
				foreach (float value3 in skillHeatAdds)
				{
					HashUtils.ContentHashOnto(value3, ref lastHash);
				}
			}
			if (ContinuousSkillIDs != null)
			{
				string[] continuousSkillIDs = ContinuousSkillIDs;
				foreach (string value4 in continuousSkillIDs)
				{
					HashUtils.ContentHashOnto(value4, ref lastHash);
				}
			}
			if (ContinuousHeatAddSpeed != null)
			{
				float[] continuousHeatAddSpeed = ContinuousHeatAddSpeed;
				foreach (float value5 in continuousHeatAddSpeed)
				{
					HashUtils.ContentHashOnto(value5, ref lastHash);
				}
			}
			if (ContinuousHeatSpeedRatio != null)
			{
				HashUtils.ContentHashOnto(ContinuousHeatSpeedRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ContinuousHeatSpeedRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ContinuousHeatSpeedRatio.dynamicKey, ref lastHash);
			}
			HashUtils.ContentHashOnto(IgnorePredicate, ref lastHash);
			if (OverHeatActions != null)
			{
				ConfigAbilityAction[] overHeatActions = OverHeatActions;
				foreach (ConfigAbilityAction configAbilityAction in overHeatActions)
				{
					if (configAbilityAction is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
					}
				}
			}
			if (CoolDownActions == null)
			{
				return;
			}
			ConfigAbilityAction[] coolDownActions = CoolDownActions;
			foreach (ConfigAbilityAction configAbilityAction2 in coolDownActions)
			{
				if (configAbilityAction2 is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityAvatarWeaponOverHeatMixin(instancedAbility, instancedModifier, this);
		}
	}
}
