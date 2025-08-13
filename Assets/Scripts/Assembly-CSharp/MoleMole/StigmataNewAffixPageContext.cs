using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class StigmataNewAffixPageContext : BasePageContext
	{
		public readonly StigmataDataItem stigmata;

		private List<StigmataDataItem> _showItemList;

		private StigmataDataItem _selectedItem;

		public StigmataNewAffixPageContext(StigmataDataItem stigmata)
		{
			config = new ContextPattern
			{
				contextName = "StigmataNewAffixPage",
				viewPrefabPath = "UI/Menus/Page/Storage/StigmataNewAffixPage"
			};
			this.stigmata = stigmata;
		}

		public override bool OnNotify(Notify ntf)
		{
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 194)
			{
				return OnFeedStigmataAffixRsp(pkt.getData<FeedStigmataAffixRsp>());
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>(), OnOkBtnClick);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Figure").GetComponent<MonoStigmataFigure>().SetupView(stigmata);
			SetupLvInfoPanel();
			SetupStigmataInfo(stigmata, base.view.transform.Find("OriginInfo"));
			_showItemList = Singleton<StorageModule>.Instance.GetStigmatasCanUseForNewAffix(stigmata);
			_showItemList.Sort(StorageDataItemBase.CompareToRarityAsc);
			_selectedItem = null;
			if (_showItemList.Count > 0)
			{
				SetupList();
				UpdateInfo();
			}
			else
			{
				SetupEmpty();
			}
			return false;
		}

		public void OnOkBtnClick()
		{
			if (_selectedItem == null)
			{
				return;
			}
			if (_selectedItem.rarity > 3)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.DoubleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Tips"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_WillConsume3StarItemHint"),
					buttonCallBack = delegate(bool confirmed)
					{
						if (confirmed)
						{
							Singleton<NetworkManager>.Instance.RequestFeedStigmataAffix(stigmata.uid, _selectedItem.uid);
						}
					}
				});
			}
			else
			{
				Singleton<NetworkManager>.Instance.RequestFeedStigmataAffix(stigmata.uid, _selectedItem.uid);
			}
		}

		public bool OnFeedStigmataAffixRsp(FeedStigmataAffixRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				List<StorageDataItemBase> list = new List<StorageDataItemBase>();
				list.Add(_selectedItem);
				List<StorageDataItemBase> materialList = list;
				StorageDataItemBase storageDataItemBase = stigmata.Clone();
				int pre_affix_id = (int)(rsp.new_pre_affix_idSpecified ? rsp.new_pre_affix_id : 0);
				int suf_affix_id = (int)(rsp.new_suf_affix_idSpecified ? rsp.new_suf_affix_id : 0);
				(storageDataItemBase as StigmataDataItem).SetAffixSkill(true, pre_affix_id, suf_affix_id);
				Singleton<MainUIManager>.Instance.ShowDialog(new EquipPowerUpEffectDialogContext(stigmata, storageDataItemBase, materialList, EquipPowerUpEffectDialogContext.DialogType.NewAffix));
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeFail"),
					desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)
				});
			}
			return false;
		}

		private void SetupStigmataInfo(StigmataDataItem m_stigmata, Transform trans)
		{
			trans.Find("Equipment/Content/Title/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(m_stigmata.GetSmallIconPath());
			string text = m_stigmata.GetDisplayTitle();
			if (m_stigmata.IsAffixIdentify)
			{
				string affixName = m_stigmata.GetAffixName();
				if (!string.IsNullOrEmpty(affixName))
				{
					text = MiscData.AddColor("Blue", affixName) + " " + text;
				}
			}
			else
			{
				text = MiscData.AddColor("WarningRed", m_stigmata.GetAffixName()) + " " + text;
			}
			trans.Find("Equipment/Content/Title/Name").GetComponent<Text>().text = text;
			MonoEquipSubStar component = trans.Find("Equipment/Content/Star/EquipStar").GetComponent<MonoEquipSubStar>();
			component.SetupView(m_stigmata.rarity, m_stigmata.GetMaxRarity());
			MonoEquipSubStar component2 = trans.Find("Equipment/Content/Star/EquipSubStar").GetComponent<MonoEquipSubStar>();
			component2.SetupView(m_stigmata.GetSubRarity(), m_stigmata.GetMaxSubRarity() - 1);
			MonoStigmataAffixSkillPanel component3 = trans.Find("AffixSkills/Skills/Content").GetComponent<MonoStigmataAffixSkillPanel>();
			component3.SetupView(m_stigmata, m_stigmata.GetAffixSkillList());
		}

		private void SetupLvInfoPanel()
		{
			base.view.transform.Find("OriginInfo/Lv/InfoRowLv/Lv/CurrentLevelNum").GetComponent<Text>().text = stigmata.level.ToString();
			base.view.transform.Find("OriginInfo/Lv/InfoRowLv/Lv/MaxLevelNum").GetComponent<Text>().text = stigmata.GetMaxLevel().ToString();
			if (stigmata.level == stigmata.GetMaxLevel())
			{
				base.view.transform.Find("OriginInfo/Lv/InfoRowLv/Lv/MaxLevelNum").GetComponent<Text>().color = MiscData.GetColor("Yellow");
			}
			base.view.transform.Find("OriginInfo/Lv/InfoRowLv/Exp/NumText").GetComponent<Text>().text = stigmata.exp.ToString();
			base.view.transform.Find("OriginInfo/Lv/InfoRowLv/Exp/MaxNumText").GetComponent<Text>().text = stigmata.GetMaxExp().ToString();
			base.view.transform.Find("OriginInfo/Lv/InfoRowLv/Exp/TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue(stigmata.exp, stigmata.GetMaxExp(), 0f);
		}

		private void SetupList()
		{
			base.view.transform.Find("SelectPanel/List").GetComponent<MonoGridScroller>().Init(OnChange, _showItemList.Count);
		}

		private void OnChange(Transform trans, int index)
		{
			StorageDataItemBase storageDataItemBase = _showItemList[index];
			bool isSelected = storageDataItemBase == _selectedItem;
			MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
			component.showProtected = true;
			component.blockSelect = storageDataItemBase.isProtected || storageDataItemBase.avatarID > 0;
			component.SetupView(storageDataItemBase, MonoItemIconButton.SelectMode.CheckWhenSelect, isSelected, false, storageDataItemBase.avatarID > 0);
			component.SetClickCallback(OnItemClick);
		}

		private void OnItemClick(StorageDataItemBase item, bool selected)
		{
			if (_selectedItem != null && item.uid == _selectedItem.uid)
			{
				_selectedItem = null;
			}
			else
			{
				_selectedItem = item as StigmataDataItem;
			}
			UpdateInfo();
			base.view.transform.Find("SelectPanel/List").GetComponent<MonoGridScroller>().RefreshCurrent();
		}

		private void UpdateInfo()
		{
			base.view.transform.Find("ResourceInfo/Equipment/Content").gameObject.SetActive(_selectedItem != null);
			base.view.transform.Find("ResourceInfo/AffixSkills/Skills/Content").gameObject.SetActive(_selectedItem != null);
			if (_selectedItem != null)
			{
				SetupStigmataInfo(_selectedItem, base.view.transform.Find("ResourceInfo"));
			}
			base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>().interactable = _selectedItem != null;
		}

		private void SetupEmpty()
		{
			base.view.transform.Find("ResourceInfo/Equipment/Content").gameObject.SetActive(false);
			base.view.transform.Find("ResourceInfo/AffixSkills/Skills/Content").gameObject.SetActive(false);
			base.view.transform.Find("SelectPanel/List").gameObject.SetActive(false);
			base.view.transform.Find("SelectPanel/EmptyText").gameObject.SetActive(true);
		}
	}
}
