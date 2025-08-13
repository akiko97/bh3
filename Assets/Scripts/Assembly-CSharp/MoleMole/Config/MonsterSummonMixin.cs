using System.Collections.Generic;

namespace MoleMole.Config
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class MonsterSummonMixin : ConfigAbilityMixin, IHashable
	{
		public MixinSummonItem[] SummonMonsters;

		public MixinEffect SummonEffect;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			if (SummonMonsters != null)
			{
				MixinSummonItem[] summonMonsters = SummonMonsters;
				foreach (MixinSummonItem mixinSummonItem in summonMonsters)
				{
					if (mixinSummonItem.MonsterName != null)
					{
						HashUtils.ContentHashOnto(mixinSummonItem.MonsterName.isDynamic, ref lastHash);
						HashUtils.ContentHashOnto(mixinSummonItem.MonsterName.fixedValue, ref lastHash);
						HashUtils.ContentHashOnto(mixinSummonItem.MonsterName.dynamicKey, ref lastHash);
					}
					if (mixinSummonItem.TypeName != null)
					{
						HashUtils.ContentHashOnto(mixinSummonItem.TypeName.isDynamic, ref lastHash);
						HashUtils.ContentHashOnto(mixinSummonItem.TypeName.fixedValue, ref lastHash);
						HashUtils.ContentHashOnto(mixinSummonItem.TypeName.dynamicKey, ref lastHash);
					}
					HashUtils.ContentHashOnto(mixinSummonItem.BaseOnTarget, ref lastHash);
					HashUtils.ContentHashOnto(mixinSummonItem.Distance, ref lastHash);
					HashUtils.ContentHashOnto(mixinSummonItem.Angle, ref lastHash);
					HashUtils.ContentHashOnto(mixinSummonItem.EffectDelay, ref lastHash);
					HashUtils.ContentHashOnto(mixinSummonItem.SummonDelay, ref lastHash);
					HashUtils.ContentHashOnto(mixinSummonItem.UseCoffinAnim, ref lastHash);
					HashUtils.ContentHashOnto(mixinSummonItem.CoffinIndex, ref lastHash);
					if (mixinSummonItem.Abilities == null)
					{
						continue;
					}
					foreach (KeyValuePair<string, ConfigEntityAbilityEntry> ability in mixinSummonItem.Abilities)
					{
						HashUtils.ContentHashOnto(ability.Key, ref lastHash);
						HashUtils.ContentHashOnto(ability.Value.AbilityName, ref lastHash);
						HashUtils.ContentHashOnto(ability.Value.AbilityOverride, ref lastHash);
					}
				}
			}
			if (SummonEffect != null)
			{
				HashUtils.ContentHashOnto(SummonEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(SummonEffect.AudioPattern, ref lastHash);
			}
		}

		public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
		{
			return new AbilityMonsterSummonAttack(instancedAbility, instancedModifier, this);
		}
	}
}
