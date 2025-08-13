namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class BronyaArsenalMixin : ConfigAbilityMixin, IHashable
	{
		public string PositionsEffect;

		public float[] DelayList;

		public string CannonEffects;

		public string ChargeEffects;

		public string HintEffects;

		public string ShootEffects;

		public string ExplodeEffects;

		public float ChargeTime = 3f;

		public float FireTime = 3f;

		public float ClearTime = 3f;

		public float FireIntervial = 0.1f;

		public string ExplodeAnimEventID;

		public float ExplodeRadius = 2f;

		public string ShakeAnimEventID;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(PositionsEffect, ref lastHash);
			if (DelayList != null)
			{
				float[] delayList = DelayList;
				foreach (float value in delayList)
				{
					HashUtils.ContentHashOnto(value, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(CannonEffects, ref lastHash);
			HashUtils.ContentHashOnto(ChargeEffects, ref lastHash);
			HashUtils.ContentHashOnto(HintEffects, ref lastHash);
			HashUtils.ContentHashOnto(ShootEffects, ref lastHash);
			HashUtils.ContentHashOnto(ExplodeEffects, ref lastHash);
			HashUtils.ContentHashOnto(ChargeTime, ref lastHash);
			HashUtils.ContentHashOnto(FireTime, ref lastHash);
			HashUtils.ContentHashOnto(ClearTime, ref lastHash);
			HashUtils.ContentHashOnto(FireIntervial, ref lastHash);
			HashUtils.ContentHashOnto(ExplodeAnimEventID, ref lastHash);
			HashUtils.ContentHashOnto(ExplodeRadius, ref lastHash);
			HashUtils.ContentHashOnto(ShakeAnimEventID, ref lastHash);
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityBronyaArsenalMixin(instancedAbility, instancedModifier, this);
		}
	}
}
