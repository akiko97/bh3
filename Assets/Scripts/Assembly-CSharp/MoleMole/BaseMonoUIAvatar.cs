using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using proto;
using Material = UnityEngine.Material;

namespace MoleMole
{
	public abstract class BaseMonoUIAvatar : MonoBehaviour, IWeaponAttacher, IBodyPartTouchable
	{
		private class ColorAdjuster
		{
			private Material _material;

			private Color[] colors;

			private string[] colorNames;

			public ColorAdjuster(Material material, string[] colorNames)
			{
				_material = material;
				this.colorNames = colorNames;
				colors = new Color[this.colorNames.Length];
				for (int i = 0; i < this.colorNames.Length; i++)
				{
					colors[i] = _material.GetColor(colorNames[i]);
				}
			}

			public void ApplyLerp(float factor)
			{
				for (int i = 0; i < colors.Length; i++)
				{
					_material.SetColor(colorNames[i], Color.Lerp(colors[i], Color.white, factor));
				}
			}

			public void ApplyMultiply(Color tintColor)
			{
				for (int i = 0; i < colors.Length; i++)
				{
					_material.SetColor(colorNames[i], colors[i] * tintColor);
				}
			}

			public void Restore()
			{
				for (int i = 0; i < colors.Length; i++)
				{
					_material.SetColor(colorNames[i], colors[i]);
				}
			}
		}

		private const string ORDINARY_FEEL_PATH = "UI/Menus/Widget/Storage/OrdinaryFeeling";

		private const string GOOD_FEEL_PATH = "UI/Menus/Widget/Storage/GoodFeeling";

		private const string BUFF_EFFECT_FOLDER = "Effect/GalTouchBuff/";

		private const float IDLE_EFFECT_INTERVAL = 15f;

		private const string SILENT_EFFECT_NAME = "EmotionSilent";

		private int _weaponMetaID;

		public int avatarID;

		public string galTouchControllerPath;

		public float effectYOffset;

		private Vector3 _originPos;

		public Renderer[] renderers;

		private GameObject _ordinaryFeelPrototype;

		private GameObject _goodFeelPrototype;

		private List<ColorAdjuster> _shadowColorAdjusterList;

		private List<ColorAdjuster> _mainColorAdjusterList;

		private bool _isShadowColorAdjusted;

		private bool _isAppliedLightColor;

		public bool tattooVisible;

		public AvatarDataItem avatarData;

		[HideInInspector]
		public AttachPoint[] attachPoints = new AttachPoint[0];

		public Renderer leftEyeRenderer;

		public Renderer rightEyeRenderer;

		public Renderer mouthRenderer;

		public Transform headRoot;

		public GameObject[] switchObjects;

		public GameObject[] galTouchHideObjects;

		private GalTouchSystem galTouchSystem;

		private RuntimeAnimatorController _originAnimatorController;

		private RuntimeAnimatorController _galtouchAnimController;

		private bool _readyToRestart;

		private float _fadeSpeed;

		private bool _fading;

		private Animator _animator;

		private float _camShakeTimer = -1f;

		private Vector3 _cameraOrigionPostion;

		private bool _galTouchInited;

		private float _idleEffectTimer;

		private AtlasMatInfoProvider _providerL;

		private AtlasMatInfoProvider _providerR;

		private AtlasMatInfoProvider _providerM;

		private GameObject _buffEffect;

		private bool _isInGalTouch;

		public int WeaponMetaID
		{
			get
			{
				return _weaponMetaID;
			}
		}

		public bool standOnSpaceshipInGameEntry { get; set; }

		GameObject IWeaponAttacher.gameObject
		{
			get
			{
				return base.gameObject;
			}
		}

		private void OnEnable()
		{
			if (Application.isEditor)
			{
				InitMaterials();
			}
		}

		private void OnDisable()
		{
			if (Application.isEditor)
			{
				RestoreLightColor();
				RestoreShadowColor();
				_mainColorAdjusterList = null;
				_shadowColorAdjusterList = null;
			}
		}

		public virtual void Init(int avatarID)
		{
			this.avatarID = avatarID;
			InitMaterials();
			InitDynamicBone();
			UploadFaceTexture();
		}

		protected virtual void Update()
		{
			if (!_isShadowColorAdjusted || Application.isEditor)
			{
				AdjustShadowColors();
			}
			if (!_isAppliedLightColor || Application.isEditor)
			{
				ApplyLightColor();
			}
			UpdateStandOnSpaceshipInGameEntry();
			UpdateGalTouchSystem();
		}

		private void InitMaterials()
		{
			InitColorAdjusterList();
			AdjustShadowColors();
			ApplyLightColor();
		}

		private void InitColorAdjusterList()
		{
			bool flag = false;
			bool flag2 = false;
			if (_shadowColorAdjusterList == null)
			{
				flag = true;
				_shadowColorAdjusterList = new List<ColorAdjuster>();
			}
			if (_mainColorAdjusterList == null)
			{
				flag2 = true;
				_mainColorAdjusterList = new List<ColorAdjuster>();
			}
			if (!flag && !flag2)
			{
				return;
			}
			string[] colorNames = new string[2] { "_FirstShadowMultColor", "_SecondShadowMultColor" };
			string[] colorNames2 = new string[1] { "_Color" };
			Renderer[] array = renderers;
			foreach (Renderer renderer in array)
			{
				Material[] materials = renderer.materials;
				foreach (Material material in materials)
				{
					if (flag && material.HasProperty("_FirstShadowMultColor"))
					{
						_shadowColorAdjusterList.Add(new ColorAdjuster(material, colorNames));
					}
					if (flag2 && material.HasProperty("_Color"))
					{
						_mainColorAdjusterList.Add(new ColorAdjuster(material, colorNames2));
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
			if (!(component != null))
			{
				return;
			}
			float avatarShadowAdjust = component.AvatarShadowAdjust;
			foreach (ColorAdjuster shadowColorAdjuster in _shadowColorAdjusterList)
			{
				shadowColorAdjuster.ApplyLerp(avatarShadowAdjust);
			}
			_isShadowColorAdjusted = true;
		}

		private void RestoreShadowColor()
		{
			foreach (ColorAdjuster shadowColorAdjuster in _shadowColorAdjusterList)
			{
				shadowColorAdjuster.Restore();
			}
		}

		private void ApplyLightColor()
		{
			Light light = UnityEngine.Object.FindObjectOfType<Light>();
			if (!(light != null))
			{
				return;
			}
			Color tintColor = light.color * light.intensity;
			foreach (ColorAdjuster mainColorAdjuster in _mainColorAdjusterList)
			{
				mainColorAdjuster.ApplyMultiply(tintColor);
			}
			_isAppliedLightColor = true;
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

		private void RestoreLightColor()
		{
			foreach (ColorAdjuster mainColorAdjuster in _mainColorAdjusterList)
			{
				mainColorAdjuster.Restore();
			}
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

		public Transform GetAttachPoint(string name)
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

		public void AttachWeapon(int weaponID, string avatarType)
		{
			if (_weaponMetaID != weaponID)
			{
				_weaponMetaID = weaponID;
				ConfigWeapon weaponConfig = WeaponData.GetWeaponConfig(weaponID);
				Transform weaponProtoTrans = Miscs.LoadResource<GameObject>(weaponConfig.Attach.PrefabPath).transform;
				WeaponAttach.AttachWeaponMesh(weaponConfig, this, weaponProtoTrans, avatarType);
				int layer = 8;
				base.gameObject.SetLayer(layer, true);
			}
		}

		public void SetTattooVisible(int visible)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			GameObject gameObject = GetAttachPoint("Stigmata").gameObject;
			if (visible == 0)
			{
				gameObject.SetActive(false);
			}
			else
			{
				if (!tattooVisible)
				{
					return;
				}
				GetAttachPoint("Stigmata").gameObject.SetActive(true);
				foreach (KeyValuePair<EquipmentSlot, StigmataDataItem> item in avatarData.GetStigmataDict())
				{
					Transform attachPoint = GetAttachPoint(((Enum)item.Key).ToString());
					if (!(attachPoint == null))
					{
						attachPoint.gameObject.SetActive(item.Value != null);
						if (item.Value != null)
						{
							StigmataFadeIn(item.Key);
						}
					}
				}
			}
		}

		public void ChangeStigmata(StigmataDataItem from, StigmataDataItem to, EquipmentSlot slot)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			StartCoroutine(CorrutineChangeStigmata(from, to, slot));
		}

		private IEnumerator CorrutineChangeStigmata(StigmataDataItem from, StigmataDataItem to, EquipmentSlot slot)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			float speed = 1f;
			Transform stigmataTrans = GetAttachPoint(((Enum)slot).ToString());
			stigmataTrans.gameObject.SetActive(true);
			Material material = stigmataTrans.GetComponent<MeshRenderer>().material;
			float emissionFactorOrigin = material.GetFloat("_EmissionFactor");
			float emissionFactor = emissionFactorOrigin;
			if (from != null)
			{
				while (emissionFactor < 14f)
				{
					emissionFactor += speed;
					material.SetFloat("_EmissionFactor", emissionFactor);
					yield return null;
				}
			}
			if (to == null)
			{
				stigmataTrans.gameObject.SetActive(false);
				yield break;
			}
			material.SetTexture("_MainTex", Miscs.LoadResource<Texture>(to.GetTattooPath()));
			while (emissionFactor < 20f)
			{
				emissionFactor += speed;
				material.SetFloat("_EmissionFactor", emissionFactor);
				yield return null;
			}
			while (emissionFactor > 1.5f)
			{
				emissionFactor -= speed;
				material.SetFloat("_EmissionFactor", emissionFactor);
				yield return null;
			}
			material.SetFloat("_EmissionFactor", emissionFactor);
		}

		public void StigmataFadeIn(EquipmentSlot slot)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			StartCoroutine(CorrutineStigmataFadeIn(slot));
		}

		private IEnumerator CorrutineStigmataFadeIn(EquipmentSlot slot)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			float speed = 1f;
			Transform stigmataTrans = GetAttachPoint(((Enum)slot).ToString());
			stigmataTrans.gameObject.SetActive(true);
			Material material = stigmataTrans.GetComponent<MeshRenderer>().material;
			float emissionFactorOrigin = material.GetFloat("_EmissionFactor");
			float emissionFactor = emissionFactorOrigin;
			while (emissionFactor < 20f)
			{
				emissionFactor += speed;
				material.SetFloat("_EmissionFactor", emissionFactor);
				yield return null;
			}
			while (emissionFactor > 1.5f)
			{
				emissionFactor -= speed;
				material.SetFloat("_EmissionFactor", emissionFactor);
				yield return null;
			}
			material.SetFloat("_EmissionFactor", emissionFactor);
		}

		public void StigmataFadeOut(EquipmentSlot slot)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			StartCoroutine(CorrutineStigmataFadeOut(slot));
		}

		private IEnumerator CorrutineStigmataFadeOut(EquipmentSlot slot)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			float speed = 1f;
			Transform stigmataTrans = GetAttachPoint(((Enum)slot).ToString());
			stigmataTrans.gameObject.SetActive(true);
			Material material = stigmataTrans.GetComponent<MeshRenderer>().material;
			float emissionFactor = material.GetFloat("_EmissionFactor");
			float emissionFactorOriginal = emissionFactor;
			float bloomFactor = material.GetFloat("_BloomFactor");
			float bloomFactorOriginal = bloomFactor;
			float speedRatio = 0.3f;
			while (emissionFactor < 10f)
			{
				speedRatio += 0.001f;
				if (speedRatio > 5f)
				{
					speedRatio = 5f;
				}
				emissionFactor += speed * speedRatio;
				material.SetFloat("_EmissionFactor", emissionFactor);
				yield return null;
			}
			material.SetFloat("_EmissionFactor", emissionFactorOriginal);
			material.SetFloat("_BloomFactor", bloomFactorOriginal);
			stigmataTrans.gameObject.SetActive(false);
		}

		private void InitDynamicBone()
		{
			bool uI_AVATAR_USE_DYNAMIC_BONE = GlobalVars.UI_AVATAR_USE_DYNAMIC_BONE;
			DynamicBone[] componentsInChildren = base.gameObject.GetComponentsInChildren<DynamicBone>();
			DynamicBone[] array = componentsInChildren;
			foreach (DynamicBone dynamicBone in array)
			{
				dynamicBone.enabled = uI_AVATAR_USE_DYNAMIC_BONE;
			}
		}

		public void SetOriginPos(Vector3 originPos)
		{
			_originPos = originPos;
		}

		private void UpdateStandOnSpaceshipInGameEntry()
		{
			if (standOnSpaceshipInGameEntry && Singleton<MainUIManager>.Instance != null && Singleton<MainUIManager>.Instance.SceneCanvas != null && Singleton<MainUIManager>.Instance.SceneCanvas is MonoGameEntry)
			{
				float y = _originPos.y;
				MonoGameEntry monoGameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
				Transform transform = monoGameEntry.spaceshipGO.transform.Find("Warship");
				base.transform.position = new Vector3(_originPos.x, y + transform.position.y - monoGameEntry.warshipDefaultYPos, _originPos.z);
			}
		}

		private void InitGalTouch()
		{
			if (!(leftEyeRenderer == null) && !(rightEyeRenderer == null) && !(mouthRenderer == null) && !(_originAnimatorController != null))
			{
				_animator = GetComponent<Animator>();
				galTouchSystem = new GalTouchSystem();
				string text = null;
				if (avatarID / 100 == 1)
				{
					text = "Kiana";
				}
				else if (avatarID / 100 == 2)
				{
					text = "Mei";
				}
				else if (avatarID / 100 == 3)
				{
					text = "Bronya";
				}
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
				galTouchSystem.Init(GetComponent<Animator>(), avatarData.avatarID, 1, leftEyeRenderer, rightEyeRenderer, mouthRenderer, _providerL, _providerR, _providerM, headRoot);
				_originAnimatorController = GetComponent<Animator>().runtimeAnimatorController;
				MonoBodyPart[] componentsInChildren = base.gameObject.GetComponentsInChildren<MonoBodyPart>();
				int i = 0;
				for (int num = componentsInChildren.Length; i < num; i++)
				{
					componentsInChildren[i].SetBodyPartTouchable(this);
				}
				int j = 0;
				for (int num2 = switchObjects.Length; j < num2; j++)
				{
					switchObjects[j].SetActive(false);
				}
				galTouchSystem.IdleChanged += OnGalTouchSystemIdleChanged;
				galTouchSystem.TouchPatternTriggered += OnTouchPatternTriggered;
				_ordinaryFeelPrototype = Resources.Load<GameObject>("UI/Menus/Widget/Storage/OrdinaryFeeling");
				_goodFeelPrototype = Resources.Load<GameObject>("UI/Menus/Widget/Storage/GoodFeeling");
				_galTouchInited = true;
			}
		}

		public void BodyPartTouched(BodyPartType type, Vector3 point)
		{
			if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
			{
				return;
			}
			BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
			if (currentPageContext != null && currentPageContext.dialogContextList.Count > 0)
			{
				return;
			}
			int heartLevel = galTouchSystem.heartLevel;
			if (!galTouchSystem.BodyPartTouched(type))
			{
				return;
			}
			GameObject gameObject = null;
			gameObject = ((heartLevel < 4) ? _ordinaryFeelPrototype : _goodFeelPrototype);
			if (gameObject != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				if (gameObject2 != null)
				{
					gameObject2.transform.position = point;
				}
			}
			_idleEffectTimer = 15f;
			GameObject gameObject3 = GameObject.Find("EmotionSilent");
			if (gameObject3 != null)
			{
				UnityEngine.Object.Destroy(gameObject3);
			}
		}

		private void GalTouchRollBuff(int partIndex)
		{
			TouchLevelItem touchLevelItem = GalTouchData.GetTouchLevelItem(galTouchSystem.heartLevel);
			if (touchLevelItem == null)
			{
				return;
			}
			float prop = touchLevelItem.prop;
			float value = UnityEngine.Random.value;
			if (value < prop)
			{
				int[] array = GalTouchData.QueryTouchBuff(avatarID, partIndex, galTouchSystem.heartLevel);
				if (array != null)
				{
					value = UnityEngine.Random.value;
					int num = (int)(value * (float)array.Length);
					Singleton<GalTouchModule>.Instance.AddBuff(avatarID, array[num]);
				}
			}
		}

		public void UpdateGalTouchSystem()
		{
			if (galTouchSystem != null)
			{
				galTouchSystem.Process(Time.deltaTime);
				if (galTouchSystem.enable)
				{
					_idleEffectTimer -= Time.deltaTime;
					if (_idleEffectTimer <= 0f)
					{
						_idleEffectTimer += 15f;
						GalTouchEffect("EmotionSilent");
					}
				}
				if (!galTouchSystem.idle)
				{
					_idleEffectTimer = 15f;
				}
			}
			if (_readyToRestart && galTouchSystem.idle)
			{
				GeneralLogicManager.RestartGame();
			}
			if (_fading)
			{
				PostFX component = Camera.main.gameObject.GetComponent<PostFX>();
				component.Exposure -= _fadeSpeed * Time.deltaTime;
				component.Exposure = ((!(component.Exposure >= 0f)) ? 0f : component.Exposure);
			}
			if (_camShakeTimer > 0f)
			{
				_camShakeTimer -= Time.deltaTime;
				if (_camShakeTimer > 0f)
				{
					Vector3 vector = new Vector3(0f, Mathf.Sin(_camShakeTimer * 900f) * 0.2f, 0f);
					Camera.main.transform.localPosition = _cameraOrigionPostion + vector;
				}
				else
				{
					_camShakeTimer = -1f;
					Camera.main.transform.localPosition = _cameraOrigionPostion;
				}
			}
			BasePageContext basePageContext = ((!(Singleton<MainUIManager>.Instance.SceneCanvas is MonoMainCanvas)) ? null : Singleton<MainUIManager>.Instance.CurrentPageContext);
			if (basePageContext != null && basePageContext.dialogContextList.Count > 0)
			{
				_idleEffectTimer = 15f;
			}
		}

		public void EnterGalTouch()
		{
			if (!_galTouchInited)
			{
				InitGalTouch();
			}
			if (_isInGalTouch)
			{
				return;
			}
			if (galTouchSystem != null)
			{
				galTouchSystem.enable = true;
				if (_animator != null)
				{
					_galtouchAnimController = GetRuntimeAnimatorControllerByID();
					_animator.runtimeAnimatorController = _galtouchAnimController;
				}
				int characterHeartLevel = Singleton<GalTouchModule>.Instance.GetCharacterHeartLevel();
				if (Singleton<GalTouchModule>.Instance != null)
				{
					Singleton<GalTouchModule>.Instance.GalTouchInfoChanged += OnGalTouchInfoChanged;
					Singleton<GalTouchModule>.Instance.GalAddBuff += OnGalTouchAddBuff;
				}
				galTouchSystem.heartLevel = ((characterHeartLevel <= 4) ? characterHeartLevel : 4);
			}
			int i = 0;
			for (int num = galTouchHideObjects.Length; i < num; i++)
			{
				galTouchHideObjects[i].SetActive(false);
			}
			int avatarGalTouchBuffId = Singleton<GalTouchModule>.Instance.GetAvatarGalTouchBuffId(avatarID);
			ProcessBuffEffectOfBuffId(avatarGalTouchBuffId);
			_idleEffectTimer = 15f;
			_isInGalTouch = true;
		}

		private RuntimeAnimatorController GetRuntimeAnimatorControllerByID()
		{
			string text = "Entities/Avatar/";
			string text2 = galTouchControllerPath;
			return Resources.Load<RuntimeAnimatorController>(text + text2);
		}

		public void ExitGalTouch()
		{
			if (_readyToRestart)
			{
				GeneralLogicManager.RestartGame();
			}
			if (!_isInGalTouch)
			{
				return;
			}
			if (galTouchSystem != null)
			{
				galTouchSystem.enable = false;
				galTouchSystem.StopFaceAnimation();
				galTouchSystem.StopVoice();
				if (_animator != null)
				{
					_animator.runtimeAnimatorController = _originAnimatorController;
					Resources.UnloadAsset(_galtouchAnimController);
					_galtouchAnimController = null;
				}
				if (Singleton<GalTouchModule>.Instance != null)
				{
					Singleton<GalTouchModule>.Instance.GalTouchInfoChanged -= OnGalTouchInfoChanged;
					Singleton<GalTouchModule>.Instance.GalAddBuff -= OnGalTouchAddBuff;
				}
			}
			int i = 0;
			for (int num = galTouchHideObjects.Length; i < num; i++)
			{
				if (galTouchHideObjects[i] != null)
				{
					galTouchHideObjects[i].SetActive(true);
				}
			}
			DetachBuffEffect();
			_isInGalTouch = false;
		}

		protected void OnDestroy()
		{
			if (_isInGalTouch && Singleton<GalTouchModule>.Instance != null)
			{
				Singleton<GalTouchModule>.Instance.GalTouchInfoChanged -= OnGalTouchInfoChanged;
				Singleton<GalTouchModule>.Instance.GalAddBuff -= OnGalTouchAddBuff;
			}
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

		private void OnGalTouchSystemIdleChanged(bool idle)
		{
			if (idle)
			{
				int i = 0;
				for (int num = switchObjects.Length; i < num; i++)
				{
					switchObjects[i].SetActive(false);
				}
				int j = 0;
				for (int num2 = galTouchHideObjects.Length; j < num2; j++)
				{
					galTouchHideObjects[j].SetActive(false);
				}
			}
		}

		private void OnTouchPatternTriggered(int partIndex)
		{
			int amount = GalTouchData.QueryTouchFeel(avatarID, partIndex, Singleton<GalTouchModule>.Instance.GetCharacterHeartLevel());
			GalTouchRollBuff(partIndex);
			Singleton<GalTouchModule>.Instance.IncreaseTouchGoodFeel(amount);
		}

		public void OnGalTouchInfoChanged(int oldGoodFeel, int oldHeartLevel, int newGoodFeel, int newHeartLevel, GoodFeelLimitType limitType)
		{
			if (oldHeartLevel != newHeartLevel)
			{
				galTouchSystem.heartLevel = ((newHeartLevel <= 4) ? newHeartLevel : 4);
			}
		}

		public void OnGalTouchAddBuff(int avatarId, int buffId)
		{
			if (avatarId == avatarID)
			{
				ProcessBuffEffectOfBuffId(buffId);
			}
		}

		private void ProcessBuffEffectOfBuffId(int buffId)
		{
			if (buffId != 0)
			{
				TouchBuffItem touchBuffItem = GalTouchData.GetTouchBuffItem(buffId);
				if (touchBuffItem != null)
				{
					AttachBuffEffect(touchBuffItem.effect);
				}
			}
			else
			{
				DetachBuffEffect();
			}
		}

		private void AttachBuffEffect(string name)
		{
			string path = "Effect/GalTouchBuff/" + name;
			GameObject gameObject = Resources.Load<GameObject>(path);
			if (gameObject != null)
			{
				if (_buffEffect != null)
				{
					UnityEngine.Object.Destroy(_buffEffect);
				}
				_buffEffect = UnityEngine.Object.Instantiate(gameObject);
				if (_buffEffect != null)
				{
					_buffEffect.transform.SetParent(base.transform, false);
				}
			}
		}

		private void DetachBuffEffect()
		{
			if (_buffEffect != null)
			{
				UnityEngine.Object.Destroy(_buffEffect);
				_buffEffect = null;
			}
		}

		public void SwitchOn(string name)
		{
			int i = 0;
			for (int num = switchObjects.Length; i < num; i++)
			{
				if (switchObjects[i].name == name)
				{
					switchObjects[i].SetActive(true);
					break;
				}
			}
		}

		public void SwitchOff(string name)
		{
			int i = 0;
			for (int num = switchObjects.Length; i < num; i++)
			{
				if (switchObjects[i].name == name)
				{
					switchObjects[i].SetActive(false);
					break;
				}
			}
		}

		public void ShowHiden(int show)
		{
			int i = 0;
			for (int num = galTouchHideObjects.Length; i < num; i++)
			{
				galTouchHideObjects[i].SetActive(show != 0);
			}
		}

		public void FadeBlack(float speed = 60f)
		{
			int i = 0;
			for (int allCamerasCount = Camera.allCamerasCount; i < allCamerasCount; i++)
			{
				Camera camera = Camera.allCameras[i];
				if (camera != Camera.main)
				{
					camera.enabled = false;
				}
				_fading = true;
				_fadeSpeed = speed;
			}
		}

		public void RestartGame()
		{
			_readyToRestart = true;
		}

		public void CameraShake(float time)
		{
			_cameraOrigionPostion = Camera.main.transform.localPosition;
			_camShakeTimer = time;
		}

		public void TriggerAudioPattern(string name)
		{
			Singleton<WwiseAudioManager>.Instance.Post(name, base.gameObject);
		}

		public void GalTouchEffect(string name)
		{
			string[] array = name.Split(',');
			string text = array[0];
			string text2 = ((array.Length < 2) ? null : array[1]);
			string text3 = ((array.Length < 3) ? null : array[2]);
			string path = "UI/Menus/Widget/Storage/" + text;
			GameObject gameObject = Resources.Load<GameObject>(path);
			if (gameObject != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				float result = 0f;
				if (text2 != null)
				{
					float.TryParse(text2, out result);
				}
				float result2 = 0f;
				if (text3 != null)
				{
					float.TryParse(text3, out result2);
				}
				gameObject2.transform.position = gameObject2.transform.position + new Vector3(result, result2 + effectYOffset, 0f);
				gameObject2.name = name;
			}
		}
	}
}
