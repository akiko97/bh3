using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoVentureInfoRow : MonoBehaviour
	{
		private VentureDataItem _ventureData;

		private Action<VentureDataItem> _onFetchBtnClick;

		private Action<VentureDataItem> _onGoBtnClick;

		private Action<VentureDataItem> _onCancelBtnClick;

		private Action<VentureDataItem> _onSpeedUpBtnClick;

		public void SetupView(VentureDataItem ventureData, Action<VentureDataItem> fetchBtnCallBack, Action<VentureDataItem> goBtnCallBack, Action<VentureDataItem> cancelBtnCallBack, Action<VentureDataItem> speedUpBtnCallBack)
		{
			_ventureData = ventureData;
			_onFetchBtnClick = fetchBtnCallBack;
			_onGoBtnClick = goBtnCallBack;
			_onCancelBtnClick = cancelBtnCallBack;
			_onSpeedUpBtnClick = speedUpBtnCallBack;
			DoSetupView();
		}

		private void DoSetupView()
		{
			base.transform.Find("Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_ventureData.IconPath);
			base.transform.Find("Title/Desc").GetComponent<Text>().text = _ventureData.VentureName;
			base.transform.Find("Title/Label").GetComponent<Text>().text = ((_ventureData.StaminaCost <= 0) ? LocalizationGeneralLogic.GetText("Menu_Label_CabinVentureTypeWithoutStamina") : LocalizationGeneralLogic.GetText("Menu_Label_CabinVentureTypeWithStamina"));
			Transform transform = base.transform.Find("Title/Difficulty");
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(i + 1 == _ventureData.Difficulty);
			}
			base.transform.Find("TimeCost").gameObject.SetActive(false);
			base.transform.Find("RemainTime").gameObject.SetActive(false);
			base.transform.Find("Finish").gameObject.SetActive(false);
			base.transform.Find("SpeedUp").gameObject.SetActive(false);
			foreach (Transform item in base.transform.Find("Buttons"))
			{
				item.gameObject.SetActive(false);
			}
			switch (_ventureData.status)
			{
			case VentureDataItem.VentureStatus.None:
				base.transform.Find("TimeCost").gameObject.SetActive(true);
				base.transform.Find("TimeCost/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(_ventureData.TimeCost);
				base.transform.Find("Buttons/Go").gameObject.SetActive(true);
				break;
			case VentureDataItem.VentureStatus.InProgress:
			{
				base.transform.Find("RemainTime").gameObject.SetActive(true);
				base.transform.Find("RemainTime/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(_ventureData.endTime, null, OnVentureFinish);
				base.transform.Find("Buttons/Cancel").gameObject.SetActive(true);
				bool active = Singleton<StorageModule>.Instance.GetAllVentureSpeedUpMaterial().Count > 0;
				base.transform.Find("SpeedUp").gameObject.SetActive(active);
				break;
			}
			case VentureDataItem.VentureStatus.Done:
				base.transform.Find("Finish").gameObject.SetActive(true);
				base.transform.Find("Buttons/Fetch").gameObject.SetActive(true);
				break;
			}
			base.transform.Find("Level/Num").GetComponent<Text>().text = _ventureData.Level.ToString();
			base.transform.Find("StaminaCost/Num").GetComponent<Text>().text = _ventureData.StaminaCost.ToString();
			Transform transform3 = base.transform.Find("Rewards");
			int rewardExp = _ventureData.RewardExp;
			transform3.GetChild(0).gameObject.SetActive(rewardExp > 0);
			if (rewardExp > 0)
			{
				transform3.GetChild(0).Find("Num/Desc").GetComponent<Text>()
					.text = "×" + rewardExp;
			}
			List<int> rewardItemIDListToShow = _ventureData.RewardItemIDListToShow;
			for (int j = 1; j < transform3.childCount; j++)
			{
				Transform child = transform3.GetChild(j);
				if (j > rewardItemIDListToShow.Count)
				{
					child.gameObject.SetActive(false);
					continue;
				}
				child.gameObject.SetActive(true);
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(rewardItemIDListToShow[j - 1]);
				child.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, OnDropItemButtonClick, false, false, false, true);
			}
		}

		private void OnVentureFinish()
		{
			_ventureData.status = VentureDataItem.VentureStatus.Done;
			Singleton<NetworkManager>.Instance.RequestIslandVenture();
			DoSetupView();
		}

		private void OnDropItemButtonClick(StorageDataItemBase dropItemData)
		{
			UIUtil.ShowItemDetail(dropItemData, true);
		}

		public void OnFetchBtnClick()
		{
			if (_onFetchBtnClick != null)
			{
				_onFetchBtnClick(_ventureData);
			}
		}

		public void OnGoBtnClick()
		{
			if (_onGoBtnClick != null)
			{
				_onGoBtnClick(_ventureData);
			}
		}

		public void OnCancelBtnClick()
		{
			if (_onCancelBtnClick != null)
			{
				_onCancelBtnClick(_ventureData);
			}
		}

		public void OnSpeedUpBtnClick()
		{
			if (_onSpeedUpBtnClick != null)
			{
				_onSpeedUpBtnClick(_ventureData);
			}
		}
	}
}
