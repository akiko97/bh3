using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class CabinOverviewPageContext : BasePageContext
	{
		private CabinDataItemBase _cabinData;

		private Dictionary<MonoIslandBuilding, CabinDataItemBase> _buildingDataDict;

		private Dictionary<CabinType, MonoIslandBuilding> _data2BuildingDict;

		private Camera _mainCamera;

		private Camera _uiCamera;

		private CanvasTimer _pageFadeOutTimer;

		private int PAGE_FADE_OUT_TRIGGER_ID = Animator.StringToHash("PageFadeOut");

		private bool _waitingForVentureList;

		private bool _cameraFocusEnd;

		public CabinOverviewPageContext(CabinDataItemBase cabinData, Dictionary<MonoIslandBuilding, CabinDataItemBase> buildingDataDict)
		{
			config = new ContextPattern
			{
				contextName = "CabinOverviewPageContext",
				viewPrefabPath = "UI/Menus/Page/Island/IslandCabinOverviewPage",
				cacheType = ViewCacheType.AlwaysCached
			};
			_cabinData = cabinData;
			_buildingDataDict = buildingDataDict;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.OnIslandCameraPreLanded)
			{
				return OnIslandCameraLanded((MonoIslandBuilding)ntf.body);
			}
			if (ntf.type == NotifyTypes.OnIslandCameraPreFocus)
			{
				return OnIslandCameraFocus();
			}
			if (ntf.type == NotifyTypes.OnCabinBeginExtend)
			{
				return OnBeginExtend((int)ntf.body);
			}
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			ushort cmdId = pkt.getCmdId();
			switch (cmdId)
			{
			case 157:
				return DoSetupView();
			case 169:
				if ((int)_cabinData.cabinType == 5 && _waitingForVentureList)
				{
					_waitingForVentureList = false;
					if (_cameraFocusEnd)
					{
						return DoEnterCabin();
					}
				}
				break;
			}
			if (cmdId == 163)
			{
				OnFinishCabinLevelUpRsp(pkt.getData<FinishCabinLevelUpRsp>());
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpBtn/Btn").GetComponent<Button>(), OnLevelUpBtnClick);
			BindViewCallback(base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpStatus/Btn").GetComponent<Button>(), OnFinishLevelUpNowBtnClick);
			BindViewCallback(base.view.transform.Find("ActionPanel/EnhanceBtns/ExtendBtn/Btn").GetComponent<Button>(), OnExtendBtnClick);
			BindViewCallback(base.view.transform.Find("ActionPanel/EnterCabinBtn/EnterCabinBtn").GetComponent<Button>(), OnEnterCabinBtnClick);
		}

		protected override bool SetupView()
		{
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			_mainCamera = GameObject.Find("IslandCameraGroup/MainCamera").GetComponent<Camera>();
			_uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
			_cameraFocusEnd = false;
			foreach (Transform item in base.view.transform.Find("EffectContainer"))
			{
				ParticleSystem[] componentsInChildren = item.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.Stop();
				}
			}
			Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowLevelUpCompleteSet[_cabinData.cabinType] = false;
			Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowNewUnlockDict[_cabinData.cabinType] = false;
			Singleton<MiHoYoGameData>.Instance.Save();
			if (_data2BuildingDict == null)
			{
				_data2BuildingDict = new Dictionary<CabinType, MonoIslandBuilding>();
				foreach (KeyValuePair<MonoIslandBuilding, CabinDataItemBase> item2 in _buildingDataDict)
				{
					_data2BuildingDict[item2.Value.cabinType] = item2.Key;
				}
			}
			DoSetupView();
			return false;
		}

		public override void BackToMainMenuPage()
		{
			Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true);
		}

		private void OnLevelUpBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new CabinEnhanceDialogContext(_cabinData, CainEnhanceType.LevelUp));
		}

		private void OnFinishLevelUpNowBtnClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
			{
				type = GeneralDialogContext.ButtonType.DoubleButton,
				title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
				desc = LocalizationGeneralLogic.GetText("Menu_Desc_ConfirmToFinishCabinLevelUpNow", Singleton<IslandModule>.Instance.GetFinishLevelUpNowHcoinCost((int)(_cabinData.levelUpEndTime - TimeUtil.Now).TotalSeconds)),
				buttonCallBack = OnConfirmToFinishLevelUpBtnClick
			});
		}

		private void OnExtendBtnClick()
		{
			int level = _cabinData.level;
			int cabinMaxLevel = _cabinData.GetCabinMaxLevel();
			if (level < cabinMaxLevel)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_CabinExtendFailLevelLow")
				});
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new CabinEnhanceDialogContext(_cabinData, CainEnhanceType.Extend));
			}
		}

		private void OnEnterCabinBtnClick()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Invalid comparison between Unknown and I4
			_cameraFocusEnd = false;
			GameObject.Find("IslandCameraGroup").GetComponent<MonoIslandCameraSM>().ToFocusing();
			if ((int)_cabinData.cabinType == 5)
			{
				_waitingForVentureList = true;
				Singleton<NetworkManager>.Instance.RequestIslandVenture();
			}
		}

		private bool OnIslandCameraLanded(MonoIslandBuilding buidling)
		{
			CabinDataItemBase cabinDataItemBase = _buildingDataDict[buidling];
			if (cabinDataItemBase.status == CabinStatus.UnLocked)
			{
				_cabinData = cabinDataItemBase;
				SetupView();
			}
			else
			{
				BackPage();
			}
			return false;
		}

		private bool OnIslandCameraFocus()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			_cameraFocusEnd = true;
			if ((int)_cabinData.cabinType == 5 && _waitingForVentureList)
			{
				Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(169));
			}
			else
			{
				DoEnterCabin();
			}
			return false;
		}

		private bool DoEnterCabin()
		{
			if (_pageFadeOutTimer != null)
			{
				_pageFadeOutTimer.Destroy();
			}
			_pageFadeOutTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.5f, 0f);
			_pageFadeOutTimer.timeUpCallback = EnterCabin;
			base.view.transform.GetComponent<Animator>().SetTrigger(PAGE_FADE_OUT_TRIGGER_ID);
			return false;
		}

		private bool OnBeginExtend(int extendGrade)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected I4, but got Unknown
			MonoIslandBuilding monoIslandBuilding = _data2BuildingDict[_cabinData.cabinType];
			CabinExtendGradeMetaData cabinExtendGradeMetaDataByKey = CabinExtendGradeMetaDataReader.GetCabinExtendGradeMetaDataByKey((int)_cabinData.cabinType, extendGrade);
			monoIslandBuilding.UpdateBuildingWhenExtend(cabinExtendGradeMetaDataByKey.buildingPath);
			monoIslandBuilding.AddHighLightMat(null);
			monoIslandBuilding.SetHighLightAlpha(1f);
			monoIslandBuilding.SetRenderQueue(E_IslandRenderQueue.Front);
			monoIslandBuilding.SetPolygonOffset(monoIslandBuilding.highlight_polygon_offset);
			Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Building_Extension");
			PlayEffect(base.view.transform.Find("EffectContainer/IslandCabinExtend"), true);
			return false;
		}

		private void EnterCabin()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new CabinDetailPageContext(_cabinData));
		}

		private bool OnFinishCabinLevelUpRsp(FinishCabinLevelUpRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Building_Upgrade");
				PlayEffect(base.view.transform.Find("EffectContainer/IslandCabinLvUp"));
				Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowLevelUpCompleteSet[_cabinData.cabinType] = false;
				Singleton<MiHoYoGameData>.Instance.Save();
			}
			return false;
		}

		private void UpdateProgressBarForLevelUp()
		{
			float value = (float)(_cabinData.levelUpEndTime - TimeUtil.Now).TotalSeconds / (float)_cabinData.GetCabinLevelUpTimeCost();
			base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpStatus/Time/Slider").GetComponent<Slider>().value = value;
		}

		private void OnLevelUpTimeOut()
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			PlayEffect(base.view.transform.Find("EffectContainer/IslandCabinLvUp"));
			Singleton<NetworkManager>.Instance.RequestGetIsland();
			Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowLevelUpCompleteSet[_cabinData.cabinType] = false;
			Singleton<MiHoYoGameData>.Instance.Save();
		}

		private void OnConfirmToFinishLevelUpBtnClick(bool confirmed)
		{
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			if (!confirmed)
			{
				return;
			}
			if (_cabinData.levelUpEndTime <= TimeUtil.Now)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_AlreadyFinishLevelUp")));
				return;
			}
			int finishLevelUpNowHcoinCost = Singleton<IslandModule>.Instance.GetFinishLevelUpNowHcoinCost((int)(_cabinData.levelUpEndTime - TimeUtil.Now).TotalSeconds);
			if (finishLevelUpNowHcoinCost > Singleton<PlayerModule>.Instance.playerData.hcoin)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("10029")));
			}
			else
			{
				Singleton<NetworkManager>.Instance.RequestFinishCabinLevelUp(_cabinData.cabinType);
			}
		}

		private Vector3 GetWorldToUIPosition(Vector3 worldPosition)
		{
			Vector3 position = _mainCamera.WorldToScreenPoint(worldPosition);
			position.z = Mathf.Clamp(position.z, _uiCamera.nearClipPlane, _uiCamera.farClipPlane);
			return _uiCamera.ScreenToWorldPoint(position);
		}

		private void PlayEffect(Transform effectTrans, bool bBuildingPos = false)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (bBuildingPos)
			{
				MonoIslandBuilding monoIslandBuilding = _data2BuildingDict[_cabinData.cabinType];
				effectTrans.position = monoIslandBuilding.transform.position;
			}
			ParticleSystem[] componentsInChildren = effectTrans.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Play();
			}
		}

		private bool DoSetupView()
		{
			base.view.transform.Find("InfoPanel/Info/CabinName").GetComponent<Text>().text = _cabinData.GetCabinName();
			bool flag = _cabinData.status == CabinStatus.Locked;
			base.view.transform.Find("ActionPanel/EnhanceBtns").gameObject.SetActive(!flag);
			base.view.transform.Find("ActionPanel/EnterCabinBtn/EnterCabinBtn").GetComponent<Button>().interactable = !flag;
			base.view.transform.Find("InfoPanel/Info/Lv").gameObject.SetActive(!flag);
			if (!flag)
			{
				base.view.transform.Find("InfoPanel/Info/Lv/Lv").GetComponent<Text>().text = "Lv." + _cabinData.level;
				base.view.transform.Find("InfoPanel/Info/Lv/Max").GetComponent<Text>().text = "/" + _cabinData.GetCabinMaxLevel();
				bool flag2 = _cabinData.levelUpEndTime > TimeUtil.Now;
				base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpStatus").gameObject.SetActive(flag2);
				base.view.transform.Find("ActionPanel/EnhanceBtns/ExtendBtn/ExtendLevel/Level").GetComponent<MonoCabinExtendGrade>().SetupView(_cabinData.extendGrade);
				base.view.transform.Find("ActionPanel/EnhanceBtns/ExtendBtn/Btn").GetComponent<Button>().interactable = _cabinData.CanExtendCabin();
				base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpBtn").gameObject.SetActive(!flag2);
				base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpBtn/Btn").GetComponent<Button>().interactable = _cabinData.CanUpLevel();
				if (_cabinData.CanUpLevel())
				{
					base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpBtn/Time/Time").GetComponent<MonoRemainTimer>().SetTargetTime(_cabinData.GetCabinLevelUpTimeCost());
				}
				else
				{
					base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpBtn/Time").gameObject.SetActive(false);
				}
				base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpBtn/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_LevelUp");
				if (flag2)
				{
					base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpStatus/Time/RemainTime/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(_cabinData.levelUpEndTime, UpdateProgressBarForLevelUp, OnLevelUpTimeOut);
				}
			}
			base.view.transform.Find("ActionPanel/UnlockCondition").gameObject.SetActive(flag);
			if (flag)
			{
				base.view.transform.Find("ActionPanel/UnlockCondition/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinUnlockNeedLevel", _cabinData.GetUnlockPlayerLevel());
			}
			return false;
		}
	}
}
