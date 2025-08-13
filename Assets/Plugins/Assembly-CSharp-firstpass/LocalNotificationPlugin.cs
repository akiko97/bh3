using System;
using UnityEngine;

public class LocalNotificationPlugin
{
	public enum Mode
	{
		Inexact = 0,
		Exact = 1,
		ExactAndAllowWhileIdle = 2
	}

	private static AndroidJavaClass pluginClass = new AndroidJavaClass("com.mihoyo.localnotification.LocalNotificationManager");

	public static void SendNotification(int id, TimeSpan delay, string title, string message)
	{
		SendNotification(id, (long)delay.TotalSeconds * 1000, title, message, message);
	}

	public static void SendNotification(int id, long delay, string title, string message, string ticker, bool sound = true, bool vibrate = true, bool lights = true, Mode mode = Mode.Inexact)
	{
		if (pluginClass != null)
		{
			pluginClass.CallStatic("SetNotification", id, delay, title, message, ticker, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, (int)mode);
		}
	}

	public static void CancelNotification(int id)
	{
		if (pluginClass != null)
		{
			pluginClass.CallStatic("CancelNotification", id);
		}
	}

	public static void ClearNotifications()
	{
		if (pluginClass != null)
		{
			pluginClass.CallStatic("ClearNotifications");
		}
	}
}
