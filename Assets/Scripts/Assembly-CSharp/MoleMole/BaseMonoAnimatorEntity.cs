using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Animator))]
	public abstract class BaseMonoAnimatorEntity : BaseMonoAbilityEntity, IFadeOff, IFrameHaltable, IWeaponAttacher
	{
		public class SpecialStateMaterialData
		{
			public Material material;

			public MaterialColorModifier.Multiplier colorMultiplier;

			public Shader originalShader;
		}

		private class ColorAdjuster
		{
			public class PropIdOrigColorPair
			{
				public int propId;

				public Color origColor;
			}

			public Material material;

			public List<PropIdOrigColorPair> propIdOrigColorList;

			public ColorAdjuster(Material mat, string[] propNames)
			{
				material = mat;
				propIdOrigColorList = new List<PropIdOrigColorPair>();
				foreach (string text in propNames)
				{
					if (mat.HasProperty(text))
					{
						propIdOrigColorList.Add(new PropIdOrigColorPair
						{
							propId = Shader.PropertyToID(text),
							origColor = mat.GetColor(text)
						});
					}
				}
			}

			public void Apply(Color adjustColor)
			{
				for (int i = 0; i < propIdOrigColorList.Count; i++)
				{
					PropIdOrigColorPair propIdOrigColorPair = propIdOrigColorList[i];
					if (material != null)
					{
						material.SetColor(propIdOrigColorPair.propId, propIdOrigColorPair.origColor * adjustColor);
					}
				}
			}

			public bool IsEmpty()
			{
				return propIdOrigColorList.Count == 0;
			}
		}

		private class LayerFader
		{
			public int layer;

			public float fromWeight;

			public float toWeight;

			public float t;

			public float duration;

			public bool isDone;
		}

		protected enum WaitTransitionState
		{
			Idle = 0,
			WaitForTransition = 1,
			DuringTransition = 2,
			TransitionDone = 3
		}

		public class MaterialFadeSetting
		{
			public float fadeDistance;

			public float fadeOffset;

			public bool recorded;

			public MaterialFadeSetting(float distance, float offset)
			{
				fadeDistance = distance;
				fadeOffset = offset;
				recorded = true;
			}
		}

		private class AnimatorEventPatternProcessItem
		{
			public AnimatorEventPattern[] patterns;

			public float lastTime;
		}

		public delegate void CollisionCallback(int layer, Vector3 forward);

		protected const float MOVE_SPEED_LIMIT = 340f;

		protected const string RANDOM_PARAM = "Random";

		private const int MAX_ADDITIVE_VELOCITY_INDEX = 20;

		private const string PREDICATE_SPLIT = ":";

		private const string DEFAULT_GROUP_NAME = "DEFAULT_MATERIAL_GROUP";

		protected const int SHADER_STACK_SIZE = 5;

		private const int FRAME_EXIT_ANIMATOR_STATE_BUFFER_COUNT = 4;

		public Action<AnimatorParameterEntry> onUserInputControllerChanged;

		public Action<AnimatorStateInfo, AnimatorStateInfo> onAnimatorStateChanged;

		public Action<Vector3> onSteerFaceDirectionSet;

		public Transform RootNode;

		protected Animator _animator;

		protected Transform _transform;

		protected Rigidbody _rigidbody;

		public ConfigAnimatorEntity animatorConfig;

		protected FrameHaltPlugin _frameHaltPlugin;

		protected ShaderTransitionPlugin _shaderTransitionPlugin;

		protected ShaderLerpPlugin _shaderLerpPlugin;

		private Material[] _materialList;

		protected List<SpecialStateMaterialData> _matListForSpecailState = new List<SpecialStateMaterialData>();

		private string[] propNamesForLightProb = new string[2] { "_Color", "_MainColor" };

		private MonoLightProbManager _lightProbManager;

		private MonoLightShadowManager _LightMapCorrectionManager;

		private List<ColorAdjuster> _bodyColorAdjusterList;

		private List<ColorAdjuster> _shadowColorAdjusterList;

		private bool _needSteer;

		private Vector3 _nextFaceDir = Vector3.zero;

		private bool _needOverrideSteer;

		private Vector3 _overrideSteer;

		private bool _needOverrideVelocity;

		private Vector3 _overrideVelocity;

		private int _hasAdditiveVelocityCount;

		private bool _muteAdditiveVelocity;

		private Dictionary<int, Vector3> _additiveVelocityDic = new Dictionary<int, Vector3>();

		private int _addictiveVelocityIndex = -1;

		private int _highspeedMovementCount;

		protected bool _animatePhysicsStarted;

		private float _recoverPosY;

		private float _uniformScale = 1f;

		private float _curMass;

		private List<LayerFader> _layerFaders;

		private List<string> _waitingAudioEvent = new List<string>();

		private float _lastTimeScale;

		private float _timeScale;

		private FixedStack<float> _timeScaleStack;

		protected List<WaitTransitionState> _transitionStates = new List<WaitTransitionState>();

		public Renderer[] renderers;

		public MaterialGroup[] materialGroups = new MaterialGroup[0];

		protected List<MaterialGroup> _instancedMaterialGroups;

		private Dictionary<int, MaterialFadeSetting> _fadeMaterialDic = new Dictionary<int, MaterialFadeSetting>();

		protected FixedStack<Shader> _shaderStack;

		protected string _currentNamedState;

		protected HashSet<string> _animEventPredicates;

		protected List<string> _maskedAnimEvents;

		protected List<string> _maskedTriggers;

		private bool _maskAllTriggers;

		[HideInInspector]
		public AttachPoint[] attachPoints = new AttachPoint[0];

		private Dictionary<int, List<string>> _activeAnimatorEventPatterns;

		protected AnimatorStateInfo _currrentAnimatorState;

		private AnimatorStateInfo _processingStateInfo;

		private AnimatorStateInfo _prevProcessingStateInfo;

		private bool _wasInTransition;

		private AnimatorEventPatternProcessItem _curProcessItem = new AnimatorEventPatternProcessItem();

		private AnimatorEventPatternProcessItem _prevProcessItem = new AnimatorEventPatternProcessItem();

		private AnimatorStateInfo[] _sameFrameExitStates = new AnimatorStateInfo[4];

		private int _sameFrameExitCount;

		private CollisionCallback _collisionCallback;

		private LayerMask _collisionLayerMask;

		private bool _waitingForCollision;

		public override Vector3 XZPosition
		{
			get
			{
				if (_transform == null)
				{
					if (base.transform == null)
					{
						return Vector3.zero;
					}
					return new Vector3(base.transform.position.x, 0f, base.transform.position.z);
				}
				return new Vector3(_transform.position.x, 0f, _transform.position.z);
			}
		}

		public Vector3 RootNodePosition
		{
			get
			{
				return RootNode.position;
			}
		}

		public Vector3 FaceDirection
		{
			get
			{
				return _transform.forward;
			}
		}

		public bool MuteAdditiveVelocity
		{
			get
			{
				return _muteAdditiveVelocity;
			}
			set
			{
				_muteAdditiveVelocity = value;
			}
		}

		public bool hasAdditiveVelocity
		{
			get
			{
				if (_muteAdditiveVelocity)
				{
					return false;
				}
				return _hasAdditiveVelocityCount > 0;
			}
		}

		public FixedStack<float> timeScaleStack
		{
			get
			{
				return _timeScaleStack;
			}
		}

		public override float TimeScale
		{
			get
			{
				return _timeScale;
			}
		}

		GameObject IWeaponAttacher.gameObject
		{
			get
			{
				return base.gameObject;
			}
		}

		public override float GetCurrentNormalizedTime()
		{
			if (_animator.IsInTransition(0))
			{
				return _animator.GetNextAnimatorStateInfo(0).normalizedTime;
			}
			return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
		}

		public void DisableShadow()
		{
			Paster[] componentsInChildren = GetComponentsInChildren<Paster>();
			Paster[] array = componentsInChildren;
			foreach (Paster paster in array)
			{
				UnityEngine.Object.Destroy(paster.gameObject);
			}
		}

		private void Start()
		{
		}

		public virtual void Awake()
		{
			if (RootNode == null)
			{
				throw new Exception("Invalid Type or State!");
			}
			_rigidbody = GetComponent<Rigidbody>();
			_animator = GetComponent<Animator>();
			if (_animator != null)
			{
				_animator.logWarnings = false;
			}
			_transform = base.gameObject.transform;
			_timeScaleStack = new FixedStack<float>(8);
			_timeScaleStack.Push(1f, true);
			_timeScale = (_lastTimeScale = _timeScaleStack.value * Singleton<LevelManager>.Instance.levelEntity.TimeScale);
			_layerFaders = new List<LayerFader>();
			_curMass = _rigidbody.mass;
		}

		protected virtual void InitPlugins()
		{
			_frameHaltPlugin = new FrameHaltPlugin(this);
			_shaderTransitionPlugin = new ShaderTransitionPlugin(this);
			_shaderLerpPlugin = new ShaderLerpPlugin(this);
		}

		public override void Init(uint runtimeID)
		{
			base.Init(runtimeID);
			_animEventPredicates = new HashSet<string> { "Always" };
			_activeAnimatorEventPatterns = new Dictionary<int, List<string>>();
			RegisterPropertyChangedCallback("Animator_MoveSpeedRatio", SyncAnimatorSpeed);
			RegisterPropertyChangedCallback("Animator_OverallSpeedRatio", SyncAnimatorSpeed);
			RegisterPropertyChangedCallback("Animator_OverallSpeedRatioMultiplied", SyncAnimatorSpeed);
			RegisterPropertyChangedCallback("Entity_MassRatio", SyncMass);
			onAnimatorBoolChanged = (Action)Delegate.Combine(onAnimatorBoolChanged, new Action(SyncAnimatorBools));
			onAnimatorIntChanged = (Action)Delegate.Combine(onAnimatorIntChanged, new Action(SyncAnimatorInts));
			onCurrentSkillIDChanged = (Action<string, string>)Delegate.Combine(onCurrentSkillIDChanged, new Action<string, string>(OnSkillEffectClear));
			_highspeedMovementCount = 0;
		}

		protected virtual void PostInit()
		{
			SetupGraphic();
			InitMaterialsForSpecialState();
		}

		protected Vector3 PickInitPosition(LayerMask mask, Vector3 initPos, float radius)
		{
			int num = 0;
			while (CollisionDetectPattern.SphereOverlapWithEntity(initPos + new Vector3(0f, 1.1f, 0f), radius, mask, base.gameObject) && num++ < 20)
			{
				Vector2 vector = UnityEngine.Random.insideUnitCircle * 0.5f;
				initPos.x += vector.x;
				initPos.z += vector.y;
			}
			return initPos;
		}

		public virtual void Preload()
		{
			base.transform.position = InLevelData.CREATE_INIT_POS;
			base.enabled = false;
			base.gameObject.SetActive(false);
		}

		public virtual void Enable()
		{
			base.enabled = true;
			base.gameObject.SetActive(true);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			onAnimatorStateChanged = null;
			onUserInputControllerChanged = null;
			onSteerFaceDirectionSet = null;
			if (_instancedMaterialGroups != null && _instancedMaterialGroups[0] != null)
			{
				_instancedMaterialGroups[0].Dispose();
			}
			for (int i = 0; i < materialGroups.Length; i++)
			{
				materialGroups[i].Dispose();
			}
		}

		protected virtual void FixedUpdate()
		{
			bool flag;
			Vector3 vector;
			if (_needOverrideSteer)
			{
				flag = true;
				vector = _overrideSteer;
			}
			else if (_needSteer)
			{
				flag = true;
				vector = _nextFaceDir;
			}
			else
			{
				flag = false;
				vector = Vector3.zero;
			}
			if (flag)
			{
				float num = Vector3.Angle(FaceDirection, vector);
				float num2 = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(FaceDirection, vector)));
				Quaternion quaternion = Quaternion.AngleAxis(num2 * num, Vector3.up);
				_rigidbody.MoveRotation(_rigidbody.rotation * quaternion);
				_needSteer = false;
				_needOverrideSteer = false;
			}
			if (_needOverrideVelocity)
			{
				_rigidbody.velocity = _overrideVelocity;
			}
			if (_animatePhysicsStarted)
			{
				ResetRigidbodyRotation();
			}
			FixedUpdatePlugins();
		}

		protected virtual void Update()
		{
			UpdatePlugins();
			_timeScale = Singleton<LevelManager>.Instance.levelEntity.TimeScale * _timeScaleStack.value * (1f + GetProperty("Entity_TimeScaleDelta"));
			if (_lastTimeScale != TimeScale)
			{
				OnTimeScaleChanged(TimeScale);
			}
			_lastTimeScale = TimeScale;
			bool flag = _animator.IsInTransition(0);
			for (int i = 0; i < _transitionStates.Count; i++)
			{
				if (_transitionStates[i] == WaitTransitionState.WaitForTransition)
				{
					if (flag)
					{
						_transitionStates[i] = WaitTransitionState.DuringTransition;
					}
				}
				else if (_transitionStates[i] == WaitTransitionState.DuringTransition && !flag)
				{
					_transitionStates[i] = WaitTransitionState.TransitionDone;
				}
			}
			UpdateLayerFading();
			ApplyLightProb();
			_instancedMaterialGroups[0].ApplyColorModifiers();
		}

		protected virtual void LateUpdate()
		{
			ProcessAnimatorStates();
		}

		private void OnDrawGizmos()
		{
		}

		public override void SteerFaceDirectionTo(Vector3 forward)
		{
			_nextFaceDir = forward;
			_nextFaceDir.y = 0f;
			_needSteer = true;
			if (onSteerFaceDirectionSet != null)
			{
				onSteerFaceDirectionSet(forward);
			}
		}

		public virtual void SetOverrideSteerFaceDirectionFrame(Vector3 overrideSteer)
		{
			_needOverrideSteer = true;
			_overrideSteer = overrideSteer;
		}

		public float GetLocomotionFloat(string name)
		{
			return _animator.GetFloat(name);
		}

		public void SetLocomotionFloat(int stateHash, float value, bool isUserInput = false)
		{
			if (isUserInput && onUserInputControllerChanged != null && value != _animator.GetFloat(stateHash))
			{
				onUserInputControllerChanged(new AnimatorParameterEntry
				{
					stateHash = stateHash,
					type = AnimatorControllerParameterType.Float,
					floatValue = value
				});
			}
			_animator.SetFloat(stateHash, value);
		}

		public void SetLocomotionFloat(string stateName, float value, bool isUserInput = false)
		{
			SetLocomotionFloat(Animator.StringToHash(stateName), value, isUserInput);
		}

		public bool GetLocomotionBool(string stateName)
		{
			return _animator.GetBool(stateName);
		}

		public void SetLocomotionBool(int stateHash, bool value, bool isUserInput = false)
		{
			if (isUserInput && onUserInputControllerChanged != null && value != _animator.GetBool(stateHash))
			{
				onUserInputControllerChanged(new AnimatorParameterEntry
				{
					stateHash = stateHash,
					type = AnimatorControllerParameterType.Bool,
					boolValue = value
				});
			}
			_animator.SetBool(stateHash, value);
		}

		public void SetLocomotionBool(string stateName, bool value, bool isUserInput = false)
		{
			SetLocomotionBool(Animator.StringToHash(stateName), value, isUserInput);
		}

		public int GetLocomotionInteger(string stateName)
		{
			return _animator.GetInteger(stateName);
		}

		public void SetLocomotionInteger(int stateHash, int value, bool isUserInput = false)
		{
			if (isUserInput && onUserInputControllerChanged != null && value != _animator.GetInteger(stateHash))
			{
				onUserInputControllerChanged(new AnimatorParameterEntry
				{
					stateHash = stateHash,
					type = AnimatorControllerParameterType.Bool,
					intValue = value
				});
			}
			_animator.SetInteger(stateHash, value);
		}

		public void SetLocomotionInteger(string stateName, int value, bool isUserInput = false)
		{
			SetLocomotionInteger(Animator.StringToHash(stateName), value, isUserInput);
		}

		public override void SetTrigger(string name)
		{
			if (base.gameObject.activeInHierarchy && !_maskAllTriggers && (_maskedTriggers == null || !_maskedTriggers.Contains(name)))
			{
				_animator.SetTrigger(name);
			}
		}

		public override void ResetTrigger(string name)
		{
			if (base.gameObject.activeInHierarchy)
			{
				_animator.ResetTrigger(name);
			}
		}

		public void PlayState(string stateName)
		{
			_animator.Play(stateName, 0);
		}

		public void SetLocomotionRandom(int n)
		{
			int value = UnityEngine.Random.Range(0, n);
			_animator.SetInteger("Random", value);
		}

		public void FrameHalt(int frameNum)
		{
			if (frameNum > 0)
			{
				_frameHaltPlugin.FrameHalt(frameNum);
			}
		}

		protected void ResetAllTriggers()
		{
			AnimatorControllerParameter[] parameters = _animator.parameters;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].type == AnimatorControllerParameterType.Trigger)
				{
					_animator.ResetTrigger(parameters[i].nameHash);
				}
			}
		}

		public override void SetNeedOverrideVelocity(bool needOverrideVelocity)
		{
			_needOverrideVelocity = needOverrideVelocity;
		}

		public override void SetOverrideVelocity(Vector3 velocity)
		{
			_overrideVelocity = velocity;
		}

		public override void SetHasAdditiveVelocity(bool value)
		{
			bool flag = hasAdditiveVelocity;
			_hasAdditiveVelocityCount += (value ? 1 : (-1));
			_hasAdditiveVelocityCount = ((_hasAdditiveVelocityCount >= 0) ? _hasAdditiveVelocityCount : 0);
			if (flag != value && onHasAdditiveVelocityChanged != null)
			{
				onHasAdditiveVelocityChanged(value);
			}
		}

		public override void SetAdditiveVelocity(Vector3 velocity)
		{
			_additiveVelocityDic.Clear();
			if (velocity != Vector3.zero)
			{
				_additiveVelocityDic.Add(_additiveVelocityDic.Count, velocity);
			}
		}

		public override int AddAdditiveVelocity(Vector3 velocity)
		{
			int num = -1;
			if (velocity != Vector3.zero)
			{
				_addictiveVelocityIndex++;
				if (_addictiveVelocityIndex > 20)
				{
					_addictiveVelocityIndex = 0;
				}
				num = _addictiveVelocityIndex;
				if (HasAdditiveVelocityOfIndex(num))
				{
					SetAdditiveVelocityOfIndex(velocity, num);
				}
				else
				{
					_additiveVelocityDic.Add(num, velocity);
				}
			}
			return num;
		}

		public override bool HasAdditiveVelocityOfIndex(int index)
		{
			return _additiveVelocityDic.ContainsKey(index);
		}

		public override void SetAdditiveVelocityOfIndex(Vector3 velocity, int index)
		{
			if (HasAdditiveVelocityOfIndex(index))
			{
				if (velocity == Vector3.zero)
				{
					_additiveVelocityDic.Remove(index);
				}
				else
				{
					_additiveVelocityDic[index] = velocity;
				}
			}
		}

		protected virtual void OnAnimatorMove()
		{
			if (_needOverrideVelocity)
			{
				return;
			}
			_rigidbody.velocity = _animator.velocity;
			if (!hasAdditiveVelocity)
			{
				return;
			}
			Vector3 zero = Vector3.zero;
			foreach (Vector3 value in _additiveVelocityDic.Values)
			{
				zero += value;
			}
			_rigidbody.velocity += zero * TimeScale;
		}

		public void DisableRootMotionAndCollision()
		{
			_animator.applyRootMotion = false;
			_rigidbody.detectCollisions = false;
		}

		public override void PushHighspeedMovement()
		{
			_highspeedMovementCount++;
			if (_highspeedMovementCount == 1)
			{
				_rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			}
		}

		public override void PopHighspeedMovement()
		{
			_highspeedMovementCount--;
			if (_highspeedMovementCount == 0)
			{
				_rigidbody.collisionDetectionMode = (GlobalVars.ENABLE_CONTINUOUS_DETECT_MODE ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete);
			}
		}

		protected void StartAnimatePhysics()
		{
			_recoverPosY = _rigidbody.position.y;
			_animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
			_animatePhysicsStarted = true;
			_rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		}

		protected void StopAnimatePhysics()
		{
			_animator.updateMode = AnimatorUpdateMode.Normal;
			_rigidbody.collisionDetectionMode = (GlobalVars.ENABLE_CONTINUOUS_DETECT_MODE ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete);
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(ResetRotationIter());
			}
			else
			{
				ResetAnimatedPhysics();
			}
			_animatePhysicsStarted = false;
		}

		public void ResetRigidbodyRotation()
		{
			Vector3 eulerAngles = _rigidbody.rotation.eulerAngles;
			if (eulerAngles.x != 0f || eulerAngles.z != 0f)
			{
				eulerAngles.x = 0f;
				eulerAngles.z = 0f;
				_rigidbody.rotation = Quaternion.Euler(eulerAngles);
			}
		}

		private void ResetAnimatedPhysics()
		{
			ResetRigidbodyRotation();
			Vector3 position = _rigidbody.position;
			position.y = _recoverPosY;
			_rigidbody.position = position;
		}

		private IEnumerator ResetRotationIter()
		{
			yield return null;
			ResetAnimatedPhysics();
		}

		public void ClearSkillEffect(string notClearSkillID)
		{
			List<MonoEffect> effectsByOwner = Singleton<EffectManager>.Instance.GetEffectsByOwner(GetRuntimeID());
			int i = 0;
			for (int count = effectsByOwner.Count; i < count; i++)
			{
				MonoEffect monoEffect = effectsByOwner[i];
				if (!string.IsNullOrEmpty(monoEffect.belongSkillName) && (string.IsNullOrEmpty(notClearSkillID) || monoEffect.belongSkillName != notClearSkillID))
				{
					monoEffect.SetDestroyImmediately();
				}
			}
		}

		protected virtual void HandlePhysicsProperty(Collider hitbox, ConfigEntityPhysicsProperty physicsProperty)
		{
		}

		public void SetUniformScale(float uniformScale)
		{
			_uniformScale = uniformScale;
			_transform.localScale *= uniformScale;
		}

		protected void SetMass(float mass)
		{
			_curMass = mass;
			SyncMass();
		}

		protected virtual void UpdatePlugins()
		{
			_frameHaltPlugin.Core();
			_shaderTransitionPlugin.Core();
			_shaderLerpPlugin.Core();
		}

		protected virtual void FixedUpdatePlugins()
		{
		}

		[AnimationCallback]
		public void TriggerEffectPattern(string patternName)
		{
			string[] array = patternName.Split(':');
			if (array.Length == 2)
			{
				TriggerEffectPattern(array[0], array[1], null);
			}
			else
			{
				TriggerEffectPattern(patternName, null, null);
			}
		}

		public virtual void TriggerEffectPattern(string patternName, string predicate1, string predicate2)
		{
			if ((predicate1 == null || _animEventPredicates.Contains(predicate1)) && (predicate2 == null || _animEventPredicates.Contains(predicate2)))
			{
				List<MonoEffect> list = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(patternName, XZPosition, FaceDirection, Vector3.one * _uniformScale, this);
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					MonoEffect monoEffect = list[i];
					monoEffect.belongSkillName = CurrentSkillID;
				}
			}
		}

		[AnimationCallback]
		public virtual void CleanOwnedObjects()
		{
			StopAllEffects();
			Singleton<AuxObjectManager>.Instance.ClearHitBoxDetectByOwnerEvade(_runtimeID);
			CastWaitingAudioEvent();
		}

		[AnimationCallback]
		public void StopAllEffects()
		{
			Singleton<EffectManager>.Instance.ClearEffectsByOwner(_runtimeID);
		}

		[AnimationCallback]
		public void StopAllEffectsImmediately()
		{
			Singleton<EffectManager>.Instance.ClearEffectsByOwnerImmediately(_runtimeID);
		}

		protected void TriggerTint(string renderDataName, float duration, float transitDuration)
		{
			Singleton<StageManager>.Instance.GetPerpStage().TriggerTint(renderDataName, duration, transitDuration);
		}

		public void StartFadeAnimatorLayerWeight(int layer, float weight, float duration)
		{
			LayerFader layerFader = null;
			for (int i = 0; i < _layerFaders.Count; i++)
			{
				if (_layerFaders[i] != null && _layerFaders[i].layer == layer)
				{
					layerFader = _layerFaders[i];
					layerFader.fromWeight = _animator.GetLayerWeight(layer);
					layerFader.toWeight = weight;
					layerFader.duration = duration;
					layerFader.t = 0f;
					layerFader.isDone = false;
					break;
				}
			}
			if (layerFader == null)
			{
				LayerFader layerFader2 = new LayerFader();
				layerFader2.layer = layer;
				layerFader2.fromWeight = _animator.GetLayerWeight(layer);
				layerFader2.toWeight = weight;
				layerFader2.duration = duration;
				layerFader = layerFader2;
				int index = _layerFaders.SeekAddPosition();
				_layerFaders[index] = layerFader;
			}
		}

		private void UpdateLayerFading()
		{
			float num = Time.deltaTime * TimeScale;
			for (int i = 0; i < _layerFaders.Count; i++)
			{
				LayerFader layerFader = _layerFaders[i];
				if (layerFader != null && !layerFader.isDone)
				{
					layerFader.t += num;
					if (layerFader.t > layerFader.duration)
					{
						_animator.SetLayerWeight(layerFader.layer, layerFader.toWeight);
						_layerFaders[i].isDone = true;
					}
					else
					{
						_animator.SetLayerWeight(layerFader.layer, Mathf.Lerp(layerFader.fromWeight, layerFader.toWeight, layerFader.t / layerFader.duration));
					}
				}
			}
		}

		public virtual float GetTargetAlpha()
		{
			return 1f;
		}

		public Material[] GetAllMaterials()
		{
			if (_materialList == null)
			{
				_materialList = _instancedMaterialGroups[_instancedMaterialGroups.Count - 1].GetAllMaterials();
			}
			return _materialList;
		}

		[AnimationCallback]
		public virtual void TriggerAudioPattern(string patternName)
		{
			string[] array = patternName.Split(':');
			if (array.Length == 2)
			{
				if (!_animEventPredicates.Contains(array[1]))
				{
					return;
				}
				patternName = array[0];
			}
			array = patternName.Split('#');
			patternName = array[0];
			string text = null;
			if (array.Length > 1)
			{
				text = array[1];
			}
			if (!patternName.StartsWith("VO_L") || GetRuntimeID() == Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID())
			{
				Singleton<WwiseAudioManager>.Instance.Post(patternName, base.gameObject);
				if (_waitingAudioEvent.Contains(patternName))
				{
					_waitingAudioEvent.Remove(patternName);
				}
				if (text != null && !_waitingAudioEvent.Contains(text))
				{
					_waitingAudioEvent.Add(text);
				}
			}
		}

		public virtual void StopAudioPattern(string name)
		{
		}

		public void CastWaitingAudioEvent()
		{
			int i = 0;
			for (int count = _waitingAudioEvent.Count; i < count; i++)
			{
				Singleton<WwiseAudioManager>.Instance.Post(_waitingAudioEvent[i], base.gameObject);
			}
			_waitingAudioEvent.Clear();
		}

		public virtual void OnTimeScaleChanged(float newTimeScale)
		{
			_animator.speed = (1f + GetProperty("Animator_OverallSpeedRatio")) * GetProperty("Animator_OverallSpeedRatioMultiplied") * newTimeScale;
		}

		public virtual void SetPause(bool pause)
		{
		}

		protected int AddWaitTransitionState()
		{
			_transitionStates.Add(WaitTransitionState.Idle);
			return _transitionStates.Count - 1;
		}

		protected void StartWaitTransitionState(int stateIx)
		{
			_transitionStates[stateIx] = ((!_animator.IsInTransition(0)) ? WaitTransitionState.WaitForTransition : WaitTransitionState.DuringTransition);
		}

		protected bool IsWaitTransitionUnactive(int stateIx)
		{
			return _transitionStates[stateIx] == WaitTransitionState.Idle || _transitionStates[stateIx] == WaitTransitionState.TransitionDone;
		}

		protected virtual void SetupGraphic()
		{
			_instancedMaterialGroups = new List<MaterialGroup>();
			MaterialGroup materialGroup = new MaterialGroup("DEFAULT_MATERIAL_GROUP", renderers);
			_instancedMaterialGroups.Add(materialGroup.GetInstancedMaterialGroup());
			_instancedMaterialGroups[0].ApplyTo(renderers);
		}

		public override void PushMaterialGroup(string targetGroupname)
		{
			_materialList = null;
			for (int i = 0; i < materialGroups.Length; i++)
			{
				if (materialGroups[i].groupName == targetGroupname)
				{
					_instancedMaterialGroups.Add(materialGroups[i].GetInstancedMaterialGroup());
					_instancedMaterialGroups[_instancedMaterialGroups.Count - 1].ApplyTo(renderers);
					break;
				}
			}
		}

		public override void PopMaterialGroup()
		{
			_materialList = null;
			_instancedMaterialGroups.RemoveAt(_instancedMaterialGroups.Count - 1);
			_instancedMaterialGroups[_instancedMaterialGroups.Count - 1].ApplyTo(renderers);
		}

		public override void SetShaderData(E_ShaderData dataType, bool bEnable)
		{
			MonoBuffShader_SpecialTransition buffShaderData = Singleton<ShaderDataManager>.Instance.GetBuffShaderData<MonoBuffShader_SpecialTransition>(dataType);
			_shaderTransitionPlugin.StartTransition(_matListForSpecailState, buffShaderData, bEnable);
		}

		public override void SetShaderDataLerp(E_ShaderData dataType, bool bEnable, float enableDuration = -1f, float disableDuration = -1f, bool bUseNewTexture = false)
		{
			MonoBuffShader_Lerp buffShaderData = Singleton<ShaderDataManager>.Instance.GetBuffShaderData<MonoBuffShader_Lerp>(dataType);
			int shaderIx = -1;
			if (bEnable && !string.IsNullOrEmpty(buffShaderData.NewShaderName))
			{
				TryInitShaderStack();
				Shader shader = Shader.Find(buffShaderData.NewShaderName);
				shaderIx = PushEffectShaderData(shader);
			}
			if (buffShaderData.Keyword != string.Empty)
			{
				for (int i = 0; i < _matListForSpecailState.Count; i++)
				{
					Material material = _matListForSpecailState[i].material;
					if (buffShaderData.Keyword == "DISTORTION")
					{
						material.SetOverrideTag("Distortion", "Character");
					}
					else
					{
						material.EnableKeyword(buffShaderData.Keyword);
					}
				}
			}
			if (bUseNewTexture)
			{
				for (int j = 0; j < _matListForSpecailState.Count; j++)
				{
					Material material2 = _matListForSpecailState[j].material;
					material2.SetTexture(buffShaderData.TexturePropertyName, buffShaderData.NewTexture);
				}
			}
			if (enableDuration > 0f)
			{
				buffShaderData.EnableDuration = enableDuration;
			}
			if (disableDuration > 0f)
			{
				buffShaderData.DisableDuration = disableDuration;
			}
			_shaderLerpPlugin.StartLerp(dataType, _matListForSpecailState, buffShaderData, bEnable, shaderIx);
		}

		private void InitMaterialsForSpecialState()
		{
			MaterialGroup materialGroup = _instancedMaterialGroups[0];
			for (int i = 0; i < materialGroup.entries.Length; i++)
			{
				for (int j = 0; j < materialGroup.entries[i].materials.Length; j++)
				{
					Material material = materialGroup.entries[i].materials[j];
					if (material.HasProperty("_SpecialState"))
					{
						_matListForSpecailState.Add(new SpecialStateMaterialData
						{
							material = material,
							colorMultiplier = materialGroup.entries[i].colorModifiers[j].AddMultiplier(),
							originalShader = material.shader
						});
					}
				}
			}
		}

		private void InitDataForLightProb()
		{
			if (_lightProbManager == null)
			{
				_lightProbManager = Singleton<StageManager>.Instance.GetPerpStage().lightProbManager;
			}
			if (_LightMapCorrectionManager == null)
			{
				_LightMapCorrectionManager = Singleton<StageManager>.Instance.GetPerpStage().lightMapCorrectManager;
			}
			if (_lightProbManager == null && _LightMapCorrectionManager == null)
			{
				return;
			}
			if (_bodyColorAdjusterList == null && _instancedMaterialGroups.Count > 0)
			{
				_bodyColorAdjusterList = new List<ColorAdjuster>();
				for (int i = 0; i < _instancedMaterialGroups[0].entries.Length; i++)
				{
					MaterialGroup.RendererMaterials rendererMaterials = _instancedMaterialGroups[0].entries[i];
					for (int j = 0; j < rendererMaterials.materials.Length; j++)
					{
						Material mat = rendererMaterials.materials[j];
						ColorAdjuster colorAdjuster = new ColorAdjuster(mat, propNamesForLightProb);
						if (!colorAdjuster.IsEmpty())
						{
							_bodyColorAdjusterList.Add(colorAdjuster);
						}
					}
				}
			}
			if (_shadowColorAdjusterList != null)
			{
				return;
			}
			_shadowColorAdjusterList = new List<ColorAdjuster>();
			Paster[] componentsInChildren = GetComponentsInChildren<Paster>();
			foreach (Paster paster in componentsInChildren)
			{
				ColorAdjuster colorAdjuster2 = new ColorAdjuster(paster.PasterMaterial, propNamesForLightProb);
				if (!colorAdjuster2.IsEmpty())
				{
					_shadowColorAdjusterList.Add(colorAdjuster2);
				}
			}
		}

		private void ApplyLightProb()
		{
			InitDataForLightProb();
			LightProbProperties ret = default(LightProbProperties);
			if (_lightProbManager != null && _lightProbManager.Evaluate(XZPosition, ref ret))
			{
				for (int i = 0; i < _instancedMaterialGroups[0].entries.Length; i++)
				{
					MaterialGroup.RendererMaterials rendererMaterials = _instancedMaterialGroups[0].entries[i];
					for (int j = 0; j < rendererMaterials.colorModifiers.Length; j++)
					{
						rendererMaterials.colorModifiers[j].Multiply(ret.bodyColor * 2f);
					}
				}
				for (int k = 0; k < _shadowColorAdjusterList.Count; k++)
				{
					ColorAdjuster colorAdjuster = _shadowColorAdjusterList[k];
					colorAdjuster.Apply(ret.shadowColor);
				}
			}
			if (!(_LightMapCorrectionManager != null) || !_LightMapCorrectionManager.Evaluate(XZPosition, ref ret))
			{
				return;
			}
			for (int l = 0; l < _instancedMaterialGroups[0].entries.Length; l++)
			{
				MaterialGroup.RendererMaterials rendererMaterials2 = _instancedMaterialGroups[0].entries[l];
				for (int m = 0; m < rendererMaterials2.colorModifiers.Length; m++)
				{
					rendererMaterials2.colorModifiers[m].Multiply(ret.bodyColor * 2f);
				}
			}
			for (int n = 0; n < _shadowColorAdjusterList.Count; n++)
			{
				ColorAdjuster colorAdjuster2 = _shadowColorAdjusterList[n];
				colorAdjuster2.Apply(ret.shadowColor);
			}
		}

		public void SetMonsterMaterialFadeEnabled(bool enable)
		{
			SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>();
			if (componentsInChildren == null || componentsInChildren.Length <= 0)
			{
				return;
			}
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (enable)
				{
					Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
					foreach (Material material in sharedMaterials)
					{
						int instanceID = material.GetInstanceID();
						if (_fadeMaterialDic.ContainsKey(instanceID) && _fadeMaterialDic[instanceID].recorded)
						{
							material.SetFloat("_FadeDistance", _fadeMaterialDic[instanceID].fadeDistance);
							material.SetFloat("_FadeOffset", _fadeMaterialDic[instanceID].fadeOffset);
						}
					}
					continue;
				}
				Material[] sharedMaterials2 = componentsInChildren[i].sharedMaterials;
				foreach (Material material2 in sharedMaterials2)
				{
					if (!_fadeMaterialDic.ContainsKey(material2.GetInstanceID()))
					{
						float distance = material2.GetFloat("_FadeDistance");
						float offset = material2.GetFloat("_FadeOffset");
						_fadeMaterialDic.Add(material2.GetInstanceID(), new MaterialFadeSetting(distance, offset));
						material2.SetFloat("_FadeDistance", 0f);
						material2.SetFloat("_FadeOffset", 0f);
					}
				}
			}
			if (enable)
			{
				_fadeMaterialDic.Clear();
			}
		}

		protected virtual void TryInitShaderStack()
		{
			if (_shaderStack == null)
			{
				_shaderStack = new FixedStack<Shader>(5, OnShaderStackChanged);
			}
		}

		protected virtual void OnShaderStackChanged(Shader fromShader, int fromIx, Shader toShader, int toIx)
		{
			if (toIx == -1)
			{
				RecoverOriginalShaders();
				return;
			}
			for (int i = 0; i < _matListForSpecailState.Count; i++)
			{
				Material material = _matListForSpecailState[i].material;
				material.shader = toShader;
			}
		}

		protected virtual int PushEffectShaderData(Shader shader)
		{
			return _shaderStack.Push(shader);
		}

		public void PopShaderStackByIndex(int index)
		{
			_shaderStack.Pop(index);
		}

		protected virtual void RecoverOriginalShaders()
		{
			for (int i = 0; i < _matListForSpecailState.Count; i++)
			{
				_matListForSpecailState[i].material.shader = _matListForSpecailState[i].originalShader;
			}
		}

		public string GetCurrentNamedState()
		{
			return _currentNamedState;
		}

		protected virtual void ApplyAnimatorProperties()
		{
			SyncAnimatorSpeed();
			SyncAnimatorBools();
			SyncAnimatorInts();
			SyncFadeLayers();
			AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
			if (animatorConfig.StateToParamBindMap.ContainsKey(currentAnimatorStateInfo.shortNameHash))
			{
				SetStateToParamWithNormalizedTime(currentAnimatorStateInfo);
			}
		}

		private void SetStateToParamWithNormalizedTime(AnimatorStateInfo curState)
		{
			AnimatorStateToParameterConfig animatorStateToParameterConfig = animatorConfig.StateToParamBindMap[curState.shortNameHash];
			float num = curState.normalizedTime - Mathf.Floor(curState.normalizedTime);
			if (num >= animatorStateToParameterConfig.NormalizedTimeStart && num < animatorStateToParameterConfig.NormalizedTimeStop)
			{
				if (!GetLocomotionBool(animatorStateToParameterConfig.ParameterID))
				{
					SetLocomotionBool(animatorStateToParameterConfig.ParameterID, true);
					if (animatorStateToParameterConfig.ParameterIDSub != null)
					{
						SetLocomotionBool(animatorStateToParameterConfig.ParameterIDSub, true);
					}
				}
			}
			else if (GetLocomotionBool(animatorStateToParameterConfig.ParameterID))
			{
				SetLocomotionBool(animatorStateToParameterConfig.ParameterID, false);
				if (animatorStateToParameterConfig.ParameterIDSub != null)
				{
					SetLocomotionBool(animatorStateToParameterConfig.ParameterIDSub, false);
				}
			}
		}

		private void ClearPreviousParamBindOnTransitionEnd(AnimatorStateInfo fromState, AnimatorStateInfo toState)
		{
			AnimatorStateToParameterConfig value = null;
			AnimatorStateToParameterConfig value2 = null;
			animatorConfig.StateToParamBindMap.TryGetValue(fromState.shortNameHash, out value);
			animatorConfig.StateToParamBindMap.TryGetValue(toState.shortNameHash, out value2);
			string text = ((value == null) ? null : value.ParameterID);
			string text2 = ((value2 == null) ? null : value2.ParameterID);
			if (text != null && text != text2)
			{
				SetLocomotionBool(text, false);
				if (value.ParameterIDSub != null)
				{
					SetLocomotionBool(value.ParameterIDSub, false);
				}
			}
		}

		private void SyncAnimatorBools()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			foreach (KeyValuePair<string, bool> animatorBoolParam in _animatorBoolParams)
			{
				_animator.SetBool(animatorBoolParam.Key, animatorBoolParam.Value);
			}
		}

		private void SyncAnimatorInts()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			foreach (KeyValuePair<string, int> animatorIntParam in _animatorIntParams)
			{
				_animator.SetInteger(animatorIntParam.Key, animatorIntParam.Value);
			}
		}

		private void SyncAnimatorSpeed()
		{
			if (base.gameObject.activeInHierarchy)
			{
				_animator.speed = (1f + GetProperty("Animator_OverallSpeedRatio")) * GetProperty("Animator_OverallSpeedRatioMultiplied") * TimeScale;
			}
		}

		private void SyncFadeLayers()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			for (int i = 0; i < _layerFaders.Count; i++)
			{
				if (_layerFaders[i] != null && _layerFaders[i].isDone)
				{
					_animator.SetLayerWeight(_layerFaders[i].layer, _layerFaders[i].toWeight);
				}
			}
		}

		protected virtual void OnSkillEffectClear(string oldID, string skillID)
		{
		}

		private void SyncMass()
		{
			_rigidbody.mass = Mathf.Min(200f, _curMass * (1f + GetProperty("Entity_MassRatio")));
		}

		public override void AddAnimEventPredicate(string predicate)
		{
			_animEventPredicates.Add(predicate);
		}

		public override void RemoveAnimEventPredicate(string predicate)
		{
			_animEventPredicates.Remove(predicate);
		}

		public override bool ContainAnimEventPredicate(string predicate)
		{
			return _animEventPredicates.Contains(predicate);
		}

		public override void MaskAnimEvent(string animEventID)
		{
			if (_maskedAnimEvents == null)
			{
				_maskedAnimEvents = new List<string>();
			}
			_maskedAnimEvents.Add(animEventID);
		}

		public override void UnmaskAnimEvent(string animEventID)
		{
			_maskedAnimEvents.Remove(animEventID);
		}

		public override void MaskTrigger(string triggerID)
		{
			if (_maskedTriggers == null)
			{
				_maskedTriggers = new List<string>();
			}
			_maskedTriggers.Add(triggerID);
		}

		public override void UnmaskTrigger(string triggerID)
		{
			_maskedTriggers.Remove(triggerID);
		}

		protected void MaskAllTriggers(bool mask)
		{
			_maskAllTriggers = mask;
		}

		public override void FireEffect(string patternName)
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(patternName, XZPosition, FaceDirection, Vector3.one * _uniformScale, this);
		}

		public override void FireEffect(string patternName, Vector3 initPos, Vector3 initDir)
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(patternName, initPos, initDir, Vector3.one * _uniformScale, this);
		}

		public override void FireEffectTo(string patternName, BaseMonoEntity to)
		{
			Singleton<EffectManager>.Instance.TriggerEntityEffectPatternFromTo(patternName, XZPosition, FaceDirection, Vector3.one * _uniformScale, this, to);
		}

		public override void PushTimeScale(float timescale, int stackIx)
		{
			_timeScaleStack.Push(stackIx, timescale);
		}

		public override void SetTimeScale(float timescale, int stackIx)
		{
			_timeScaleStack.Set(stackIx, timescale);
		}

		public override void PopTimeScale(int stackIx)
		{
			if (_timeScaleStack.IsOccupied(stackIx))
			{
				_timeScaleStack.Pop(stackIx);
			}
		}

		public override Transform GetAttachPoint(string name)
		{
			for (int i = 0; i < attachPoints.Length; i++)
			{
				if (attachPoints[i].name == name)
				{
					return attachPoints[i].pointTransform;
				}
			}
			return base.transform;
		}

		public bool HasAttachPoint(string name)
		{
			for (int i = 0; i < attachPoints.Length; i++)
			{
				if (attachPoints[i].name == name)
				{
					return true;
				}
			}
			return false;
		}

		public void RebindAttachPoint(string name, string other)
		{
			Transform attachPoint = GetAttachPoint(other);
			for (int i = 0; i < attachPoints.Length; i++)
			{
				if (attachPoints[i].name == name)
				{
					attachPoints[i].pointTransform = attachPoint;
					break;
				}
			}
		}

		[AnimationCallback]
		private void TimeSlowTriggerForce(float time)
		{
			Singleton<LevelManager>.Instance.levelActor.TimeSlow(time);
		}

		[AnimationCallback]
		private void TriggerExposure(float time)
		{
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			if (mainCamera != null)
			{
				mainCamera.ActExposureEffect(time, 0f, time, 10f);
			}
		}

		[AnimationCallback]
		private void TriggerCameraShake(string arg)
		{
			string[] array = arg.Split(',');
			float range = float.Parse(array[0].Trim());
			float time = float.Parse(array[1].Trim());
			Singleton<CameraManager>.Instance.GetMainCamera().ActShakeEffect(time, range, 0f, 2, false, false);
		}

		public abstract void MultiAnimEventHandler(string multiAnimEventID);

		public abstract void AnimEventHandler(string animEventID);

		public abstract void DeadHandler();

		public abstract void ClearAttackTarget();

		public abstract void ClearAttackTriggers();

		protected void AttachAnimatorEventPattern(int animatorStateHash, string eventPattern)
		{
			List<string> value;
			_activeAnimatorEventPatterns.TryGetValue(animatorStateHash, out value);
			if (value == null)
			{
				value = new List<string>();
				_activeAnimatorEventPatterns.Add(animatorStateHash, value);
			}
			bool flag = false;
			int i = 0;
			for (int count = value.Count; i < count; i++)
			{
				if (eventPattern == value[i])
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				value.Add(eventPattern);
			}
		}

		protected void DetachAnimatorEventPattern(int animatorStateHash, string eventPattern)
		{
			if (!_activeAnimatorEventPatterns.ContainsKey(animatorStateHash))
			{
				return;
			}
			List<string> list = _activeAnimatorEventPatterns[animatorStateHash];
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				if (eventPattern == list[i])
				{
					list.RemoveAt(i);
					break;
				}
			}
		}

		public void AddAnimatorEventPattern(int stateHash, string eventPattern)
		{
			AttachAnimatorEventPattern(stateHash, eventPattern);
		}

		public void RemoveAnimatorEventPattern(int stateHash, string eventPattern)
		{
			DetachAnimatorEventPattern(stateHash, eventPattern);
		}

		protected virtual void OnAnimatorStateChanged(AnimatorStateInfo fromState, AnimatorStateInfo toState)
		{
			if (onAnimatorStateChanged != null)
			{
				onAnimatorStateChanged(fromState, toState);
			}
		}

		public void AddFrameExitedAnimatorStates(AnimatorStateInfo stateInfo)
		{
			_sameFrameExitStates[_sameFrameExitCount] = stateInfo;
			_sameFrameExitCount++;
		}

		private void ProcessAnimatorStates()
		{
			bool flag = _animator.IsInTransition(0);
			AnimatorStateInfo animatorStateInfo;
			if (flag)
			{
				animatorStateInfo = _animator.GetNextAnimatorStateInfo(0);
				_prevProcessingStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
			}
			else
			{
				animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
			}
			if (_sameFrameExitCount > 1)
			{
				for (int i = 1; i < _sameFrameExitCount; i++)
				{
					if (_processingStateInfo.fullPathHash != _sameFrameExitStates[i].fullPathHash)
					{
						OnAnimatorStateChanged(_processingStateInfo, _sameFrameExitStates[i]);
						ClearPreviousParamBindOnTransitionEnd(_processingStateInfo, _sameFrameExitStates[i]);
						_processingStateInfo = _sameFrameExitStates[i];
					}
				}
			}
			_sameFrameExitCount = 0;
			if (_wasInTransition && !flag)
			{
				ClearPreviousParamBindOnTransitionEnd(_prevProcessingStateInfo, animatorStateInfo);
			}
			else if (!flag && _processingStateInfo.fullPathHash != animatorStateInfo.fullPathHash)
			{
				ClearPreviousParamBindOnTransitionEnd(_processingStateInfo, animatorStateInfo);
			}
			if (animatorConfig.StateToParamBindMap.ContainsKey(animatorStateInfo.shortNameHash))
			{
				SetStateToParamWithNormalizedTime(animatorStateInfo);
			}
			if (animatorStateInfo.fullPathHash != _processingStateInfo.fullPathHash)
			{
				OnAnimatorStateChanged(_processingStateInfo, animatorStateInfo);
				if (_animator.IsInTransition(0))
				{
					_prevProcessItem.patterns = _curProcessItem.patterns;
					_prevProcessItem.lastTime = _curProcessItem.lastTime;
				}
				else
				{
					_prevProcessItem.patterns = null;
					_prevProcessItem.lastTime = 0f;
				}
				if (_curProcessItem.patterns != null)
				{
					ProcessTimeRange(_curProcessItem.patterns, _curProcessItem.lastTime, 1f, 2);
				}
				_curProcessItem.patterns = null;
				_curProcessItem.lastTime = 0f;
				if (_activeAnimatorEventPatterns.ContainsKey(animatorStateInfo.shortNameHash))
				{
					_curProcessItem.patterns = new AnimatorEventPattern[_activeAnimatorEventPatterns[animatorStateInfo.shortNameHash].Count];
					_curProcessItem.lastTime = animatorStateInfo.normalizedTime;
					for (int j = 0; j < _curProcessItem.patterns.Length; j++)
					{
						AnimatorEventPattern animatorEventPattern = AnimatorEventData.GetAnimatorEventPattern(_activeAnimatorEventPatterns[animatorStateInfo.shortNameHash][j]);
						_curProcessItem.patterns[j] = animatorEventPattern;
					}
					ProcessTimeRange(_curProcessItem.patterns, 0f, _curProcessItem.lastTime, 1);
				}
			}
			else if (!_wasInTransition && flag)
			{
				OnAnimatorStateChanged(_processingStateInfo, animatorStateInfo);
				if (_curProcessItem.patterns != null)
				{
					ProcessTimeRange(_curProcessItem.patterns, _curProcessItem.lastTime, 1f, 2);
				}
				_prevProcessItem.patterns = _curProcessItem.patterns;
				_prevProcessItem.lastTime = _curProcessItem.lastTime;
				_curProcessItem.lastTime = animatorStateInfo.normalizedTime;
				if (_curProcessItem.patterns != null)
				{
					ProcessTimeRange(_curProcessItem.patterns, 0f, _curProcessItem.lastTime, 1);
				}
			}
			_wasInTransition = flag;
			_processingStateInfo = animatorStateInfo;
			ProcessAnimatorPattern();
		}

		private void ProcessAnimatorPattern()
		{
			if (_curProcessItem.patterns != null)
			{
				float lastTime = _curProcessItem.lastTime;
				float normalizedTime = _processingStateInfo.normalizedTime;
				normalizedTime -= (float)(int)normalizedTime;
				if (normalizedTime >= lastTime)
				{
					ProcessTimeRange(_curProcessItem.patterns, lastTime, normalizedTime);
				}
				else
				{
					ProcessTimeRange(_curProcessItem.patterns, lastTime, 1f);
					ProcessTimeRange(_curProcessItem.patterns, 0f, normalizedTime);
				}
				_curProcessItem.lastTime = normalizedTime;
			}
			if (_prevProcessItem.patterns != null)
			{
				if (_wasInTransition)
				{
					ProcessTimeRange(_prevProcessItem.patterns, _prevProcessItem.lastTime, _prevProcessingStateInfo.normalizedTime, 1);
					_prevProcessItem.lastTime = _prevProcessingStateInfo.normalizedTime;
				}
				else
				{
					_prevProcessItem.patterns = null;
					_prevProcessItem.lastTime = 0f;
				}
			}
		}

		private void ProcessTimeRange(AnimatorEventPattern[] patterns, float from, float to, int mode = 0)
		{
			for (int i = 0; i < patterns.Length; i++)
			{
				if (patterns[i] != null)
				{
					ProcessTimeRange(patterns[i], from, to, mode);
				}
			}
		}

		private void ProcessTimeRange(AnimatorEventPattern pattern, float from, float to, int mode = 0)
		{
			float num = Mathf.Clamp01(from);
			float num2 = Mathf.Clamp01(to);
			if (!(num < num2))
			{
				return;
			}
			int i = 0;
			for (int num3 = pattern.animatorEvents.Length; i < num3; i++)
			{
				AnimatorEvent animatorEvent = pattern.animatorEvents[i];
				if (!(animatorEvent.normalizedTime >= num) || !(animatorEvent.normalizedTime < num2))
				{
					continue;
				}
				switch (mode)
				{
				case 0:
					animatorEvent.HandleAnimatorEvent(this);
					break;
				case 1:
					if (animatorEvent.forceTrigger && !animatorEvent.forceTriggerOnTransitionOut)
					{
						animatorEvent.HandleAnimatorEvent(this);
					}
					break;
				case 2:
					if (animatorEvent.forceTriggerOnTransitionOut)
					{
						animatorEvent.HandleAnimatorEvent(this);
					}
					break;
				}
			}
		}

		public void SetCheckForCollision(LayerMask collisionMask, CollisionCallback callback)
		{
			_collisionCallback = callback;
			_collisionLayerMask = collisionMask;
			_waitingForCollision = true;
		}

		public void ClearCheckForCollision()
		{
			_collisionCallback = null;
			_waitingForCollision = false;
		}

		private bool GetXZContact(Collision collision, out ContactPoint contact)
		{
			contact = collision.contacts[0];
			for (int i = 0; i < collision.contacts.Length; i++)
			{
				if (!(Vector3.Angle(collision.contacts[i].normal, Vector3.up) < 20f))
				{
					contact = collision.contacts[i];
					return true;
				}
			}
			return false;
		}

		protected virtual void OnCollisionEnter(Collision collision)
		{
			CheckCollisionHandling(collision);
		}

		protected virtual void OnCollisionStay(Collision collision)
		{
			CheckCollisionHandling(collision);
		}

		private void CheckCollisionHandling(Collision collision)
		{
			ContactPoint contact;
			if (_waitingForCollision && _collisionLayerMask.ContainsLayer(collision.gameObject.layer) && GetXZContact(collision, out contact))
			{
				Vector3 normal = contact.normal;
				normal.y = 0f;
				_collisionCallback(collision.gameObject.layer, normal);
				_waitingForCollision = false;
			}
		}

		public void SyncAnimatorState(int stateHash, float normalizedTime)
		{
			if (normalizedTime == 0f)
			{
				_animator.CrossFade(stateHash, 0.1f, 0, 0.1f);
			}
			else
			{
				_animator.Play(stateHash, 0, normalizedTime);
			}
		}

		public void SyncPosition(Vector3 targetPosition)
		{
			_rigidbody.MovePosition(targetPosition);
		}

		public abstract void SetUseLocalController(bool enabled);

		public bool IsFrameHalting()
		{
			return _frameHaltPlugin.IsActive();
		}

		uint IFrameHaltable.GetRuntimeID()
		{
			return GetRuntimeID();
		}

		uint IFadeOff.GetRuntimeID()
		{
			return GetRuntimeID();
		}
	}
}
