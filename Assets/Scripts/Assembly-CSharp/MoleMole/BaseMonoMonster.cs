using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseMonoMonster : BaseMonoAnimatorEntity, IAIEntity, IFadeOff, IFrameHaltable, IRetreatable, IAttacker
	{
		private enum AttackSpeedState
		{
			Idle = 0,
			WaitingAttackTimeStart = 1,
			DuringAttackTime = 2,
			AttackTimeEnded = 3
		}

		public enum DestroyMode
		{
			SetToBeRemoved = 0,
			DeactivateOnly = 1
		}

		protected const string MOVE_SPEED_PARAM = "MoveSpeed";

		protected const string ORDER_MOVE_PARAM = "IsMove";

		protected const string MOVE_HORIZONTAL_PARAM = "IsMoveHorizontal";

		protected const string TRIGGER_HIT_PARAM = "HitTrigger";

		protected const string TRIGGER_THROW_PARAM = "ThrowTrigger";

		protected const string TRIGGER_THROW_DOWN_PARAM = "ThrowDownTrigger";

		protected const string TRIGGER_THROW_BLOW_PARAM = "ThrowBlowTrigger";

		protected const string DAMAGE_RATIO_PARAM = "DamageRatio";

		protected const string HIT_TIME_OFFSET_RATIO_PARAM = "HitTimeOffsetRatio";

		protected const string DIE_PARAM = "IsDead";

		protected const string TRIGGER_LIGHT_HIT_PARAM = "LightHitTrigger";

		protected const string IS_HEAVY_RETREAT = "IsHeavyRetreat";

		protected const string HIT_EFFECT_AUX_PARAM = "HitEffectAux";

		protected const string IS_STANDBY_WALK_STEER_PARAM = "IsStandByWalkSteer";

		protected const string ABS_MOVE_SPEED_PARAM = "AbsMoveSpeed";

		protected const string TRIGGER_LEVEL_END_WIN = "LevelWin";

		protected const string TRIGGER_LEVEL_END_LOSE = "LevelLose";

		protected const int MONSTER_BASE_SHADER_IX = 0;

		protected const int MONSTER_ELITE_SHADER_IX = 1;

		public Collider hitbox;

		public MonoBodyPartEntity[] bodyParts;

		private bool _isToBeRemove;

		[NonSerialized]
		public bool isStaticInScene;

		private bool _canMoveHorizontal;

		private string _currentSkillID;

		private float _inactiveTimer = -1f;

		private int _monsterTagID;

		private float _originalMass;

		protected RetreatPlugin _retreatPlugin;

		private bool _checkOutsideWall;

		public Action<BaseMonoEntity> onAttackTargetChanged;

		public Action<BaseMonoMonster> onDie;

		public Action<BaseMonoMonster, bool, bool> onHitStateChanged;

		public ConfigMonster config;

		protected BaseMonsterAIController _aiController;

		private bool _preloaded;

		private int _muteControlCount;

		private bool _isAlive;

		private BaseMonoEntity _attackTarget;

		private Coroutine _fastDieCoroutine;

		private Coroutine _waitHitDieCoroutine;

		protected float _standBySteerTime;

		private bool _hasSteeredThisFrame;

		private int _walkSteerStateHash;

		private bool _usingThrowMass;

		private float _moveSpeedRatio;

		private AttackSpeedState _attackSpeedState;

		private int _attackSpeedTimeScaleIx;

		private int _muteSteerIx;

		private Dictionary<int, string> _patternMap = new Dictionary<int, string>();

		private Transform _mainCameraTrans;

		private bool _usingTransparentShader;

		private int _noCollisionCount;

		private Coroutine _checkOutsideWallCoroutine;

		public DestroyMode destroyMode;

		public int MonsterTagID
		{
			get
			{
				return _monsterTagID;
			}
		}

		public override string CurrentSkillID
		{
			get
			{
				return _currentSkillID;
			}
		}

		public bool hasArmor { get; set; }

		public uint uniqueMonsterID { get; private set; }

		public string AIModeName
		{
			get
			{
				return config.StateMachinePattern.AIMode;
			}
		}

		public string MonsterName { get; private set; }

		public string TypeName { get; private set; }

		public BaseMonoEntity AttackTarget
		{
			get
			{
				return _attackTarget;
			}
		}

		public bool OrderMove
		{
			get
			{
				return GetLocomotionBool("IsMove");
			}
			set
			{
				SetLocomotionBool("IsMove", value, true);
			}
		}

		public bool MoveHorizontal
		{
			get
			{
				return _canMoveHorizontal && GetLocomotionBool("IsMoveHorizontal");
			}
			set
			{
				if (_canMoveHorizontal)
				{
					SetLocomotionBool("IsMoveHorizontal", value, true);
				}
			}
		}

		public float MoveSpeedRatio
		{
			set
			{
				float value2 = Mathf.Abs(value);
				float num = Mathf.Sign(value);
				float aniMinSpeedRatio = config.StateMachinePattern.AniMinSpeedRatio;
				float aniMaxSpeedRatio = config.StateMachinePattern.AniMaxSpeedRatio;
				_moveSpeedRatio = num * Mathf.Clamp(value2, aniMinSpeedRatio, aniMaxSpeedRatio);
				SyncAnimatorMoveSpeed();
			}
		}

		Transform IAttacker.transform
		{
			get
			{
				return base.transform;
			}
		}

		Vector3 IAttacker.FaceDirection
		{
			get
			{
				return base.FaceDirection;
			}
		}

		Vector3 IAIEntity.FaceDirection
		{
			get
			{
				return base.FaceDirection;
			}
		}

		Transform IAIEntity.transform
		{
			get
			{
				return base.transform;
			}
		}

		Vector3 IAIEntity.RootNodePosition
		{
			get
			{
				return base.RootNodePosition;
			}
		}

		public event AnimatedHitBoxCreatedHandler onAnimatedHitBoxCreatedCallBack;

		private void SetCurrentSKillID(string value)
		{
			if (onCurrentSkillIDChanged != null)
			{
				onCurrentSkillIDChanged(_currentSkillID, value);
			}
			_currentSkillID = value;
		}

		public bool isGoingToAttack(float deltaTime)
		{
			if (CurrentSkillID == null)
			{
				return false;
			}
			AnimatorStateInfo animatorStateInfo = ((!_animator.IsInTransition(0)) ? _animator.GetCurrentAnimatorStateInfo(0) : _animator.GetNextAnimatorStateInfo(0));
			AnimationClip animationClip = ((!_animator.IsInTransition(0)) ? _animator.GetCurrentAnimatorClipInfo(0)[0].clip : _animator.GetNextAnimatorClipInfo(0)[0].clip);
			AnimationEvent[] events = animationClip.events;
			if (events.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < events.Length; i++)
			{
				if (events[i].functionName == "AnimEventHandler" && events[i].stringParameter.IndexOf("Hint") < 0)
				{
					float num = animationClip.length * animatorStateInfo.normalizedTime;
					if (num > events[i].time - deltaTime && num < events[i].time)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void Awake()
		{
			base.Awake();
			_originalMass = _rigidbody.mass;
		}

		public void PreInit(string monsterName, string typeName, uint uniqueMonsterID = 0, bool disableBehaviorWhenInit = false)
		{
			MonsterName = monsterName;
			TypeName = typeName;
			this.uniqueMonsterID = uniqueMonsterID;
			string configType = string.Empty;
			UniqueMonsterMetaData uniqueMonsterMetaData = null;
			if (uniqueMonsterID != 0)
			{
				uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
				configType = uniqueMonsterMetaData.configType;
			}
			config = MonsterData.GetMonsterConfig(monsterName, typeName, configType);
			animatorConfig = config;
			commonConfig = config.CommonConfig;
			Init(0u);
			string empty = string.Empty;
			empty = ((uniqueMonsterMetaData == null) ? MonsterData.GetMonsterConfigMetaData(monsterName, typeName).AIName : uniqueMonsterMetaData.AIName);
			InitController(empty, disableBehaviorWhenInit);
			InitPlugins();
			InitSkillAnimatorEventPattern();
			AttachEffectOverrides();
			base.PostInit();
			InitDynamicBone();
			_preloaded = true;
		}

		public void Init(string monsterName, string typeName, uint runtimeID, Vector3 initPos, uint uniqueMonsterID = 0, string overrideAIName = null, bool checkOutsideWall = true, bool isElite = false, bool disableBehaviorWhenInit = false, int monsterTagID = 0)
		{
			if (!_preloaded)
			{
				MonsterName = monsterName;
				TypeName = typeName;
				this.uniqueMonsterID = uniqueMonsterID;
				string configType = string.Empty;
				if (uniqueMonsterID != 0)
				{
					UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
					configType = uniqueMonsterMetaData.configType;
				}
				config = MonsterData.GetMonsterConfig(monsterName, typeName, configType);
				animatorConfig = config;
				commonConfig = config.CommonConfig;
				Init(runtimeID);
			}
			else
			{
				_runtimeID = runtimeID;
			}
			_isAlive = true;
			_monsterTagID = monsterTagID;
			initPos.y += config.CommonArguments.CreatePosYOffset;
			LayerMask mask = (1 << InLevelData.AVATAR_LAYER) | (1 << InLevelData.MONSTER_LAYER);
			initPos = PickInitPosition(mask, initPos, config.CommonArguments.CollisionRadius);
			base.transform.position = initPos;
			_animEventPredicates.Add(config.CommonArguments.DefaultAnimEventPredicate);
			if (!_preloaded)
			{
				string empty = string.Empty;
				empty = ((overrideAIName != null) ? overrideAIName : ((uniqueMonsterID == 0) ? MonsterData.GetMonsterConfigMetaData(monsterName, typeName).AIName : MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID).AIName));
				InitController(empty, disableBehaviorWhenInit);
				InitPlugins();
				InitSkillAnimatorEventPattern();
				AttachEffectOverrides();
			}
			InitBodyParts();
			MoveSpeedRatio = 1f;
			_attackSpeedState = AttackSpeedState.Idle;
			_attackSpeedTimeScaleIx = PushProperty("Animator_OverallSpeedRatio", 0f);
			onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(onCurrentSkillIDChanged, new Action<string, string>(OnSkillIDChanged));
			onIsGhostChanged = (Action<bool>)Delegate.Combine(onIsGhostChanged, new Action<bool>(OnIsGhostChanged));
			RegisterPropertyChangedCallback("Entity_AttackSpeed", OnAttackSpeedChanged);
			_muteSteerIx = AddWaitTransitionState();
			_canMoveHorizontal = _animator.HasParameter("IsMoveHorizontal");
			PostInit();
			InitUniqueMonsterAndElite(isElite);
			ApplyAnimatorConfig();
			_mainCameraTrans = Singleton<CameraManager>.Instance.GetMainCamera().transform;
			if (GlobalVars.ENABLE_CONTINUOUS_DETECT_MODE)
			{
				_rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			}
			InitBornSound();
			_checkOutsideWall = checkOutsideWall;
			if (_checkOutsideWall)
			{
				StartCoroutine(CheckOutsideWall());
			}
		}

		protected override void PostInit()
		{
			if (!_preloaded)
			{
				base.PostInit();
				InitDynamicBone();
			}
			if (config.StateMachinePattern.UseStandByWalkSteer)
			{
				_walkSteerStateHash = Animator.StringToHash(config.StateMachinePattern.WalkSteerAnimatorStateName);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (config != null)
			{
				ApplyAnimatorProperties();
			}
		}

		protected override void ApplyAnimatorProperties()
		{
			base.ApplyAnimatorProperties();
			SyncAnimatorMoveSpeed();
		}

		private void InitSkillAnimatorEventPattern()
		{
			foreach (string key in config.Skills.Keys)
			{
				ConfigMonsterSkill configMonsterSkill = config.Skills[key];
				if (configMonsterSkill.AnimatorEventPattern != null)
				{
					int i = 0;
					for (int num = configMonsterSkill.AnimatorStateNames.Length; i < num; i++)
					{
						AttachAnimatorEventPattern(Animator.StringToHash(configMonsterSkill.AnimatorStateNames[i]), configMonsterSkill.AnimatorEventPattern);
					}
				}
			}
		}

		private void InitUniqueMonsterAndElite(bool isElite)
		{
			if (uniqueMonsterID != 0)
			{
				List<float> scale = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID).scale;
				if (scale.Count == 3)
				{
					SetUniformScale(scale[0]);
				}
			}
			else if (isElite)
			{
				SetUniformScale(1.1f);
			}
			if (config.CommonArguments.UseEliteShader && (uniqueMonsterID != 0 || isElite))
			{
				SetEliteShader();
				PostSetEliteMat();
			}
		}

		protected void InitController(string AIName, bool disableBehaviorWhenInit)
		{
			_aiController = new BTreeMonsterAIController(this, AIName, disableBehaviorWhenInit);
			if (!disableBehaviorWhenInit)
			{
				((BTreeMonsterAIController)_aiController).EnableBehavior();
			}
			if (uniqueMonsterID != 0)
			{
				UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
				List<string> attackCDNames = uniqueMonsterMetaData.attackCDNames;
				List<float> attackCDs = uniqueMonsterMetaData.attackCDs;
				BTreeMonsterAIController bTreeMonsterAIController = (BTreeMonsterAIController)_aiController;
				for (int i = 0; i < attackCDNames.Count; i++)
				{
					bTreeMonsterAIController.SetBehaviorVariable(attackCDNames[i], attackCDs[i]);
				}
			}
		}

		protected override void InitPlugins()
		{
			base.InitPlugins();
			_retreatPlugin = new RetreatPlugin(this);
		}

		private void ApplyAnimatorConfig()
		{
			foreach (string key in config.AnimatorConfig.Keys)
			{
				bool value = config.AnimatorConfig[key];
				SetPersistentAnimatorBool(key, value);
			}
		}

		private BaseMonsterAIController CreateAIController()
		{
			Type type = Type.GetType("MoleMole." + AIModeName + "MonsterAIController");
			return (BaseMonsterAIController)Activator.CreateInstance(type, new object[1] { this });
		}

		public IAIController GetActiveAIController()
		{
			return _aiController;
		}

		public bool IsAIControllerActive()
		{
			return _aiController.active;
		}

		public void SetUseAIController(bool isUse)
		{
			_aiController.SetActive(isUse);
		}

		public void SetCountedMuteControl(bool mute)
		{
			_muteControlCount += (mute ? 1 : (-1));
		}

		public bool IsMuteControl()
		{
			return _muteControlCount > 0;
		}

		public override bool IsToBeRemove()
		{
			return _isToBeRemove;
		}

		public override bool IsActive()
		{
			return _isAlive && base.gameObject.activeSelf;
		}

		public void onAnimatedHitBoxCreated(MonoAnimatedHitboxDetect hitBox, ConfigEntityAttackPattern attackPattern)
		{
			if (this.onAnimatedHitBoxCreatedCallBack != null)
			{
				this.onAnimatedHitBoxCreatedCallBack(hitBox, attackPattern);
			}
		}

		public override void SetAttackTarget(BaseMonoEntity newTarget)
		{
			bool flag = false;
			if (_attackTarget != newTarget)
			{
				flag = true;
			}
			_attackTarget = newTarget;
			if (onAttackTargetChanged != null && flag)
			{
				onAttackTargetChanged(newTarget);
			}
		}

		public override void CleanOwnedObjects()
		{
			base.CleanOwnedObjects();
			Singleton<AuxObjectManager>.Instance.ClearAuxObjects<MonoAttackHint>(GetRuntimeID());
		}

		public override void SetDied(KillEffect killEffect)
		{
			if (!_isAlive && killEffect == KillEffect.KillImmediately)
			{
				if (_fastDieCoroutine != null)
				{
					StopCoroutine(_fastDieCoroutine);
					_fastDieCoroutine = null;
				}
				if (_waitHitDieCoroutine != null)
				{
					StopCoroutine(_waitHitDieCoroutine);
					_waitHitDieCoroutine = null;
				}
				SetDestroy();
				return;
			}
			_isAlive = false;
			_aiController.SetActive(false);
			if ((killEffect == KillEffect.KillFastImmediately || killEffect == KillEffect.KillFastWithDieAnim || killEffect == KillEffect.KillFastWithNormalAnim) && config.StateMachinePattern.FastDieEffectPattern == null)
			{
				killEffect = KillEffect.KillNow;
			}
			CleanOwnedObjects();
			CastWaitingAudioEvent();
			if (_aiController != null)
			{
				((BTreeMonsterAIController)_aiController).DisableBehavior();
			}
			switch (killEffect)
			{
			case KillEffect.KillNow:
			case KillEffect.KillFastWithDieAnim:
			case KillEffect.KillFastWithNormalAnim:
				DieNow(killEffect);
				break;
			case KillEffect.KillImmediately:
				SetDestroy();
				DieNow(killEffect);
				break;
			case KillEffect.KillTillHitAnimationEnd:
				_waitHitDieCoroutine = StartCoroutine(WaitHitAnimationFinishIter());
				break;
			}
		}

		public void KillDead(KillEffect killEffect)
		{
			if (_waitHitDieCoroutine != null)
			{
				StopCoroutine(_waitHitDieCoroutine);
				_waitHitDieCoroutine = null;
				DieNow(killEffect);
			}
			else if (_fastDieCoroutine == null)
			{
			}
		}

		private void DieNow(KillEffect killEffect)
		{
			if (onDie != null)
			{
				onDie(this);
			}
			if (killEffect != KillEffect.KillImmediately)
			{
				float num = 0f;
				MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(GetRuntimeID());
				AbilityState abilityState = AbilityState.None;
				if (IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw) && config.StateMachinePattern.ThrowDieEffectPattern != null)
				{
					killEffect = KillEffect.KillFastWithNormalAnim;
					num = 0.1f;
				}
				else if (actor != null && actor.abilityState.ContainsState(AbilityState.Frozen))
				{
					killEffect = KillEffect.KillFastWithNormalAnim;
					num = 0.1f;
					abilityState = actor.abilityState;
				}
				else
				{
					num = config.StateMachinePattern.FastDieAnimationWaitDuration;
				}
				switch (killEffect)
				{
				case KillEffect.KillNow:
					ResetAllTriggers();
					SetTrigger("IsDead");
					MaskAllTriggers(true);
					break;
				case KillEffect.KillFastWithDieAnim:
					ResetAllTriggers();
					SetTrigger("IsDead");
					MaskAllTriggers(true);
					_fastDieCoroutine = StartCoroutine(FastDieIter(num, abilityState));
					break;
				case KillEffect.KillFastWithNormalAnim:
					_fastDieCoroutine = StartCoroutine(FastDieIter(num, abilityState));
					break;
				case KillEffect.KillFastImmediately:
					_fastDieCoroutine = StartCoroutine(FastDieIter(0f, abilityState));
					break;
				}
				hitbox.enabled = false;
				for (int i = 0; i < bodyParts.Length; i++)
				{
					bodyParts[i].hitbox.enabled = false;
				}
				if (config.CommonArguments.HitboxInactiveDelay > 0f)
				{
					_inactiveTimer = config.CommonArguments.HitboxInactiveDelay;
				}
				else
				{
					base.gameObject.layer = InLevelData.INACTIVE_ENTITY_LAYER;
				}
			}
		}

		private IEnumerator FastDieIter(float killFastDuration, AbilityState abilityState = AbilityState.None)
		{
			float timer = killFastDuration;
			while (timer > 0f)
			{
				timer -= Time.deltaTime * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
				yield return null;
			}
			_animator.speed = 0f;
			if (_retreatPlugin.IsActive())
			{
				_retreatPlugin.CancelActiveRetreat();
			}
			timer = 0.3f;
			while (timer > 0.24000001f)
			{
				timer -= Time.deltaTime * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
				yield return null;
			}
			string effectPattern = null;
			effectPattern = (abilityState.ContainsState(AbilityState.Frozen) ? "Frozen_Die" : ((!IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw)) ? config.StateMachinePattern.FastDieEffectPattern : config.StateMachinePattern.ThrowDieEffectPattern));
			if (effectPattern != null)
			{
				TriggerEffectPattern(effectPattern);
			}
			while (timer > 0f)
			{
				timer -= Time.deltaTime * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
				yield return null;
			}
			_fastDieCoroutine = null;
			DeadHandler();
		}

        private IEnumerator WaitHitAnimationFinishIter()
        {
            float timer = 0.2f;
            while (!this.IsAnimatorInTag(MonsterData.MonsterTagGroup.ShowHit) && !this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw) && timer > 0f)
            {
				float t = Singleton<LevelManager>.Instance.levelEntity.TimeScale;
                timer -= ((!this._frameHaltPlugin.IsActive()) ? (Time.deltaTime * t) : 0f);
                yield return null;
            }
            if (this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
            {
                while (this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
                {
                    yield return null;
                }
                this.DieNow(KillEffect.KillFastImmediately);
            }
            else if (this.IsAnimatorInTag(MonsterData.MonsterTagGroup.ShowHit))
            {
                while (this.IsAnimatorInTag(MonsterData.MonsterTagGroup.ShowHit) && this.GetCurrentNormalizedTime() < 0.7f)
                {
                    yield return null;
                }
                this.DieNow(KillEffect.KillNow);
            }
            else
            {
                this.DieNow(KillEffect.KillFastImmediately);
            }
            this._waitHitDieCoroutine = null;
            yield break;
        }

        protected override void OnDestroy()
		{
			StopCheckOutsideWallCoroutine();
			base.OnDestroy();
			onAttackTargetChanged = null;
			onDie = null;
			if (_aiController != null)
			{
				((BTreeMonsterAIController)_aiController).DisableBehavior();
			}
		}

		public Vector3 GetDropPosition()
		{
			return (!(base.RootNodePosition.y > 1f)) ? new Vector3(base.RootNodePosition.x, 1f, base.RootNodePosition.z) : new Vector3(base.RootNodePosition.x, base.RootNodePosition.y, base.RootNodePosition.z);
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		protected override void UpdatePlugins()
		{
			_frameHaltPlugin.Core();
			_retreatPlugin.Core();
			_shaderTransitionPlugin.Core();
			_shaderLerpPlugin.Core();
		}

		protected override void FixedUpdatePlugins()
		{
			_retreatPlugin.FixedCore();
		}

		public override void SteerFaceDirectionTo(Vector3 dir)
		{
			if (IsWaitTransitionUnactive(_muteSteerIx))
			{
				base.SteerFaceDirectionTo(dir);
				_hasSteeredThisFrame = true;
			}
		}

		protected override void Update()
		{
			base.Update();
			_aiController.Core();
			UpdateAttackSpeed();
			if (!_usingThrowMass)
			{
				if (IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
				{
					_usingThrowMass = true;
					SetMass(_originalMass * 0.1f * (1f + GetProperty("Entity_MassRatio")));
				}
			}
			else if (!IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
			{
				_usingThrowMass = false;
				SetMass(_originalMass * (1f + GetProperty("Entity_MassRatio")));
			}
			if (config.StateMachinePattern.UseStandByWalkSteer && IsAnimatorInTag(MonsterData.MonsterTagGroup.Idle))
			{
				if (_hasSteeredThisFrame)
				{
					_standBySteerTime += Time.deltaTime * TimeScale;
					if (!GetLocomotionBool("IsStandByWalkSteer") && _standBySteerTime > config.StateMachinePattern.WalkSteerTimeThreshold)
					{
						SetLocomotionBool("IsStandByWalkSteer", true);
					}
				}
				else
				{
					if (GetLocomotionBool("IsStandByWalkSteer"))
					{
						SetLocomotionBool("IsStandByWalkSteer", false);
					}
					_standBySteerTime = 0f;
				}
			}
			if (config.StateMachinePattern.KeepHitboxStanding)
			{
				Transform transform = hitbox.transform;
				Vector3 eulerAngles = transform.eulerAngles;
				eulerAngles.x = -90f;
				eulerAngles.y = 0f;
				eulerAngles.z = 0f;
				transform.eulerAngles = eulerAngles;
				if (RootNode.position.y < config.StateMachinePattern.KeepHitboxStandingMinHeight)
				{
					Vector3 xZPosition = XZPosition;
					xZPosition.y = config.StateMachinePattern.KeepHitboxStandingMinHeight;
					transform.position = xZPosition;
				}
				else
				{
					transform.localPosition = Vector3.zero;
				}
			}
			if (_inactiveTimer > 0f)
			{
				_inactiveTimer -= Time.deltaTime * TimeScale;
				if (_inactiveTimer <= 0f)
				{
					base.gameObject.layer = InLevelData.INACTIVE_ENTITY_LAYER;
				}
			}
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			_hasSteeredThisFrame = false;
			if (config.CommonArguments.UseSwitchShader && (_shaderStack == null || _shaderStack.GetRealTopIndex() == 0))
			{
				LateUpdateShader();
			}
		}

		protected virtual void UpdateControl()
		{
		}

		public float GetOriginMoveSpeed(string moveSpeedKey)
		{
			float num = 1f;
			if (uniqueMonsterID != 0)
			{
				UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
				num = uniqueMonsterMetaData.moveSpeedRatio;
			}
			return (float)config.DynamicArguments[moveSpeedKey] * num;
		}

		private void SyncAnimatorMoveSpeed()
		{
			float num = _moveSpeedRatio * (1f + GetProperty("Animator_MoveSpeedRatio"));
			_animator.SetFloat("MoveSpeed", num);
			if (config.StateMachinePattern.UseAbsMoveSpeed)
			{
				_animator.SetFloat("AbsMoveSpeed", Mathf.Abs(num));
			}
		}

		public bool IsAnimatorInTag(MonsterData.MonsterTagGroup tagGroup, AnimatorStateInfo stateInfo)
		{
			return MonsterData.MONSTER_TAG_GROUPS[(int)tagGroup].Contains(stateInfo.tagHash);
		}

		public bool IsAnimatorInTag(MonsterData.MonsterTagGroup tagGroup)
		{
			return IsAnimatorInTag(tagGroup, _animator.GetCurrentAnimatorStateInfo(0));
		}

		public int GetAnimatorTag()
		{
			return _animator.GetCurrentAnimatorStateInfo(0).tagHash;
		}

		private void UpdateAttackSpeed()
		{
			if (_attackSpeedState == AttackSpeedState.WaitingAttackTimeStart)
			{
				float currentNormalizedTime = GetCurrentNormalizedTime();
				if (currentNormalizedTime > config.Skills[CurrentSkillID].AttackNormalizedTimeStart)
				{
					SetPropertyByStackIndex("Animator_OverallSpeedRatio", _attackSpeedTimeScaleIx, GetProperty("Entity_AttackSpeed"));
					_attackSpeedState = AttackSpeedState.DuringAttackTime;
				}
			}
			else if (_attackSpeedState == AttackSpeedState.DuringAttackTime)
			{
				float currentNormalizedTime2 = GetCurrentNormalizedTime();
				if (currentNormalizedTime2 > config.Skills[CurrentSkillID].AttackNormalizedTimeStop)
				{
					SetPropertyByStackIndex("Animator_OverallSpeedRatio", _attackSpeedTimeScaleIx, 0f);
					_attackSpeedState = AttackSpeedState.AttackTimeEnded;
				}
			}
		}

		private void OnSkillIDChanged(string oldID, string skillID)
		{
			if (_attackSpeedState == AttackSpeedState.DuringAttackTime || skillID == null)
			{
				SetPropertyByStackIndex("Animator_OverallSpeedRatio", _attackSpeedTimeScaleIx, 0f);
				_attackSpeedState = AttackSpeedState.Idle;
			}
			else if (config.Skills[skillID].AttackNormalizedTimeStop != 0f)
			{
				_attackSpeedState = AttackSpeedState.WaitingAttackTimeStart;
			}
			else
			{
				_attackSpeedState = AttackSpeedState.Idle;
			}
			if (skillID == null)
			{
				SetMass(_originalMass);
			}
			else
			{
				SetMass(_originalMass * config.Skills[skillID].MassRatio);
			}
		}

		private void OnAttackSpeedChanged()
		{
			if (_attackSpeedState == AttackSpeedState.DuringAttackTime)
			{
				SetPropertyByStackIndex("Animator_OverallSpeedRatio", _attackSpeedTimeScaleIx, GetProperty("Entity_AttackSpeed"));
			}
		}

		protected override void OnAnimatorMove()
		{
			base.OnAnimatorMove();
			_rigidbody.velocity *= 1f + GetProperty("Animator_RigidBodyVelocityRatio");
		}

		public virtual void BeHit(int frameHalt, AttackResult.AnimatorHitEffect hitEffect, AttackResult.AnimatorHitEffectAux hitEffectAux, KillEffect killEffect, BeHitEffect beHitEffect, float aniDamageRatio, Vector3 hitForward, float retreatVelocity)
		{
			bool flag;
			switch (beHitEffect)
			{
			case BeHitEffect.KillingBeHit:
				if (config.StateMachinePattern.FastDieEffectPattern == null)
				{
					killEffect = KillEffect.KillNow;
				}
				switch (killEffect)
				{
				case KillEffect.KillNow:
					if (config.StateMachinePattern.ThrowBlowDieNamedState != null && (hitEffect == AttackResult.AnimatorHitEffect.ThrowUp || hitEffect == AttackResult.AnimatorHitEffect.ThrowUpBlow || hitEffect == AttackResult.AnimatorHitEffect.ThrowBlow))
					{
						SetLocomotionBool("IsHeavyRetreat", true);
						BlowVelocityScaledRetreat(hitForward, retreatVelocity, config.StateMachinePattern.ThrowBlowDieNamedState);
					}
					flag = false;
					break;
				case KillEffect.KillFastWithNormalAnim:
					flag = true;
					break;
				case KillEffect.KillTillHitAnimationEnd:
					flag = true;
					break;
				default:
					flag = false;
					break;
				}
				break;
			case BeHitEffect.OverkillBeHit:
				switch (killEffect)
				{
				case KillEffect.KillNow:
					if (config.StateMachinePattern.ThrowBlowDieNamedState != null && (hitEffect == AttackResult.AnimatorHitEffect.ThrowUp || hitEffect == AttackResult.AnimatorHitEffect.ThrowUpBlow || hitEffect == AttackResult.AnimatorHitEffect.ThrowBlow))
					{
						SetLocomotionBool("IsHeavyRetreat", true);
						BlowVelocityScaledRetreat(hitForward, retreatVelocity, config.StateMachinePattern.ThrowBlowDieNamedState);
					}
					flag = false;
					KillDead(killEffect);
					break;
				case KillEffect.KillTillHitAnimationEnd:
					flag = true;
					if (hitEffect == AttackResult.AnimatorHitEffect.Light)
					{
						hitEffect = AttackResult.AnimatorHitEffect.Normal;
					}
					break;
				default:
					flag = false;
					KillDead(killEffect);
					break;
				}
				break;
			default:
				flag = true;
				break;
			}
			if (!flag)
			{
				return;
			}
			if (beHitEffect == BeHitEffect.KillingBeHit && beHitEffect == BeHitEffect.OverkillBeHit && hitEffect <= AttackResult.AnimatorHitEffect.Light)
			{
				hitEffect = AttackResult.AnimatorHitEffect.Normal;
			}
			if (hitEffect == AttackResult.AnimatorHitEffect.Mute)
			{
				return;
			}
			if (hitEffect == AttackResult.AnimatorHitEffect.Light)
			{
				SetTrigger("LightHitTrigger");
				FrameHalt(frameHalt);
			}
			else
			{
				if (hitEffect <= AttackResult.AnimatorHitEffect.Light)
				{
					return;
				}
				if (onBeHitCanceled != null)
				{
					onBeHitCanceled(CurrentSkillID);
				}
				bool value = retreatVelocity > config.StateMachinePattern.HeavyRetreatThreshold;
				SetLocomotionBool("IsHeavyRetreat", value);
				SetLocomotionFloat("DamageRatio", aniDamageRatio);
				SetLocomotionFloat("HitTimeOffsetRatio", UnityEngine.Random.value);
				int value2 = (int)hitEffectAux;
				if (config.StateMachinePattern.UseRandomLeftRightHitEffectAsNormal && hitEffectAux == AttackResult.AnimatorHitEffectAux.Normal)
				{
					value2 = UnityEngine.Random.Range(1, 3);
				}
				if (config.StateMachinePattern.UseBackHitAngleCheck)
				{
					float num = Vector3.Angle(base.FaceDirection, hitForward);
					if (num < config.StateMachinePattern.BackHitDegreeThreshold)
					{
						value2 = 5;
					}
				}
				if (config.StateMachinePattern.UseLeftRightHitAngleCheck)
				{
					bool flag2 = true;
					float num2 = Vector3.Angle(base.transform.right, hitForward);
					if (num2 < config.StateMachinePattern.LeftRightHitAngleRange)
					{
						value2 = 7;
						flag2 = false;
					}
					if (flag2)
					{
						float num3 = Vector3.Angle(-base.transform.right, hitForward);
						if (num3 < config.StateMachinePattern.LeftRightHitAngleRange)
						{
							value2 = 6;
						}
					}
				}
				_animator.SetInteger("HitEffectAux", value2);
				ResetTrigger("LightHitTrigger");
				ResetTrigger("HitTrigger");
				ResetTrigger("ThrowTrigger");
				ResetTrigger("ThrowBlowTrigger");
				ResetTrigger("ThrowDownTrigger");
				SetTrigger("HitTrigger");
				FrameHalt(frameHalt);
				ClearSkillEffect(null);
				CastWaitingAudioEvent();
				Singleton<AuxObjectManager>.Instance.ClearAuxObjects<MonoAttackHint>(GetRuntimeID());
				switch (hitEffect)
				{
				case AttackResult.AnimatorHitEffect.ThrowUp:
					SetTrigger("ThrowTrigger");
					StandRetreat(hitForward, retreatVelocity);
					return;
				case AttackResult.AnimatorHitEffect.ThrowUpBlow:
					SetTrigger("ThrowTrigger");
					if (config.StateMachinePattern.ThrowUpNamedState != null)
					{
						BlowDecelerateRetreat(hitForward, retreatVelocity, config.StateMachinePattern.ThrowUpNamedState, config.StateMachinePattern.ThrowUpNamedStateRetreatStopNormalizedTime);
					}
					else
					{
						StandRetreat(hitForward, retreatVelocity);
					}
					return;
				case AttackResult.AnimatorHitEffect.ThrowDown:
					SetTrigger("ThrowDownTrigger");
					StandRetreat(hitForward, retreatVelocity);
					return;
				case AttackResult.AnimatorHitEffect.ThrowBlow:
					if (config.StateMachinePattern.ThrowBlowNamedState != null)
					{
						SetTrigger("ThrowBlowTrigger");
						BlowVelocityScaledRetreat(hitForward, retreatVelocity, config.StateMachinePattern.ThrowBlowNamedState);
					}
					else
					{
						StandRetreat(hitForward, retreatVelocity);
					}
					return;
				case AttackResult.AnimatorHitEffect.ThrowAirBlow:
					if (config.StateMachinePattern.ThrowBlowAirNamedState != null && IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
					{
						SetTrigger("ThrowBlowTrigger");
						BlowDecelerateRetreat(hitForward, retreatVelocity, config.StateMachinePattern.ThrowBlowAirNamedState, config.StateMachinePattern.ThrowBlowAirNamedStateRetreatStopNormalizedTime);
					}
					else
					{
						StandRetreat(hitForward, retreatVelocity);
					}
					return;
				case AttackResult.AnimatorHitEffect.KnockDown:
					if (this is BaseMonoDarkAvatar)
					{
						SetTrigger("TriggerKnockDown");
						StandRetreat(hitForward, retreatVelocity);
						return;
					}
					break;
				}
				if (hitEffect == AttackResult.AnimatorHitEffect.FaceAttacker)
				{
					SetOverrideSteerFaceDirectionFrame(-hitForward);
					StandRetreat(hitForward, retreatVelocity);
				}
				else
				{
					StandRetreat(hitForward, retreatVelocity);
				}
			}
		}

		protected virtual void StandRetreat(Vector3 retreatDir, float retreatVelocity)
		{
			if (retreatVelocity != 0f)
			{
				_retreatPlugin.StandRetreat(retreatDir, retreatVelocity);
			}
		}

		protected virtual void BlowDecelerateRetreat(Vector3 retreatDir, float retreatVelocity, string namedState, float endNormalizedTime)
		{
			SetOverrideSteerFaceDirectionFrame(-retreatDir);
			if (retreatVelocity != 0f)
			{
				_retreatPlugin.BlowDecelerateRetreat(retreatDir, retreatVelocity, namedState, config.StateMachinePattern.RetreatBlowVelocityRatio, endNormalizedTime);
			}
		}

		protected virtual void BlowVelocityScaledRetreat(Vector3 retreatDir, float retreatVelocity, string namedState)
		{
			SetOverrideSteerFaceDirectionFrame(-retreatDir);
			if (retreatVelocity != 0f)
			{
				_retreatPlugin.BlowVelocityScaledRetreat(retreatDir, retreatVelocity, namedState, retreatVelocity * config.StateMachinePattern.RetreatToVelocityScaleRatio);
			}
		}

		public override BaseMonoEntity GetAttackTarget()
		{
			return AttackTarget;
		}

		public override void TriggerAttackPattern(string animEventID, LayerMask layerMask)
		{
			ConfigMonsterAnimEvent configMonsterAnimEvent = SharedAnimEventData.ResolveAnimEvent(config, animEventID);
			if (configMonsterAnimEvent.CameraShake != null && configMonsterAnimEvent.CameraShake.ShakeOnNotHit)
			{
				AttackPattern.ActCameraShake(configMonsterAnimEvent.CameraShake);
			}
			if (configMonsterAnimEvent.AttackPattern != null)
			{
				configMonsterAnimEvent.AttackPattern.patternMethod(animEventID, configMonsterAnimEvent.AttackPattern, this, layerMask);
			}
		}

		protected void MuteSteerTillNextState()
		{
			StartWaitTransitionState(_muteSteerIx);
		}

		[AnimationCallback]
		public override void AnimEventHandler(string animEventID)
		{
			ConfigMonsterAnimEvent configMonsterAnimEvent = SharedAnimEventData.ResolveAnimEvent(config, animEventID);
			if (configMonsterAnimEvent == null || (_maskedAnimEvents != null && _maskedAnimEvents.Contains(animEventID)) || !_animEventPredicates.Contains(configMonsterAnimEvent.Predicate) || !_animEventPredicates.Contains(configMonsterAnimEvent.Predicate2))
			{
				return;
			}
			if (configMonsterAnimEvent.TriggerAbility != null)
			{
				EvtAbilityStart evtAbilityStart = new EvtAbilityStart(_runtimeID);
				evtAbilityStart.abilityID = configMonsterAnimEvent.TriggerAbility.ID;
				evtAbilityStart.abilityName = configMonsterAnimEvent.TriggerAbility.Name;
				evtAbilityStart.abilityArgument = configMonsterAnimEvent.TriggerAbility.Argument;
				Singleton<EventManager>.Instance.FireEvent(evtAbilityStart);
			}
			if (configMonsterAnimEvent.CameraShake != null && configMonsterAnimEvent.CameraShake.ShakeOnNotHit)
			{
				AttackPattern.ActCameraShake(configMonsterAnimEvent.CameraShake);
			}
			if (configMonsterAnimEvent.AttackPattern != null)
			{
				if (configMonsterAnimEvent.AttackProperty == null || configMonsterAnimEvent.AttackProperty.AttackTargetting == MixinTargetting.Enemy)
				{
					configMonsterAnimEvent.AttackPattern.patternMethod(animEventID, configMonsterAnimEvent.AttackPattern, this, AttackPattern.GetLayerMask(this));
				}
				else
				{
					configMonsterAnimEvent.AttackPattern.patternMethod(animEventID, configMonsterAnimEvent.AttackPattern, this, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(_runtimeID, configMonsterAnimEvent.AttackProperty.AttackTargetting));
				}
			}
			if (configMonsterAnimEvent.AttackHint != null)
			{
				HandleAttackHint(configMonsterAnimEvent.AttackHint);
			}
			if (configMonsterAnimEvent.PhysicsProperty != null)
			{
				ConfigEntityPhysicsProperty physicsProperty = configMonsterAnimEvent.PhysicsProperty;
				HandlePhysicsProperty(hitbox, physicsProperty);
			}
			if (configMonsterAnimEvent.TriggerEffectPattern != null)
			{
				TriggerEffectPattern(configMonsterAnimEvent.TriggerEffectPattern.EffectPattern);
			}
			if (configMonsterAnimEvent.TriggerTintCamera != null)
			{
				TriggerTint(configMonsterAnimEvent.TriggerTintCamera.RenderDataName, configMonsterAnimEvent.TriggerTintCamera.Duration, configMonsterAnimEvent.TriggerTintCamera.TransitDuration);
			}
			if (configMonsterAnimEvent.TimeSlow != null && (configMonsterAnimEvent.TimeSlow.Force || (AttackTarget != null && AttackTarget.IsActive() && Vector3.Distance(XZPosition, AttackTarget.XZPosition) < 2f)))
			{
				Singleton<LevelManager>.Instance.levelActor.TimeSlow(configMonsterAnimEvent.TimeSlow.Duration, configMonsterAnimEvent.TimeSlow.SlowRatio, null);
			}
		}

		[AnimationCallback]
		public override void MultiAnimEventHandler(string multiAnimEventID)
		{
			ConfigMultiAnimEvent configMultiAnimEvent = config.MultiAnimEvents[multiAnimEventID];
			for (int i = 0; i < configMultiAnimEvent.AnimEventNames.Length; i++)
			{
				AnimEventHandler(configMultiAnimEvent.AnimEventNames[i]);
			}
		}

		public override void DeadHandler()
		{
			if (config.StateMachinePattern.DieAnimEventID != null)
			{
				AnimEventHandler(config.StateMachinePattern.DieAnimEventID);
			}
			SetDestroy();
		}

		private void OnIsGhostChanged(bool isGhost)
		{
			hitbox.enabled = !isGhost;
		}

		private void HandleAttackHint(ConfigMonsterAttackHint attackHintConfig)
		{
			string empty = string.Empty;
			if (attackHintConfig is RectAttackHint)
			{
				empty = "RectAttackHint";
			}
			else if (attackHintConfig is CircleAttackHint)
			{
				empty = "CircleAttackHint";
			}
			else
			{
				if (!(attackHintConfig is SectorAttackHint))
				{
					throw new Exception("Invalid Type or State!");
				}
				empty = "SectorAttackHint";
			}
			MonoAttackHint monoAttackHint = Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoAttackHint>(empty, GetRuntimeID());
			monoAttackHint.Init(this, AttackTarget as BaseMonoAnimatorEntity, attackHintConfig);
		}

		public void ClearHitTrigger()
		{
			_animator.ResetTrigger("HitTrigger");
		}

		public override void ClearAttackTriggers()
		{
			if (!IsActive())
			{
				return;
			}
			foreach (string key in config.Skills.Keys)
			{
				_animator.ResetTrigger(key);
			}
		}

		public override void ClearAttackTarget()
		{
			ClearAttackTriggers();
			SetAttackTarget(null);
		}

		[AnimationCallback]
		private void TriggerAttackScreenShake(string attackName)
		{
			ConfigEntityCameraShake cameraShake = SharedAnimEventData.ResolveAnimEvent(config, attackName).CameraShake;
			AttackPattern.ActCameraShake(cameraShake);
		}

		private void NamedStateChanged(string fromNamedState, string toNamedState)
		{
			_currentNamedState = toNamedState;
			ConfigNamedState configNamedState = null;
			ConfigNamedState configNamedState2 = null;
			if (fromNamedState != null)
			{
				configNamedState = config.NamedStates[fromNamedState];
			}
			if (toNamedState != null)
			{
				configNamedState2 = config.NamedStates[toNamedState];
			}
			if (configNamedState2 != null && configNamedState2.HighSpeedMovement)
			{
				PushHighspeedMovement();
			}
			if (configNamedState != null && configNamedState.HighSpeedMovement)
			{
				PopHighspeedMovement();
			}
		}

		private void SkillIDChanged(string fromSkillID, AnimatorStateInfo fromState, string toSkillID, AnimatorStateInfo toState)
		{
			ConfigMonsterSkill configMonsterSkill = null;
			ConfigMonsterSkill configMonsterSkill2 = null;
			if (fromSkillID != null)
			{
				configMonsterSkill = config.Skills[fromSkillID];
			}
			if (toSkillID != null)
			{
				configMonsterSkill2 = config.Skills[toSkillID];
			}
			if (configMonsterSkill2 != null && configMonsterSkill2.HighSpeedMovement)
			{
				PushHighspeedMovement();
			}
			if (configMonsterSkill != null && configMonsterSkill.HighSpeedMovement)
			{
				PopHighspeedMovement();
			}
			if (configMonsterSkill2 != null && configMonsterSkill2.Unselectable)
			{
				SetCountedDenySelect(true);
			}
			if (configMonsterSkill != null && configMonsterSkill.Unselectable)
			{
				SetCountedDenySelect(false);
			}
			if ((toSkillID != null || fromSkillID == null) && toSkillID != null)
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtAttackStart(GetRuntimeID(), toSkillID));
				if (configMonsterSkill2.SteerToTargetOnEnter && AttackTarget != null && AttackTarget.IsActive())
				{
					Vector3 dir = AttackTarget.XZPosition - XZPosition;
					SteerFaceDirectionTo(dir);
					if (_animator.IsInTransition(0))
					{
						MuteSteerTillNextState();
					}
				}
			}
			SetCurrentSKillID(toSkillID);
		}

		protected override void OnAnimatorStateChanged(AnimatorStateInfo fromState, AnimatorStateInfo toState)
		{
			base.OnAnimatorStateChanged(fromState, toState);
			CheckHitStateChanged(fromState, toState);
			string value;
			config.StateToNamedStateMap.TryGetValue(fromState.shortNameHash, out value);
			string value2;
			config.StateToNamedStateMap.TryGetValue(toState.shortNameHash, out value2);
			if (value != null || value2 != null)
			{
				NamedStateChanged(value, value2);
			}
			string value3;
			config.StateToSkillIDMap.TryGetValue(fromState.shortNameHash, out value3);
			string value4;
			config.StateToSkillIDMap.TryGetValue(toState.shortNameHash, out value4);
			if (value3 != null || value4 != null)
			{
				SkillIDChanged(value3, fromState, value4, toState);
			}
			if (config.StateMachinePattern.UseStandByWalkSteer)
			{
				if (fromState.shortNameHash != _walkSteerStateHash && toState.shortNameHash == _walkSteerStateHash)
				{
					SetNeedOverrideVelocity(true);
					SetOverrideVelocity(Vector3.zero);
				}
				else if (toState.shortNameHash != _walkSteerStateHash && fromState.shortNameHash == _walkSteerStateHash)
				{
					SetNeedOverrideVelocity(false);
				}
			}
			ResetRigidbodyRotation();
		}

		private void CheckHitStateChanged(AnimatorStateInfo fromState, AnimatorStateInfo toState)
		{
			bool flag = IsAnimatorInTag(MonsterData.MonsterTagGroup.ShowHit, fromState) || IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw, fromState);
			bool flag2 = IsAnimatorInTag(MonsterData.MonsterTagGroup.ShowHit, toState) || IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw, toState);
			if (flag != flag2 && onHitStateChanged != null)
			{
				onHitStateChanged(this, flag, flag2);
			}
		}

		public void SetSoleAnimatorEventPattern(int stateHash, string animatorEventPatternName)
		{
			if (_patternMap.ContainsKey(stateHash))
			{
				if (_patternMap[stateHash] == animatorEventPatternName)
				{
					return;
				}
				DetachAnimatorEventPattern(stateHash, _patternMap[stateHash]);
			}
			AttachAnimatorEventPattern(stateHash, animatorEventPatternName);
			_patternMap[stateHash] = animatorEventPatternName;
		}

		public void SetSoleSkillAnimatorEventPattern(string skillName, string animatorEventPatternName)
		{
			if (!config.Skills.ContainsKey(skillName))
			{
				return;
			}
			string[] animatorStateNames = config.Skills[skillName].AnimatorStateNames;
			if (animatorStateNames != null)
			{
				int i = 0;
				for (int num = animatorStateNames.Length; i < num; i++)
				{
					SetSoleAnimatorEventPattern(Animator.StringToHash(animatorStateNames[i]), animatorEventPatternName);
				}
			}
		}

		public void ClearSoleAnimatorEventPattern(int stateHash)
		{
			if (_patternMap.ContainsKey(stateHash))
			{
				DetachAnimatorEventPattern(stateHash, _patternMap[stateHash]);
				_patternMap.Remove(stateHash);
			}
		}

		public void ClearSoleSkillAnimatorEventPattern(string skillName)
		{
			if (!config.Skills.ContainsKey(skillName))
			{
				return;
			}
			string[] animatorStateNames = config.Skills[skillName].AnimatorStateNames;
			if (animatorStateNames != null)
			{
				int i = 0;
				for (int num = animatorStateNames.Length; i < num; i++)
				{
					ClearSoleAnimatorEventPattern(Animator.StringToHash(animatorStateNames[i]));
				}
			}
		}

		protected override void OnSkillEffectClear(string oldID, string skillID)
		{
			if (skillID != null)
			{
				ConfigMonsterSkill configMonsterSkill = config.Skills[skillID];
				if (configMonsterSkill.NeedClearEffect)
				{
					ClearSkillEffect(skillID);
				}
			}
		}

		private void AttachEffectOverrides()
		{
			if (config.CommonArguments.EffectPredicates != null && config.CommonArguments.EffectPredicates.Length != 0)
			{
				MonoEffectOverride monoEffectOverride = GetComponent<MonoEffectOverride>();
				if (monoEffectOverride == null)
				{
					monoEffectOverride = base.gameObject.AddComponent<MonoEffectOverride>();
				}
				monoEffectOverride.effectPredicates.AddRange(config.CommonArguments.EffectPredicates);
			}
		}

		private void FadeOutHandler(float duration)
		{
		}

		private void FadeInHandler(float duration)
		{
		}

		protected void LateUpdateShader()
		{
			Vector3 lhs = base.transform.position - _mainCameraTrans.position;
			float num = Vector3.Dot(lhs, _mainCameraTrans.forward);
			if (_usingTransparentShader)
			{
				if (num > config.CommonArguments.UseTransparentShaderDistanceThreshold)
				{
					SwitchTransparentShader(false);
					_usingTransparentShader = false;
				}
			}
			else if (num < config.CommonArguments.UseTransparentShaderDistanceThreshold)
			{
				SwitchTransparentShader(true);
				_usingTransparentShader = true;
			}
		}

		protected void SwitchTransparentShader(bool useTransparent)
		{
			Material[] allMaterials = GetAllMaterials();
			for (int i = 0; i < allMaterials.Length; i++)
			{
				if (useTransparent)
				{
					allMaterials[i].shader = MonsterData.MONSTER_TRANSPARENT_SHADER;
				}
				else
				{
					allMaterials[i].shader = MonsterData.MONSTER_OPAQUE_SHADER;
				}
			}
		}

		public virtual void SetEliteShader()
		{
			TryInitShaderStack();
			_shaderStack.Push(1, MonsterData.MONSTER_ELITE_SHADER);
		}

		public virtual void PostSetEliteMat()
		{
			for (int i = 0; i < _matListForSpecailState.Count; i++)
			{
				Material material = _matListForSpecailState[i].material;
				material.SetColor("_EliteColor1", config.EliteArguments.EliteColor1);
				material.SetColor("_EliteColor2", config.EliteArguments.EliteColor2);
				material.SetFloat("_EliteEmissionScaler1", config.EliteArguments.EliteEmissionScaler1);
				material.SetFloat("_EliteEmissionScaler2", config.EliteArguments.EliteEmissionScaler2);
				material.SetFloat("_EliteNormalDisplacement1", config.EliteArguments.EliteNormalDisplacement1);
				material.SetFloat("_EliteNormalDisplacement2", config.EliteArguments.EliteNormalDisplacement2);
			}
		}

		public virtual void SwitchEliteShader(bool enable)
		{
			if (_shaderStack != null)
			{
				if (enable && !_shaderStack.IsOccupied(1))
				{
					_shaderStack.Push(1, MonsterData.MONSTER_ELITE_SHADER);
				}
				else if (!enable && _shaderStack.IsOccupied(1))
				{
					_shaderStack.Pop(1);
				}
			}
		}

		protected override void TryInitShaderStack()
		{
			if (_shaderStack == null)
			{
				base.TryInitShaderStack();
				_shaderStack.Push(MonsterData.MONSTER_OPAQUE_SHADER, true);
			}
		}

		protected override void RecoverOriginalShaders()
		{
			if (config.CommonArguments.UseSwitchShader)
			{
				SwitchTransparentShader(_usingTransparentShader);
			}
			else
			{
				base.RecoverOriginalShaders();
			}
		}

		protected override void OnShaderStackChanged(Shader fromShader, int fromIx, Shader toShader, int toIx)
		{
			if (toIx == 0)
			{
				RecoverOriginalShaders();
			}
			else
			{
				base.OnShaderStackChanged(fromShader, fromIx, toShader, toIx);
			}
		}

		protected override int PushEffectShaderData(Shader shader)
		{
			return _shaderStack.PushAbove(1, shader);
		}

		private void InitDynamicBone()
		{
			bool mONSTER_USE_DYNAMIC_BONE = GlobalVars.MONSTER_USE_DYNAMIC_BONE;
			DynamicBone[] componentsInChildren = base.gameObject.GetComponentsInChildren<DynamicBone>();
			DynamicBone[] array = componentsInChildren;
			foreach (DynamicBone dynamicBone in array)
			{
				dynamicBone.enabled = mONSTER_USE_DYNAMIC_BONE;
			}
		}

		public override void PushNoCollision()
		{
			_noCollisionCount++;
			if (_noCollisionCount == 1)
			{
				base.gameObject.layer = InLevelData.INACTIVE_ENTITY_LAYER;
			}
		}

		public override void PopNoCollision()
		{
			_noCollisionCount--;
			if (_noCollisionCount == 0)
			{
				base.gameObject.layer = InLevelData.MONSTER_LAYER;
			}
		}

		private void InitBodyParts()
		{
			MonoBodyPartEntity[] array = bodyParts;
			foreach (MonoBodyPartEntity monoBodyPartEntity in array)
			{
				monoBodyPartEntity.Init(Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), this);
			}
		}

		public List<BaseMonoAbilityEntity> GetAllHitboxEnabledBodyParts()
		{
			List<BaseMonoAbilityEntity> list = new List<BaseMonoAbilityEntity>();
			MonoBodyPartEntity[] array = bodyParts;
			foreach (MonoBodyPartEntity monoBodyPartEntity in array)
			{
				if (monoBodyPartEntity.hitbox.enabled)
				{
					list.Add(monoBodyPartEntity);
				}
			}
			return list;
		}

		private void InitBornSound()
		{
			if (uniqueMonsterID == 0 && !(this is BaseMonoBoss))
			{
				BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
				MonoEntityAudio component = localAvatar.GetComponent<MonoEntityAudio>();
				if (component != null)
				{
					component.PostMonsterBorn();
				}
			}
		}

		private IEnumerator CheckOutsideWall()
		{
			while (true)
			{
				Miscs.CheckOutsideWallAndDrag(base.transform);
				yield return new WaitForSeconds(0.2f);
			}
		}

		private void StopCheckOutsideWallCoroutine()
		{
			if (_checkOutsideWallCoroutine != null)
			{
				StopCoroutine(_checkOutsideWallCoroutine);
				_checkOutsideWallCoroutine = null;
			}
		}

		public override void SetUseLocalController(bool enabled)
		{
			SetUseAIController(enabled);
		}

		public bool IsRetreating()
		{
			return _retreatPlugin.IsActive();
		}

		public void SetDestroy()
		{
			if (destroyMode == DestroyMode.SetToBeRemoved)
			{
				_isToBeRemove = true;
			}
			else if (destroyMode == DestroyMode.DeactivateOnly)
			{
				base.gameObject.SetActive(false);
			}
		}

		uint IRetreatable.GetRuntimeID()
		{
			return GetRuntimeID();
		}

		string IRetreatable.GetCurrentNamedState()
		{
			return GetCurrentNamedState();
		}

		uint IAttacker.GetRuntimeID()
		{
			return GetRuntimeID();
		}

		float IAttacker.Evaluate(DynamicFloat target)
		{
			return Evaluate(target);
		}

		int IAttacker.Evaluate(DynamicInt target)
		{
			return Evaluate(target);
		}

		uint IAIEntity.GetRuntimeID()
		{
			return GetRuntimeID();
		}
	}
}
