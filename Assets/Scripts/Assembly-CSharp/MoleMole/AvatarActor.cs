using System;
using System.Collections.Generic;
using FullInspector;
using MoleMole.Config;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class AvatarActor : BaseAbilityActor
	{
		public enum SwitchEffectLevel
		{
			Idle = 0,
			SwitchEffect = 1,
			QTESwitchEffect = 2
		}

		[Serializable]
		public class SKillInfo
		{
			public string skillName;

			public SafeFloat CD = 0f;

			public SafeFloat costSP = 0f;

			public SafeFloat needSP = 0f;

			public SafeFloat cdTimer = 0f;

			public SafeInt32 MaxChargesCount = 0;

			public SafeInt32 chargesCounter = 0;

			public bool canHold;

			public bool IsInstantTrigger;

			public bool muted;

			public bool muteHighlighted;

			public string maskIconPath;

			public int avatarSkillID;

			public string iconPath;

			public ReviveSkillCDAction reviveCDAction;
		}

		private enum TiedState
		{
			Idle = 0,
			Tieing = 1
		}

		private enum ParalyzeState
		{
			Idle = 0,
			WaitForGrounded = 1,
			ParalyzeFreezed = 2,
			ParalyzeHitResuming = 3
		}

		private const float COOLED_DOWN_TIME = -253f;

		public BaseMonoAvatar avatar;

		public SafeFloat critical = 0f;

		[InspectorCollapsedFoldout]
		public ConfigAvatar config;

		[ShowInInspector]
		protected bool _isOnStage;

		public string avatarIconPath;

		public List<SKillInfo> skillInfoList;

		private Dictionary<string, SKillInfo> _skillInfoMap;

		private EntityTimer _switchInTimer;

		private bool _allowOtherCanSwithInWhenSelfOnStage;

		private bool _useATKButtonHoldMode;

		private bool _isInQTEWarning;

		private bool _muteOtherQTE;

		public Action<bool> onQTEChange;

		public Action<string, int, int> onSkillChargeChanged;

		public Action<string, float, float> onSkillSPNeedChanged;

		[InspectorCollapsedFoldout]
		public AvatarDataItem avatarDataItem;

		public List<string> maskedSkillButtons;

		private static string[] DEFAULT_SKILL_BUTTON_NAMES = new string[3] { "ATK", "SKL01", "SKL02" };

		private TiedState _tiedState;

		private ParalyzeState _paralyzeState;

		private EntityTimer _paralyzeTimer;

		private int _paralyzeMassRatioIx;

		private int _freezeMassRatioIx;

		private int _stunMassRatioIx;

		private int _paralyzeAnimatorSpeedIx;

		private int _freezeAnimatorSpeedIx;

		public bool AllowOtherSwitchIn
		{
			get
			{
				return _allowOtherCanSwithInWhenSelfOnStage;
			}
			set
			{
				_allowOtherCanSwithInWhenSelfOnStage = value;
			}
		}

		public SwitchEffectLevel switchButtonEffect { get; private set; }

		public string CurrentQTEName { get; private set; }

		public bool IsInQTE
		{
			get
			{
				return _isInQTEWarning;
			}
		}

		public bool MuteOtherQTE
		{
			get
			{
				return _muteOtherQTE;
			}
			set
			{
				_muteOtherQTE = value;
			}
		}

		public bool isLeader { get; set; }

		public override void Init(BaseMonoEntity entity)
		{
			avatar = (BaseMonoAvatar)entity;
			runtimeID = avatar.GetRuntimeID();
			config = AvatarData.GetAvatarConfig(avatar.AvatarTypeName);
			commonConfig = config.CommonConfig;
			base.Init(entity);
			skillInfoList = new List<SKillInfo>();
			_skillInfoMap = new Dictionary<string, SKillInfo>();
			maskedSkillButtons = new List<string>();
			foreach (string key in config.Skills.Keys)
			{
				ConfigAvatarSkill skillConfig = config.Skills[key];
				string skillName = GetSkillNameByAnimEventID(key);
				if (!Miscs.ArrayContains(DEFAULT_SKILL_BUTTON_NAMES, skillName) || _skillInfoMap.ContainsKey(skillName))
				{
					continue;
				}
				SKillInfo sKillInfo = new SKillInfo();
				sKillInfo.skillName = skillName;
				sKillInfo.cdTimer = -253f;
				sKillInfo.CD = Mathf.Max(0f, skillConfig.SkillCD + Evaluate(skillConfig.SkillCDDelta));
				sKillInfo.costSP = Mathf.Max(0f, skillConfig.SPCost + Evaluate(skillConfig.SPCostDelta));
				sKillInfo.needSP = Mathf.Max(0f, skillConfig.SPNeed + Evaluate(skillConfig.SPCostDelta));
				sKillInfo.MaxChargesCount = skillConfig.ChargesCount + Evaluate(skillConfig.ChargesCountDelta);
				sKillInfo.canHold = skillConfig.CanHold;
				sKillInfo.reviveCDAction = skillConfig.ReviveCDAction;
				sKillInfo.IsInstantTrigger = skillConfig.IsInstantTrigger;
				sKillInfo.muted = false;
				sKillInfo.muteHighlighted = skillConfig.MuteHighlighted;
				sKillInfo.maskIconPath = null;
				SKillInfo sKillInfo2 = sKillInfo;
				sKillInfo2.chargesCounter = sKillInfo2.MaxChargesCount;
				if (skillName == "ATK")
				{
					sKillInfo2.muteHighlighted = true;
				}
				skillInfoList.Add(sKillInfo2);
				_skillInfoMap.Add(skillName, sKillInfo2);
				if (skillConfig.SkillCDDelta.isDynamic)
				{
					RegisterPropertyChangedCallback(skillConfig.SkillCDDelta.dynamicKey, delegate
					{
						_skillInfoMap[skillName].CD = Mathf.Max(0f, skillConfig.SkillCD + Evaluate(skillConfig.SkillCDDelta));
					});
				}
				if (skillConfig.ChargesCountDelta.isDynamic)
				{
					RegisterPropertyChangedCallback(skillConfig.ChargesCountDelta.dynamicKey, delegate
					{
						int arg = _skillInfoMap[skillName].chargesCounter;
						_skillInfoMap[skillName].MaxChargesCount = skillConfig.ChargesCount + Evaluate(skillConfig.ChargesCountDelta);
						_skillInfoMap[skillName].chargesCounter = _skillInfoMap[skillName].MaxChargesCount;
						if (onSkillChargeChanged != null)
						{
							onSkillChargeChanged(skillName, arg, _skillInfoMap[skillName].chargesCounter);
						}
					});
				}
				if (!skillConfig.SPCostDelta.isDynamic)
				{
					continue;
				}
				RegisterPropertyChangedCallback(skillConfig.SPCostDelta.dynamicKey, delegate
				{
					float arg = _skillInfoMap[skillName].needSP;
					_skillInfoMap[skillName].costSP = Mathf.Max(0f, skillConfig.SPCost + Evaluate(skillConfig.SPCostDelta));
					_skillInfoMap[skillName].needSP = Mathf.Max(0f, skillConfig.SPNeed + Evaluate(skillConfig.SPCostDelta));
					if (onSkillSPNeedChanged != null)
					{
						onSkillSPNeedChanged(skillName, arg, _skillInfoMap[skillName].needSP);
					}
				});
			}
			BaseMonoAvatar baseMonoAvatar = avatar;
			baseMonoAvatar.onActiveChanged = (Action<bool>)Delegate.Combine(baseMonoAvatar.onActiveChanged, new Action<bool>(OnActiveChanged));
			_switchInTimer = new EntityTimer(config.CommonArguments.SwitchInCD, entity);
			EntityTimer switchInTimer = _switchInTimer;
			switchInTimer.timeupAction = (Action)Delegate.Combine(switchInTimer.timeupAction, new Action(OnSwitchInReady));
			_switchInTimer.Reset(false);
			_switchInTimer.isTimeUp = true;
			AddPlugin(new AvatarAIPlugin(this));
			Singleton<EventManager>.Instance.FireEvent(new EvtAvatarCreated(runtimeID));
			_paralyzeTimer = new EntityTimer();
			_paralyzeTimer.SetActive(false);
			_paralyzeState = ParalyzeState.Idle;
			InitAbilityStateImmune();
			InitDebuffDurationRatio();
			CurrentQTEName = string.Empty;
		}

		private void InitAbilityStateImmune()
		{
			ConfigDebuffResistance debuffResistance = config.DebuffResistance;
			List<AbilityState> immuneStates = debuffResistance.ImmuneStates;
			foreach (AbilityState item in immuneStates)
			{
				SetAbilityStateImmune(item, true);
			}
		}

		private void InitDebuffDurationRatio()
		{
			ConfigDebuffResistance debuffResistance = config.DebuffResistance;
			float durationRatio = debuffResistance.DurationRatio;
			if (durationRatio > 0f)
			{
				PushProperty("Actor_DebuffDurationRatioDelta", 0f - durationRatio);
			}
		}

		public void InitAvatarDataItem(AvatarDataItem avatarDataItem, bool isLocal, bool isHelper, bool isLeader, bool leaderSkillOn)
		{
			this.avatarDataItem = avatarDataItem;
			this.isLeader = isLeader;
			InitHPAndSP();
			level = avatarDataItem.level;
			attack = avatarDataItem.FinalAttack;
			critical = avatarDataItem.FinalCritical;
			defense = avatarDataItem.FinalDefense;
			avatarIconPath = avatarDataItem.IconPathInLevel;
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			if (levelActor.levelMode == LevelActor.Mode.Single)
			{
				_isOnStage = isLocal || isHelper;
			}
			else
			{
				_isOnStage = true;
			}
			maskedSkillButtons.AddRange(config.CommonArguments.MaskedSkillButtons);
			AvatarData.UnlockAvatarAbilities(avatarDataItem, this, isLeader || leaderSkillOn);
			SetupSkillInfo(avatarDataItem);
			List<ConfigEquipmentSkillEntry> skillEntryList = new List<ConfigEquipmentSkillEntry>();
			EquipmentSkillData.AddAvatarWeaponEquipSkillAbilities(avatarDataItem, this, ref skillEntryList);
			EquipmentSkillData.AddAvatarStigmataEquipSkillAbilities(avatarDataItem, this, ref skillEntryList);
			EquipmentSkillData.AddAvatarSetEquipSkillAbilities(avatarDataItem, this, ref skillEntryList);
			SetupWeaponActiveSkillInfo(skillEntryList);
			if (isHelper)
			{
				AddPlugin(new AvatarHelperStatePlugin(this));
			}
		}

		public void InitGalTouchBuff(AvatarDataItem avatarDataItem)
		{
			int num = Singleton<GalTouchModule>.Instance.UseBuff(avatarDataItem.avatarID);
			if (num != 0)
			{
				TouchBuffItem touchBuffItem = GalTouchData.GetTouchBuffItem(num);
				if (touchBuffItem != null)
				{
					int characterHeartLevel = Singleton<GalTouchModule>.Instance.GetCharacterHeartLevel(avatarDataItem.avatarID);
					float calculatedParam = GalTouchBuffData.GetCalculatedParam(touchBuffItem.param1, touchBuffItem.param1Add, characterHeartLevel);
					float calculatedParam2 = GalTouchBuffData.GetCalculatedParam(touchBuffItem.param2, touchBuffItem.param2Add, characterHeartLevel);
					float calculatedParam3 = GalTouchBuffData.GetCalculatedParam(touchBuffItem.param3, touchBuffItem.param3Add, characterHeartLevel);
					GalTouchBuffData.ApplyGalTouchBuffEntry(this, num, calculatedParam, calculatedParam2, calculatedParam3);
				}
			}
		}

		private void InitHPAndSP()
		{
			baseMaxHP = (maxHP = (HP = avatarDataItem.FinalHP));
			baseMaxSP = (maxSP = avatarDataItem.FinalSP);
			SP = 0f;
		}

		private void ResetHPAndSPWhenRevive()
		{
			DelegateUtils.UpdateField(ref maxHP, avatarDataItem.FinalHP, onMaxHPChanged);
			DelegateUtils.UpdateField(ref maxSP, avatarDataItem.FinalSP, onMaxSPChanged);
			DelegateUtils.UpdateField(ref HP, maxHP, 0f, onHPChanged);
			DelegateUtils.UpdateField(ref SP, maxSP, 0f, onSPChanged);
			HPPropertyChangedCallback();
			SPPropertyChangedCallback();
		}

		private bool SetupSkillInfo(AvatarDataItem avatarDataItem)
		{
			foreach (AvatarSkillDataItem skillData in avatarDataItem.skillDataList)
			{
				if (_skillInfoMap.ContainsKey(skillData.ButtonName))
				{
					_skillInfoMap[skillData.ButtonName].avatarSkillID = skillData.skillID;
					_skillInfoMap[skillData.ButtonName].iconPath = skillData.IconPathInLevel;
				}
			}
			return false;
		}

		private void SetupWeaponActiveSkillInfo(List<ConfigEquipmentSkillEntry> skillEntryList)
		{
			foreach (ConfigEquipmentSkillEntry skillEntry in skillEntryList)
			{
				if (skillEntry.IsActiveSkill)
				{
					EquipSkillMetaData equipSkillMetaDataByKey = EquipSkillMetaDataReader.GetEquipSkillMetaDataByKey(skillEntry.EquipmentSkillID);
					SKillInfo sKillInfo = new SKillInfo();
					sKillInfo.skillName = "SKL_WEAPON";
					sKillInfo.CD = skillEntry.SkillCD;
					sKillInfo.costSP = skillEntry.SPCost;
					sKillInfo.needSP = skillEntry.SPNeed;
					sKillInfo.MaxChargesCount = skillEntry.MaxChargesCount;
					sKillInfo.chargesCounter = skillEntry.MaxChargesCount;
					sKillInfo.cdTimer = -253f;
					sKillInfo.canHold = false;
					sKillInfo.avatarSkillID = skillEntry.EquipmentSkillID;
					sKillInfo.iconPath = equipSkillMetaDataByKey.skillIconPath;
					SKillInfo sKillInfo2 = sKillInfo;
					skillInfoList.Add(sKillInfo2);
					_skillInfoMap.Add("SKL_WEAPON", sKillInfo2);
					break;
				}
			}
		}

		public Dictionary<string, object> CreateAppliedAbility(ConfigAbility abilityConfig)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			appliedAbilities.Add(Tuple.Create(abilityConfig, dictionary));
			return dictionary;
		}

		public override void PostInit()
		{
			base.PostInit();
			abilityPlugin.onKillBehavior = ActorAbilityPlugin.OnKillBehavior.RemoveAllDebuffsAndDurationed;
			Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelBuffState>(runtimeID);
		}

		public override bool OnEventWithPlugins(BaseEvent evt)
		{
			bool result = base.OnEventWithPlugins(evt);
			if (evt is EvtSkillStart)
			{
				return OnSkillStart((EvtSkillStart)evt);
			}
			if (evt is EvtHittingOther)
			{
				return OnHittingOther((EvtHittingOther)evt);
			}
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			if (evt is EvtKilled)
			{
				return OnKill((EvtKilled)evt);
			}
			if (evt is EvtAttackLanded)
			{
				return OnAttackLanded((EvtAttackLanded)evt);
			}
			if (evt is EvtDamageLanded)
			{
				return OnDamageLanded((EvtDamageLanded)evt);
			}
			if (evt is EvtAvatarSwapOutStart)
			{
				return OnAvatarSwappedOutStart((EvtAvatarSwapOutStart)evt);
			}
			return result;
		}

		public override bool OnEventResolves(BaseEvent evt)
		{
			bool flag = false;
			if (evt is EvtHittingOther)
			{
				flag |= OnHittingOtherResolve((EvtHittingOther)evt);
			}
			else if (evt is EvtBeingHit)
			{
				flag |= OnBeingHitResolve((EvtBeingHit)evt);
			}
			flag |= base.OnEventResolves(evt);
			return false;
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			bool flag = base.ListenEvent(evt);
			if (evt is EvtLevelBuffState)
			{
				flag |= OnLevelBuffState((EvtLevelBuffState)evt);
			}
			return flag;
		}

		private bool OnSkillStart(EvtSkillStart evt)
		{
			string skillID = evt.skillID;
			string skillNameByAnimEventID = GetSkillNameByAnimEventID(skillID);
			float skillSPCost = GetSkillSPCost(skillNameByAnimEventID);
			DelegateUtils.UpdateField(ref SP, Mathf.Clamp((float)SP - skillSPCost, 0f, maxSP), 0f - skillSPCost, onSPChanged);
			if ((int)_skillInfoMap[skillNameByAnimEventID].MaxChargesCount > 0)
			{
				int num = _skillInfoMap[skillNameByAnimEventID].chargesCounter;
				int num2 = Mathf.Clamp(num - 1, 0, _skillInfoMap[skillNameByAnimEventID].MaxChargesCount);
				_skillInfoMap[skillNameByAnimEventID].chargesCounter = num2;
				if (onSkillChargeChanged != null)
				{
					onSkillChargeChanged(evt.skillID, num, num2);
				}
				if (!IsSkillInCD(skillNameByAnimEventID))
				{
					_skillInfoMap[skillNameByAnimEventID].cdTimer = GetSkillCD(skillNameByAnimEventID);
				}
			}
			else
			{
				_skillInfoMap[skillNameByAnimEventID].cdTimer = GetSkillCD(skillNameByAnimEventID);
			}
			avatar.ClearAttackTriggers();
			return true;
		}

		protected virtual bool OnHittingOther(EvtHittingOther evt)
		{
			if (evt.attackData == null)
			{
				evt.attackData = DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(this, evt.animEventID);
			}
			if (evt.attackData.hitCollision == null && evt.hitCollision != null)
			{
				evt.attackData.hitCollision = evt.hitCollision;
			}
			else if (evt.hitCollision == null && evt.attackData.hitCollision == null)
			{
				BaseMonoAnimatorEntity baseMonoAnimatorEntity = Singleton<EventManager>.Instance.GetEntity(evt.toID) as BaseMonoAnimatorEntity;
				if (baseMonoAnimatorEntity != null)
				{
					evt.hitCollision = new AttackResult.HitCollsion
					{
						hitPoint = baseMonoAnimatorEntity.RootNodePosition,
						hitDir = baseMonoAnimatorEntity.XZPosition - avatar.XZPosition
					};
				}
			}
			return true;
		}

		protected virtual bool OnHittingOtherResolve(EvtHittingOther evt)
		{
			evt.Resolve();
			Singleton<EventManager>.Instance.FireEvent(new EvtBeingHit(evt.toID, runtimeID, evt.animEventID, evt.attackData));
			MarkImportantEventIsHandled(evt);
			return true;
		}

		protected virtual bool OnBeingHit(EvtBeingHit evt)
		{
			if (!_isOnStage)
			{
				evt.attackData.Reject(AttackResult.RejectType.RejectAll);
				return true;
			}
			if (!isAlive)
			{
				evt.attackData.Reject(AttackResult.RejectType.RejectAll);
				return true;
			}
			DamageModelLogic.ResolveAttackDataByAttackee(this, evt.attackData);
			return true;
		}

		protected virtual bool OnBeingHitResolve(EvtBeingHit evt)
		{
			evt.Resolve();
			if (evt.attackData.isAnimEventAttack && abilityState.ContainsState(AbilityState.BlockAnimEventAttack))
			{
				evt.attackData.Reject(AttackResult.RejectType.RejectAll);
			}
			if (evt.attackData.rejected)
			{
				if (evt.attackData.rejectState == AttackResult.RejectType.RejectButShowAttackEffect)
				{
					AmendHitCollision(evt.attackData);
					FireAttackDataEffects(evt.attackData);
				}
				return false;
			}
			AttackResult attackResult = DamageModelLogic.ResolveAttackDataFinal(this, evt.attackData);
			AmendHitCollision(attackResult);
			if (abilityState.ContainsState(AbilityState.Invincible))
			{
				attackResult.damage = 0f;
				attackResult.plainDamage = 0f;
				attackResult.fireDamage = 0f;
				attackResult.thunderDamage = 0f;
				attackResult.iceDamage = 0f;
				attackResult.alienDamage = 0f;
				attackResult.hitLevel = AttackResult.ActorHitLevel.Mute;
				attackResult.hitEffect = AttackResult.AnimatorHitEffect.Mute;
				attackResult.frameHalt += 5;
			}
			else if (abilityState.ContainsState(AbilityState.Endure))
			{
				attackResult.hitEffect = AttackResult.AnimatorHitEffect.Mute;
				attackResult.frameHalt += 5;
			}
			if (!attackResult.isAnimEventAttack)
			{
				attackResult.hitEffect = AttackResult.AnimatorHitEffect.Mute;
				attackResult.hitEffectPattern = AttackResult.HitEffectPattern.OnlyBeHit;
				attackResult.attackCameraShake = null;
			}
			float totalDamage = attackResult.GetTotalDamage();
			float num = (float)HP - totalDamage;
			if (num <= 0f)
			{
				num = 0f;
			}
			if (abilityState.ContainsState(AbilityState.Undamagable))
			{
				DelegateUtils.UpdateField(ref HP, HP, num - (float)HP, onHPChanged);
			}
			else
			{
				DelegateUtils.UpdateField(ref HP, num, num - (float)HP, onHPChanged);
				evt.resolvedDamage = totalDamage;
			}
			if ((float)HP == 0f)
			{
				if ((abilityState & AbilityState.Limbo) != AbilityState.None)
				{
					evt.beHitEffect = BeHitEffect.NormalBeHit;
					BeingHit(attackResult, BeHitEffect.NormalBeHit, evt.sourceID);
				}
				else
				{
					evt.beHitEffect = BeHitEffect.KillingBeHit;
					Kill(evt.sourceID, evt.animEventID, KillEffect.KillNow);
					BeingHit(attackResult, BeHitEffect.KillingBeHit, evt.sourceID);
				}
			}
			else
			{
				evt.beHitEffect = BeHitEffect.NormalBeHit;
				BeingHit(attackResult, BeHitEffect.NormalBeHit, evt.sourceID);
			}
			FireAttackDataEffects(attackResult);
			if (_tiedState == TiedState.Tieing && evt.attackData.isAnimEventAttack && evt.attackData.hitEffect >= AttackResult.AnimatorHitEffect.Normal)
			{
				abilityPlugin.RemoveModifierByState(AbilityState.Tied);
			}
			if (evt.attackData.isAnimEventAttack)
			{
				EvtAttackLanded evt2 = new EvtAttackLanded(evt.sourceID, runtimeID, evt.animEventID, attackResult);
				Singleton<EventManager>.Instance.FireEvent(evt2);
			}
			else
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtDamageLanded(evt.sourceID, runtimeID, attackResult));
			}
			MarkImportantEventIsHandled(evt);
			return true;
		}

		public void AmendHitCollision(AttackResult attackResult)
		{
			if (attackResult.hitCollision == null)
			{
				attackResult.hitCollision = new AttackResult.HitCollsion
				{
					hitPoint = avatar.RootNodePosition,
					hitDir = -avatar.FaceDirection
				};
			}
			else if (attackResult.hitCollision.hitDir == Vector3.zero)
			{
				attackResult.hitCollision.hitPoint = avatar.RootNodePosition;
				attackResult.hitCollision.hitDir = -avatar.FaceDirection;
			}
		}

		public void FireAttackDataEffects(AttackResult attackResult)
		{
			if (attackResult.attackEffectPattern != null && (attackResult.hitEffectPattern == AttackResult.HitEffectPattern.Normal || attackResult.hitEffectPattern == AttackResult.HitEffectPattern.OnlyAttack))
			{
				AttackPattern.ActAttackEffects(attackResult.attackEffectPattern, avatar, attackResult.hitCollision.hitPoint, attackResult.hitCollision.hitDir);
			}
			if (attackResult.attackCameraShake != null)
			{
				AttackPattern.ActCameraShake(attackResult.attackCameraShake);
			}
		}

		private bool OnKill(EvtKilled evt)
		{
			abilityPlugin.RemoveAllDebuffModifiers();
			Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelBuffState>(runtimeID);
			return true;
		}

		private bool OnAttackLanded(EvtAttackLanded evt)
		{
			AttackLanded(evt);
			if (evt.animEventID == null)
			{
				return true;
			}
			ConfigAvatarAnimEvent configAvatarAnimEvent = SharedAnimEventData.ResolveAnimEvent(config, evt.animEventID);
			if (configAvatarAnimEvent == null)
			{
				return true;
			}
			if ((bool)isAlive)
			{
				float num = configAvatarAnimEvent.AttackProperty.SPRecover * config.CommonArguments.AttackSPRecoverRatio * (1f + GetProperty("Actor_SPRecoverRatio"));
				DelegateUtils.UpdateField(ref SP, Mathf.Clamp((float)SP + num, 0f, maxSP), num, onSPChanged);
				float num2 = evt.attackResult.damage * GetProperty("Actor_AttackStealHPRatio");
				DelegateUtils.UpdateField(ref HP, Mathf.Clamp((float)HP + num2, 0f, maxHP), num2, onHPChanged);
			}
			MarkImportantEventIsHandled(evt);
			return true;
		}

		private bool OnDamageLanded(EvtDamageLanded evt)
		{
			DamageLanded(evt);
			return true;
		}

		public bool OnAvatarSwappedOutStart(EvtAvatarSwapOutStart evt)
		{
			bool flag = false;
			foreach (BaseMonoAvatar allPlayerAvatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
			{
				if (allPlayerAvatar.GetRuntimeID() != runtimeID)
				{
					AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(allPlayerAvatar.GetRuntimeID());
					if (actor.CanSwitchInQTE())
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				_switchInTimer.timespan *= config.CommonArguments.QTESwitchInCDRatio;
			}
			_switchInTimer.Reset(true);
			return true;
		}

		public bool OnLevelBuffState(EvtLevelBuffState evt)
		{
			if (evt.levelBuff == LevelBuffType.WitchTime && evt.state == LevelBuffState.Start && Singleton<AvatarManager>.Instance.IsLocalAvatar(runtimeID) && evt.sourceId == runtimeID)
			{
				MonoEntityAudio component = entity.GetComponent<MonoEntityAudio>();
				if (component != null)
				{
					component.PostWitchTime();
				}
			}
			return true;
		}

		public override void Core()
		{
			base.Core();
			if (!_allowOtherCanSwithInWhenSelfOnStage)
			{
				_switchInTimer.Core(1f);
			}
			if (!isAlive)
			{
				return;
			}
			MonoLevelEntity levelEntity = Singleton<LevelManager>.Instance.levelEntity;
			for (int i = 0; i < skillInfoList.Count; i++)
			{
				SKillInfo sKillInfo = skillInfoList[i];
				if (!((float)sKillInfo.cdTimer >= 0f))
				{
					continue;
				}
				sKillInfo.cdTimer = (float)sKillInfo.cdTimer - levelEntity.TimeScale * Time.deltaTime;
				if ((float)sKillInfo.cdTimer < 0f)
				{
					sKillInfo.cdTimer = -253f;
				}
				if ((float)sKillInfo.cdTimer == -253f && (int)sKillInfo.MaxChargesCount > 0)
				{
					int num = sKillInfo.chargesCounter;
					int num2 = Mathf.Clamp(num + 1, 0, sKillInfo.MaxChargesCount);
					sKillInfo.chargesCounter = num2;
					if (onSkillChargeChanged != null)
					{
						onSkillChargeChanged(sKillInfo.skillName, num, num2);
					}
					if ((int)sKillInfo.chargesCounter < (int)sKillInfo.MaxChargesCount)
					{
						sKillInfo.cdTimer = GetSkillCD(sKillInfo.skillName);
					}
				}
			}
			UpdateAbilityState();
		}

		public virtual void AttackLanded(EvtAttackLanded evt)
		{
			avatar.FrameHalt(evt.attackResult.frameHalt);
		}

		protected virtual void DamageLanded(EvtDamageLanded evt)
		{
			avatar.FrameHalt(evt.attackResult.frameHalt);
		}

		public virtual void BeingHit(AttackResult attackResult, BeHitEffect beHitEffect, uint sourceID)
		{
			bool doSteerToHitForward = false;
			bool targetLockSource = false;
			if (attackResult.hitEffect > AttackResult.AnimatorHitEffect.Light && attackResult.hitCollision.hitDir != Vector3.zero)
			{
				doSteerToHitForward = true;
			}
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(runtimeID) && avatar.AttackTarget == null && avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.AllowTriggerInput) && Singleton<RuntimeIDManager>.Instance.ParseCategory(sourceID) == 4)
			{
				BaseMonoMonster baseMonoMonster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(sourceID);
				if (baseMonoMonster != null && !baseMonoMonster.denySelect)
				{
					targetLockSource = true;
				}
			}
			if (_paralyzeState == ParalyzeState.ParalyzeFreezed && attackResult.hitEffect > AttackResult.AnimatorHitEffect.Light)
			{
				avatar.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx, 1f);
				_paralyzeTimer.timespan = 0.35f;
				_paralyzeTimer.Reset(true);
				_paralyzeState = ParalyzeState.ParalyzeHitResuming;
			}
			if (abilityState.ContainsState(AbilityState.Frozen))
			{
				doSteerToHitForward = false;
				if (attackResult.hitEffect > AttackResult.AnimatorHitEffect.Light)
				{
					attackResult.hitEffect = AttackResult.AnimatorHitEffect.Light;
				}
			}
			avatar.BeHit(attackResult.frameHalt, attackResult.hitEffect, attackResult.hitEffectAux, attackResult.killEffect, beHitEffect, attackResult.aniDamageRatio, attackResult.hitCollision.hitDir, attackResult.retreatVelocity, sourceID, targetLockSource, doSteerToHitForward);
		}

		public virtual void Kill(uint killerID, string killerAnimEventID, KillEffect killEffect)
		{
			if (onJustKilled != null)
			{
				onJustKilled(killerID, killerAnimEventID, killEffect);
			}
			if (!isAlive)
			{
				if (killEffect == KillEffect.KillImmediately)
				{
					avatar.SetDied(KillEffect.KillImmediately);
				}
			}
			else
			{
				isAlive = false;
				Singleton<EventManager>.Instance.FireEvent(new EvtKilled(runtimeID, killerID, killerAnimEventID));
				avatar.SetDied(KillEffect.KillNow);
			}
		}

		public void ForceKill()
		{
			Kill(562036737u, null, KillEffect.KillNow);
		}

		public override void ForceKill(uint killerID, KillEffect killEffect)
		{
			Kill(killerID, null, killEffect);
		}

		public void Revive(Vector3 revivePosition)
		{
			isAlive = true;
			ResetHPAndSPWhenRevive();
			SetSkillCDWhenRevive();
			abilityPlugin.ResetKilled();
			avatar.Revive(revivePosition);
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
			{
				if (localAvatar == avatar || localAvatar.switchState == BaseMonoAvatar.AvatarSwitchState.OffStage)
				{
					Singleton<LevelManager>.Instance.levelActor.SingleModeSwapTo(revivePosition, localAvatar.FaceDirection, avatar);
				}
				else
				{
					Singleton<LevelManager>.Instance.levelActor.TriggerSwapLocalAvatar(localAvatar.GetRuntimeID(), runtimeID, true);
				}
			}
			else if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi || Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.MultiRemote)
			{
				Singleton<LevelManager>.Instance.levelActor.SingleModeSwapTo(revivePosition, localAvatar.FaceDirection, avatar);
				if (localAvatar != avatar && localAvatar.switchState == BaseMonoAvatar.AvatarSwitchState.OnStage)
				{
					localAvatar.RefreshController();
				}
			}
		}

		private void SetSkillCDWhenRevive()
		{
			foreach (SKillInfo value in _skillInfoMap.Values)
			{
				ReviveSkillCDAction reviveCDAction = value.reviveCDAction;
				if (reviveCDAction != ReviveSkillCDAction.KeepLast && reviveCDAction == ReviveSkillCDAction.Cleer)
				{
					value.cdTimer = -253f;
					value.chargesCounter = value.MaxChargesCount;
				}
			}
		}

		public override void OnRemoval()
		{
			base.OnRemoval();
			DisableQTEAttack();
			onSkillChargeChanged = null;
			onSkillSPNeedChanged = null;
			onQTEChange = null;
		}

		private void OnActiveChanged(bool active)
		{
			_isOnStage = active;
		}

		public void OnSwitchInReady()
		{
			_switchInTimer.timespan = config.CommonArguments.SwitchInCD;
		}

		public void SetMuteSkill(string skillName, bool muted)
		{
			_skillInfoMap[skillName].muted = muted;
		}

		public bool CanUseSkill(string skillName)
		{
			if (_skillInfoMap[skillName].muted)
			{
				return false;
			}
			if (IsSkillLocked(skillName))
			{
				return false;
			}
			if ((int)_skillInfoMap[skillName].MaxChargesCount > 0)
			{
				if (skillName == "SKL01")
				{
					return IsSPEnough(skillName);
				}
				return IsSPEnough(skillName) && (int)_skillInfoMap[skillName].chargesCounter > 0;
			}
			return IsSPEnough(skillName) && !IsSkillInCD(skillName);
		}

		public bool HasChargesLeft(string skillName)
		{
			return (int)_skillInfoMap[skillName].MaxChargesCount <= 0 || (int)_skillInfoMap[skillName].chargesCounter > 0;
		}

		public void SetAllowOtherCanSwitchIn(bool canSwitchIn)
		{
			_allowOtherCanSwithInWhenSelfOnStage = canSwitchIn;
		}

		public void SetAttackButtonHoldMode(bool useHoldMod)
		{
			_useATKButtonHoldMode = useHoldMod;
		}

		public bool IsAttackButtonHoldMode()
		{
			return _useATKButtonHoldMode || avatar.IsAttackHoldMode();
		}

		public void EnableQTEAttack(string QTEName)
		{
			CurrentQTEName = QTEName;
			_isInQTEWarning = true;
			if (onQTEChange != null)
			{
				onQTEChange(true);
			}
		}

		public void DisableQTEAttack()
		{
			CurrentQTEName = string.Empty;
			_isInQTEWarning = false;
			if (onQTEChange != null)
			{
				onQTEChange(false);
			}
		}

		public bool CanSwitchInQTE()
		{
			return !string.IsNullOrEmpty(CurrentQTEName);
		}

		public bool IsSPEnough(string skillName)
		{
			return (float)SP >= GetSkillSPCost(skillName) && (float)SP >= GetSkillSPNeed(skillName);
		}

		public bool IsSkillInCD(string skillName)
		{
			return (float)_skillInfoMap[skillName].cdTimer >= 0f;
		}

		public bool IsSkillLocked(string skillName)
		{
			return maskedSkillButtons.Contains(skillName);
		}

		public bool IsSkillHasCD(string skillName)
		{
			return (float)_skillInfoMap[skillName].CD > 0f;
		}

		public bool IsSwitchInCD()
		{
			return !_switchInTimer.isTimeUp;
		}

		public void ChangeSwitchInCDTime(float CDTime)
		{
			_switchInTimer = new EntityTimer(CDTime, entity);
			EntityTimer switchInTimer = _switchInTimer;
			switchInTimer.timeupAction = (Action)Delegate.Combine(switchInTimer.timeupAction, new Action(OnSwitchInReady));
			_switchInTimer.Reset(false);
			_switchInTimer.isTimeUp = true;
		}

		public void ResetSwitchInTimer()
		{
			_switchInTimer.Reset(false);
			_switchInTimer.isTimeUp = false;
		}

		public float GetSkillSPNeed(string skillName)
		{
			float result = 0f;
			switch (skillName)
			{
			case "SKL01":
			case "SKL02":
			case "SKL_WEAPON":
			{
				SKillInfo sKillInfo = _skillInfoMap[skillName];
				float num = Mathf.Max(sKillInfo.costSP, sKillInfo.needSP);
				result = num;
				break;
			}
			}
			return result;
		}

		public float GetSkillSPCost(string skillName)
		{
			float result = 0f;
			switch (skillName)
			{
			case "SKL01":
			case "SKL02":
			case "SKL_WEAPON":
			{
				float property = GetProperty("Actor_SkillSPCostDelta");
				float property2 = GetProperty("Actor_SkillSPCostRatio");
				result = ((float)_skillInfoMap[skillName].costSP + property) * (1f + property2);
				break;
			}
			}
			return result;
		}

		public float GetSkillCD(string skillName)
		{
			float num = _skillInfoMap[skillName].CD;
			if (skillName == "SKL01")
			{
				num *= 1f + GetProperty("Actor_SKL01CDRatio");
			}
			else if (skillName == "SKL02")
			{
				num *= 1f + GetProperty("Actor_SKL02CDRatio");
			}
			return num;
		}

		public float GetSwtichCDRatio()
		{
			return _switchInTimer.timer / _switchInTimer.timespan;
		}

		public string GetSkillNameByAnimEventID(string animEventID)
		{
			return (!animEventID.StartsWith("ATK")) ? animEventID : "ATK";
		}

		public SKillInfo GetSkillInfo(string skillName)
		{
			return (!_skillInfoMap.ContainsKey(skillName)) ? null : _skillInfoMap[skillName];
		}

		public bool IsOnStage()
		{
			return _isOnStage;
		}

		public void SetAvatarAttackRatio(float ratio)
		{
			attack = (float)attack * ratio;
		}

		protected override void OnAbilityStateAdd(AbilityState state, bool muteDisplayEffect)
		{
			base.OnAbilityStateAdd(state, muteDisplayEffect);
			switch (state)
			{
			case AbilityState.Stun:
				avatar.SetTrigger("TriggerHit");
				avatar.SetLocomotionBool("BuffStun", true);
				_stunMassRatioIx = PushProperty("Entity_MassRatio", 1000f);
				break;
			case AbilityState.Paralyze:
				avatar.SetTrigger("TriggerHit");
				avatar.SetLocomotionBool("BuffParalyze", true);
				_paralyzeAnimatorSpeedIx = avatar.PushProperty("Animator_OverallSpeedRatioMultiplied", 1f);
				avatar.SetCountedMuteControl(true);
				_paralyzeMassRatioIx = PushProperty("Entity_MassRatio", 1000f);
				_paralyzeTimer.Reset(true);
				_paralyzeTimer.timespan = 0.35f;
				_paralyzeState = ParalyzeState.ParalyzeHitResuming;
				avatar.OrderMove = false;
				avatar.ClearAttackTriggers();
				break;
			case AbilityState.Frozen:
				avatar.SetLocomotionBool("BuffParalyze", true);
				_freezeAnimatorSpeedIx = avatar.PushProperty("Animator_OverallSpeedRatioMultiplied", 0f);
				avatar.SetCountedMuteControl(true);
				_freezeMassRatioIx = PushProperty("Entity_MassRatio", 1000f);
				break;
			case AbilityState.Tied:
				_tiedState = TiedState.Tieing;
				break;
			}
			Singleton<EventManager>.Instance.FireEvent(new EvtBuffAdd(runtimeID, state));
		}

		protected override void OnAbilityStateRemove(AbilityState state)
		{
			base.OnAbilityStateRemove(state);
			switch (state)
			{
			case AbilityState.Stun:
				avatar.SetLocomotionBool("BuffStun", false);
				PopProperty("Entity_MassRatio", _stunMassRatioIx);
				avatar.ClearAttackTriggers();
				break;
			case AbilityState.Paralyze:
				avatar.SetLocomotionBool("BuffParalyze", false);
				avatar.PopProperty("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx);
				avatar.SetCountedMuteControl(false);
				PopProperty("Entity_MassRatio", _paralyzeMassRatioIx);
				_paralyzeState = ParalyzeState.Idle;
				avatar.OrderMove = false;
				avatar.ClearAttackTriggers();
				break;
			case AbilityState.Frozen:
				avatar.SetLocomotionBool("BuffParalyze", false);
				avatar.PopProperty("Animator_OverallSpeedRatioMultiplied", _freezeAnimatorSpeedIx);
				avatar.SetCountedMuteControl(false);
				avatar.PopProperty("Entity_MassRatio", _freezeMassRatioIx);
				avatar.ClearAttackTriggers();
				break;
			case AbilityState.Tied:
				_tiedState = TiedState.Idle;
				break;
			}
			Singleton<EventManager>.Instance.FireEvent(new EvtBuffRemove(runtimeID, state));
		}

		private void UpdateAbilityState()
		{
			if (_paralyzeState == ParalyzeState.WaitForGrounded)
			{
				if (!avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.Throw))
				{
					avatar.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx, 0f);
					_paralyzeState = ParalyzeState.ParalyzeFreezed;
				}
			}
			else if (_paralyzeState != ParalyzeState.ParalyzeFreezed && _paralyzeState == ParalyzeState.ParalyzeHitResuming)
			{
				_paralyzeTimer.Core(1f);
				if (avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.Throw))
				{
					avatar.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx, 1f);
					_paralyzeTimer.Reset(false);
					_paralyzeState = ParalyzeState.WaitForGrounded;
				}
				else if (_paralyzeTimer.isTimeUp)
				{
					avatar.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx, 0f);
					_paralyzeTimer.Reset(false);
					_paralyzeState = ParalyzeState.ParalyzeFreezed;
				}
			}
		}
	}
}
