using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class VentureSpeedUpDialogContext : BaseDialogContext
	{
		private VentureDataItem _ventureData;

		private List<StorageDataItemBase> _showItemList;

		private MaterialDataItem _selectedItem;

		private int _num_materials;

		public VentureSpeedUpDialogContext(VentureDataItem ventureData)
		{
			config = new ContextPattern
			{
				contextName = "VentureSpeedUpDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/VentureSpeedUpDialog"
			};
			_ventureData = ventureData;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			return false;
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
			_num_materials = 1;
			_showItemList = Singleton<StorageModule>.Instance.GetAllVentureSpeedUpMaterial();
			SetupList();
			UpdateSelectedItem(_selectedItem);
			return false;
		}

		public void OnDecreaseBtnClick()
		{
			if (_num_materials > 1)
			{
				_num_materials--;
				UpdateInfo();
			}
		}

		public void OnIncreaseBtnClick()
		{
			if (_num_materials < _selectedItem.number && !IsEnough(_num_materials))
			{
				_num_materials++;
				UpdateInfo();
			}
		}

		public void OnOKBtnClick()
		{
			if (_ventureData.status == VentureDataItem.VentureStatus.Done)
			{
				Destroy();
				return;
			}
			Singleton<NetworkManager>.Instance.RequestSpeedUpIslandVenture(_ventureData.VentureID, _selectedItem.ID, _num_materials);
			Destroy();
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
			_selectedItem = ((selectedItem == null) ? (_showItemList[0] as MaterialDataItem) : (selectedItem as MaterialDataItem));
			_num_materials = 1;
			base.view.transform.Find("Dialog/Content/Materials").GetComponent<MonoGridScroller>().RefreshCurrent();
			UpdateInfo();
		}

		private void UpdateInfo()
		{
			base.view.transform.Find("Dialog/Content/Info/UseNum/Text").GetComponent<Text>().text = _num_materials.ToString();
			int speedUpTime = MaterialVentureSpeedUpDataReader.GetMaterialVentureSpeedUpDataByKey(_selectedItem.ID).SpeedUpTime;
			int num = speedUpTime * _num_materials;
			base.view.transform.Find("Dialog/Content/Info/Duration/Group/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(num);
			TimeSpan value = TimeSpan.FromSeconds(num);
			DateTime targetTime = _ventureData.endTime.Subtract(value);
			base.view.transform.Find("Dialog/Content/Info/Remain/Group/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(targetTime);
		}

		public bool IsEnough(int num)
		{
			int speedUpTime = MaterialVentureSpeedUpDataReader.GetMaterialVentureSpeedUpDataByKey(_selectedItem.ID).SpeedUpTime;
			int num2 = speedUpTime * num;
			TimeSpan value = TimeSpan.FromSeconds(num2);
			DateTime dateTime = _ventureData.endTime.Subtract(value);
			return dateTime <= TimeUtil.Now;
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
	}
}
