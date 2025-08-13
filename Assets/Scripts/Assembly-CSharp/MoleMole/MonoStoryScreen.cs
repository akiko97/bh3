using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoStoryScreen : BaseMonoDynamicObject
	{
		public enum SelectScreenSide
		{
			Left = 0,
			Right = 1,
			None = 2
		}

		public enum StoryScreenState
		{
			Loading = 0,
			Opening = 1,
			Closing = 2
		}

		public enum DialogState
		{
			Default = 0,
			Displaying = 1,
			ChatEnd = 2,
			DialogEnd = 3
		}

		private const string TEXTURE_PATH = "UI/RenderTexture/TestRenderTexture";

		private const string CONTAINER_PREFAB_PATH = "UI/Menus/Widget/Storage/StoryScreen3dModel";

		private const int AVATAR_SOUND_ONLY = 11;

		private const int AVATAR_TERESAR = 21;

		private const int AVATAR_AI = 31;

		private const int AVATAR_WENDY = 41;

		private const int AVATAR_KSKSLITA = 51;

		private StoryScreenState _screenState;

		private bool _isDialogShowing;

		private DialogState _dialogState;

		private bool _isToBeRemoved;

		private GameObject _containerLeft;

		private GameObject _containerRight;

		private RenderTextureWrapper _leftRenderTexture;

		private RenderTextureWrapper _rightRenderTexture;

		public RawImage _targetDisplayLeftImage;

		public RawImage _targetDisplayRightImage;

		[Header("Story Weights")]
		public Text _dialogueText;

		public Image _leftSpeakingImage;

		public Image _rightSpeakingImage;

		public Text _leftSpeakingNameText;

		public Text _rightSpeakingNameText;

		public Transform _leftSoundOnly;

		public Transform _rightSoundOnly;

		public Transform _leftSource;

		public Transform _rightSource;

		public Transform _leftScreen;

		public Transform _rightScreen;

		[Header("Story Camera Control")]
		public float leftModelCamOffsetX = 3f;

		public float rightModelCamOffsetX = 3f;

		public float backModelCamOffsetZ = 3f;

		public float downModelCamOffsetY = -10f;

		private float _rtHeight = 300f;

		private float _rtWidth = 300f;

		private RenderTextureFormat _rtFormat = RenderTextureFormat.ARGBHalf;

		private int _rtDepth = 24;

		private Sprite soundOnlySprite;

		public float screenScale = 0.01f;

		public Canvas _storyScreenCanvas;

		public float screenZOffset = 2f;

		public float screenYOffset;

		public float screenXOffset;

		public float screenXRotation;

		public float screenYRotation;

		public float screenZRotation;

		private BaseMonoAvatar _avatar;

		private int _currentLeftModelAvatarID;

		private int _currentRightModelAvatarID;

		private TypewriterEffect _typewritterEffect;

		public Action<bool> onOpenAnimationChange;

		public DialogState StoryDialogState
		{
			get
			{
				return _dialogState;
			}
			set
			{
				_dialogState = value;
			}
		}

		public TypewriterEffect Typewritter
		{
			get
			{
				return _typewritterEffect;
			}
		}

		protected void Awake()
		{
			_isToBeRemoved = false;
		}

		public override void Init(uint runtimeID, uint ownerID)
		{
			base.Init(runtimeID, ownerID);
		}

		protected override void Start()
		{
			base.Start();
		}

		public void SetupView(int plotID)
		{
			_avatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			PlotMetaData plotMetaData = PlotMetaDataReader.TryGetPlotMetaDataByKey(plotID);
			PlotDataItem plotDataItem = new PlotDataItem(plotMetaData);
			DialogDataItem dialogDataItem = null;
			DialogDataItem dialogDataItem2 = null;
			if (plotDataItem != null)
			{
				dialogDataItem = DialogMetaDataReader.GetFirstLeftDialogDataItem(plotDataItem);
				dialogDataItem2 = DialogMetaDataReader.GetFirstRightDialogDataItem(plotDataItem);
			}
			InitScreenTransfomSetting();
			InitAvatarModelSetting(dialogDataItem.avatarID, dialogDataItem2.avatarID);
			InitScreenWeights(dialogDataItem, dialogDataItem2);
			Singleton<MainUIManager>.Instance.ShowPage(new InLevelPlotPageContext(this, plotID));
			_screenState = StoryScreenState.Loading;
		}

		private void InitScreenWeights(DialogDataItem leftDataItem, DialogDataItem rightDataItem)
		{
			_leftSpeakingNameText.text = GetAvatarName(leftDataItem.avatarID);
			_rightSpeakingNameText.text = GetAvatarName(rightDataItem.avatarID);
			_typewritterEffect = _dialogueText.GetComponent<TypewriterEffect>();
			_dialogueText.text = string.Empty;
			Color color = MiscData.GetColor("PlotSpeakerOffBgColor");
			_leftSpeakingImage.color = color;
			_rightSpeakingImage.color = color;
			if (leftDataItem != null)
			{
				_leftSoundOnly.gameObject.SetActive(NeedShowSoundOnly(leftDataItem.avatarID));
				SetSourceByName(leftDataItem.source, SelectScreenSide.Left);
			}
			if (rightDataItem != null)
			{
				_rightSoundOnly.gameObject.SetActive(NeedShowSoundOnly(rightDataItem.avatarID));
				SetSourceByName(rightDataItem.source, SelectScreenSide.Right);
			}
			Animation component = base.transform.Find("Canvas/Plot3DDialog/FaceTime/Face_1/Window/3dModel").GetComponent<Animation>();
			if (component != null)
			{
				component.Play("PlotScreenNoise");
			}
			Animation component2 = base.transform.Find("Canvas/Plot3DDialog/FaceTime/Face_2/Window/3dModel").GetComponent<Animation>();
			if (component2 != null)
			{
				component2.Play("PlotScreenNoise1");
			}
		}

		private void SetSourceByName(string source, SelectScreenSide side)
		{
			switch (side)
			{
			case SelectScreenSide.Left:
				_leftSource.GetComponent<Text>().text = LocalizationGeneralLogic.GetText(source);
				break;
			case SelectScreenSide.Right:
				_rightSource.GetComponent<Text>().text = LocalizationGeneralLogic.GetText(source);
				break;
			}
		}

		private void InitScreenTransfomSetting()
		{
			screenZOffset = _avatar.config.StoryCameraSetting.screenZOffset;
			screenYOffset = _avatar.config.StoryCameraSetting.screenYOffset;
			screenXOffset = _avatar.config.StoryCameraSetting.screenXOffset;
			screenScale = _avatar.config.StoryCameraSetting.screenScale;
			base.transform.forward = _avatar.transform.forward;
			RectTransform component = base.transform.GetComponent<RectTransform>();
			screenXRotation = component.eulerAngles.x;
			screenYRotation = component.eulerAngles.y;
			screenZRotation = component.eulerAngles.z;
			_storyScreenCanvas = base.transform.Find("Canvas").GetComponent<Canvas>();
			if (_storyScreenCanvas != null)
			{
				_storyScreenCanvas.gameObject.SetActive(false);
			}
			if (_storyScreenCanvas != null)
			{
				RectTransform component2 = _storyScreenCanvas.GetComponent<RectTransform>();
				component2.SetLocalScaleX(screenScale);
				component2.SetLocalScaleY(screenScale);
				component2.SetLocalScaleZ(screenScale);
			}
			Vector3 vector = Vector3.Cross(_avatar.FaceDirection, Vector3.up);
			base.transform.position = _avatar.XZPosition + _avatar.FaceDirection * screenZOffset + Vector3.up * screenYOffset + vector * screenXOffset;
		}

		private void InitAvatarModelSetting(int leftAvatarID, int rightAvatarID)
		{
			Vector3 vector = Vector3.Cross(_avatar.FaceDirection, Vector3.up);
			_containerLeft = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Storage/StoryScreen3dModel"));
			_containerRight = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Storage/StoryScreen3dModel"));
			_leftRenderTexture = GraphicsUtils.GetRenderTexture((int)_rtWidth, (int)_rtHeight, _rtDepth, _rtFormat);
			_rightRenderTexture = GraphicsUtils.GetRenderTexture((int)_rtWidth, (int)_rtHeight, _rtDepth, _rtFormat);
			Camera component = _containerLeft.transform.Find("StoryCamera").GetComponent<Camera>();
			Camera component2 = _containerRight.transform.Find("StoryCamera").GetComponent<Camera>();
			_leftRenderTexture.BindToCamera(component);
			_rightRenderTexture.BindToCamera(component2);
			soundOnlySprite = Miscs.GetSpriteByPrefab("SpriteOutput/StoryImgs/SoundOnly");
			_targetDisplayLeftImage.texture = ((!NeedShowSoundOnly(leftAvatarID)) ? ((Texture)(RenderTexture)_leftRenderTexture) : ((Texture)soundOnlySprite.texture));
			_targetDisplayRightImage.texture = ((!NeedShowSoundOnly(rightAvatarID)) ? ((Texture)(RenderTexture)_rightRenderTexture) : ((Texture)soundOnlySprite.texture));
			_containerLeft.transform.position = _avatar.XZPosition - _avatar.FaceDirection * backModelCamOffsetZ - vector * leftModelCamOffsetX + Vector3.up * downModelCamOffsetY;
			_containerRight.transform.position = _avatar.XZPosition - _avatar.FaceDirection * backModelCamOffsetZ + vector * rightModelCamOffsetX + Vector3.up * downModelCamOffsetY;
			_containerLeft.transform.forward = -_avatar.FaceDirection;
			_containerRight.transform.forward = -_avatar.FaceDirection;
			string avatarUIModelPrefabPath = GetAvatarUIModelPrefabPath(leftAvatarID);
			string avatarUIModelPrefabPath2 = GetAvatarUIModelPrefabPath(rightAvatarID);
			_currentLeftModelAvatarID = leftAvatarID;
			_currentRightModelAvatarID = rightAvatarID;
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			if (!string.IsNullOrEmpty(avatarUIModelPrefabPath))
			{
				gameObject = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(avatarUIModelPrefabPath));
				gameObject.transform.position = Vector3.zero;
				gameObject.transform.SetParent(_containerLeft.transform.Find("Model").transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.forward = -_avatar.FaceDirection;
				BaseMonoUIAvatar component3 = gameObject.GetComponent<BaseMonoUIAvatar>();
				if (component3 != null)
				{
					component3.Init(leftAvatarID);
				}
			}
			if (!string.IsNullOrEmpty(avatarUIModelPrefabPath2))
			{
				gameObject2 = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(avatarUIModelPrefabPath2));
				gameObject2.transform.position = Vector3.zero;
				gameObject2.transform.SetParent(_containerRight.transform.Find("Model").transform);
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.forward = -_avatar.FaceDirection;
				BaseMonoUIAvatar component4 = gameObject2.GetComponent<BaseMonoUIAvatar>();
				if (component4 != null)
				{
					component4.Init(rightAvatarID);
				}
			}
			Transform transform = _containerLeft.transform.Find("StoryCamera").transform;
			Transform transform2 = _containerRight.transform.Find("StoryCamera").transform;
			Vector3 avatarUIModelCameraPos = GetAvatarUIModelCameraPos(_currentLeftModelAvatarID);
			Vector3 avatarUIModelCameraPos2 = GetAvatarUIModelCameraPos(_currentRightModelAvatarID);
			Vector3 avatarUIModelCameraEuler = GetAvatarUIModelCameraEuler(_currentLeftModelAvatarID);
			Vector3 avatarUIModelCameraEuler2 = GetAvatarUIModelCameraEuler(_currentRightModelAvatarID);
			transform.localPosition = avatarUIModelCameraPos;
			transform.localEulerAngles = avatarUIModelCameraEuler;
			transform2.localPosition = avatarUIModelCameraPos2;
			transform2.localEulerAngles = avatarUIModelCameraEuler2;
		}

		protected override void Update()
		{
			base.Update();
			if (_screenState == StoryScreenState.Loading)
			{
				if (_storyScreenCanvas != null)
				{
					_storyScreenCanvas.gameObject.SetActive(true);
					StartShow();
					_screenState = StoryScreenState.Opening;
				}
			}
			else if (_screenState == StoryScreenState.Opening)
			{
				if (_storyScreenCanvas != null)
				{
					RectTransform component = _storyScreenCanvas.GetComponent<RectTransform>();
					component.SetLocalScaleX(screenScale);
					component.SetLocalScaleY(screenScale);
					component.SetLocalScaleZ(screenScale);
				}
				if (_avatar != null)
				{
					Vector3 vector = Vector3.Cross(_avatar.FaceDirection, Vector3.up);
					base.transform.position = _avatar.XZPosition + _avatar.FaceDirection * screenZOffset + Vector3.up * screenYOffset + vector * screenXOffset;
				}
				RectTransform component2 = base.transform.GetComponent<RectTransform>();
				if (component2 != null)
				{
					component2.SetLocalEulerAnglesX(screenXRotation);
					component2.SetLocalEulerAnglesY(screenYRotation);
					component2.SetLocalEulerAnglesZ(screenZRotation);
				}
			}
		}

		public void RefreshCurrentSpeakerWidgets(DialogDataItem dialogDataItem)
		{
			Color color = MiscData.GetColor("PlotSpeakerOnBgColor");
			Color color2 = MiscData.GetColor("PlotSpeakerOffBgColor");
			_leftSpeakingImage.color = ((dialogDataItem.screenSide != SelectScreenSide.Left) ? color2 : color);
			_rightSpeakingImage.color = ((dialogDataItem.screenSide != SelectScreenSide.Right) ? color2 : color);
			Color color3 = MiscData.GetColor("PlotSpeakerFadeScreenColor");
			if (dialogDataItem.screenSide == SelectScreenSide.Left)
			{
				_leftSpeakingNameText.text = GetAvatarName(dialogDataItem.avatarID);
				bool flag = NeedShowSoundOnly(dialogDataItem.avatarID);
				_leftSoundOnly.gameObject.SetActive(flag);
				_targetDisplayLeftImage.texture = ((!flag) ? ((Texture)(RenderTexture)_leftRenderTexture) : ((Texture)soundOnlySprite.texture));
				_targetDisplayLeftImage.color = Color.white;
				_targetDisplayRightImage.color = color3;
			}
			else if (dialogDataItem.screenSide == SelectScreenSide.Right)
			{
				_rightSpeakingNameText.text = GetAvatarName(dialogDataItem.avatarID);
				bool flag2 = NeedShowSoundOnly(dialogDataItem.avatarID);
				_rightSoundOnly.gameObject.SetActive(flag2);
				_targetDisplayRightImage.texture = ((!flag2) ? ((Texture)(RenderTexture)_rightRenderTexture) : ((Texture)soundOnlySprite.texture));
				_targetDisplayLeftImage.color = color3;
				_targetDisplayRightImage.color = Color.white;
			}
			SetSourceByName(dialogDataItem.source, dialogDataItem.screenSide);
		}

		public void RefreshAvatar3dModel(int avatarID, SelectScreenSide side)
		{
			Transform transform = null;
			bool flag = false;
			if (side == SelectScreenSide.Left)
			{
				transform = _containerLeft.transform.Find("Model");
				if (_currentLeftModelAvatarID != avatarID)
				{
					flag = true;
					_currentLeftModelAvatarID = avatarID;
				}
			}
			else
			{
				transform = _containerRight.transform.Find("Model");
				if (_currentRightModelAvatarID != avatarID)
				{
					flag = true;
					_currentRightModelAvatarID = avatarID;
				}
			}
			Animation animation = ((side != SelectScreenSide.Left) ? _rightScreen.FindChild("Name").GetComponent<Animation>() : _leftScreen.FindChild("Name").GetComponent<Animation>());
			animation.Play("PlotScreenCurrent");
			if (flag)
			{
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
				}
				Animation animation2 = ((side != SelectScreenSide.Left) ? _rightScreen.GetComponent<Animation>() : _leftScreen.GetComponent<Animation>());
				animation2.Play("PlotScreenSwitch");
				StartCoroutine(SyncLoadModelAndSetPos(avatarID, transform));
			}
		}

		private IEnumerator SyncLoadModelAndSetPos(int avatarID, Transform modelParent)
		{
			if (modelParent == null)
			{
				yield break;
			}
			string prefabPath = GetAvatarUIModelPrefabPath(avatarID);
			if (!string.IsNullOrEmpty(prefabPath))
			{
				Transform modelTrans = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(prefabPath)).transform;
				modelTrans.transform.position = Vector3.zero;
				modelTrans.SetParent(modelParent.transform);
				modelTrans.transform.localPosition = Vector3.zero;
				modelTrans.transform.forward = -_avatar.FaceDirection;
				BaseMonoUIAvatar uiAvatar = modelTrans.GetComponent<BaseMonoUIAvatar>();
				if (uiAvatar != null)
				{
					uiAvatar.Init(avatarID);
				}
				Transform leftCameraTrans = _containerLeft.transform.Find("StoryCamera").transform;
				Transform rightCameraTrans = _containerRight.transform.Find("StoryCamera").transform;
				Vector3 leftAvatarCameraPos = GetAvatarUIModelCameraPos(_currentLeftModelAvatarID);
				Vector3 rightAvatarCameraPos = GetAvatarUIModelCameraPos(_currentRightModelAvatarID);
				Vector3 leftAvatarCameraEulerAngle = GetAvatarUIModelCameraEuler(_currentLeftModelAvatarID);
				Vector3 rightAvatarCameraEulerAngle = GetAvatarUIModelCameraEuler(_currentRightModelAvatarID);
				leftCameraTrans.localPosition = leftAvatarCameraPos;
				leftCameraTrans.localEulerAngles = leftAvatarCameraEulerAngle;
				rightCameraTrans.localPosition = rightAvatarCameraPos;
				rightCameraTrans.localEulerAngles = rightAvatarCameraEulerAngle;
			}
		}

		private string GetAvatarUIModelPrefabPath(int avatarID)
		{
			AvatarDataItem avatarDataItem = Singleton<AvatarModule>.Instance.TryGetAvatarByID(avatarID);
			if (avatarDataItem != null)
			{
				string avatarRegistryKey = avatarDataItem.AvatarRegistryKey;
				return string.Format("Entities/Avatar/{0}/Avatar_{0}_UI", avatarRegistryKey);
			}
			if (avatarID == 21)
			{
				string arg = "Theresa";
				return string.Format("Entities/Avatar/Theresa_Story/Avatar_{0}_Story_UI", arg);
			}
			return string.Empty;
		}

		private string GetAvatarName(int avatarID)
		{
			AvatarDataItem avatarDataItem = Singleton<AvatarModule>.Instance.TryGetAvatarByID(avatarID);
			if (avatarDataItem != null)
			{
				if (avatarDataItem.IsEasterner())
				{
					return string.Format("{0}\t{1}", avatarDataItem.ClassLastName, avatarDataItem.ClassFirstName);
				}
				return string.Format("{0}·{1}", avatarDataItem.ClassFirstName, avatarDataItem.ClassLastName);
			}
			switch (avatarID)
			{
			case 11:
				return LocalizationGeneralLogic.GetText("UnknownName");
			case 21:
				return LocalizationGeneralLogic.GetText("Teresa");
			case 31:
				return LocalizationGeneralLogic.GetText("Ai");
			case 41:
				return LocalizationGeneralLogic.GetText("Wendy");
			case 51:
				return LocalizationGeneralLogic.GetText("Ksksliya");
			default:
				return LocalizationGeneralLogic.GetText("UnknownSpeaker");
			}
		}

		private bool NeedShowSoundOnly(int avatarID)
		{
			return avatarID == 11 || avatarID == 31 || avatarID == 41 || avatarID == 51;
		}

		private Vector3 GetAvatarUIModelCameraPos(int avatarID)
		{
			string empty = string.Empty;
			switch (avatarID)
			{
			case 21:
				empty = "Theresa";
				break;
			case 41:
				empty = "Wendy";
				break;
			default:
			{
				AvatarDataItem avatarDataItem = Singleton<AvatarModule>.Instance.TryGetAvatarByID(avatarID);
				empty = ((avatarDataItem != null) ? avatarDataItem.AvatarRegistryKey : string.Empty);
				break;
			}
			}
			ConfigPlotAvatarCameraPosInfo plotAvatarCameraPosInfo = MiscData.GetPlotAvatarCameraPosInfo();
			empty = ((!plotAvatarCameraPosInfo.AvatarPlotCameraPosInfos.ContainsKey(empty)) ? "Default" : empty);
			return plotAvatarCameraPosInfo.AvatarPlotCameraPosInfos[empty].LookAt.Position;
		}

		private Vector3 GetAvatarUIModelCameraEuler(int avatarID)
		{
			string empty = string.Empty;
			switch (avatarID)
			{
			case 21:
				empty = "Theresa";
				break;
			case 41:
				empty = "Wendy";
				break;
			default:
			{
				AvatarDataItem avatarDataItem = Singleton<AvatarModule>.Instance.TryGetAvatarByID(avatarID);
				empty = ((avatarDataItem != null) ? avatarDataItem.AvatarRegistryKey : string.Empty);
				break;
			}
			}
			ConfigPlotAvatarCameraPosInfo plotAvatarCameraPosInfo = MiscData.GetPlotAvatarCameraPosInfo();
			empty = ((!plotAvatarCameraPosInfo.AvatarPlotCameraPosInfos.ContainsKey(empty)) ? "Default" : empty);
			return plotAvatarCameraPosInfo.AvatarPlotCameraPosInfos[empty].LookAt.EulerAngle;
		}

		public override bool IsToBeRemove()
		{
			return _isToBeRemoved;
		}

		public override bool IsActive()
		{
			return !_isToBeRemoved;
		}

		public void StartShow()
		{
			Singleton<ApplicationManager>.Instance.StartCoroutine(ShowProcessIter());
		}

		private IEnumerator ShowProcessIter()
		{
			Animation showAnimation = _storyScreenCanvas.transform.Find("Plot3DDialog").GetComponent<Animation>();
			if (showAnimation != null)
			{
				if (showAnimation.isPlaying)
				{
					showAnimation.Stop();
				}
				showAnimation.Play();
			}
			while (showAnimation.isPlaying)
			{
				yield return null;
			}
			if (onOpenAnimationChange != null)
			{
				onOpenAnimationChange(true);
			}
		}

		public void StartDie()
		{
			StartCoroutine(DieProcessIter());
		}

		private IEnumerator DieProcessIter()
		{
			if (_storyScreenCanvas == null)
			{
				yield break;
			}
			Animation endAnimation = _storyScreenCanvas.transform.Find("Plot3DDialog").GetComponent<Animation>();
			if (!(endAnimation != null))
			{
				yield break;
			}
			if (endAnimation.isPlaying)
			{
				endAnimation.Stop();
			}
			AnimationState endAnimationState = endAnimation["PlotDialogAppear"];
			if (endAnimationState != null)
			{
				endAnimationState.speed = -1f;
				endAnimationState.time = endAnimationState.clip.length;
				endAnimation.Play();
				while (endAnimation.isPlaying)
				{
					yield return null;
				}
				if (onOpenAnimationChange != null)
				{
					onOpenAnimationChange(false);
					Singleton<LevelDesignManager>.Instance.SetPause(false);
					Singleton<LevelDesignManager>.Instance.SetMuteAvatarVoice(false);
					SetDied();
				}
			}
		}

		public override void SetDied()
		{
			base.SetDied();
			_isToBeRemoved = true;
			UnityEngine.Object.DestroyImmediate(_containerLeft);
			UnityEngine.Object.DestroyImmediate(_containerRight);
			if (_leftRenderTexture != null)
			{
				GraphicsUtils.ReleaseRenderTexture(_leftRenderTexture);
				_leftRenderTexture = null;
			}
			if (_rightRenderTexture != null)
			{
				GraphicsUtils.ReleaseRenderTexture(_rightRenderTexture);
				_rightRenderTexture = null;
			}
			_targetDisplayLeftImage = null;
			_targetDisplayRightImage = null;
			_containerLeft = null;
			_containerRight = null;
			StopAllCoroutines();
		}

		public void SetDisplayText(string content)
		{
			if (content == null || _dialogueText == null)
			{
				return;
			}
			_dialogueText.text = content;
			if (_storyScreenCanvas != null)
			{
				Animation componentInParent = _dialogueText.GetComponentInParent<Animation>();
				if (componentInParent.isPlaying)
				{
					componentInParent.Stop();
				}
				componentInParent.Play();
				if (_typewritterEffect != null)
				{
					_isDialogShowing = true;
					_typewritterEffect.RestartRead();
				}
			}
		}

		public void SkipDialog()
		{
			if (_typewritterEffect != null)
			{
				_typewritterEffect.Finish();
			}
		}

		public void FinishiDialog()
		{
			_isDialogShowing = false;
		}

		public bool IsDialogShowing()
		{
			return _isDialogShowing;
		}

		public bool IsDialogProcessingOpen()
		{
			Animation component = _storyScreenCanvas.transform.Find("Plot3DDialog").GetComponent<Animation>();
			if (component == null)
			{
				return false;
			}
			return component.isPlaying || _screenState == StoryScreenState.Loading;
		}
	}
}
