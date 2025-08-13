using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class AvatarChangeEquipPageContext : BasePageContext
	{
		public readonly AvatarDataItem avatarData;

		public readonly StorageDataItemBase storageItem;

		public readonly EquipmentSlot slot;

		private List<StorageDataItemBase> _showItemList;

		private StorageDataItemBase _selectedItem;

		public AvatarChangeEquipPageContext(AvatarDataItem avatarData, StorageDataItemBase storageItem, EquipmentSlot slot)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			config = new ContextPattern
			{
				contextName = "AvatarChangeEquipPageContext",
				viewPrefabPath = "UI/Menus/Page/AvatarChangeEquipPage"
			};
			showSpaceShip = true;
			this.avatarData = avatarData;
			this.storageItem = storageItem;
			this.slot = slot;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 40:
				return OnDressEquipmentRsp(pkt.getData<DressEquipmentRsp>());
			case 136:
				return OnExchangeAvatarWeaponRsp(pkt.getData<ExchangeAvatarWeaponRsp>());
			default:
				return false;
			}
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>(), OnOkBtnClick);
			BindViewCallback(base.view.transform.Find("ActionBtns/DetailBtn").GetComponent<Button>(), OnDetailBtnClick);
		}

		protected override bool SetupView()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Invalid comparison between Unknown and I4
			base.view.transform.Find("AvatarDetailProfile").GetComponent<MonoAvatarDetailProfile>().SetupView(avatarData);
			UIUtil.Create3DAvatarByPage(avatarData, MiscData.PageInfoKey.AvatarDetailPage, GetCurrentTabName());
			if ((int)slot == 1)
			{
				base.view.transform.Find("SelectPanel/Info/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectWeapon");
			}
			else
			{
				base.view.transform.Find("SelectPanel/Info/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectStigmata");
			}
			_showItemList = GetFilterList();
			if (_showItemList.Count > 0)
			{
				_selectedItem = _showItemList[0];
				SetupList();
				UpdateInfo();
			}
			else
			{
				SetupEmpty();
			}
			return false;
		}

		public override void BackToMainMenuPage()
		{
			UIUtil.SetAvatarTattooVisible(false, avatarData);
			base.BackToMainMenuPage();
		}

		public override void OnLandedFromBackPage()
		{
			base.OnLandedFromBackPage();
			SetupList();
			UpdateInfo();
		}

		public void OnOkBtnClick()
		{
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			if (_selectedItem != null)
			{
				StorageDataItemBase dataItem = ((storageItem == null || storageItem != _selectedItem) ? _selectedItem : null);
				if (_selectedItem != storageItem && _selectedItem.avatarID > 0)
				{
					string textID = ((!(_selectedItem is StigmataDataItem)) ? "Menu_Desc_WeaponAlreadyEquip" : "Menu_Desc_StigmataAlreadyEquip");
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
					{
						type = GeneralDialogContext.ButtonType.DoubleButton,
						title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
						desc = LocalizationGeneralLogic.GetText(textID, Singleton<AvatarModule>.Instance.GetAvatarByID(_selectedItem.avatarID).FullName),
						buttonCallBack = DressItemAlreadyEquiped
					});
				}
				else
				{
					Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(avatarData.avatarID, dataItem, slot);
				}
			}
		}

		public void OnDetailBtnClick()
		{
			if (_selectedItem != null)
			{
				StorageItemDetailPageContext storageItemDetailPageContext = new StorageItemDetailPageContext(_selectedItem, true);
				storageItemDetailPageContext.uiEquipOwner = avatarData;
				storageItemDetailPageContext.showIdentifyBtnOnly = true;
				Singleton<MainUIManager>.Instance.ShowPage(storageItemDetailPageContext);
			}
		}

		private bool OnDressEquipmentRsp(DressEquipmentRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				if ((int)slot != 1)
				{
					bool flag = storageItem != null && storageItem == _selectedItem;
					StorageDataItemBase storageDataItemBase = ((!flag) ? _selectedItem : null);
					BaseMonoUIAvatar uIAvatar = UIUtil.GetUIAvatar(avatarData.avatarID);
					if (flag)
					{
						if (uIAvatar != null)
						{
							uIAvatar.StigmataFadeOut(slot);
						}
					}
					else if (storageDataItemBase != null && uIAvatar != null)
					{
						uIAvatar.ChangeStigmata(storageItem as StigmataDataItem, _selectedItem as StigmataDataItem, slot);
					}
					EquipSetDataItem ownEquipSetData = avatarData.GetOwnEquipSetData();
					if (ownEquipSetData != null && ownEquipSetData.ownNum == 3)
					{
						Singleton<WwiseAudioManager>.Instance.Post("VO_M_Con_07_OneSuite");
					}
				}
				BackPage();
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

		private bool OnExchangeAvatarWeaponRsp(ExchangeAvatarWeaponRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode != 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
					desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)
				});
			}
			else
			{
				BackPage();
			}
			return false;
		}

		private List<StorageDataItemBase> GetFilterList()
		{
			List<StorageDataItemBase> list = Singleton<StorageModule>.Instance.UserStorageItemList.FindAll((StorageDataItemBase x) => Filter(x));
			list.Sort(StorageDataItemBase.CompareToRarityDesc);
			if (storageItem != null)
			{
				list.Remove(storageItem);
				list.Insert(0, storageItem);
			}
			return list;
		}

		private bool Filter(StorageDataItemBase item)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected I4, but got Unknown
			bool flag = false;
			bool flag2 = false;
			EquipmentSlot val = slot;
			switch ((int)val - 1)
			{
			case 0:
				flag = item.GetType() == typeof(WeaponDataItem);
				flag2 = item.GetBaseType() == avatarData.WeaponBaseTypeList[0];
				break;
			case 1:
				flag = item.GetType() == typeof(StigmataDataItem);
				flag2 = item.GetBaseType() == 1;
				break;
			case 2:
				flag = item.GetType() == typeof(StigmataDataItem);
				flag2 = item.GetBaseType() == 2;
				break;
			case 3:
				flag = item.GetType() == typeof(StigmataDataItem);
				flag2 = item.GetBaseType() == 3;
				break;
			}
			return flag && flag2;
		}

		private void SetupList()
		{
			base.view.transform.Find("SelectPanel/Info/Content/ScrollView").GetComponent<MonoGridScroller>().Init(OnChange, _showItemList.Count);
		}

		private void OnChange(Transform trans, int index)
		{
			StorageDataItemBase storageDataItemBase = _showItemList[index];
			bool isSelected = storageDataItemBase == _selectedItem;
			MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
			int newCostSum = GetNewCostSum(storageDataItemBase.GetCost());
			bool bShowCostOver = newCostSum > avatarData.MaxCost;
			bool bUsed = storageDataItemBase.avatarID > 0 && storageDataItemBase.avatarID != avatarData.avatarID;
			component.SetupView(storageDataItemBase, MonoItemIconButton.SelectMode.SmallWhenUnSelect, isSelected, bShowCostOver, bUsed);
			component.SetClickCallback(OnItemClick);
			trans.Find("AlreadyEquip").gameObject.SetActive(storageItem != null && storageItem == storageDataItemBase);
		}

		private void OnItemClick(StorageDataItemBase item, bool selected)
		{
			if (!selected)
			{
				_selectedItem = item;
				UpdateInfo();
				base.view.transform.Find("SelectPanel/Info/Content/ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
			}
		}

		private void UpdateInfo()
		{
			base.view.transform.Find("SelectPanel/Info/Content/SelectedEquip/Name").GetComponent<Text>().text = _selectedItem.GetDisplayTitle();
			base.view.transform.Find("SelectPanel/Info/Content/SelectedEquip/Lv").GetComponent<Text>().text = "LV." + _selectedItem.level;
			MonoItemAttributeDiff component = base.view.transform.Find("EquipInfoPanel/AttrDiff").GetComponent<MonoItemAttributeDiff>();
			component.showKeepIcon = true;
			component.SetupView(avatarData, storageItem, _selectedItem, SetupAttr);
			SetupSkill();
			SetupActionBtns();
			SetupProfile();
		}

		private void SetupProfile()
		{
			int newCost = ((_selectedItem != null) ? _selectedItem.GetCost() : 0);
			base.view.transform.Find("AvatarDetailProfile").GetComponent<MonoAvatarDetailProfile>().UpdateInfo(GetNewCostSum(newCost), avatarData.MaxCost, avatarData, _selectedItem);
		}

		private int GetNewCostSum(int newCost)
		{
			int currentCost = avatarData.GetCurrentCost();
			int num = ((storageItem != null) ? storageItem.GetCost() : 0);
			return currentCost - num + newCost;
		}

		private void SetupSkill()
		{
			int num = 3;
			List<EquipSkillDataItem> skills;
			if (_selectedItem is WeaponDataItem)
			{
				skills = (_selectedItem as WeaponDataItem).skills;
				base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView/Content/SetSkills").gameObject.SetActive(false);
				base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView/Content/AffixSkills").gameObject.SetActive(false);
			}
			else
			{
				StigmataDataItem stigmataDataItem = _selectedItem as StigmataDataItem;
				skills = stigmataDataItem.skills;
				Transform setSkillsTrans = base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView/Content/SetSkills");
				SortedDictionary<int, EquipSkillDataItem> allSetSkills = stigmataDataItem.GetAllSetSkills();
				if (allSetSkills.Count == 0)
				{
					setSkillsTrans.gameObject.SetActive(false);
				}
				else
				{
					setSkillsTrans.gameObject.SetActive(true);
					setSkillsTrans.Find("Name/Text").GetComponent<Text>().text = stigmataDataItem.GetEquipSetName();
					Transform transform = setSkillsTrans.Find("Desc");
					int effectiveSetSkillNumberIfEquip = GetEffectiveSetSkillNumberIfEquip();
					for (int i = 0; i < setSkillsTrans.Find("Desc").childCount; i++)
					{
						int key = i + 2;
						Transform child = transform.GetChild(i);
						if (child == null)
						{
							continue;
						}
						EquipSkillDataItem value;
						allSetSkills.TryGetValue(key, out value);
						if (value == null)
						{
							child.gameObject.SetActive(false);
							continue;
						}
						child.Find("Desc").GetComponent<Text>().text = value.GetSkillDisplay();
						if (i < effectiveSetSkillNumberIfEquip)
						{
							child.Find("Desc").GetComponent<Text>().color = MiscData.GetColor("TotalWhite");
							child.Find("Label").GetComponent<Text>().color = MiscData.GetColor("Blue");
							child.Find("Label/EffectStatus").GetComponent<Text>().color = MiscData.GetColor("Blue");
							child.Find("Label/EffectStatus").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Label_SkillEffectiveIfEquip");
						}
						else
						{
							Color color = MiscData.GetColor("TextGrey");
							child.Find("Desc").GetComponent<Text>().color = color;
							child.Find("Label").GetComponent<Text>().color = color;
							child.Find("Label/EffectStatus").GetComponent<Text>().color = color;
							child.Find("Label/EffectStatus").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Label_SkillIneffectIfEquip");
						}
					}
					Toggle component = setSkillsTrans.Find("Btn").GetComponent<Toggle>();
					UpdataSKillBtn(setSkillsTrans, component.isOn);
					BindViewCallback(component, delegate(bool isOn)
					{
						setSkillsTrans.Find("Desc").gameObject.SetActive(isOn);
						setSkillsTrans.Find("Btn/CloseImg").gameObject.SetActive(isOn);
						setSkillsTrans.Find("Btn/OpenImg").gameObject.SetActive(!isOn);
					});
				}
				List<StigmataDataItem.AffixSkillData> affixSkillList = stigmataDataItem.GetAffixSkillList();
				Transform affixSkillsTrans = base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView/Content/AffixSkills");
				if (affixSkillList.Count < 1)
				{
					affixSkillsTrans.gameObject.SetActive(false);
				}
				else
				{
					affixSkillsTrans.gameObject.SetActive(true);
					Transform transform2 = affixSkillsTrans.Find("Desc");
					for (int num2 = 0; num2 < transform2.childCount; num2++)
					{
						Transform child2 = transform2.GetChild(num2);
						child2.gameObject.SetActive(num2 < affixSkillList.Count);
						if (num2 < affixSkillList.Count)
						{
							StigmataDataItem.AffixSkillData affixSkillData = affixSkillList[num2];
							if ((bool)child2.Find("Label"))
							{
								child2.Find("Label").GetComponent<Text>().text = stigmataDataItem.GetAffixName();
							}
							child2.Find("Desc").GetComponent<Text>().text = ((!stigmataDataItem.IsAffixIdentify) ? string.Empty : affixSkillData.skill.GetSkillDisplay());
						}
					}
					Toggle component2 = affixSkillsTrans.Find("Btn").GetComponent<Toggle>();
					UpdataSKillBtn(affixSkillsTrans, component2.isOn);
					BindViewCallback(component2, delegate(bool isOn)
					{
						affixSkillsTrans.Find("Desc").gameObject.SetActive(isOn);
						affixSkillsTrans.Find("Btn/CloseImg").gameObject.SetActive(isOn);
						affixSkillsTrans.Find("Btn/OpenImg").gameObject.SetActive(!isOn);
					});
				}
			}
			Transform setNaturalSkillsTrans = base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView/Content/NaturalSkills");
			setNaturalSkillsTrans.gameObject.SetActive(skills.Count > 0);
			string text = LocalizationGeneralLogic.GetText("Menu_Title_WeaponSkill");
			if (_selectedItem is StigmataDataItem)
			{
				text = LocalizationGeneralLogic.GetText("Menu_Title_StigmataSkill");
			}
			setNaturalSkillsTrans.Find("Name/Label").GetComponent<Text>().text = text;
			for (int num3 = 1; num3 <= num; num3++)
			{
				Transform transform3 = base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView/Content/NaturalSkills/Desc/Skill_" + num3);
				transform3.gameObject.SetActive(true);
				if (num3 > skills.Count)
				{
					transform3.gameObject.SetActive(false);
					continue;
				}
				EquipSkillDataItem skillData = skills[num3 - 1];
				UpdateSkillContent(transform3, skillData);
				Toggle component3 = setNaturalSkillsTrans.Find("Btn").GetComponent<Toggle>();
				component3.isOn = true;
				UpdataSKillBtn(setNaturalSkillsTrans, component3.isOn);
				BindViewCallback(component3, delegate(bool isOn)
				{
					setNaturalSkillsTrans.Find("Desc").gameObject.SetActive(isOn);
					setNaturalSkillsTrans.Find("Btn/CloseImg").gameObject.SetActive(isOn);
					setNaturalSkillsTrans.Find("Btn/OpenImg").gameObject.SetActive(!isOn);
				});
			}
		}

		private void UpdateSkillContent(Transform trans, EquipSkillDataItem skillData)
		{
			trans.Find("Label").GetComponent<Text>().text = skillData.skillName;
			trans.Find("Desc").GetComponent<Text>().text = skillData.GetSkillDisplay(_selectedItem.level);
		}

		private void UpdataSKillBtn(Transform trans, bool isOn)
		{
			trans.Find("Desc").gameObject.SetActive(isOn);
			trans.Find("Btn/CloseImg").gameObject.SetActive(isOn);
			trans.Find("Btn/OpenImg").gameObject.SetActive(!isOn);
		}

		private void SetupEmpty()
		{
			base.view.transform.Find("EquipInfoPanel/Skills/ScrollerView").gameObject.SetActive(false);
			foreach (Transform item in base.view.transform.Find("SelectPanel/Info/Content"))
			{
				item.gameObject.SetActive(false);
			}
		}

		private void SetupActionBtns()
		{
			Transform transform = base.view.transform.Find("ActionBtns/OkBtn");
			base.view.transform.Find("ActionBtns/WarnMsg").gameObject.SetActive(false);
			string empty = string.Empty;
			bool flag = false;
			if (storageItem != null && storageItem == _selectedItem)
			{
				empty = "Menu_Action_Unequip";
				flag = !(storageItem is WeaponDataItem);
			}
			else
			{
				empty = "Menu_Action_Equip";
				bool flag2 = _selectedItem.avatarID > 0;
				bool flag3 = IsCostOver(storageItem, _selectedItem, avatarData);
				bool flag4 = _selectedItem is StigmataDataItem && !(_selectedItem as StigmataDataItem).IsAffixIdentify;
				flag = !flag3;
				if (flag4)
				{
					flag = false;
				}
				base.view.transform.Find("ActionBtns/WarnMsg").gameObject.SetActive(!flag);
				string text = string.Empty;
				if (flag2)
				{
					base.view.transform.Find("ActionBtns/WarnMsg").gameObject.SetActive(true);
					bool flag5 = IsCostOver(_selectedItem, storageItem, Singleton<AvatarModule>.Instance.GetAvatarByID(_selectedItem.avatarID));
					if (flag3)
					{
						text = LocalizationGeneralLogic.GetText("Menu_EquipWarning_CostLack");
						flag = false;
					}
					else if (flag5)
					{
						text = LocalizationGeneralLogic.GetText("Menu_EquipWarning_OppositeCostOver");
						flag = false;
					}
					else
					{
						flag = true;
					}
				}
				else if (flag4)
				{
					base.view.transform.Find("ActionBtns/WarnMsg").gameObject.SetActive(true);
					text = LocalizationGeneralLogic.GetText("Menu_AffixTab_NotIdentifyName");
					flag = false;
				}
				else if (flag3)
				{
					base.view.transform.Find("ActionBtns/WarnMsg").gameObject.SetActive(true);
					text = LocalizationGeneralLogic.GetText("Menu_EquipWarning_CostLack");
				}
				base.view.transform.Find("ActionBtns/WarnMsg/Text").GetComponent<Text>().text = text;
				base.view.transform.Find("ActionBtns/WarnMsg").gameObject.SetActive(!flag);
			}
			transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(empty);
			transform.GetComponent<Button>().interactable = flag;
		}

		private bool IsCostOver(StorageDataItemBase from, StorageDataItemBase to, AvatarDataItem avatarData)
		{
			int num = ((from != null) ? from.GetCost() : 0);
			int num2 = ((to != null) ? to.GetCost() : 0);
			return avatarData.GetCurrentCost() - num + num2 > avatarData.MaxCost;
		}

		private void DressItemAlreadyEquiped(bool confirm)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Invalid comparison between Unknown and I4
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Invalid comparison between Unknown and I4
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Invalid comparison between Unknown and I4
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			if (!confirm)
			{
				return;
			}
			if (storageItem == null)
			{
				if ((int)slot != 1)
				{
					AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(_selectedItem.avatarID);
					EquipmentSlot val = avatarByID.SearchEquipSlot(_selectedItem);
					if ((int)val > 0)
					{
						Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(avatarByID.avatarID, null, val);
						Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(avatarData.avatarID, _selectedItem, val);
					}
				}
			}
			else if (storageItem is StigmataDataItem)
			{
				AvatarDataItem avatarByID2 = Singleton<AvatarModule>.Instance.GetAvatarByID(_selectedItem.avatarID);
				EquipmentSlot val2 = avatarByID2.SearchEquipSlot(_selectedItem);
				if ((int)val2 > 0)
				{
					Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(avatarByID2.avatarID, null, val2);
					Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(avatarData.avatarID, null, val2);
					Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(avatarData.avatarID, _selectedItem, val2);
					Singleton<NetworkManager>.Instance.RequestDressEquipmentReq(avatarByID2.avatarID, storageItem, val2);
				}
			}
			else if (storageItem is WeaponDataItem)
			{
				Singleton<NetworkManager>.Instance.RequestExchangeAvatarWeapon(avatarData.avatarID, _selectedItem.avatarID);
			}
		}

		private string GetCurrentTabName()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)slot == 1)
			{
				return "WeaponTab";
			}
			return "StigmataTab";
		}

		private void SetupAttr(Transform trans, float paramBefore, float paramAfter)
		{
			int num = Mathf.FloorToInt(paramBefore);
			int num2 = Mathf.FloorToInt(paramAfter);
			trans.gameObject.SetActive(num > 0 || num2 > 0);
			if (trans.gameObject.activeSelf)
			{
				Color color;
				UIUtil.TryParseHexString("#00C3FFFF", out color);
				Color white = Color.white;
				Color red = Color.red;
				int num3 = num2 - num;
				Text component = trans.Find("Values/Current").GetComponent<Text>();
				component.text = num.ToString();
				Color newColor = ((num3 > 0) ? color : ((num3 >= 0) ? white : red));
				Text component2 = trans.Find("Values/New").GetComponent<Text>();
				FormatText(component2, num2, newColor, false, string.Empty, string.Empty);
				Text component3 = trans.Find("Values/Delta").GetComponent<Text>();
				FormatText(component3, num3, newColor, true, "[", "]");
				component3.gameObject.SetActive(num3 != 0);
			}
		}

		private void FormatText(Text textComp, int number, Color newColor, bool bSign, string prefix, string suffix)
		{
			string text = ((!bSign) ? string.Empty : ((number <= 0) ? string.Empty : "+"));
			string text2 = string.Format("{0}{1}{2}{3}", prefix, text, number, suffix);
			textComp.text = text2;
			textComp.color = newColor;
		}

		private int GetEffectiveSetSkillNumberIfEquip()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Invalid comparison between Unknown and I4
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			if (!(_selectedItem is StigmataDataItem))
			{
				return 0;
			}
			int num = 0;
			foreach (KeyValuePair<EquipmentSlot, StorageDataItemBase> item in avatarData.equipsMap)
			{
				if ((int)item.Key != 1 && item.Key != slot && item.Value != null)
				{
					StigmataDataItem stigmataDataItem = item.Value as StigmataDataItem;
					int equipmentSetID = stigmataDataItem.GetEquipmentSetID();
					if (equipmentSetID != 0 && equipmentSetID == (_selectedItem as StigmataDataItem).GetEquipmentSetID())
					{
						num++;
					}
				}
			}
			return num;
		}
	}
}
