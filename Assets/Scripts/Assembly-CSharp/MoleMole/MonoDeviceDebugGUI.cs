using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace MoleMole
{
	public class MonoDeviceDebugGUI : MonoBehaviour
	{
		private class LogEntry
		{
			public string message;

			public string stackTrace;

			public LogType type;
		}

		private enum DeviceLogState
		{
			Collapsed = 0,
			CollapsedWaitTouchDone = 1,
			Expanded = 2,
			ExpanedWaitTouchDone = 3,
			Crashed = 4
		}

		private const int MAX_LOG_COUNT = 60;

		private List<LogEntry> _logs;

		private int _queueIx;

		private DeviceLogState _state;

		private Vector2 _scrollPos;

		private GUIStyle _bgStyle;

		private GUIStyle _redStyle;

		private GUIStyle _yellowStyle;

		private GUIStyle _whiteStyle;

		private void Awake()
		{
			_logs = new List<LogEntry>();
			for (int i = 0; i < 60; i++)
			{
				_logs.Add(new LogEntry());
			}
			_queueIx = 0;
			_redStyle = new GUIStyle();
			_redStyle.normal.textColor = Color.red;
			_yellowStyle = new GUIStyle();
			_yellowStyle.normal.textColor = Color.yellow;
			_whiteStyle = new GUIStyle();
			_whiteStyle.normal.textColor = Color.white;
			_bgStyle = new GUIStyle();
			Texture2D texture2D = new Texture2D(16, 16);
			Color[] array = new Color[256];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = new Color(0f, 0f, 0f, 1f);
			}
			texture2D.SetPixels(array);
			_bgStyle.normal.background = texture2D;
			_state = DeviceLogState.CollapsedWaitTouchDone;
			Application.logMessageReceived += LogCallback;
		}

		private void OnDestroy()
		{
			Application.logMessageReceived -= LogCallback;
		}

		private void Update()
		{
			if (_state == DeviceLogState.Collapsed)
			{
				if (Input.touchCount > 4)
				{
					_state = DeviceLogState.ExpanedWaitTouchDone;
				}
			}
			else if (_state == DeviceLogState.ExpanedWaitTouchDone)
			{
				if (Input.touchCount < 5)
				{
					_state = DeviceLogState.Expanded;
				}
			}
			else if (_state == DeviceLogState.Expanded || _state == DeviceLogState.Crashed)
			{
				if (Input.touchCount > 4)
				{
					_state = DeviceLogState.CollapsedWaitTouchDone;
				}
			}
			else if (_state == DeviceLogState.CollapsedWaitTouchDone && Input.touchCount < 5)
			{
				_state = DeviceLogState.Collapsed;
			}
		}

		private void ShowCrashReport()
		{
			CrashReport[] reports = CrashReport.reports;
			GUILayout.Label("Crash reports:");
			CrashReport[] array = reports;
			foreach (CrashReport crashReport in array)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Crash: " + crashReport.time);
				if (GUILayout.Button("Log"))
				{
				}
				if (GUILayout.Button("Remove"))
				{
					crashReport.Remove();
				}
				GUILayout.EndHorizontal();
			}
		}

		private void LogCallback(string logString, string stackTrace, LogType type)
		{
			if (type == LogType.Exception)
			{
				OnException();
			}
			LogEntry logEntry = _logs[_queueIx++ % 60];
			logEntry.message = logString;
			logEntry.stackTrace = stackTrace;
			logEntry.type = type;
			if (type == LogType.Exception)
			{
				if (!Singleton<QAManager>.Instance.IsSentMessage(logEntry.message, logEntry.stackTrace))
				{
					StartCoroutine(SendExceptionToSever(logEntry));
				}
				else
				{
					OnExceptionContinue();
				}
			}
		}

		private IEnumerator SendExceptionToSever(LogEntry entry)
		{
			yield return Singleton<QAManager>.Instance.SendMessageToSeverSync(entry.message, entry.stackTrace);
			yield return null;
			yield return null;
			yield return null;
			yield return null;
			yield return null;
			if (Singleton<MainUIManager>.Instance != null)
			{
				string uidStr = Singleton<PlayerModule>.Instance.playerData.userId.ToString();
				string vidStr = Singleton<NetworkManager>.Instance.GetGameVersion();
				string nowStr = TimeUtil.Now.ToString();
				string errDesc = "time=" + nowStr + "\nuid=" + uidStr + "\nvid=" + vidStr + "\nmsg=" + entry.message;
				if (GlobalVars.ENABLE_EXCEPTION_CONTINUE)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new HintWithConfirmDialogContext(errDesc, LocalizationGeneralLogic.GetText("Menu_ContinueGame"), LocalizationGeneralLogic.GetText("Menu_QuitGame"), OnExceptionConfirm, LocalizationGeneralLogic.GetText("Menu_Title_FatalError")));
				}
				else
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new HintWithConfirmDialogContext(errDesc, LocalizationGeneralLogic.GetText("Menu_QuitGame"), Application.Quit, LocalizationGeneralLogic.GetText("Menu_Title_FatalError")));
				}
			}
			else
			{
				Application.Quit();
			}
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Time.timeScale = 0f;
			if (Singleton<EventManager>.Instance != null)
			{
				Singleton<EventManager>.Instance.DropEventsAndStop();
			}
			if (BehaviorManager.instance != null)
			{
				BehaviorManager.instance.gameObject.SetActive(false);
			}
		}

		private void OnException()
		{
			_state = DeviceLogState.Crashed;
			Application.logMessageReceived -= LogCallback;
			if (Singleton<LevelManager>.Instance != null)
			{
				Singleton<LevelManager>.Instance.SetPause(true);
			}
		}

		private void OnExceptionContinue()
		{
			Application.logMessageReceived += LogCallback;
			if (Singleton<LevelManager>.Instance != null)
			{
				Singleton<LevelManager>.Instance.SetPause(false);
			}
		}

		private void OnExceptionQuit()
		{
			if (Singleton<EventManager>.Instance != null)
			{
				Singleton<EventManager>.Instance.DropEventsAndStop();
			}
			if (BehaviorManager.instance != null)
			{
				BehaviorManager.instance.gameObject.SetActive(false);
			}
			Application.Quit();
		}

		private void OnExceptionConfirm(bool isOK)
		{
			if (isOK)
			{
				OnExceptionContinue();
			}
			else
			{
				OnExceptionQuit();
			}
		}
	}
}
