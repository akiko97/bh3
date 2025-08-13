using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class DropNewItemDialogContext : BaseSequenceDialogContext
	{
		public float TimerSpan = 2f;

		private StorageDataItemBase _storageItem;

		private SequenceAnimationManager _animationManager;

		private bool _onlyShow;

		private CanvasTimer _timer;

		public DropNewItemDialogContext(StorageDataItemBase itemData, bool useTimer = true, bool onlyShow = false)
		{
			config = new ContextPattern
			{
				contextName = "DropNewItemDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/NewDropItemGotDialog",
				cacheType = ViewCacheType.DontCache
			};
			_storageItem = itemData;
			_onlyShow = onlyShow;
			if (useTimer)
			{
				_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(TimerSpan, 0f);
				_timer.timeUpCallback = DialogEnd;
				_timer.StopRun();
			}
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG/Button").GetComponent<Button>(), DialogEnd);
		}

		protected override bool SetupView()
		{
			_animationManager = new SequenceAnimationManager();
			base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(false);
			base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(false);
			base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(false);
			if (_storageItem is WeaponDataItem)
			{
				base.view.transform.Find("ItemPanel/3dModel").gameObject.SetActive(true);
				base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(_storageItem as WeaponDataItem);
				_animationManager.AddAnimation(base.view.transform.Find("ItemPanel/3dModel").GetComponent<MonoAnimationinSequence>());
			}
			else if (_storageItem is StigmataDataItem)
			{
				base.view.transform.Find("ItemPanel/StigmataIcon").gameObject.SetActive(true);
				base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoStigmataFigure>().SetupView(_storageItem as StigmataDataItem);
				_animationManager.AddAnimation(base.view.transform.Find("ItemPanel/StigmataIcon/Image").GetComponent<MonoAnimationinSequence>());
			}
			else
			{
				string prefabPath = ((!(_storageItem is EndlessToolDataItem)) ? _storageItem.GetImagePath() : (_storageItem as EndlessToolDataItem).GetIconPath());
				base.view.transform.Find("ItemPanel/OtherIcon").gameObject.SetActive(true);
				base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
				_animationManager.AddAnimation(base.view.transform.Find("ItemPanel/OtherIcon/Image").GetComponent<MonoAnimationinSequence>());
			}
			base.view.transform.Find("NewAvatarEffect/Green").gameObject.SetActive(false);
			base.view.transform.Find("NewAvatarEffect/Blue").gameObject.SetActive(false);
			base.view.transform.Find("NewAvatarEffect/Purple").gameObject.SetActive(false);
			base.view.transform.Find("NewAvatarEffect/Orange").gameObject.SetActive(false);
			base.view.transform.Find("NewAvatarEffect/" + MiscData.Config.RarityColor[_storageItem.rarity]).gameObject.SetActive(true);
			if (!_onlyShow)
			{
				base.view.transform.Find("ItemPanel/Title/DescPanel/Desc").GetComponent<Text>().text = _storageItem.GetDisplayTitle();
				_animationManager.AddAnimation(base.view.transform.Find("ItemPanel/Title").GetComponent<MonoAnimationinSequence>());
				Transform transform = base.view.transform.Find("ItemPanel/Stars");
				if (_storageItem is AvatarFragmentDataItem || _storageItem is AvatarCardDataItem)
				{
					transform.gameObject.SetActive(false);
				}
				else
				{
					transform.gameObject.SetActive(true);
					for (int i = 0; i < transform.childCount; i++)
					{
						Transform child = transform.GetChild(i);
						child.gameObject.SetActive(i < _storageItem.rarity);
						if (i < _storageItem.rarity)
						{
							bool flag = _storageItem is AvatarCardDataItem;
							child.Find("1").gameObject.SetActive(!flag);
							child.Find("2").gameObject.SetActive(flag);
						}
					}
					_animationManager.AddAllChildrenInTransform(transform);
				}
			}
			_animationManager.StartPlay(0f, false);
			if (_timer != null && Singleton<TutorialModule>.Instance != null && !Singleton<TutorialModule>.Instance.IsInTutorial)
			{
				_timer.StartRun();
			}
			AvatarCardDataItem avatarCardDataItem = _storageItem as AvatarCardDataItem;
			if (avatarCardDataItem != null && !avatarCardDataItem.IsSplite())
			{
				int avatarID = AvatarMetaDataReaderExtend.GetAvatarIDsByKey(avatarCardDataItem.ID).avatarID;
				AvatarUnlockDialogContext dialogContext = new AvatarUnlockDialogContext(avatarID, true);
				Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
			}
			PostOpenningAudioEvent();
			return false;
		}

		private void DialogEnd()
		{
			Destroy();
		}

		public override void Destroy()
		{
			if (_timer != null)
			{
				_timer.Destroy();
			}
			base.Destroy();
		}

		private void PostOpenningAudioEvent()
		{
			if (_storageItem is StigmataDataItem)
			{
				Singleton<WwiseAudioManager>.Instance.Post("UI_Item_Tattoo_PTL_Display");
			}
			else if (_storageItem is AvatarCardDataItem)
			{
				Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Large");
			}
			else
			{
				Singleton<WwiseAudioManager>.Instance.Post("UI_Upgrade_PTL_Small");
			}
		}
	}
}
