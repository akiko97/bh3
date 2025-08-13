using UnityEngine;

public class ClipboardManager
{
	public static void CopyToClipboard(string input)
	{
		AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.mihoyo.ClipboardTools");
		AndroidJavaObject currentActivity = GetCurrentActivity();
		if (currentActivity != null)
		{
			androidJavaObject.Call("copyTextToClipboard", currentActivity, input);
		}
	}

	public static string GetTextFromClipboard()
	{
		AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.mihoyo.ClipboardTools");
		AndroidJavaObject currentActivity = GetCurrentActivity();
		if (currentActivity == null)
		{
			return null;
		}
		return androidJavaObject.Call<string>("getTextFromClipboard", new object[0]);
	}

	public static AndroidJavaObject GetCurrentActivity()
	{
		return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
	}
}
