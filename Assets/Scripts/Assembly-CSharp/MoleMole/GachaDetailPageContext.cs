using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class GachaDetailPageContext : BasePageContext
	{
		private const string ITEM_ICON_BUTTON_PREFAB_PATH = "UI/Menus/Widget/Map/DropItemButton";

		private GachaDisplayCommonData _displayData;

		private int _gachaType;

		private List<StorageDataItemBase> upAvatarDataList;

		private List<StorageDataItemBase> upWeaponDataList;

		private List<StorageDataItemBase> upStigmataDataList;

		public GachaDetailPageContext(GachaDisplayCommonData displayData, int gachaType)
		{
			config = new ContextPattern
			{
				contextName = "GachaDetailPageContext",
				viewPrefabPath = "UI/Menus/Page/Gacha/GachaDetailInfoPage",
				cacheType = ViewCacheType.AlwaysCached
			};
			_displayData = displayData;
			_gachaType = gachaType;
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Content/ScrollView/Content/Title/Intro").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(_displayData.content);
			if (string.IsNullOrEmpty(_displayData.title_image))
			{
				base.view.transform.Find("Content/ScrollView/Content/Title/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.GachaTypeTitleFigures[_gachaType]);
			}
			else
			{
				UIUtil.TrySetupEventSprite(base.view.transform.Find("Content/ScrollView/Content/Title/Image").GetComponent<Image>(), _displayData.title_image);
			}
			base.view.transform.Find("Content/ScrollView/Content/Title/Time").GetComponent<Text>().text = _displayData.title;
			SetupUpAvatarPanel();
			SetupUpWeaponPanel();
			SetupUpStigmataPanel();
			base.view.transform.Find("Content/ScrollView/Content/RulePanel").gameObject.SetActive(_displayData.ruleSpecified);
			if (_displayData.ruleSpecified)
			{
				base.view.transform.Find("Content/ScrollView/Content/RulePanel/TextContent/Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(_displayData.rule);
			}
			base.view.transform.Find("Content/ScrollView/Content/ContentDetailPanel").gameObject.SetActive(_displayData.content_detailSpecified);
			if (_displayData.content_detailSpecified)
			{
				base.view.transform.Find("Content/ScrollView/Content/ContentDetailPanel/TextContent/Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(_displayData.content_detail);
			}
			base.view.transform.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn");
			return false;
		}

		private void SetupUpAvatarPanel()
		{
			Transform transform = base.view.transform.Find("Content/ScrollView/Content/UpAvatarPanel");
			transform.gameObject.SetActive(_displayData.up_avatar_list.Count > 0);
			if (_displayData.up_avatar_list.Count <= 0)
			{
				return;
			}
			upAvatarDataList = new List<StorageDataItemBase>();
			foreach (uint item2 in _displayData.up_avatar_list)
			{
				AvatarCardMetaData avatarCardMetaDataByKey = AvatarCardMetaDataReader.GetAvatarCardMetaDataByKey(AvatarMetaDataReaderExtend.GetAvatarIDsByKey((int)item2).avatarCardID);
				AvatarCardDataItem item = new AvatarCardDataItem(avatarCardMetaDataByKey);
				upAvatarDataList.Add(item);
			}
			SetupUpContent(transform.Find("AvatarPanel"), transform.Find("AvatarNamePanel/Text").GetComponent<Text>(), upAvatarDataList);
		}

		private void SetupUpWeaponPanel()
		{
			Transform transform = base.view.transform.Find("Content/ScrollView/Content/UpWeaponPanel");
			transform.gameObject.SetActive(_displayData.up_weapon_list.Count > 0);
			if (_displayData.up_weapon_list.Count <= 0)
			{
				return;
			}
			upWeaponDataList = new List<StorageDataItemBase>();
			foreach (WeaponDetailData item in _displayData.up_weapon_list)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)item.id, (int)item.level);
				upWeaponDataList.Add(dummyStorageDataItem);
			}
			SetupUpContent(transform.Find("WeaponPanel"), transform.Find("WeaponNamePanel/Text").GetComponent<Text>(), upWeaponDataList);
		}

		private void SetupUpStigmataPanel()
		{
			Transform transform = base.view.transform.Find("Content/ScrollView/Content/UpStigmataPanel");
			transform.gameObject.SetActive(_displayData.up_stigmata_list.Count > 0);
			if (_displayData.up_stigmata_list.Count <= 0)
			{
				return;
			}
			upStigmataDataList = new List<StorageDataItemBase>();
			foreach (StigmataDetailData item in _displayData.up_stigmata_list)
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)item.id, (int)item.level);
				upStigmataDataList.Add(dummyStorageDataItem);
			}
			SetupUpContent(transform.Find("StigmataPanel"), transform.Find("StigmataNamePanel/Text").GetComponent<Text>(), upStigmataDataList);
		}

		private void SetupUpContent(Transform gridTrans, Text nameText, List<StorageDataItemBase> itemList)
		{
			string text = string.Empty;
			gridTrans.DestroyChildren();
			foreach (StorageDataItemBase item in itemList)
			{
				if (item is AvatarCardDataItem)
				{
					text += Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(item.ID).avatarID).FullName;
					text += Environment.NewLine;
				}
				else if (item is WeaponDataItem || item is StigmataDataItem)
				{
					text += item.GetDisplayTitle();
					text += Environment.NewLine;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("UI/Menus/Widget/Map/DropItemButton"));
				if (!(gameObject == null))
				{
					gameObject.transform.SetParent(gridTrans, false);
					gameObject.GetComponent<MonoLevelDropIconButton>().SetupView(item, OnItemClick, true);
					gameObject.GetComponent<CanvasGroup>().alpha = 1f;
				}
			}
			nameText.text = text;
		}

		private void OnItemClick(StorageDataItemBase itemData)
		{
			UIUtil.ShowItemDetail(itemData, true);
		}
	}
}
