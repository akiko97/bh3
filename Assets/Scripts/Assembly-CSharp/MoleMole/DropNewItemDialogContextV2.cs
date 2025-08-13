using System.Collections.Generic;
using MoleMole.Config;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class DropNewItemDialogContextV2 : BaseSequenceDialogContext
	{
		private const string DROP_ITEM_PREFAB_PATH = "UI/Menus/Widget/Storage/DropItem";

		private const string DROP_AVATAR_CARD_PREFAB_PATH = "UI/Menus/Widget/Storage/DropAvatarCard";

		private List<float> _delayTimerSpanList = new List<float> { 1.2f, 1.4f, 2f };

		private List<CanvasTimer> _delayTimerList = new List<CanvasTimer>();

		private CanvasTimer _firstDelayTimer;

		private CanvasTimer _secondDelayTimer;

		private CanvasTimer _thirdDelayTimer;

		private List<Tuple<StorageDataItemBase, bool>> _storageItemList;

		private SequenceAnimationManager _animationManager;

		public DropNewItemDialogContextV2(List<Tuple<StorageDataItemBase, bool>> itemDataList)
		{
			config = new ContextPattern
			{
				contextName = "DropNewItemDialogContextV2",
				viewPrefabPath = "UI/Menus/Dialog/NewDropItemGotDialogV2",
				cacheType = ViewCacheType.DontCache
			};
			_storageItemList = itemDataList;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.AnimCallBack)
			{
				return OnAnimationCallBack(ntf.body.ToString());
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG/Button").GetComponent<Button>(), Destroy);
		}

		protected override bool SetupView()
		{
			_animationManager = new SequenceAnimationManager();
			Transform transform = base.view.transform.Find("ItemPanel");
			transform.DestroyChildren();
			for (int i = 0; i < _storageItemList.Count; i++)
			{
				StorageDataItemBase item = _storageItemList[i].Item1;
				string path = "UI/Menus/Widget/Storage/DropItem";
				if (item is AvatarCardDataItem)
				{
					path = "UI/Menus/Widget/Storage/DropAvatarCard";
				}
				Transform dropItemTrans = Object.Instantiate(Miscs.LoadResource<GameObject>(path)).transform;
				dropItemTrans.name = (i + 1).ToString();
				dropItemTrans.SetParent(transform, false);
				dropItemTrans.GetComponent<MonoDropNewItemShow>().SetupView(item);
				if (!(item is AvatarFragmentDataItem))
				{
					CanvasTimer canvasTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(_delayTimerSpanList[i], 0f);
					canvasTimer.timeUpCallback = delegate
					{
						ShowItemStar(dropItemTrans);
					};
					canvasTimer.StartRun();
				}
			}
			return false;
		}

		public override void Destroy()
		{
			foreach (CanvasTimer delayTimer in _delayTimerList)
			{
				if (delayTimer != null)
				{
					delayTimer.Destroy();
				}
			}
			_animationManager.ClearAnimations();
			base.Destroy();
		}

		private void ShowItemStar(Transform itemTrans)
		{
			if (!(itemTrans == null) && !(itemTrans.Find("Item/Stars") == null))
			{
				Transform trans = itemTrans.Find("Item/Stars");
				if (_animationManager.IsPlaying)
				{
					_animationManager.AddAllChildrenInTransform(trans);
					return;
				}
				_animationManager.ClearAnimations();
				_animationManager.AddAllChildrenInTransform(trans);
				_animationManager.StartPlay();
			}
		}

		private bool OnAnimationCallBack(string msg)
		{
			if (msg == "ShowStigmata")
			{
				Transform transform = base.view.transform.Find("ItemPanel");
				foreach (Transform item in transform)
				{
					item.GetComponent<MonoDropNewItemShow>().ResetRectMaskForStigmata();
				}
			}
			return false;
		}
	}
}
