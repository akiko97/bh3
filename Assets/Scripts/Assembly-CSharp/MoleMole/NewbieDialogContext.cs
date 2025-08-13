using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class NewbieDialogContext : BaseDialogContext
	{
		public enum BubblePosType
		{
			None = 0,
			LeftUp = 1,
			LeftBottom = 2,
			RightUp = 3,
			RightBottom = 4
		}

		public enum HandIconPosType
		{
			None = 0,
			Up = 1,
			Bottom = 2,
			Left = 3,
			Right = 4,
			Downward = 5,
			Tips = 6,
			Arrow = 7
		}

		private const float HAND_ICON_ANIM_MOVE_DISTANCE = 30f;

		private const float HAND_ICON_ANIM_MOVE_DURATION = 0.15f;

		private const string expPrefabPath = "SpriteOutput/RewardGotIcons/Exp";

		private const string scoinPrefabPath = "SpriteOutput/RewardGotIcons/SCoin";

		private const string hcoinPrefabPath = "SpriteOutput/RewardGotIcons/HCoin";

		private const string friendPointPrefabPath = "SpriteOutput/RewardGotIcons/FriendPoint";

		private const string skillPointPrefabPath = "SpriteOutput/RewardGotIcons/SkillPoint";

		private const string staminaPrefabPath = "SpriteOutput/RewardGotIcons/Stamina";

		public bool disableMask;

		public Transform highlightTrans;

		public string highlightPath = string.Empty;

		public BubblePosType bubblePosType;

		public HandIconPosType handIconPosType;

		public bool disableHighlightEffect;

		public string guideDesc;

		public bool isMaskClickable;

		public bool disableHighlightInvoke;

		public Action preCallback;

		public Action pointerDownCallback;

		public Func<bool> pointerUpCallback;

		public Action destroyCallback;

		public Action destroyButNoClickedCallback;

		public Action destroyButHasClickedCallback;

		public float delayShowTime;

		public float delayInputTime;

		public Sprite guideSprite;

		public bool destroyByOthers;

		public bool bShowReward;

		public int rewardID;

		public int guideID;

		public bool bShowSkip;

		private Coroutine _delayShowCoroution;

		private bool _hasClicked;

		private NewbieHighlightInfo _newbieHightlightInfo;

		private Coroutine _handIconAnimCoroution;

		public bool playHighlightAnim;

		public BaseContext referredContext;

		private Coroutine _highlightEffectCoroution;

		private float _hightlightEffectAngleSpeed = 300f;

		private bool _isHighlightEffectStarted;

		private bool _waitDestroy;

		private Coroutine _delayInputCoroutine;

		private Coroutine _destroyCoroutine;

		private bool _hasSetup;

		private bool _hasDestroied;

		private SequenceAnimationManager _animationManager;

		private bool _bRewardClicked;

		public NewbieDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "NewbieDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/Newbie/NewbieDialog",
				cacheType = ViewCacheType.DontCache
			};
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 186)
			{
				return OnGetGuideRewardRsp(pkt.getData<GetGuideRewardRsp>());
			}
			return false;
		}

		public override bool OnNotify(Notify ntf)
		{
			return false;
		}

		protected override bool SetupView()
		{
			if (_hasSetup)
			{
				return false;
			}
			_hasSetup = true;
			SetupReward();
			if (_newbieHightlightInfo != null && highlightTrans == null)
			{
				Destroy();
				return false;
			}
			if (!CheckViewValid())
			{
				Destroy();
				return false;
			}
			if (highlightTrans != null)
			{
				playHighlightAnim = true;
			}
			SetupDisableInput();
			SetupMask();
			SetupGuideImage();
			SetReferredContext();
			if (delayShowTime > 0f && Singleton<ApplicationManager>.Instance != null)
			{
				_delayShowCoroution = Singleton<ApplicationManager>.Instance.StartCoroutine(WaitSetup());
			}
			else
			{
				SetupBubble();
				SetupHandIcon();
				HighlightView();
				SetupSkip();
			}
			return false;
		}

		public override void Destroy()
		{
			if (!_hasDestroied)
			{
				_hasDestroied = true;
				base.Destroy();
				StopDelayShowCoroution();
				StopHandIconAnim();
				StopHighlightEffectCoroutine();
				StopDelayInputCoroution();
				StopDestroyCoroution();
				RecoverHighlightView();
				if (_hasClicked && destroyButHasClickedCallback != null)
				{
					destroyButHasClickedCallback();
				}
				if (!_hasClicked && destroyButNoClickedCallback != null)
				{
					destroyButNoClickedCallback();
				}
				if (destroyCallback != null)
				{
					destroyCallback();
				}
			}
		}

		public override bool NeedRecoverWhenPageStartUp()
		{
			return false;
		}

		public override void SetActive(bool enabled)
		{
			playHighlightAnim = enabled;
			SetHighlightEffectActive(enabled);
			base.SetActive(enabled);
			if (!enabled && !_waitDestroy)
			{
				Destroy();
			}
		}

		public void SetAvailable(bool available)
		{
			playHighlightAnim = available;
			SetHighlightEffectActive(available);
			if (available)
			{
				CanvasGroup component = base.view.transform.GetComponent<CanvasGroup>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				return;
			}
			CanvasGroup canvasGroup = base.view.transform.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = base.view.AddComponent<CanvasGroup>();
			}
			canvasGroup.alpha = 0f;
			canvasGroup.blocksRaycasts = false;
		}

		private IEnumerator WaitDestroy()
		{
			yield return null;
			_destroyCoroutine = null;
			Destroy();
		}

		private IEnumerator WaitSetup()
		{
			yield return new WaitForSeconds(delayShowTime);
			if (base.view == null || base.view.transform == null)
			{
				Destroy();
				yield break;
			}
			SetupBubble();
			SetupHandIcon();
			HighlightView();
			SetupSkip();
			_delayShowCoroution = null;
		}

		private bool CheckViewValid()
		{
			return !disableMask || !(highlightTrans == null);
		}

		private void SetReferredContext()
		{
			if (!(highlightTrans == null) && !(highlightTrans.gameObject == null))
			{
				ContextIdentifier componentInParent = highlightTrans.gameObject.GetComponentInParent<ContextIdentifier>();
				if (componentInParent != null)
				{
					referredContext = componentInParent.context;
				}
			}
		}

		private void SetupDisableInput()
		{
			if (delayInputTime < 0.1f && handIconPosType == HandIconPosType.None)
			{
				delayInputTime = 1f;
			}
			if (delayInputTime > 0f && Singleton<ApplicationManager>.Instance != null)
			{
				_delayInputCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(WaitEnableInput());
				return;
			}
			Transform transform = base.view.transform.Find("DisableInput");
			transform.gameObject.SetActive(false);
		}

		private IEnumerator WaitEnableInput()
		{
			yield return new WaitForSeconds(delayInputTime);
			Transform disableInputTrans = base.view.transform.Find("DisableInput");
			disableInputTrans.gameObject.SetActive(false);
			_delayInputCoroutine = null;
		}

		private void SetupReward()
		{
			_animationManager = new SequenceAnimationManager(OnAllBoxAnimationEnd);
			if (bShowReward)
			{
				_bRewardClicked = false;
				base.view.transform.Find("Reward").gameObject.SetActive(true);
				highlightTrans = base.view.transform.Find("Reward/Button");
				BindViewCallback(highlightTrans, EventTriggerType.PointerClick, OnRewardPreCallback);
				BindViewCallback(highlightTrans, EventTriggerType.PointerClick, OnRewardClick);
				RewardData rewardDataByKey = RewardDataReader.GetRewardDataByKey(rewardID);
				Transform transform = base.view.transform.Find("Reward/Content/1");
				transform.gameObject.SetActive(false);
				Transform transform2 = base.view.transform.Find("Reward/Content/2");
				transform2.gameObject.SetActive(false);
				Transform transform3 = base.view.transform.Find("Reward/Content/3");
				transform3.gameObject.SetActive(false);
				Transform item = transform.Find("Item");
				Transform item2 = transform2.Find("Item");
				Transform item3 = transform3.Find("Item");
				List<Transform> list = new List<Transform>();
				list.Add(transform);
				list.Add(transform2);
				list.Add(transform3);
				List<Transform> list2 = list;
				list = new List<Transform>();
				list.Add(item);
				list.Add(item2);
				list.Add(item3);
				List<Transform> list3 = list;
				List<RewardUIData> rewardUIDataList = GetRewardUIDataList(rewardDataByKey);
				for (int i = 0; i < rewardUIDataList.Count; i++)
				{
					if (i < list2.Count)
					{
						Transform transform4 = list2[i];
						Transform transform5 = list3[i];
						transform4.gameObject.SetActive(true);
						_animationManager.AddAnimation(transform4.GetComponent<MonoAnimationinSequence>());
						RewardUIData rewardUIData = rewardUIDataList[i];
						if (rewardUIData.rewardType == ResourceType.Item)
						{
							StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(rewardUIData.itemID, rewardUIData.level);
							dummyStorageDataItem.number = rewardUIData.value;
							transform5.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, null, true);
						}
						else
						{
							HideRewardTransSomePart(transform5);
							transform5.GetComponent<MonoLevelDropIconButton>().Clear();
							transform5.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(rewardUIData.iconPath);
							transform5.Find("BG").gameObject.SetActive(true);
							transform5.Find("BG/Desc").GetComponent<Text>().text = "x" + rewardUIData.value;
						}
					}
				}
			}
			else
			{
				base.view.transform.Find("Reward").gameObject.SetActive(false);
			}
		}

		private List<RewardUIData> GetRewardUIDataList(RewardData rewardData)
		{
			List<RewardUIData> list = new List<RewardUIData>();
			if (rewardData.RewardExp > 0)
			{
				RewardUIData playerExpData = RewardUIData.GetPlayerExpData(rewardData.RewardExp);
				playerExpData.itemID = rewardData.RewardID;
				list.Add(playerExpData);
			}
			if (rewardData.RewardSCoin > 0)
			{
				RewardUIData scoinData = RewardUIData.GetScoinData(rewardData.RewardSCoin);
				scoinData.itemID = rewardData.RewardID;
				list.Add(scoinData);
			}
			if (rewardData.RewardHCoin > 0)
			{
				RewardUIData hcoinData = RewardUIData.GetHcoinData(rewardData.RewardHCoin);
				hcoinData.itemID = rewardData.RewardID;
				list.Add(hcoinData);
			}
			if (rewardData.RewardStamina > 0)
			{
				RewardUIData staminaData = RewardUIData.GetStaminaData(rewardData.RewardStamina);
				staminaData.itemID = rewardData.RewardID;
				list.Add(staminaData);
			}
			if (rewardData.RewardSkillPoint > 0)
			{
				RewardUIData skillPointData = RewardUIData.GetSkillPointData(rewardData.RewardSkillPoint);
				skillPointData.itemID = rewardData.RewardID;
				list.Add(skillPointData);
			}
			if (rewardData.RewardFriendPoint > 0)
			{
				RewardUIData friendPointData = RewardUIData.GetFriendPointData(rewardData.RewardFriendPoint);
				friendPointData.itemID = rewardData.RewardID;
				list.Add(friendPointData);
			}
			if (rewardData.RewardItem1ID > 0)
			{
				RewardUIData item = new RewardUIData(ResourceType.Item, rewardData.RewardItem1Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardData.RewardItem1ID, rewardData.RewardItem1Level);
				list.Add(item);
			}
			if (rewardData.RewardItem2ID > 0)
			{
				RewardUIData item2 = new RewardUIData(ResourceType.Item, rewardData.RewardItem2Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardData.RewardItem2ID, rewardData.RewardItem2Level);
				list.Add(item2);
			}
			if (rewardData.RewardItem3ID > 0)
			{
				RewardUIData item3 = new RewardUIData(ResourceType.Item, rewardData.RewardItem3Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardData.RewardItem3ID, rewardData.RewardItem3Level);
				list.Add(item3);
			}
			if (rewardData.RewardItem4ID > 0)
			{
				RewardUIData item4 = new RewardUIData(ResourceType.Item, rewardData.RewardItem4Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardData.RewardItem4ID, rewardData.RewardItem4Level);
				list.Add(item4);
			}
			if (rewardData.RewardItem5ID > 0)
			{
				RewardUIData item5 = new RewardUIData(ResourceType.Item, rewardData.RewardItem5Num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, rewardData.RewardItem5ID, rewardData.RewardItem5Level);
				list.Add(item5);
			}
			return list;
		}

		private void HideRewardTransSomePart(Transform rewardTrans)
		{
			rewardTrans.Find("BG/UnidentifyText").gameObject.SetActive(false);
			rewardTrans.Find("NewMark").gameObject.SetActive(false);
			rewardTrans.Find("AvatarStar").gameObject.SetActive(false);
			rewardTrans.Find("Star").gameObject.SetActive(false);
			rewardTrans.Find("StigmataType").gameObject.SetActive(false);
			rewardTrans.Find("FragmentIcon").gameObject.SetActive(false);
		}

		private void OnRewardPreCallback(BaseEventData data = null)
		{
			OnNewbieMaskPreCallback(data);
		}

		private void OnRewardClick(BaseEventData data = null)
		{
			if (!_bRewardClicked)
			{
				Singleton<NetworkManager>.Instance.RequestGuideReward(guideID);
				_bRewardClicked = true;
			}
		}

		private bool OnGetGuideRewardRsp(GetGuideRewardRsp rsp)
		{
			_animationManager.StartPlay();
			return false;
		}

		private void OnAllBoxAnimationEnd()
		{
			if (Singleton<ApplicationManager>.Instance != null)
			{
				Singleton<ApplicationManager>.Instance.StartCoroutine(RewardDelayClick());
			}
		}

		private IEnumerator RewardDelayClick()
		{
			yield return new WaitForSeconds(0.8f);
			OnNewbieMaskClick();
		}

		private void SetupMask()
		{
			Transform transform = base.view.transform.FindChild("Mask");
			transform.GetComponent<Image>().enabled = !disableMask;
			if ((!disableMask && highlightTrans == null) || isMaskClickable || handIconPosType == HandIconPosType.None || handIconPosType == HandIconPosType.Arrow)
			{
				BindViewCallback(transform, EventTriggerType.PointerClick, OnNewbieMaskPreCallback);
				BindViewCallback(transform, EventTriggerType.PointerClick, OnNewbieMaskClick);
			}
		}

		private void SetupGuideImage()
		{
			if (guideSprite != null)
			{
				Transform transform = base.view.transform.Find("GuideImage");
				transform.gameObject.SetActive(true);
				transform.GetComponent<Image>().sprite = guideSprite;
			}
		}

		private void SetupSkip()
		{
			if (!(base.view == null) && !(base.view.transform == null))
			{
				Transform transform = base.view.transform.Find("Skip");
				transform.gameObject.SetActive(bShowSkip);
				if (bShowSkip)
				{
					BindViewCallback(transform.GetComponent<Button>(), OnSkipClick);
				}
			}
		}

		private void SetupBubble()
		{
			if (base.view == null || base.view.transform == null)
			{
				return;
			}
			Transform transform = base.view.transform.Find("Bubble");
			if (transform == null)
			{
				return;
			}
			if (bubblePosType == BubblePosType.None)
			{
				transform.gameObject.SetActive(false);
				return;
			}
			transform.gameObject.SetActive(true);
			foreach (Transform item in transform)
			{
				if (item.gameObject.name == bubblePosType.ToString())
				{
					item.gameObject.SetActive(true);
				}
				else
				{
					item.gameObject.SetActive(false);
				}
			}
			Transform transform3 = transform.FindChild(bubblePosType.ToString() + "/Head/Icon");
			if (!(transform3 == null))
			{
				float num = UnityEngine.Random.Range(0f, 3f);
				if (num <= 1f)
				{
					transform3.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/Newbie/CharaHeadAIMusume01");
				}
				else if (num <= 2f)
				{
					transform3.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/Newbie/CharaHeadAIMusume02");
				}
				else
				{
					transform3.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/Newbie/CharaHeadAIMusume03");
				}
				Transform transform4 = transform.FindChild(bubblePosType.ToString() + "/Text");
				if (!string.IsNullOrEmpty(guideDesc))
				{
					transform4.FindChild("Panel/Text").GetComponent<Text>().text = guideDesc;
				}
			}
		}

		private void OnSkipClick()
		{
			Singleton<TutorialModule>.Instance.TryToSkipTutorial(guideID, OnSkipFinish);
		}

		private void OnSkipFinish()
		{
			Destroy();
		}

		private void SetupHandIcon()
		{
			if (base.view == null || base.view.transform == null)
			{
				return;
			}
			Transform transform = base.view.transform.FindChild("HandIcon");
			if (transform == null)
			{
				return;
			}
			if (handIconPosType == HandIconPosType.Downward)
			{
				base.view.transform.FindChild("HandIcon/Downward").gameObject.SetActive(true);
				return;
			}
			if (handIconPosType == HandIconPosType.Tips)
			{
				base.view.transform.FindChild("Tips").gameObject.SetActive(true);
				return;
			}
			if (handIconPosType == HandIconPosType.Arrow)
			{
				if (!(highlightTrans == null))
				{
					base.view.transform.FindChild("Arrow").gameObject.SetActive(true);
					base.view.transform.FindChild("Arrow").position = highlightTrans.GetComponent<RectTransform>().position;
				}
				return;
			}
			if (handIconPosType == HandIconPosType.None || highlightTrans == null)
			{
				transform.gameObject.SetActive(false);
				return;
			}
			RectTransform component = highlightTrans.GetComponent<RectTransform>();
			Vector3[] array = new Vector3[4];
			component.GetWorldCorners(array);
			foreach (Transform item in transform)
			{
				if (item.gameObject.name == handIconPosType.ToString())
				{
					item.gameObject.SetActive(true);
				}
				else
				{
					item.gameObject.SetActive(false);
				}
			}
			Transform transform3 = base.view.transform.FindChild("HandIcon/" + handIconPosType);
			if (!(transform3 == null))
			{
				RectTransform component2 = transform3.GetComponent<RectTransform>();
				Vector3[] array2 = new Vector3[4];
				component2.GetWorldCorners(array2);
				float num = 0f;
				float num2 = 0f;
				if (Singleton<LevelManager>.Instance != null)
				{
					float num3 = 0.5f;
					num = Math.Max(Math.Max(Math.Max(Math.Abs(array2[1].x - array2[0].x), Math.Abs(array2[2].x - array2[1].x)), Math.Abs(array2[3].x - array2[2].x)), Math.Abs(array2[0].x - array2[3].x)) + num3;
					num2 = Math.Max(Math.Max(Math.Max(Math.Abs(array2[1].y - array2[0].y), Math.Abs(array2[2].y - array2[1].y)), Math.Abs(array2[3].y - array2[2].y)), Math.Abs(array2[0].y - array2[3].y)) + num3;
				}
				else
				{
					num = (num2 = 0f);
				}
				if (handIconPosType == HandIconPosType.Left)
				{
					transform3.position = new Vector3(array[0].x - num / 2f, (array[0].y + array[1].y) / 2f, transform3.position.z);
				}
				else if (handIconPosType == HandIconPosType.Right)
				{
					transform3.position = new Vector3(array[2].x + num / 2f, (array[0].y + array[1].y) / 2f, transform3.position.z);
				}
				else if (handIconPosType == HandIconPosType.Up)
				{
					transform3.position = new Vector3((array[0].x + array[3].x) / 2f, array[1].y + num2 / 2f, transform3.position.z);
				}
				else if (handIconPosType == HandIconPosType.Bottom)
				{
					transform3.position = new Vector3((array[0].x + array[3].x) / 2f, array[0].y - num2 / 2f, transform3.position.z);
				}
				StopHandIconAnim();
				if (handIconPosType != HandIconPosType.None && Singleton<ApplicationManager>.Instance != null)
				{
					_handIconAnimCoroution = Singleton<ApplicationManager>.Instance.StartCoroutine(PlayHandIconAnim(component2));
				}
			}
		}

		private IEnumerator PlayHandIconAnim(RectTransform handIconRectTransform)
		{
			float timer = 0f;
			bool isReversed = false;
			float lerpStart = 0f;
			float lerpEnd = 0f;
			Vector2 originPos = new Vector2(handIconRectTransform.anchoredPosition.x, handIconRectTransform.anchoredPosition.y);
			while (true)
			{
				if (!playHighlightAnim)
				{
					yield return null;
				}
				timer += Time.deltaTime;
				float delta = ((handIconPosType != HandIconPosType.Right && handIconPosType != HandIconPosType.Up) ? (-30f) : 30f);
				if (handIconPosType == HandIconPosType.Left || handIconPosType == HandIconPosType.Right)
				{
					lerpStart = ((!isReversed) ? (originPos.x + delta) : originPos.x);
					lerpEnd = ((!isReversed) ? originPos.x : (originPos.x + delta));
				}
				else
				{
					lerpStart = ((!isReversed) ? (originPos.y + delta) : originPos.y);
					lerpEnd = ((!isReversed) ? originPos.y : (originPos.y + delta));
				}
				float progress_normalized = timer / 0.15f;
				if (progress_normalized >= 1f)
				{
					isReversed = !isReversed;
					timer = 0f;
					yield return null;
				}
				float position_normalized = 2f * progress_normalized - progress_normalized * progress_normalized;
				if (handIconPosType == HandIconPosType.Left || handIconPosType == HandIconPosType.Right)
				{
					float x = Mathf.Lerp(lerpStart, lerpEnd, Mathf.Clamp01(position_normalized));
					float y = originPos.y;
					handIconRectTransform.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
				}
				else
				{
					float x2 = originPos.x;
					float y2 = Mathf.Lerp(lerpStart, lerpEnd, Mathf.Clamp01(position_normalized));
					handIconRectTransform.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x2, y2);
				}
				yield return null;
			}
		}

		private void StopHandIconAnim()
		{
			if (_handIconAnimCoroution != null && Singleton<ApplicationManager>.Instance != null)
			{
				Singleton<ApplicationManager>.Instance.StopCoroutine(_handIconAnimCoroution);
				_handIconAnimCoroution = null;
			}
		}

		private void StopDelayShowCoroution()
		{
			if (_delayShowCoroution != null && Singleton<ApplicationManager>.Instance != null)
			{
				Singleton<ApplicationManager>.Instance.StopCoroutine(_delayShowCoroution);
				_delayShowCoroution = null;
			}
		}

		private void StopHighlightEffectCoroutine()
		{
			if (_highlightEffectCoroution != null && Singleton<ApplicationManager>.Instance != null)
			{
				Singleton<ApplicationManager>.Instance.StopCoroutine(_highlightEffectCoroution);
				_highlightEffectCoroution = null;
			}
		}

		private void StopDelayInputCoroution()
		{
			if (_delayInputCoroutine != null && Singleton<ApplicationManager>.Instance != null)
			{
				Singleton<ApplicationManager>.Instance.StopCoroutine(_delayInputCoroutine);
				_delayInputCoroutine = null;
			}
		}

		private void StopDestroyCoroution()
		{
			if (_destroyCoroutine != null && Singleton<ApplicationManager>.Instance != null)
			{
				Singleton<ApplicationManager>.Instance.StopCoroutine(_destroyCoroutine);
				_destroyCoroutine = null;
			}
		}

		private void HighlightView()
		{
			if (highlightPath != string.Empty)
			{
				BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
				if (sceneCanvas == null)
				{
					Destroy();
					return;
				}
				highlightTrans = sceneCanvas.transform.FindChild(highlightPath);
				if (highlightTrans == null || highlightTrans.gameObject == null || !highlightTrans.gameObject.activeInHierarchy)
				{
					Destroy();
					return;
				}
			}
			if (!(base.view == null) && !(base.view.transform == null) && !(highlightTrans == null) && !(highlightTrans.gameObject == null) && highlightTrans.gameObject.activeInHierarchy && (_newbieHightlightInfo == null || !(_newbieHightlightInfo.originTrans == highlightTrans)))
			{
				if (!disableHighlightEffect && Singleton<ApplicationManager>.Instance != null)
				{
					_highlightEffectCoroution = Singleton<ApplicationManager>.Instance.StartCoroutine(PlayHighlightEffect());
				}
				if (!bShowReward)
				{
					_newbieHightlightInfo = new NewbieHighlightInfo(this, highlightTrans, base.view.transform, disableHighlightInvoke, OnNewbiePanelPreCallback, OnHighlightPointerDown, OnHighlightPointerUp);
				}
			}
		}

		private IEnumerator PlayHighlightEffect()
		{
			Transform effectTrans = base.view.transform.Find("Tutorial_Glow");
			if (highlightTrans == null || effectTrans == null)
			{
				yield break;
			}
			_isHighlightEffectStarted = true;
			float timer = 0f;
			RectTransform highlightRectTransform = highlightTrans.GetComponent<RectTransform>();
			Vector3[] hightlighWorldCorners = new Vector3[4];
			highlightRectTransform.GetWorldCorners(hightlighWorldCorners);
			float sizeOffset = 1f;
			float highlightWorldWidth = Math.Max(Math.Max(Math.Max(Math.Abs(hightlighWorldCorners[1].x - hightlighWorldCorners[0].x), Math.Abs(hightlighWorldCorners[2].x - hightlighWorldCorners[1].x)), Math.Abs(hightlighWorldCorners[3].x - hightlighWorldCorners[2].x)), Math.Abs(hightlighWorldCorners[0].x - hightlighWorldCorners[3].x)) + sizeOffset;
			float highlightWorldHeight = Math.Max(Math.Max(Math.Max(Math.Abs(hightlighWorldCorners[1].y - hightlighWorldCorners[0].y), Math.Abs(hightlighWorldCorners[2].y - hightlighWorldCorners[1].y)), Math.Abs(hightlighWorldCorners[3].y - hightlighWorldCorners[2].y)), Math.Abs(hightlighWorldCorners[0].y - hightlighWorldCorners[3].y)) + sizeOffset;
			Vector3 center = Vector3.zero;
			Vector3[] array = hightlighWorldCorners;
			foreach (Vector3 corner in array)
			{
				center += corner;
			}
			center /= 4f;
			Vector3 startPos = center + new Vector3(Mathf.Cos(timer * _hightlightEffectAngleSpeed * ((float)Math.PI / 180f)) * highlightWorldWidth / 2.5f, Mathf.Sin(timer * _hightlightEffectAngleSpeed * ((float)Math.PI / 180f)) * highlightWorldHeight / 2.5f, highlightTrans.position.z);
			effectTrans.position = startPos;
			SetHighlightEffectActive(playHighlightAnim);
			while (true)
			{
				timer += Time.deltaTime;
				effectTrans.position = center + new Vector3(Mathf.Cos(timer * _hightlightEffectAngleSpeed * ((float)Math.PI / 180f)) * highlightWorldWidth / 2.5f, Mathf.Sin(timer * _hightlightEffectAngleSpeed * ((float)Math.PI / 180f)) * highlightWorldHeight / 2.5f, highlightTrans.position.z);
				effectTrans.SetLocalPositionZ(0f);
				yield return null;
			}
		}

		private Transform SetHighlightEffectActive(bool isActive)
		{
			if (highlightTrans != null && base.view != null && base.view.transform != null)
			{
				Transform transform = base.view.transform.Find("Tutorial_Glow");
				if (transform != null && transform.gameObject != null && (!isActive || _isHighlightEffectStarted))
				{
					transform.gameObject.SetActive(isActive);
				}
				return transform;
			}
			return null;
		}

		private void RecoverHighlightView()
		{
			if (_newbieHightlightInfo == null || highlightTrans == null)
			{
				_newbieHightlightInfo = null;
				return;
			}
			_newbieHightlightInfo.Recover();
			_newbieHightlightInfo = null;
		}

		private void OnNewbiePanelPreCallback(BaseEventData data = null)
		{
			_hasClicked = true;
			if (!destroyByOthers)
			{
				_waitDestroy = true;
			}
			if (preCallback != null)
			{
				preCallback();
			}
		}

		private void OnHighlightPointerDown(BaseEventData data = null)
		{
			if (pointerDownCallback != null)
			{
				pointerDownCallback();
			}
		}

		private void OnHighlightPointerUp(BaseEventData data = null)
		{
			bool flag = false;
			if (pointerUpCallback != null)
			{
				flag = pointerUpCallback();
			}
			if (!destroyByOthers || (destroyByOthers && flag))
			{
				Destroy();
			}
		}

		private void OnNewbieMaskPreCallback(BaseEventData data = null)
		{
			_hasClicked = true;
			if (!destroyByOthers)
			{
				_waitDestroy = true;
			}
			if (preCallback != null)
			{
				preCallback();
			}
		}

		private void OnNewbieMaskClick(BaseEventData data = null)
		{
			bool flag = false;
			if (pointerUpCallback != null)
			{
				flag = pointerUpCallback();
			}
			if (!destroyByOthers || (destroyByOthers && flag))
			{
				Destroy();
			}
		}
	}
}
