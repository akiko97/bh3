using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class GachaResultPageContext : BasePageContext
	{
		private const string DROP_ITEM_ANIMATION_NAME = "DropItemScale10";

		private List<StorageDataItemBase> _itemList;

		private GachaType _type;

		private int _cost;

		private GachaDisplayInfo _displayInfo;

		private HashSet<int> _rareItemList;

		private SequenceAnimationManager _animationManager;

		private List<MonoLevelDropIconButton> _dropList = new List<MonoLevelDropIconButton>();

		public GachaResultPageContext(GachaDisplayInfo displayInfo, GachaType type, List<StorageDataItemBase> itemList, List<GachaItem> gachaItemList, int cost)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			config = new ContextPattern
			{
				contextName = "GachaResultPageContext",
				viewPrefabPath = "UI/Menus/Page/Gacha/GachaResultPage",
				cacheType = ViewCacheType.AlwaysCached
			};
			_type = type;
			_itemList = itemList;
			_rareItemList = new HashSet<int>();
			foreach (GachaItem gachaItem in gachaItemList)
			{
				if (gachaItem.is_rare_dropSpecified && gachaItem.is_rare_drop)
				{
					_rareItemList.Add((int)gachaItem.item_id);
				}
			}
			_cost = cost;
			_displayInfo = displayInfo;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 63)
			{
				return OnGetGachaDisplayRsp(pkt.getData<GetGachaDisplayRsp>());
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BottomPanel/OKBtn").GetComponent<Button>(), OnOkBtnClick);
		}

		protected override bool SetupView()
		{
			Singleton<MainUIManager>.Instance.LockUI(true);
			base.view.transform.Find("BlockPanel").gameObject.SetActive(true);
			MergeAllMaterialAndSort();
			UpdateView();
			return false;
		}

		private void OnOkBtnClick()
		{
			Singleton<MainUIManager>.Instance.BackPage();
		}

		private bool OnGetGachaDisplayRsp(GetGachaDisplayRsp rsp)
		{
			UpdateView();
			return false;
		}

		private void OnScrollChange(Transform trans, int index)
		{
			StorageDataItemBase itemData = _itemList[index];
			trans.GetComponent<MonoLevelDropIconButton>().SetupView(itemData, OnDropItemBtnClick, true, true);
			trans.GetComponent<MonoAnimationinSequence>().animationName = "DropItemScale10";
		}

		private void OnDropItemBtnClick(StorageDataItemBase itemData)
		{
			UIUtil.ShowItemDetail(itemData, true);
		}

		private void OnAllAnimationEnd()
		{
			try
			{
				float num = 0.1f;
				if (_dropList.Count > 0)
				{
					num += 1f + 0.2f * (float)(_dropList.Count - 1);
				}
				Singleton<ApplicationManager>.Instance.StartCoroutine(OnToFragmentOver(num));
			}
			catch
			{
				base.view.transform.Find("BlockPanel").gameObject.SetActive(false);
			}
			foreach (Transform item in base.view.transform.Find("Drops/ScrollView/Content"))
			{
				item.SetLocalScaleX(1f);
				item.SetLocalScaleY(1f);
				item.GetComponent<CanvasGroup>().alpha = 1f;
			}
			PlayVFXSequence();
			Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
		}

		private void UpdateView()
		{
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Expected I4, but got Unknown
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Invalid comparison between Unknown and I4
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Expected I4, but got Unknown
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Invalid comparison between Unknown and I4
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Expected I4, but got Unknown
			base.view.transform.Find("BlockPanel").gameObject.SetActive(true);
			base.view.transform.Find("Drops/ScrollView").GetComponent<MonoGridScroller>().Init(OnScrollChange, _itemList.Count, new Vector2(0f, 1f));
			_animationManager = new SequenceAnimationManager(OnAllAnimationEnd);
			_animationManager.AddAllChildrenInTransform(base.view.transform.Find("Drops/ScrollView/Content"));
			InitDropItems();
			GachaType type = _type;
			switch ((int)type - 1)
			{
			case 1:
			case 2:
			{
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)_displayInfo.hcoinGachaData.ticket_material_id);
				if ((int)_type == 3)
				{
					dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)_displayInfo.specialGachaData.ticket_material_id);
				}
				base.view.transform.Find("BottomPanel/Cost/Cost/Layout/Name").GetComponent<Text>().text = dummyStorageDataItem.GetDisplayTitle();
				base.view.transform.Find("BottomPanel/Cost/Cost/Layout/Num").GetComponent<Text>().text = _cost.ToString();
				base.view.transform.Find("BottomPanel/Cost/Left/Layout/Name").GetComponent<Text>().text = dummyStorageDataItem.GetDisplayTitle();
				int num = 0;
				StorageDataItemBase storageDataItemBase = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(dummyStorageDataItem.ID);
				if (storageDataItemBase != null)
				{
					num = storageDataItemBase.number;
				}
				base.view.transform.Find("BottomPanel/Cost/Left/Layout/Num").GetComponent<Text>().text = num.ToString();
				HcoinGachaData val = (((int)_type != 2) ? _displayInfo.specialGachaData : _displayInfo.hcoinGachaData);
				if (string.IsNullOrEmpty(val.common_data.title_image))
				{
					base.view.transform.Find("BottomPanel/Title").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.GachaTypeTitleFigures[(int)_type]);
				}
				else
				{
					UIUtil.TrySetupEventSprite(base.view.transform.Find("BottomPanel/Title").GetComponent<Image>(), val.common_data.title_image);
				}
				break;
			}
			case 0:
			{
				base.view.transform.Find("BottomPanel/Cost/Cost/Layout/Name").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("10119");
				base.view.transform.Find("BottomPanel/Cost/Cost/Layout/Num").GetComponent<Text>().text = _cost.ToString();
				base.view.transform.Find("BottomPanel/Cost/Left/Layout/Name").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("10119");
				base.view.transform.Find("BottomPanel/Cost/Left/Layout/Num").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.friendsPoint.ToString();
				FriendsPointGachaData friendPointGachaData = _displayInfo.friendPointGachaData;
				if (string.IsNullOrEmpty(friendPointGachaData.common_data.title_image))
				{
					base.view.transform.Find("BottomPanel/Title").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.GachaTypeTitleFigures[(int)_type]);
				}
				else
				{
					UIUtil.TrySetupEventSprite(base.view.transform.Find("BottomPanel/Title").GetComponent<Image>(), friendPointGachaData.common_data.title_image);
				}
				break;
			}
			}
			base.view.transform.Find("Drops/ScrollView").GetComponent<RectMask>().SetGraphicDirty();
			_animationManager.StartPlay(0.1f);
		}

		private void InitDropItems()
		{
			_dropList.Clear();
			Transform transform = base.view.transform.Find("Drops/ScrollView/Content");
			foreach (Transform item in transform)
			{
				MonoLevelDropIconButton component = item.GetComponent<MonoLevelDropIconButton>();
				component.StopRareEffect();
				if (component != null && (_rareItemList.Contains(component.GetDropItemID()) || component.IsAvatarCard()))
				{
					_dropList.Add(component);
				}
			}
		}

		private void PlayVFXSequence()
		{
			float num = 0.4f;
			for (int i = 0; i < _dropList.Count; i++)
			{
				MonoLevelDropIconButton monoLevelDropIconButton = _dropList[i];
				if (monoLevelDropIconButton != null && monoLevelDropIconButton.gameObject != null)
				{
					monoLevelDropIconButton.PlayVFX(num, _rareItemList.Contains(monoLevelDropIconButton.GetDropItemID()));
					num += 0.2f;
				}
			}
		}

		private IEnumerator OnToFragmentOver(float delay)
		{
			yield return new WaitForSeconds(delay);
			if ((bool)base.view)
			{
				base.view.transform.Find("BlockPanel").gameObject.SetActive(false);
			}
		}

		private void MergeAllMaterialAndSort()
		{
			List<StorageDataItemBase> list = new List<StorageDataItemBase>();
			foreach (StorageDataItemBase item in _itemList)
			{
				if (item is MaterialDataItem)
				{
					bool flag = false;
					foreach (StorageDataItemBase item2 in list)
					{
						if (item2.ID == item.ID)
						{
							item2.number += item.number;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(item);
					}
				}
				else
				{
					list.Add(item);
				}
			}
			_itemList = list;
			_itemList.Sort(GachaGotItemComparor);
		}

		private int GachaGotItemComparor(StorageDataItemBase left, StorageDataItemBase right)
		{
			ItemType itemTypePriority = GetItemTypePriority(left);
			ItemType itemTypePriority2 = GetItemTypePriority(right);
			if (itemTypePriority != itemTypePriority2)
			{
				return itemTypePriority - itemTypePriority2;
			}
			switch (itemTypePriority)
			{
			case ItemType.AvatarCard:
			{
				int unlockStar3 = AvatarMetaDataReader.GetAvatarMetaDataByKey(AvatarMetaDataReaderExtend.GetAvatarIDsByKey((left as AvatarCardDataItem).ID).avatarID).unlockStar;
				int unlockStar4 = AvatarMetaDataReader.GetAvatarMetaDataByKey(AvatarMetaDataReaderExtend.GetAvatarIDsByKey((right as AvatarCardDataItem).ID).avatarID).unlockStar;
				if (unlockStar3 == unlockStar4)
				{
					return left.ID - right.ID;
				}
				return unlockStar4 - unlockStar3;
			}
			case ItemType.AvatarFragment:
			{
				int unlockStar = AvatarMetaDataReader.GetAvatarMetaDataByKey(AvatarMetaDataReaderExtend.GetAvatarIDsByKey((left as AvatarFragmentDataItem).ID).avatarID).unlockStar;
				int unlockStar2 = AvatarMetaDataReader.GetAvatarMetaDataByKey(AvatarMetaDataReaderExtend.GetAvatarIDsByKey((right as AvatarFragmentDataItem).ID).avatarID).unlockStar;
				if (unlockStar == unlockStar2)
				{
					return left.ID - right.ID;
				}
				return unlockStar2 - unlockStar;
			}
			default:
				if (left.rarity == right.rarity)
				{
					return left.ID - right.ID;
				}
				return right.rarity - left.rarity;
			}
		}

		private ItemType GetItemTypePriority(StorageDataItemBase itemData)
		{
			if (itemData is AvatarCardDataItem)
			{
				return ItemType.AvatarCard;
			}
			if (itemData is AvatarFragmentDataItem)
			{
				return ItemType.AvatarFragment;
			}
			if (itemData is WeaponDataItem)
			{
				return ItemType.Weapon;
			}
			if (itemData is StigmataDataItem)
			{
				return ItemType.Stigmata;
			}
			return ItemType.Material;
		}
	}
}
