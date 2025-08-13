using System;
using System.Collections.Generic;
using System.IO;
using FullInspector;
using UnityEngine;

namespace MoleMole
{
	public class LevelDamageStasticsPlugin : BaseActorPlugin
	{
		public enum TargetType
		{
			AttackTarget = 0,
			LocalAvatar = 1
		}

		public const int SIMPLE_RESULT_NUM = 7;

		private LevelActor _levelActor;

		[ShowInInspector]
		private Dictionary<int, AvatarStastics> _avatarStasticsDict = new Dictionary<int, AvatarStastics>();

		[ShowInInspector]
		private Dictionary<MonsterKey, MonsterStastics> _monsterAverageStasticsDict = new Dictionary<MonsterKey, MonsterStastics>();

		[ShowInInspector]
		private PlayerStastics _playerStastics = new PlayerStastics();

		[ShowInInspector]
		private Dictionary<uint, MonsterStastics> _monsterStasticsDict = new Dictionary<uint, MonsterStastics>();

		private string resultStr = string.Empty;

		public int screenRotateTimes;

		public float stageTime;

		public float allDamage;

		public float monsterDamage;

		public float avatarDamage;

		public float avatarWeaponDdamage;

		public uint avatarAttackTimes;

		public Dictionary<string, uint> attackTimeList = new Dictionary<string, uint>();

		public uint monsterAttackTimes;

		public uint avatarBeingHitTimes;

		public uint monstersBeingHitTimes;

		public uint normalAttackTimes;

		public uint specialAttackTimes;

		public uint avatarEffectHitTimes;

		public uint avatarBreakTimes;

		public uint avatarBeingBreakTimes;

		public uint missTimes;

		public float spGet;

		public uint evadeTimes;

		public uint avatarSkill01Times;

		public uint avatarSkill02Times;

		public uint avatarActiveWeaponSkillTimes;

		public uint evadeEffectTimes;

		public uint evadeSuccessTimes;

		public bool isStageCreated;

		public bool isUpdating;

		private float _updateTimer;

		public List<string> basicInfoList = new List<string>();

		public List<string> extraInfoList = new List<string>();

		public List<float> simpleInfoList = new List<float>();

		private bool _isInit;

		public LevelDamageStasticsPlugin(LevelActor levelActor)
		{
			_levelActor = levelActor;
		}

		public void ControlDamageStastics(DamageStastcisControlType type)
		{
			switch (type)
			{
			case DamageStastcisControlType.DamageStasticsStart:
				ResetBasicPara();
				isUpdating = true;
				break;
			case DamageStastcisControlType.DamageStasticsEnd:
				ResetBasicPara();
				isUpdating = false;
				break;
			case DamageStastcisControlType.DamageStasticsPause:
				isUpdating = false;
				break;
			case DamageStastcisControlType.DamageStasticsResume:
				isUpdating = true;
				break;
			case DamageStastcisControlType.DamageStasticsResult:
				ShowResult();
				break;
			case DamageStastcisControlType.DamageStasticsStoreResult:
				StoreResult();
				break;
			case DamageStastcisControlType.DamageStasticsStoreResultShow:
				ShowStoreResult();
				break;
			}
		}

		public override void OnAdded()
		{
			Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackStart>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelBuffState>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtEvadeStart>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtEvadeSuccess>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtDefendStart>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtSkillStart>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtDefendSuccess>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarCreated>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapInEnd>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapOutStart>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(_levelActor.runtimeID);
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.onLevelComboChanged = (Action<int, int>)Delegate.Combine(levelActor.onLevelComboChanged, new Action<int, int>(OnLevelComboChanged));
		}

		public override void OnRemoved()
		{
			Singleton<EventManager>.Instance.RemoveEventListener<EvtBeingHit>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtAttackStart>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelBuffState>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtSkillStart>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtEvadeStart>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtEvadeSuccess>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtDefendStart>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtDefendSuccess>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarCreated>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarSwapInEnd>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarSwapOutStart>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(_levelActor.runtimeID);
			Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(_levelActor.runtimeID);
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
			actor.onSPChanged = (Action<float, float, float>)Delegate.Remove(actor.onSPChanged, new Action<float, float, float>(OnSPChanged));
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.onLevelComboChanged = (Action<int, int>)Delegate.Remove(levelActor.onLevelComboChanged, new Action<int, int>(OnLevelComboChanged));
		}

		private void OnHPChanged(float from, float to, float delta)
		{
			if (isUpdating)
			{
				BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
				AvatarStastics avatarStastics = GetAvatarStastics(localAvatar.GetRuntimeID());
				if (to > from)
				{
					avatarStastics.hpGain = (float)avatarStastics.hpGain + (to - from);
				}
				if ((float)avatarStastics.hpMax < to)
				{
					avatarStastics.hpMax = to;
				}
			}
		}

		private void OnSPChanged(float from, float to, float delta)
		{
			if (!isUpdating)
			{
				return;
			}
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			AvatarStastics avatarStastics = GetAvatarStastics(localAvatar.GetRuntimeID());
			if (to > from)
			{
				avatarStastics.SpRecover = (float)avatarStastics.SpRecover + (to - from);
				if (to - from <= (float)AvatarStastics.SELF_SP_RECOVE_UPBOUND)
				{
					avatarStastics.selfSPRecover = (float)avatarStastics.selfSPRecover + (to - from);
				}
				spGet += to - from;
			}
			else
			{
				avatarStastics.spUse = (float)avatarStastics.spUse + (from - to);
			}
			if ((float)avatarStastics.spMax < to)
			{
				avatarStastics.spMax = to;
			}
		}

		private void OnLevelComboChanged(int from, int to)
		{
			int num = to + 1;
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			AvatarStastics avatarStastics = GetAvatarStastics(localAvatar.GetRuntimeID());
			if (avatarStastics != null && (float)avatarStastics.comboMax < (float)num)
			{
				avatarStastics.comboMax = num;
			}
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			if (Singleton<LevelManager>.Instance.IsPaused())
			{
				return false;
			}
			if (evt is EvtStageReady)
			{
				return ListenStageReady((EvtStageReady)evt);
			}
			if (evt is EvtBeingHit)
			{
				return ListenBeingHit((EvtBeingHit)evt);
			}
			if (evt is EvtAttackStart)
			{
				return ListenAttackStart((EvtAttackStart)evt);
			}
			if (evt is EvtLevelBuffState)
			{
				return ListenLevelBuffState((EvtLevelBuffState)evt);
			}
			if (evt is EvtEvadeStart)
			{
				return ListenEvadeStart((EvtEvadeStart)evt);
			}
			if (evt is EvtDefendStart)
			{
				return ListenDefendStart((EvtDefendStart)evt);
			}
			if (evt is EvtEvadeSuccess)
			{
				return ListenEvadeSuccess((EvtEvadeSuccess)evt);
			}
			if (evt is EvtDefendSuccess)
			{
				return ListenDefendSuccess((EvtDefendSuccess)evt);
			}
			if (evt is EvtSkillStart)
			{
				return ListenSkillStart((EvtSkillStart)evt);
			}
			if (evt is EvtAvatarCreated)
			{
				return ListenAvatarCreated((EvtAvatarCreated)evt);
			}
			if (evt is EvtAvatarSwapInEnd)
			{
				return ListenAvatarSwapInEnd((EvtAvatarSwapInEnd)evt);
			}
			if (evt is EvtAvatarSwapOutStart)
			{
				return ListenAvatarSwapOutStart((EvtAvatarSwapOutStart)evt);
			}
			if (evt is EvtMonsterCreated)
			{
				ListenMonsterCreated((EvtMonsterCreated)evt);
			}
			else if (evt is EvtKilled)
			{
				ListenKilled((EvtKilled)evt);
			}
			return false;
		}

		private bool ListenAvatarSwapOutStart(EvtAvatarSwapOutStart evt)
		{
			AvatarStastics avatarStastics = GetAvatarStastics(evt.targetID);
			if (avatarStastics == null)
			{
				return true;
			}
			++avatarStastics.swapOutTimes;
			avatarStastics.isOnStage = false;
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
			actor.onSPChanged = (Action<float, float, float>)Delegate.Remove(actor.onSPChanged, new Action<float, float, float>(OnSPChanged));
			return true;
		}

		private bool ListenAvatarSwapInEnd(EvtAvatarSwapInEnd evt)
		{
			AvatarStastics avatarStastics = GetAvatarStastics(evt.targetID);
			if (avatarStastics == null)
			{
				return true;
			}
			++avatarStastics.swapInTimes;
			avatarStastics.isOnStage = true;
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
			actor.onSPChanged = (Action<float, float, float>)Delegate.Combine(actor.onSPChanged, new Action<float, float, float>(OnSPChanged));
			return true;
		}

		private bool ListenAvatarCreated(EvtAvatarCreated evt)
		{
			if (Singleton<AvatarManager>.Instance.IsPlayerAvatar(evt.avatarID))
			{
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.avatarID);
				actor.onHPChanged = (Action<float, float, float>)Delegate.Combine(actor.onHPChanged, new Action<float, float, float>(OnHPChanged));
				actor.onSPChanged = (Action<float, float, float>)Delegate.Combine(actor.onSPChanged, new Action<float, float, float>(OnSPChanged));
				AvatarStastics avatarStastics = GetAvatarStastics(evt.avatarID);
				avatarStastics.hpBegin = actor.HP;
				avatarStastics.spBegin = actor.SP;
				avatarStastics.hpMax = actor.maxHP;
				avatarStastics.spMax = actor.maxSP;
			}
			return true;
		}

		private bool ListenStageReady(EvtStageReady evt)
		{
			if (!isStageCreated && _playerStastics != null)
			{
				_playerStastics.ResetPlayerStasticsData();
			}
			isStageCreated = true;
			return true;
		}

		private bool ListenSkillStart(EvtSkillStart evt)
		{
			if (!isUpdating)
			{
				return true;
			}
			ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID);
			if (num == 3)
			{
				AvatarStastics avatarStastics = GetAvatarStastics(evt.targetID);
				if (avatarStastics == null)
				{
					return true;
				}
				if (evt.skillID == "SKL01")
				{
					++avatarStastics.avatarSkill01Times;
					++avatarStastics.avatarEvadeEffectTimes;
					avatarSkill01Times++;
				}
				else if (evt.skillID == "SKL02")
				{
					++avatarStastics.avatarSkill02Times;
					avatarSkill02Times++;
				}
				else if (evt.skillID == "SKL_WEAPON")
				{
					++avatarStastics.avatarActiveWeaponSkillTimes;
					avatarActiveWeaponSkillTimes++;
				}
			}
			return true;
		}

		private bool ListenEvadeStart(EvtEvadeStart evt)
		{
			if (isUpdating)
			{
				AvatarStastics avatarStastics = GetAvatarStastics(evt.targetID);
				if (avatarStastics != null)
				{
					++avatarStastics.avatarEvadeTimes;
				}
			}
			return true;
		}

		private bool ListenDefendStart(EvtDefendStart evt)
		{
			if (isUpdating)
			{
				AvatarStastics avatarStastics = GetAvatarStastics(evt.targetID);
				if (avatarStastics != null)
				{
					++avatarStastics.avatarEvadeTimes;
				}
			}
			return true;
		}

		private bool ListenEvadeSuccess(EvtEvadeSuccess evt)
		{
			if (isUpdating)
			{
				AvatarStastics avatarStastics = GetAvatarStastics(evt.targetID);
				if (avatarStastics != null)
				{
					++avatarStastics.avatarEvadeSuccessTimes;
				}
				evadeSuccessTimes++;
			}
			return true;
		}

		private bool ListenDefendSuccess(EvtDefendSuccess evt)
		{
			if (isUpdating)
			{
				AvatarStastics avatarStastics = GetAvatarStastics(evt.targetID);
				if (avatarStastics != null)
				{
					++avatarStastics.avatarEvadeSuccessTimes;
				}
				evadeSuccessTimes++;
			}
			return true;
		}

		private bool ListenLevelBuffState(EvtLevelBuffState evt)
		{
			if (isUpdating && evt.levelBuff == LevelBuffType.WitchTime)
			{
				AvatarStastics avatarStastics = GetAvatarStastics(evt.sourceId);
				if (avatarStastics != null)
				{
				}
				evadeEffectTimes++;
			}
			return true;
		}

		private bool ListenAttackStart(EvtAttackStart evt)
		{
			if (isUpdating)
			{
				switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID))
				{
				case 3:
				{
					avatarAttackTimes++;
					if (attackTimeList.ContainsKey(evt.skillID))
					{
						Dictionary<string, uint> dictionary2;
						Dictionary<string, uint> dictionary = (dictionary2 = attackTimeList);
						string skillID;
						string key = (skillID = evt.skillID);
						uint num = dictionary2[skillID];
						dictionary[key] = num + 1;
					}
					else
					{
						attackTimeList[evt.skillID] = 1u;
					}
					AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.targetID);
					if (actor == null || !actor.config.Skills.ContainsKey(evt.skillID) || actor.config.Skills[evt.skillID].SkillCategoryTag == null)
					{
						break;
					}
					for (int i = 0; i < actor.config.Skills[evt.skillID].SkillCategoryTag.Length; i++)
					{
						if (actor.config.Skills[evt.skillID].SkillCategoryTag[i] == AttackResult.AttackCategoryTag.Branch || actor.config.Skills[evt.skillID].SkillCategoryTag[i] == AttackResult.AttackCategoryTag.Charge)
						{
							AvatarStastics avatarStastics = GetAvatarStastics(evt.targetID);
							if (avatarStastics != null)
							{
								++avatarStastics.avatarSpecialAttackTimes;
							}
							specialAttackTimes++;
							break;
						}
					}
					break;
				}
				case 4:
					monsterAttackTimes++;
					break;
				}
			}
			return true;
		}

		private bool ListenBeingHit(EvtBeingHit evt)
		{
			if (evt.attackData.rejected)
			{
				return false;
			}
			if (isUpdating)
			{
				if (!evt.attackData.IsFinalResolved())
				{
					return false;
				}
				if (!evt.attackData.isAnimEventAttack)
				{
					return false;
				}
				allDamage += evt.attackData.GetTotalDamage();
				ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID);
				ushort num2 = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.sourceID);
				switch (num)
				{
				case 3:
				{
					if (num2 == 4)
					{
						MonsterStastics monsterStastics = GetMonsterStastics(evt.sourceID);
						if (monsterStastics != null)
						{
							monsterStastics.damage = (float)monsterStastics.damage + evt.attackData.GetTotalDamage();
							++monsterStastics.hitAvatarTimes;
							if (evt.attackData.hitEffect > AttackResult.AnimatorHitEffect.Light)
							{
								++monsterStastics.breakAvatarTimes;
							}
						}
					}
					AvatarStastics avatarStastics2 = GetAvatarStastics(evt.targetID);
					if (avatarStastics2 == null)
					{
						return true;
					}
					avatarStastics2.avatarBeDamaged = (float)avatarStastics2.avatarBeDamaged + evt.attackData.GetTotalDamage();
					++avatarStastics2.avatarBeingHitTimes;
					if (evt.attackData.hitEffect > AttackResult.AnimatorHitEffect.Light)
					{
						++avatarStastics2.avatarBeingBreakTimes;
					}
					monsterDamage += evt.attackData.GetTotalDamage();
					avatarBeingHitTimes++;
					if (evt.attackData.attackerAniDamageRatio > evt.attackData.attackeeAniDefenceRatio)
					{
						avatarBeingHitTimes++;
					}
					if (evt.attackData.hitLevel == AttackResult.ActorHitLevel.Normal)
					{
						avatarStastics2.behitNormalDamageMax = Mathf.Max(evt.attackData.GetTotalDamage(), avatarStastics2.behitNormalDamageMax);
					}
					else if (evt.attackData.hitLevel == AttackResult.ActorHitLevel.Critical)
					{
						avatarStastics2.behitCriticalDamageMax = Mathf.Max(evt.attackData.GetTotalDamage(), avatarStastics2.behitCriticalDamageMax);
					}
					break;
				}
				case 4:
				{
					MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
					if (num2 == 3)
					{
						AvatarStastics avatarStastics = GetAvatarStastics(evt.sourceID);
						if (avatarStastics == null)
						{
							return true;
						}
						float natureDamageBonusRatio = DamageModelLogic.GetNatureDamageBonusRatio(evt.attackData.attackerNature, evt.attackData.attackeeNature, actor);
						if (natureDamageBonusRatio > 1f)
						{
							avatarStastics.restrictionDamage = (float)avatarStastics.restrictionDamage + evt.attackData.GetTotalDamage();
						}
						else if (natureDamageBonusRatio < 1f)
						{
							avatarStastics.beRestrictedDamage = (float)avatarStastics.beRestrictedDamage + evt.attackData.GetTotalDamage();
						}
						else if (Mathf.Approximately(natureDamageBonusRatio, 1f))
						{
							avatarStastics.normalDamage = (float)avatarStastics.normalDamage + evt.attackData.GetTotalDamage();
						}
						if (evt.attackData.attackCategoryTag.ContainsTag(AttackResult.AttackCategoryTag.Weapon))
						{
							avatarStastics.avatarActiveWeaponSkillDamage = (float)avatarStastics.avatarActiveWeaponSkillDamage + evt.attackData.GetTotalDamage();
							avatarWeaponDdamage += evt.attackData.GetTotalDamage();
						}
						if (evt.attackData.hitEffect > AttackResult.AnimatorHitEffect.Light)
						{
							++avatarStastics.avatarBreakTimes;
						}
						avatarStastics.avatarDamage = (float)avatarStastics.avatarDamage + evt.attackData.GetTotalDamage();
						++avatarStastics.avatarHitTimes;
						if (evt.attackData.isInComboCount)
						{
							++avatarStastics.avatarEffectHitTimes;
						}
						if (evt.attackData.hitLevel == AttackResult.ActorHitLevel.Normal)
						{
							avatarStastics.hitNormalDamageMax = Mathf.Max(evt.attackData.GetTotalDamage(), avatarStastics.hitNormalDamageMax);
						}
						else if (evt.attackData.hitLevel == AttackResult.ActorHitLevel.Critical)
						{
							avatarStastics.hitCriticalDamageMax = Mathf.Max(evt.attackData.GetTotalDamage(), avatarStastics.hitCriticalDamageMax);
						}
					}
					avatarDamage += evt.attackData.GetTotalDamage();
					if (evt.attackData.isInComboCount)
					{
						avatarEffectHitTimes++;
					}
					monstersBeingHitTimes++;
					if (evt.attackData.attackerAniDamageRatio > evt.attackData.attackeeAniDefenceRatio)
					{
						avatarBreakTimes++;
					}
					break;
				}
				}
			}
			return true;
		}

		private bool ListenMonsterCreated(EvtMonsterCreated evt)
		{
			GetMonsterStastics(evt.monsterID);
			return false;
		}

		private bool ListenKilled(EvtKilled evt)
		{
			switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID))
			{
			case 3:
			{
				AvatarStastics avatarStastics = GetAvatarStastics(evt.targetID);
				if (avatarStastics == null)
				{
					return true;
				}
				avatarStastics.isAlive = false;
				break;
			}
			case 4:
			{
				MonsterStastics monsterStastics = GetMonsterStastics(evt.targetID);
				if (monsterStastics != null)
				{
					monsterStastics.isAlive = false;
				}
				break;
			}
			}
			return false;
		}

		private void ResetBasicPara()
		{
			resultStr = string.Empty;
			allDamage = 0f;
			monsterDamage = 0f;
			avatarDamage = 0f;
			avatarBreakTimes = 0u;
			avatarBeingBreakTimes = 0u;
			avatarEffectHitTimes = 0u;
			missTimes = 0u;
			spGet = 0f;
			normalAttackTimes = 0u;
			specialAttackTimes = 0u;
			evadeTimes = 0u;
			evadeEffectTimes = 0u;
			screenRotateTimes = 0;
			stageTime = 0f;
			avatarAttackTimes = 0u;
			attackTimeList.Clear();
			monsterAttackTimes = 0u;
			avatarBeingHitTimes = 0u;
			monstersBeingHitTimes = 0u;
			avatarSkill01Times = 0u;
			avatarSkill02Times = 0u;
			_updateTimer = 0f;
		}

		public override void Core()
		{
			if (!_isInit)
			{
				InitDate();
			}
			if (isStageCreated && _playerStastics != null)
			{
				stageTime += Time.deltaTime;
				PlayerStastics playerStastics = _playerStastics;
				playerStastics.stageTime = (float)playerStastics.stageTime + Time.deltaTime;
			}
			if (!isUpdating)
			{
				return;
			}
			_updateTimer += Time.deltaTime;
			Dictionary<int, AvatarStastics>.Enumerator enumerator = _avatarStasticsDict.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					AvatarStastics value = enumerator.Current.Value;
					if ((bool)value.isAlive && (bool)value.isOnStage)
					{
						value.battleTime = (float)value.battleTime + Time.deltaTime;
						value.onStageTime = (float)value.onStageTime + Time.deltaTime;
					}
				}
			}
			finally
			{
				enumerator.Dispose();
			}
			Dictionary<uint, MonsterStastics>.Enumerator enumerator2 = _monsterStasticsDict.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					MonsterStastics value2 = enumerator2.Current.Value;
					if (value2.isAlive)
					{
						value2.aliveTime = (float)value2.aliveTime + Time.deltaTime;
					}
				}
			}
			finally
			{
				enumerator.Dispose();
			}
		}

		public void ShowResult()
		{
			if (Singleton<LevelScoreManager>.Instance.useDebugFunction)
			{
				return;
			}
			List<AvatarStastics> list = new List<AvatarStastics>();
			List<MonsterStastics> list2 = new List<MonsterStastics>();
			foreach (KeyValuePair<int, AvatarStastics> item in _avatarStasticsDict)
			{
				AvatarStastics value = item.Value;
				value.dps = ((!((float)value.battleTime > 0f)) ? 0f : ((float)value.avatarDamage / (float)value.battleTime));
				value.restrictionDamageRatio = ((!((float)value.avatarDamage > 0f)) ? 0f : ((float)value.restrictionDamage / (float)value.avatarDamage));
				value.beRestrictedDamageRatio = ((!((float)value.avatarDamage > 0f)) ? 0f : ((float)value.beRestrictedDamage / (float)value.avatarDamage));
				value.normalDamageRatio = ((!((float)value.avatarDamage > 0f)) ? 0f : ((float)value.normalDamage / (float)value.avatarDamage));
				list.Add(value);
			}
			foreach (KeyValuePair<uint, MonsterStastics> item2 in _monsterStasticsDict)
			{
				MonsterStastics value2 = item2.Value;
				value2.dps = (float)value2.damage / (float)value2.aliveTime;
				if (!_monsterAverageStasticsDict.ContainsKey(value2.key))
				{
					_monsterAverageStasticsDict[value2.key] = new MonsterStastics(value2.key.monsterName, value2.key.configType, value2.key.level);
				}
				MonsterStastics monsterStastics = _monsterAverageStasticsDict[value2.key];
				++monsterStastics.monsterCount;
				monsterStastics.damage = (float)monsterStastics.damage + (float)value2.damage;
				monsterStastics.aliveTime = (float)monsterStastics.aliveTime + (float)value2.aliveTime;
				monsterStastics.hitAvatarTimes = (int)monsterStastics.hitAvatarTimes + (int)value2.hitAvatarTimes;
				monsterStastics.breakAvatarTimes = (int)monsterStastics.breakAvatarTimes + (int)value2.breakAvatarTimes;
				monsterStastics.dps = (float)monsterStastics.dps + (float)value2.dps;
			}
			foreach (KeyValuePair<MonsterKey, MonsterStastics> item3 in _monsterAverageStasticsDict)
			{
				MonsterStastics value3 = item3.Value;
				value3.damage = (float)value3.damage / (float)(int)value3.monsterCount;
				value3.aliveTime = (float)value3.aliveTime / (float)(int)value3.monsterCount;
				value3.hitAvatarTimes = (int)value3.hitAvatarTimes / (int)value3.monsterCount;
				value3.dps = (float)value3.dps / (float)(int)value3.monsterCount;
				list2.Add(value3);
			}
			PlayerStastics playerStastics = _playerStastics;
			Singleton<NetworkManager>.Instance.RequestStageInnerDataReport(list, list2, playerStastics);
		}

		public void StoreResult()
		{
			simpleInfoList.Add(_updateTimer);
			simpleInfoList.Add(avatarDamage);
			simpleInfoList.Add(avatarDamage / _updateTimer);
			simpleInfoList.Add(spGet);
			simpleInfoList.Add(avatarEffectHitTimes);
		}

		public void ShowStoreResult()
		{
			string text = string.Empty;
			for (int i = 0; i < simpleInfoList.Count; i++)
			{
				if (i % 7 == 0)
				{
					text += "\n";
				}
				text = text + simpleInfoList[i] + "\t";
			}
		}

		public void AddScreenRotateTimes()
		{
			screenRotateTimes++;
			if (isStageCreated && _playerStastics != null)
			{
				++_playerStastics.screenRotateTimes;
			}
		}

		public AvatarStastics GetAvatarStastics(uint avatarRuntimeID)
		{
			BaseMonoAvatar helperAvatar = Singleton<AvatarManager>.Instance.GetHelperAvatar();
			if (helperAvatar != null && helperAvatar.GetRuntimeID() == avatarRuntimeID)
			{
				return null;
			}
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatarRuntimeID);
			if (actor == null)
			{
				return null;
			}
			if (!_avatarStasticsDict.ContainsKey(actor.avatarDataItem.avatarID))
			{
				AvatarStastics avatarStastics = new AvatarStastics(actor.avatarDataItem.avatarID);
				avatarStastics.avatarLevel = actor.level;
				avatarStastics.avatarStar = actor.avatarDataItem.star;
				avatarStastics.stageID = Singleton<LevelScoreManager>.Instance.LevelId;
				_avatarStasticsDict[actor.avatarDataItem.avatarID] = avatarStastics;
			}
			return _avatarStasticsDict[actor.avatarDataItem.avatarID];
		}

		private MonsterStastics GetMonsterStastics(uint monsterRuntimeID)
		{
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(monsterRuntimeID);
			if (actor == null)
			{
				return null;
			}
			BaseMonoMonster monster = actor.monster;
			if (!_monsterStasticsDict.ContainsKey(monsterRuntimeID))
			{
				_monsterStasticsDict[monsterRuntimeID] = new MonsterStastics(monster.MonsterName, monster.TypeName, actor.level);
			}
			return _monsterStasticsDict[monsterRuntimeID];
		}

		private void InitDate()
		{
			BaseMonoAvatar baseMonoAvatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
			if (baseMonoAvatar == null)
			{
				return;
			}
			if (_levelActor.levelMode == LevelActor.Mode.Single)
			{
				AvatarStastics avatarStastics = GetAvatarStastics(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
				avatarStastics.isOnStage = true;
				++avatarStastics.swapInTimes;
			}
			else if (_levelActor.levelMode == LevelActor.Mode.Multi)
			{
				foreach (BaseMonoAvatar allPlayerAvatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
				{
					AvatarStastics avatarStastics2 = GetAvatarStastics(allPlayerAvatar.GetRuntimeID());
					avatarStastics2.isOnStage = true;
					++avatarStastics2.swapInTimes;
				}
			}
			_isInit = true;
		}

		private void PrintIntoFile(string sPath, string content, bool clean = true)
		{
			FileInfo fileInfo = new FileInfo(sPath);
			StreamWriter streamWriter = (fileInfo.Exists ? new StreamWriter(sPath, false) : fileInfo.CreateText());
			streamWriter.Write(content);
			streamWriter.Close();
			streamWriter.Dispose();
		}
	}
}
