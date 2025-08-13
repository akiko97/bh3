using System;
using System.Collections.Generic;

namespace MoleMole.Config
{
	[GeneratePartialHash]
	public class ConfigPropObject : IHashable, IEntityConfig, IOnLoaded
	{
		public enum E_RetreatType
		{
			Common = 0,
			Spike = 1
		}

		public static Dictionary<string, ConfigAbilityPropertyEntry> EMTPY_PROPERTIES = new Dictionary<string, ConfigAbilityPropertyEntry>();

		public string Name;

		public string PrefabPath;

		public ConfigPropObjectCommonArguments CommonArguments = ConfigPropObjectCommonArguments.EMPTY;

		public ConfigPropArguments PropArguments;

		public ConfigEntityAttackEffect BeHitEffect;

		public ConfigEntityAbilityEntry[] Abilities = ConfigEntityAbilityEntry.EMPTY;

		public Dictionary<string, ConfigPropAnimEvent> AnimEvents;

		[NonSerialized]
		public ConfigCommonEntity CommonConfig;

		ConfigEntityAnimEvent IEntityConfig.TryGetAnimEvent(string animEventID)
		{
			ConfigPropAnimEvent value;
			AnimEvents.TryGetValue(animEventID, out value);
			return value;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(Name, ref lastHash);
			HashUtils.ContentHashOnto(PrefabPath, ref lastHash);
			if (CommonArguments != null)
			{
				HashUtils.ContentHashOnto((int)CommonArguments.Nature, ref lastHash);
				HashUtils.ContentHashOnto((int)CommonArguments.Class, ref lastHash);
				HashUtils.ContentHashOnto((int)CommonArguments.RoleName, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.DefaultAnimEventPredicate, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.CreatePosYOffset, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.CreateCollisionRadius, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.CreateCollisionHeight, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.CollisionLevel, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.CollisionRadius, ref lastHash);
				if (CommonArguments.PreloadEffectPatternGroups != null)
				{
					string[] preloadEffectPatternGroups = CommonArguments.PreloadEffectPatternGroups;
					foreach (string value in preloadEffectPatternGroups)
					{
						HashUtils.ContentHashOnto(value, ref lastHash);
					}
				}
				if (CommonArguments.RequestSoundBankNames != null)
				{
					string[] requestSoundBankNames = CommonArguments.RequestSoundBankNames;
					foreach (string value2 in requestSoundBankNames)
					{
						HashUtils.ContentHashOnto(value2, ref lastHash);
					}
				}
				if (CommonArguments.EffectPredicates != null)
				{
					string[] effectPredicates = CommonArguments.EffectPredicates;
					foreach (string value3 in effectPredicates)
					{
						HashUtils.ContentHashOnto(value3, ref lastHash);
					}
				}
				HashUtils.ContentHashOnto(CommonArguments.HasLowPrefab, ref lastHash);
				HashUtils.ContentHashOnto(CommonArguments.CameraMinAngleRatio, ref lastHash);
			}
			if (PropArguments != null)
			{
				HashUtils.ContentHashOnto(PropArguments.IsTargetable, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.IsTriggerField, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.OnlyReduceHPByOne, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.HP, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.Attack, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.UseOwnerAttack, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.EffectDuration, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.CD, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.Length, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.TriggerHitWhenFieldEnter, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.AnimEventIDForHit, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.Acceleration, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.MaxMoveSpeed, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.WarningRange, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.EscapeRange, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.DieWhenFieldEnter, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.OnKillEffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.OnDestroyEffectPattern, ref lastHash);
				HashUtils.ContentHashOnto((int)PropArguments.RetreatType, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.Duration, ref lastHash);
				HashUtils.ContentHashOnto(PropArguments.CanAffectMonsters, ref lastHash);
			}
			if (BeHitEffect != null)
			{
				HashUtils.ContentHashOnto(BeHitEffect.EffectPattern, ref lastHash);
				HashUtils.ContentHashOnto(BeHitEffect.SwitchName, ref lastHash);
				HashUtils.ContentHashOnto(BeHitEffect.MuteAttackEffect, ref lastHash);
				HashUtils.ContentHashOnto((int)BeHitEffect.AttackEffectTriggerPos, ref lastHash);
			}
			if (Abilities != null)
			{
				ConfigEntityAbilityEntry[] abilities = Abilities;
				foreach (ConfigEntityAbilityEntry configEntityAbilityEntry in abilities)
				{
					HashUtils.ContentHashOnto(configEntityAbilityEntry.AbilityName, ref lastHash);
					HashUtils.ContentHashOnto(configEntityAbilityEntry.AbilityOverride, ref lastHash);
				}
			}
			if (AnimEvents == null)
			{
				return;
			}
			foreach (KeyValuePair<string, ConfigPropAnimEvent> animEvent in AnimEvents)
			{
				HashUtils.ContentHashOnto(animEvent.Key, ref lastHash);
				HashUtils.ContentHashOnto(animEvent.Value.Predicate, ref lastHash);
				HashUtils.ContentHashOnto(animEvent.Value.Predicate2, ref lastHash);
				if (animEvent.Value.AttackPattern != null)
				{
				}
				if (animEvent.Value.AttackProperty != null)
				{
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.DamagePercentage, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.AddedDamageValue, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.NormalDamage, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.NormalDamagePercentage, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.FireDamage, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.FireDamagePercentage, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.ThunderDamage, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.ThunderDamagePercentage, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.IceDamage, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.IceDamagePercentage, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.AlienDamage, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.AlienDamagePercentage, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.AniDamageRatio, ref lastHash);
					HashUtils.ContentHashOnto((int)animEvent.Value.AttackProperty.HitType, ref lastHash);
					HashUtils.ContentHashOnto((int)animEvent.Value.AttackProperty.HitEffect, ref lastHash);
					HashUtils.ContentHashOnto((int)animEvent.Value.AttackProperty.HitEffectAux, ref lastHash);
					HashUtils.ContentHashOnto((int)animEvent.Value.AttackProperty.KillEffect, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.FrameHalt, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.RetreatVelocity, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.IsAnimEventAttack, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.IsInComboCount, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.SPRecover, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.WitchTimeRatio, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.NoTriggerEvadeAndDefend, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackProperty.NoBreakFrameHaltAdd, ref lastHash);
					HashUtils.ContentHashOnto((int)animEvent.Value.AttackProperty.AttackTargetting, ref lastHash);
					if (animEvent.Value.AttackProperty.CategoryTag != null)
					{
						AttackResult.AttackCategoryTag[] categoryTag = animEvent.Value.AttackProperty.CategoryTag;
						foreach (AttackResult.AttackCategoryTag value4 in categoryTag)
						{
							HashUtils.ContentHashOnto((int)value4, ref lastHash);
						}
					}
				}
				if (animEvent.Value.CameraShake != null)
				{
					HashUtils.ContentHashOnto(animEvent.Value.CameraShake.ShakeOnNotHit, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.CameraShake.ShakeRange, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.CameraShake.ShakeTime, ref lastHash);
					if (animEvent.Value.CameraShake.ShakeAngle.HasValue)
					{
						HashUtils.ContentHashOnto(animEvent.Value.CameraShake.ShakeAngle.Value, ref lastHash);
					}
					HashUtils.ContentHashOnto(animEvent.Value.CameraShake.ShakeStepFrame, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.CameraShake.ClearPreviousShake, ref lastHash);
				}
				if (animEvent.Value.AttackEffect != null)
				{
					HashUtils.ContentHashOnto(animEvent.Value.AttackEffect.EffectPattern, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackEffect.SwitchName, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.AttackEffect.MuteAttackEffect, ref lastHash);
					HashUtils.ContentHashOnto((int)animEvent.Value.AttackEffect.AttackEffectTriggerPos, ref lastHash);
				}
				if (animEvent.Value.TriggerAbility != null)
				{
					HashUtils.ContentHashOnto(animEvent.Value.TriggerAbility.ID, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.TriggerAbility.Name, ref lastHash);
				}
				if (animEvent.Value.TriggerEffectPattern != null)
				{
					HashUtils.ContentHashOnto(animEvent.Value.TriggerEffectPattern.EffectPattern, ref lastHash);
				}
				if (animEvent.Value.TimeSlow != null)
				{
					HashUtils.ContentHashOnto(animEvent.Value.TimeSlow.Force, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.TimeSlow.Duration, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.TimeSlow.SlowRatio, ref lastHash);
				}
				if (animEvent.Value.TriggerTintCamera != null)
				{
					HashUtils.ContentHashOnto(animEvent.Value.TriggerTintCamera.RenderDataName, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.TriggerTintCamera.Duration, ref lastHash);
					HashUtils.ContentHashOnto(animEvent.Value.TriggerTintCamera.TransitDuration, ref lastHash);
				}
			}
		}

		public void OnLoaded()
		{
			CommonConfig = new ConfigCommonEntity
			{
				EntityProperties = EMTPY_PROPERTIES,
				CommonArguments = CommonArguments
			};
		}
	}
}
