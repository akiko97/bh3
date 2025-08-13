using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class DropLinkDialogContext : BaseDialogContext
	{
		public readonly MaterialDataItem dropItem;

		private List<LevelDataItem> _dropLevelDataList;

		private Action<LevelDataItem> _customDropLinkCallBack;

		public DropLinkDialogContext(MaterialDataItem dropItem, Action<LevelDataItem> onDropLinkClick = null)
		{
			config = new ContextPattern
			{
				contextName = "DropLinkDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/ItemDropLinkDialog"
			};
			this.dropItem = dropItem;
			SetupLevelList(onDropLinkClick);
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Dialog/Content/Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(dropItem.GetImagePath());
			base.view.transform.Find("Dialog/Content/Star/EquipStar").GetComponent<MonoEquipStar>().SetupView(dropItem.rarity);
			base.view.transform.Find("Dialog/Content/NameText").GetComponent<Text>().text = dropItem.GetDisplayTitle();
			base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = dropItem.GetDescription();
			SetupDropLinks();
			return false;
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Close();
		}

		public void Close()
		{
			Destroy();
		}

		private void SetupDropLinks()
		{
			Transform transform = base.view.transform.Find("Dialog/Content/DropLinks/Content");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				LevelDataItem levelData = ((i < _dropLevelDataList.Count) ? _dropLevelDataList[i] : null);
				child.GetComponent<MonoDropLink>().SetupView(levelData, _customDropLinkCallBack);
			}
		}

		private void SetupLevelList(Action<LevelDataItem> onDropLinkClick)
		{
			_dropLevelDataList = new List<LevelDataItem>();
			_customDropLinkCallBack = onDropLinkClick;
			List<int> dropList = dropItem.GetDropList();
			List<uint> list = new List<uint>();
			foreach (int item in dropList)
			{
				LevelDataItem levelDataItem = Singleton<LevelModule>.Instance.TryGetLevelById(item);
				if (levelDataItem == null)
				{
					levelDataItem = new LevelDataItem(item);
				}
				_dropLevelDataList.Add(levelDataItem);
				if (!levelDataItem.dropDisplayInfoReceived)
				{
					list.Add((uint)levelDataItem.levelId);
				}
			}
			_dropLevelDataList.Sort((LevelDataItem left, LevelDataItem right) => left.levelId - right.levelId);
			if (list.Count > 0)
			{
				Singleton<NetworkManager>.Instance.RequestLevelDropList(list);
			}
		}
	}
}
