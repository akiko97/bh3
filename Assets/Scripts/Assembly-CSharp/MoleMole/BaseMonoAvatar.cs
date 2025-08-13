using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseMonoAvatar : BaseMonoAnimatorEntity, IAIEntity, IFadeOff, IFrameHaltable, IRetreatable, IAttacker
	{
		private class ShadowColorAdjuster
		{
			private Material _material;

			private Color _first;

			private Color _second;

			public ShadowColorAdjuster(Material material)
			{
				_material = material;
				_first = material.GetColor("_FirstShadowMultColor");
				_second = material.GetColor("_SecondShadowMultColor");
			}

			public void Apply(float factor)
			{
				_material.SetColor("_FirstShadowMultColor", Color.Lerp(_first, Color.white, factor));
				_material.SetColor("_SecondShadowMultColor", Color.Lerp(_second, Color.white, factor));
			}
		}

		private enum AttackSpeedState
		{
			Idle = 0,
			WaitingAttackTimeStart = 1,
			DuringAttackTime = 2,
			AttackTimeEnded = 3
		}

		private enum SkillEnterSteerState
		{
			Idle = 0,
			WaitingForStart = 1,
			Steering = 2
		}

		public enum AvatarSwitchState
		{
			OnStage = 0,
			OffStage = 1,
			SwitchingOut = 2
		}

		public enum AvatarSwapOutType
		{
			Normal = 0,
			Force = 1,
			Delayed = 2
		}

		protected const string MOVE_SPEED_PARAM = "MoveSpeed";

		protected const string ORDER_MOVE_PARAM = "IsMove";

		protected const string RUN_STEP_ON_RIGHT_PARAM = "RunStepOnRight";

		protected const string TRIGGER_ATTACK_PARAM = "TriggerAttack";

		protected const string TRIGGER_HOLD_ATTACK_PARAM = "TriggerHoldAttack";

		protected const string TRIGGER_HIT_PARAM = "TriggerHit";

		protected const string TRIGGER_KNOCK_DOWN_PARAM = "TriggerKnockDown";

		protected const string DIE_PARAM = "IsDead";

		protected const string TRIGGER_SKILL_PARAM = "TriggerSkill_";

		protected const string DAMAGE_RATIO_PARAM = "DamageRatio";

		protected const string COMBAT_TO_STANDBY_CD_PARAM = "CombatToStandByCD";

		protected const string TRIGGER_SWITCH_OUT = "TriggerSwitchOut";

		protected const string TRIGGER_APPEAR = "TriggerAppear";

		protected const string TRIGGER_SWITCH_IN = "TriggerSwitchIn";

		protected const string TRIGGER_WEAPON_PARAM = "TriggerWeapon";

		private const float AVATAR_ONMOVE_MASS_RATIO = 1f;

		private const float AVATAR_STABLE_MASS_RATIO = 100f;

		private const float AVATAR_DASH_MASS_RATIO = 1f;

		public Collider hitbox;

		private bool _isDeadAlready;

		private bool _isToBeRemoved;

		private string _currentSkillID;

		private int _equipedWeaponID = -1;

		public Action<BaseMonoEntity> onAttackTargetChanged;

		public Action<bool> onLockDirectionChanged;

		public Action<BaseMonoAvatar> onDie;

		public ConfigAvatar config;

		protected Action<BaseMonoAvatar> _attackTargetSelectAction;

		protected BaseAvatarInputController _inputController;

		protected BaseAvatarAIController _aiController;

		private AvatarControlData _controlData;

		private float _steerLerpRatio = 1f;

		private bool _hasUpdatedControlThisFrame;

		private AvatarControlData _lastFrameControlData;

		private List<ShadowColorAdjuster> _shadowColorAdjusterList;

		private bool _isShadowColorAdjusted;

		private int _muteControlCount;

		private bool _isAlive;

		private BaseMonoEntity _attackTarget;

		private List<BaseMonoEntity> _subAttackTargetList;

		private bool _isLockDirection;

		private bool _muteAnimRetarget;

		private AttackSpeedState _attackSpeedState;

		private int _attackSpeedTimeScaleIx;

		private int _attackMoveRatioIx;

		private float _moveSpeedRatio;

		private bool _hasGotParameterHodeMode;

		private bool _isHodeMode;

		private bool _isFromAttackOrSkill;

		private float _baseMassRatio = 1f;

		private float _skillMassRatio = 1f;

		private int _muteSteerIx;

		private int _muteLockAttackTargetIx;

		private SkillEnterSteerState _activeEnterSteerState;

		private SkillEnterSteerOption _activeEnterSteerOption;

		private Vector3 _activeEnterSteerClampStart;

		private float _clearAttackTargetTimer;

		private string _activeCameraAnimName;

		private List<int> _attachedEffects = new List<int>();

		private DynamicBone[] _dynamicBones;

		[Header("During these skillIDs dynamic bone animation ")]
		public string[] muteDynamicBonesSkillIDs;

		private int _noCollisionCount;

		private bool _delayedSwapOutTriggered;

		private Coroutine _waitMoveSoundCoroutine;

		public Renderer leftEyeRenderer;

		public Renderer rightEyeRenderer;

		public Renderer mouthRenderer;

		private FaceAnimation _faceAnimation;

		private AtlasMatInfoProvider _providerL;

		private AtlasMatInfoProvider _providerR;

		private AtlasMatInfoProvider _providerM;

		public override string CurrentSkillID
		{
			get
			{
				return _currentSkillID;
			}
		}

		public int EquipedWeaponID
		{
			get
			{
				return _equipedWeaponID;
			}
		}

		public bool isLeader { get; set; }

		public override float TimeScale
		{
			get
			{
				if (base.gameObject.activeSelf)
				{
					return base.TimeScale;
				}
				return Singleton<LevelManager>.Instance.levelEntity.TimeScale;
			}
		}

		public uint AvatarTypeID { get; private set; }

		public string AvatarTypeName { get; private set; }

		public BaseMonoEntity AttackTarget
		{
			get
			{
				return _attackTarget;
			}
		}

		public List<BaseMonoEntity> SubAttackTargetList
		{
			get
			{
				return _subAttackTargetList;
			}
		}

		public bool IsLockDirection
		{
			get
			{
				return _isLockDirection;
			}
			set
			{
				bool flag = false;
				if (value != _isLockDirection)
				{
					flag = true;
				}
				_isLockDirection = value;
				if (onLockDirectionChanged != null && flag)
				{
					onLockDirectionChanged(value);
				}
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

		public float MoveSpeedRatio
		{
			set
			{
				float aniMinSpeedRatio = config.StateMachinePattern.AniMinSpeedRatio;
				float aniMaxSpeedRatio = config.StateMachinePattern.AniMaxSpeedRatio;
				_moveSpeedRatio = Mathf.Clamp(value, aniMinSpeedRatio, aniMaxSpeedRatio);
				SyncAnimatorMoveSpeed();
			}
		}

		public AvatarSwitchState switchState { get; private set; }

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

		private void SetCurrentSkillID(string value)
		{
			if (onCurrentSkillIDChanged != null)
			{
				onCurrentSkillIDChanged(_currentSkillID, value);
			}
			_currentSkillID = value;
		}

		public void Init(bool isLocal, uint runtimeID, string avatarTypeName, int weaponID, Vector3 initPos, Vector3 initForward, bool isLeader)
		{
			AvatarTypeName = avatarTypeName;
			AvatarTypeID = AvatarData.GetAvatarTypeIDByName(AvatarTypeName);
			config = AvatarData.GetAvatarConfig(AvatarTypeName);
			animatorConfig = config;
			commonConfig = config.CommonConfig;
			Init(runtimeID);
			_isAlive = true;
			this.isLeader = isLeader;
			_attackTargetSelectAction = config.AttackTargetSelectPattern.selectMethod;
			MoveSpeedRatio = 1f;
			initPos.y += config.CommonArguments.CreatePosYOffset;
			LayerMask mask = (1 << InLevelData.AVATAR_LAYER) | (1 << InLevelData.MONSTER_LAYER);
			initPos = PickInitPosition(mask, initPos, config.CommonArguments.CollisionRadius);
			base.transform.position = initPos;
			initForward.y = 0f;
			base.transform.forward = initForward;
			_rigidbody.rotation = base.transform.rotation;
			Debug.DrawLine(XZPosition, XZPosition + base.transform.forward * 5f, Color.cyan, 2f);
			_animEventPredicates.Add(config.CommonArguments.DefaultAnimEventPredicate);
			InitController();
			InitSkillAnimatorEventPattern();
			InitPlugins();
			base.gameObject.name = "Avatar_" + AvatarTypeName + "_" + runtimeID;
			_muteSteerIx = AddWaitTransitionState();
			_muteLockAttackTargetIx = AddWaitTransitionState();
			_attackSpeedState = AttackSpeedState.Idle;
			_attackSpeedTimeScaleIx = PushProperty("Animator_OverallSpeedRatio", 0f);
			onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(onCurrentSkillIDChanged, new Action<string, string>(OnSkillIDChanged));
			onActiveChanged = (Action<bool>)Delegate.Combine(onActiveChanged, new Action<bool>(DisableAttachedEffectOnActiveChanged));
			onIsGhostChanged = (Action<bool>)Delegate.Combine(onIsGhostChanged, new Action<bool>(OnIsGhostChanged));
			RegisterPropertyChangedCallback("Entity_AttackSpeed", OnAttackSpeedChanged);
			_attackMoveRatioIx = PushProperty("Animator_RigidBodyVelocityRatio", 0f);
			RegisterPropertyChangedCallback("Entity_AttackMoveRatio", OnAttackMoveChanged);
			AttachEffectOverrides();
			_equipedWeaponID = weaponID;
			_subAttackTargetList = new List<BaseMonoEntity>();
			UploadFaceTexture();
			InitFaceAnimation();
			PostInit();
			if (GlobalVars.ENABLE_CONTINUOUS_DETECT_MODE)
			{
				_rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			}
		}

		protected override void PostInit()
		{
			base.PostInit();
			InitMaterials();
			WeaponData.WeaponModelAndEffectAttach(_equipedWeaponID, AvatarTypeName, this);
			InitDynamicBone();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			switchState = AvatarSwitchState.OnStage;
			if (config != null)
			{
				ApplyAnimatorProperties();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			switchState = AvatarSwitchState.OffStage;
			_waitMoveSoundCoroutine = null;
		}

		protected override void ApplyAnimatorProperties()
		{
			base.ApplyAnimatorProperties();
			SyncAnimatorMoveSpeed();
		}

		protected void InitController()
		{
			_inputController = new KianaInputController(this);
			_aiController = new BTreeAvatarAIController(this);
			_controlData = AvatarControlData.emptyControlData;
			_lastFrameControlData = new AvatarControlData();
		}

		private void InitSkillAnimatorEventPattern()
		{
			foreach (string key in config.Skills.Keys)
			{
				ConfigAvatarSkill configAvatarSkill = config.Skills[key];
				if (configAvatarSkill.AnimatorEventPattern != null)
				{
					int i = 0;
					for (int num = configAvatarSkill.AnimatorStateNames.Length; i < num; i++)
					{
						AttachAnimatorEventPattern(Animator.StringToHash(configAvatarSkill.AnimatorStateNames[i]), configAvatarSkill.AnimatorEventPattern);
					}
				}
			}
		}

		public AvatarControlData GetActiveControlData()
		{
			if (_muteControlCount > 0)
			{
				return AvatarControlData.emptyControlData;
			}
			return (!_hasUpdatedControlThisFrame) ? _lastFrameControlData : _controlData;
		}

		public void ForceUseAIController()
		{
			_inputController.SetActive(false);
			_aiController.SetActive(true);
		}

		public void SetCountedMuteControl(bool mute)
		{
			_muteControlCount += (mute ? 1 : (-1));
			if (_muteControlCount < 0)
			{
				_muteControlCount = 0;
			}
		}

		public bool IsControlMuted()
		{
			return _muteControlCount > 0;
		}

		public void RefreshController()
		{
			_inputController.controlData.FrameReset();
			_aiController.controlData.FrameReset();
			bool flag = Singleton<AvatarManager>.Instance.IsLocalAvatar(GetRuntimeID());
			bool isAutoBattle = Singleton<AvatarManager>.Instance.isAutoBattle;
			LevelActor levelActor = (LevelActor)Singleton<EventManager>.Instance.GetActor(562036737u);
			if (levelActor.levelMode == LevelActor.Mode.Single)
			{
				_inputController.SetActive(flag);
				_aiController.SetActive(isAutoBattle);
			}
			else if (levelActor.levelMode == LevelActor.Mode.Multi)
			{
				_inputController.SetActive(flag);
				_aiController.SetActive(isAutoBattle || !flag);
			}
			else if (levelActor.levelMode == LevelActor.Mode.MultiRemote)
			{
				_inputController.SetActive(flag);
				_aiController.SetActive(false);
			}
			else if (levelActor.levelMode == LevelActor.Mode.NetworkedMP)
			{
				_inputController.SetActive(flag);
				_aiController.SetActive(false);
			}
			ClearAttackTriggers();
		}

		public IAIController GetActiveAIController()
		{
			return _aiController;
		}

		public BaseAvatarInputController GetInputController()
		{
			return _inputController;
		}

		public bool IsAIActive()
		{
			return _aiController.active;
		}

		public virtual bool IsDeadAlready()
		{
			return _isDeadAlready;
		}

		public override bool IsToBeRemove()
		{
			return _isToBeRemoved;
		}

		public override bool IsActive()
		{
			return _isAlive && base.gameObject.activeSelf;
		}

		public bool IsAlive()
		{
			return _isAlive;
		}

		public void onAnimatedHitBoxCreated(MonoAnimatedHitboxDetect hitBox, ConfigEntityAttackPattern attackPattern)
		{
			if (this.onAnimatedHitBoxCreatedCallBack != null)
			{
				this.onAnimatedHitBoxCreatedCallBack(hitBox, attackPattern);
			}
		}

		public void AddTargetToSubAttackList(BaseMonoEntity target)
		{
			if (!(target == null) && !_subAttackTargetList.Contains(target))
			{
				_subAttackTargetList.Add(target);
			}
		}

		public void ClearSubAttackList()
		{
			_subAttackTargetList.Clear();
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

		public void SetMuteAnimRetarget(bool mute)
		{
			_muteAnimRetarget = mute;
		}

		public override void SetDied(KillEffect killEffect)
		{
			_isAlive = false;
			CleanOwnedObjects();
			CastWaitingAudioEvent();
			switch (killEffect)
			{
			case KillEffect.KillNow:
				hitbox.enabled = false;
				_animator.SetBool("IsDead", true);
				break;
			case KillEffect.KillImmediately:
				_isToBeRemoved = true;
				DeadHandler();
				break;
			}
		}

		public virtual void Revive(Vector3 revivePosition)
		{
			_isAlive = true;
			_isDeadAlready = false;
			_transform.position = revivePosition;
			hitbox.enabled = true;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			onAttackTargetChanged = null;
			onLockDirectionChanged = null;
			onDie = null;
			ClearFaceAnimation();
			if (_aiController != null)
			{
				((BTreeAvatarAIController)_aiController).DisableBehavior();
			}
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
		}

		protected override void Update()
		{
			base.Update();
			UpdateControl();
			UpdateAttackSpeed();
			UpdateFaceAnimation();
			if (!_isShadowColorAdjusted || Application.isEditor)
			{
				AdjustShadowColors();
			}
		}

		protected virtual void UpdateControl()
		{
			if (_inputController.active)
			{
				_inputController.Core();
				_controlData = _inputController.controlData;
			}
			if (_aiController.active)
			{
				_aiController.Core();
				if (!_controlData.hasAnyControl)
				{
					_controlData = _aiController.controlData;
				}
			}
			_hasUpdatedControlThisFrame = true;
			if (_muteControlCount > 0)
			{
				return;
			}
			if (_controlData.hasOrderMove)
			{
				OrderMove = _controlData.orderMove;
			}
			if (_controlData.hasSetAttackTarget)
			{
				SetAttackTarget(_controlData.attackTarget);
			}
			if (_controlData.hasSteer && !IsAnimatorInTag(AvatarData.AvatarTagGroup.MuteJoyStickInput) && !IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackTargetLeadDirection) && IsWaitTransitionUnactive(_muteSteerIx))
			{
				SteerFaceDirectionTo(Vector3.Lerp(base.FaceDirection, _controlData.steerDirection, _steerLerpRatio * _controlData.lerpRatio * Time.deltaTime * TimeScale));
			}
			if (IsAnimatorInTag(AvatarData.AvatarTagGroup.AllowTriggerInput))
			{
				if (_controlData.useAttack)
				{
					TriggerAttack();
				}
				if (_controlData.useHoldAttack)
				{
					TriggerHoldAttack();
				}
				for (int i = 1; i < _controlData.useSkills.Length; i++)
				{
					if (_controlData.useSkills[i])
					{
						TriggerSkill(i);
					}
				}
			}
			if (IsWaitTransitionUnactive(_muteSteerIx) && IsWaitTransitionUnactive(_muteLockAttackTargetIx) && IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackTargetLeadDirection) && AttackTarget != null && AttackTarget.IsActive())
			{
				Vector3 forward = AttackTarget.XZPosition - XZPosition;
				SteerFaceDirectionTo(forward);
			}
			else if (_activeEnterSteerState > SkillEnterSteerState.Idle)
			{
				if (_activeEnterSteerState == SkillEnterSteerState.WaitingForStart)
				{
					float currentNormalizedTime = GetCurrentNormalizedTime();
					if (currentNormalizedTime > _activeEnterSteerOption.MaxSteerNormalizedTimeEnd)
					{
						_activeEnterSteerState = SkillEnterSteerState.Idle;
						_activeEnterSteerOption = null;
					}
					else if (currentNormalizedTime > _activeEnterSteerOption.MaxSteerNormalizedTimeStart)
					{
						_activeEnterSteerState = SkillEnterSteerState.Steering;
					}
				}
				else if (_activeEnterSteerState == SkillEnterSteerState.Steering)
				{
					if (GetCurrentNormalizedTime() > _activeEnterSteerOption.MaxSteerNormalizedTimeEnd)
					{
						_activeEnterSteerState = SkillEnterSteerState.Idle;
						_activeEnterSteerOption = null;
					}
					else if (AttackTarget != null && AttackTarget.IsActive())
					{
						Vector3 normalized = (AttackTarget.XZPosition - XZPosition).normalized;
						SteerFaceDirectionTo(Vector3.Slerp(base.FaceDirection, normalized, _activeEnterSteerOption.SteerLerpRatio * TimeScale * Time.deltaTime));
					}
					else if (_controlData.hasSteer)
					{
						Vector3 normalized = CalculateSteerTargetForwardWithOption(_controlData.steerDirection, _activeEnterSteerClampStart, _activeEnterSteerOption);
						SteerFaceDirectionTo(Vector3.Slerp(base.FaceDirection, normalized, _activeEnterSteerOption.SteerLerpRatio * TimeScale * Time.deltaTime));
					}
				}
			}
			if (AttackTarget != null && !AttackTarget.IsActive())
			{
				SetAttackTarget(null);
			}
			else if (_clearAttackTargetTimer > 0f)
			{
				_clearAttackTargetTimer -= Time.deltaTime * TimeScale;
				if (_clearAttackTargetTimer <= 0f)
				{
					SetAttackTarget(null);
				}
			}
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

		private void OnIsGhostChanged(bool isGhost)
		{
			if (_isAlive)
			{
				hitbox.enabled = !isGhost;
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
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(_runtimeID))
			{
				if (oldID != null && config.Skills[oldID].MuteCameraControl)
				{
					Singleton<CameraManager>.Instance.GetMainCamera().SetMuteManualCameraControl(false);
				}
				if (skillID != null && config.Skills[skillID].MuteCameraControl)
				{
					Singleton<CameraManager>.Instance.GetMainCamera().SetMuteManualCameraControl(true);
				}
			}
			if (skillID == null)
			{
				_skillMassRatio = 1f;
			}
			else if (config.Skills[skillID].MassRatio != 1f)
			{
				_skillMassRatio = config.Skills[skillID].MassRatio;
			}
			Singleton<LevelManager>.Instance.levelActor.SetLevelComboTimerState(LevelActor.ComboTimerState.Running);
			CheckDynamicBoneMute(oldID, skillID);
		}

		private void OnAttackSpeedChanged()
		{
			if (_attackSpeedState == AttackSpeedState.DuringAttackTime)
			{
				SetPropertyByStackIndex("Animator_OverallSpeedRatio", _attackSpeedTimeScaleIx, GetProperty("Entity_AttackSpeed"));
			}
		}

		private void OnAttackMoveChanged()
		{
			if (_attackSpeedState == AttackSpeedState.DuringAttackTime)
			{
				SetPropertyByStackIndex("Animator_RigidBodyVelocityRatio", _attackMoveRatioIx, GetProperty("Entity_AttackMoveRatio"));
			}
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			_lastFrameControlData.CopyFrom(_controlData);
			_hasUpdatedControlThisFrame = false;
			_controlData = AvatarControlData.emptyControlData;
			if (_inputController.active)
			{
				_inputController.controlData.FrameReset();
			}
			if (_aiController.active)
			{
				_aiController.controlData.FrameReset();
			}
		}

		private void InitMaterials()
		{
			InitOriginalShadowColorList();
			AdjustShadowColors();
		}

		private void InitOriginalShadowColorList()
		{
			_shadowColorAdjusterList = new List<ShadowColorAdjuster>();
			if (_instancedMaterialGroups.Count <= 0)
			{
				return;
			}
			MaterialGroup.RendererMaterials[] entries = _instancedMaterialGroups[0].entries;
			foreach (MaterialGroup.RendererMaterials rendererMaterials in entries)
			{
				Material[] materials = rendererMaterials.materials;
				foreach (Material material in materials)
				{
					if (material.HasProperty("_FirstShadowMultColor"))
					{
						_shadowColorAdjusterList.Add(new ShadowColorAdjuster(material));
					}
				}
			}
		}

		private void AdjustShadowColors()
		{
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			PostFXBase component = main.GetComponent<PostFXBase>();
			if (component != null)
			{
				float avatarShadowAdjust = component.AvatarShadowAdjust;
				for (int i = 0; i < _shadowColorAdjusterList.Count; i++)
				{
					_shadowColorAdjusterList[i].Apply(avatarShadowAdjust);
				}
				_isShadowColorAdjusted = true;
			}
		}

		[AnimationCallback]
		protected void RunOnRightFoot()
		{
			_animator.SetFloat("RunStepOnRight", 1f);
		}

		[AnimationCallback]
		protected void RunOnLeftFoot()
		{
			_animator.SetFloat("RunStepOnRight", 0f);
		}

		private void SyncAnimatorMoveSpeed()
		{
			float num = _moveSpeedRatio * (1f + GetProperty("Animator_MoveSpeedRatio")) - 1f;
			_animator.SetFloat("MoveSpeed", 1f + num * 0.35f);
		}

		public void TriggerAttack()
		{
			SetTrigger("TriggerAttack");
			ResetTrigger("TriggerWeapon");
		}

		public bool IsAttackHoldMode()
		{
			if (!_hasGotParameterHodeMode && _animator != null)
			{
				_isHodeMode = _animator.HasParameter("_IsHoldMode");
				_hasGotParameterHodeMode = true;
			}
			return _isHodeMode && _animator != null && _animator.GetBool("_IsHoldMode");
		}

		public void TriggerHoldAttack()
		{
			_animator.SetTrigger("TriggerHoldAttack");
			ResetTrigger("TriggerWeapon");
		}

		public virtual void TriggerSkill(int skillNum)
		{
			_animator.ResetTrigger("TriggerAttack");
			string skillIDBySkillNum = GetSkillIDBySkillNum(skillNum);
			if (!CanUseSkill(skillIDBySkillNum))
			{
				return;
			}
			if (IsSkillInstantTrigger(skillIDBySkillNum))
			{
				if (skillIDBySkillNum == "SKL_WEAPON")
				{
					TriggerWeaponInstantSkill(skillIDBySkillNum);
				}
				else
				{
					TriggerAvatarInstantSkill(skillIDBySkillNum);
				}
				return;
			}
			string skillTriggerBySkillNum = GetSkillTriggerBySkillNum(skillNum);
			SetTrigger(skillTriggerBySkillNum);
			if (skillTriggerBySkillNum == "TriggerWeapon")
			{
				bool flag = true;
				if (CurrentSkillID != null && config.Skills[CurrentSkillID].SkillType == AvatarSkillType.AttackStart)
				{
					flag = false;
				}
				if (flag)
				{
					SetTrigger("TriggerAttack");
				}
			}
		}

		private string GetSkillTriggerBySkillNum(int skillNum)
		{
			return (skillNum != 3) ? ("TriggerSkill_" + skillNum) : "TriggerWeapon";
		}

		private void TriggerAvatarInstantSkill(string skillID)
		{
			ConfigAvatarSkill configAvatarSkill = config.Skills[skillID];
			string instantTriggerEventName = GetInstantTriggerEventName(skillID);
			if (configAvatarSkill != null && !string.IsNullOrEmpty(instantTriggerEventName))
			{
				AnimEventHandler(instantTriggerEventName);
				switch (configAvatarSkill.SkillType)
				{
				case AvatarSkillType.AttackStart:
					Singleton<EventManager>.Instance.FireEvent(new EvtAttackStart(GetRuntimeID(), skillID));
					break;
				case AvatarSkillType.SkillStart:
					Singleton<EventManager>.Instance.FireEvent(new EvtSkillStart(GetRuntimeID(), skillID));
					break;
				}
			}
		}

		private void TriggerWeaponInstantSkill(string skillID)
		{
			StartWeaponSkillAbility();
			Singleton<EventManager>.Instance.FireEvent(new EvtSkillStart(GetRuntimeID(), skillID));
		}

		private void StartWeaponSkillAbility()
		{
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(GetRuntimeID());
			if (actor != null)
			{
				AvatarActor.SKillInfo skillInfo = actor.GetSkillInfo("SKL_WEAPON");
				ConfigEquipmentSkillEntry equipmentSkillConfig = EquipmentSkillData.GetEquipmentSkillConfig(skillInfo.avatarSkillID);
				EvtAbilityStart evtAbilityStart = new EvtAbilityStart(GetRuntimeID());
				evtAbilityStart.abilityName = equipmentSkillConfig.AbilityName;
				Singleton<EventManager>.Instance.FireEvent(evtAbilityStart);
			}
		}

		private string GetInstantTriggerEventName(string instantSkillID)
		{
			if (config.Skills.ContainsKey(instantSkillID))
			{
				return config.Skills[instantSkillID].InstantTriggerEvent;
			}
			return string.Empty;
		}

		private bool IsSkillInstantTrigger(string skillID)
		{
			bool result = false;
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(GetRuntimeID());
			if (actor != null)
			{
				if (skillID == "SKL_WEAPON")
				{
					AvatarActor.SKillInfo skillInfo = actor.GetSkillInfo("SKL_WEAPON");
					ConfigEquipmentSkillEntry equipmentSkillConfig = EquipmentSkillData.GetEquipmentSkillConfig(skillInfo.avatarSkillID);
					result = equipmentSkillConfig.IsInstantTrigger;
				}
				else if (actor.config.Skills.ContainsKey(skillID))
				{
					result = actor.config.Skills[skillID].IsInstantTrigger;
				}
			}
			return result;
		}

		private bool CanUseSkill(string skillName)
		{
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(GetRuntimeID());
			if (actor == null)
			{
				return false;
			}
			return actor.CanUseSkill(skillName);
		}

		private string GetSkillIDBySkillNum(int skillNum)
		{
			string empty = string.Empty;
			switch (skillNum)
			{
			case 1:
				return "SKL01";
			case 2:
				return "SKL02";
			case 3:
				return "SKL_WEAPON";
			default:
				throw new Exception("Invalid Type or State!");
			}
		}

		public virtual void RunBSStart()
		{
			if (IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackOrSkill))
			{
				_isFromAttackOrSkill = true;
				_steerLerpRatio = 1.3f;
			}
		}

		public virtual void RunBSStop()
		{
			if (!IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackOrSkill) && _isFromAttackOrSkill)
			{
				_isFromAttackOrSkill = false;
				_steerLerpRatio = 1f;
			}
		}

		public virtual void TriggerAppear()
		{
			_animator.SetTrigger("TriggerAppear");
		}

		private void SetAvatarMassByAnimatorTag(AnimatorStateInfo toState)
		{
			int tagHash = toState.tagHash;
			if (tagHash == AvatarData.AVATAR_APPEAR_TAG || tagHash == AvatarData.AVATAR_IDLESUB_TAG || tagHash == AvatarData.AVATAR_DIE_TAG)
			{
				_baseMassRatio = 100f;
			}
			else if (tagHash == AvatarData.AVATAR_SKL_TAG || tagHash == AvatarData.AVATAR_SKL_NO_TARGET_TAG)
			{
				_baseMassRatio = 1f;
			}
			else
			{
				_baseMassRatio = 1f;
			}
			SetMass(1f * _baseMassRatio * _skillMassRatio);
		}

		protected override void OnAnimatorStateChanged(AnimatorStateInfo fromState, AnimatorStateInfo toState)
		{
			base.OnAnimatorStateChanged(fromState, toState);
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
			SetAvatarMassByAnimatorTag(toState);
			SwitchAnimatorStateChanged(fromState, toState);
			UpdatePlaySoundOnAnimatorStateChanged(fromState, toState);
			ResetRigidbodyRotation();
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
			ConfigAvatarSkill configAvatarSkill = null;
			ConfigAvatarSkill configAvatarSkill2 = null;
			if (fromSkillID != null)
			{
				configAvatarSkill = config.Skills[fromSkillID];
			}
			if (toSkillID != null)
			{
				configAvatarSkill2 = config.Skills[toSkillID];
			}
			if (_activeCameraAnimName != null)
			{
				MonoAuxObject auxObject = Singleton<AuxObjectManager>.Instance.GetAuxObject(_runtimeID, _activeCameraAnimName);
				if (auxObject != null)
				{
					auxObject.SetDestroy();
					_activeCameraAnimName = null;
				}
			}
			_activeEnterSteerState = SkillEnterSteerState.Idle;
			_activeEnterSteerOption = null;
			if (configAvatarSkill2 != null && configAvatarSkill2.HighSpeedMovement)
			{
				PushHighspeedMovement();
			}
			if (configAvatarSkill != null && configAvatarSkill.HighSpeedMovement)
			{
				PopHighspeedMovement();
			}
			if (IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackOrSkill, fromState) && !IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackOrSkill, toState))
			{
				if (!_muteAnimRetarget)
				{
					if (AvatarData.RUN_CLEAR_ATTACK_TARGET && IsAnimatorInTag(AvatarData.AvatarTagGroup.Movement, toState))
					{
						SetAttackTarget(null);
					}
					else
					{
						ClearAttackTargetTimed();
					}
				}
				SetPropertyByStackIndex("Animator_RigidBodyVelocityRatio", _attackMoveRatioIx, 0f);
			}
			else if (toSkillID != null)
			{
				switch (configAvatarSkill2.SkillType)
				{
				case AvatarSkillType.AttackStart:
					Singleton<EventManager>.Instance.FireEvent(new EvtAttackStart(GetRuntimeID(), toSkillID));
					break;
				case AvatarSkillType.SkillStart:
					Singleton<EventManager>.Instance.FireEvent(new EvtSkillStart(GetRuntimeID(), toSkillID));
					break;
				}
				if (configAvatarSkill2.ForceMuteSteer && _animator.IsInTransition(0))
				{
					MuteSteerTillNextState();
				}
				if (IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackWithNoTarget, toState))
				{
					if (AvatarData.NO_TARGET_SKILL_CLEAR_AVATAR_TARGET && !_muteAnimRetarget)
					{
						SetAttackTarget(null);
					}
					if (configAvatarSkill2.EnterSteer != SkillEnterSetting.MuteFreeSteer && configAvatarSkill2.EnterSteer != SkillEnterSetting.MuteRetarget && configAvatarSkill2.EnterSteer != SkillEnterSetting.OnlyRetargetWhenNoTarget)
					{
						EnterSteerFaceDirectionWithSteerOption(GetActiveControlData().steerDirection, configAvatarSkill2.EnterSteerOption, true);
					}
				}
				else if ((IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackTargetLeadDirection, toState) || IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackSteerOnEnter, toState)) && configAvatarSkill2.EnterSteer != SkillEnterSetting.MuteRetarget)
				{
					if (!_muteAnimRetarget && (AttackTarget == null || !AttackTarget.IsActive() || (GetActiveControlData().hasSteer && configAvatarSkill2.EnterSteer != SkillEnterSetting.OnlyRetargetWhenNoTarget)))
					{
						SelectTarget();
					}
					if (AttackTarget == null || !AttackTarget.IsActive())
					{
						if (configAvatarSkill2.EnterSteer != SkillEnterSetting.MuteFreeSteer && configAvatarSkill2.EnterSteer != SkillEnterSetting.MuteRetarget && configAvatarSkill2.EnterSteer != SkillEnterSetting.OnlyRetargetWhenNoTarget)
						{
							EnterSteerFaceDirectionWithSteerOption(GetActiveControlData().steerDirection, configAvatarSkill2.EnterSteerOption, true);
						}
					}
					else
					{
						Vector3 targetForward = AttackTarget.XZPosition - XZPosition;
						EnterSteerFaceDirectionWithSteerOption(targetForward, configAvatarSkill2.EnterSteerOption, false);
						if (_animator.IsInTransition(0))
						{
							MuteSteerTillNextState();
						}
					}
				}
				else if (AttackTarget != null && AttackTarget.IsActive())
				{
					_clearAttackTargetTimer = 0f;
				}
				if (IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackTargetLeadDirection, toState))
				{
					SetPropertyByStackIndex("Animator_RigidBodyVelocityRatio", _attackMoveRatioIx, GetProperty("Entity_AttackMoveRatio"));
				}
				else
				{
					SetPropertyByStackIndex("Animator_RigidBodyVelocityRatio", _attackMoveRatioIx, 0f);
				}
			}
			SetCurrentSkillID(toSkillID);
		}

		public bool IsAnimatorInTag(AvatarData.AvatarTagGroup tagGroup, AnimatorStateInfo stateInfo)
		{
			return AvatarData.AVATAR_TAG_GROUPS[(int)tagGroup].Contains(stateInfo.tagHash);
		}

		public bool IsAnimatorInTag(AvatarData.AvatarTagGroup tagGroup)
		{
			return IsAnimatorInTag(tagGroup, _animator.GetCurrentAnimatorStateInfo(0));
		}

		protected override void OnAnimatorMove()
		{
			base.OnAnimatorMove();
			if (IsAnimatorInTag(AvatarData.AvatarTagGroup.Movement))
			{
				_rigidbody.velocity *= _moveSpeedRatio + GetProperty("Animator_MoveSpeedRatio");
			}
			_rigidbody.velocity *= 1f + GetProperty("Animator_RigidBodyVelocityRatio");
		}

		protected void MuteSteerTillNextState()
		{
			StartWaitTransitionState(_muteSteerIx);
		}

		protected void MuteLockAttackTargetTillNextState()
		{
			StartWaitTransitionState(_muteLockAttackTargetIx);
		}

		private void EnterSteerFaceDirectionWithSteerOption(Vector3 targetForward, SkillEnterSteerOption option, bool isFreeSteer)
		{
			if (option == null)
			{
				if (!isFreeSteer || GetActiveControlData().hasSteer)
				{
					SteerFaceDirectionTo(targetForward);
				}
			}
			else if (option.SteerType == SkillEnterSteerOption.EnterSteerType.Instant)
			{
				if (!isFreeSteer || GetActiveControlData().hasSteer)
				{
					SteerFaceDirectionTo(CalculateSteerTargetForwardWithOption(targetForward, base.FaceDirection, option));
				}
			}
			else if (option.MuteSteerWhenNoEnemy && _subAttackTargetList.Count <= 0 && _attackTarget == null)
			{
				MuteSteerTillNextState();
			}
			else
			{
				_activeEnterSteerOption = option;
				_activeEnterSteerClampStart = base.FaceDirection;
				_activeEnterSteerState = SkillEnterSteerState.WaitingForStart;
			}
		}

		private Vector3 CalculateSteerTargetForwardWithOption(Vector3 targetForward, Vector3 clampBaseForward, SkillEnterSteerOption option)
		{
			float value = Miscs.AngleFromToIgnoreY(clampBaseForward, targetForward);
			value = Mathf.Clamp(value, 0f - option.MaxSteeringAngle, option.MaxSteeringAngle);
			return (Quaternion.AngleAxis(value, Vector3.up) * clampBaseForward).normalized;
		}

		public void SelectTarget()
		{
			_clearAttackTargetTimer = 0f;
			_attackTargetSelectAction(this);
		}

		public void ClearAttackTargetTimed(float duration = 0.5f)
		{
			if (!(AttackTarget == null))
			{
				_clearAttackTargetTimer = duration;
			}
		}

		public override void ClearAttackTriggers()
		{
			if (IsActive())
			{
				_animator.ResetTrigger("TriggerAttack");
				_animator.ResetTrigger("TriggerHoldAttack");
				ClearSkillTriggers();
			}
		}

		public void ClearSkillTriggers()
		{
			if (IsActive())
			{
				if (!_delayedSwapOutTriggered)
				{
					_animator.ResetTrigger("TriggerSwitchOut");
				}
				for (int i = 1; i <= 3; i++)
				{
					string skillTriggerBySkillNum = GetSkillTriggerBySkillNum(i);
					_animator.ResetTrigger(skillTriggerBySkillNum);
				}
			}
		}

		[AnimationCallback]
		public override void ClearAttackTarget()
		{
			ClearAttackTriggers();
			SetAttackTarget(null);
		}

		public override BaseMonoEntity GetAttackTarget()
		{
			return AttackTarget;
		}

		public override void TriggerAttackPattern(string animEventID, LayerMask layerMask)
		{
			ConfigAvatarAnimEvent configAvatarAnimEvent = SharedAnimEventData.ResolveAnimEvent(config, animEventID);
			if (configAvatarAnimEvent.CameraShake != null && configAvatarAnimEvent.CameraShake.ShakeOnNotHit)
			{
				AttackPattern.ActCameraShake(configAvatarAnimEvent.CameraShake);
			}
			if (configAvatarAnimEvent.AttackPattern != null)
			{
				configAvatarAnimEvent.AttackPattern.patternMethod(animEventID, configAvatarAnimEvent.AttackPattern, this, layerMask);
			}
		}

		public virtual void BeHit(int frameHalt, AttackResult.AnimatorHitEffect hitEffect, AttackResult.AnimatorHitEffectAux hitEffectAux, KillEffect killEffect, BeHitEffect beHitEffect, float aniDamageRatio, Vector3 hitForward, float retreatVelocity, uint sourceID, bool targetLockSource, bool doSteerToHitForward)
		{
			if (!IsActive())
			{
				return;
			}
			if (hitEffect == AttackResult.AnimatorHitEffect.ThrowDown || hitEffect == AttackResult.AnimatorHitEffect.ThrowBlow)
			{
				hitEffect = AttackResult.AnimatorHitEffect.KnockDown;
			}
			_animator.ResetTrigger("TriggerKnockDown");
			_animator.SetFloat("DamageRatio", aniDamageRatio);
			if (hitEffect > AttackResult.AnimatorHitEffect.Light)
			{
				_animator.SetTrigger("TriggerHit");
				ClearSkillEffect(null);
				ClearAttackTriggers();
				CastWaitingAudioEvent();
				switch (hitEffect)
				{
				case AttackResult.AnimatorHitEffect.KnockDown:
					_animator.SetTrigger("TriggerKnockDown");
					break;
				case AttackResult.AnimatorHitEffect.FaceAttacker:
					doSteerToHitForward = true;
					break;
				}
				if (onBeHitCanceled != null)
				{
					onBeHitCanceled(CurrentSkillID);
				}
			}
			if (doSteerToHitForward)
			{
				SteerFaceDirectionTo(-hitForward);
				MuteSteerTillNextState();
			}
			if (targetLockSource)
			{
				BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(sourceID);
				if (entity != null && entity.IsActive() && entity is BaseMonoMonster)
				{
					SetAttackTarget(entity);
					ClearAttackTargetTimed();
				}
			}
			FrameHalt(frameHalt);
		}

		public void ClearHitTrigger()
		{
			_animator.ResetTrigger("TriggerHit");
		}

		public Transform GetFollowTransform(uint followMode)
		{
			if (followMode == 1)
			{
				return _transform;
			}
			throw new Exception("Invalid Type or State!");
		}

		[AnimationCallback]
		public void TriggerCameraAnimation(string cameraAnimName)
		{
			PlayAvatarCameraAnimation(cameraAnimName, MainCameraFollowState.EnterPolarMode.NearestPointOnSphere, true);
		}

		private void PlayAvatarCameraAnimation(string cameraAnimName, MainCameraFollowState.EnterPolarMode enterPolarMode, bool exitTransitionLerp)
		{
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(_runtimeID))
			{
				MonoAuxObject monoAuxObject = Singleton<AuxObjectManager>.Instance.CreateSimpleAuxObject(cameraAnimName, _runtimeID);
				_activeCameraAnimName = cameraAnimName;
				monoAuxObject.transform.parent = _transform;
				MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
				mainCamera.PlayAvatarCameraAnimationThenTransitToFollow(monoAuxObject.GetComponent<Animation>(), this, enterPolarMode, exitTransitionLerp);
			}
		}

		[AnimationCallback]
		private void TriggerCameraPullFurther(float time)
		{
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(_runtimeID))
			{
				MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
				mainCamera.SetTimedPullZ(1.9f, 0f, 0f, 0f, time, 0f, string.Empty);
			}
		}

		[AnimationCallback]
		private void TriggerCameraPullFar(float time)
		{
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(_runtimeID))
			{
				MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
				mainCamera.SetTimedPullZ(1.3f, 0f, 0f, 0f, time, 0f, string.Empty);
			}
		}

		[AnimationCallback]
		private void TriggerCameraPushNear(float time)
		{
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(_runtimeID))
			{
				MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
				mainCamera.SetTimedPullZ(0.8f, 0f, 0f, 0f, time, 0f, string.Empty);
			}
		}

		[AnimationCallback]
		private void TrggerCameraRotateToFaceDirection()
		{
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(_runtimeID))
			{
				MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
				mainCamera.SetRotateToFaceDirection();
			}
		}

		[AnimationCallback]
		public void TriggerAttackScreenShake(string attackName)
		{
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(_runtimeID))
			{
				ConfigEntityCameraShake cameraShake = SharedAnimEventData.ResolveAnimEvent(config, attackName).CameraShake;
				AttackPattern.ActCameraShake(cameraShake);
			}
		}

		protected override void OnSkillEffectClear(string oldID, string skillID)
		{
			if (skillID != null)
			{
				ConfigAvatarSkill configAvatarSkill = config.Skills[skillID];
				if (configAvatarSkill.NeedClearEffect)
				{
					ClearSkillEffect(skillID);
				}
			}
		}

		private void AttachEffectOverrides()
		{
			MonoEffectOverride monoEffectOverride = GetComponent<MonoEffectOverride>();
			if (monoEffectOverride == null)
			{
				monoEffectOverride = base.gameObject.AddComponent<MonoEffectOverride>();
			}
			monoEffectOverride.effectPredicates.AddRange(config.CommonArguments.EffectPredicates);
		}

		public override int AttachEffect(string effectPattern)
		{
			int num = base.AttachEffect(effectPattern);
			if (!base.gameObject.activeSelf)
			{
				Singleton<EffectManager>.Instance.SetIndexedEntityEffectPatternActive(num, false);
			}
			int index = _attachedEffects.SeekAddPosition();
			_attachedEffects[index] = num;
			return num;
		}

		public override void DetachEffect(int patternIx)
		{
			base.DetachEffect(patternIx);
			for (int i = 0; i < _attachedEffects.Count; i++)
			{
				if (_attachedEffects[i] == patternIx)
				{
					_attachedEffects[i] = -1;
					break;
				}
			}
		}

		private void DisableAttachedEffectOnActiveChanged(bool active)
		{
			if (active)
			{
				for (int i = 0; i < _attachedEffects.Count; i++)
				{
					if (_attachedEffects[i] != -1 && Singleton<EffectManager>.Instance.GetIndexedEntityEffectPattern(_attachedEffects[i]) != null)
					{
						Singleton<EffectManager>.Instance.SetIndexedEntityEffectPatternActive(_attachedEffects[i], true);
					}
				}
				return;
			}
			for (int j = 0; j < _attachedEffects.Count; j++)
			{
				if (_attachedEffects[j] != -1 && Singleton<EffectManager>.Instance != null && Singleton<EffectManager>.Instance.GetIndexedEntityEffectPattern(_attachedEffects[j]) != null)
				{
					Singleton<EffectManager>.Instance.SetIndexedEntityEffectPatternActive(_attachedEffects[j], false);
				}
			}
		}

		[AnimationCallback]
		private void TimeSlowTrigger(float time)
		{
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(_runtimeID) && AttackTarget != null && AttackTarget.IsActive() && Vector3.Distance(XZPosition, AttackTarget.XZPosition) < 2f)
			{
				Singleton<LevelManager>.Instance.levelActor.TimeSlow(time);
			}
		}

		[AnimationCallback]
		public override void DeadHandler()
		{
			if (!_isToBeRemoved)
			{
				if (CurrentSkillID != null)
				{
					SetCurrentSkillID(null);
				}
				if (onDie != null)
				{
					onDie(this);
				}
				_isDeadAlready = true;
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

		[AnimationCallback]
		public override void AnimEventHandler(string animEventID)
		{
			ConfigAvatarAnimEvent configAvatarAnimEvent = SharedAnimEventData.ResolveAnimEvent(config, animEventID);
			if (configAvatarAnimEvent == null || (_maskedAnimEvents != null && _maskedAnimEvents.Contains(animEventID)) || !_animEventPredicates.Contains(configAvatarAnimEvent.Predicate) || !_animEventPredicates.Contains(configAvatarAnimEvent.Predicate2))
			{
				return;
			}
			if (configAvatarAnimEvent.CameraShake != null && configAvatarAnimEvent.CameraShake.ShakeOnNotHit && Singleton<AvatarManager>.Instance.IsLocalAvatar(_runtimeID))
			{
				AttackPattern.ActCameraShake(configAvatarAnimEvent.CameraShake);
			}
			if (configAvatarAnimEvent.AttackPattern != null)
			{
				configAvatarAnimEvent.AttackPattern.patternMethod(animEventID, configAvatarAnimEvent.AttackPattern, this, AttackPattern.GetLayerMask(this));
			}
			if (configAvatarAnimEvent.TriggerAbility != null)
			{
				EvtAbilityStart evtAbilityStart = new EvtAbilityStart(_runtimeID);
				evtAbilityStart.abilityID = configAvatarAnimEvent.TriggerAbility.ID;
				evtAbilityStart.abilityName = configAvatarAnimEvent.TriggerAbility.Name;
				evtAbilityStart.abilityArgument = configAvatarAnimEvent.TriggerAbility.Argument;
				Singleton<EventManager>.Instance.FireEvent(evtAbilityStart);
			}
			if (configAvatarAnimEvent.PhysicsProperty != null)
			{
				ConfigAvatarPhysicsProperty physicsProperty = configAvatarAnimEvent.PhysicsProperty;
				if (physicsProperty.IsFreezeDirection)
				{
					MuteLockAttackTargetTillNextState();
				}
			}
			if (configAvatarAnimEvent.CameraAction != null && Singleton<AvatarManager>.Instance.IsLocalAvatar(_runtimeID))
			{
				DoCameraAction(configAvatarAnimEvent.CameraAction);
			}
			if (configAvatarAnimEvent.TimeSlow != null && Singleton<AvatarManager>.Instance.IsLocalAvatar(_runtimeID) && (configAvatarAnimEvent.TimeSlow.Force || (AttackTarget != null && AttackTarget.IsActive() && Vector3.Distance(XZPosition, AttackTarget.XZPosition) < 2f)))
			{
				Singleton<LevelManager>.Instance.levelActor.TimeSlow(configAvatarAnimEvent.TimeSlow.Duration, configAvatarAnimEvent.TimeSlow.SlowRatio, null);
			}
			if (configAvatarAnimEvent.TriggerEffectPattern != null)
			{
				TriggerEffectPattern(configAvatarAnimEvent.TriggerEffectPattern.EffectPattern);
			}
			if (configAvatarAnimEvent.TriggerTintCamera != null)
			{
				TriggerTint(configAvatarAnimEvent.TriggerTintCamera.RenderDataName, configAvatarAnimEvent.TriggerTintCamera.Duration, configAvatarAnimEvent.TriggerTintCamera.TransitDuration);
			}
		}

		public bool CheckAnimEventPredicate(string animEventID)
		{
			ConfigAvatarAnimEvent configAvatarAnimEvent = SharedAnimEventData.ResolveAnimEvent(config, animEventID);
			if (configAvatarAnimEvent == null || (_maskedAnimEvents != null && _maskedAnimEvents.Contains(animEventID)) || !_animEventPredicates.Contains(configAvatarAnimEvent.Predicate) || !_animEventPredicates.Contains(configAvatarAnimEvent.Predicate2))
			{
				return false;
			}
			return true;
		}

		public void DoCameraAction(ConfigAvatarCameraAction actionConfig)
		{
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			if (actionConfig is SetCameraDistance)
			{
				SetCameraDistance setCameraDistance = (SetCameraDistance)actionConfig;
				mainCamera.SetTimedPullZ(setCameraDistance.RadiusRatio, setCameraDistance.Elevation, setCameraDistance.CenterY, setCameraDistance.FOVOffset, setCameraDistance.Time, setCameraDistance.LerpTime, setCameraDistance.LerpCurve);
			}
			else if (actionConfig is PlayAvatarCameraAnimation)
			{
				PlayAvatarCameraAnimation playAvatarCameraAnimation = (PlayAvatarCameraAnimation)actionConfig;
				PlayAvatarCameraAnimation(playAvatarCameraAnimation.CameraAnimName, playAvatarCameraAnimation.EnterPolarMode, playAvatarCameraAnimation.ExitTransitionLerp);
			}
			else if (actionConfig is SuddenRecover)
			{
				Singleton<CameraManager>.Instance.GetMainCamera().SuddenRecover();
			}
		}

		[AnimationCallback]
		private void TriggerTintCamera(float duration)
		{
			TriggerTint("Effect_Tint", duration, 0.5f);
		}

		[AnimationCallback]
		private void ForceSteerBack()
		{
			SteerFaceDirectionTo(-base.FaceDirection);
		}

		[AnimationCallback]
		private void ShowCloseUpPanel(string name)
		{
			Singleton<MainUIManager>.Instance.ShowPage(new MonsterCloseUpPageContext(name));
		}

		[AnimationCallback]
		private void HideCloseUpPanel()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MonsterCloseUpEnd));
		}

		private void InitDynamicBone()
		{
			bool aVATAR_USE_DYNAMIC_BONE = GlobalVars.AVATAR_USE_DYNAMIC_BONE;
			_dynamicBones = base.gameObject.GetComponentsInChildren<DynamicBone>();
			DynamicBone[] dynamicBones = _dynamicBones;
			foreach (DynamicBone dynamicBone in dynamicBones)
			{
				dynamicBone.enabled = aVATAR_USE_DYNAMIC_BONE;
			}
		}

		private void CheckDynamicBoneMute(string oldSkillID, string newSkillID)
		{
			if (!GlobalVars.AVATAR_USE_DYNAMIC_BONE)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < muteDynamicBonesSkillIDs.Length; i++)
			{
				if (muteDynamicBonesSkillIDs[i] == oldSkillID)
				{
					flag = true;
				}
				if (muteDynamicBonesSkillIDs[i] == newSkillID)
				{
					flag2 = true;
				}
			}
			if (!flag && flag2)
			{
				for (int j = 0; j < _dynamicBones.Length; j++)
				{
					_dynamicBones[j].SetWeight(0f);
				}
			}
			else if (flag && !flag2)
			{
				for (int k = 0; k < _dynamicBones.Length; k++)
				{
					_dynamicBones[k].SetWeight(1f);
				}
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
				base.gameObject.layer = InLevelData.AVATAR_LAYER;
			}
		}

		public virtual void TriggerSwitchIn()
		{
			if (switchState == AvatarSwitchState.SwitchingOut)
			{
				_animator.Play(config.StateMachinePattern.SwitchInAnimatorStateHash, 0);
			}
			else
			{
				_animator.SetTrigger("TriggerSwitchIn");
			}
		}

		public virtual void TriggerSwitchOut(AvatarSwapOutType swapOutType)
		{
			switch (swapOutType)
			{
			case AvatarSwapOutType.Force:
				_animator.Play(config.StateMachinePattern.SwitchOutAnimatorStateHash, 0);
				break;
			case AvatarSwapOutType.Normal:
				_animator.SetTrigger("TriggerSwitchOut");
				break;
			case AvatarSwapOutType.Delayed:
				_animator.SetTrigger("TriggerSwitchOut");
				_delayedSwapOutTriggered = true;
				break;
			}
		}

		public bool IsSwitchOutTriggerSet()
		{
			return _animator.GetBool("TriggerSwitchOut");
		}

		public void ResetTriggerSwitchOut()
		{
			_animator.ResetTrigger("TriggerSwitchOut");
			_delayedSwapOutTriggered = false;
			switchState = AvatarSwitchState.OnStage;
		}

		private void SwitchOutFinishHandle(bool setInactive)
		{
			ClearAttackTriggers();
			OrderMove = false;
			if (CurrentSkillID != null)
			{
				SetCurrentSkillID(null);
			}
			if (setInactive)
			{
				base.gameObject.SetActive(false);
			}
		}

		private void SwitchAnimatorStateChanged(AnimatorStateInfo from, AnimatorStateInfo to)
		{
			if (from.shortNameHash != config.StateMachinePattern.SwitchInAnimatorStateHash || to.shortNameHash != config.StateMachinePattern.SwitchOutAnimatorStateHash)
			{
				if (to.shortNameHash == config.StateMachinePattern.SwitchInAnimatorStateHash || to.shortNameHash == config.StateMachinePattern.SwitchOutAnimatorStateHash)
				{
					SetNeedOverrideVelocity(true);
					SetOverrideVelocity(Vector3.zero);
					_rigidbody.detectCollisions = false;
					_animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				}
				else if (from.shortNameHash == config.StateMachinePattern.SwitchInAnimatorStateHash || from.shortNameHash == config.StateMachinePattern.SwitchOutAnimatorStateHash)
				{
					SetNeedOverrideVelocity(false);
					_rigidbody.detectCollisions = true;
					_animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
				}
			}
			if (to.shortNameHash == config.StateMachinePattern.SwitchOutAnimatorStateHash)
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtAvatarSwapOutStart(_runtimeID));
				switchState = AvatarSwitchState.SwitchingOut;
				if (_delayedSwapOutTriggered)
				{
					Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Avatar_SwitchRoleOut", XZPosition, base.FaceDirection, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
					_delayedSwapOutTriggered = false;
				}
			}
			else if (from.shortNameHash == config.StateMachinePattern.SwitchOutAnimatorStateHash)
			{
				if (to.shortNameHash == config.StateMachinePattern.SwitchInAnimatorStateHash)
				{
					SwitchOutFinishHandle(false);
					switchState = AvatarSwitchState.OnStage;
				}
				else
				{
					SwitchOutFinishHandle(true);
				}
			}
			if (from.shortNameHash == config.StateMachinePattern.SwitchInAnimatorStateHash)
			{
				Singleton<EventManager>.Instance.FireEvent(new EvtAvatarSwapInEnd(_runtimeID));
			}
		}

		public virtual void PickHPMedic(uint HPMedicRuntimeID)
		{
			if (base.gameObject.activeSelf)
			{
				FireEffect("Ability_HealHP_Pick");
			}
			BaseMonoDynamicObject baseMonoDynamicObject = Singleton<DynamicObjectManager>.Instance.TryGetDynamicObjectByRuntimeID(HPMedicRuntimeID);
			MonoGoods monoGoods = (MonoGoods)baseMonoDynamicObject;
			MonoEntityAudio component = GetComponent<MonoEntityAudio>();
			if (component != null && !monoGoods.muteSound && GetRuntimeID() == Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID())
			{
				AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(GetRuntimeID());
				if ((double)(float)actor.HP > (double)(float)actor.maxHP * 0.5)
				{
					component.PostPickupHPHigh();
				}
				else
				{
					component.PostPickupHPLow();
				}
			}
		}

		public virtual void PickupEquipItem(int rarity, uint equipItemRuntimeID)
		{
			List<MonoEffect> list = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue("Ability_GetEquipItem", this, false);
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				Singleton<DynamicObjectManager>.Instance.SetParticleColorByRarity(list[i].gameObject, rarity);
			}
			BaseMonoDynamicObject baseMonoDynamicObject = Singleton<DynamicObjectManager>.Instance.TryGetDynamicObjectByRuntimeID(equipItemRuntimeID);
			MonoGoods monoGoods = (MonoGoods)baseMonoDynamicObject;
			MonoEntityAudio component = GetComponent<MonoEntityAudio>();
			if (component != null && !monoGoods.muteSound && GetRuntimeID() == Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID())
			{
				component.PostPickupEquipItem();
			}
		}

		public virtual void PickupCoin(uint coinRuntimeID)
		{
			if (base.gameObject.activeSelf)
			{
				FireEffect("Ability_GetCoin");
			}
			BaseMonoDynamicObject baseMonoDynamicObject = Singleton<DynamicObjectManager>.Instance.TryGetDynamicObjectByRuntimeID(coinRuntimeID);
			MonoGoods monoGoods = (MonoGoods)baseMonoDynamicObject;
			MonoEntityAudio component = GetComponent<MonoEntityAudio>();
			if (component != null && !monoGoods.muteSound && GetRuntimeID() == Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID())
			{
				component.PostPickupCoin();
			}
		}

		private void UpdatePlaySoundOnAnimatorStateChanged(AnimatorStateInfo fromState, AnimatorStateInfo toState)
		{
			if (fromState.tagHash != AvatarData.AVATAR_MOVESUB_TAG && toState.tagHash == AvatarData.AVATAR_MOVESUB_TAG && _waitMoveSoundCoroutine == null && base.gameObject.activeInHierarchy)
			{
				_waitMoveSoundCoroutine = StartCoroutine(WaitPlayMoveSound());
			}
			if (fromState.tagHash == AvatarData.AVATAR_MOVESUB_TAG && toState.tagHash != AvatarData.AVATAR_MOVESUB_TAG && _waitMoveSoundCoroutine != null && base.gameObject.activeInHierarchy)
			{
				StopMoveSoundCoroutine();
			}
		}

		private IEnumerator WaitPlayMoveSound()
		{
			int random = UnityEngine.Random.Range(3, 11);
			float beginTime = 0f;
			while (beginTime <= (float)random)
			{
				beginTime += Time.deltaTime * TimeScale;
				yield return null;
			}
			MonoEntityAudio entityAudio = GetComponent<MonoEntityAudio>();
			if (entityAudio != null)
			{
				entityAudio.PostMove();
			}
			_waitMoveSoundCoroutine = null;
		}

		private void StopMoveSoundCoroutine()
		{
			if (_waitMoveSoundCoroutine != null)
			{
				StopCoroutine(_waitMoveSoundCoroutine);
				_waitMoveSoundCoroutine = null;
			}
		}

		private void InitFaceAnimation()
		{
			if (leftEyeRenderer == null || rightEyeRenderer == null || mouthRenderer == null || _faceAnimation != null)
			{
				return;
			}
			string text = AvatarTypeName.Substring(0, AvatarTypeName.IndexOf("_"));
			ConfigFaceAnimation faceAnimation = FaceAnimationData.GetFaceAnimation(text);
			if (!(faceAnimation == null))
			{
				_faceAnimation = new FaceAnimation();
				string path = "FaceAtlas/" + text + "/Eye/Atlas";
				string path2 = "FaceAtlas/" + text + "/Mouth/Atlas";
				if (_providerL == null)
				{
					_providerL = Resources.Load<AtlasMatInfoProvider>(path);
					_providerL.RetainReference();
				}
				if (_providerR == null)
				{
					_providerR = Resources.Load<AtlasMatInfoProvider>(path);
					_providerR.RetainReference();
				}
				if (_providerM == null)
				{
					_providerM = Resources.Load<AtlasMatInfoProvider>(path2);
					_providerM.RetainReference();
				}
				if (!(_providerL == null) && !(_providerR == null) && !(_providerM == null))
				{
					FacePartControl facePartControl = new FacePartControl();
					facePartControl.Init(_providerL, leftEyeRenderer);
					FacePartControl facePartControl2 = new FacePartControl();
					facePartControl2.Init(_providerR, rightEyeRenderer);
					FacePartControl facePartControl3 = new FacePartControl();
					facePartControl3.Init(_providerM, mouthRenderer);
					_faceAnimation.Setup(faceAnimation, facePartControl, facePartControl2, facePartControl3);
				}
			}
		}

		private void UpdateFaceAnimation()
		{
			if (_faceAnimation != null)
			{
				_faceAnimation.Process(Time.deltaTime);
			}
		}

		private void ClearFaceAnimation()
		{
			if (_providerL != null)
			{
				if (_providerL.ReleaseReference())
				{
					Resources.UnloadAsset(_providerL);
				}
				_providerL = null;
			}
			if (_providerR != null)
			{
				if (_providerR.ReleaseReference())
				{
					Resources.UnloadAsset(_providerR);
				}
				_providerR = null;
			}
			if (_providerM != null)
			{
				if (_providerM.ReleaseReference())
				{
					Resources.UnloadAsset(_providerM);
				}
				_providerM = null;
			}
		}

		private void UploadFaceTexture()
		{
			if (leftEyeRenderer != null)
			{
				leftEyeRenderer.sharedMaterial.mainTexture = leftEyeRenderer.sharedMaterial.mainTexture;
			}
			if (rightEyeRenderer != null)
			{
				rightEyeRenderer.sharedMaterial.mainTexture = rightEyeRenderer.sharedMaterial.mainTexture;
			}
			if (mouthRenderer != null)
			{
				mouthRenderer.sharedMaterial.mainTexture = mouthRenderer.sharedMaterial.mainTexture;
			}
		}

		[AnimationCallback]
		protected void TriggerFaceAnimation(string name)
		{
			if (_faceAnimation != null)
			{
				_faceAnimation.PlayFaceAnimation(name, FaceAnimationPlayMode.Clamp);
			}
		}

		public void DetachWeapon()
		{
			ConfigWeapon weaponConfig = WeaponData.GetWeaponConfig(_equipedWeaponID);
			weaponConfig.Attach.GetDetachHandler()(weaponConfig.Attach, this, AvatarTypeName);
		}

		public override void SetUseLocalController(bool enabled)
		{
			if (enabled)
			{
				RefreshController();
				return;
			}
			_inputController.SetActive(false);
			_aiController.SetActive(false);
		}

		public void SetAttackSelectMethod(Action<BaseMonoAvatar> selector)
		{
			_attackTargetSelectAction = selector;
		}

		public void DebugSetControllableAI()
		{
			BTreeAvatarAIController bTreeAvatarAIController = GetActiveAIController() as BTreeAvatarAIController;
			bTreeAvatarAIController.ChangeBehavior("test/AvatarAutoBattleBehavior_Attack_Test");
			bTreeAvatarAIController.SetBehaviorVariable("DoAttack", true);
			bTreeAvatarAIController.SetActive(true);
			_inputController.SetActive(true);
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

		uint IRetreatable.GetRuntimeID()
		{
			return GetRuntimeID();
		}

		string IRetreatable.GetCurrentNamedState()
		{
			return GetCurrentNamedState();
		}
	}
}
