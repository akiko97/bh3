using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MoleMole
{
	public class ApplicationManager
	{
		private GameObject _go;

		private MonoApplicationBehaviour _applicationBehaviour;

		private List<Coroutine> _coroutines;

		private int _manualCoroutineID;

		private List<Tuple<int, IEnumerator>> _manualCoroutines;

		public MonoApplicationBehaviour applicationBehaviour
		{
			get
			{
				return _applicationBehaviour;
			}
		}

		public ApplicationManager()
		{
			if (_go != null)
			{
				UnityEngine.Object.Destroy(_go);
			}
			_go = new GameObject("ApplicationManagerGO");
			UnityEngine.Object.DontDestroyOnLoad(_go);
			_applicationBehaviour = _go.AddComponent<MonoApplicationBehaviour>();
			_applicationBehaviour.Init(this);
			_go.AddComponent<MonoTalkingData>();
			_go.AddComponent<MonoNotificationServices>();
			_go.AddComponent<AntiCheatPlugin>();
			_coroutines = new List<Coroutine>();
			_manualCoroutines = new List<Tuple<int, IEnumerator>>();
		}

		public void Destroy()
		{
			if (_go != null)
			{
				UnityEngine.Object.Destroy(_go);
			}
			StopAllCoroutines();
		}

		public static void Quit()
		{
			Application.Quit();
		}

		public Coroutine StartCoroutine(IEnumerator routine)
		{
			Coroutine coroutine = _applicationBehaviour.StartCoroutine(routine);
			_coroutines.Add(coroutine);
			return coroutine;
		}

		public void StopCoroutine(Coroutine routine)
		{
			if (routine != null && _coroutines.Contains(routine))
			{
				if (_applicationBehaviour != null)
				{
					_applicationBehaviour.StopCoroutine(routine);
				}
				_coroutines.Remove(routine);
			}
		}

		public void StopAllCoroutines()
		{
			foreach (Coroutine coroutine in _coroutines)
			{
				if (coroutine != null && _applicationBehaviour != null)
				{
					_applicationBehaviour.StopCoroutine(coroutine);
				}
			}
			_coroutines.Clear();
			_manualCoroutines.Clear();
		}

		public void Invoke(float duration, Action callback)
		{
			if ((bool)_applicationBehaviour)
			{
				_applicationBehaviour.StartCoroutine(InvokeCoroutine(duration, callback));
			}
		}

		public void InvokeNextFrame(Action callback)
		{
			if ((bool)_applicationBehaviour)
			{
				_applicationBehaviour.StartCoroutine(InvokeNextFrameCoroutine(callback));
			}
		}

		public void InvokeAfterFrames(int frames, Action callback)
		{
			if ((bool)_applicationBehaviour)
			{
				_applicationBehaviour.StartCoroutine(InvokeAfterFramesCoroutine(frames, callback));
			}
		}

		private IEnumerator InvokeCoroutine(float duration, Action callback)
		{
			yield return new WaitForSeconds(duration);
			if (callback != null)
			{
				callback();
			}
		}

		private IEnumerator InvokeNextFrameCoroutine(Action callback)
		{
			yield return null;
			if (callback != null)
			{
				callback();
			}
		}

		private IEnumerator InvokeAfterFramesCoroutine(int frames, Action callback)
		{
			for (int i = 0; i < frames; i++)
			{
				yield return null;
			}
			if (callback != null)
			{
				callback();
			}
		}

		public int StartCoroutineManual(IEnumerator routine)
		{
			int num = GenerateManualCoroutineID();
			_manualCoroutines.Add(new Tuple<int, IEnumerator>(num, routine));
			ManualCoroutineMoveNext(num);
			return num;
		}

		public void StopCoroutineManual(int manualCoroutineID)
		{
			for (int i = 0; i < _manualCoroutines.Count; i++)
			{
				Tuple<int, IEnumerator> tuple = _manualCoroutines[i];
				if (tuple != null && tuple.Item1 == manualCoroutineID)
				{
					_manualCoroutines[i] = null;
					break;
				}
			}
		}

		public void ManualCoroutinesMoveNext()
		{
			for (int i = 0; i < _manualCoroutines.Count; i++)
			{
				Tuple<int, IEnumerator> tuple = _manualCoroutines[i];
				if (tuple != null)
				{
					bool flag = true;
					if (tuple.Item2 != null)
					{
						flag = tuple.Item2.MoveNext();
					}
					if (!flag)
					{
						_manualCoroutines[i] = null;
					}
				}
			}
		}

		private void ManualCoroutineMoveNext(int manualCoroutineID)
		{
			for (int i = 0; i < _manualCoroutines.Count; i++)
			{
				Tuple<int, IEnumerator> tuple = _manualCoroutines[i];
				if (tuple != null && tuple.Item1 == manualCoroutineID)
				{
					bool flag = true;
					if (tuple.Item2 != null)
					{
						flag = tuple.Item2.MoveNext();
					}
					if (!flag)
					{
						_manualCoroutines[i] = null;
					}
					break;
				}
			}
		}

		public void ClearFinishCoroutines()
		{
			for (int i = 0; i < _manualCoroutines.Count; i++)
			{
				Tuple<int, IEnumerator> tuple = _manualCoroutines[i];
				if (tuple == null)
				{
					_manualCoroutines.RemoveAt(i);
					i--;
				}
			}
		}

		private int GenerateManualCoroutineID()
		{
			return _manualCoroutineID++;
		}

		public void DetectCheat()
		{
			if (AntiCheatPlugin.Detect())
			{
				try
				{
					Singleton<NetworkManager>.Instance.RequestAntiCheatSDKReport();
					ReportProcList();
				}
				catch (Exception ex)
				{
					SuperDebug.VeryImportantError("Exception: " + ex.ToString());
					AntiCheatQuit("Menu_Title_AntiCheatQuit", "Menu_Desc_AntiCheatQuit");
				}
			}
		}

		public void ReportProcList()
		{
			byte[] buf = AntiCheatPlugin.ReadProcList();
			Singleton<QAManager>.Instance.SendFileToServer(MiscData.Config.DumpFileUploadUrl, "anti-cheat", buf, delegate
			{
				AntiCheatQuit("Menu_Title_AntiCheatQuit", "Menu_Desc_AntiCheatQuit");
			}, delegate
			{
				AntiCheatQuit("Menu_Title_AntiCheatQuit", "Menu_Desc_AntiCheatQuit");
			}, 10f);
		}

		public void AntiCheatQuit(string title, string text)
		{
			try
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText(title),
					desc = LocalizationGeneralLogic.GetText(text),
					notDestroyAfterTouchBG = true,
					hideCloseBtn = true,
					buttonCallBack = delegate
					{
						Quit();
					}
				});
			}
			catch (Exception ex)
			{
				SuperDebug.VeryImportantError("Exception: " + ex.ToString());
				Quit();
			}
		}

		public void DetectEmulator()
		{
			if (!AntiEmulatorPlugin.Detect())
			{
				return;
			}
			try
			{
				Singleton<NetworkManager>.Instance.RequestAntiCheatSDKReport();
				byte[] fileContent = AntiEmulatorPlugin.GetFileContent();
				Singleton<QAManager>.Instance.SendFileToServer(MiscData.Config.DumpFileUploadUrl, "anti-cheat", fileContent, delegate
				{
					AntiCheatQuit("Menu_Title_AntiCheatQuit", "Menu_Desc_AntiCheatQuit");
				}, delegate
				{
					AntiCheatQuit("Menu_Title_AntiCheatQuit", "Menu_Desc_AntiCheatQuit");
				}, 10f, "Emulator");
			}
			catch (Exception ex)
			{
				SuperDebug.VeryImportantError("Exception: " + ex.ToString());
				AntiCheatQuit("Menu_Title_AntiCheatQuit", "Menu_Desc_AntiCheatQuit");
			}
		}
	}
}
