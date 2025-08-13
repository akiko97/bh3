namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class PositionDirectionHitMixin : ConfigAbilityMixin, IHashable
	{
		public string[] AnimEventIDs;

		public DynamicFloat ForwardAngleRangeMax = new DynamicFloat
		{
			fixedValue = 180f
		};

		public DynamicFloat ForwardAngleRangeMin = DynamicFloat.ZERO;

		public DynamicFloat BackDamageRatio = DynamicFloat.ONE;

		public DynamicFloat PosAngleRangeMin = DynamicFloat.ZERO;

		public DynamicFloat PosAngleRangeMax = new DynamicFloat
		{
			fixedValue = 180f
		};

		public DynamicFloat HitRange = DynamicFloat.ZERO;

		public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (AnimEventIDs != null)
			{
				string[] animEventIDs = AnimEventIDs;
				foreach (string value in animEventIDs)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			if (ForwardAngleRangeMax != null)
			{
				HashUtils.ContentHashOnto(ForwardAngleRangeMax.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ForwardAngleRangeMax.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ForwardAngleRangeMax.dynamicKey, ref lastHash);
			}
			if (ForwardAngleRangeMin != null)
			{
				HashUtils.ContentHashOnto(ForwardAngleRangeMin.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(ForwardAngleRangeMin.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(ForwardAngleRangeMin.dynamicKey, ref lastHash);
			}
			if (BackDamageRatio != null)
			{
				HashUtils.ContentHashOnto(BackDamageRatio.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(BackDamageRatio.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(BackDamageRatio.dynamicKey, ref lastHash);
			}
			if (PosAngleRangeMin != null)
			{
				HashUtils.ContentHashOnto(PosAngleRangeMin.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(PosAngleRangeMin.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(PosAngleRangeMin.dynamicKey, ref lastHash);
			}
			if (PosAngleRangeMax != null)
			{
				HashUtils.ContentHashOnto(PosAngleRangeMax.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(PosAngleRangeMax.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(PosAngleRangeMax.dynamicKey, ref lastHash);
			}
			if (HitRange != null)
			{
				HashUtils.ContentHashOnto(HitRange.isDynamic, ref lastHash);
				HashUtils.ContentHashOnto(HitRange.fixedValue, ref lastHash);
				HashUtils.ContentHashOnto(HitRange.dynamicKey, ref lastHash);
			}
			if (Actions == null)
			{
				return;
			}
			ConfigAbilityAction[] actions = Actions;
			foreach (ConfigAbilityAction configAbilityAction in actions)
			{
				if (configAbilityAction is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
				}
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityPositionDirectionHitMixin(instancedAbility, instancedModifier, this);
		}
	}
}
