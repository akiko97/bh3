using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoActButton : MonoBehaviour
	{
		private enum BGAniStep
		{
			None = 0,
			Prepare = 1,
			FadeIn = 2
		}

		private enum BGState
		{
			Idle = 0,
			Fading = 1
		}

		private const string ACTIVITY_LEVEL_PANEL = "UI/Menus/Widget/Map/LevelPanelActivity";

		private const string MATERIAL_COLORIZE_PATH = "Material/ImageColorize";

		private const string MATERIAL_COLOR_PATH = "Material/ImageMonoColor";

		private const float BG_ANI_PREPARE_SPAN = 0.1f;

		private const float BG_FADE_IN_SPAN = 0.4f;

		private const float UNSELECT_POSITION_X = -22f;

		private const int UNSELECT_FONTSIZE = 24;

		private const float SELECT_POSITION_X = -42f;

		private const int SELECT_FONTSIZE = 30;

		private MonoActScroller _scroller;

		private WeekDayActivityDataItem _activityDatta;

		private Transform _bgTrans;

		private MonoActivityInfoPanel _infoPanel;

		private ActDataItem _actData;

		private bool _needFade;

		private float _bgAniTimer;

		private BGAniStep _step;

		private static BGState _state;

		private GameObject _selectedGameObject;

		private GameObject _unselectedGameObject;

		private GameObject _hideGameObject;

		private GameObject _hideDotGameObject;

		private GameObject _imageGameObject;

		private GameObject _selectDotGameObject;

		private GameObject _unselectDotGameObject;

		private Transform _descTransform;

		private Image _imageImage;

		private Text _descText;

		private Outline _descOutline;

		private Image _preImage;

		private CanvasGroup _preCanvasGroup;

		private Image _postImage;

		private CanvasGroup _postCanvasGroup;

		private GameObject _exBGGameObject;

		private CanvasGroup _exBGCanvasGroup;

		private Material _unselectMaterial;

		private Sprite _bgImgSprite;

		private Sprite _activityBgImgSprite;

		public bool selected { get; private set; }

		private void InitCache()
		{
			_selectedGameObject = base.transform.Find("Selected").gameObject;
			_unselectedGameObject = base.transform.Find("Unselected").gameObject;
			_hideGameObject = base.transform.Find("Hide").gameObject;
			_imageGameObject = base.transform.Find("Image").gameObject;
			_imageImage = _imageGameObject.GetComponent<Image>();
			_hideDotGameObject = base.transform.Find("TimeLineDots/HideDot").gameObject;
			_selectDotGameObject = base.transform.Find("TimeLineDots/SelectDot").gameObject;
			_unselectDotGameObject = base.transform.Find("TimeLineDots/UnselectDot").gameObject;
			_descTransform = base.transform.Find("Desc");
			_descText = _descTransform.GetComponent<Text>();
			_descOutline = _descTransform.GetComponent<Outline>();
			_preImage = _bgTrans.Find("Pre").GetComponent<Image>();
			_preCanvasGroup = _bgTrans.Find("Pre").GetComponent<CanvasGroup>();
			_postImage = _bgTrans.Find("Post").GetComponent<Image>();
			_postCanvasGroup = _bgTrans.Find("Post").GetComponent<CanvasGroup>();
			_exBGGameObject = _bgTrans.Find("ExBG").gameObject;
			_exBGCanvasGroup = _bgTrans.Find("ExBG").GetComponent<CanvasGroup>();
			_unselectMaterial = Miscs.LoadResource<Material>("Material/ImageMonoColor");
			if (_actData != null)
			{
				_bgImgSprite = Miscs.GetSpriteByPrefab(_actData.BGImgPath);
			}
			if (_activityDatta != null)
			{
				_activityBgImgSprite = Miscs.GetSpriteByPrefab(_activityDatta.GetBgImgPath());
			}
		}

		private void Start()
		{
			_scroller = base.transform.parent.parent.GetComponent<MonoActScroller>();
			base.transform.Find("Btn").GetComponent<Button>().onClick.AddListener(OnClick);
		}

		private void OnDisable()
		{
			_bgAniTimer = 0f;
			_step = BGAniStep.None;
			_state = BGState.Idle;
		}

		private void Update()
		{
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Invalid comparison between Unknown and I4
			switch (_step)
			{
			case BGAniStep.Prepare:
				_bgAniTimer += Time.deltaTime;
				if (_bgAniTimer >= 0.1f)
				{
					if (!_needFade)
					{
						_step = BGAniStep.None;
					}
					else if (_state != BGState.Fading)
					{
						_bgAniTimer = 0f;
						_step = BGAniStep.FadeIn;
						_state = BGState.Fading;
						_postImage.sprite = ((_actData == null) ? _activityBgImgSprite : _bgImgSprite);
					}
				}
				break;
			case BGAniStep.FadeIn:
				_bgAniTimer += Time.deltaTime;
				_preCanvasGroup.alpha = Mathf.Clamp(1f - _bgAniTimer / 0.4f, 0f, 1f);
				if (_bgAniTimer >= 0.4f)
				{
					_step = BGAniStep.None;
					_state = BGState.Idle;
					_preImage.sprite = _postImage.sprite;
					_preCanvasGroup.alpha = 1f;
				}
				if (_activityDatta != null && (int)_activityDatta.GetActivityType() == 3)
				{
					_exBGGameObject.SetActive(true);
					_exBGCanvasGroup.alpha = Mathf.Clamp(_bgAniTimer / 0.4f, 0f, 1f);
					if (_bgAniTimer >= 0.4f)
					{
						_exBGCanvasGroup.alpha = 1f;
					}
				}
				else
				{
					_exBGCanvasGroup.alpha = Mathf.Clamp(1f - _bgAniTimer / 0.4f, 0f, 1f);
					if (_bgAniTimer >= 0.4f)
					{
						_exBGGameObject.SetActive(false);
					}
				}
				break;
			}
		}

		private void OnClick()
		{
			_scroller.ClickToChangeCenter(base.transform);
		}

		public void SetupActView(ActDataItem actData, List<LevelDataItem> levels, Transform levelScrollTrans, LevelBtnClickCallBack OnLevelClick, Transform bgTrans, Dictionary<LevelDataItem, Transform> levelTransDict, int totalFinishChallengeNum)
		{
			_actData = actData;
			_bgTrans = bgTrans;
			InitCache();
			_selectedGameObject.SetActive(true);
			_unselectedGameObject.SetActive(true);
			_hideGameObject.SetActive(false);
			_imageGameObject.SetActive(true);
			if (!string.IsNullOrEmpty(actData.smallImgPath))
			{
				_imageImage.sprite = Miscs.GetSpriteByPrefab(actData.smallImgPath);
			}
			_descText.text = actData.actTitle;
			Transform transform = Object.Instantiate(Miscs.LoadResource<GameObject>(actData.levelPanelPath)).transform;
			transform.SetParent(levelScrollTrans.Find("Content"), false);
			transform.GetComponent<MonoLevelPanel>().SetupView(levels, OnLevelClick, levelTransDict, null, totalFinishChallengeNum);
			base.transform.GetComponent<MonoItemStatus>().isValid = true;
			transform.GetComponent<MonoItemStatus>().isValid = true;
			_bgTrans.gameObject.SetActive(true);
			if (actData != null)
			{
				_preImage.sprite = _bgImgSprite;
				_preCanvasGroup.alpha = 1f;
				_postImage.sprite = _bgImgSprite;
				_postCanvasGroup.alpha = 1f;
			}
		}

		public void SetupActivityView(WeekDayActivityDataItem activityData, MonoActivityInfoPanel infoPanel, List<LevelDataItem> levels, Transform levelScrollTrans, LevelBtnClickCallBack OnLevelClick, Transform bgTrans, Dictionary<LevelDataItem, Transform> levelTransDict)
		{
			_activityDatta = activityData;
			_infoPanel = infoPanel;
			_bgTrans = bgTrans;
			InitCache();
			_selectedGameObject.SetActive(true);
			_unselectedGameObject.SetActive(true);
			_hideGameObject.SetActive(false);
			_imageGameObject.SetActive(true);
			if (!string.IsNullOrEmpty(activityData.GetSmallImgPath()))
			{
				_imageImage.sprite = Miscs.GetSpriteByPrefab(activityData.GetSmallImgPath());
			}
			_descText.text = activityData.GetActitityTitle();
			Transform transform = Object.Instantiate(Miscs.LoadResource<GameObject>((activityData == null || activityData.GetStatus() != ActivityDataItemBase.Status.InProgress) ? "UI/Menus/Widget/Map/LevelPanelActivity" : activityData.GetLevelPanelPath())).transform;
			transform.SetParent(levelScrollTrans.Find("Content"), false);
			transform.GetComponent<MonoLevelPanel>().SetupView(levels, OnLevelClick, levelTransDict, activityData);
			base.transform.GetComponent<MonoItemStatus>().isValid = true;
			transform.GetComponent<MonoItemStatus>().isValid = true;
			_bgTrans.gameObject.SetActive(true);
			if (_activityDatta != null)
			{
				_preImage.sprite = Miscs.GetSpriteByPrefab(_activityDatta.GetBgImgPath());
				_postImage.sprite = Miscs.GetSpriteByPrefab(_activityDatta.GetBgImgPath());
				_infoPanel.SetupView(_activityDatta);
			}
		}

		public void SetupStatus(bool isSelect)
		{
			_hideGameObject.SetActive(false);
			_hideDotGameObject.SetActive(false);
			selected = isSelect;
			if (!isSelect)
			{
				_selectedGameObject.SetActive(false);
				_unselectedGameObject.SetActive(true);
				_selectDotGameObject.SetActive(false);
				_unselectDotGameObject.SetActive(true);
				_imageImage.color = MiscData.GetColor("ActImageUnSelect");
				_descTransform.SetLocalPositionX(-22f);
				_descText.fontSize = 24;
				_descText.color = MiscData.GetColor("ActDescGray");
				_descOutline.enabled = false;
				_needFade = false;
			}
			else
			{
				_selectedGameObject.SetActive(true);
				_unselectedGameObject.SetActive(false);
				_selectDotGameObject.SetActive(true);
				_unselectDotGameObject.SetActive(false);
				_imageImage.material = null;
				_imageImage.color = MiscData.GetColor("TotalWhite");
				_descTransform.SetLocalPositionX(-42f);
				_descText.fontSize = 30;
				_descText.color = Color.white;
				_descOutline.enabled = true;
				if (_step != BGAniStep.FadeIn)
				{
					_step = BGAniStep.Prepare;
					_bgAniTimer = 0f;
					_needFade = true;
				}
			}
			if (_actData == null && _activityDatta != null)
			{
				if (_activityDatta.GetStatus() == ActivityDataItemBase.Status.Unavailable)
				{
					_imageImage.material = _unselectMaterial;
					_imageImage.color = MiscData.GetColor("Blue");
				}
				if (isSelect)
				{
					_infoPanel.SetupView(_activityDatta);
				}
			}
		}

		public ActDataItem GetActData()
		{
			return _actData;
		}

		public WeekDayActivityDataItem GetWeekDayActivityData()
		{
			return _activityDatta;
		}
	}
}
