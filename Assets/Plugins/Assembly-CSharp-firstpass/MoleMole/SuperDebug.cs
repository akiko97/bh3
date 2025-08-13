using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MoleMole
{
	public static class SuperDebug
	{
		public const int DEFAULT = 0;

		public const int LEVEL = 1;

		public const int EVENT = 2;

		public const int LUA = 3;

		public const int AI = 4;

		public const int ABILITY = 5;

		public const int NETWORK = 6;

		public const int ANIMATION = 7;

		public const int QA = 8;

		public const int ASSETBUNDLE = 9;

		public const int APNS = 10;

		public const int MP = 11;

		public static bool[] DEBUG_SWITCH;

		public static string[] LOG_PREFIX;

		public static Action<string, string> sendToServerAction;

		static SuperDebug()
		{
			DEBUG_SWITCH = new bool[12]
			{
				true, false, false, false, false, false, true, false, false, true,
				true, false
			};
			LOG_PREFIX = new string[12]
			{
				"DEFAULT:", "LEVEL:", "EVENT:", "LUA:", "AI:", "ABILITY:", "NETWORK:", "ANIMATION:", "QA:", "ASSETBUNDLE:",
				"APNS:", "MP:"
			};
		}

		[Conditional("NG_HSOD_DEBUG")]
		public static void Log(string log)
		{
		}

		[Conditional("NG_HSOD_DEBUG")]
		public static void LogWarning(string log)
		{
		}

		[Conditional("NG_HSOD_DEBUG")]
		public static void LogError(string log)
		{
		}

		[Conditional("NG_HSOD_DEBUG")]
		public static void Log(int type, string log)
		{
			if (DEBUG_SWITCH[type])
			{
				UnityEngine.Debug.Log(LOG_PREFIX[type] + log);
			}
		}

		[Conditional("NG_HSOD_DEBUG")]
		public static void LogWarning(int type, string log)
		{
			UnityEngine.Debug.LogWarning(LOG_PREFIX[type] + log);
		}

		[Conditional("NG_HSOD_DEBUG")]
		public static void LogError(int type, string log)
		{
		}

		[Conditional("NG_HSOD_DEBUG")]
		public static void DrawLine(int type, Vector3 start, Vector3 end, Color? color = null, float duration = 1f, bool depthTest = true)
		{
			if (DEBUG_SWITCH[type])
			{
				UnityEngine.Debug.DrawLine(start, end, (!color.HasValue) ? Color.blue : color.Value, duration, depthTest);
			}
		}

		[Conditional("NG_HSOD_DEBUG")]
		public static void CheckDictionaryKey<K, V>(Dictionary<K, V> dict, K key)
		{
			if (!dict.ContainsKey(key))
			{
				throw new Exception(string.Format("dict missing key: {0}", key.ToString()));
			}
		}

		[Conditional("NG_HSOD_DEBUG")]
		public static void Assert(bool cond, string msg = null)
		{
			if (msg == null)
			{
				msg = "Assertion Error";
			}
			if (!cond)
			{
				UnityEngine.Debug.LogError(msg);
			}
		}

		[Conditional("NG_HSOD_DEBUG")]
		public static void Error(string msg = null)
		{
			if (msg == null)
			{
				msg = "Error";
			}
		}

		public static void VeryImportantAssert(bool cond, string msg = null)
		{
			if (msg == null)
			{
				msg = "Very Important Assertion Error";
			}
			if (!cond)
			{
				UnityEngine.Debug.LogError(msg);
				if (sendToServerAction != null)
				{
					sendToServerAction(msg, StackTraceUtility.ExtractStackTrace());
				}
			}
		}

		public static void VeryImportantAssert(bool cond, Func<string> callback = null)
		{
			if (!cond)
			{
				string empty = string.Empty;
				empty = ((callback == null) ? "Very Important Assertion Error" : callback());
				UnityEngine.Debug.LogError(empty);
				if (sendToServerAction != null)
				{
					sendToServerAction(empty, StackTraceUtility.ExtractStackTrace());
				}
			}
		}

		public static void VeryImportantError(string msg = null)
		{
			if (msg == null)
			{
				msg = "Very Important Error";
			}
			if (sendToServerAction != null)
			{
				sendToServerAction(msg, StackTraceUtility.ExtractStackTrace());
			}
		}

		public static void CloseAllDebugs()
		{
			for (int i = 0; i < DEBUG_SWITCH.Length; i++)
			{
				DEBUG_SWITCH[i] = false;
			}
		}
	}
}
