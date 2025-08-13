using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoCabinMainInfo : MonoBehaviour
	{
		private MonoIslandBuilding _target;

		private CabinDataItemBase _cabinData;

		private MonoIslandCameraSM _cameraSM;

		private bool _isUpdating;

		private Camera _mainCamera;

		private Camera _uiCamera;

		private Vector3 _offset = new Vector3(0f, 10f, 0f);

		private bool _isUpLevel;

		private Transform _Locked;

		private Transform _LvInfo;

		private Transform _Output;

		private Transform _LvInfo_Lv_Name;

		private Transform _LvInfo_Lv_Lv;

		private Transform _LvInfo_LvUpProgress;

		private Transform _LvInfo_LvUpProgress_TimeRemain_HPSlider_Slider;

		private Transform _LvInfo_LvUpProgress_TimeRemain_Time;

		private Transform _Day;

		private Transform _Hrs;

		private Transform _Min;

		private Transform _Sec;

		private Transform _DayText;

		private Transform _HrsText;

		private Transform _MinText;

		private Transform _SecText;

		private Transform _LvInfo_Lv_PopUp_New;

		private Transform _LvInfo_Lv_PopUp_PopUp;

		private Text _LvInfo_Lv_Name_TextComp;

		private Text _LvInfo_Lv_Lv_TextComp;

		private Image _LvInfo_LvUpProgress_TimeRemain_HPSlider_Slider_ImageComp;

		private Text _Day_TextComp;

		private Text _Hrs_TextComp;

		private Text _Min_TextComp;

		private Text _Sec_TextComp;

		private Transform _LvInfo_Button;

		private Button _LvInfo_Button_UI;

		private bool _lastFrame_output_active;

		private CabinStatus _lastFrame_cabinStatus;

		private E_TimeFormat _lastFrame_timeFormat;

		private float _fetch_scoin_time;

		public void BindingTargetBuilding(MonoIslandBuilding target, CabinDataItemBase cabinData)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			_cabinData = cabinData;
			_target = target;
			_isUpdating = true;
			Dictionary<CabinType, bool> cabinNeedToShowNewUnlockDict = Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowNewUnlockDict;
			bool flag = cabinNeedToShowNewUnlockDict.ContainsKey(_cabinData.cabinType) && cabinNeedToShowNewUnlockDict[_cabinData.cabinType];
			_LvInfo_Lv_PopUp_New.gameObject.SetActive(flag);
			_LvInfo_Lv_PopUp_PopUp.gameObject.SetActive(false);
			if (!flag)
			{
				RefreshPopUp();
			}
			_target.GetModel().RefreshLockStyle(_cabinData.status);
			bool flag2 = _cabinData is CabinCollectDataItem && (_cabinData as CabinCollectDataItem).CanFetchScoin();
			_Output.gameObject.SetActive(flag2);
			_lastFrame_output_active = flag2;
			_Locked.gameObject.SetActive(_cabinData.status == CabinStatus.Locked);
			_LvInfo.gameObject.SetActive(_cabinData.status == CabinStatus.UnLocked);
			_lastFrame_cabinStatus = _cabinData.status;
			if (_cabinData.status == CabinStatus.UnLocked)
			{
				_LvInfo_Lv_Name_TextComp.text = _cabinData.GetCabinName();
			}
			bool flag3 = _cabinData.levelUpEndTime > TimeUtil.Now;
			_LvInfo_LvUpProgress.gameObject.SetActive(flag3);
			if (flag3)
			{
				E_TimeFormat timeFormat = GetTimeFormat(_cabinData.levelUpEndTime - TimeUtil.Now);
				SetUITimeFormat(timeFormat);
				_lastFrame_timeFormat = timeFormat;
			}
		}

		public void RefreshPopUp()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			if ((int)_cabinData.cabinType == 5 && Singleton<IslandModule>.Instance.GetVentureDoneNum() > 0)
			{
				_LvInfo_Lv_PopUp_PopUp.gameObject.SetActive(true);
			}
		}

		public void ReStart()
		{
			BindingTargetBuilding(_target, _cabinData);
		}

		private void Awake()
		{
			_mainCamera = GameObject.Find("IslandCameraGroup/MainCamera").GetComponent<Camera>();
			_uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
			_cameraSM = GameObject.Find("IslandCameraGroup").GetComponent<MonoIslandCameraSM>();
			_LvInfo_Lv_PopUp_New = base.transform.Find("LvInfo/Lv/PopUp/New");
			_LvInfo_Lv_PopUp_PopUp = base.transform.Find("LvInfo/Lv/PopUp/PopUp");
			_Locked = base.transform.Find("Locked");
			_LvInfo = base.transform.Find("LvInfo");
			_Output = base.transform.Find("Output");
			_LvInfo_Lv_Name = base.transform.Find("LvInfo/Lv/Name");
			_LvInfo_Lv_Lv = base.transform.Find("LvInfo/Lv/Lv");
			_LvInfo_LvUpProgress = base.transform.Find("LvInfo/LvUpProgress");
			_LvInfo_LvUpProgress_TimeRemain_HPSlider_Slider = base.transform.Find("LvInfo/LvUpProgress/TimeRemain/HPSlider/Slider");
			_LvInfo_LvUpProgress_TimeRemain_Time = base.transform.Find("LvInfo/LvUpProgress/TimeRemain/Time");
			_Day = _LvInfo_LvUpProgress_TimeRemain_Time.Find("Day");
			_Hrs = _LvInfo_LvUpProgress_TimeRemain_Time.Find("Hrs");
			_Min = _LvInfo_LvUpProgress_TimeRemain_Time.Find("Min");
			_Sec = _LvInfo_LvUpProgress_TimeRemain_Time.Find("Sec");
			_DayText = _LvInfo_LvUpProgress_TimeRemain_Time.Find("DayText");
			_HrsText = _LvInfo_LvUpProgress_TimeRemain_Time.Find("HrsText");
			_MinText = _LvInfo_LvUpProgress_TimeRemain_Time.Find("MinText");
			_SecText = _LvInfo_LvUpProgress_TimeRemain_Time.Find("SecText");
			_LvInfo_Lv_Name_TextComp = _LvInfo_Lv_Name.GetComponent<Text>();
			_LvInfo_Lv_Lv_TextComp = _LvInfo_Lv_Lv.GetComponent<Text>();
			_LvInfo_LvUpProgress_TimeRemain_HPSlider_Slider_ImageComp = _LvInfo_LvUpProgress_TimeRemain_HPSlider_Slider.GetComponent<Image>();
			_Day_TextComp = _Day.GetComponent<Text>();
			_Hrs_TextComp = _Hrs.GetComponent<Text>();
			_Min_TextComp = _Min.GetComponent<Text>();
			_Sec_TextComp = _Sec.GetComponent<Text>();
			_LvInfo_Button = base.transform.Find("LvInfo/Button");
			_LvInfo_Button_UI = _LvInfo_Button.GetComponent<Button>();
			BindViewCallback(_LvInfo_Button_UI, OnUIClick);
		}

		private void Update()
		{
			if (_isUpdating)
			{
				UpdateUIPosition();
				UpdateStatus();
				if (_cabinData.status == CabinStatus.UnLocked)
				{
					UpdateOutput();
					UpdateBackgroundVenture();
					UpdateLevelUp();
					UpdateCabinLevel();
				}
				_lastFrame_cabinStatus = _cabinData.status;
			}
		}

		private void UpdateUIPosition()
		{
			Vector3 worldToUIPosition = GetWorldToUIPosition(_target.transform.position + _offset);
			base.transform.position = worldToUIPosition;
		}

		private void UpdateStatus()
		{
			if (_lastFrame_cabinStatus != _cabinData.status)
			{
				_Locked.gameObject.SetActive(_cabinData.status == CabinStatus.Locked);
				_LvInfo.gameObject.SetActive(_cabinData.status == CabinStatus.UnLocked);
			}
		}

		private void UpdateCabinLevel()
		{
			_LvInfo_Lv_Lv_TextComp.text = "Lv." + _cabinData.level;
		}

		private void UpdateLevelUp()
		{
			bool flag = _cabinData.levelUpEndTime > TimeUtil.Now;
			if (_LvInfo_LvUpProgress.gameObject.activeSelf != flag)
			{
				_LvInfo_LvUpProgress.gameObject.SetActive(flag);
			}
			if (flag)
			{
				_isUpLevel = true;
				float fillAmount = (float)(_cabinData.levelUpEndTime - TimeUtil.Now).TotalSeconds / (float)_cabinData.GetCabinLevelUpTimeCost();
				_LvInfo_LvUpProgress_TimeRemain_HPSlider_Slider_ImageComp.fillAmount = fillAmount;
				UpdateRemainTime();
			}
			else if (_isUpLevel)
			{
				_isUpLevel = false;
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnCabinLevelUpSucc, _target));
				Singleton<NetworkManager>.Instance.RequestGetIsland();
			}
		}

		private void UpdateOutput()
		{
			CabinCollectDataItem cabinCollectDataItem = _cabinData as CabinCollectDataItem;
			if (cabinCollectDataItem != null)
			{
				bool flag = cabinCollectDataItem.HasScoin();
				if (_lastFrame_output_active != flag)
				{
					_Output.gameObject.SetActive(flag);
				}
				_lastFrame_output_active = flag;
				if (Time.time > _fetch_scoin_time + 2f && cabinCollectDataItem.TimeToFetch())
				{
					Singleton<NetworkManager>.Instance.RequestGetCollectCabin();
					_fetch_scoin_time = Time.time;
				}
			}
		}

		private void UpdateBackgroundVenture()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			if ((int)_cabinData.cabinType == 5 && Singleton<IslandModule>.Instance.RefreshVentureBackground())
			{
				Singleton<NetworkManager>.Instance.RequestIslandVenture();
				Singleton<IslandModule>.Instance.UnRegisterVentureInProgress();
			}
		}

		private Vector3 GetWorldToUIPosition(Vector3 worldPosition)
		{
			Vector3 position = _mainCamera.WorldToScreenPoint(worldPosition);
			position.z = _uiCamera.nearClipPlane + 0.1f;
			return _uiCamera.ScreenToWorldPoint(position);
		}

		private void SetUITimeFormat(E_TimeFormat timeFormat)
		{
			switch (timeFormat)
			{
			case E_TimeFormat.Day:
				_Day.gameObject.SetActive(true);
				_DayText.gameObject.SetActive(true);
				_Hrs.gameObject.SetActive(true);
				_HrsText.gameObject.SetActive(true);
				_Min.gameObject.SetActive(false);
				_MinText.gameObject.SetActive(false);
				_Sec.gameObject.SetActive(false);
				_SecText.gameObject.SetActive(false);
				break;
			case E_TimeFormat.Hour:
				_Day.gameObject.SetActive(false);
				_DayText.gameObject.SetActive(false);
				_Hrs.gameObject.SetActive(true);
				_HrsText.gameObject.SetActive(true);
				_Min.gameObject.SetActive(true);
				_MinText.gameObject.SetActive(true);
				_Sec.gameObject.SetActive(false);
				_SecText.gameObject.SetActive(false);
				break;
			default:
				_Day.gameObject.SetActive(false);
				_DayText.gameObject.SetActive(false);
				_Hrs.gameObject.SetActive(false);
				_HrsText.gameObject.SetActive(false);
				_Min.gameObject.SetActive(true);
				_MinText.gameObject.SetActive(true);
				_Sec.gameObject.SetActive(true);
				_SecText.gameObject.SetActive(true);
				break;
			}
		}

		private void UpdateRemainTime()
		{
			TimeSpan timeSpan = _cabinData.levelUpEndTime - TimeUtil.Now;
			_Day_TextComp.text = string.Format("{0:D2}", timeSpan.Days);
			_Hrs_TextComp.text = string.Format("{0:D2}", timeSpan.Hours);
			_Min_TextComp.text = string.Format("{0:D2}", timeSpan.Minutes);
			_Sec_TextComp.text = string.Format("{0:D2}", timeSpan.Seconds);
			E_TimeFormat timeFormat = GetTimeFormat(timeSpan);
			if (_lastFrame_timeFormat != timeFormat)
			{
				SetUITimeFormat(timeFormat);
			}
			_lastFrame_timeFormat = timeFormat;
		}

		private E_TimeFormat GetTimeFormat(TimeSpan timeSpan)
		{
			if (timeSpan.TotalDays >= 1.0)
			{
				return E_TimeFormat.Day;
			}
			if (timeSpan.TotalHours >= 1.0)
			{
				return E_TimeFormat.Hour;
			}
			return E_TimeFormat.Minute;
		}

		public void OnScoinBtnClick()
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandScoinBtnClick, _target));
		}

		public void OnUIClick()
		{
			_cameraSM.GotoState(E_IslandCameraState.ToLanded, _target);
		}

		private void BindViewCallback(Button button, UnityAction callback)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(callback);
		}

		private void UnbindViewCallback(Button button)
		{
			button.onClick.RemoveAllListeners();
		}

		private void OnDestroy()
		{
			UnbindViewCallback(_LvInfo_Button_UI);
		}
	}
}
