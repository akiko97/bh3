using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class IslandMainPageContext : BasePageContext
	{
		private const string CABIN_BASE_INFO_PREFAB_PATH = "UI/Menus/Widget/Island/CabinInfoUI";

		private Dictionary<MonoIslandBuilding, CabinDataItemBase> _buildingDataDict;

		private Dictionary<CabinType, MonoIslandBuilding> _cabinBuildingDict;

		private Camera _mainCamera;

		private Camera _uiCamera;

		private Vector3 _offset = new Vector3(0f, 10f, 0f);

		private float FETCH_SCOIN_MISSION_RATIO_TOTAL = 200f;

		private CanvasTimer _cabinLevelUpEffectDelayTimer;

		public IslandMainPageContext(GameObject view, Dictionary<MonoIslandBuilding, CabinDataItemBase> buildingDataDict)
		{
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			config = new ContextPattern
			{
				contextName = "IslandMainPageContext",
				viewPrefabPath = "UI/Menus/Page/Island/IslandMainPage",
				cacheType = ViewCacheType.AlwaysCached
			};
			base.view = view;
			_buildingDataDict = buildingDataDict;
			_cabinBuildingDict = new Dictionary<CabinType, MonoIslandBuilding>();
			foreach (KeyValuePair<MonoIslandBuilding, CabinDataItemBase> item in _buildingDataDict)
			{
				_cabinBuildingDict[item.Value.cabinType] = item.Key;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.OnIslandCameraPreLanded)
			{
				return OnIslandCameraLanded((MonoIslandBuilding)ntf.body);
			}
			if (ntf.type == NotifyTypes.OnCabinLevelUpSucc)
			{
				return OnCabinLevelUpSucc((MonoIslandBuilding)ntf.body);
			}
			if (ntf.type == NotifyTypes.OnIslandScoinBtnClick)
			{
				return OnScoinBtnClick((MonoIslandBuilding)ntf.body);
			}
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 182)
			{
				OnIslandCollectRsp(pkt.getData<IslandCollectRsp>());
			}
			if (cmdId == 169)
			{
				return OnGetIslandVentureRsp(pkt.getData<GetIslandVentureRsp>());
			}
			return false;
		}

		public override void StartUp(Transform canvasTrans, Transform viewParent = null)
		{
			base.StartUp(canvasTrans, viewParent);
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("ControllPanel/CamResetBtn").GetComponent<Button>(), OnCamResetBtnClick);
			BindViewCallback(base.view.transform.Find("ControllPanel/CamZoomOutBtn").GetComponent<Button>(), OnCamZoomOutBtnClick);
		}

		protected override bool SetupView()
		{
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Expected I4, but got Unknown
			_mainCamera = GameObject.Find("IslandCameraGroup/MainCamera").GetComponent<Camera>();
			_uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
			foreach (Transform item in base.view.transform.Find("EffectContainer"))
			{
				ParticleSystem[] componentsInChildren = item.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.Stop();
				}
			}
			Singleton<NetworkManager>.Instance.RequestGetCollectCabin();
			Transform transform2 = base.view.transform.Find("CabinBaseInfoPanel");
			transform2.DestroyChildren();
			foreach (KeyValuePair<MonoIslandBuilding, CabinDataItemBase> item2 in _buildingDataDict)
			{
				Transform transform3 = Object.Instantiate(Resources.Load<GameObject>("UI/Menus/Widget/Island/CabinInfoUI")).transform;
				transform3.SetParent(transform2, false);
				transform3.GetComponent<MonoCabinMainInfo>().BindingTargetBuilding(item2.Key, item2.Value);
				transform3.gameObject.name = string.Format("CabinInfoUI_{0}", (int)item2.Value.cabinType);
			}
			KeyValuePair<MonoIslandBuilding, CabinDataItemBase> pair;
			foreach (KeyValuePair<MonoIslandBuilding, CabinDataItemBase> item3 in _buildingDataDict)
			{
				pair = item3;
				if (pair.Value.NeedToShowLevelUpComplete())
				{
					_cabinLevelUpEffectDelayTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.5f, 0f);
					_cabinLevelUpEffectDelayTimer.timeUpCallback = delegate
					{
						ShowCabinLevelUpCompleteEffect(pair.Key, pair.Value);
					};
					_cabinLevelUpEffectDelayTimer.StartRun();
					break;
				}
			}
			return false;
		}

		public override void BackPage()
		{
			Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true);
		}

		public override void BackToMainMenuPage()
		{
			Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true);
		}

		public override void OnLandedFromBackPage()
		{
			GameObject.Find("IslandCameraGroup").GetComponent<MonoIslandCameraSM>().BackToBase();
			foreach (Transform item in base.view.transform.Find("EffectContainer"))
			{
				ParticleSystem[] componentsInChildren = item.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.Stop();
				}
			}
			Transform transform2 = base.view.transform.Find("CabinBaseInfoPanel");
			foreach (Transform item2 in transform2)
			{
				item2.GetComponent<MonoCabinMainInfo>().ReStart();
			}
		}

		public override void SetActive(bool enabled)
		{
			_notifyQueue.Clear();
			base.SetActive(enabled);
		}

		private void OnCamResetBtnClick()
		{
			GameObject.Find("IslandCameraGroup").GetComponent<MonoIslandCameraSM>().CameraToBasePos();
		}

		private void OnCamZoomOutBtnClick()
		{
			GameObject.Find("IslandCameraGroup").GetComponent<MonoIslandCameraSM>().ExitFocusing();
		}

		private bool OnIslandCameraLanded(MonoIslandBuilding building)
		{
			Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Building_ZoomIn");
			Singleton<MainUIManager>.Instance.ShowPage(new CabinOverviewPageContext(_buildingDataDict[building], _buildingDataDict));
			return false;
		}

		private bool OnCabinLevelUpSucc(MonoIslandBuilding building)
		{
			PlayEffect(base.view.transform.Find("EffectContainer/IslandCabinLvUp"), building, true);
			return false;
		}

		private bool OnScoinBtnClick(MonoIslandBuilding building)
		{
			Singleton<NetworkManager>.Instance.RequestIslandCollect();
			return false;
		}

		private bool OnGetIslandVentureRsp(GetIslandVentureRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Transform transform = base.view.transform.Find("CabinBaseInfoPanel");
				foreach (Transform item in transform)
				{
					item.GetComponent<MonoCabinMainInfo>().RefreshPopUp();
				}
			}
			return false;
		}

		private bool OnIslandCollectRsp(IslandCollectRsp rsp)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Transform transform = base.view.transform.Find("EffectContainer/IslandCollectionGoldCoin");
				int fetchScoin = (int)rsp.add_scoin;
				CabinCollectDataItem cabinCollectDataItem = _buildingDataDict[_cabinBuildingDict[(CabinType)3]] as CabinCollectDataItem;
				float burstRate = ((!rsp.is_extraSpecified || !rsp.is_extra) ? 1f : cabinCollectDataItem.crtExtraRatio);
				ParticleSystem component = transform.GetComponent<ParticleSystem>();
				ParticleSystem.EmissionModule emission = component.emission;
				emission.rate = new ParticleSystem.MinMaxCurve
				{
					constantMax = Mathf.Clamp((float)fetchScoin / cabinCollectDataItem.topLimit, 0.1f, 1f) * FETCH_SCOIN_MISSION_RATIO_TOTAL
				};
				Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Collect_Gold");
				PlayEffect(transform, _cabinBuildingDict[(CabinType)3], true);
				CanvasTimer canvasTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.2f, 0f);
				canvasTimer.timeUpCallback = delegate
				{
					ShowGetScoinHintDialog(fetchScoin, burstRate, rsp.drop_item_list);
				};
			}
			return false;
		}

		private Vector3 GetWorldToUIPosition(Vector3 worldPosition)
		{
			Vector3 position = _mainCamera.WorldToScreenPoint(worldPosition);
			position.z = Mathf.Clamp(position.z, _uiCamera.nearClipPlane, _uiCamera.farClipPlane);
			return _uiCamera.ScreenToWorldPoint(position);
		}

		private void PlayEffect(Transform effectTrans, MonoIslandBuilding building, bool toUIPos = false)
		{
			effectTrans.position = ((!toUIPos) ? building.transform.position : GetWorldToUIPosition(building.transform.position + _offset));
			ParticleSystem[] componentsInChildren = effectTrans.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Play();
			}
		}

		private void ShowGetScoinHintDialog(int scoinNum, float burstRate, List<DropItem> dropItems)
		{
			if (Singleton<MainUIManager>.Instance.SceneCanvas != null)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new IslandCollectGotDialogContext(scoinNum, burstRate, dropItems));
			}
		}

		private void ShowCabinLevelUpCompleteEffect(MonoIslandBuilding target, CabinDataItemBase cabinData)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnCabinLevelUpSucc, target));
			Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowLevelUpCompleteSet[cabinData.cabinType] = false;
			Singleton<MiHoYoGameData>.Instance.Save();
			_cabinLevelUpEffectDelayTimer.Destroy();
		}
	}
}
