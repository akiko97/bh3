using UnityEngine;

namespace MoleMole
{
	public class MonoTalkingData : MonoBehaviour
	{
		private void Start()
		{
			string channelName = Singleton<NetworkManager>.Instance.channelConfig.ChannelName;
			TalkingDataPlugin.SetLogEnabled(true);
			TalkingDataPlugin.SetExceptionReportEnabled(true);
			TalkingDataPlugin.SessionStarted("A0F361D0ACD1B1846FCEE7327ADEF913", channelName);
		}

		public void OnApplicationQuit()
		{
			TalkingDataPlugin.SessionStoped();
		}
	}
}
