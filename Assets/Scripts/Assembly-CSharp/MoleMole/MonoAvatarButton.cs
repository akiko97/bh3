using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoAvatarButton : MonoBehaviour
	{
		private const float HP_START_RATIO = 0.56f;

		private const float SP_START_RATIO = 0.314f;

		public uint avatarRuntimeID;

		private BaseMonoAvatar _avatar;

		private AvatarActor _avatarActor;

		private Image _hpBarImg;

		private Image _spBarImg;

		private Image _frameLightImg;

		private Animation _frameLightAnimation;

		private Button _button;

		private Image _iconImage;

		private Image _CDMaskImg;

		private MonoEffect _buttonQTEEffect;

		private MonoEffect _buttonSwitchInEffect;

		public int index;

		public bool canChange = true;

		private bool onlyForShow;

		public void Init(uint runtimeID)
		{
			_button = GetComponent<Button>();
			_frameLightImg = base.transform.Find("FrameLightBG").GetComponent<Image>();
			_frameLightAnimation = base.transform.Find("FrameLightBG").GetComponent<Animation>();
			_hpBarImg = base.transform.Find("HPBar/Inner").GetComponent<Image>();
			_spBarImg = base.transform.Find("SPBar/Inner").GetComponent<Image>();
			_iconImage = base.transform.Find("Icon").GetComponent<Image>();
			_CDMaskImg = base.transform.Find("CDMask").GetComponent<Image>();
			_buttonQTEEffect = base.transform.Find("ButtonEffect/Button_QTE_Effect").GetComponent<MonoEffect>();
			_buttonSwitchInEffect = base.transform.Find("ButtonEffect/Button_Switch_In_Effect").GetComponent<MonoEffect>();
			onlyForShow = false;
			SetupAvatar(runtimeID);
		}

		public void InitForReviveButton(BaseMonoAvatar avatar)
		{
			_button = GetComponent<Button>();
			_hpBarImg = base.transform.Find("HPBar/Inner").GetComponent<Image>();
			_spBarImg = base.transform.Find("SPBar/Inner").GetComponent<Image>();
			_iconImage = base.transform.Find("Icon").GetComponent<Image>();
			_CDMaskImg = base.transform.Find("CDMask").GetComponent<Image>();
			avatarRuntimeID = avatar.GetRuntimeID();
			_avatar = avatar;
			_avatarActor = (AvatarActor)Singleton<EventManager>.Instance.GetActor(avatarRuntimeID);
			string avatarIconPath = _avatarActor.avatarIconPath;
			GameObject gameObject = Miscs.LoadResource<GameObject>(avatarIconPath);
			_iconImage.sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
			Transform transform = base.transform.Find("Attr");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				child.gameObject.SetActive(false);
			}
			transform.Find(_avatarActor.avatarDataItem.Attribute.ToString()).gameObject.SetActive(true);
			onlyForShow = true;
			_button.interactable = true;
			base.transform.Find("CDMask").gameObject.SetActive(true);
		}

		public void SetIndex(int index)
		{
			this.index = index;
			MonoKeyButton component = GetComponent<MonoKeyButton>();
			UnityEngine.Object.Destroy(component);
		}

		public void OnSetActive(bool active)
		{
			if (active)
			{
				AvatarActor avatarActor = _avatarActor;
				avatarActor.onHPChanged = (Action<float, float, float>)Delegate.Combine(avatarActor.onHPChanged, new Action<float, float, float>(OnHPChanged));
				AvatarActor avatarActor2 = _avatarActor;
				avatarActor2.onSPChanged = (Action<float, float, float>)Delegate.Combine(avatarActor2.onSPChanged, new Action<float, float, float>(OnSPChanged));
				AvatarActor avatarActor3 = _avatarActor;
				avatarActor3.onMaxHPChanged = (Action<float, float>)Delegate.Combine(avatarActor3.onMaxHPChanged, new Action<float, float>(OnMaxHPChanged));
				AvatarActor avatarActor4 = _avatarActor;
				avatarActor4.onMaxSPChanged = (Action<float, float>)Delegate.Combine(avatarActor4.onMaxSPChanged, new Action<float, float>(OnMaxSPChanged));
				RefreshBarView();
			}
			else
			{
				AvatarActor avatarActor5 = _avatarActor;
				avatarActor5.onHPChanged = (Action<float, float, float>)Delegate.Remove(avatarActor5.onHPChanged, new Action<float, float, float>(OnHPChanged));
				AvatarActor avatarActor6 = _avatarActor;
				avatarActor6.onSPChanged = (Action<float, float, float>)Delegate.Remove(avatarActor6.onSPChanged, new Action<float, float, float>(OnSPChanged));
				AvatarActor avatarActor7 = _avatarActor;
				avatarActor7.onMaxHPChanged = (Action<float, float>)Delegate.Remove(avatarActor7.onMaxHPChanged, new Action<float, float>(OnMaxHPChanged));
				AvatarActor avatarActor8 = _avatarActor;
				avatarActor8.onMaxSPChanged = (Action<float, float>)Delegate.Remove(avatarActor8.onMaxSPChanged, new Action<float, float>(OnMaxSPChanged));
			}
		}

		public void SetupAvatar(uint runtimeId)
		{
			avatarRuntimeID = runtimeId;
			_avatar = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(avatarRuntimeID);
			_avatarActor = (AvatarActor)Singleton<EventManager>.Instance.GetActor(avatarRuntimeID);
			AvatarActor avatarActor = _avatarActor;
			avatarActor.onHPChanged = (Action<float, float, float>)Delegate.Combine(avatarActor.onHPChanged, new Action<float, float, float>(OnHPChanged));
			AvatarActor avatarActor2 = _avatarActor;
			avatarActor2.onSPChanged = (Action<float, float, float>)Delegate.Combine(avatarActor2.onSPChanged, new Action<float, float, float>(OnSPChanged));
			AvatarActor avatarActor3 = _avatarActor;
			avatarActor3.onMaxHPChanged = (Action<float, float>)Delegate.Combine(avatarActor3.onMaxHPChanged, new Action<float, float>(OnMaxHPChanged));
			AvatarActor avatarActor4 = _avatarActor;
			avatarActor4.onMaxSPChanged = (Action<float, float>)Delegate.Combine(avatarActor4.onMaxSPChanged, new Action<float, float>(OnMaxSPChanged));
			_iconImage.sprite = Miscs.GetSpriteByPrefab(_avatarActor.avatarDataItem.IconPathInLevel);
			Transform transform = base.transform.Find("Attr");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				child.gameObject.SetActive(false);
			}
			transform.Find(_avatarActor.avatarDataItem.Attribute.ToString()).gameObject.SetActive(true);
			RefreshBarView();
		}

		private void RefreshBarView()
		{
			float hPBarByRatio = (float)_avatarActor.HP / (float)_avatarActor.maxHP;
			SetHPBarByRatio(hPBarByRatio);
			float sPBarByRatio = (float)_avatarActor.SP / (float)_avatarActor.maxSP;
			SetSPBarByRatio(sPBarByRatio);
		}

		private void Update()
		{
			if (onlyForShow)
			{
				return;
			}
			if (!_avatar.IsAlive())
			{
				if (!_button.interactable)
				{
					_button.interactable = true;
				}
				base.transform.Find("CDMask").gameObject.SetActive(true);
				_frameLightImg.gameObject.SetActive(false);
				_buttonQTEEffect.gameObject.SetActive(false);
				_buttonSwitchInEffect.gameObject.SetActive(false);
				_CDMaskImg.fillAmount = 1f;
				return;
			}
			if (_avatarActor.AllowOtherSwitchIn)
			{
			}
			if (_avatarActor.IsSwitchInCD())
			{
				base.transform.Find("CDMask").gameObject.SetActive(true);
				if (_button.interactable)
				{
					_button.interactable = false;
				}
				float swtichCDRatio = _avatarActor.GetSwtichCDRatio();
				_CDMaskImg.fillAmount = 1f - swtichCDRatio;
			}
			else
			{
				if (!_button.interactable)
				{
					_button.interactable = true;
				}
				base.transform.Find("CDMask").gameObject.SetActive(false);
			}
			if (_avatarActor.IsSPEnough("SKL02"))
			{
				_frameLightImg.gameObject.SetActive(true);
				_frameLightAnimation.Play("FrameLight", PlayMode.StopAll);
			}
			else
			{
				_frameLightImg.gameObject.SetActive(false);
				_frameLightAnimation.Stop("FrameLight");
			}
			_buttonQTEEffect.gameObject.SetActive(CheckCanShowButtonQTEEffect());
			_buttonSwitchInEffect.gameObject.SetActive(CheckCanShowButtonAllowSwitchEffect());
		}

		private bool CheckCanShowButtonQTEEffect()
		{
			bool flag = false;
			return !Singleton<LevelManager>.Instance.IsPaused() && _avatarActor.IsInQTE;
		}

		private bool CheckCanShowButtonAllowSwitchEffect()
		{
			bool result = false;
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
			if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(_avatar.GetRuntimeID()) && actor != null && actor.IsOnStage() && !Singleton<LevelManager>.Instance.IsPaused() && actor.AllowOtherSwitchIn)
			{
				result = true;
			}
			return result;
		}

		public void OnClick()
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Invalid comparison between Unknown and I4
			if (!canChange)
			{
				return;
			}
			BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
			if (_avatar.IsAlive())
			{
				if (!_avatar.IsControlMuted() && localAvatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.AllowTriggerInput) && localAvatar.GetRuntimeID() != avatarRuntimeID)
				{
					Singleton<LevelManager>.Instance.levelActor.TriggerSwapLocalAvatar(localAvatar.GetRuntimeID(), avatarRuntimeID, false);
				}
			}
			else if ((int)Singleton<LevelScoreManager>.Instance.LevelType != 4)
			{
				Singleton<LevelManager>.Instance.SetPause(true);
				if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new InLevelReviveDialogContext(avatarRuntimeID, localAvatar.transform.position));
				}
				else if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi || Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.MultiRemote)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new InLevelReviveDialogContext(avatarRuntimeID, _avatar.transform.position));
				}
			}
		}

		private void OnHPChanged(float from, float to, float delta)
		{
			float hPBarByRatio = to / (float)_avatarActor.maxHP;
			SetHPBarByRatio(hPBarByRatio);
		}

		private void OnSPChanged(float from, float to, float delta)
		{
			float num = to / (float)_avatarActor.maxSP;
			if (from != to && Mathf.Approximately(num, 1f))
			{
				ActButtonBlingEffect();
			}
			SetSPBarByRatio(num);
		}

		private void OnMaxHPChanged(float from, float to)
		{
			float hPBarByRatio = (float)_avatarActor.HP / (float)_avatarActor.maxHP;
			SetHPBarByRatio(hPBarByRatio);
		}

		private void OnMaxSPChanged(float from, float to)
		{
			float num = (float)_avatarActor.SP / (float)_avatarActor.maxSP;
			if (from != to && Mathf.Approximately(num, 1f))
			{
				ActButtonBlingEffect();
			}
			SetSPBarByRatio(num);
		}

		private void SetHPBarByRatio(float ratio)
		{
			_hpBarImg.fillAmount = ratio * 0.44f + 0.56f;
		}

		private void SetSPBarByRatio(float ratio)
		{
			_spBarImg.fillAmount = ratio * 0.18599999f + 0.314f;
		}

		private void ActButtonBlingEffect()
		{
			if (base.gameObject.activeInHierarchy && Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning)
			{
				Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Avatar_Button_Bling", base.transform.TransformPoint(((RectTransform)base.transform).rect.center), base.transform.forward, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
			}
		}

		public void OnClickForRevive()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AvatarSelectForRevive, _avatar.GetRuntimeID()));
		}

		private void OnDestroy()
		{
			if (_buttonQTEEffect != null)
			{
				_buttonQTEEffect.gameObject.SetActive(false);
			}
			if (_buttonSwitchInEffect != null)
			{
				_buttonSwitchInEffect.gameObject.SetActive(false);
			}
			if (_hpBarImg != null)
			{
				_hpBarImg.sprite = null;
				_hpBarImg = null;
			}
			if (_spBarImg != null)
			{
				_spBarImg.sprite = null;
				_spBarImg = null;
			}
			if (_frameLightImg != null)
			{
				_frameLightImg.sprite = null;
				_frameLightImg = null;
			}
			if (_button != null)
			{
				SpriteState spriteState = new SpriteState
				{
					disabledSprite = null,
					highlightedSprite = null,
					pressedSprite = null
				};
				_button.spriteState = spriteState;
				_button.transition = Selectable.Transition.None;
				_button = null;
			}
			if (_iconImage != null)
			{
				_iconImage.sprite = null;
				_iconImage = null;
			}
			if (_CDMaskImg != null)
			{
				_CDMaskImg.sprite = null;
				_CDMaskImg = null;
			}
		}
	}
}
