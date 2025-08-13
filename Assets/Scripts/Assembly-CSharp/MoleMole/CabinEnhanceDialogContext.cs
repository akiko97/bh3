using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class CabinEnhanceDialogContext : BaseDialogContext
	{
		private CainEnhanceType _enhanceType;

		private CabinDataItemBase _cabinData;

		private List<StorageDataItemBase> _itemNeedList;

		private List<EvoItem> _materialList;

		private Dictionary<int, EvoItem> _resourceDict;

		private bool _materialEnough;

		private int _extendGradBefore;

		public CabinEnhanceDialogContext(CabinDataItemBase cabinData, CainEnhanceType enhanceType)
		{
			config = new ContextPattern
			{
				contextName = "CabinEnhanceDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/CabinEnhanceDialog"
			};
			_cabinData = cabinData;
			_extendGradBefore = _cabinData.extendGrade;
			_enhanceType = enhanceType;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 159)
			{
				OnLevelUpCabinRsp(pkt.getData<LevelUpCabinRsp>());
			}
			if (cmdId == 161)
			{
				return OnExtendCabinRspp(pkt.getData<ExtendCabinRsp>());
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/Consume/Btn").GetComponent<Button>(), OnActionButtonClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			base.view.transform.Find("Dialog/Content/CabinLevelUpInfo").gameObject.SetActive(_enhanceType == CainEnhanceType.LevelUp);
			base.view.transform.Find("Dialog/Content/Materials/RemainTime").gameObject.SetActive(_enhanceType == CainEnhanceType.LevelUp);
			base.view.transform.Find("Dialog/Content/CabinExtendInfo").gameObject.SetActive(_enhanceType == CainEnhanceType.Extend);
			base.view.transform.Find("Dialog/Content/Materials/RedTips").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/Materials/RemainTime").gameObject.SetActive(false);
			int num = 0;
			switch (_enhanceType)
			{
			case CainEnhanceType.LevelUp:
			{
				base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_LevelUpCabin");
				base.view.transform.Find("Dialog/Content/Consume/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Action_BeginToLevelUp");
				Transform transform3 = base.view.transform.Find("Dialog/Content/CabinLevelUpInfo/CabinLevelUpInfo");
				transform3.Find("CabinName").GetComponent<Text>().text = _cabinData.GetCabinName();
				transform3.Find("CurrentLevel").GetComponent<Text>().text = "Lv." + _cabinData.level;
				transform3.Find("NextLevel").GetComponent<Text>().text = "Lv." + (_cabinData.level + 1);
				_itemNeedList = _cabinData.GetLevelUpItemNeed();
				base.view.transform.Find("Dialog/Content/Materials/RemainTime").gameObject.SetActive(true);
				base.view.transform.Find("Dialog/Content/Materials/RemainTime/Time").GetComponent<MonoRemainTimer>().SetTargetTime(_cabinData.GetCabinLevelUpTimeCost());
				num = _cabinData.GetCabinLevelUpScoinCost();
				SetupCabinLevelUpDiff(_cabinData.cabinType);
				break;
			}
			case CainEnhanceType.Extend:
			{
				base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_ExtendCabin");
				base.view.transform.Find("Dialog/Content/Consume/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Action_BeginToExtend");
				Transform transform = base.view.transform.Find("Dialog/Content/CabinExtendInfo/CabinExtendInfo");
				transform.Find("CabinName").GetComponent<Text>().text = _cabinData.GetCabinName();
				transform.Find("CurrentLevel").GetComponent<MonoCabinExtendGrade>().SetupView(_cabinData.extendGrade);
				transform.Find("NextLevel").GetComponent<MonoCabinExtendGrade>().SetupView(_cabinData.extendGrade + 1);
				Transform transform2 = base.view.transform.Find("Dialog/Content/CabinExtendInfo/CabinExtendLevelInfo");
				transform2.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Label_LevelTopLimit");
				transform2.Find("CurrentLevel").GetComponent<Text>().text = "Lv." + _cabinData.GetCabinMaxLevel();
				transform2.Find("NextLevel").GetComponent<Text>().text = "Lv." + _cabinData.GetCabinMaxLevelNextExntendGrade();
				_itemNeedList = _cabinData.GetExtendItemNeed();
				num = _cabinData.GetCabinExntendScoinCost();
				break;
			}
			}
			GetEvoResourceList(out _materialList, out _resourceDict);
			base.view.transform.Find("Dialog/Content/Materials/Materials").GetComponent<MonoGridScroller>().Init(OnChange, _materialList.Count, new Vector2(0f, 0f));
			base.view.transform.Find("Dialog/Content/Consume/SCoinNum").GetComponent<Text>().text = num.ToString();
			bool flag = num <= Singleton<PlayerModule>.Instance.playerData.scoin;
			bool flag2 = flag && _materialEnough;
			bool flag3 = true;
			bool flag4 = true;
			bool flag5 = _cabinData.level < _cabinData.GetCabinMaxLevel();
			if (_enhanceType == CainEnhanceType.LevelUp)
			{
				flag3 = _cabinData.GetPlayerLevelNeedToUpLevel() <= Singleton<PlayerModule>.Instance.playerData.teamLevel;
				flag4 = !Singleton<IslandModule>.Instance.HasCabinLevelUpInProgress();
				flag2 = flag2 && flag3;
				flag2 = flag2 && flag4;
				flag2 = flag2 && flag5;
			}
			base.view.transform.Find("Dialog/Content/Consume/Btn").GetComponent<Button>().interactable = flag2;
			if (!flag)
			{
				base.view.transform.Find("Dialog/Content/Consume/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Err_ScoinLack");
			}
			if (!_materialEnough)
			{
				base.view.transform.Find("Dialog/Content/Consume/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Err_ItemLack");
			}
			if (_enhanceType == CainEnhanceType.LevelUp)
			{
				if (!flag3)
				{
					base.view.transform.Find("Dialog/Content/Materials/RedTips").gameObject.SetActive(true);
					base.view.transform.Find("Dialog/Content/Materials/RemainTime").gameObject.SetActive(false);
					base.view.transform.Find("Dialog/Content/Materials/RedTips/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CommandLevelLack", _cabinData.GetPlayerLevelNeedToUpLevel());
				}
				if (!flag4)
				{
					base.view.transform.Find("Dialog/Content/Materials/RedTips").gameObject.SetActive(true);
					base.view.transform.Find("Dialog/Content/Materials/RemainTime").gameObject.SetActive(false);
					base.view.transform.Find("Dialog/Content/Materials/RedTips/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_OnlyOneCabinCanUpLevel");
				}
				if (!flag5)
				{
					base.view.transform.Find("Dialog/Content/Materials/RedTips").gameObject.SetActive(true);
					base.view.transform.Find("Dialog/Content/Materials/RemainTime").gameObject.SetActive(false);
					if (_cabinData.CanExtendCabin())
					{
						base.view.transform.Find("Dialog/Content/Materials/RedTips/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_NeedToExtend");
					}
					else
					{
						base.view.transform.Find("Dialog/Content/Materials/RedTips/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_AlreadyMax");
					}
				}
			}
			return false;
		}

		public void Close()
		{
			Destroy();
		}

		public void OnActionButtonClick()
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			switch (_enhanceType)
			{
			case CainEnhanceType.Extend:
				Singleton<NetworkManager>.Instance.RequestExtendCabin(_cabinData.cabinType);
				break;
			case CainEnhanceType.LevelUp:
				Singleton<NetworkManager>.Instance.RequestCabinLevelUp(_cabinData.cabinType);
				break;
			}
		}

		private bool OnLevelUpCabinRsp(LevelUpCabinRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode != 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					desc = ((Enum)rsp.retcode).ToString()
				});
			}
			else
			{
				Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowLevelUpCompleteSet[_cabinData.cabinType] = true;
				Singleton<MiHoYoGameData>.Instance.Save();
			}
			Destroy();
			return false;
		}

		private bool OnExtendCabinRspp(ExtendCabinRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode != 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					desc = ((Enum)rsp.retcode).ToString()
				});
			}
			else
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnCabinBeginExtend, _extendGradBefore + 1));
			}
			Destroy();
			return false;
		}

		private void OnChange(Transform trans, int index)
		{
			MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
			component.SetupView(_materialList[index].item, MonoItemIconButton.SelectMode.ConsumeMaterial);
			component.transform.Find("NotEnough").gameObject.SetActive(!_materialList[index].enough);
			component.SetClickCallback(OnResourceItemButtonClick);
		}

		private void OnResourceItemButtonClick(StorageDataItemBase item, bool selelcted = false)
		{
			if (_resourceDict[item.ID].enough)
			{
				UIUtil.ShowItemDetail(item, true);
			}
			else if (item is MaterialDataItem)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new DropLinkDialogContext(item as MaterialDataItem, OnDropLinkClick));
			}
		}

		private void GetEvoResourceList(out List<EvoItem> list, out Dictionary<int, EvoItem> dict)
		{
			list = new List<EvoItem>();
			dict = new Dictionary<int, EvoItem>();
			_materialEnough = true;
			foreach (StorageDataItemBase itemNeed in _itemNeedList)
			{
				int iD = itemNeed.ID;
				int number = itemNeed.number;
				EvoItem evoItem = new EvoItem();
				List<StorageDataItemBase> list2 = Singleton<StorageModule>.Instance.TryGetStorageDataItemByMetaId(iD, number);
				if (list2.Count > 0)
				{
					list2.Sort(StorageDataItemBase.CompareToLevelAsc);
					evoItem.item = list2[0].Clone();
					evoItem.enough = true;
					evoItem.item.number = number;
				}
				else
				{
					evoItem.item = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(iD);
					evoItem.enough = false;
					evoItem.item.number = number;
					_materialEnough = false;
				}
				list.Add(evoItem);
				if (!dict.ContainsKey(iD))
				{
					dict.Add(iD, evoItem);
				}
			}
		}

		private void SetupCabinLevelUpDiff(CabinType cabinType)
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Expected I4, but got Unknown
			Transform transform = base.view.transform.Find("Dialog/Content/CabinLevelUpInfo/Info1");
			Transform transform2 = base.view.transform.Find("Dialog/Content/CabinLevelUpInfo/Info2");
			Transform transform3 = base.view.transform.Find("Dialog/Content/CabinLevelUpInfo/Info3");
			Transform transform4 = base.view.transform.Find("Dialog/Content/CabinLevelUpInfo/CabinOtherInfo");
			transform.gameObject.SetActive(false);
			transform2.gameObject.SetActive(false);
			transform3.gameObject.SetActive(false);
			transform4.gameObject.SetActive(false);
			switch ((int)cabinType - 1)
			{
			case 0:
			{
				transform.gameObject.SetActive(true);
				transform.Find("Current").gameObject.SetActive(true);
				transform.Find("UpIcon").gameObject.SetActive(true);
				int maxPowerCost = Singleton<IslandModule>.Instance.GetMaxPowerCost();
				int nextLevelMaxPowerCost = Singleton<IslandModule>.Instance.GetNextLevelMaxPowerCost();
				transform.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinLevelUp_MaxPower");
				transform.Find("Current").GetComponent<Text>().text = maxPowerCost.ToString();
				transform.Find("Next").GetComponent<Text>().text = nextLevelMaxPowerCost.ToString();
				break;
			}
			case 1:
				SetupTechInfo(transform);
				break;
			case 5:
				SetupTechInfo(transform);
				break;
			case 6:
				SetupTechInfo(transform);
				break;
			case 2:
			{
				SetupTechInfo(transform);
				int num = (int)CabinCollectLevelMetaDataReader.GetCabinCollectLevelDataMetaDataByKey(_cabinData.level).scoinGrowthBase;
				int num2 = (int)CabinCollectLevelMetaDataReader.GetCabinCollectLevelDataMetaDataByKey(_cabinData.level + 1).scoinGrowthBase;
				if (num != num2)
				{
					transform2.gameObject.SetActive(true);
					transform2.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinLevelUp_ScoinGrowth");
					transform2.Find("Current").GetComponent<Text>().text = num.ToString();
					transform2.Find("Next").GetComponent<Text>().text = num2.ToString();
				}
				num = (int)CabinCollectLevelMetaDataReader.GetCabinCollectLevelDataMetaDataByKey(_cabinData.level).scoinStorageBase;
				num2 = (int)CabinCollectLevelMetaDataReader.GetCabinCollectLevelDataMetaDataByKey(_cabinData.level + 1).scoinStorageBase;
				if (num != num2)
				{
					transform3.gameObject.SetActive(true);
					transform3.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinLevelUp_ScoinStorage");
					transform3.Find("Current").GetComponent<Text>().text = num.ToString();
					transform3.Find("Next").GetComponent<Text>().text = num2.ToString();
				}
				break;
			}
			case 3:
				SetupTechInfo(transform);
				break;
			case 4:
			{
				SetupTechInfo(transform);
				int maxVentureNumBase = CabinVentureLevelMetaDataReader.GetCabinVentureLevelDataMetaDataByKey(_cabinData.level).maxVentureNumBase;
				int maxVentureNumBase2 = CabinVentureLevelMetaDataReader.GetCabinVentureLevelDataMetaDataByKey(_cabinData.level + 1).maxVentureNumBase;
				if (maxVentureNumBase != maxVentureNumBase2)
				{
					transform2.gameObject.SetActive(true);
					transform2.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinLevelUp_MaxVenture");
					transform2.Find("Current").GetComponent<Text>().text = maxVentureNumBase.ToString();
					transform2.Find("Next").GetComponent<Text>().text = maxVentureNumBase2.ToString();
				}
				maxVentureNumBase = CabinVentureLevelMetaDataReader.GetCabinVentureLevelDataMetaDataByKey(_cabinData.level).maxVentureInProgressNumBase;
				maxVentureNumBase2 = CabinVentureLevelMetaDataReader.GetCabinVentureLevelDataMetaDataByKey(_cabinData.level + 1).maxVentureInProgressNumBase;
				if (maxVentureNumBase != maxVentureNumBase2)
				{
					transform3.gameObject.SetActive(true);
					transform3.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinLevelUp_MaxInProgressVenture");
					transform3.Find("Current").GetComponent<Text>().text = maxVentureNumBase.ToString();
					transform3.Find("Next").GetComponent<Text>().text = maxVentureNumBase2.ToString();
				}
				break;
			}
			}
		}

		private void SetupTechInfo(Transform techTran)
		{
			int availableNodesDiff = _cabinData._techTree.GetAvailableNodesDiff(_cabinData.level + 1);
			if (availableNodesDiff > 0)
			{
				techTran.gameObject.SetActive(true);
				techTran.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinLevelUp_NewAvailableTechNode");
				techTran.Find("Current").gameObject.SetActive(false);
				techTran.Find("UpIcon").gameObject.SetActive(false);
				techTran.Find("Next").GetComponent<Text>().text = availableNodesDiff.ToString();
			}
		}

		private void OnDropLinkClick(LevelDataItem levelData)
		{
			if (levelData != null)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralConfirmDialogContext
				{
					type = GeneralConfirmDialogContext.ButtonType.DoubleButton,
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_LeaveIslandHint"),
					buttonCallBack = delegate(bool confirmed)
					{
						ShowChapterSelectPage(confirmed, levelData);
					}
				});
			}
		}

		private void ShowChapterSelectPage(bool confirmed, LevelDataItem levelData)
		{
			if (confirmed)
			{
				Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true, true, delegate
				{
					ShowChapterSelectPageWhenMainSceneLoaded(levelData);
				});
			}
		}

		private void ShowChapterSelectPageWhenMainSceneLoaded(LevelDataItem levelData)
		{
			Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(levelData));
		}
	}
}
