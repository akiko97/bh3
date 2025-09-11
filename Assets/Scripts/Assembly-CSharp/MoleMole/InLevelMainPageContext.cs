using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class InLevelMainPageContext : BasePageContext
	{
		public enum InLevelMainPageShowState
		{
			Show = 0,
			Hide = 1,
			Changing = 2
		}

		private const int COUNT_DOWN_TEXT_WARNING_SECOND = 10;

		private const int MIN_SP_DISPLAY_TEXT_INT_VALUE = 2;

		private const float _localAvatarHPWarningRatio = 0.3f;

		private const float _maxHurtWarningRatio = 2f;

		private const float _minHurtWarningRatio = 0.5f;

		private MonoComboText _comboText;

		private MonoHPDisplayText _hpDisplayText;

		private MonoSPDisplayText _spDisplayText;

		private Text _timeCountDownText;

		private Text _addTimeText;

		private Text _timerText;

		private Animation _mainPageFadeAnim;

		private InLevelMainPageShowState _showState;

		private Transform _buttonOverHeatPlugin;

		public Dictionary<string, MonoSkillButton> skillButtonDict;

		public MonoAvatarButtonContainer avatarButtonContainer;

		private float _tutorialDelayInputTime = 0.5f;

		private float _hurtRatio;

		private LocalAvatarHealthMode _healthMode;

		private bool _pauseDialogShown;

		private bool _pauseBtnEnable = true;

		private bool _hasShowTeamBuff;

		private bool _isOverheat;

		private float _overheatRatio;

		public bool PauseBtnEnabled
		{
			get
			{
				return _pauseBtnEnable;
			}
			set
			{
				_pauseBtnEnable = value;
			}
		}

		public InLevelMainPageContext(GameObject view = null)
		{
			config = new ContextPattern
			{
				contextName = "InLevelMainPageContext",
				viewPrefabPath = "UI/Menus/Page/InLevel/InLevelMainPage"
			};
			base.view = view;
			skillButtonDict = new Dictionary<string, MonoSkillButton>();
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.OnAvatarCreate)
			{
				return OnAvatarCreate((uint)ntf.body);
			}
			if (ntf.type == NotifyTypes.PostStageReady)
			{
				return PostStageReady();
			}
			if (ntf.type == NotifyTypes.ShowDamegeText)
			{
				return ShowDamegeText((EvtBeingHit)ntf.body);
			}
			if (ntf.type == NotifyTypes.AttackLanded)
			{
				return OnAttackLandedNotify((EvtAttackLanded)ntf.body);
			}
			if (ntf.type == NotifyTypes.SetTimeCountDownText)
			{
				return SetTimeCountDownText((float)ntf.body);
			}
			if (ntf.type == NotifyTypes.SetTimesUpText)
			{
				return SetTimesUpText((string)ntf.body);
			}
			if (ntf.type == NotifyTypes.ShowAddTimeText)
			{
				return ShowAddTimeText((float)ntf.body);
			}
			if (ntf.type == NotifyTypes.SetDefendModeText)
			{
				return SetDefendModeText((string)ntf.body);
			}
			if (ntf.type == NotifyTypes.ShowDefendModeText)
			{
				return SetDefendModeTextEnable((bool)ntf.body);
			}
			if (ntf.type == NotifyTypes.SetTimeCountDownTextActive)
			{
				return SetTimeCountDownTextActive((bool)ntf.body);
			}
			if (ntf.type == NotifyTypes.ShowHelperCutIn)
			{
				return ShowHelperCutIn();
			}
			if (ntf.type == NotifyTypes.DropItemConutChanged)
			{
				return SetDropItemCount((int)ntf.body);
			}
			if (ntf.type == NotifyTypes.ShowLevelDisplayText)
			{
				return ShowLevelDisplayText((string)ntf.body);
			}
			if (ntf.type == NotifyTypes.SetTimerText)
			{
				return SetTimerText((float)ntf.body);
			}
			if (ntf.type == NotifyTypes.SetupLocalAvatarStatus)
			{
				return SetupLocalAvatarStatus(ntf.body as AvatarActor);
			}
			if (ntf.type == NotifyTypes.TutorialPlayerTeaching)
			{
				return OnTutorialPlayerTeaching((LevelTutorialPlayerTeaching)ntf.body);
			}
			if (ntf.type == NotifyTypes.TutorialUltraAttack)
			{
				return OnTutorialUltraAttackNotify((LevelTutorialUltraAttack)ntf.body);
			}
			if (ntf.type == NotifyTypes.TutorialBranchAttack)
			{
				return OnTutorialBranchAttackNotify((LevelTutorialBranchAttack)ntf.body);
			}
			if (ntf.type == NotifyTypes.TutorialEliteAttack)
			{
				return OnTutorialEliteAttackNotify((LevelTutorialEliteAttack)ntf.body);
			}
			if (ntf.type == NotifyTypes.TutorialSwapAttack)
			{
				return OnTutorialSwapAttackNotify((LevelTutorialSwapAttack)ntf.body);
			}
			if (ntf.type == NotifyTypes.TutorialSwapAndRestrain)
			{
				return OnTutorialSwapAndRestrain((LevelTutorialSwapAndRestrain)ntf.body);
			}
			if (ntf.type == NotifyTypes.TutorialMonsterBlock)
			{
				return OnTutorialMonsterBlock((LevelTutorialMonsterBlock)ntf.body);
			}
			if (ntf.type == NotifyTypes.TutorialMonsterTeleport)
			{
				return OnTutorialMonsterTeleport((LevelTutorialMonsterTeleport)ntf.body);
			}
			if (ntf.type == NotifyTypes.TutorialNatureRestrain)
			{
				return OnTutorialNatureRestrain((LevelTutorialNatureRestrain)ntf.body);
			}
			if (ntf.type == NotifyTypes.TutorialMonsterShield)
			{
				return OnTutorialMonsterShield((LevelTutorialMonsterShield)ntf.body);
			}
			if (ntf.type == NotifyTypes.TutorialMonsterRobotDodge)
			{
				return OnTutorialMonsterRobotDodge((LevelTutorialMonsterRobotDodge)ntf.body);
			}
			if (ntf.type == NotifyTypes.EvadeBtnVisible)
			{
				return OnEvadeBtnVisibleControl((bool)ntf.body);
			}
			if (ntf.type == NotifyTypes.AttackBtnVisible)
			{
				return OnAttackBtnVisibleControl((bool)ntf.body);
			}
			if (ntf.type == NotifyTypes.JoystickVisible)
			{
				return OnJoystickVisibleControl((bool)ntf.body);
			}
			if (ntf.type == NotifyTypes.PauseBtnEnable)
			{
				return OnPauseBtnEnable((bool)ntf.body);
			}
			if (ntf.type == NotifyTypes.PauseBtnVisible)
			{
				return OnPauseBtnVisible((bool)ntf.body);
			}
			if (ntf.type == NotifyTypes.SwapBtnVisible)
			{
				return OnSwapBtnVisible((bool)ntf.body);
			}
			if (ntf.type == NotifyTypes.EcoModeVisible)
			{
				return OnEcoModeVisible((bool)ntf.body);
			}
			if (ntf.type == NotifyTypes.OnSocketConnect)
			{
				return OnSocketConnect();
			}
			if (ntf.type == NotifyTypes.OnSocketDisconnect)
			{
				return OnSocketDisconnect();
			}
			if (ntf.type == NotifyTypes.ResitComboClear)
			{
				return OnResistComboClear();
			}
			if (ntf.type == NotifyTypes.OnQuitGameDialogShow)
			{
				return OnQuitGameDialogShow();
			}
			if (ntf.type == NotifyTypes.OnQuitGameDialogDestroy)
			{
				return OnQuitGameDialogDestroy();
			}
			if (ntf.type == NotifyTypes.BattleBegin)
			{
				return OnBattleBegin();
			}
			if (ntf.type == NotifyTypes.LoadingSceneDestroyed)
			{
				return OnLoadingSceneDestroyed();
			}
			return false;
		}

		public override void StartUp(Transform canvasTrans, Transform viewParent = null)
		{
			base.StartUp(canvasTrans, viewParent);
			_comboText.transform.localScale = Vector3.zero;
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.onLevelComboChanged = (Action<int, int>)Delegate.Combine(levelActor.onLevelComboChanged, new Action<int, int>(UpdateComboText));
		}

		public override void Destroy()
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.onLevelComboChanged = (Action<int, int>)Delegate.Remove(levelActor.onLevelComboChanged, new Action<int, int>(UpdateComboText));
			base.Destroy();
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("FuncBtns/PauseBtn").GetComponent<Button>(), OnPauseBtnClick);
		}

		protected override bool SetupView()
		{
			_comboText = base.view.transform.Find("LocalAvatarStatus/ComboText").GetComponent<MonoComboText>();
			_hpDisplayText = base.view.transform.Find("LocalAvatarStatus/HP").GetComponent<MonoHPDisplayText>();
			_spDisplayText = base.view.transform.Find("LocalAvatarStatus/SP").GetComponent<MonoSPDisplayText>();
			_mainPageFadeAnim = base.view.gameObject.GetComponent<Animation>();
			_timeCountDownText = base.view.transform.Find("TimeCountDown/Timing/Text").GetComponent<Text>();
			_addTimeText = base.view.transform.Find("TimeCountDown/AddTimeText").GetComponent<Text>();
			_timerText = base.view.transform.Find("LevelInfoPanel/Timer/Text").GetComponent<Text>();
			_buttonOverHeatPlugin = base.view.transform.Find("InputController/SkillButton_1/ButtonOverHeatPlugin");
			avatarButtonContainer = base.view.transform.Find("AvatarBtns").GetComponent<MonoAvatarButtonContainer>();
			base.view.transform.Find("MonsterStatus").gameObject.SetActive(false);
			base.view.transform.Find("TimeCountDown").gameObject.SetActive(false);
			_addTimeText.gameObject.SetActive(false);
			_timerText.text = "00:00";
			SetupDebugMenu();
			base.view.transform.Find("InputController").gameObject.SetActive(false);
			base.view.transform.Find("HelperCutIn").gameObject.SetActive(false);
			base.view.transform.Find("LevelDisplayText").gameObject.SetActive(false);
			base.view.transform.Find("LevelInfoPanel/DropItem/NumText").GetComponent<Text>().text = "x0";
			if (GlobalVars.DEBUG_FEATURE_ON)
			{
				base.view.transform.Find("DebugWidget").gameObject.SetActive(GlobalVars.DEBUG_FEATURE_ON);
			}
			else
			{
				UnityEngine.Object.Destroy(base.view.transform.Find("DebugWidget").gameObject);
			}
			_showState = InLevelMainPageShowState.Show;
			OnEcoModeVisible(IsEcoMode());
			return false;
		}

		private bool IsEcoMode()
		{
			if (Singleton<PlayerModule>.Instance.playerData.userId == 0)
			{
				return false;
			}
			return Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting.IsEcoMode;
		}

		public void OnPauseBtnClick()
		{
			if (_pauseBtnEnable && !Singleton<LevelManager>.Instance.IsPaused())
			{
				Singleton<LevelManager>.Instance.SetPause(true);
				InLevelPauseDialogContext inLevelPauseDialogContext = new InLevelPauseDialogContext();
				inLevelPauseDialogContext.OnClosed += delegate
				{
					_pauseDialogShown = false;
				};
				Singleton<MainUIManager>.Instance.ShowDialog(inLevelPauseDialogContext);
				_pauseDialogShown = true;
			}
		}

		public void OnAutoToggleValueChange(bool value)
		{
			Singleton<AvatarManager>.Instance.SetAutoBattle(value);
		}

		public void SetInLevelMainPageActive(bool active, bool instant = false, bool force = false)
		{
			if (!(_mainPageFadeAnim != null) || (_showState == InLevelMainPageShowState.Changing && !force))
			{
				return;
			}
			SetActive(true);
			InLevelMainPageShowState inLevelMainPageShowState = ((!active) ? InLevelMainPageShowState.Hide : InLevelMainPageShowState.Show);
			if (instant)
			{
				SetActive(active);
				_showState = ((!active) ? InLevelMainPageShowState.Hide : InLevelMainPageShowState.Show);
			}
			else if (_showState != inLevelMainPageShowState || force)
			{
				string animation = ((!active) ? "InlevelmainPageFadeout" : "InlevelmainPageFadein");
				_mainPageFadeAnim.Play(animation);
				_showState = InLevelMainPageShowState.Changing;
				if (!base.IsActive)
				{
					SetActive(true);
				}
				Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(WaitFadeAnimation(_mainPageFadeAnim, active, null));
			}
		}

		private IEnumerator WaitFadeAnimation(Animation animation, bool active, Action callback)
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PauseBtnEnable, false));
			while (animation != null && animation.isPlaying)
			{
				yield return null;
			}
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PauseBtnEnable, true));
			if (active)
			{
				_showState = InLevelMainPageShowState.Show;
			}
			else
			{
				_showState = InLevelMainPageShowState.Hide;
				SetActive(false);
			}
			if (callback != null)
			{
				callback();
			}
		}

		private bool OnLoadingSceneDestroyed()
		{
			Singleton<EventManager>.Instance.FireEvent(new EvtLoadingState(EvtLoadingState.State.Destroy));
			return false;
		}

		private bool OnAvatarCreate(uint runtimeId)
		{
			avatarButtonContainer.AddAvatarButton(runtimeId);
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(runtimeId))
			{
				Singleton<MainUIManager>.Instance.GetInLevelUICanvas().OnUpdateLocalAvatar(runtimeId, 0u);
			}
			return false;
		}

		private bool PostStageReady()
		{
			Singleton<MainUIManager>.Instance.GetInLevelUICanvas().OnUpdateLocalAvatarAbilityDisplay(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), 0u);
			return false;
		}

		private bool ShowDamegeText(EvtBeingHit evt)
		{
			bool flag = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) != 4;
			BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(evt.targetID);
			if (entity == null)
			{
				return false;
			}
			if (!Singleton<CameraManager>.Instance.GetMainCamera().IsEntityVisible(entity))
			{
				return false;
			}
			if (flag && !GlobalVars.LEVEL_MODE_DEBUG)
			{
				return false;
			}
			if (evt.attackData.rejected)
			{
				return false;
			}
			if (evt.attackData == null || evt.attackData.hitCollision == null || evt.attackData.resolveStep != AttackData.AttackDataStep.FinalResolved)
			{
				return false;
			}
			base.view.transform.Find("DamageTextContainer").GetComponent<MonoDamageTextContainer>().ShowDamageText(evt.attackData, entity);
			return false;
		}

		private bool SetTimeCountDownTextActive(bool active)
		{
			base.view.transform.Find("TimeCountDown").gameObject.SetActive(active);
			return false;
		}

		private bool SetTimeCountDownText(float remainTime)
		{
			int num = Mathf.CeilToInt(remainTime) / 60;
			int num2 = Mathf.CeilToInt(remainTime) - 60 * num;
			_timeCountDownText.text = string.Format("{0:D2}:{1:D2}", num, num2);
			if (remainTime <= 10f)
			{
				base.view.transform.Find("TimeCountDown").GetComponent<Animation>().Play();
				_timeCountDownText.color = MiscData.GetColor("WarningRed");
			}
			else
			{
				_timeCountDownText.color = MiscData.GetColor("TotalWhite");
			}
			return false;
		}

		private bool ShowAddTimeText(float addTime)
		{
			if (addTime > 0f)
			{
				_addTimeText.text = "+" + addTime + "s";
			}
			else
			{
				_addTimeText.text = addTime + "s";
			}
			_addTimeText.gameObject.SetActive(true);
			base.view.transform.Find("TimeCountDown").GetComponent<Animation>().Play();
			return false;
		}

		private bool SetTimesUpText(string msg)
		{
			ShowLevelDisplayText(LocalizationGeneralLogic.GetText("LevelDisplay_TimeUp"));
			return false;
		}

		private bool SetDropItemCount(int count)
		{
			base.view.transform.Find("LevelInfoPanel/DropItem/NumText").GetComponent<Text>().text = "x" + count;
			return false;
		}

		private bool SetTimerText(float time)
		{
			int num = Mathf.CeilToInt(time) / 60;
			int num2 = Mathf.CeilToInt(time) - 60 * num;
			_timerText.text = string.Format("{0:D2}:{1:D2}", num, num2);
			return false;
		}

		private bool SetDefendModeTextEnable(bool enable)
		{
			base.view.transform.Find("DefenseMode").gameObject.SetActive(enable);
			return false;
		}

		private bool SetDefendModeText(string msg)
		{
			Text component = base.view.transform.Find("DefenseMode/Timing/Text").GetComponent<Text>();
			if (component != null)
			{
				component.text = msg;
				return true;
			}
			return false;
		}

		private bool OnAttackLandedNotify(EvtAttackLanded evt)
		{
			if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.attackeeID) == 4)
			{
				MonsterActor monsterActor = Singleton<EventManager>.Instance.GetActor(evt.attackeeID) as MonsterActor;
				if (monsterActor != null && (monsterActor.showSubHpBarWhenAttackLanded || Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi))
				{
					MonoSubMonsterHPBarContainer component = base.view.transform.Find("SubMonsterHPBarContainer").GetComponent<MonoSubMonsterHPBarContainer>();
					component.OnAttackLandedEvt(evt);
				}
			}
			return false;
		}

		private bool OnTutorialPlayerTeaching(LevelTutorialPlayerTeaching tutorial)
		{
			if (tutorial.step == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					isMaskClickable = true,
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep1Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 1)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					isMaskClickable = true,
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep2Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 2)
			{
				Transform highlightTrans = base.view.transform.Find("InputController/MoveJoystick");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					highlightTrans = highlightTrans,
					delayInputTime = _tutorialDelayInputTime,
					disableHighlightInvoke = true,
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Up,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep3Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 3)
			{
				Transform highlightTrans2 = base.view.transform.Find("InputController/MoveJoystick");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					highlightTrans = highlightTrans2,
					delayInputTime = _tutorialDelayInputTime,
					disableHighlightInvoke = true,
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Up,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStpe4Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 4)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					isMaskClickable = true,
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep5Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 5)
			{
				Transform highlightTrans3 = base.view.transform.Find("LocalAvatarStatus/HP/Bar");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					highlightTrans = highlightTrans3,
					delayInputTime = _tutorialDelayInputTime,
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep6Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 6)
			{
				Transform highlightTrans4 = base.view.transform.Find("InputController/SkillButton_1");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					highlightTrans = highlightTrans4,
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStpe7Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 7)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					isMaskClickable = true,
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutoriaStep8Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 8)
			{
				Transform highlightTrans5 = base.view.transform.Find("MonsterStatus/HPBar");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					highlightTrans = highlightTrans5,
					delayInputTime = _tutorialDelayInputTime,
					isMaskClickable = true,
					disableHighlightInvoke = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Bottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep8_1Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 9)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutoriaStep9Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 10)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutoriaStep10Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 11)
			{
				Transform highlightTrans6 = base.view.transform.Find("InputController/SkillButton_2");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					highlightTrans = highlightTrans6,
					delayInputTime = _tutorialDelayInputTime,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutoriaStep11Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 12)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					isMaskClickable = true,
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutoriaStep12Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 13)
			{
				Transform highlightTrans7 = base.view.transform.Find("InputController/SkillButton_2");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					highlightTrans = highlightTrans7,
					isMaskClickable = true,
					disableHighlightEffect = true,
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep13Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 14)
			{
				Transform highlightTrans8 = base.view.transform.Find("InputController/SkillButton_2");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					highlightTrans = highlightTrans8,
					isMaskClickable = true,
					disableHighlightEffect = true,
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep14Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 15)
			{
				Transform highlightTrans9 = base.view.transform.Find("InputController/SkillButton_2");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					highlightTrans = highlightTrans9,
					delayInputTime = _tutorialDelayInputTime,
					isMaskClickable = false,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep15Done();
						return false;
					}
				});
			}
			return false;
		}

		private bool OnTutorialUltraAttackNotify(LevelTutorialUltraAttack tutorial)
		{
			if (tutorial.step == 0)
			{
				Transform highlightTrans = base.view.transform.Find("InputController/SkillButton_3");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					highlightTrans = highlightTrans,
					delayInputTime = _tutorialDelayInputTime,
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnStep1Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 1)
			{
				Transform highlightTrans2 = base.view.transform.Find("LocalAvatarStatus/SP");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					highlightTrans = highlightTrans2,
					delayInputTime = _tutorialDelayInputTime,
					isMaskClickable = true,
					disableHighlightInvoke = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnStep2Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 2)
			{
				Transform highlightTrans3 = base.view.transform.Find("LocalAvatarStatus/SP");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					highlightTrans = highlightTrans3,
					delayInputTime = _tutorialDelayInputTime,
					isMaskClickable = true,
					disableHighlightInvoke = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnStep3Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 3)
			{
				Transform transform = base.view.transform.Find("InputController/SkillButton_3");
				transform.GetComponent<Button>().interactable = true;
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					highlightTrans = transform,
					delayInputTime = _tutorialDelayInputTime,
					isMaskClickable = false,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnStep4Done();
						return false;
					}
				});
			}
			else if (tutorial.step != 4)
			{
			}
			return false;
		}

		private bool OnTutorialBranchAttackNotify(LevelTutorialBranchAttack tutorial)
		{
			if (tutorial.step == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnStep1Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 1)
			{
				string prefabPath = "SpriteOutput/GuideImgs/PicSkillGuide 1";
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					guideSprite = Miscs.GetSpriteByPrefab(prefabPath),
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnStep2Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 2)
			{
				string prefabPath = "SpriteOutput/GuideImgs/PicSkillGuide 1";
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					guideSprite = Miscs.GetSpriteByPrefab(prefabPath),
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnStep3Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 3)
			{
				string prefabPath = "SpriteOutput/GuideImgs/PicSkillGuide 1";
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					guideSprite = Miscs.GetSpriteByPrefab(prefabPath),
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnStep4Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 4)
			{
				Transform highlightTrans = base.view.transform.Find("InputController/SkillButton_1");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					highlightTrans = highlightTrans,
					delayInputTime = _tutorialDelayInputTime,
					destroyByOthers = true,
					disableMask = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerDownCallback = tutorial.OnStep5PointerDown,
					pointerUpCallback = tutorial.OnStep5PoointerUp
				});
			}
			else if (tutorial.step == 5)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnStep6Done();
						return false;
					}
				});
			}
			return false;
		}

		private bool OnTutorialEliteAttackNotify(LevelTutorialEliteAttack tutorial)
		{
			if (tutorial.step == 0)
			{
				Transform highlightTrans = base.view.transform.Find("MonsterStatus/ShieldBar");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					highlightTrans = highlightTrans,
					delayInputTime = _tutorialDelayInputTime,
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.RightUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep1Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 1)
			{
				Transform highlightTrans2 = base.view.transform.Find("MonsterStatus/ShieldBar");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					highlightTrans = highlightTrans2,
					delayInputTime = _tutorialDelayInputTime,
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.RightUp,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep2Done();
						return false;
					}
				});
			}
			return false;
		}

		private bool OnTutorialSwapAttackNotify(LevelTutorialSwapAttack tutorial)
		{
			Transform transform = null;
			Transform transform2 = base.view.transform.Find("AvatarBtns");
			MonoAvatarButton avatarButtonByRuntimeID = transform2.GetComponent<MonoAvatarButtonContainer>().GetAvatarButtonByRuntimeID(tutorial.targetSwapAvatarId);
			transform = avatarButtonByRuntimeID.gameObject.transform;
			if (transform == null)
			{
				return false;
			}
			if (tutorial.step == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					highlightTrans = transform,
					delayInputTime = _tutorialDelayInputTime,
					disableHighlightInvoke = true,
					isMaskClickable = true,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep1Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 1)
			{
				Transform highlightTrans = transform.Find("HPBar");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					highlightTrans = highlightTrans,
					delayInputTime = _tutorialDelayInputTime,
					disableHighlightInvoke = true,
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep2Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 2)
			{
				Transform highlightTrans2 = transform.Find("SPBar");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					highlightTrans = highlightTrans2,
					delayInputTime = _tutorialDelayInputTime,
					disableHighlightInvoke = true,
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep3Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 3)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					highlightTrans = transform,
					delayInputTime = _tutorialDelayInputTime,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep4Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 4)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					highlightTrans = transform,
					delayInputTime = _tutorialDelayInputTime,
					isMaskClickable = true,
					disableHighlightInvoke = true,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep5Done();
						return false;
					}
				});
			}
			return false;
		}

		private bool OnTutorialSwapAndRestrain(LevelTutorialSwapAndRestrain tutorial)
		{
			Transform transform = null;
			Transform transform2 = base.view.transform.Find("AvatarBtns");
			MonoAvatarButton avatarButtonByRuntimeID = transform2.GetComponent<MonoAvatarButtonContainer>().GetAvatarButtonByRuntimeID(tutorial.targetSwapAvatarId);
			transform = avatarButtonByRuntimeID.gameObject.transform;
			string prefabPath = "SpriteOutput/GuideImgs/PicNatureGuide1";
			if (transform == null)
			{
				return false;
			}
			if (tutorial.step == 0)
			{
				Transform highlightTrans = base.view.transform.Find("MonsterStatus/DamageMark");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					delayInputTime = _tutorialDelayInputTime,
					highlightTrans = highlightTrans,
					isMaskClickable = true,
					disableHighlightInvoke = true,
					handIconPosType = NewbieDialogContext.HandIconPosType.Bottom,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep0Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 1)
			{
				Transform highlightTrans2 = base.view.transform.Find("MonsterStatus/DamageMark");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					delayInputTime = _tutorialDelayInputTime,
					highlightTrans = highlightTrans2,
					isMaskClickable = true,
					disableHighlightInvoke = true,
					handIconPosType = NewbieDialogContext.HandIconPosType.Bottom,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep1Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 2)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					guideSprite = Miscs.GetSpriteByPrefab(prefabPath),
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep2Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 3)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					guideSprite = Miscs.GetSpriteByPrefab(prefabPath),
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep3Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 4)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					guideSprite = Miscs.GetSpriteByPrefab(prefabPath),
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep4Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 5)
			{
				Transform highlightTrans3 = base.view.transform.Find("MonsterStatus/DamageMark");
				bool flag = !tutorial.isFirstDead;
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					highlightTrans = highlightTrans3,
					delayInputTime = _tutorialDelayInputTime,
					disableHighlightInvoke = true,
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					handIconPosType = (flag ? NewbieDialogContext.HandIconPosType.Bottom : NewbieDialogContext.HandIconPosType.None),
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep5Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 6)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					delayShowTime = 0.01f,
					highlightTrans = transform,
					delayInputTime = _tutorialDelayInputTime,
					disableHighlightInvoke = true,
					isMaskClickable = true,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep6Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 7)
			{
				Transform highlightTrans4 = transform.Find("HPBar");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					highlightTrans = highlightTrans4,
					delayInputTime = _tutorialDelayInputTime,
					disableHighlightInvoke = true,
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep7Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 8)
			{
				Transform highlightTrans5 = transform.Find("SPBar");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					highlightTrans = highlightTrans5,
					delayInputTime = _tutorialDelayInputTime,
					disableHighlightInvoke = true,
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep8Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 9)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					highlightTrans = transform,
					delayInputTime = _tutorialDelayInputTime,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep9Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 10)
			{
				Transform highlightTrans6 = base.view.transform.Find("MonsterStatus/DamageMark");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					delayInputTime = _tutorialDelayInputTime,
					highlightTrans = highlightTrans6,
					isMaskClickable = true,
					disableHighlightInvoke = true,
					handIconPosType = NewbieDialogContext.HandIconPosType.Bottom,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep10Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 11)
			{
				string prefabPath2 = "SpriteOutput/GuideImgs/PicSkillGuide 2";
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					guideSprite = Miscs.GetSpriteByPrefab(prefabPath2),
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep11Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 12)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					isMaskClickable = true,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep12Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 13)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep13Done();
						return false;
					}
				});
			}
			return false;
		}

		private bool OnTutorialMonsterBlock(LevelTutorialMonsterBlock tutorial)
		{
			if (tutorial.step == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutoriaStep1Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 1)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					isMaskClickable = true,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep2Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 2)
			{
				Transform highlightTrans = base.view.transform.Find("InputController/SkillButton_2");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					highlightTrans = highlightTrans,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep3Done();
						return false;
					}
				});
			}
			return false;
		}

		private bool OnTutorialNatureRestrain(LevelTutorialNatureRestrain tutorial)
		{
			if (tutorial.step == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep1Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 1)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep2Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 2)
			{
				Transform highlightTrans = base.view.transform.Find("MonsterStatus/DamageMark");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					disableHighlightEffect = true,
					delayInputTime = _tutorialDelayInputTime,
					highlightTrans = highlightTrans,
					isMaskClickable = true,
					disableHighlightInvoke = true,
					handIconPosType = NewbieDialogContext.HandIconPosType.Bottom,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep3Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 3)
			{
				string prefabPath = "SpriteOutput/GuideImgs/PicNatureGuide1";
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					guideSprite = Miscs.GetSpriteByPrefab(prefabPath),
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep4Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 4)
			{
				string prefabPath2 = "SpriteOutput/GuideImgs/PicNatureGuide1";
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					guideSprite = Miscs.GetSpriteByPrefab(prefabPath2),
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep5Done();
						return false;
					}
				});
			}
			return false;
		}

		private bool OnTutorialMonsterTeleport(LevelTutorialMonsterTeleport tutorial)
		{
			if (tutorial.step == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftUp,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep1Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 1)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep2Done();
						return false;
					}
				});
			}
			return false;
		}

		private bool OnTutorialMonsterShield(LevelTutorialMonsterShield tutorial)
		{
			if (tutorial.step == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutoriaStep1Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 1)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					isMaskClickable = true,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep2Done();
						return false;
					}
				});
			}
			else if (tutorial.step == 2)
			{
				Transform highlightTrans = base.view.transform.Find("InputController/SkillButton_2");
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					highlightTrans = highlightTrans,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					handIconPosType = NewbieDialogContext.HandIconPosType.Left,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutorialStep3Done();
						return false;
					}
				});
			}
			return false;
		}

		private bool OnTutorialMonsterRobotDodge(LevelTutorialMonsterRobotDodge tutorial)
		{
			if (tutorial.step == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new NewbieDialogContext
				{
					delayInputTime = _tutorialDelayInputTime,
					bubblePosType = NewbieDialogContext.BubblePosType.LeftBottom,
					isMaskClickable = true,
					guideDesc = tutorial.GetDisplayTarget(tutorial.step),
					pointerUpCallback = delegate
					{
						tutorial.OnTutoriaStep1Done();
						return false;
					}
				});
			}
			return false;
		}

		private bool OnEvadeBtnVisibleControl(bool visible)
		{
			Transform transform = base.view.transform.Find("InputController/SkillButton_2");
			if (transform != null)
			{
				transform.gameObject.SetActive(visible);
			}
			return false;
		}

		private bool OnAttackBtnVisibleControl(bool visible)
		{
			Transform transform = base.view.transform.Find("InputController/SkillButton_1");
			if (transform != null)
			{
				transform.gameObject.SetActive(visible);
			}
			return false;
		}

		private bool OnJoystickVisibleControl(bool visible)
		{
			Transform transform = base.view.transform.Find("InputController/MoveJoystick");
			if (transform != null)
			{
				transform.gameObject.SetActive(visible);
			}
			return false;
		}

		private bool OnPauseBtnVisible(bool visible)
		{
			Transform transform = base.view.transform.Find("FuncBtns/PauseBtn");
			if (transform != null)
			{
				transform.gameObject.SetActive(visible);
			}
			return false;
		}

		private bool OnPauseBtnEnable(bool enable)
		{
			PauseBtnEnabled = enable;
			return false;
		}

		private bool OnSwapBtnVisible(bool visible)
		{
			Transform transform = base.view.transform.Find("AvatarBtns");
			if (transform != null)
			{
				transform.gameObject.SetActive(visible);
			}
			return false;
		}

		private bool OnEcoModeVisible(bool visible)
		{
			Transform transform = base.view.transform.Find("Ecomode");
			if (transform != null)
			{
				transform.gameObject.SetActive(visible);
			}
			return false;
		}

		private bool OnBattleBegin()
		{
			if (_hasShowTeamBuff)
			{
				return false;
			}
			SetupTeamBuff();
			base.view.transform.Find("TeamBuff").gameObject.SetActive(true);
			base.view.transform.Find("TeamBuff").GetComponent<Animation>().Play();
			_hasShowTeamBuff = true;
			return false;
		}

		private void SetupDebugMenu()
		{
			base.view.transform.Find("DebugWidget").gameObject.SetActive(GlobalVars.LEVEL_MODE_DEBUG);
		}

		public void OnLocalAvatarChanged(AvatarActor avatarBefore, AvatarActor avatarAfter)
		{
			SetupLocalAvatarStatus(avatarAfter);
			avatarAfter.onHPChanged = (Action<float, float, float>)Delegate.Combine(avatarAfter.onHPChanged, new Action<float, float, float>(UpdateHPView));
			avatarAfter.onMaxHPChanged = (Action<float, float>)Delegate.Combine(avatarAfter.onMaxHPChanged, new Action<float, float>(UpdateMaxHPView));
			avatarAfter.onSPChanged = (Action<float, float, float>)Delegate.Combine(avatarAfter.onSPChanged, new Action<float, float, float>(UpdateSPView));
			avatarAfter.onMaxSPChanged = (Action<float, float>)Delegate.Combine(avatarAfter.onMaxSPChanged, new Action<float, float>(UpdateMaxSPView));
			if (avatarBefore != null)
			{
				avatarBefore.onHPChanged = (Action<float, float, float>)Delegate.Remove(avatarBefore.onHPChanged, new Action<float, float, float>(UpdateHPView));
				avatarBefore.onMaxHPChanged = (Action<float, float>)Delegate.Remove(avatarBefore.onMaxHPChanged, new Action<float, float>(UpdateMaxHPView));
				avatarBefore.onSPChanged = (Action<float, float, float>)Delegate.Remove(avatarBefore.onSPChanged, new Action<float, float, float>(UpdateSPView));
				avatarBefore.onMaxSPChanged = (Action<float, float>)Delegate.Remove(avatarBefore.onMaxSPChanged, new Action<float, float>(UpdateMaxSPView));
			}
			if (avatarBefore != null && avatarAfter != null)
			{
				avatarButtonContainer.PlaySwapAvatarAnim(avatarBefore.runtimeID, avatarAfter.runtimeID);
			}
			if (avatarAfter != null)
			{
				bool active = !avatarAfter.IsSkillLocked("SKL02");
				base.view.transform.Find("LocalAvatarStatus/SP").gameObject.SetActive(active);
			}
			MonoMonsterStatus component = base.view.transform.Find("MonsterStatus").GetComponent<MonoMonsterStatus>();
			if (component.gameObject.activeSelf)
			{
				component.SetupNatureBonus();
				component.SetupMonsterNameByLevelPunish();
			}
			_healthMode = IsLocalAvatarInLowHP();
			SetHPWarningByHealthMode(_healthMode);
		}

		private bool SetupLocalAvatarStatus(AvatarActor avatar)
		{
			UpdateHPView(avatar.HP, avatar.HP, 0f);
			UpdateSPView(avatar.SP, avatar.SP, 0f);
			SetContoller(avatar);
			return false;
		}

		private void UpdateMaxHPView(float from, float to)
		{
			uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
			UpdateHPView(actor.HP, actor.HP, 0f);
		}

		private void UpdateHPView(float from, float to, float delta)
		{
			uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
			int num = UIUtil.FloorToIntCustom(to);
			int num2 = UIUtil.FloorToIntCustom(actor.maxHP);
			MonoSliderGroup component = base.view.transform.Find("LocalAvatarStatus/HP/Bar").GetComponent<MonoSliderGroup>();
			component.UpdateValue(num, num2, 0f);
			base.view.transform.Find("LocalAvatarStatus/HP/NumText/Num").GetComponent<Text>().text = num.ToString();
			base.view.transform.Find("LocalAvatarStatus/HP/NumText/MaxNum").GetComponent<Text>().text = num2.ToString();
			_healthMode = IsLocalAvatarInLowHP();
			SetHPWarningByHealthMode(_healthMode);
			if (_hpDisplayText != null)
			{
				int delta2 = UIUtil.FloorToIntCustom(delta);
				_hpDisplayText.SetupView((int)from, num, delta2);
			}
		}

		public void RefreshSPView(float from, float to, float delta)
		{
			UpdateSPView(from, to, delta);
		}

		private void UpdateMaxSPView(float from, float to)
		{
			uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
			UpdateSPView(actor.SP, actor.SP, 0f);
		}

		private void UpdateSPView(float from, float to, float delta)
		{
			uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
			if (actor.IsSkillLocked("SKL02"))
			{
				base.view.transform.Find("LocalAvatarStatus/SP").gameObject.SetActive(false);
				return;
			}
			base.view.transform.Find("LocalAvatarStatus/SP").gameObject.SetActive(true);
			int num = UIUtil.FloorToIntCustom(to);
			int num2 = UIUtil.FloorToIntCustom(actor.maxSP);
			MonoMaskSlider component = base.view.transform.Find("LocalAvatarStatus/SP/Bar/MaskSlider").GetComponent<MonoMaskSlider>();
			component.UpdateValue(num, num2, 0f);
			base.view.transform.Find("LocalAvatarStatus/SP/NumText/MaxNum").GetComponent<Text>().text = num2.ToString();
			bool flag = num == num2;
			Text component2 = base.view.transform.Find("LocalAvatarStatus/SP/NumText/Num").GetComponent<Text>();
			component2.text = ((!flag) ? num.ToString() : "MAX");
			component2.color = ((!flag) ? Color.white : MiscData.GetColor("TextOrange"));
			if (!(_spDisplayText != null))
			{
				return;
			}
			bool showText = false;
			if (delta > 0f)
			{
				int num3 = UIUtil.FloorToIntCustom(delta);
				if (num3 >= 2)
				{
					showText = true;
				}
			}
			_spDisplayText.SetupView(from, to, delta, showText);
		}

		private void UpdateComboText(int from, int to)
		{
			_comboText.SetupView(from, to);
		}

		private bool OnResistComboClear()
		{
			_comboText.ActBlingEffect();
			return false;
		}

		private LocalAvatarHealthMode IsLocalAvatarInLowHP()
		{
			uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
			if (actor != null && (float)actor.HP < (float)actor.maxHP * 0.3f)
			{
				_hurtRatio = 1f - (float)actor.HP / ((float)actor.maxHP * 0.3f);
				return LocalAvatarHealthMode.Unhealthy;
			}
			return LocalAvatarHealthMode.Healthy;
		}

		private void SetHPWarningByHealthMode(LocalAvatarHealthMode mode)
		{
			Animation component = base.view.transform.Find("RedFrame").GetComponent<Animation>();
			CanvasGroup component2 = base.view.transform.Find("RedFrame").GetComponent<CanvasGroup>();
			MonoSliderGroup component3 = base.view.transform.Find("LocalAvatarStatus/HP/Bar").GetComponent<MonoSliderGroup>();
			if (component == null || component2 == null || component3 == null)
			{
				return;
			}
			if (mode == LocalAvatarHealthMode.Unhealthy)
			{
				base.view.transform.Find("RedFrame").gameObject.SetActive(true);
				foreach (AnimationState item in component)
				{
					item.speed = Mathf.Lerp(0.5f, 2f, _hurtRatio);
				}
				component.Play();
				component3.SetupInDanageView(mode);
			}
			else
			{
				component.Stop();
				component2.alpha = 0f;
				component3.SetupInDanageView(mode);
				base.view.transform.Find("RedFrame").gameObject.SetActive(false);
			}
		}

		public void HideMonsterStatus()
		{
			base.view.transform.Find("MonsterStatus").gameObject.SetActive(false);
		}

		public void OnTargetMonsterChange(MonsterActor targetBefore, MonsterActor targetAfter)
		{
			MonoMonsterStatus component = base.view.transform.Find("MonsterStatus").GetComponent<MonoMonsterStatus>();
			component.SetupView(targetBefore, targetAfter);
			component.SetupNatureBonus();
			component.SetupMonsterNameByLevelPunish();
			MonoSubMonsterHPBarContainer component2 = base.view.transform.Find("SubMonsterHPBarContainer").GetComponent<MonoSubMonsterHPBarContainer>();
			component2.OnTargetMonsterChange(targetBefore, targetAfter);
		}

		private void SetContoller(AvatarActor avatarActor)
		{
			BaseAvatarInputController inputController = avatarActor.avatar.GetInputController();
			skillButtonDict.Clear();
			MonoSkillButton[] componentsInChildren = base.view.transform.Find("InputController").GetComponentsInChildren<MonoSkillButton>(true);
			MonoSkillButton[] array = componentsInChildren;
			foreach (MonoSkillButton monoSkillButton in array)
			{
				if (avatarActor.GetSkillInfo(monoSkillButton.SkillName) != null)
				{
					skillButtonDict.Add(monoSkillButton.SkillName, monoSkillButton);
					monoSkillButton.InitSkillButton(inputController);
					monoSkillButton.gameObject.SetActive(!avatarActor.IsSkillLocked(monoSkillButton.SkillName));
				}
				else
				{
					monoSkillButton.gameObject.SetActive(false);
				}
			}
			MonoJoystick component = base.view.transform.Find("InputController/MoveJoystick").GetComponent<MonoJoystick>();
			component.InitJoystick(inputController);
			base.view.transform.Find("InputController").gameObject.SetActive(true);
		}

		public MonoSkillButton GetSkillButtonBySkillID(string skillID)
		{
			return skillButtonDict[skillID];
		}

		public Transform GetSPBar()
		{
			return base.view.transform.Find("LocalAvatarStatus/SP");
		}

		private bool ShowHelperCutIn()
		{
			base.view.transform.Find("HelperCutIn/Content/Name").GetComponent<Text>().text = Singleton<LevelScoreManager>.Instance.friendDetailItem.nickName;
			string iconPath = Singleton<LevelScoreManager>.Instance.friendDetailItem.leaderAvatar.IconPath;
			base.view.transform.Find("HelperCutIn/Content/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(iconPath);
			base.view.transform.Find("HelperCutIn").gameObject.SetActive(true);
			return false;
		}

		public bool SetOverHeatViewActive(bool active)
		{
			_buttonOverHeatPlugin.gameObject.SetActive(active);
			return false;
		}

		public void OnIsOverheatChanged(float wasOverheat, float isOverheat)
		{
			_isOverheat = isOverheat > 0f;
			UpdateOverHeatView(_isOverheat, _overheatRatio);
		}

		public void OnOverheatRatioChanged(float oldRatio, float newRatio)
		{
			_overheatRatio = newRatio;
			UpdateOverHeatView(_isOverheat, _overheatRatio);
		}

		public void UpdateOverHeatView(bool isOverHeat, float ratio)
		{
			float num = 0.5f;
			CanvasGroup component = _buttonOverHeatPlugin.GetComponent<CanvasGroup>();
			if (!isOverHeat && ratio <= num)
			{
				component.alpha = ratio / num;
			}
			else
			{
				component.alpha = 1f;
			}
			_buttonOverHeatPlugin.Find("FrameWhite").gameObject.SetActive(!isOverHeat);
			_buttonOverHeatPlugin.Find("FrameRed").gameObject.SetActive(isOverHeat);
			Image component2 = _buttonOverHeatPlugin.Find("SpColor").GetComponent<Image>();
			Image component3 = _buttonOverHeatPlugin.Find("SpDisable").GetComponent<Image>();
			component2.gameObject.SetActive(!isOverHeat);
			component3.gameObject.SetActive(isOverHeat);
			if (isOverHeat)
			{
				component3.fillAmount = ratio;
			}
			else
			{
				component2.fillAmount = ratio;
			}
		}

		private bool ShowLevelDisplayText(string text)
		{
			base.view.transform.Find("LevelDisplayText/Text").GetComponent<Text>().text = text;
			base.view.transform.Find("LevelDisplayText").gameObject.SetActive(true);
			return false;
		}

		private bool ShowVerticalDrawing(string text)
		{
			return ShowLevelDisplayText(text);
		}

		private void SetupTeamBuff()
		{
			AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetTeamLeader().GetRuntimeID());
			AvatarSkillDataItem leaderSkill = actor.avatarDataItem.GetLeaderSkill();
			base.view.transform.Find("TeamBuff/Self/SkillName").GetComponent<Text>().text = leaderSkill.SkillName;
			base.view.transform.Find("TeamBuff/Self/Desc").GetComponent<Text>().text = leaderSkill.SkillShortInfo;
			base.view.transform.Find("TeamBuff/Friend").gameObject.SetActive(false);
			BaseMonoAvatar helperAvatar = Singleton<AvatarManager>.Instance.GetHelperAvatar();
			if (helperAvatar != null && Singleton<FriendModule>.Instance.IsMyFriend(Singleton<LevelScoreManager>.Instance.friendDetailItem.uid))
			{
				AvatarActor actor2 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(helperAvatar.GetRuntimeID());
				AvatarSkillDataItem leaderSkill2 = actor2.avatarDataItem.GetLeaderSkill();
				base.view.transform.Find("TeamBuff/Friend").gameObject.SetActive(true);
				base.view.transform.Find("TeamBuff/Friend/SkillName").GetComponent<Text>().text = leaderSkill2.SkillName;
				base.view.transform.Find("TeamBuff/Friend/Desc").GetComponent<Text>().text = leaderSkill2.SkillShortInfo;
				base.view.transform.Find("TeamBuff/Friend/SkillName/Hint").gameObject.SetActive(false);
			}
		}

		private bool OnSocketConnect()
		{
			TryPauseGameByOthers(false);
			return false;
		}

		private bool OnSocketDisconnect()
		{
			TryPauseGameByOthers(true);
			return false;
		}

		private bool OnQuitGameDialogDestroy()
		{
			TryPauseGameByOthers(false);
			return false;
		}

		private bool OnQuitGameDialogShow()
		{
			TryPauseGameByOthers(true);
			return false;
		}

		private void TryPauseGameByOthers(bool pause)
		{
			if (Singleton<LevelManager>.Instance != null && Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning && Singleton<LevelManager>.Instance.IsPaused() != pause && !_pauseDialogShown && _showState != InLevelMainPageShowState.Changing)
			{
				Singleton<LevelManager>.Instance.SetPause(pause);
			}
		}
	}
}
