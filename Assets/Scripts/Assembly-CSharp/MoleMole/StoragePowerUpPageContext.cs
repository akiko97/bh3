using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class StoragePowerUpPageContext : BasePageContext
	{
		public const int MAX_SELECT_NUM = 6;

		public readonly StorageDataItemBase storageItem;

		public AvatarDataItem uiEquipOwner;

		private StorageDataItemBase _storageItemBeforePowerup;

		private List<StorageDataItemBase> _resourceList;

		public StoragePowerUpPageContext(StorageDataItemBase storageItem)
		{
			config = new ContextPattern
			{
				contextName = "StoragePowerUpPageContext",
				viewPrefabPath = "UI/Menus/Page/Storage/WeaponPowerUpPage"
			};
			if (storageItem is StigmataDataItem)
			{
				config.viewPrefabPath = "UI/Menus/Page/Storage/StigmataPowerUpPage";
			}
			this.storageItem = storageItem;
			_resourceList = new List<StorageDataItemBase>();
		}

		public override bool OnNotify(Notify ntf)
		{
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 32:
				return OnEquipmentPowerUpRsp(pkt.getData<EquipmentPowerUpRsp>());
			case 75:
				SetupItemProtectedStatus();
				break;
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>(), OnOkBtnClick);
			BindViewCallback(base.view.transform.Find("ActionBtns/ClearBtn").GetComponent<Button>(), OnClearBtnClick);
			if (storageItem is StigmataDataItem)
			{
				BindViewCallback(base.view.transform.Find("PowerUpInfo/Equipment/LockButton").GetComponent<Button>(), OnLockButtonClick);
			}
			else
			{
				BindViewCallback(base.view.transform.Find("Info/Info/Content/Equipment/LockButton").GetComponent<Button>(), OnLockButtonClick);
			}
		}

		protected override bool SetupView()
		{
			SetupInfoPanel();
			SetupResourceListView();
			OnSetResourceList();
			return false;
		}

		public override void OnLandedFromBackPage()
		{
			base.OnLandedFromBackPage();
			SetupView();
		}

		public void OnOkBtnClick()
		{
			if (_resourceList.Count <= 0)
			{
				return;
			}
			bool flag = false;
			foreach (StorageDataItemBase resource in _resourceList)
			{
				if ((resource is WeaponDataItem || resource is StigmataDataItem) && resource.rarity >= 3)
				{
					flag = true;
					break;
				}
			}
			if (flag)
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
							DoStartPowerup();
						}
					}
				});
			}
			else
			{
				DoStartPowerup();
			}
		}

		public void OnClearBtnClick()
		{
			_resourceList.Clear();
			SetupResourceListView();
			OnSetResourceList();
		}

		public void OnLockButtonClick()
		{
			Singleton<NetworkManager>.Instance.RequestChangeEquipmentProtectdStatus(storageItem);
		}

		private bool OnEquipmentPowerUpRsp(EquipmentPowerUpRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new EquipPowerUpEffectDialogContext(_storageItemBeforePowerup, storageItem, _resourceList, EquipPowerUpEffectDialogContext.DialogType.PowerUp, (int)rsp.boost_rate));
				_resourceList.Clear();
				SetupResourceListView();
				OnSetResourceList();
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

		private void SetupInfoPanel()
		{
			if (storageItem is WeaponDataItem)
			{
				SetupWeapon();
			}
			else if (storageItem is StigmataDataItem)
			{
				SetupStigmata();
			}
		}

		private void SetupWeapon()
		{
			string prefabPath = MiscData.Config.PrefabPath.WeaponBaseTypeIcon[storageItem.GetBaseType()];
			base.view.transform.Find("Info/Info/Title/Equipment/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
			base.view.transform.Find("Info/Info/Title/Equipment/Name").GetComponent<Text>().text = storageItem.GetDisplayTitle();
			base.view.transform.Find("Info/Info/Content/Equipment/Cost/Num").GetComponent<Text>().text = storageItem.GetCost().ToString();
			MonoEquipSubStar component = base.view.transform.Find("Info/Info/Content/Equipment/Star/EquipStar").GetComponent<MonoEquipSubStar>();
			component.SetupView(storageItem.rarity, storageItem.GetMaxRarity());
			MonoEquipSubStar component2 = base.view.transform.Find("Info/Info/Content/Equipment/Star/EquipSubStar").GetComponent<MonoEquipSubStar>();
			component2.SetupView(storageItem.GetSubRarity(), storageItem.GetMaxSubRarity() - 1);
			base.view.transform.Find("Info/Info/Content/Equipment/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(storageItem as WeaponDataItem, true);
			SetupItemProtectedStatus();
		}

		private void SetupStigmata()
		{
			StigmataDataItem stigmataDataItem = storageItem as StigmataDataItem;
			base.view.transform.Find("PowerUpInfo/Equipment/Title/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(stigmataDataItem.GetSmallIconPath());
			base.view.transform.Find("PowerUpInfo/Equipment/Title/Name").GetComponent<Text>().text = storageItem.GetDisplayTitle();
			base.view.transform.Find("PowerUpInfo/Equipment/Cost/Num").GetComponent<Text>().text = storageItem.GetCost().ToString();
			MonoEquipSubStar component = base.view.transform.Find("PowerUpInfo/Equipment/Star/EquipStar").GetComponent<MonoEquipSubStar>();
			component.SetupView(storageItem.rarity, storageItem.GetMaxRarity());
			MonoEquipSubStar component2 = base.view.transform.Find("PowerUpInfo/Equipment/Star/EquipSubStar").GetComponent<MonoEquipSubStar>();
			component2.SetupView(storageItem.GetSubRarity(), storageItem.GetMaxSubRarity() - 1);
			base.view.transform.Find("Info/Figure").GetComponent<MonoStigmataFigure>().SetupView(storageItem as StigmataDataItem);
			SetupItemProtectedStatus();
		}

		private void SetupItemProtectedStatus()
		{
			Transform transform = ((!(storageItem is StigmataDataItem)) ? base.view.transform.Find("Info/Info/Content/Equipment/LockButton") : base.view.transform.Find("PowerUpInfo/Equipment/LockButton"));
			bool isProtected = storageItem.isProtected;
			transform.Find("LockedImage").gameObject.SetActive(isProtected);
			transform.Find("UnlockedImage").gameObject.SetActive(!isProtected);
		}

		private void SetupResourceListView()
		{
			for (int i = 1; i <= 6; i++)
			{
				StorageDataItemBase item = ((i > _resourceList.Count) ? null : _resourceList[i - 1]);
				MonoItemIconButton component = base.view.transform.Find("ResourceList/Content/" + i).GetComponent<MonoItemIconButton>();
				component.SetupView(item);
				component.SetClickCallback(OnResourceItemButtonClick);
			}
			base.view.transform.Find("ActionBtns/OkBtn").GetComponent<Button>().interactable = storageItem.level < storageItem.GetMaxLevel();
		}

		private void OnResourceItemButtonClick(StorageDataItemBase item, bool selelcted = false)
		{
			Singleton<MainUIManager>.Instance.ShowPage(new StorageShowPageContext
			{
				featureType = StorageShowPageContext.FeatureType.SelectForPowerUp,
				selectedResources = _resourceList,
				powerUpTarget = storageItem,
				defaultTab = GetStorageShowPageDefaultTab()
			});
		}

		private string GetStorageShowPageDefaultTab()
		{
			if (storageItem is WeaponDataItem)
			{
				return "WeaponTab";
			}
			if (storageItem is StigmataDataItem)
			{
				return "StigmataTab";
			}
			if (storageItem is AvatarFragmentDataItem)
			{
				return "FragmentTab";
			}
			return "ItemTab";
		}

		private void SetupAttributesPanel(int lvAfter)
		{
			StorageDataItemBase storageDataItemBase = storageItem.Clone();
			storageDataItemBase.level = lvAfter;
			MonoItemAttributeDiff component = base.view.transform.Find("Attributes/InfoPanel/BasicStatus").GetComponent<MonoItemAttributeDiff>();
			component.showKeepIcon = storageItem.level != lvAfter;
			component.SetupView(uiEquipOwner, storageItem, storageDataItemBase, SetupAttr);
		}

		private void SetupLvInfoPanel(int expGet, int lvAfter)
		{
			base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/CurrentLevelNum").GetComponent<Text>().text = storageItem.level.ToString();
			base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/NextLevelNum").GetComponent<Text>().text = lvAfter.ToString();
			base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/Exp/NumText").GetComponent<Text>().text = storageItem.exp.ToString();
			base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/Exp/MaxNumText").GetComponent<Text>().text = storageItem.GetMaxExp().ToString();
			base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/Exp/TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue(storageItem.exp, storageItem.GetMaxExp(), 0f);
			base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/Exp/TiltSliderNext").gameObject.SetActive(expGet > 0);
			if (expGet > 0)
			{
				float value = ((lvAfter <= storageItem.level) ? (storageItem.exp + expGet) : storageItem.GetMaxExp());
				base.view.transform.Find("PowerUpInfo/Lv/InfoRowLv/Exp/TiltSliderNext").GetComponent<MonoMaskSlider>().UpdateValue(value, storageItem.GetMaxExp(), 0f);
			}
		}

		private void SetupScoinExp(int scoinNeed, int expGet)
		{
			base.view.transform.Find("PowerUpInfo/ScoinExp/Content/Scoin/Num").GetComponent<Text>().text = scoinNeed.ToString();
			base.view.transform.Find("PowerUpInfo/ScoinExp/Content/Exp/Num").GetComponent<Text>().text = expGet.ToString();
		}

		private void OnSetResourceList()
		{
			float scoinNeed;
			float expGet;
			UIUtil.CalCulateExpFromItems(out scoinNeed, out expGet, _resourceList, storageItem);
			int num = UIUtil.CalculateLvWithExp(expGet, storageItem);
			SetupScoinExp(Mathf.RoundToInt(scoinNeed), Mathf.RoundToInt(expGet));
			SetupAttributesPanel(num);
			SetupLvInfoPanel(Mathf.RoundToInt(expGet), Mathf.RoundToInt(num));
		}

		private void SetupAttr(Transform trans, float paramBefore, float paramAfter)
		{
			int num = Mathf.FloorToInt(paramBefore);
			int num2 = Mathf.FloorToInt(paramAfter);
			trans.gameObject.SetActive(num > 0 || num2 > 0);
			if (trans.gameObject.activeSelf)
			{
				int num3 = num2 - num;
				Transform transform = ((num3 < 0) ? trans.Find("DownNm") : trans.Find("UpNum"));
				trans.Find("UpNum").gameObject.SetActive(num3 >= 0);
				trans.Find("DownNm").gameObject.SetActive(num3 < 0);
				transform.Find("Num").GetComponent<Text>().text = num.ToString();
				transform.Find("Diff/DiffNum").GetComponent<Text>().text = num3.ToString();
			}
		}

		private void DoStartPowerup()
		{
			_storageItemBeforePowerup = storageItem.Clone();
			LoadingWheelWidgetContext loadingWheelWidgetContext = new LoadingWheelWidgetContext(32);
			loadingWheelWidgetContext.ignoreMaxWaitTime = true;
			Singleton<MainUIManager>.Instance.ShowWidget(loadingWheelWidgetContext);
			Singleton<NetworkManager>.Instance.RequestEquipmentPowerUp(storageItem, _resourceList);
		}
	}
}
