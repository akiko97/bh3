using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoSkillButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public enum PointerState
		{
			PointerUp = 0,
			PointerDown = 1
		}

		private const string YELLOW_BG_PATH = "SpriteOutput/AvatarInLevelBtnIcons/BtnUniqueSkill2";

		private const string BLUE_BG_PATH = "SpriteOutput/AvatarInLevelBtnIcons/BtnSkill";

		private const string MATERIAL_COLORIZE_BLUE_PATH = "Material/ImageColorizeSkillIconBlue";

		private const string MATERIAL_COLORIZE_YELLOW_PATH = "Material/ImageColorizeSkillIconYellow";

		public string SkillName;

		public string KeyButtonCode;

		public bool _isPointerHold;

		private bool _buttonHoldBegin;

		private BaseAvatarInputController _controller;

		private BaseMonoAvatar _avatar;

		private AvatarActor _avatarActor;

		private AvatarActor.SKillInfo _skillInfo;

		public Func<PointerState, bool> onPointerStateChange;

		private Image _image;

		private Button _button;

		private Image _maskImg;

		private float _holdTime;

		private float _totalHoldTime;

		private static float HOLD_TIME_THRESHOLD = 0.2f;

		private bool _buttonClickTrigger;

		private bool _muteButtonHighLight;

		private string _maskImagePath;

		private MonoSkillButtonChargeCount _chargeCount;

		private bool _hasPlayedUltraReadySound;

		private void Awake()
		{
			_image = base.transform.Find("Image").GetComponent<Image>();
			_maskImg = base.transform.Find("ImageMask").GetComponent<Image>();
			_button = base.gameObject.GetComponent<Button>();
		}

		public void InitSkillButton(BaseAvatarInputController controller)
		{
			_controller = controller;
			if (_avatarActor != null)
			{
				AvatarActor avatarActor = _avatarActor;
				avatarActor.onSkillChargeChanged = (Action<string, int, int>)Delegate.Remove(avatarActor.onSkillChargeChanged, new Action<string, int, int>(OnSkillChargeChanged));
				AvatarActor avatarActor2 = _avatarActor;
				avatarActor2.onSkillSPNeedChanged = (Action<string, float, float>)Delegate.Remove(avatarActor2.onSkillSPNeedChanged, new Action<string, float, float>(OnSkillSPNeedChanged));
				AvatarActor avatarActor3 = _avatarActor;
				avatarActor3.onSPChanged = (Action<float, float, float>)Delegate.Remove(avatarActor3.onSPChanged, new Action<float, float, float>(OnSPChanged));
			}
			_avatar = _controller.avatar;
			_avatarActor = (AvatarActor)Singleton<EventManager>.Instance.GetActor(_avatar.GetRuntimeID());
			_skillInfo = _avatarActor.GetSkillInfo(SkillName);
			base.gameObject.SetActive(true);
			if ((int)_skillInfo.MaxChargesCount > 0)
			{
				AvatarActor avatarActor4 = _avatarActor;
				avatarActor4.onSkillChargeChanged = (Action<string, int, int>)Delegate.Combine(avatarActor4.onSkillChargeChanged, new Action<string, int, int>(OnSkillChargeChanged));
			}
			AvatarActor avatarActor5 = _avatarActor;
			avatarActor5.onSPChanged = (Action<float, float, float>)Delegate.Combine(avatarActor5.onSPChanged, new Action<float, float, float>(OnSPChanged));
			AvatarActor avatarActor6 = _avatarActor;
			avatarActor6.onSkillSPNeedChanged = (Action<string, float, float>)Delegate.Combine(avatarActor6.onSkillSPNeedChanged, new Action<string, float, float>(OnSkillSPNeedChanged));
			RefreshSkillInfo();
		}

		public void RefreshSkillInfo()
		{
			bool flag = _avatarActor.CanUseSkill(SkillName);
			_image.sprite = Miscs.GetSpriteByPrefab(GetSkillIconPath());
			_muteButtonHighLight = _skillInfo.muteHighlighted;
			_chargeCount = base.transform.Find("ChargesCount").GetComponent<MonoSkillButtonChargeCount>();
			if ((int)_skillInfo.MaxChargesCount > 0)
			{
				_chargeCount.gameObject.SetActive(true);
				_chargeCount.SetupView(_skillInfo.MaxChargesCount, _skillInfo.chargesCounter);
				SetButtonHighlighted((int)_skillInfo.chargesCounter > 0 && flag);
			}
			else
			{
				_chargeCount.gameObject.SetActive(false);
			}
			SetButtonInteractable(flag, SkillName == "SKL02");
			int num = Mathf.FloorToInt(_avatarActor.GetSkillSPNeed(SkillName));
			base.transform.Find("NeedSP").gameObject.SetActive(num > 0);
			if (num > 0)
			{
				base.transform.Find("NeedSP/Num").GetComponent<Text>().text = num.ToString();
			}
			if (_muteButtonHighLight)
			{
				SetButtonHighlighted(flag);
			}
		}

		private string GetSkillIconPath()
		{
			_maskImagePath = _skillInfo.maskIconPath;
			if (string.IsNullOrEmpty(_maskImagePath))
			{
				return _skillInfo.iconPath;
			}
			return _maskImagePath;
		}

		private void Update()
		{
			UpdateForCDMask();
			TryToPlayUltraSound();
			if (_isPointerHold)
			{
				_totalHoldTime += Time.deltaTime;
				_holdTime += Time.deltaTime;
				if (_holdTime > HOLD_TIME_THRESHOLD && _avatarActor != null && _avatarActor.IsAttackButtonHoldMode() && SkillName == "ATK")
				{
					_controller.TryHold(SkillName);
					_holdTime = 0f;
				}
			}
			if (_isPointerHold && _skillInfo != null && _skillInfo.canHold)
			{
				UseSkill();
			}
			else if (_buttonClickTrigger)
			{
				_buttonClickTrigger = false;
				UseSkill();
			}
		}

		private void UpdateForCDMask()
		{
			if (_avatarActor.IsSkillInCD(SkillName))
			{
				float skillCD = _avatarActor.GetSkillCD(SkillName);
				SetButtonInteractable(_avatarActor.CanUseSkill(SkillName));
				if (skillCD == 0f)
				{
					_maskImg.fillAmount = 1f;
				}
				else
				{
					_maskImg.fillAmount = (float)_avatarActor.GetSkillInfo(SkillName).cdTimer / skillCD;
				}
			}
			else
			{
				if (_maskImg.fillAmount != 0f)
				{
					_maskImg.fillAmount = 0f;
				}
				SetButtonInteractable(_avatarActor.CanUseSkill(SkillName));
			}
		}

		private void TryToPlayUltraSound()
		{
			if (!IsUltraSkill())
			{
				return;
			}
			if (!_avatarActor.CanUseSkill(SkillName))
			{
				_hasPlayedUltraReadySound = false;
			}
			else if (!_hasPlayedUltraReadySound)
			{
				MonoEntityAudio component = _avatar.GetComponent<MonoEntityAudio>();
				if (component != null)
				{
					component.PostUltraReady();
				}
				_hasPlayedUltraReadySound = true;
			}
		}

		private void UseSkill()
		{
			if (_avatarActor.CanUseSkill(SkillName))
			{
				_controller.TryUseSkill(SkillName);
			}
		}

		public bool IsPointerHold()
		{
			return _isPointerHold;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (_button.interactable)
			{
				_isPointerHold = true;
				_buttonClickTrigger = true;
				if (onPointerStateChange != null)
				{
					_buttonClickTrigger = onPointerStateChange(PointerState.PointerDown);
				}
				if (SkillName == "ATK" && _avatarActor.IsAttackButtonHoldMode())
				{
					_buttonHoldBegin = true;
					_buttonClickTrigger = false;
				}
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (_button.interactable)
			{
				if (_buttonHoldBegin && _totalHoldTime < HOLD_TIME_THRESHOLD)
				{
					_buttonClickTrigger = true;
				}
				_buttonHoldBegin = false;
				_isPointerHold = false;
				_holdTime = 0f;
				_totalHoldTime = 0f;
				if (onPointerStateChange != null)
				{
					onPointerStateChange(PointerState.PointerUp);
				}
			}
		}

		private void UpdateForKeyboradInput()
		{
			if (!string.IsNullOrEmpty(KeyButtonCode))
			{
				if (Input.GetButtonDown(KeyButtonCode))
				{
					OnPointerDown(null);
				}
				else if (Input.GetButtonUp(KeyButtonCode))
				{
					OnPointerUp(null);
				}
			}
		}

		private void OnSkillChargeChanged(string skillID, int from, int to)
		{
			if (skillID != SkillName)
			{
				return;
			}
			if (_chargeCount != null)
			{
				_chargeCount.SetupView(_skillInfo.MaxChargesCount, to);
			}
			if (to > 0 && _avatarActor.CanUseSkill(SkillName))
			{
				SetButtonHighlighted(true);
				if (from == 0)
				{
					ActButtonBlingEffect();
				}
			}
			else
			{
				SetButtonHighlighted(false);
			}
		}

		private void OnSkillSPNeedChanged(string skillID, float from, float to)
		{
			if (!(skillID != SkillName))
			{
				RefreshSkillInfo();
			}
		}

		private void OnSPChanged(float field, float newValue, float delta)
		{
			SetButtonInteractable(_avatarActor.CanUseSkill(SkillName));
		}

		private void SetButtonHighlighted(bool highlighted)
		{
			if (!string.IsNullOrEmpty(_maskImagePath))
			{
				highlighted = false;
			}
			if (_muteButtonHighLight)
			{
				highlighted = false;
			}
			base.transform.Find("Trigger").gameObject.SetActive(highlighted);
			SetImageMaterial();
		}

		private void SetButtonInteractable(bool interactable, bool force = false)
		{
			bool interactable2 = _button.interactable;
			if (interactable2 != interactable || force)
			{
				_button.interactable = interactable;
				SetImageMaterial();
				SetButtonHighlighted(interactable);
				_image.GetComponent<CanvasGroup>().alpha = ((!interactable) ? 0.3f : 1f);
				if (_chargeCount.gameObject.activeSelf)
				{
					_chargeCount.GetComponent<CanvasGroup>().alpha = ((!interactable) ? 0.3f : 1f);
				}
				if (SkillName == "SKL02" && !interactable2 && _button.interactable)
				{
					ActButtonBlingEffect();
				}
			}
		}

		private void SetImageMaterial()
		{
		}

		private void ActButtonBlingEffect()
		{
			if (base.gameObject.activeInHierarchy && Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning)
			{
				Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Avatar_Button_Bling", base.transform.position, base.transform.forward, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
			}
		}

		private bool IsUltraSkill()
		{
			return SkillName == "SKL02";
		}

		private void OnDestroy()
		{
			_image = null;
			_maskImg = null;
			_button = null;
		}
	}
}
