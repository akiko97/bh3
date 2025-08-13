using System;
using System.IO;
using System.Reflection;
using UnityEngine;

public class AkBasePathGetter
{
	public static string GetPlatformName()
	{
		try
		{
			Type type = null;
			type = Type.GetType("AkCustomPlatformNameGetter");
			if (type != null)
			{
				MethodInfo methodInfo = null;
				methodInfo = type.GetMethod("GetPlatformName");
				if (methodInfo != null)
				{
					string text = (string)methodInfo.Invoke(null, null);
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
				}
			}
		}
		catch
		{
		}
		string text2 = "Undefined platform sub-folder";
		return "Android";
	}

	public static string GetPlatformBasePath()
	{
		string empty = string.Empty;
		empty = Path.Combine(GetFullSoundBankPath(), GetPlatformName());
		FixSlashes(ref empty);
		return empty;
	}

	public static string GetFullSoundBankPath()
	{
		string path = AkInitializer.GetBasePath();
		FixSlashes(ref path);
		return path;
	}

	public static void FixSlashes(ref string path)
	{
		string text = Path.DirectorySeparatorChar.ToString();
		string empty = string.Empty;
		empty = ((Path.DirectorySeparatorChar != '/') ? "/" : "\\");
		path.Trim();
		path = path.Replace(empty, text);
		path = path.TrimStart('\\');
		if (!path.EndsWith(text))
		{
			path += text;
		}
	}

	public static string GetValidBasePath()
	{
		string platformBasePath = GetPlatformBasePath();
		bool flag = true;
		if (platformBasePath == string.Empty || !flag)
		{
			Debug.LogError("WwiseUnity: Could not locate the SoundBanks. Did you make sure to copy them to the StreamingAssets folder?");
			return string.Empty;
		}
		return platformBasePath;
	}
}
