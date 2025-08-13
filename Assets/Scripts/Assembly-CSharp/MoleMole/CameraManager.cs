using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class CameraManager
	{
		private uint _mainCameraRuntimeID;

		private uint _inlevelCameraRuntimeID;

		private Dictionary<uint, BaseMonoCamera> _cameraDict;

		public bool controlledRotateKeepManual { get; set; }

		private CameraManager()
		{
			_cameraDict = new Dictionary<uint, BaseMonoCamera>();
			_mainCameraRuntimeID = 0u;
			_inlevelCameraRuntimeID = 0u;
		}

		public void InitAtAwake()
		{
		}

		public void InitAtStart()
		{
			Singleton<CameraManager>.Instance.CreateCamera(1u);
			Singleton<CameraManager>.Instance.CreateCamera(2u);
			Singleton<CameraManager>.Instance.GetMainCamera().gameObject.SetActive(false);
			Singleton<CameraManager>.Instance.GetInLevelUICamera().gameObject.SetActive(false);
		}

		public uint CreateCamera(uint cameraType)
		{
			BaseMonoCamera baseMonoCamera = null;
			GameObject gameObject = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(CameraData.GetPrefabResPath(cameraType)));
			baseMonoCamera = gameObject.GetComponent<BaseMonoCamera>();
			uint nextRuntimeID = Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(2);
			RegisterCameraData(cameraType, baseMonoCamera, nextRuntimeID);
			switch (cameraType)
			{
			case 1u:
				((MonoMainCamera)baseMonoCamera).Init(nextRuntimeID);
				break;
			case 2u:
				((MonoInLevelUICamera)baseMonoCamera).Init(nextRuntimeID);
				break;
			default:
				throw new Exception("Invalid Type or State!");
			}
			return baseMonoCamera.GetRuntimeID();
		}

		public uint CreateCamera(uint cameraType, uint followEntityRuntimeID, uint followMode)
		{
			BaseMonoCamera baseMonoCamera = null;
			return baseMonoCamera.GetRuntimeID();
		}

		public void RegisterCameraData(uint cameraType, BaseMonoCamera camera, uint runtimeID)
		{
			_cameraDict.Add(runtimeID, camera);
			switch (cameraType)
			{
			case 1u:
				_mainCameraRuntimeID = runtimeID;
				break;
			case 2u:
				_inlevelCameraRuntimeID = runtimeID;
				break;
			}
		}

		public void Core()
		{
		}

		public List<BaseMonoCamera> GetAllCameras()
		{
			List<BaseMonoCamera> list = new List<BaseMonoCamera>();
			list.AddRange(_cameraDict.Values);
			return list;
		}

		public BaseMonoCamera GetCameraByRuntimeID(uint runtimeID)
		{
			return _cameraDict[runtimeID];
		}

		public MonoMainCamera GetMainCamera()
		{
			return (MonoMainCamera)_cameraDict[_mainCameraRuntimeID];
		}

		public MonoInLevelUICamera GetInLevelUICamera()
		{
			return (MonoInLevelUICamera)_cameraDict[_inlevelCameraRuntimeID];
		}

		public BaseMonoCamera GetCameraByFollowEntityRuntimeID(uint entityRuntimeID)
		{
			return Singleton<CameraManager>.Instance.GetMainCamera();
		}

		public bool RemoveCameraByRuntimeID(uint runtimeID)
		{
			Singleton<EventManager>.Instance.TryRemoveActor(runtimeID);
			UnityEngine.Object.Destroy(_cameraDict[runtimeID].gameObject);
			return _cameraDict.Remove(runtimeID);
		}

		public void RemoveAllCameras()
		{
			List<uint> list = new List<uint>();
			foreach (KeyValuePair<uint, BaseMonoCamera> item in _cameraDict)
			{
				list.Add(item.Key);
			}
			foreach (uint item2 in list)
			{
				RemoveCameraByRuntimeID(item2);
			}
		}

		public void EnableBossCamera(uint targetId)
		{
			BaseMonoEntity monsterByRuntimeID = Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(targetId);
			if (monsterByRuntimeID != null)
			{
				MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
				mainCamera.followState.TransitBaseState(mainCamera.followState.followAvatarAndBossState);
				mainCamera.followState.followAvatarAndBossState.bossTarget = monsterByRuntimeID;
				if (!mainCamera.followState.isCameraLocateRatioUserDefined)
				{
					mainCamera.SetCameraLocateRatio(0.735f);
				}
			}
		}

		public void DisableBossCamera()
		{
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			mainCamera.followState.TransitBaseState(mainCamera.followState.followAvatarState, true);
			if (!mainCamera.followState.isCameraLocateRatioUserDefined)
			{
				mainCamera.SetCameraLocateRatio(0.535f);
			}
		}

		public void EnableCrowdCamera()
		{
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			mainCamera.followState.TransitBaseState(mainCamera.followState.followAvatarAndCrowdState);
		}

		public void DisableCrowdCamera()
		{
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			mainCamera.followState.TransitBaseState(mainCamera.followState.followAvatarState, true);
		}
	}
}
