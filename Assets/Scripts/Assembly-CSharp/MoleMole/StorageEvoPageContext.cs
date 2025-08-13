using System;
using System.Collections.Generic;
using System.Linq;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class StorageEvoPageContext : BasePageContext
	{
		public const int MAX_SELECT_NUM = 5;

		public readonly StorageDataItemBase storageItem;

		public readonly StorageDataItemBase targetItem;

		private List<EvoItem> _resourceList;

		private Dictionary<int, EvoItem> _resourceDict;

		private StorageDataItemBase _beforeEvoItemData;

		public AvatarDataItem uiEquipOwner;

		public StorageEvoPageContext(StorageDataItemBase storageItem)
		{
			config = new ContextPattern
			{
				contextName = "StorageItemDetailPageContext",
				viewPrefabPath = "UI/Menus/Page/Storage/WeaponEvoPage"
			};
			if (storageItem is StigmataDataItem)
			{
				config.viewPrefabPath = "UI/Menus/Page/Storage/StigmataEvoPage";
			}
			this.storageItem = storageItem;
			targetItem = storageItem.GetEvoStorageItem();
		}

		public override bool OnNotify(Notify ntf)
		{
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 38)
			{
				return OnEquipmenEvoRsp(pkt.getData<EquipmentEvoRsp>());
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>(), OnOkBtnClick);
		}

		protected override bool SetupView()
		{
			SetupInfoPanels();
			GetEvoResourceList(out _resourceList, out _resourceDict);
			SetupResourceList();
			int coinNeedToUpRarity = storageItem.GetCoinNeedToUpRarity();
			base.view.transform.Find("Scoin/Content/Scoin/Num").GetComponent<Text>().text = coinNeedToUpRarity.ToString();
			bool flag = storageItem.level == storageItem.GetMaxLevel();
			bool flag2 = Singleton<PlayerModule>.Instance.playerData.scoin >= coinNeedToUpRarity;
			bool flag3 = IsAllResoursesEnough();
			bool flag4 = IsAvatarCostEnough();
			bool flag5 = flag && flag2 && flag3 && flag4;
			Text component = base.view.transform.Find("Scoin/Content/Tip/Tips").GetComponent<Text>();
			component.gameObject.SetActive(!flag5);
			if (!flag)
			{
				component.text = LocalizationGeneralLogic.GetText("Menu_Desc_LvNotMax");
			}
			else if (!flag2)
			{
				component.text = LocalizationGeneralLogic.GetText("Menu_Desc_ScoinLack");
			}
			else if (!flag3)
			{
				component.text = LocalizationGeneralLogic.GetText("Menu_Desc_ResourceLack");
			}
			else if (!flag4)
			{
				component.text = LocalizationGeneralLogic.GetText("Menu_CostOver");
			}
			base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>().interactable = flag5;
			return false;
		}

		public void OnOkBtnClick()
		{
			List<StorageDataItemBase> list = new List<StorageDataItemBase>();
			bool flag = false;
			foreach (EvoItem resource in _resourceList)
			{
				list.Add(resource.item);
				if (resource.item.level > 1 || resource.item.exp > 0)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.DoubleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Tips"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_ConsumeExpEquipmentHint"),
					buttonCallBack = delegate(bool confirmed)
					{
						if (confirmed)
						{
							DoRequestEvo(list);
						}
					}
				});
			}
			else
			{
				DoRequestEvo(list);
			}
		}

		private void DoRequestEvo(List<StorageDataItemBase> resourceList)
		{
			_beforeEvoItemData = storageItem.Clone();
			Singleton<NetworkManager>.Instance.RequestEquipmentEvo(resourceList, storageItem);
		}

		private bool OnEquipmenEvoRsp(EquipmentEvoRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Invalid comparison between Unknown and I4
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Invalid comparison between Unknown and I4
			if ((int)rsp.retcode == 0)
			{
				List<StorageDataItemBase> list = new List<StorageDataItemBase>();
				foreach (EvoItem resource in _resourceList)
				{
					list.Add(resource.item);
				}
				EquipmentType type = rsp.new_item.type;
				Type typeFromHandle;
				if ((int)type != 3)
				{
					if ((int)type != 4)
					{
						return false;
					}
					typeFromHandle = typeof(StigmataDataItem);
				}
				else
				{
					typeFromHandle = typeof(WeaponDataItem);
				}
				StorageDataItemBase storageItemByTypeAndID = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(typeFromHandle, (int)rsp.new_item.id_or_unique_id);
				if (storageItemByTypeAndID == null)
				{
					return false;
				}
				Singleton<MainUIManager>.Instance.ShowDialog(new EquipPowerUpEffectDialogContext(_beforeEvoItemData, storageItemByTypeAndID, list, EquipPowerUpEffectDialogContext.DialogType.Evo));
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
					desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)
				});
			}
			return false;
		}

		private void SetupInfoPanels()
		{
			if (storageItem is WeaponDataItem)
			{
				SetupWeapon(base.view.transform.Find("InfoBefore"), storageItem as WeaponDataItem, false);
				SetupWeapon(base.view.transform.Find("InfoAfter"), targetItem as WeaponDataItem, true);
			}
			else if (storageItem is StigmataDataItem)
			{
				SetupStigmatasView(storageItem);
			}
		}

		private void SetupWeapon(Transform trans, WeaponDataItem weapon, bool isEvoWeapon)
		{
			MonoEquipSubStar component = trans.Find("Content/Equipment/Info/Stars/Star/EquipStar").GetComponent<MonoEquipSubStar>();
			component.SetupView(weapon.rarity, weapon.GetMaxRarity());
			MonoEquipSubStar component2 = trans.Find("Content/Equipment/Info/Stars/Star/EquipSubStar").GetComponent<MonoEquipSubStar>();
			component2.SetupView(weapon.GetSubRarity(), weapon.GetMaxSubRarity() - 1);
			string prefabPath = MiscData.Config.PrefabPath.WeaponBaseTypeIcon[weapon.GetBaseType()];
			trans.Find("Title/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
			trans.Find("Title/Name").GetComponent<Text>().text = weapon.GetDisplayTitle();
			trans.Find("Content/Equipment/Info/Cost/Num").GetComponent<Text>().text = weapon.GetCost().ToString();
			if (isEvoWeapon)
			{
				trans.Find("Content/Equipment/Info/BasicStatus").GetComponent<MonoItemAttributeDiff>().SetupView(uiEquipOwner, storageItem, targetItem, ShowAfterAttr);
			}
			else
			{
				trans.Find("Content/Equipment/Info/BasicStatus").GetComponent<MonoItemAttributeDiff>().SetupView(uiEquipOwner, storageItem, targetItem, ShowBeforeAttr);
			}
			trans.Find("Content/Equipment/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(weapon, true);
			trans.Find("Content/Equipment/Lv/CurrentLevelNum").GetComponent<Text>().text = weapon.level.ToString();
			trans.Find("Content/Equipment/Lv/MaxLevelNum").GetComponent<Text>().text = weapon.GetMaxLevel().ToString();
		}

		private void ShowBeforeAttr(Transform trans, float paramBefore, float paramAfter)
		{
			int num = Mathf.FloorToInt(paramBefore);
			int num2 = Mathf.FloorToInt(paramAfter);
			trans.gameObject.SetActive(num != num2);
			if (trans.gameObject.activeSelf)
			{
				trans.Find("UpNum").gameObject.SetActive(true);
				trans.Find("DownNm").gameObject.SetActive(false);
				trans.Find("UpNum/Num").GetComponent<Text>().text = num.ToString();
				trans.Find("UpNum/Diff").gameObject.SetActive(false);
			}
		}

		private void ShowAfterAttr(Transform trans, float paramBefore, float paramAfter)
		{
			int num = Mathf.FloorToInt(paramBefore);
			int num2 = Mathf.FloorToInt(paramAfter);
			trans.gameObject.SetActive(num != num2);
			if (trans.gameObject.activeSelf)
			{
				int num3 = num2 - num;
				Transform transform = ((num3 < 0) ? trans.Find("DownNm") : trans.Find("UpNum"));
				trans.Find("UpNum").gameObject.SetActive(num3 >= 0);
				trans.Find("DownNm").gameObject.SetActive(num3 < 0);
				transform.Find("Num").GetComponent<Text>().text = num2.ToString();
				transform.Find("Diff/DiffNum").GetComponent<Text>().text = num3.ToString();
			}
		}

		private void SetupStigmatasView(StorageDataItemBase stigmata)
		{
			StigmataDataItem stigmataDataItem = stigmata.GetEvoStorageItem() as StigmataDataItem;
			StigmataDataItem stigmataDataItem2 = stigmata as StigmataDataItem;
			int pre_affix_id = ((stigmataDataItem2.PreAffixSkill != null) ? stigmataDataItem2.PreAffixSkill.affixID : 0);
			int suf_affix_id = ((stigmataDataItem2.SufAffixSkill != null) ? stigmataDataItem2.SufAffixSkill.affixID : 0);
			stigmataDataItem.SetAffixSkill(stigmataDataItem2.IsAffixIdentify, pre_affix_id, suf_affix_id);
			base.view.transform.Find("InfoBefore/Attributes/InfoPanel/BasicStatus").GetComponent<MonoItemAttributeDiff>().SetupView(uiEquipOwner, stigmata, stigmataDataItem, ShowBeforeAttr);
			base.view.transform.Find("InfoAfter/Attributes/InfoPanel/BasicStatus").GetComponent<MonoItemAttributeDiff>().SetupView(uiEquipOwner, stigmata, stigmataDataItem, ShowAfterAttr);
			base.view.transform.Find("InfoAfter/Equipment/Title/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(stigmataDataItem.GetSmallIconPath());
			base.view.transform.Find("InfoAfter/Equipment/Title/Name").GetComponent<Text>().text = stigmataDataItem.GetDisplayTitle();
			base.view.transform.Find("InfoAfter/Equipment/Cost/Num").GetComponent<Text>().text = stigmataDataItem.GetCost().ToString();
			MonoEquipSubStar component = base.view.transform.Find("InfoAfter/Equipment/Star/EquipStar").GetComponent<MonoEquipSubStar>();
			component.SetupView(stigmataDataItem.rarity, stigmataDataItem.GetMaxRarity());
			MonoEquipSubStar component2 = base.view.transform.Find("InfoAfter/Equipment/Star/EquipSubStar").GetComponent<MonoEquipSubStar>();
			component2.SetupView(stigmataDataItem.GetSubRarity(), stigmataDataItem.GetMaxSubRarity() - 1);
			base.view.transform.Find("InfoAfter/Figure").GetComponent<MonoStigmataFigure>().SetupView(stigmataDataItem);
			base.view.transform.Find("InfoAfter/Equipment/Lv/CurrentLevelNum").GetComponent<Text>().text = stigmataDataItem.level.ToString();
			base.view.transform.Find("InfoAfter/Equipment/Lv/MaxLevelNum").GetComponent<Text>().text = stigmataDataItem.GetMaxLevel().ToString();
		}

		private void SetupResourceList()
		{
			for (int i = 1; i <= 5; i++)
			{
				MonoItemIconButton component = base.view.transform.Find("ResourceList/Content/" + i).GetComponent<MonoItemIconButton>();
				if (i <= _resourceList.Count)
				{
					component.gameObject.SetActive(true);
					EvoItem evoItem = _resourceList[i - 1];
					component.SetupView(evoItem.item, MonoItemIconButton.SelectMode.ConsumeMaterial);
					component.SetClickCallback(OnResourceItemButtonClick);
					component.transform.Find("NotEnough").gameObject.SetActive(!evoItem.enough);
				}
				else
				{
					component.gameObject.SetActive(false);
				}
			}
		}

		private void GetEvoResourceList(out List<EvoItem> list, out Dictionary<int, EvoItem> dict)
		{
			list = new List<EvoItem>();
			dict = new Dictionary<int, EvoItem>();
			HashSet<int> hashSet = new HashSet<int>();
			foreach (KeyValuePair<int, int> item in storageItem.GetEvoMaterial())
			{
				int key = item.Key;
				int value = item.Value;
				EvoItem evoItem = new EvoItem();
				List<StorageDataItemBase> list2 = Singleton<StorageModule>.Instance.TryGetStorageDataItemByMetaId(key, value);
				if (list2.Count > 0)
				{
					list2.Sort(StorageDataItemBase.CompareToLevelAsc);
					evoItem.item = list2[0].Clone();
					if (evoItem.item is WeaponDataItem)
					{
						bool flag = false;
						foreach (StorageDataItemBase item2 in list2)
						{
							if (item2.uid == storageItem.uid || Singleton<AvatarModule>.Instance.TryGetAvatarByID(item2.avatarID) != null || item2.isProtected || hashSet.Contains(item2.uid))
							{
								continue;
							}
							flag = true;
							evoItem.item = item2.Clone();
							hashSet.Add(item2.uid);
							break;
						}
						evoItem.enough = flag;
						if (!flag)
						{
							evoItem.item = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(key);
						}
						evoItem.item.number = 1;
					}
					else
					{
						evoItem.enough = true;
						evoItem.item.number = value;
					}
				}
				else
				{
					evoItem.item = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(key);
					evoItem.enough = false;
					evoItem.item.number = value;
				}
				list.Add(evoItem);
				if (!dict.ContainsKey(key))
				{
					dict.Add(key, evoItem);
				}
			}
		}

		private void OnResourceItemButtonClick(StorageDataItemBase item, bool selelcted = false)
		{
			if (_resourceDict[item.ID].enough)
			{
				UIUtil.ShowItemDetail(item, true);
			}
			else if (item is MaterialDataItem)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new DropLinkDialogContext(item as MaterialDataItem));
			}
			else
			{
				UIUtil.ShowItemDetail(item, true);
			}
		}

		private bool IsAllResoursesEnough()
		{
			return _resourceList.All((EvoItem x) => x.enough);
		}

		private bool IsAvatarCostEnough()
		{
			if (AvatarMetaDataReader.GetAvatarMetaDataByKey(storageItem.avatarID) == null)
			{
				return true;
			}
			AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(storageItem.avatarID);
			if (avatarByID.MaxCost >= avatarByID.GetCurrentCost() + targetItem.GetCost() - storageItem.GetCost())
			{
				return true;
			}
			return false;
		}
	}
}
