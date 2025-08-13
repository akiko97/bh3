using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class WwiseAudioManager
	{
		private struct SoundBankScale
		{
			public string[] soundBankNames;
		}

		public bool useImplicitLoading;

		private List<SoundBankScale> soundBankScaleStack = new List<SoundBankScale>();

		private List<string> manualPrepareBanks = new List<string>();

		private GameObject _defaultPlayObject;

		private Transform _followingTransform;

		private Vector3 _followingOffset = Vector3.zero;

		private GameObject _customListenerObject;

		private string _overridedLanguage;

		private GameObject defaultPlayObject
		{
			get
			{
				if (_defaultPlayObject == null)
				{
					_defaultPlayObject = new GameObject("_WwiseDefaultPlayer");
					UnityEngine.Object.DontDestroyOnLoad(_defaultPlayObject);
					_defaultPlayObject.AddComponent<BoxCollider>();
				}
				return _defaultPlayObject;
			}
		}

		private WwiseAudioManager()
		{
		}

		public void InitAtAwake()
		{
		}

		public void Destroy()
		{
			if (_defaultPlayObject != null)
			{
				UnityEngine.Object.Destroy(_defaultPlayObject);
			}
		}

		public void PushSoundBankScale(string[] soundBankNames)
		{
			if (soundBankNames != null)
			{
				SoundBankScale item = new SoundBankScale
				{
					soundBankNames = soundBankNames
				};
				int i = 0;
				for (int num = soundBankNames.Length; i < num; i++)
				{
					AkBankManager.LoadBank(soundBankNames[i]);
				}
				soundBankScaleStack.Insert(0, item);
			}
		}

		public void Core()
		{
			if (_followingTransform != null && _customListenerObject != null)
			{
				_customListenerObject.transform.position = _followingTransform.position + _followingOffset;
				_customListenerObject.transform.rotation = Camera.main.transform.rotation;
			}
		}

		public void SetListenerFollowing(Transform trans, Vector3 offset)
		{
			if (_customListenerObject == null)
			{
				_customListenerObject = new GameObject("_WwiseListener");
				_customListenerObject.AddComponent<AkAudioListener>();
				AkAudioListener component = Camera.main.GetComponent<AkAudioListener>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
			}
			_followingTransform = trans;
			_followingOffset = offset;
		}

		public void ResetListener()
		{
			if (!(_customListenerObject == null))
			{
				_followingTransform = null;
				_followingOffset = Vector3.zero;
				UnityEngine.Object.Destroy(_customListenerObject);
				_customListenerObject = null;
				Camera.main.gameObject.AddComponent<AkAudioListener>();
			}
		}

		public Transform GetFollowingTransform()
		{
			return _followingTransform;
		}

		public void PopSoundBankScale()
		{
			if (soundBankScaleStack.Count != 0)
			{
				SoundBankScale soundBankScale = soundBankScaleStack[0];
				int i = 0;
				for (int num = soundBankScale.soundBankNames.Length; i < num; i++)
				{
					AkBankManager.UnloadBank(soundBankScale.soundBankNames[i]);
				}
				soundBankScaleStack.RemoveAt(0);
			}
		}

		public void Post(string evtName, GameObject gameObj = null, AkCallbackManager.EventCallback endCallback = null, object cookie = null)
		{
			GameObject in_gameObjectID = ((!(gameObj == null)) ? gameObj : defaultPlayObject);
			uint num = ((endCallback != null) ? AkSoundEngine.PostEvent(evtName, in_gameObjectID, 1u, endCallback, cookie) : AkSoundEngine.PostEvent(evtName, in_gameObjectID));
			if (num == 0 && useImplicitLoading)
			{
				if (PrepareEvent(evtName))
				{
					num = ((endCallback != null) ? AkSoundEngine.PostEvent(evtName, in_gameObjectID, 1u, endCallback, cookie) : AkSoundEngine.PostEvent(evtName, in_gameObjectID));
				}
				if (num != 0)
				{
				}
			}
		}

		public void SetState(string machineName, string stateName)
		{
			AkSoundEngine.SetState(machineName, stateName);
		}

		public void SetParam(string paramName, float value)
		{
			AkSoundEngine.SetRTPCValue(paramName, value);
		}

		public void SetSwitch(string switchGroupName, string switchName)
		{
			AkSoundEngine.SetSwitch(switchGroupName, switchName, defaultPlayObject);
		}

		public void SetSwitch(string switchGroupName, string switchName, GameObject obj)
		{
			AkSoundEngine.SetSwitch(switchGroupName, switchName, obj);
		}

		private bool PrepareEvent(string name)
		{
			int num = name.IndexOf("_");
			if (num == -1)
			{
				return false;
			}
			string name2 = name.Substring(0, num);
			if (!LoadBankImplicit(name2))
			{
				return false;
			}
			return true;
		}

		private bool LoadBankImplicit(string name)
		{
			return false;
		}

		public void ClearImplicitLoadedBanks()
		{
		}

		public void ManualPrepareBank(string name)
		{
			if (!IsBankLoaded(name))
			{
				AkBankManager.LoadBank(name);
				manualPrepareBanks.Add(name);
			}
		}

		public void ManualUnloadBank(string name)
		{
			int i = 0;
			for (int count = manualPrepareBanks.Count; i < count; i++)
			{
				if (name == manualPrepareBanks[i])
				{
					AkBankManager.UnloadBank(name);
					manualPrepareBanks.RemoveAt(i);
					break;
				}
			}
		}

		public void ClearManualPrepareBank()
		{
			int i = 0;
			for (int count = manualPrepareBanks.Count; i < count; i++)
			{
				AkBankManager.UnloadBank(manualPrepareBanks[i]);
			}
			manualPrepareBanks.Clear();
		}

		public bool IsBankLoaded(string name)
		{
			int i = 0;
			for (int count = soundBankScaleStack.Count; i < count; i++)
			{
				int j = 0;
				for (int num = soundBankScaleStack[i].soundBankNames.Length; j < num; j++)
				{
					if (name == soundBankScaleStack[i].soundBankNames[j])
					{
						return true;
					}
				}
			}
			int k = 0;
			for (int count2 = manualPrepareBanks.Count; k < count2; k++)
			{
				if (name == manualPrepareBanks[k])
				{
					return true;
				}
			}
			return false;
		}

		public void ClearUp()
		{
			if (_defaultPlayObject != null)
			{
				UnityEngine.Object.Destroy(_defaultPlayObject);
				_defaultPlayObject = null;
			}
			int i = 0;
			for (int count = soundBankScaleStack.Count; i < count; i++)
			{
				SoundBankScale soundBankScale = soundBankScaleStack[i];
				int j = 0;
				for (int num = soundBankScale.soundBankNames.Length; j < num; j++)
				{
					AkBankManager.UnloadBank(soundBankScale.soundBankNames[j]);
				}
			}
			soundBankScaleStack.Clear();
			ClearManualPrepareBank();
		}

		public void StopAll()
		{
			AkSoundEngine.StopAll();
		}

		public void StopAll(GameObject gameObject)
		{
			AkSoundEngine.StopAll(gameObject);
		}

		public string GetLanguage()
		{
			return (_overridedLanguage == null) ? AkInitializer.GetCurrentLanguage() : _overridedLanguage;
		}

		public void SetLanguage(string lang)
		{
			string language = GetLanguage();
			if (language != null && language != lang)
			{
				AKRESULT aKRESULT = AkSoundEngine.SetCurrentLanguage(lang);
				if (aKRESULT == AKRESULT.AK_Success)
				{
					_overridedLanguage = lang;
					ReloadBanks();
				}
			}
		}

		public void ReloadBanks()
		{
			int i = 0;
			uint out_bankID;
			for (int count = soundBankScaleStack.Count; i < count; i++)
			{
				SoundBankScale soundBankScale = soundBankScaleStack[i];
				int j = 0;
				for (int num = soundBankScale.soundBankNames.Length; j < num; j++)
				{
					AkSoundEngine.UnloadBank(soundBankScale.soundBankNames[j], IntPtr.Zero, null, null);
					AkSoundEngine.LoadBank(soundBankScale.soundBankNames[j], -1, out out_bankID);
				}
			}
			int k = 0;
			for (int count2 = manualPrepareBanks.Count; k < count2; k++)
			{
				AkSoundEngine.UnloadBank(manualPrepareBanks[k], IntPtr.Zero, null, null);
				AkSoundEngine.LoadBank(manualPrepareBanks[k], -1, out out_bankID);
			}
		}
	}
}
