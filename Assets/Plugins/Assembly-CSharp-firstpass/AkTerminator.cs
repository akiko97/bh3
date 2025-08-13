using System;
using System.Threading;
using UnityEngine;

[AddComponentMenu("Wwise/AkTerminator")]
public class AkTerminator : MonoBehaviour
{
	private static AkTerminator ms_Instance;

	private void Awake()
	{
		if (ms_Instance != null)
		{
			if (ms_Instance != this)
			{
				UnityEngine.Object.DestroyImmediate(this);
			}
		}
		else
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
			ms_Instance = this;
		}
	}

	private void OnApplicationQuit()
	{
		Terminate();
	}

	private void OnDestroy()
	{
		if (ms_Instance == this)
		{
			ms_Instance = null;
		}
	}

	private void Terminate()
	{
		if (!(ms_Instance != null) || !(ms_Instance == this) || !AkSoundEngine.IsInitialized())
		{
			return;
		}
		AkSoundEngine.StopAll();
		AkSoundEngine.RenderAudio();
		for (uint num = 0u; num < 50; num++)
		{
			AkCallbackManager.PostCallbacks();
			using (EventWaitHandle eventWaitHandle = new ManualResetEvent(false))
			{
				eventWaitHandle.WaitOne(TimeSpan.FromMilliseconds(1.0));
			}
		}
		AkSoundEngine.Term();
		ms_Instance = null;
		AkCallbackManager.Term();
		AkBankManager.Reset();
	}
}
