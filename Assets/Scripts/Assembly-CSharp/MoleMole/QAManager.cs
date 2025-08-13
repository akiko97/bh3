using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using UnityEngine;

namespace MoleMole
{
	public class QAManager
	{
		private class MessageEntry
		{
			public int id;

			public string message;

			public string stackTrace;

			private static int _lastId;

			public MessageEntry(string msg, string st)
			{
				id = ++_lastId;
				message = msg;
				stackTrace = st;
			}

			public override int GetHashCode()
			{
				return message.GetHashCode() ^ stackTrace.GetHashCode();
			}
		}

		private int _mainThreadID;

		private string _channelName = string.Empty;

		private List<MessageEntry> _sendToServerMessages = new List<MessageEntry>();

		private HashSet<int> _sentMessageHashes = new HashSet<int>();

		public QAManager()
		{
			_mainThreadID = Thread.CurrentThread.ManagedThreadId;
			ConfigChannel configChannel = ConfigUtil.LoadJSONConfig<ConfigChannel>("DataPersistent/BuildChannel/ChannelConfig");
			if (configChannel != null)
			{
				_channelName = configChannel.ChannelName;
			}
			SuperDebug.sendToServerAction = SendMessageToSever;
		}

		public void Destroy()
		{
		}

		[Conditional("NG_HSOD_DEBUG")]
		public void SetFPSContext(string context)
		{
			MonoFPSIndicator monoFPSIndicator = UnityEngine.Object.FindObjectOfType<MonoFPSIndicator>();
			if ((bool)monoFPSIndicator)
			{
				MonoFPSIndicator component = monoFPSIndicator.GetComponent<MonoFPSIndicator>();
				component.logContext = context;
			}
		}

		[Conditional("NG_HSOD_DEBUG")]
		public void LogFPS(string context, float fpsAvg, float fpsMin, float fpsMax)
		{
		}

		public bool IsSentMessage(string msg, string stackTrace)
		{
			MessageEntry messageEntry = new MessageEntry(msg, stackTrace);
			return _sentMessageHashes.Contains(messageEntry.GetHashCode());
		}

		public void SendMessageToSever(string msg, string stackTrace)
		{
			MessageEntry messageEntry = new MessageEntry(msg, stackTrace);
			int hashCode = messageEntry.GetHashCode();
			if (!_sentMessageHashes.Contains(hashCode))
			{
				_sentMessageHashes.Add(hashCode);
				if (_mainThreadID == Thread.CurrentThread.ManagedThreadId && Singleton<ApplicationManager>.Instance.applicationBehaviour != null)
				{
					Singleton<ApplicationManager>.Instance.applicationBehaviour.StartCoroutine(SendMessageToSeverCoroutine(messageEntry));
				}
				else
				{
					_sendToServerMessages.Add(messageEntry);
				}
			}
		}

		public IEnumerator SendMessageToSeverSync(string msg, string stackTrace)
		{
			MessageEntry entry = new MessageEntry(msg, stackTrace);
			int hash = entry.GetHashCode();
			if (!_sentMessageHashes.Contains(hash))
			{
				_sentMessageHashes.Add(hash);
				yield return SendMessageToSeverCoroutine(entry);
			}
		}

		public void UpdateSendMessageToSever()
		{
			if (_sendToServerMessages.Count > 0 && Singleton<ApplicationManager>.Instance.applicationBehaviour != null)
			{
				List<MessageEntry> list = new List<MessageEntry>(_sendToServerMessages);
				_sendToServerMessages.Clear();
				for (int i = 0; i < list.Count; i++)
				{
					Singleton<ApplicationManager>.Instance.applicationBehaviour.StartCoroutine(SendMessageToSeverCoroutine(list[i]));
				}
			}
		}

		private IEnumerator SendMessageToSeverCoroutine(MessageEntry msg)
		{
			int uid = -1;
			if (Singleton<PlayerModule>.Instance != null && Singleton<PlayerModule>.Instance.playerData != null)
			{
				uid = Singleton<PlayerModule>.Instance.playerData.userId;
			}
			string vid = string.Empty;
			if (Singleton<NetworkManager>.Instance != null)
			{
				vid = Singleton<NetworkManager>.Instance.GetGameVersion();
			}
			string asb = string.Empty;
			if (Singleton<AssetBundleManager>.Instance != null && Singleton<AssetBundleManager>.Instance.Loader != null)
			{
				asb = Singleton<AssetBundleManager>.Instance.Loader.assetBoundleSVNVersion;
			}
			StringBuilder url = new StringBuilder();
			url.Append("http://139.196.152.26:10080/recorder?");
			url.Append("&channelName=" + _channelName);
			url.Append("&uid=" + uid);
			url.Append("&vid=" + vid);
			url.Append("&asb=" + asb);
			url.Append("&time=" + WWW.EscapeURL(TimeUtil.Now.ToString()));
			url.Append("&operatingSystem=" + WWW.EscapeURL(SystemInfo.operatingSystem));
			url.Append("&deviceModel=" + WWW.EscapeURL(SystemInfo.deviceModel));
			url.Append("&graphicsDeviceName=" + WWW.EscapeURL(SystemInfo.graphicsDeviceName));
			url.Append("&graphicsDeviceType=" + SystemInfo.graphicsDeviceType);
			url.Append("&graphicsDeviceVendor=" + WWW.EscapeURL(SystemInfo.graphicsDeviceVendor));
			url.Append("&graphicsDeviceVersion=" + WWW.EscapeURL(SystemInfo.graphicsDeviceVersion));
			url.Append("&graphicsMemorySize=" + SystemInfo.graphicsMemorySize);
			url.Append("&systemMemorySize=" + SystemInfo.systemMemorySize);
			url.Append("&processorCount=" + SystemInfo.processorCount);
			url.Append("&processorFrequency=" + SystemInfo.processorFrequency);
			url.Append("&processorType=" + WWW.EscapeURL(SystemInfo.processorType));
			url.Append("&msgId=" + msg.id);
			url.Append("&msg=" + WWW.EscapeURL(msg.message));
			url.Append("&st=" + WWW.EscapeURL(msg.stackTrace));
			yield return new WWW(url.ToString());
		}

		public void SendFileToServer(string url, string fileType, byte[] buf, Action<string> successCallback, Action failCallback, float timeoutSecond = 5f, string prefix = null, bool needDispose = true)
		{
			string arg = Singleton<PlayerModule>.Instance.playerData.userId.ToString();
			string gameVersion = Singleton<NetworkManager>.Instance.GetGameVersion();
			string arg2 = TimeUtil.Now.ToString("yyyy-MM-dd_HH-mm-ss");
			string text = string.Format("{0}-{1}-{2}.txt", arg, gameVersion, arg2);
			if (!string.IsNullOrEmpty(prefix))
			{
				text = string.Format("{0}-{1}", prefix, text);
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("File-Type", fileType);
			dictionary.Add("File-Name", text);
			Dictionary<string, string> headers = dictionary;
			Singleton<ApplicationManager>.Instance.applicationBehaviour.StartCoroutine(Miscs.WWWRequestWithTimeOut(url, successCallback, failCallback, timeoutSecond, buf, headers, needDispose));
		}
	}
}
