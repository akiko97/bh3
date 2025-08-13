using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MaterialUseDialogContext : BaseDialogContext
	{
		public AvatarDataItem avatarData;

		private List<StorageDataItemBase> _showItemList;

		private MaterialDataItem _selectedItem;

		private int _useNumber;

		public MaterialUseDialogContext(AvatarDataItem avatarData)
		{
			config = new ContextPattern
			{
				contextName = "MaterialUseDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/UseMaterialDialog"
			};
			this.avatarData = avatarData;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 27:
				return OnGetEquipmentDataRsp(pkt.getData<GetEquipmentDataRsp>());
			case 36:
				return OnAddAvatarExpByMaterialRsp(pkt.getData<AddAvatarExpByMaterialRsp>());
			default:
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.SelectItemIconChange)
			{
				UpdateSelectedItem((StorageDataItemBase)ntf.body);
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/Info/UseNum/DecreaseBtn").GetComponent<Button>(), OnDecreaseBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/Info/UseNum/IncreaseBtn").GetComponent<Button>(), OnIncreaseBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/OKBtn").GetComponent<Button>(), OnOKBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/CancelBtn").GetComponent<Button>(), OnCancelBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), OnCancelBtnClick);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
		}

		protected override bool SetupView()
		{
			_useNumber = 1;
			_showItemList = Singleton<StorageModule>.Instance.GetAllAvatarExpAddMaterial();
			if (_showItemList.Count > 0)
			{
				SetupList();
				UpdateSelectedItem(_selectedItem);
			}
			return false;
		}

		public void OnDecreaseBtnClick()
		{
			if (_selectedItem != null && _useNumber > 1)
			{
				_useNumber--;
				UpdateInfo();
			}
		}

		public void OnIncreaseBtnClick()
		{
			if (_selectedItem != null && _useNumber < _selectedItem.number)
			{
				_useNumber++;
				UpdateInfo();
			}
		}

		public void OnOKBtnClick()
		{
			if (_selectedItem != null && _useNumber > 0)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.BeforeAvatarLevelUp, avatarData));
				Singleton<NetworkManager>.Instance.RequestAddAvatarExpByMaterial(avatarData.avatarID, _selectedItem.ID, _useNumber);
			}
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		public void OnCancelBtnClick()
		{
			Destroy();
		}

		private void UpdateSelectedItem(StorageDataItemBase selectedItem)
		{
			if (selectedItem != null)
			{
				_selectedItem = selectedItem as MaterialDataItem;
			}
			else
			{
				_selectedItem = _showItemList[0] as MaterialDataItem;
			}
			_useNumber = 1;
			base.view.transform.Find("Dialog/Content/Materials").GetComponent<MonoGridScroller>().RefreshCurrent();
			UpdateInfo();
		}

		private bool OnAddAvatarExpByMaterialRsp(AddAvatarExpByMaterialRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				UIUtil.UpdateAvatarSkillStatusInLocalData(avatarData);
			}
			Destroy();
			return false;
		}

		private bool OnGetEquipmentDataRsp(GetEquipmentDataRsp rsp)
		{
			_useNumber = 1;
			if (Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(typeof(MaterialDataItem), _selectedItem.ID) == null)
			{
				_selectedItem = null;
			}
			if (_selectedItem == null)
			{
				_showItemList = Singleton<StorageModule>.Instance.GetAllAvatarExpAddMaterial();
				if (_showItemList.Count <= 0)
				{
					Destroy();
					return false;
				}
				SetupList();
			}
			UpdateSelectedItem(_selectedItem);
			return false;
		}

		private void SetupList()
		{
			base.view.transform.Find("Dialog/Content/Materials").GetComponent<MonoGridScroller>().Init(OnChange, _showItemList.Count);
		}

		private void OnChange(Transform trans, int index)
		{
			bool isSelected = _showItemList[index] == _selectedItem;
			MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
			component.SetupView(_showItemList[index], MonoItemIconButton.SelectMode.SmallWhenUnSelect, isSelected);
			component.SetClickCallback(OnItemClick);
		}

		private void OnItemClick(StorageDataItemBase item, bool selected)
		{
			if (!selected)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SelectItemIconChange, item));
			}
		}

		private void UpdateInfo()
		{
			MaterialAvatarExpBonusMetaData materialAvatarExpBonusMetaData = MaterialAvatarExpBonusMetaDataReader.TryGetMaterialAvatarExpBonusMetaDataByKey(_selectedItem.ID);
			float num = 100f;
			if (materialAvatarExpBonusMetaData != null)
			{
				switch ((EntityNature)avatarData.Attribute)
				{
				case EntityNature.Biology:
					num = materialAvatarExpBonusMetaData.biologyExpBonus;
					break;
				case EntityNature.Psycho:
					num = materialAvatarExpBonusMetaData.psychoExpBonus;
					break;
				case EntityNature.Mechanic:
					num = materialAvatarExpBonusMetaData.mechanicExpBonus;
					break;
				}
			}
			num /= 100f;
			float addExp = _selectedItem.GetAvatarExpProvideNum() * num * (float)_useNumber;
			bool isAfterLevelMax = false;
			int num2 = CalculateLevelAfter(avatarData, addExp, out isAfterLevelMax);
			base.view.transform.Find("Dialog/Content/Info/UseNum/Text").GetComponent<Text>().text = _useNumber.ToString();
			base.view.transform.Find("Dialog/Content/Info/Exp/Num").GetComponent<Text>().text = addExp.ToString();
			base.view.transform.Find("Dialog/Content/Info/Level/LevelNow").GetComponent<Text>().text = avatarData.level.ToString();
			Text component = base.view.transform.Find("Dialog/Content/Info/Level/LevelAfter/Num").GetComponent<Text>();
			component.text = num2.ToString();
			base.view.transform.Find("Dialog/Content/Info/Level/LevelAfter/LvMax").gameObject.SetActive(isAfterLevelMax);
			base.view.transform.Find("Dialog/Content/Info/UseNum/IncreaseBtn").GetComponent<Button>().interactable = !isAfterLevelMax;
		}

		private int CalculateLevelAfter(AvatarDataItem avatar, float addExp, out bool isAfterLevelMax)
		{
			int avatarLevelLimit = Singleton<PlayerModule>.Instance.playerData.AvatarLevelLimit;
			List<AvatarLevelMetaData> itemList = AvatarLevelMetaDataReader.GetItemList();
			int num = Mathf.Min(itemList.Count, avatarLevelLimit);
			float num2 = addExp + (float)avatar.exp;
			int num3 = avatar.level;
			while (num2 > 0f && num3 < num && (float)itemList[num3 - 1].exp <= num2)
			{
				num2 -= (float)itemList[num3 - 1].exp;
				num3++;
			}
			isAfterLevelMax = num2 > 0f && num3 == num && (float)itemList[num3 - 1].exp <= num2;
			return num3;
		}
	}
}
