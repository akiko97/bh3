using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public static class DamageModelLogic
	{
		public const int MAX_DEFENCE_PUNISH_LEVLE_DIFFERENCE = 10;

		public const int MIN_DEFENCE_PUNISH_LEVEL_DIFFERENCE = 0;

		public const int MAX_ATTACK_PUNISH_LEVEL_DIFFERENCE = 10;

		public const int MIN_ATTACK_PUNISH_LEVEL_DIFFERENCE = 0;

		private static float[,] DEFAULT_MONSTER_NATURE_DAMAGE_TABLE = new float[4, 4]
		{
			{ 1f, 1f, 1f, 1f },
			{ 1f, 1f, 1.3f, 0.7f },
			{ 1f, 0.7f, 1f, 1.3f },
			{ 1f, 1.3f, 0.7f, 1f }
		};

		private static float[,] DEFAULT_AVATAR_NATURE_DAMAGE_TABLE = new float[4, 4]
		{
			{ 1f, 1f, 1f, 1f },
			{ 1f, 1f, 1f, 1f },
			{ 1f, 1f, 1f, 1f },
			{ 1f, 1f, 1f, 1f }
		};

		private static int[,] DEFAULT_NATURE_CIRCLE = new int[4, 4]
		{
			{ 0, 0, 0, 0 },
			{ 0, 0, 1, -1 },
			{ 0, -1, 0, 1 },
			{ 0, 1, -1, 0 }
		};

		public static AttackData CreateAttackDataFromAttackerAnimEvent(BaseActor from, string animEventID)
		{
			if (from is AvatarActor)
			{
				AvatarActor avatarActor = (AvatarActor)from;
				ConfigAvatarAnimEvent configAvatarAnimEvent = SharedAnimEventData.ResolveAnimEvent(avatarActor.config, animEventID);
				return CreateAttackDataFromAttackProperty(from, configAvatarAnimEvent.AttackProperty, configAvatarAnimEvent.AttackEffect, configAvatarAnimEvent.CameraShake);
			}
			if (from is MonsterActor)
			{
				MonsterActor monsterActor = (MonsterActor)from;
				ConfigMonsterAnimEvent configMonsterAnimEvent = SharedAnimEventData.ResolveAnimEvent(monsterActor.config, animEventID);
				AttackData attackData = CreateAttackDataFromAttackProperty(from, configMonsterAnimEvent.AttackProperty, configMonsterAnimEvent.AttackEffect, configMonsterAnimEvent.CameraShake);
				monsterActor.RefillAttackDataDamagePercentage(animEventID, ref attackData);
				return attackData;
			}
			if (from is PropObjectActor)
			{
				PropObjectActor propObjectActor = (PropObjectActor)from;
				ConfigPropAnimEvent configPropAnimEvent = SharedAnimEventData.ResolveAnimEvent(propObjectActor.config, animEventID);
				return CreateAttackDataFromAttackProperty(from, configPropAnimEvent.AttackProperty, configPropAnimEvent.AttackEffect, configPropAnimEvent.CameraShake);
			}
			return null;
		}

		public static AttackData CreateAttackDataFromAttackProperty(BaseActor from, ConfigEntityAttackProperty attackProperty, ConfigEntityAttackEffect attackEffect, ConfigEntityCameraShake cameraShake)
		{
			AttackData attackData = new AttackData();
			if (from is AvatarActor)
			{
				AvatarActor avatarActor = (AvatarActor)from;
				attackData.attackCategoryTag = attackProperty.CategoryTagCombined;
				attackData.attackerClass = avatarActor.config.CommonArguments.Class;
				attackData.attackerNature = (EntityNature)avatarActor.avatarDataItem.Attribute;
				attackData.attackerCategory = 3;
				attackData.attackerAniDamageRatio = attackProperty.AniDamageRatio;
				attackData.frameHalt = attackProperty.FrameHalt;
				attackData.hitType = attackProperty.HitType;
				attackData.hitEffect = attackProperty.HitEffect;
				attackData.hitEffectAux = attackProperty.HitEffectAux;
				attackData.attackerLevel = avatarActor.level;
				attackData.retreatVelocity = attackProperty.RetreatVelocity;
				attackData.attackerCritChance = ((float)avatarActor.critical + avatarActor.GetProperty("Actor_CriticalDelta")) * (1f + avatarActor.GetProperty("Actor_CriticalRatio")) / (float)(75 + (int)avatarActor.level * 5);
				attackData.attackerCritDamageRatio = 2f;
				attackData.attackerAttackValue = avatarActor.attack;
				attackData.attackerAttackPercentage = attackProperty.DamagePercentage;
				attackData.attackerAddedAttackValue = attackProperty.AddedDamageValue;
				attackData.attackerNormalDamage = attackProperty.NormalDamage;
				attackData.attackerNormalDamagePercentage = attackProperty.NormalDamagePercentage;
				attackData.addedAttackerNormalDamageRatio = avatarActor.GetProperty("Actor_NormalAttackRatio");
				attackData.attackerFireDamage = attackProperty.FireDamage;
				attackData.attackerFireDamagePercentage = attackProperty.FireDamagePercentage;
				attackData.addedAttackerFireDamageRatio = avatarActor.GetProperty("Actor_FireAttackRatio");
				attackData.attackerThunderDamage = attackProperty.ThunderDamage;
				attackData.attackerThunderDamagePercentage = attackProperty.ThunderDamagePercentage;
				attackData.addedAttackerThunderDamageRatio = avatarActor.GetProperty("Actor_ThunderAttackRatio");
				attackData.attackerIceDamage = attackProperty.IceDamage;
				attackData.attackerIceDamagePercentage = attackProperty.IceDamagePercentage;
				attackData.addedAttackerIceDamageRatio = avatarActor.GetProperty("Actor_IceAttackRatio");
				attackData.attackerAlienDamage = attackProperty.AlienDamage;
				attackData.attackerAlienDamagePercentage = attackProperty.AlienDamagePercentage;
				attackData.addedAttackerAlienDamageRatio = avatarActor.GetProperty("Actor_AllienAttackRatio");
				attackData.killEffect = attackProperty.KillEffect;
				attackData.hitEffectPattern = AttackResult.HitEffectPattern.Normal;
				attackData.isInComboCount = attackProperty.IsInComboCount;
				attackData.isAnimEventAttack = attackProperty.IsAnimEventAttack;
				attackData.noBreakFrameHaltAdd = attackProperty.NoBreakFrameHaltAdd;
				attackData.attackEffectPattern = attackEffect;
				if (attackData.attackEffectPattern == null)
				{
					attackData.attackEffectPattern = ((!(attackData.attackerAniDamageRatio >= 0.8f)) ? InLevelData.InLevelMiscData.DefaultAvatarAttackSmallEffect : InLevelData.InLevelMiscData.DefaultAvatarAttackBigEffect);
				}
				if (Singleton<AvatarManager>.Instance.IsLocalAvatar(avatarActor.runtimeID))
				{
					attackData.attackCameraShake = cameraShake;
				}
				attackData.attackerAniDamageRatio += avatarActor.GetProperty("Actor_AniDamageDelta");
				attackData.attackerCritChance += avatarActor.GetProperty("Actor_CriticalChanceDelta");
				attackData.attackerCritDamageRatio += avatarActor.GetProperty("Actor_CriticalDamageRatio");
				attackData.attackerAttackValue = (attackData.attackerAttackValue + avatarActor.GetProperty("Actor_AttackDelta")) * (1f + avatarActor.GetProperty("Actor_AttackRatio"));
				attackData.addedDamageRatio = avatarActor.GetProperty("Actor_AddedDamageRatio");
				attackData.addedAttackRatio = avatarActor.GetProperty("Actor_AddedAttackRatio");
				attackData.attackerShieldDamageRatio = 1f + avatarActor.GetProperty("Actor_ShieldDamageRatio");
				attackData.attackerShieldDamageDelta = avatarActor.GetProperty("Actor_ShieldDamageDelta");
				attackData.retreatVelocity *= 1f + avatarActor.GetProperty("Actor_RetreatRatio");
			}
			else if (from is MonsterActor)
			{
				MonsterActor monsterActor = (MonsterActor)from;
				attackData.attackCategoryTag = attackProperty.CategoryTagCombined;
				attackData.attackerClass = monsterActor.config.CommonArguments.Class;
				attackData.attackerNature = (EntityNature)monsterActor.metaConfig.nature;
				attackData.attackerCategory = 4;
				attackData.attackerAniDamageRatio = attackProperty.AniDamageRatio;
				attackData.frameHalt = attackProperty.FrameHalt;
				attackData.hitType = attackProperty.HitType;
				attackData.hitEffect = attackProperty.HitEffect;
				attackData.hitEffectAux = attackProperty.HitEffectAux;
				attackData.hitEffectPattern = AttackResult.HitEffectPattern.Normal;
				attackData.retreatVelocity = attackProperty.RetreatVelocity;
				attackData.attackerLevel = monsterActor.level;
				attackData.attackerAttackValue = monsterActor.attack;
				attackData.attackerAttackPercentage = attackProperty.DamagePercentage;
				attackData.attackerAddedAttackValue = attackProperty.AddedDamageValue;
				attackData.attackerNormalDamage = attackProperty.NormalDamage;
				attackData.attackerNormalDamagePercentage = attackProperty.NormalDamagePercentage;
				attackData.addedAttackerNormalDamageRatio = monsterActor.GetProperty("Actor_NormalAttackRatio");
				attackData.attackerFireDamage = attackProperty.FireDamage;
				attackData.attackerFireDamagePercentage = attackProperty.FireDamagePercentage;
				attackData.addedAttackerFireDamageRatio = monsterActor.GetProperty("Actor_FireAttackRatio");
				attackData.attackerThunderDamage = attackProperty.ThunderDamage;
				attackData.attackerThunderDamagePercentage = attackProperty.ThunderDamagePercentage;
				attackData.addedAttackerThunderDamageRatio = monsterActor.GetProperty("Actor_ThunderAttackRatio");
				attackData.attackerIceDamage = attackProperty.IceDamage;
				attackData.attackerIceDamagePercentage = attackProperty.IceDamagePercentage;
				attackData.addedAttackerIceDamageRatio = monsterActor.GetProperty("Actor_IceAttackRatio");
				attackData.attackerAlienDamage = attackProperty.AlienDamage;
				attackData.attackerAlienDamagePercentage = attackProperty.AlienDamagePercentage;
				attackData.addedAttackerAlienDamageRatio = monsterActor.GetProperty("Actor_AllienAttackRatio");
				attackData.noTriggerEvadeAndDefend = attackProperty.NoTriggerEvadeAndDefend;
				attackData.attackEffectPattern = attackEffect;
				if (attackData.attackEffectPattern == null)
				{
					attackData.attackEffectPattern = InLevelData.InLevelMiscData.DefaultMonsterAttackEffect;
				}
				attackData.attackCameraShake = cameraShake;
				attackData.isAnimEventAttack = attackProperty.IsAnimEventAttack;
				attackData.attackerAniDamageRatio += monsterActor.GetProperty("Actor_AniDamageDelta");
				attackData.attackerAttackValue = (attackData.attackerAttackValue + monsterActor.GetProperty("Actor_AttackDelta")) * (1f + monsterActor.GetProperty("Actor_AttackRatio"));
				attackData.addedAttackRatio = monsterActor.GetProperty("Actor_AddedAttackRatio");
				attackData.attackerShieldDamageRatio = 1f + monsterActor.GetProperty("Actor_ShieldDamageRatio");
				attackData.attackerShieldDamageDelta = monsterActor.GetProperty("Actor_ShieldDamageDelta");
				attackData.retreatVelocity *= 1f + monsterActor.GetProperty("Actor_RetreatRatio");
			}
			else if (from is BaseAbilityActor)
			{
				BaseAbilityActor baseAbilityActor = (BaseAbilityActor)from;
				attackData.attackCategoryTag = attackProperty.CategoryTagCombined;
				attackData.attackerClass = EntityClass.Default;
				attackData.attackerNature = EntityNature.Pure;
				attackData.attackerCategory = 7;
				attackData.attackerAniDamageRatio = attackProperty.AniDamageRatio;
				attackData.frameHalt = attackProperty.FrameHalt;
				attackData.hitType = attackProperty.HitType;
				attackData.hitEffect = attackProperty.HitEffect;
				attackData.hitEffectAux = attackProperty.HitEffectAux;
				attackData.retreatVelocity = attackProperty.RetreatVelocity;
				attackData.attackerLevel = 0;
				attackData.attackerAttackPercentage = attackProperty.DamagePercentage;
				attackData.attackerAttackValue = baseAbilityActor.attack;
				attackData.attackerAttackPercentage = attackProperty.DamagePercentage;
				attackData.attackerAddedAttackValue = attackProperty.AddedDamageValue;
				attackData.attackerNormalDamage = attackProperty.NormalDamage;
				attackData.attackerNormalDamagePercentage = attackProperty.NormalDamagePercentage;
				attackData.addedAttackerNormalDamageRatio = baseAbilityActor.GetProperty("Actor_NormalAttackRatio");
				attackData.attackerFireDamage = attackProperty.FireDamage;
				attackData.attackerFireDamagePercentage = attackProperty.FireDamagePercentage;
				attackData.addedAttackerFireDamageRatio = baseAbilityActor.GetProperty("Actor_FireAttackRatio");
				attackData.attackerThunderDamage = attackProperty.ThunderDamage;
				attackData.attackerThunderDamagePercentage = attackProperty.ThunderDamagePercentage;
				attackData.addedAttackerThunderDamageRatio = baseAbilityActor.GetProperty("Actor_ThunderAttackRatio");
				attackData.attackerIceDamage = attackProperty.IceDamage;
				attackData.attackerIceDamagePercentage = attackProperty.IceDamagePercentage;
				attackData.addedAttackerIceDamageRatio = baseAbilityActor.GetProperty("Actor_IceAttackRatio");
				attackData.attackerAlienDamage = attackProperty.AlienDamage;
				attackData.attackerAlienDamagePercentage = attackProperty.AlienDamagePercentage;
				attackData.addedAttackerAlienDamageRatio = baseAbilityActor.GetProperty("Actor_AllienAttackRatio");
				attackData.attackEffectPattern = attackEffect;
				if (attackData.attackEffectPattern == null)
				{
					attackData.attackEffectPattern = InLevelData.InLevelMiscData.DefaultMonsterAttackEffect;
				}
				attackData.attackCameraShake = cameraShake;
				attackData.isAnimEventAttack = attackProperty.IsAnimEventAttack;
				attackData.attackerAniDamageRatio += baseAbilityActor.GetProperty("Actor_AniDamageDelta");
				attackData.attackerAttackValue = (attackData.attackerAttackValue + baseAbilityActor.GetProperty("Actor_AttackDelta")) * (1f + baseAbilityActor.GetProperty("Actor_AttackRatio"));
				attackData.addedAttackRatio = baseAbilityActor.GetProperty("Actor_AddedAttackRatio");
				attackData.attackerShieldDamageRatio = 1f + baseAbilityActor.GetProperty("Actor_ShieldDamageRatio");
				attackData.attackerShieldDamageDelta = baseAbilityActor.GetProperty("Actor_ShieldDamageDelta");
				attackData.retreatVelocity *= 1f + baseAbilityActor.GetProperty("Actor_RetreatRatio");
			}
			attackData.resolveStep = AttackData.AttackDataStep.AttackerResolved;
			return attackData;
		}

		public static void ResolveAttackDataByAttackee(BaseActor to, AttackData attackData)
		{
			if (attackData.rejected)
			{
				return;
			}
			if (to is AvatarActor)
			{
				AvatarActor avatarActor = (AvatarActor)to;
				bool flag = Singleton<LevelScoreManager>.Instance.IsAllowLevelPunish();
				int levelDifference = Mathf.Clamp(attackData.attackerLevel - Singleton<PlayerModule>.Instance.playerData.teamLevel, 0, 10);
				if (attackData.attackerCategory == 4 && flag)
				{
					attackData.attackerAniDamageRatio *= 1f + AvatarDefencePunishMetaDataReader.GetAvatarDefencePunishMetaDataByKey(levelDifference).AttackRatioIncrease;
				}
				attackData.attackeeAniDefenceRatio = GetAnimDefenceRatio(avatarActor);
				attackData.damage = (attackData.attackerAttackValue * attackData.attackerAttackPercentage + attackData.attackerAddedAttackValue) * (1f + attackData.addedAttackRatio) * (1f + attackData.addedDamageRatio);
				float defence = ((float)avatarActor.defense + avatarActor.GetProperty("Actor_DefenceDelta")) * (1f + avatarActor.GetProperty("Actor_DefenceRatio"));
				float defenceRatio = GetDefenceRatio(defence, attackData.attackerLevel);
				float num = (1f - defenceRatio) * avatarActor.GetProperty("Actor_DamageReduceRatio");
				attackData.attackeeAddedDamageTakeRatio += avatarActor.GetProperty("Actor_DamageTakeRatio");
				attackData.damage = attackData.damage * num * (1f + attackData.attackeeAddedDamageTakeRatio);
				attackData.plainDamage = (attackData.attackerNormalDamage + attackData.attackerAttackValue * attackData.attackerNormalDamagePercentage) * (1f + attackData.addedAttackerNormalDamageRatio) * (1f + attackData.addedDamageRatio) * avatarActor.GetProperty("Actor_ResistAllElementAttackRatio") * avatarActor.GetProperty("Actor_ResistNormalAttackRatio") * (1f + avatarActor.GetProperty("Actor_NormalAttackTakeRatio"));
				attackData.fireDamage = (attackData.attackerFireDamage + attackData.attackerAttackValue * attackData.attackerFireDamagePercentage) * (1f + attackData.addedAttackerFireDamageRatio) * (1f + attackData.addedDamageRatio) * avatarActor.GetProperty("Actor_ResistAllElementAttackRatio") * avatarActor.GetProperty("Actor_ResistFireAttackRatio") * (1f + avatarActor.GetProperty("Actor_FireAttackTakeRatio"));
				attackData.thunderDamage = (attackData.attackerThunderDamage + attackData.attackerAttackValue * attackData.attackerThunderDamagePercentage) * (1f + attackData.addedAttackerThunderDamageRatio) * (1f + attackData.addedDamageRatio) * avatarActor.GetProperty("Actor_ResistAllElementAttackRatio") * avatarActor.GetProperty("Actor_ResistThunderAttackRatio") * (1f + avatarActor.GetProperty("Actor_ThunderAttackTakeRatio"));
				attackData.iceDamage = (attackData.attackerIceDamage + attackData.attackerAttackValue * attackData.attackerIceDamagePercentage) * (1f + attackData.addedAttackerIceDamageRatio) * (1f + attackData.addedDamageRatio) * avatarActor.GetProperty("Actor_ResistAllElementAttackRatio") * avatarActor.GetProperty("Actor_ResistIceAttackRatio") * (1f + avatarActor.GetProperty("Actor_IceAttackTakeRatio"));
				attackData.alienDamage = (attackData.attackerAlienDamage + attackData.attackerAttackValue * attackData.attackerAlienDamagePercentage) * (1f + attackData.addedAttackerAlienDamageRatio) * (1f + attackData.addedDamageRatio) * avatarActor.GetProperty("Actor_ResistAllElementAttackRatio") * avatarActor.GetProperty("Actor_ResistAllienAttackRatio") * (1f + avatarActor.GetProperty("Actor_AllienAttackTakeRatio"));
				attackData.attackeeNature = (EntityNature)avatarActor.avatarDataItem.Attribute;
				attackData.attackeeClass = avatarActor.config.CommonArguments.Class;
				float natureDamageBonusRatio = GetNatureDamageBonusRatio(attackData.attackerNature, attackData.attackeeNature, avatarActor);
				float num2 = 0f;
				if (attackData.attackerCategory == 4 && flag)
				{
					num2 = AvatarDefencePunishMetaDataReader.GetAvatarDefencePunishMetaDataByKey(levelDifference).DamageIncreaseRate;
				}
				float num3 = Mathf.Clamp(1f - attackData.attackerAddedAllDamageReduceRatio, 0f, 1f);
				attackData.damage *= natureDamageBonusRatio * (1f + num2) * num3;
				attackData.plainDamage *= natureDamageBonusRatio * (1f + num2) * num3;
				attackData.fireDamage *= natureDamageBonusRatio * (1f + num2) * num3;
				attackData.thunderDamage *= natureDamageBonusRatio * (1f + num2) * num3;
				attackData.iceDamage *= natureDamageBonusRatio * (1f + num2) * num3;
				attackData.alienDamage *= natureDamageBonusRatio * (1f + num2) * num3;
				attackData.natureDamageRatio = natureDamageBonusRatio;
				if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(avatarActor.runtimeID))
				{
					attackData.attackCameraShake = null;
				}
				attackData.attackeeAniDefenceRatio += avatarActor.GetProperty("Actor_AniDefenceDelta");
			}
			else if (to is MonsterActor)
			{
				MonsterActor monsterActor = (MonsterActor)to;
				int levelDifference2 = Mathf.Clamp((int)monsterActor.level - Singleton<PlayerModule>.Instance.playerData.teamLevel, 0, 10);
				if (attackData.attackerCategory == 3 && Singleton<LevelScoreManager>.Instance.IsAllowLevelPunish())
				{
					attackData.attackerAniDamageRatio *= 1f - AvatarAttackPunishMetaDataReader.GetAvatarAttackPunishMetaDataByKey(levelDifference2).AttackRatioReduce;
				}
				attackData.attackeeAniDefenceRatio = GetAnimDefenceRatio(monsterActor);
				attackData.damage = (attackData.attackerAttackValue * attackData.attackerAttackPercentage + attackData.attackerAddedAttackValue) * (1f + attackData.addedAttackRatio) * (1f + attackData.addedDamageRatio);
				float defence2 = ((float)monsterActor.defense + monsterActor.GetProperty("Actor_DefenceDelta")) * (1f + monsterActor.GetProperty("Actor_DefenceRatio"));
				float defenceRatio2 = GetDefenceRatio(defence2, attackData.attackerLevel);
				float num4 = (1f - defenceRatio2) * monsterActor.GetProperty("Actor_DamageReduceRatio");
				attackData.attackeeAddedDamageTakeRatio += monsterActor.GetProperty("Actor_DamageTakeRatio");
				attackData.damage = attackData.damage * num4 * (1f + attackData.attackeeAddedDamageTakeRatio);
				attackData.plainDamage = (attackData.attackerNormalDamage + attackData.attackerAttackValue * attackData.attackerNormalDamagePercentage) * (1f + attackData.addedAttackerNormalDamageRatio) * (1f + attackData.addedDamageRatio) * monsterActor.GetProperty("Actor_ResistAllElementAttackRatio") * monsterActor.GetProperty("Actor_ResistNormalAttackRatio") * (1f + monsterActor.GetProperty("Actor_NormalAttackTakeRatio"));
				attackData.fireDamage = (attackData.attackerFireDamage + attackData.attackerAttackValue * attackData.attackerFireDamagePercentage) * (1f + attackData.addedAttackerFireDamageRatio) * (1f + attackData.addedDamageRatio) * monsterActor.GetProperty("Actor_ResistAllElementAttackRatio") * monsterActor.GetProperty("Actor_ResistFireAttackRatio") * (1f + monsterActor.GetProperty("Actor_FireAttackTakeRatio"));
				attackData.thunderDamage = (attackData.attackerThunderDamage + attackData.attackerAttackValue * attackData.attackerThunderDamagePercentage) * (1f + attackData.addedAttackerThunderDamageRatio) * (1f + attackData.addedDamageRatio) * monsterActor.GetProperty("Actor_ResistAllElementAttackRatio") * monsterActor.GetProperty("Actor_ResistThunderAttackRatio") * (1f + monsterActor.GetProperty("Actor_ThunderAttackTakeRatio"));
				attackData.iceDamage = (attackData.attackerIceDamage + attackData.attackerAttackValue * attackData.attackerIceDamagePercentage) * (1f + attackData.addedAttackerIceDamageRatio) * (1f + attackData.addedDamageRatio) * monsterActor.GetProperty("Actor_ResistAllElementAttackRatio") * monsterActor.GetProperty("Actor_ResistIceAttackRatio") * (1f + monsterActor.GetProperty("Actor_IceAttackTakeRatio"));
				attackData.alienDamage = (attackData.attackerAlienDamage + attackData.attackerAttackValue * attackData.attackerAlienDamagePercentage) * (1f + attackData.addedAttackerAlienDamageRatio) * (1f + attackData.addedDamageRatio) * monsterActor.GetProperty("Actor_ResistAllElementAttackRatio") * monsterActor.GetProperty("Actor_ResistAllienAttackRatio") * (1f + monsterActor.GetProperty("Actor_AllienAttackTakeRatio"));
				attackData.attackeeNature = (EntityNature)monsterActor.metaConfig.nature;
				attackData.attackeeClass = monsterActor.config.CommonArguments.Class;
				float natureDamageBonusRatio2 = GetNatureDamageBonusRatio(attackData.attackerNature, attackData.attackeeNature, monsterActor);
				float num5 = 0f;
				if (attackData.attackerCategory == 3 && Singleton<LevelScoreManager>.Instance.IsAllowLevelPunish())
				{
					num5 = AvatarAttackPunishMetaDataReader.GetAvatarAttackPunishMetaDataByKey(levelDifference2).DamageReduceRate;
				}
				float num6 = Mathf.Clamp(1f - attackData.attackerAddedAllDamageReduceRatio, 0f, 1f);
				attackData.damage *= natureDamageBonusRatio2 * (1f - num5) * num6;
				attackData.plainDamage *= natureDamageBonusRatio2 * (1f - num5) * num6;
				attackData.fireDamage *= natureDamageBonusRatio2 * (1f - num5) * num6;
				attackData.thunderDamage *= natureDamageBonusRatio2 * (1f - num5) * num6;
				attackData.iceDamage *= natureDamageBonusRatio2 * (1f - num5) * num6;
				attackData.alienDamage *= natureDamageBonusRatio2 * (1f - num5) * num6;
				attackData.natureDamageRatio = natureDamageBonusRatio2;
				if (monsterActor.monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
				{
					attackData.retreatVelocity *= monsterActor.config.CommonArguments.BePushedSpeedRatioThrow;
				}
				else
				{
					attackData.retreatVelocity *= monsterActor.config.CommonArguments.BePushedSpeedRatio;
				}
				attackData.retreatVelocity *= 1f + monsterActor.GetProperty("Actor_BeRetreatRatio");
				if (attackData.isAnimEventAttack && Random.value < attackData.attackerCritChance)
				{
					attackData.damage *= attackData.attackerCritDamageRatio;
					attackData.hitLevel = AttackResult.ActorHitLevel.Critical;
				}
				attackData.attackeeAniDefenceRatio += monsterActor.GetProperty("Actor_AniDefenceDelta");
				if (attackData.frameHalt > 1)
				{
					attackData.frameHalt += 2;
				}
				if (attackData.attackeeAniDefenceRatio > attackData.attackerAniDamageRatio && attackData.frameHalt > 1)
				{
					attackData.frameHalt += attackData.noBreakFrameHaltAdd;
				}
			}
			else if (to is PropObjectActor)
			{
				PropObjectActor propObjectActor = (PropObjectActor)to;
				attackData.attackeeAniDefenceRatio = 0f;
				attackData.beHitEffectPattern = propObjectActor.config.BeHitEffect;
				attackData.damage = (attackData.attackerAttackValue * attackData.attackerAttackPercentage + attackData.attackerAddedAttackValue) * (1f + attackData.addedAttackRatio) * (1f + attackData.addedDamageRatio);
				attackData.plainDamage = (attackData.attackerNormalDamage + attackData.attackerAttackValue * attackData.attackerNormalDamagePercentage) * (1f + attackData.addedAttackerNormalDamageRatio) * (1f + attackData.addedDamageRatio) * propObjectActor.GetProperty("Actor_ResistAllElementAttackRatio") * propObjectActor.GetProperty("Actor_ResistNormalAttackRatio") * (1f + propObjectActor.GetProperty("Actor_NormalAttackTakeRatio"));
				attackData.fireDamage = (attackData.attackerFireDamage + attackData.attackerAttackValue * attackData.attackerFireDamagePercentage) * (1f + attackData.addedAttackerFireDamageRatio) * (1f + attackData.addedDamageRatio) * propObjectActor.GetProperty("Actor_ResistAllElementAttackRatio") * propObjectActor.GetProperty("Actor_ResistFireAttackRatio") * (1f + propObjectActor.GetProperty("Actor_FireAttackTakeRatio"));
				attackData.thunderDamage = (attackData.attackerThunderDamage + attackData.attackerAttackValue * attackData.attackerThunderDamagePercentage) * (1f + attackData.addedAttackerThunderDamageRatio) * (1f + attackData.addedDamageRatio) * propObjectActor.GetProperty("Actor_ResistAllElementAttackRatio") * propObjectActor.GetProperty("Actor_ResistThunderAttackRatio") * (1f + propObjectActor.GetProperty("Actor_ThunderAttackTakeRatio"));
				attackData.iceDamage = (attackData.attackerIceDamage + attackData.attackerAttackValue * attackData.attackerIceDamagePercentage) * (1f + attackData.addedAttackerIceDamageRatio) * (1f + attackData.addedDamageRatio) * propObjectActor.GetProperty("Actor_ResistAllElementAttackRatio") * propObjectActor.GetProperty("Actor_ResistIceAttackRatio") * (1f + propObjectActor.GetProperty("Actor_IceAttackTakeRatio"));
				attackData.alienDamage = (attackData.attackerAlienDamage + attackData.attackerAttackValue * attackData.attackerAlienDamagePercentage) * (1f + attackData.addedAttackerAlienDamageRatio) * (1f + attackData.addedDamageRatio) * propObjectActor.GetProperty("Actor_ResistAllElementAttackRatio") * propObjectActor.GetProperty("Actor_ResistAllienAttackRatio") * (1f + propObjectActor.GetProperty("Actor_AllienAttackTakeRatio"));
				attackData.attackeeAniDefenceRatio += propObjectActor.GetProperty("Actor_AniDefenceDelta");
				if (attackData.frameHalt > 1)
				{
					attackData.frameHalt += 2;
				}
			}
			attackData.resolveStep = AttackData.AttackDataStep.AttackeeResolved;
		}

		public static AttackResult ResolveAttackDataFinal(BaseActor attackee, AttackData attackData)
		{
			if (attackData.rejected)
			{
				return attackData;
			}
			if (attackData.attackeeAniDefenceRatio > attackData.attackerAniDamageRatio && attackData.hitEffect > AttackResult.AnimatorHitEffect.Light)
			{
				attackData.hitEffect = AttackResult.AnimatorHitEffect.Light;
			}
			if (attackee is MonsterActor && attackData.beHitEffectPattern == null)
			{
				MonsterActor monsterActor = (MonsterActor)attackee;
				if (attackData.isAnimEventAttack && attackData.hitEffect == AttackResult.AnimatorHitEffect.Light && attackData.attackerAniDamageRatio > 0.4f)
				{
					if ((attackData.attackEffectPattern == null || !attackData.attackEffectPattern.MuteAttackEffect) && monsterActor.IsActive() && monsterActor.monster.CurrentSkillID != null)
					{
						ConfigMonsterSkill configMonsterSkill = monsterActor.config.Skills[monsterActor.monster.CurrentSkillID];
						if (monsterActor.monster.GetCurrentNormalizedTime() < configMonsterSkill.AttackNormalizedTimeStop)
						{
							attackData.beHitEffectPattern = InLevelData.InLevelMiscData.NoBreakBehitEffect;
						}
					}
				}
				else if (monsterActor.abilityState.ContainsState(AbilityState.Frozen))
				{
					attackData.beHitEffectPattern = InLevelData.InLevelMiscData.FrozenBehitEffect;
				}
				else if (attackData.attackerAniDamageRatio <= 0.6f)
				{
					attackData.beHitEffectPattern = monsterActor.config.StateMachinePattern.BeHitEffect;
				}
				else if (attackData.attackerAniDamageRatio <= 0.8f)
				{
					attackData.beHitEffectPattern = monsterActor.config.StateMachinePattern.BeHitEffectMid;
				}
				else
				{
					attackData.beHitEffectPattern = monsterActor.config.StateMachinePattern.BeHitEffectBig;
				}
			}
			attackData.aniDamageRatio = attackData.attackerAniDamageRatio;
			List<KeyValuePair<AttackResult.ElementType, float>> list = new List<KeyValuePair<AttackResult.ElementType, float>>();
			list.Add(new KeyValuePair<AttackResult.ElementType, float>(AttackResult.ElementType.Plain, attackData.plainDamage));
			list.Add(new KeyValuePair<AttackResult.ElementType, float>(AttackResult.ElementType.Ice, attackData.iceDamage));
			list.Add(new KeyValuePair<AttackResult.ElementType, float>(AttackResult.ElementType.Fire, attackData.fireDamage));
			list.Add(new KeyValuePair<AttackResult.ElementType, float>(AttackResult.ElementType.Thunder, attackData.thunderDamage));
			list.Add(new KeyValuePair<AttackResult.ElementType, float>(AttackResult.ElementType.Alien, attackData.alienDamage));
			KeyValuePair<AttackResult.ElementType, float> keyValuePair = list[0];
			for (int i = 1; i < list.Count; i++)
			{
				KeyValuePair<AttackResult.ElementType, float> keyValuePair2 = list[i];
				if (keyValuePair2.Value > keyValuePair.Value)
				{
					keyValuePair = keyValuePair2;
				}
			}
			attackData.plainDamage = 0f;
			attackData.iceDamage = 0f;
			attackData.fireDamage = 0f;
			attackData.thunderDamage = 0f;
			attackData.alienDamage = 0f;
			switch (keyValuePair.Key)
			{
			case AttackResult.ElementType.Plain:
				attackData.plainDamage = keyValuePair.Value;
				break;
			case AttackResult.ElementType.Ice:
				attackData.iceDamage = keyValuePair.Value;
				break;
			case AttackResult.ElementType.Fire:
				attackData.fireDamage = keyValuePair.Value;
				break;
			case AttackResult.ElementType.Thunder:
				attackData.thunderDamage = keyValuePair.Value;
				break;
			case AttackResult.ElementType.Alien:
				attackData.alienDamage = keyValuePair.Value;
				break;
			}
			attackData.resolveStep = AttackData.AttackDataStep.FinalResolved;
			return attackData;
		}

		private static float GetAnimDefenceRatio(BaseActor actor)
		{
			float result = 0f;
			if (actor is MonsterActor)
			{
				MonsterActor monsterActor = (MonsterActor)actor;
				float defaultAnimDefenceRatio = monsterActor.config.StateMachinePattern.DefaultAnimDefenceRatio;
				string currentSkillID = monsterActor.monster.CurrentSkillID;
				result = defaultAnimDefenceRatio;
				if (monsterActor.monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
				{
					result = monsterActor.config.StateMachinePattern.ThrowAnimDefenceRatio + monsterActor.GetProperty("Actor_ThrowAniDefenceDelta");
				}
				else if (!string.IsNullOrEmpty(currentSkillID) && monsterActor.config.Skills.ContainsKey(currentSkillID))
				{
					float num = monsterActor.monster.GetCurrentNormalizedTime() % 1f;
					ConfigMonsterSkill configMonsterSkill = monsterActor.config.Skills[currentSkillID];
					if (num > configMonsterSkill.AnimDefenceNormalizedTimeStart && num < configMonsterSkill.AnimDefenceNormalizedTimeStop)
					{
						result = configMonsterSkill.AnimDefenceRatio;
					}
				}
			}
			else if (actor is AvatarActor)
			{
				AvatarActor avatarActor = (AvatarActor)actor;
				result = avatarActor.config.StateMachinePattern.DefaultAnimDefenceRatio;
				string currentSkillID2 = avatarActor.avatar.CurrentSkillID;
				if (!string.IsNullOrEmpty(currentSkillID2) && avatarActor.config.Skills.ContainsKey(currentSkillID2))
				{
					float num2 = avatarActor.avatar.GetCurrentNormalizedTime() % 1f;
					ConfigAvatarSkill configAvatarSkill = avatarActor.config.Skills[currentSkillID2];
					if (num2 > configAvatarSkill.AnimDefenceNormalizedTimeStart && num2 < configAvatarSkill.AnimDefenceNormalizedTimeStop)
					{
						result = configAvatarSkill.AnimDefenceRatio;
					}
				}
			}
			return result;
		}

		public static float GetNatureDamageBonusRatio(EntityNature attackerNature, EntityNature attackeeNature, BaseAbilityActor attackee)
		{
			if (attackee is MonsterActor)
			{
				return GetFixedDamageBonusFactor(DEFAULT_MONSTER_NATURE_DAMAGE_TABLE[(int)attackerNature, (int)attackeeNature]);
			}
			if (attackee is AvatarActor)
			{
				return DEFAULT_AVATAR_NATURE_DAMAGE_TABLE[(int)attackerNature, (int)attackeeNature];
			}
			return 0f;
		}

		private static float GetFixedDamageBonusFactor(float damageBonus)
		{
			float num = damageBonus;
			if (damageBonus > 1f)
			{
				num *= Mathf.Clamp(Singleton<LevelManager>.Instance.levelActor.upLevelNatureBonusFactor, 1f, Singleton<LevelManager>.Instance.levelActor.upLevelNatureBonusFactor);
			}
			else if (damageBonus < 1f)
			{
				num *= Mathf.Clamp(Singleton<LevelManager>.Instance.levelActor.downLevelNatureBonusFactor, 0f, 1f);
			}
			return num;
		}

		public static int GetNatureBonusType(EntityNature attackerNature, EntityNature attackeeNature)
		{
			return DEFAULT_NATURE_CIRCLE[(int)attackerNature, (int)attackeeNature];
		}

		public static float GetDefenceRatio(float defence, int attackerLevel)
		{
			if (defence < 0f)
			{
				return 0f;
			}
			return defence / ((float)(300 + attackerLevel * 20) + defence);
		}
	}
}
