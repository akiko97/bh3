using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class IslandCollectGotDialogContext : BaseDialogContext
	{
		private int _scoinNum;

		private float _burstRate;

		private List<DropItem> _materials;

		private int _max_materials_num = 5;

		public IslandCollectGotDialogContext(int scoinNum, float burstRate, List<DropItem> dropItems)
		{
			config = new ContextPattern
			{
				contextName = "IslandCollectGotDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/IslandCollectGotDialog",
				cacheType = ViewCacheType.DontCache
			};
			_scoinNum = scoinNum;
			_burstRate = burstRate;
			_materials = dropItems;
		}

		protected override bool SetupView()
		{
			string text = MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Scoin")) + " + " + _scoinNum;
			if (_burstRate > 1f)
			{
				string text2 = text;
				text = text2 + " ， " + MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Desc_Critical")) + " × " + string.Format("{0:0%}", _burstRate);
			}
			base.view.transform.Find("Dialog/Content/TextScoin/line/Desc").GetComponent<Text>().text = text;
			base.view.transform.Find("Dialog/Content/MaterialList").gameObject.SetActive(_materials.Count > 0);
			if (_materials.Count < 1)
			{
				return false;
			}
			if (_materials.Count > _max_materials_num)
			{
			}
			if (Singleton<IslandModule>.Instance.IsDropMaterials() || _materials.Count > 0)
			{
			}
			for (int i = 0; i < _max_materials_num; i++)
			{
				Transform transform = base.view.transform.Find(string.Format("Dialog/Content/MaterialList/{0}", (i + 1).ToString()));
				if (i < _materials.Count)
				{
					transform.gameObject.SetActive(true);
					int item_id = (int)_materials[i].item_id;
					int level = (int)_materials[i].level;
					StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(item_id, level);
					if (dummyStorageDataItem != null)
					{
						transform.Find("ItemIcon/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(dummyStorageDataItem.GetIconPath());
						transform.Find("Star").GetComponent<MonoItemIconStar>().SetupView(dummyStorageDataItem.rarity, dummyStorageDataItem.rarity);
						transform.Find("Text").GetComponent<Text>().text = string.Format("x{0}", _materials[i].num);
					}
				}
				else
				{
					transform.gameObject.SetActive(false);
				}
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/OKBtn/Btn").GetComponent<Button>(), Close);
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Close();
		}

		private void Close()
		{
			Destroy();
		}
	}
}
