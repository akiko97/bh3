using System.Collections;
using System.Collections.Generic;
using FullInspector;
using LuaInterface;
using MoleMole.Config;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class MonsterActor : BaseAbilityActor
	{
		private enum ParalyzeState
		{
			Idle = 0,
			WaitingForGrounded = 1,
			ParalyzeFreezed = 2,
			ParalyzeHitResuming = 3
		}

		private enum FrozenState
		{
			Idle = 0,
			WaitingForGrounded = 1,
			FrozenFreezed = 2
		}

		public const float GOODS_DROP_MAX_DISTANCE = 2f;

		public BaseMonoMonster monster;

		public bool isElite;

		public uint uniqueMonsterID;

		[InspectorCollapsedFoldout]
		public ConfigMonster config;

		public MonsterConfigMetaData metaConfig;

		public List<LDDropDataItem> dropDataItems;

		public bool needDropReward = true;

		public bool showSubHpBarWhenAttackLanded;

		public float avatarExpReward;

		public float scoinReward;

		private ParalyzeState _paralyzeState;

		private EntityTimer _paralyzeTimer;

		private FrozenState _frozenState;

		private int _paralyzeAnimatorSpeedIx;

		private int _frozenAnmatorSpeedIx;

		public override void Init(BaseMonoEntity entity)
		{
			monster = (BaseMonoMonster)entity;
			runtimeID = monster.GetRuntimeID();
			uniqueMonsterID = monster.uniqueMonsterID;
			string configType = string.Empty;
			if (uniqueMonsterID != 0)
			{
				UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
				configType = uniqueMonsterMetaData.configType;
			}
			config = MonsterData.GetMonsterConfig(monster.MonsterName, monster.TypeName, configType);
			commonConfig = config.CommonConfig;
			metaConfig = MonsterData.GetMonsterConfigMetaData(monster.MonsterName, monster.TypeName);
			base.Init(entity);
			Singleton<EventManager>.Instance.FireEvent(new EvtMonsterCreated(runtimeID));
			_paralyzeTimer = new EntityTimer();
			_paralyzeTimer.SetActive(false);
			_paralyzeState = ParalyzeState.Idle;
			AddPlugin(new MonsterAIPlugin(this));
			InitAbilityStateImmune();
			InitDebuffDurationRatio();
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

		public void InitLevelData(int level, bool isElite)
		{
			base.level = level;
			NPCLevelMetaData nPCLevelMetaDataByKey = NPCLevelMetaDataReader.GetNPCLevelMetaDataByKey(level);
			baseMaxHP = (maxHP = (HP = metaConfig.HP * nPCLevelMetaDataByKey.HPRatio));
			defense = metaConfig.defense * nPCLevelMetaDataByKey.DEFRatio;
			attack = metaConfig.attack * nPCLevelMetaDataByKey.ATKRatio;
			PushProperty("Actor_ResistAllElementAttackRatio", nPCLevelMetaDataByKey.ElementalResistRatio);
			this.isElite = isElite;
			if (isElite)
			{
				baseMaxHP = (maxHP = (HP = (float)maxHP * config.EliteArguments.HPRatio));
				defense = (float)defense * config.EliteArguments.DefenseRatio;
				attack = (float)attack * config.EliteArguments.AttackRatio;
			}
			foreach (KeyValuePair<string, ConfigEntityAbilityEntry> ability in config.Abilities)
			{
				abilityIDMap.Add(ability.Key, ability.Value.AbilityName);
				if (ability.Value.AbilityName != "Noop")
				{
					appliedAbilities.Add(Tuple.Create(AbilityData.GetAbilityConfig(ability.Value.AbilityName, ability.Value.AbilityOverride), AbilityData.EMPTY_OVERRIDE_MAP));
				}
			}
			InitUniqueMonsterConfig(nPCLevelMetaDataByKey);
		}

		private void InitUniqueMonsterConfig(NPCLevelMetaData npcLevelMetaData)
		{
			if (uniqueMonsterID == 0)
			{
				return;
			}
			UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
			baseMaxHP = (maxHP = (HP = metaConfig.HP * uniqueMonsterMetaData.HPRatio * npcLevelMetaData.HPRatio));
			defense = metaConfig.defense * uniqueMonsterMetaData.defenseRatio * npcLevelMetaData.DEFRatio;
			attack = metaConfig.attack * uniqueMonsterMetaData.attackRatio * npcLevelMetaData.ATKRatio;
			if (uniqueMonsterMetaData.abilities.Length <= 0)
			{
				return;
			}
			LuaState luaState = new LuaState();
			object[] array = luaState.DoString(uniqueMonsterMetaData.abilities);
			LuaTable luaTable = (LuaTable)array[0];
			foreach (DictionaryEntry item2 in luaTable)
			{
				string abilityName = (string)item2.Key;
				LuaTable luaTable2 = (LuaTable)item2.Value;
				string monsterName = uniqueMonsterMetaData.monsterName;
				ConfigAbility item = ((monsterName != null) ? AbilityData.GetAbilityConfig(abilityName, monsterName) : AbilityData.GetAbilityConfig(abilityName));
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				foreach (DictionaryEntry item3 in luaTable2)
				{
					string key = (string)item3.Key;
					if (item3.Value is double)
					{
						dictionary.Add(key, (float)(double)item3.Value);
					}
					else if (item3.Value is string)
					{
						dictionary.Add(key, (string)item3.Value);
					}
				}
				appliedAbilities.Add(Tuple.Create(item, dictionary));
			}
		}

		public void EnableWarningFieldActor(float warningRadius, float escapeRadius)
		{
			MonsterAIPlugin plugin = GetPlugin<MonsterAIPlugin>();
			if (plugin != null)
			{
				plugin.InitWarningField(warningRadius, escapeRadius);
			}
		}

		public override bool OnEventWithPlugins(BaseEvent evt)
		{
			bool result = base.OnEventWithPlugins(evt);
			if (evt is EvtHittingOther)
			{
				return OnHittingOther((EvtHittingOther)evt);
			}
			if (evt is EvtBeingHit)
			{
				return OnBeingHit((EvtBeingHit)evt);
			}
			if (evt is EvtAttackLanded)
			{
				return OnAttackLanded((EvtAttackLanded)evt);
			}
			if (evt is EvtDamageLanded)
			{
				return OnDamageLanded((EvtDamageLanded)evt);
			}
			if (evt is EvtKilled)
			{
				return OnKill((EvtKilled)evt);
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
			return flag | base.OnEventResolves(evt);
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			return base.ListenEvent(evt);
		}

		private bool OnHittingOther(EvtHittingOther evt)
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
				BaseActor actor = Singleton<EventManager>.Instance.GetActor(evt.toID);
				if (actor != null)
				{
					BaseMonoEntity baseMonoEntity = Singleton<EventManager>.Instance.GetEntity(evt.toID);
					evt.hitCollision = new AttackResult.HitCollsion
					{
						hitPoint = baseMonoEntity.GetAttachPoint("RootNode").position,
						hitDir = baseMonoEntity.XZPosition - monster.XZPosition
					};
				}
			}
			return true;
		}

		private bool OnHittingOtherResolve(EvtHittingOther evt)
		{
			evt.Resolve();
			Singleton<EventManager>.Instance.FireEvent(new EvtBeingHit(evt.toID, runtimeID, evt.animEventID, evt.attackData));
			MarkImportantEventIsHandled(evt);
			return true;
		}

		protected virtual bool OnBeingHit(EvtBeingHit evt)
		{
			DamageModelLogic.ResolveAttackDataByAttackee(this, evt.attackData);
			return true;
		}

		protected virtual bool OnBeingHitResolve(EvtBeingHit evt)
		{
			evt.Resolve();
			if (evt.attackData.rejected)
			{
				if (evt.attackData.rejectState == AttackResult.RejectType.RejectButShowAttackEffect)
				{
					AmendHitCollision(evt.attackData);
					FireAttackDataEffects(evt.attackData);
				}
				return false;
			}
			if (!isAlive || evt.attackData.GetTotalDamage() > (float)HP)
			{
				evt.attackData.attackeeAniDefenceRatio = 0f;
			}
			AttackResult attackResult = DamageModelLogic.ResolveAttackDataFinal(this, evt.attackData);
			AmendHitCollision(attackResult);
			if ((bool)isAlive)
			{
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
					attackResult.hitLevel = AttackResult.ActorHitLevel.Normal;
					attackResult.hitEffectPattern = AttackResult.HitEffectPattern.OnlyBeHit;
					attackResult.attackCameraShake = null;
					attackResult.killEffect = KillEffect.KillNow;
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
						BeingHit(attackResult, BeHitEffect.NormalBeHit);
					}
					else
					{
						if (attackResult.killEffect != KillEffect.KillTillHitAnimationEnd)
						{
							if (monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw) || evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowUp || evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowUpBlow)
							{
								attackResult.killEffect = KillEffect.KillFastWithNormalAnim;
							}
							else if ((abilityState & AbilityState.WitchTimeSlowed) != AbilityState.None || attackResult.aniDamageRatio >= 0.9f)
							{
								attackResult.killEffect = KillEffect.KillFastWithDieAnim;
							}
						}
						Kill(evt.sourceID, evt.animEventID, attackResult.killEffect);
						evt.beHitEffect = BeHitEffect.KillingBeHit;
						BeingHit(attackResult, BeHitEffect.KillingBeHit);
					}
				}
				else
				{
					evt.beHitEffect = BeHitEffect.NormalBeHit;
					BeingHit(attackResult, BeHitEffect.NormalBeHit);
				}
			}
			else
			{
				evt.beHitEffect = BeHitEffect.OverkillBeHit;
				BeingHit(attackResult, BeHitEffect.OverkillBeHit);
			}
			FireAttackDataEffects(attackResult);
			if (evt.attackData.isAnimEventAttack)
			{
				EvtAttackLanded evtAttackLanded = new EvtAttackLanded(evt.sourceID, runtimeID, evt.animEventID, attackResult);
				Singleton<EventManager>.Instance.FireEvent(evtAttackLanded);
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AttackLanded, evtAttackLanded));
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
					hitPoint = monster.RootNodePosition,
					hitDir = -monster.FaceDirection
				};
			}
			else if (attackResult.hitCollision.hitDir == Vector3.zero)
			{
				attackResult.hitCollision.hitPoint = monster.RootNodePosition;
				attackResult.hitCollision.hitDir = -monster.FaceDirection;
			}
		}

		public void FireAttackDataEffects(AttackResult attackResult)
		{
			if (attackResult.attackEffectPattern != null && (attackResult.hitEffectPattern == AttackResult.HitEffectPattern.Normal || attackResult.hitEffectPattern == AttackResult.HitEffectPattern.OnlyAttack))
			{
				AttackPattern.ActAttackEffects(attackResult.attackEffectPattern, monster, attackResult.hitCollision.hitPoint, attackResult.hitCollision.hitDir);
			}
			if (attackResult.beHitEffectPattern != null && (attackResult.hitEffectPattern == AttackResult.HitEffectPattern.Normal || attackResult.hitEffectPattern == AttackResult.HitEffectPattern.OnlyBeHit))
			{
				AttackPattern.ActAttackEffects(attackResult.beHitEffectPattern, monster, attackResult.hitCollision.hitPoint, attackResult.hitCollision.hitDir);
			}
			if (attackResult.attackCameraShake != null)
			{
				AttackPattern.ActCameraShake(attackResult.attackCameraShake);
			}
		}

		private bool OnAttackLanded(EvtAttackLanded evt)
		{
			AttackLanded(evt);
			MarkImportantEventIsHandled(evt);
			return true;
		}

		private bool OnDamageLanded(EvtDamageLanded evt)
		{
			DamageLanded(evt);
			return true;
		}

		private bool OnKill(EvtKilled evt)
		{
			RemovePlugin<MonsterAIPlugin>();
			return true;
		}

		public override void OnRemoval()
		{
			base.OnRemoval();
			if (Singleton<LevelManager>.Instance == null || Singleton<LevelManager>.Instance.levelActor.levelState != LevelActor.LevelState.LevelRunning)
			{
				return;
			}
			if (dropDataItems != null && needDropReward)
			{
				foreach (LDDropDataItem dropDataItem in dropDataItems)
				{
					dropDataItem.CreateDropGoods(monster.GetDropPosition(), Vector3.forward);
				}
			}
			Singleton<LevelScoreManager>.Instance.avatarExpInside += avatarExpReward;
		}

		public override void Core()
		{
			base.Core();
			UpdateAbilityState();
		}

		protected virtual void AttackLanded(EvtAttackLanded evt)
		{
			if (!evt.attackResult.isFromBullet)
			{
				monster.FrameHalt(evt.attackResult.frameHalt);
			}
		}

		protected virtual void DamageLanded(EvtDamageLanded evt)
		{
			monster.FrameHalt(evt.attackResult.frameHalt);
		}

		public virtual void BeingHit(AttackResult attackResult, BeHitEffect beHitEffect)
		{
			if (_paralyzeState == ParalyzeState.ParalyzeFreezed && attackResult.hitEffect > AttackResult.AnimatorHitEffect.Light)
			{
				monster.PopProperty("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx);
				_paralyzeAnimatorSpeedIx = monster.PushProperty("Animator_OverallSpeedRatioMultiplied", 1f);
				monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx, 1f);
				if (attackResult.hitEffect == AttackResult.AnimatorHitEffect.ThrowUp || attackResult.hitEffect == AttackResult.AnimatorHitEffect.ThrowUpBlow)
				{
					_paralyzeTimer.timespan = 0.5f;
				}
				else
				{
					_paralyzeTimer.timespan = 0.35f;
				}
				_paralyzeTimer.Reset(true);
				_paralyzeState = ParalyzeState.ParalyzeHitResuming;
			}
			if (abilityState.ContainsState(AbilityState.Frozen) && attackResult.hitEffect > AttackResult.AnimatorHitEffect.Light)
			{
				attackResult.hitEffect = AttackResult.AnimatorHitEffect.Light;
			}
			monster.BeHit(attackResult.frameHalt, attackResult.hitEffect, attackResult.hitEffectAux, attackResult.killEffect, beHitEffect, attackResult.aniDamageRatio, attackResult.hitCollision.hitDir, attackResult.retreatVelocity);
		}

		public virtual void Kill(uint killerID, string animEventID, KillEffect killEffect)
		{
			if (onJustKilled != null)
			{
				onJustKilled(killerID, null, killEffect);
			}
			if (!isAlive)
			{
				if (killEffect == KillEffect.KillImmediately)
				{
					monster.SetDied(KillEffect.KillImmediately);
				}
			}
			else
			{
				isAlive = false;
				Singleton<EventManager>.Instance.FireEvent(new EvtKilled(runtimeID, killerID, animEventID));
				monster.SetDied(killEffect);
			}
		}

		public virtual void ForceKill()
		{
			ForceKill(562036737u, KillEffect.KillNow);
		}

		public void ForceRemoveImmediatelly()
		{
			isAlive = false;
			needDropReward = false;
			monster.SetDied(KillEffect.KillImmediately);
		}

		public override void ForceKill(uint killerID, KillEffect killEffect)
		{
			Kill(killerID, null, killEffect);
		}

		public void SetMonsterHPRatio(float ratio)
		{
			baseMaxHP = (float)baseMaxHP * ratio;
			maxHP = (float)maxHP * ratio;
			HP = (float)HP * ratio;
		}

		public void SetMonsterAttackRatio(float ratio)
		{
			attack = (float)attack * ratio;
		}

		protected override void OnAbilityStateAdd(AbilityState state, bool muteDisplayEffect)
		{
			base.OnAbilityStateAdd(state, muteDisplayEffect);
			switch (state)
			{
			case AbilityState.Stun:
				monster.SetLocomotionBool("BuffStun", true);
				monster.SetCountedMuteControl(true);
				break;
			case AbilityState.Paralyze:
				monster.SetLocomotionBool("BuffParalyze", true);
				_paralyzeAnimatorSpeedIx = monster.PushProperty("Animator_OverallSpeedRatioMultiplied", 1f);
				_paralyzeTimer.Reset(true);
				_paralyzeTimer.timespan = 0.35f;
				_paralyzeState = ParalyzeState.ParalyzeHitResuming;
				monster.SetCountedMuteControl(true);
				break;
			case AbilityState.Frozen:
				if (monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
				{
					_frozenAnmatorSpeedIx = monster.PushProperty("Animator_OverallSpeedRatioMultiplied", 1f);
					_frozenState = FrozenState.WaitingForGrounded;
				}
				else
				{
					_frozenAnmatorSpeedIx = monster.PushProperty("Animator_OverallSpeedRatioMultiplied", 0f);
					_frozenState = FrozenState.FrozenFreezed;
				}
				monster.SetCountedMuteControl(true);
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
				monster.SetLocomotionBool("BuffStun", false);
				monster.SetCountedMuteControl(false);
				break;
			case AbilityState.Paralyze:
				monster.SetLocomotionBool("BuffParalyze", false);
				monster.PopProperty("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx);
				monster.SetCountedMuteControl(false);
				_paralyzeState = ParalyzeState.Idle;
				break;
			case AbilityState.Frozen:
				monster.PopProperty("Animator_OverallSpeedRatioMultiplied", _frozenAnmatorSpeedIx);
				_frozenState = FrozenState.Idle;
				monster.SetCountedMuteControl(false);
				break;
			}
			Singleton<EventManager>.Instance.FireEvent(new EvtBuffRemove(runtimeID, state));
		}

		private void UpdateAbilityState()
		{
			if (_paralyzeState == ParalyzeState.WaitingForGrounded)
			{
				if (monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Grounded))
				{
					if (abilityState.ContainsState(AbilityState.SlowWhenFrozenOrParalyze))
					{
						monster.PopProperty("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx);
						_paralyzeAnimatorSpeedIx = monster.PushProperty("Animator_OverallSpeedRatioMultiplied", 1f);
						monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx, 0.1f);
					}
					else
					{
						monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx, 0f);
					}
					_paralyzeState = ParalyzeState.ParalyzeFreezed;
				}
			}
			else if (_paralyzeState != ParalyzeState.ParalyzeFreezed && _paralyzeState == ParalyzeState.ParalyzeHitResuming)
			{
				_paralyzeTimer.Core(1f);
				if (!monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Grounded))
				{
					monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx, 1f);
					_paralyzeTimer.Reset(false);
					_paralyzeState = ParalyzeState.WaitingForGrounded;
				}
				else if (_paralyzeTimer.isTimeUp)
				{
					if (abilityState.ContainsState(AbilityState.SlowWhenFrozenOrParalyze))
					{
						monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx, 0.1f);
					}
					else
					{
						monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _paralyzeAnimatorSpeedIx, 0f);
					}
					_paralyzeTimer.Reset(false);
					_paralyzeState = ParalyzeState.ParalyzeFreezed;
				}
			}
			if (_frozenState == FrozenState.WaitingForGrounded)
			{
				if (monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Grounded))
				{
					if (abilityState.ContainsState(AbilityState.SlowWhenFrozenOrParalyze))
					{
						monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _frozenAnmatorSpeedIx, 0.1f);
					}
					else
					{
						monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _frozenAnmatorSpeedIx, 0f);
					}
					_frozenState = FrozenState.FrozenFreezed;
				}
			}
			else if (_frozenState == FrozenState.FrozenFreezed)
			{
				if (abilityState.ContainsState(AbilityState.SlowWhenFrozenOrParalyze))
				{
					monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _frozenAnmatorSpeedIx, 0.1f);
				}
				else
				{
					monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", _frozenAnmatorSpeedIx, 0f);
				}
			}
		}

		public void RefillAttackDataDamagePercentage(string animEventID, ref AttackData attackData)
		{
			if (uniqueMonsterID == 0 || SharedAnimEventData.IsSharedAnimEventID(animEventID))
			{
				return;
			}
			UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
			List<float> aTKRatios = uniqueMonsterMetaData.ATKRatios;
			List<List<string>> aTKRatioNames = config.ATKRatioNames;
			int num = -1;
			for (int i = 0; i < aTKRatioNames.Count; i++)
			{
				List<string> list = aTKRatioNames[i];
				foreach (string item in list)
				{
					if (item == animEventID)
					{
						num = i;
						break;
					}
				}
				if (num != -1)
				{
					break;
				}
			}
			if (num != -1 && num < aTKRatios.Count)
			{
				attackData.attackerAttackPercentage *= aTKRatios[num];
			}
		}
	}
}
