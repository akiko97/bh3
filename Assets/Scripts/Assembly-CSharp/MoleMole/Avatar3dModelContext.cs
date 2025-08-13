using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class Avatar3dModelContext : BaseWidgetContext
	{
		private Dictionary<int, Transform> _avatarModelDict;

		private Dictionary<int, float> _avatarDefaultYDict;

		private Coroutine _createAvatarCoroutine;

		private bool _setAvatarPosFlag = true;

		public Avatar3dModelContext(GameObject view = null)
		{
			config = new ContextPattern
			{
				contextName = "Avatar3dModelContext",
				viewPrefabPath = "UI/Menus/Widget/AvatarContainer",
				cacheType = ViewCacheType.DontCache,
				dontDestroyView = true
			};
			_avatarModelDict = new Dictionary<int, Transform>();
			_avatarDefaultYDict = new Dictionary<int, float>();
			base.view = view;
		}

		public Transform GetAvatarById(int avatarID)
		{
			return _avatarModelDict[avatarID];
		}

		public bool ContainUIAvatar(int avatarID)
		{
			return _avatarModelDict.ContainsKey(avatarID);
		}

		public List<Transform> GetAllAvatars()
		{
			List<Transform> list = new List<Transform>();
			foreach (Transform value in _avatarModelDict.Values)
			{
				list.Add(value);
			}
			return list;
		}

		public void SetStandOnSpaceship(int avatarID)
		{
			Transform transform = _avatarModelDict[avatarID];
			transform.GetComponent<BaseMonoUIAvatar>().standOnSpaceshipInGameEntry = true;
		}

		public void TriggerAvatarTurnAround(int avatarID)
		{
			Transform transform = _avatarModelDict[avatarID];
			transform.GetComponent<Animator>().SetTrigger("TriggerTurnAround");
			Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasShowAvatarTurnAroundAnim = true;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.CreateAvatarUIModels)
			{
				return OnRecvCreateAvatarNotify((List<Avatar3dModelDataItem>)ntf.body);
			}
			if (ntf.type == NotifyTypes.SetSpaceShipActive)
			{
				return OnSetSpaceShipActive(((Tuple<bool, bool>)ntf.body).Item1, ((Tuple<bool, bool>)ntf.body).Item2);
			}
			if (ntf.type == NotifyTypes.PlayAvtarChangeEffect)
			{
				return PlayAvtarChangeEffect();
			}
			return false;
		}

		protected override bool SetupView()
		{
			base.view.name = "AvatarContainer";
			SetLockViewActive(false);
			base.view.transform.Find("AvatarChangeEffect").gameObject.SetActive(false);
			return false;
		}

		public void TriggerStartGalTouch(int avatarID)
		{
			Transform transform = _avatarModelDict[avatarID];
			BaseMonoUIAvatar component = transform.GetComponent<BaseMonoUIAvatar>();
			if (component != null)
			{
				component.EnterGalTouch();
			}
		}

		public void TriggerStartGalTouch()
		{
			List<Transform> allAvatars = GetAllAvatars();
			int i = 0;
			for (int count = allAvatars.Count; i < count; i++)
			{
				BaseMonoUIAvatar component = allAvatars[i].GetComponent<BaseMonoUIAvatar>();
				if (component != null)
				{
					component.EnterGalTouch();
				}
			}
		}

		public void TriggerStopGalTouch()
		{
			foreach (Transform value in _avatarModelDict.Values)
			{
				BaseMonoUIAvatar component = value.GetComponent<BaseMonoUIAvatar>();
				if (component != null)
				{
					component.ExitGalTouch();
				}
			}
		}

		private bool OnSetSpaceShipActive(bool active, bool setCameraComponentOnly = false)
		{
			base.view.SetActive(active);
			return false;
		}

		private bool OnRecvCreateAvatarNotify(List<Avatar3dModelDataItem> avatarDataList)
		{
			if (_createAvatarCoroutine != null)
			{
				Singleton<ApplicationManager>.Instance.StopCoroutine(_createAvatarCoroutine);
			}
			_createAvatarCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(DoCreateAvatarUIModels(avatarDataList));
			return false;
		}

		private bool PlayAvtarChangeEffect()
		{
			base.view.transform.Find("AvatarChangeEffect").gameObject.SetActive(true);
			ParticleSystem component = base.view.transform.Find("AvatarChangeEffect").GetComponent<ParticleSystem>();
			component.Play();
			return false;
		}

		private string GetAvatarPrefaPath(string avatarRegistryKey)
		{
			return string.Format("Entities/Avatar/{0}/Avatar_{0}_UI", avatarRegistryKey);
		}

		private IEnumerator DoCreateAvatarUIModels(List<Avatar3dModelDataItem> avatarDataList)
		{
			List<int> removeKeys = new List<int>();
			foreach (int avatarId in _avatarModelDict.Keys)
			{
				if (avatarDataList.Find((Avatar3dModelDataItem x) => x.avatar.avatarID == avatarId) == null)
				{
					removeKeys.Add(avatarId);
				}
			}
			foreach (int key in removeKeys)
			{
				if (_avatarModelDict[key] != null)
				{
					Object.Destroy(_avatarModelDict[key].gameObject);
				}
				_avatarModelDict.Remove(key);
				_avatarDefaultYDict.Remove(key);
			}
			foreach (Avatar3dModelDataItem data in avatarDataList)
			{
				Transform modelTrans = null;
				bool needSetTriggerFlag = true;
				MonoGameEntry monoGameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
				if (_avatarModelDict.ContainsKey(data.avatar.avatarID))
				{
					modelTrans = _avatarModelDict[data.avatar.avatarID];
					needSetTriggerFlag = false;
				}
				else
				{
					BaseMonoUIAvatar monoAvatar = base.view.GetComponentInChildren<BaseMonoUIAvatar>();
					bool loadFlag = true;
					if (monoAvatar != null)
					{
						if (monoAvatar.avatarID == data.avatar.avatarID)
						{
							modelTrans = monoAvatar.gameObject.transform;
							_setAvatarPosFlag = false;
							loadFlag = false;
						}
						else
						{
							Object.DestroyImmediate(monoAvatar.gameObject);
							_setAvatarPosFlag = true;
						}
					}
					if (loadFlag)
					{
						modelTrans = Object.Instantiate(Miscs.LoadResource<GameObject>(GetAvatarPrefaPath(data.avatar.AvatarRegistryKey))).transform;
						modelTrans.SetParent(base.view.transform);
						monoAvatar = modelTrans.GetComponent<BaseMonoUIAvatar>();
						if (monoGameEntry != null)
						{
							Object.DontDestroyOnLoad(base.view);
						}
					}
					monoAvatar.avatarData = data.avatar;
					monoAvatar.tattooVisible = false;
					monoAvatar.SetTattooVisible(0);
					monoAvatar.Init(data.avatar.avatarID);
					_avatarModelDict.Add(data.avatar.avatarID, modelTrans);
					_avatarDefaultYDict.Add(data.avatar.avatarID, modelTrans.localPosition.y);
					if (monoGameEntry == null)
					{
						modelTrans.GetComponent<BaseMonoUIAvatar>().standOnSpaceshipInGameEntry = false;
					}
				}
				SetAvatarModelView(modelTrans, data.avatar, data.pos, data.eulerAngles, data.showLockViewIfLock);
				if (needSetTriggerFlag)
				{
					if (monoGameEntry != null)
					{
						modelTrans.GetComponent<Animator>().SetTrigger("TriggerStandByBack");
					}
					else
					{
						modelTrans.GetComponent<Animator>().SetTrigger("TriggerStandBy");
					}
					if (monoGameEntry == null && !Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasShowAvatarTurnAroundAnim)
					{
						modelTrans.GetComponent<Animator>().SetTrigger("TriggerTurnAround");
						Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasShowAvatarTurnAroundAnim = true;
					}
				}
			}
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.FinishCreateAvatarUIModels));
			yield return null;
		}

		private void SetAvatarModelView(Transform modelTrans, AvatarDataItem avatarData, Vector3 pos, Vector3 eulerAngles, bool showLockViewIfLock)
		{
			if (_setAvatarPosFlag)
			{
				modelTrans.SetLocalPositionX(pos.x);
				modelTrans.SetLocalPositionZ(pos.z);
				modelTrans.SetLocalPositionY(_avatarDefaultYDict[avatarData.avatarID] + pos.y);
				modelTrans.GetComponent<BaseMonoUIAvatar>().SetOriginPos(modelTrans.position);
				MonoGameEntry monoGameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
				if (monoGameEntry != null)
				{
					modelTrans.Translate(0f, 0f, 20f);
				}
			}
			modelTrans.eulerAngles = eulerAngles;
			bool lockViewActive = showLockViewIfLock && !avatarData.UnLocked;
			SetLockViewActive(lockViewActive);
			Transform transform = base.view.transform.Find("Lock");
			transform.position = new Vector3(modelTrans.position.x, transform.position.y, modelTrans.position.z);
			SetAvatarAttachWeaponView(modelTrans, avatarData);
		}

		private void SetLockViewActive(bool active)
		{
			base.view.transform.Find("Lock").gameObject.SetActive(active);
		}

		private void SetAvatarAttachWeaponView(Transform modelTrans, AvatarDataItem avatarData)
		{
			WeaponDataItem weapon = avatarData.GetWeapon();
			if (weapon != null)
			{
				BaseMonoUIAvatar component = modelTrans.GetComponent<BaseMonoUIAvatar>();
				if (component.WeaponMetaID != weapon.ID)
				{
					component.AttachWeapon(weapon.ID, avatarData.AvatarRegistryKey);
				}
			}
		}
	}
}
