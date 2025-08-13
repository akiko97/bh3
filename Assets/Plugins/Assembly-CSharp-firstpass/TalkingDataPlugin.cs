using System;
using System.Collections.Generic;
using UnityEngine;

public class TalkingDataPlugin
{
	private const string version = "1.3.14";

	public static void SetLocation(double latitude, double longitude)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform == RuntimePlatform.WindowsEditor)
		{
		}
	}

	public static void SetLogEnabled(bool enable)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
			androidJavaClass.SetStatic("LOG_ON", enable);
		}
	}

	public static void SessionStarted(string appKey, string channelId)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			Debug.Log("TalkingData App Analytics Unity3d SDK version is 1.3.14");
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
			Debug.Log("android start");
			androidJavaClass.CallStatic("init", androidJavaObject, appKey, channelId);
			androidJavaClass.CallStatic("onResume", androidJavaObject);
		}
	}

	public static void SessionStoped()
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			Debug.Log("android stop");
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
			androidJavaClass.CallStatic("onPause", androidJavaObject);
		}
	}

	public static void SetExceptionReportEnabled(bool enable)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
			androidJavaClass.CallStatic("setReportUncaughtExceptions", enable);
		}
	}

	public static void TrackEvent(string eventId)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
			androidJavaClass.CallStatic("onEvent", androidJavaObject, eventId);
		}
	}

	public static void TrackEventWithLabel(string eventId, string eventLabel)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
			androidJavaClass.CallStatic("onEvent", androidJavaObject, eventId, eventLabel);
		}
	}

	public static void TrackEventWithParameters(string eventId, string eventLabel, Dictionary<string, object> parameters)
	{
		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || parameters == null || parameters.Count <= 0 || parameters.Count > 10)
		{
			return;
		}
		int count = parameters.Count;
		AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap", count);
		IntPtr methodID = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
		object[] array = new object[2];
		foreach (KeyValuePair<string, object> parameter in parameters)
		{
			array[0] = new AndroidJavaObject("java.lang.String", parameter.Key);
			if (typeof(string).IsInstanceOfType(parameter.Value))
			{
				array[1] = new AndroidJavaObject("java.lang.String", parameter.Value);
			}
			else
			{
				array[1] = new AndroidJavaObject("java.lang.Double", string.Empty + parameter.Value);
			}
			AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
		}
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject2 = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
		androidJavaClass.CallStatic("onEvent", androidJavaObject2, eventId, eventLabel, androidJavaObject);
	}

	public static void TrackPageBegin(string pageName)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
			androidJavaClass.CallStatic("onPageStart", androidJavaObject, pageName);
		}
	}

	public static void TrackPageEnd(string pageName)
	{
		if (Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.WindowsEditor)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tendcloud.tenddata.TCAgent");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
			androidJavaClass.CallStatic("onPageEnd", androidJavaObject, pageName);
		}
	}
}
