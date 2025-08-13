using System;
using System.Collections.Generic;

namespace MoleMole.Config
{
	public class ConfigAbilityModifier
	{
		public enum ModifierTimeScale
		{
			Owner = 0,
			Caster = 1,
			Level = 2
		}

		public enum ModifierStacking
		{
			Unique = 0,
			Refresh = 1,
			Prolong = 2,
			Multiple = 3
		}

		public ModifierTimeScale TimeScale;

		public ModifierStacking Stacking;

		public bool IsBuff;

		public bool IsDebuff;

		[NonSerialized]
		public string ModifierName;

		public bool IsUnique;

		public DynamicFloat Duration = new DynamicFloat
		{
			fixedValue = 0f
		};

		public DynamicFloat ThinkInterval = new DynamicFloat
		{
			fixedValue = 0f
		};

		public ConfigAbilityMixin[] ModifierMixins = new ConfigAbilityMixin[0];

		public SortedList<string, DynamicFloat> Properties = new SortedList<string, DynamicFloat>();

		public SortedList<string, DynamicFloat> EntityProperties = new SortedList<string, DynamicFloat>();

		public AbilityState State;

		public ConfigAbilityStateOption StateOption;

		public bool MuteStateDisplayEffect;

		public bool ApplyAttackerWitchTimeRatio;

		public ConfigAbilityAction[] OnAdded = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnRemoved = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnBeingHit = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnBeingHitResolved = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnAttackLanded = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnThinkInterval = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnEvadeStart = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnEvadeSuccess = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnDefendStart = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnDefendSuccess = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnMonsterCreated = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnAvatarCreated = ConfigAbilityAction.EMPTY;

		public ConfigAbilityAction[] OnKilled = ConfigAbilityAction.EMPTY;

		[NonSerialized]
		public int localID = -1;
	}
}
