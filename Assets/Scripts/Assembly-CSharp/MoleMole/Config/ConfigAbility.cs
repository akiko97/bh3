using System;
using System.Collections.Generic;
using FullInspector;

namespace MoleMole.Config
{
	[GeneratePartialHash]
	public class ConfigAbility : IHashable, IOnLoaded
	{
		private const string DEFAULT_MODIFIER_NAME = "__DEFAULT_MODIFIER";

		public static Dictionary<string, ConfigAbilityModifier> EMPTY_MODIFIERS = new Dictionary<string, ConfigAbilityModifier>();

		public string AbilityName;

		[InspectorNullable]
		public string UseAbilityArgumentAsSpecialKey;

		[InspectorNullable]
		public string SetAbilityArgumentToOverrideMap;

		public ConfigAbilityMixin[] AbilityMixins = ConfigAbilityMixin.EMPTY;

		public ConfigDynamicArguments AbilitySpecials = ConfigDynamicArguments.EMPTY;

		public Dictionary<string, ConfigAbilityModifier> Modifiers = EMPTY_MODIFIERS;

		[InspectorNullable]
		public ConfigAbilityModifier DefaultModifier;

		public ConfigAbilityAction[] OnAdded = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnRemoved = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnAbilityStart = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnKilled = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnDestroy = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnFieldEnter = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnFieldExit = ConfigAbilityAction.EMPTY;

		[NonSerialized]
		public List<BaseActionContainer> InvokeSites;

		[NonSerialized]
		public ConfigAbilityModifier[] ModifierIDMap;

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(AbilityName, ref lastHash);
			HashUtils.ContentHashOnto(UseAbilityArgumentAsSpecialKey, ref lastHash);
			HashUtils.ContentHashOnto(SetAbilityArgumentToOverrideMap, ref lastHash);
			if (AbilityMixins != null)
			{
				ConfigAbilityMixin[] abilityMixins = AbilityMixins;
				foreach (ConfigAbilityMixin configAbilityMixin in abilityMixins)
				{
					if (configAbilityMixin is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityMixin, ref lastHash);
					}
				}
			}
			if (AbilitySpecials != null)
			{
				foreach (KeyValuePair<string, object> abilitySpecial in AbilitySpecials)
				{
					HashUtils.ContentHashOnto(abilitySpecial.Key, ref lastHash);
					HashUtils.ContentHashOntoFallback(abilitySpecial.Value, ref lastHash);
				}
			}
			if (Modifiers != null)
			{
				foreach (KeyValuePair<string, ConfigAbilityModifier> modifier in Modifiers)
				{
					HashUtils.ContentHashOnto(modifier.Key, ref lastHash);
					HashUtils.ContentHashOnto((int)modifier.Value.TimeScale, ref lastHash);
					HashUtils.ContentHashOnto((int)modifier.Value.Stacking, ref lastHash);
					HashUtils.ContentHashOnto(modifier.Value.IsBuff, ref lastHash);
					HashUtils.ContentHashOnto(modifier.Value.IsDebuff, ref lastHash);
					HashUtils.ContentHashOnto(modifier.Value.IsUnique, ref lastHash);
					if (modifier.Value.Duration != null)
					{
						HashUtils.ContentHashOnto(modifier.Value.Duration.isDynamic, ref lastHash);
						HashUtils.ContentHashOnto(modifier.Value.Duration.fixedValue, ref lastHash);
						HashUtils.ContentHashOnto(modifier.Value.Duration.dynamicKey, ref lastHash);
					}
					if (modifier.Value.ThinkInterval != null)
					{
						HashUtils.ContentHashOnto(modifier.Value.ThinkInterval.isDynamic, ref lastHash);
						HashUtils.ContentHashOnto(modifier.Value.ThinkInterval.fixedValue, ref lastHash);
						HashUtils.ContentHashOnto(modifier.Value.ThinkInterval.dynamicKey, ref lastHash);
					}
					if (modifier.Value.ModifierMixins != null)
					{
						ConfigAbilityMixin[] modifierMixins = modifier.Value.ModifierMixins;
						foreach (ConfigAbilityMixin configAbilityMixin2 in modifierMixins)
						{
							if (configAbilityMixin2 is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityMixin2, ref lastHash);
							}
						}
					}
					if (modifier.Value.Properties != null)
					{
						foreach (KeyValuePair<string, DynamicFloat> property in modifier.Value.Properties)
						{
							HashUtils.ContentHashOnto(property.Key, ref lastHash);
							HashUtils.ContentHashOnto(property.Value.isDynamic, ref lastHash);
							HashUtils.ContentHashOnto(property.Value.fixedValue, ref lastHash);
							HashUtils.ContentHashOnto(property.Value.dynamicKey, ref lastHash);
						}
					}
					if (modifier.Value.EntityProperties != null)
					{
						foreach (KeyValuePair<string, DynamicFloat> entityProperty in modifier.Value.EntityProperties)
						{
							HashUtils.ContentHashOnto(entityProperty.Key, ref lastHash);
							HashUtils.ContentHashOnto(entityProperty.Value.isDynamic, ref lastHash);
							HashUtils.ContentHashOnto(entityProperty.Value.fixedValue, ref lastHash);
							HashUtils.ContentHashOnto(entityProperty.Value.dynamicKey, ref lastHash);
						}
					}
					HashUtils.ContentHashOnto((int)modifier.Value.State, ref lastHash);
					if (modifier.Value.StateOption != null)
					{
					}
					HashUtils.ContentHashOnto(modifier.Value.MuteStateDisplayEffect, ref lastHash);
					HashUtils.ContentHashOnto(modifier.Value.ApplyAttackerWitchTimeRatio, ref lastHash);
					if (modifier.Value.OnAdded != null)
					{
						ConfigAbilityAction[] onAdded = modifier.Value.OnAdded;
						foreach (ConfigAbilityAction configAbilityAction in onAdded)
						{
							if (configAbilityAction is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityAction, ref lastHash);
							}
						}
					}
					if (modifier.Value.OnRemoved != null)
					{
						ConfigAbilityAction[] onRemoved = modifier.Value.OnRemoved;
						foreach (ConfigAbilityAction configAbilityAction2 in onRemoved)
						{
							if (configAbilityAction2 is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityAction2, ref lastHash);
							}
						}
					}
					if (modifier.Value.OnBeingHit != null)
					{
						ConfigAbilityAction[] onBeingHit = modifier.Value.OnBeingHit;
						foreach (ConfigAbilityAction configAbilityAction3 in onBeingHit)
						{
							if (configAbilityAction3 is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityAction3, ref lastHash);
							}
						}
					}
					if (modifier.Value.OnBeingHitResolved != null)
					{
						ConfigAbilityAction[] onBeingHitResolved = modifier.Value.OnBeingHitResolved;
						foreach (ConfigAbilityAction configAbilityAction4 in onBeingHitResolved)
						{
							if (configAbilityAction4 is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityAction4, ref lastHash);
							}
						}
					}
					if (modifier.Value.OnAttackLanded != null)
					{
						ConfigAbilityAction[] onAttackLanded = modifier.Value.OnAttackLanded;
						foreach (ConfigAbilityAction configAbilityAction5 in onAttackLanded)
						{
							if (configAbilityAction5 is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityAction5, ref lastHash);
							}
						}
					}
					if (modifier.Value.OnThinkInterval != null)
					{
						ConfigAbilityAction[] onThinkInterval = modifier.Value.OnThinkInterval;
						foreach (ConfigAbilityAction configAbilityAction6 in onThinkInterval)
						{
							if (configAbilityAction6 is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityAction6, ref lastHash);
							}
						}
					}
					if (modifier.Value.OnEvadeStart != null)
					{
						ConfigAbilityAction[] onEvadeStart = modifier.Value.OnEvadeStart;
						foreach (ConfigAbilityAction configAbilityAction7 in onEvadeStart)
						{
							if (configAbilityAction7 is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityAction7, ref lastHash);
							}
						}
					}
					if (modifier.Value.OnEvadeSuccess != null)
					{
						ConfigAbilityAction[] onEvadeSuccess = modifier.Value.OnEvadeSuccess;
						foreach (ConfigAbilityAction configAbilityAction8 in onEvadeSuccess)
						{
							if (configAbilityAction8 is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityAction8, ref lastHash);
							}
						}
					}
					if (modifier.Value.OnDefendStart != null)
					{
						ConfigAbilityAction[] onDefendStart = modifier.Value.OnDefendStart;
						foreach (ConfigAbilityAction configAbilityAction9 in onDefendStart)
						{
							if (configAbilityAction9 is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityAction9, ref lastHash);
							}
						}
					}
					if (modifier.Value.OnDefendSuccess != null)
					{
						ConfigAbilityAction[] onDefendSuccess = modifier.Value.OnDefendSuccess;
						foreach (ConfigAbilityAction configAbilityAction10 in onDefendSuccess)
						{
							if (configAbilityAction10 is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityAction10, ref lastHash);
							}
						}
					}
					if (modifier.Value.OnMonsterCreated != null)
					{
						ConfigAbilityAction[] onMonsterCreated = modifier.Value.OnMonsterCreated;
						foreach (ConfigAbilityAction configAbilityAction11 in onMonsterCreated)
						{
							if (configAbilityAction11 is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityAction11, ref lastHash);
							}
						}
					}
					if (modifier.Value.OnAvatarCreated != null)
					{
						ConfigAbilityAction[] onAvatarCreated = modifier.Value.OnAvatarCreated;
						foreach (ConfigAbilityAction configAbilityAction12 in onAvatarCreated)
						{
							if (configAbilityAction12 is IHashable)
							{
								HashUtils.ContentHashOnto((IHashable)configAbilityAction12, ref lastHash);
							}
						}
					}
					if (modifier.Value.OnKilled == null)
					{
						continue;
					}
					ConfigAbilityAction[] onKilled = modifier.Value.OnKilled;
					foreach (ConfigAbilityAction configAbilityAction13 in onKilled)
					{
						if (configAbilityAction13 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction13, ref lastHash);
						}
					}
				}
			}
			if (DefaultModifier != null)
			{
				HashUtils.ContentHashOnto((int)DefaultModifier.TimeScale, ref lastHash);
				HashUtils.ContentHashOnto((int)DefaultModifier.Stacking, ref lastHash);
				HashUtils.ContentHashOnto(DefaultModifier.IsBuff, ref lastHash);
				HashUtils.ContentHashOnto(DefaultModifier.IsDebuff, ref lastHash);
				HashUtils.ContentHashOnto(DefaultModifier.IsUnique, ref lastHash);
				if (DefaultModifier.Duration != null)
				{
					HashUtils.ContentHashOnto(DefaultModifier.Duration.isDynamic, ref lastHash);
					HashUtils.ContentHashOnto(DefaultModifier.Duration.fixedValue, ref lastHash);
					HashUtils.ContentHashOnto(DefaultModifier.Duration.dynamicKey, ref lastHash);
				}
				if (DefaultModifier.ThinkInterval != null)
				{
					HashUtils.ContentHashOnto(DefaultModifier.ThinkInterval.isDynamic, ref lastHash);
					HashUtils.ContentHashOnto(DefaultModifier.ThinkInterval.fixedValue, ref lastHash);
					HashUtils.ContentHashOnto(DefaultModifier.ThinkInterval.dynamicKey, ref lastHash);
				}
				if (DefaultModifier.ModifierMixins != null)
				{
					ConfigAbilityMixin[] modifierMixins2 = DefaultModifier.ModifierMixins;
					foreach (ConfigAbilityMixin configAbilityMixin3 in modifierMixins2)
					{
						if (configAbilityMixin3 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityMixin3, ref lastHash);
						}
					}
				}
				if (DefaultModifier.Properties != null)
				{
					foreach (KeyValuePair<string, DynamicFloat> property2 in DefaultModifier.Properties)
					{
						HashUtils.ContentHashOnto(property2.Key, ref lastHash);
						HashUtils.ContentHashOnto(property2.Value.isDynamic, ref lastHash);
						HashUtils.ContentHashOnto(property2.Value.fixedValue, ref lastHash);
						HashUtils.ContentHashOnto(property2.Value.dynamicKey, ref lastHash);
					}
				}
				if (DefaultModifier.EntityProperties != null)
				{
					foreach (KeyValuePair<string, DynamicFloat> entityProperty2 in DefaultModifier.EntityProperties)
					{
						HashUtils.ContentHashOnto(entityProperty2.Key, ref lastHash);
						HashUtils.ContentHashOnto(entityProperty2.Value.isDynamic, ref lastHash);
						HashUtils.ContentHashOnto(entityProperty2.Value.fixedValue, ref lastHash);
						HashUtils.ContentHashOnto(entityProperty2.Value.dynamicKey, ref lastHash);
					}
				}
				HashUtils.ContentHashOnto((int)DefaultModifier.State, ref lastHash);
				if (DefaultModifier.StateOption != null)
				{
				}
				HashUtils.ContentHashOnto(DefaultModifier.MuteStateDisplayEffect, ref lastHash);
				HashUtils.ContentHashOnto(DefaultModifier.ApplyAttackerWitchTimeRatio, ref lastHash);
				if (DefaultModifier.OnAdded != null)
				{
					ConfigAbilityAction[] onAdded2 = DefaultModifier.OnAdded;
					foreach (ConfigAbilityAction configAbilityAction14 in onAdded2)
					{
						if (configAbilityAction14 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction14, ref lastHash);
						}
					}
				}
				if (DefaultModifier.OnRemoved != null)
				{
					ConfigAbilityAction[] onRemoved2 = DefaultModifier.OnRemoved;
					foreach (ConfigAbilityAction configAbilityAction15 in onRemoved2)
					{
						if (configAbilityAction15 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction15, ref lastHash);
						}
					}
				}
				if (DefaultModifier.OnBeingHit != null)
				{
					ConfigAbilityAction[] onBeingHit2 = DefaultModifier.OnBeingHit;
					foreach (ConfigAbilityAction configAbilityAction16 in onBeingHit2)
					{
						if (configAbilityAction16 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction16, ref lastHash);
						}
					}
				}
				if (DefaultModifier.OnBeingHitResolved != null)
				{
					ConfigAbilityAction[] onBeingHitResolved2 = DefaultModifier.OnBeingHitResolved;
					foreach (ConfigAbilityAction configAbilityAction17 in onBeingHitResolved2)
					{
						if (configAbilityAction17 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction17, ref lastHash);
						}
					}
				}
				if (DefaultModifier.OnAttackLanded != null)
				{
					ConfigAbilityAction[] onAttackLanded2 = DefaultModifier.OnAttackLanded;
					foreach (ConfigAbilityAction configAbilityAction18 in onAttackLanded2)
					{
						if (configAbilityAction18 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction18, ref lastHash);
						}
					}
				}
				if (DefaultModifier.OnThinkInterval != null)
				{
					ConfigAbilityAction[] onThinkInterval2 = DefaultModifier.OnThinkInterval;
					foreach (ConfigAbilityAction configAbilityAction19 in onThinkInterval2)
					{
						if (configAbilityAction19 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction19, ref lastHash);
						}
					}
				}
				if (DefaultModifier.OnEvadeStart != null)
				{
					ConfigAbilityAction[] onEvadeStart2 = DefaultModifier.OnEvadeStart;
					foreach (ConfigAbilityAction configAbilityAction20 in onEvadeStart2)
					{
						if (configAbilityAction20 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction20, ref lastHash);
						}
					}
				}
				if (DefaultModifier.OnEvadeSuccess != null)
				{
					ConfigAbilityAction[] onEvadeSuccess2 = DefaultModifier.OnEvadeSuccess;
					foreach (ConfigAbilityAction configAbilityAction21 in onEvadeSuccess2)
					{
						if (configAbilityAction21 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction21, ref lastHash);
						}
					}
				}
				if (DefaultModifier.OnDefendStart != null)
				{
					ConfigAbilityAction[] onDefendStart2 = DefaultModifier.OnDefendStart;
					foreach (ConfigAbilityAction configAbilityAction22 in onDefendStart2)
					{
						if (configAbilityAction22 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction22, ref lastHash);
						}
					}
				}
				if (DefaultModifier.OnDefendSuccess != null)
				{
					ConfigAbilityAction[] onDefendSuccess2 = DefaultModifier.OnDefendSuccess;
					foreach (ConfigAbilityAction configAbilityAction23 in onDefendSuccess2)
					{
						if (configAbilityAction23 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction23, ref lastHash);
						}
					}
				}
				if (DefaultModifier.OnMonsterCreated != null)
				{
					ConfigAbilityAction[] onMonsterCreated2 = DefaultModifier.OnMonsterCreated;
					foreach (ConfigAbilityAction configAbilityAction24 in onMonsterCreated2)
					{
						if (configAbilityAction24 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction24, ref lastHash);
						}
					}
				}
				if (DefaultModifier.OnAvatarCreated != null)
				{
					ConfigAbilityAction[] onAvatarCreated2 = DefaultModifier.OnAvatarCreated;
					foreach (ConfigAbilityAction configAbilityAction25 in onAvatarCreated2)
					{
						if (configAbilityAction25 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction25, ref lastHash);
						}
					}
				}
				if (DefaultModifier.OnKilled != null)
				{
					ConfigAbilityAction[] onKilled2 = DefaultModifier.OnKilled;
					foreach (ConfigAbilityAction configAbilityAction26 in onKilled2)
					{
						if (configAbilityAction26 is IHashable)
						{
							HashUtils.ContentHashOnto((IHashable)configAbilityAction26, ref lastHash);
						}
					}
				}
			}
			if (OnAdded != null)
			{
				ConfigAbilityAction[] onAdded3 = OnAdded;
				foreach (ConfigAbilityAction configAbilityAction27 in onAdded3)
				{
					if (configAbilityAction27 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction27, ref lastHash);
					}
				}
			}
			if (OnRemoved != null)
			{
				ConfigAbilityAction[] onRemoved3 = OnRemoved;
				foreach (ConfigAbilityAction configAbilityAction28 in onRemoved3)
				{
					if (configAbilityAction28 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction28, ref lastHash);
					}
				}
			}
			if (OnAbilityStart != null)
			{
				ConfigAbilityAction[] onAbilityStart = OnAbilityStart;
				foreach (ConfigAbilityAction configAbilityAction29 in onAbilityStart)
				{
					if (configAbilityAction29 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction29, ref lastHash);
					}
				}
			}
			if (OnKilled != null)
			{
				ConfigAbilityAction[] onKilled3 = OnKilled;
				foreach (ConfigAbilityAction configAbilityAction30 in onKilled3)
				{
					if (configAbilityAction30 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction30, ref lastHash);
					}
				}
			}
			if (OnDestroy != null)
			{
				ConfigAbilityAction[] onDestroy = OnDestroy;
				foreach (ConfigAbilityAction configAbilityAction31 in onDestroy)
				{
					if (configAbilityAction31 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction31, ref lastHash);
					}
				}
			}
			if (OnFieldEnter != null)
			{
				ConfigAbilityAction[] onFieldEnter = OnFieldEnter;
				foreach (ConfigAbilityAction configAbilityAction32 in onFieldEnter)
				{
					if (configAbilityAction32 is IHashable)
					{
						HashUtils.ContentHashOnto((IHashable)configAbilityAction32, ref lastHash);
					}
				}
			}
			if (OnFieldExit == null)
			{
				return;
			}
			ConfigAbilityAction[] onFieldExit = OnFieldExit;
			foreach (ConfigAbilityAction configAbilityAction33 in onFieldExit)
			{
				if (configAbilityAction33 is IHashable)
				{
					HashUtils.ContentHashOnto((IHashable)configAbilityAction33, ref lastHash);
				}
			}
		}

		public void OnLoaded()
		{
			if (AbilitySpecials != ConfigDynamicArguments.EMPTY)
			{
				List<string> list = new List<string>(AbilitySpecials.Keys);
				foreach (string item in list)
				{
					if (AbilitySpecials[item] is int)
					{
						AbilitySpecials[item] = (float)(int)AbilitySpecials[item];
					}
				}
			}
			foreach (KeyValuePair<string, ConfigAbilityModifier> modifier in Modifiers)
			{
				modifier.Value.ModifierName = modifier.Key;
				ConfigAbilityModifier value = modifier.Value;
				if ((AbilityState.Endure | AbilityState.MoveSpeedUp | AbilityState.AttackSpeedUp | AbilityState.PowerUp | AbilityState.Shielded | AbilityState.CritUp | AbilityState.Immune | AbilityState.MaxMoveSpeed | AbilityState.Undamagable).ContainsState(value.State))
				{
					value.IsBuff = true;
				}
				else if ((AbilityState.Bleed | AbilityState.Stun | AbilityState.Paralyze | AbilityState.Burn | AbilityState.Poisoned | AbilityState.Frozen | AbilityState.MoveSpeedDown | AbilityState.AttackSpeedDown | AbilityState.Weak | AbilityState.Fragile | AbilityState.TargetLocked | AbilityState.Tied).ContainsState(value.State))
				{
					value.IsDebuff = true;
				}
				if (value.IsBuff)
				{
					if (value.Stacking == ConfigAbilityModifier.ModifierStacking.Unique)
					{
						value.Stacking = ConfigAbilityModifier.ModifierStacking.Refresh;
					}
				}
				else if (!value.IsDebuff)
				{
				}
				if (value.State != AbilityState.None && value.StateOption != null)
				{
					value.StateOption.ChangeModifierConfig(value);
				}
			}
			if (DefaultModifier != null)
			{
				if (Modifiers == EMPTY_MODIFIERS)
				{
					Modifiers = new Dictionary<string, ConfigAbilityModifier>();
				}
				DefaultModifier.ModifierName = "__DEFAULT_MODIFIER";
				Modifiers.Add("__DEFAULT_MODIFIER", DefaultModifier);
				List<ConfigAbilityAction> list2 = new List<ConfigAbilityAction>(OnAdded);
				list2.Insert(0, new ApplyModifier
				{
					ModifierName = "__DEFAULT_MODIFIER",
					Target = AbilityTargetting.Caster
				});
				OnAdded = list2.ToArray();
				DefaultModifier = null;
			}
			InvokeSites = new List<BaseActionContainer>();
			AddSubActions(OnAdded);
			AddSubActions(OnRemoved);
			AddSubActions(OnAbilityStart);
			AddSubActions(OnKilled);
			AddSubActions(OnDestroy);
			AddSubActions(OnFieldEnter);
			AddSubActions(OnFieldExit);
			AddMixins(AbilityMixins);
			string[] array = new string[Modifiers.Count];
			Modifiers.Keys.CopyTo(array, 0);
			Array.Sort(array);
			ModifierIDMap = new ConfigAbilityModifier[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				ConfigAbilityModifier configAbilityModifier = Modifiers[array[i]];
				configAbilityModifier.localID = i;
				ModifierIDMap[i] = configAbilityModifier;
				AddSubActions(configAbilityModifier.OnAdded);
				AddSubActions(configAbilityModifier.OnRemoved);
				AddSubActions(configAbilityModifier.OnBeingHit);
				AddSubActions(configAbilityModifier.OnBeingHitResolved);
				AddSubActions(configAbilityModifier.OnAttackLanded);
				AddSubActions(configAbilityModifier.OnThinkInterval);
				AddSubActions(configAbilityModifier.OnEvadeStart);
				AddSubActions(configAbilityModifier.OnEvadeSuccess);
				AddSubActions(configAbilityModifier.OnMonsterCreated);
				AddSubActions(configAbilityModifier.OnAvatarCreated);
				AddSubActions(configAbilityModifier.OnKilled);
				AddMixins(configAbilityModifier.ModifierMixins);
			}
		}

		private void AddSubAction(ConfigAbilityAction action)
		{
			InvokeSites.Add(action);
			action.localID = InvokeSites.Count - 1;
		}

		private void AddSubActions(ConfigAbilityAction[] actions)
		{
			foreach (ConfigAbilityAction configAbilityAction in actions)
			{
				AddSubAction(configAbilityAction);
				ConfigAbilityAction[][] allSubActions = configAbilityAction.GetAllSubActions();
				for (int j = 0; j < allSubActions.Length; j++)
				{
					AddSubActions(allSubActions[j]);
				}
			}
		}

		private void AddSubActions(ConfigAbilityAction[][] actions)
		{
			for (int i = 0; i < actions.Length; i++)
			{
				AddSubActions(actions[i]);
			}
		}

		private void AddMixins(ConfigAbilityMixin[] mixins)
		{
			foreach (ConfigAbilityMixin configAbilityMixin in mixins)
			{
				InvokeSites.Add(configAbilityMixin);
				configAbilityMixin.localID = InvokeSites.Count - 1;
				AddSubActions(configAbilityMixin.GetAllSubActions());
			}
		}
	}
}
