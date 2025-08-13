using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AntiEmulatorPlugin : MonoBehaviour
{
	private static bool _isEnabled;

	private static List<string> _deviceModelList;

	public static void Init(bool enabled, List<string> deviceModelList)
	{
		_isEnabled = enabled;
		_deviceModelList = deviceModelList;
	}

	public static bool Detect()
	{
		if (_isEnabled && IsEmulator())
		{
			return true;
		}
		return false;
	}

	public static bool IsEmulator()
	{
		return _deviceModelList != null && _deviceModelList.Contains(SystemInfo.deviceModel);
	}

	public static byte[] GetFileContent()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("deviceModel=" + SystemInfo.deviceModel);
		stringBuilder.AppendLine("deviceName=" + SystemInfo.deviceName);
		stringBuilder.AppendLine("deviceType=" + SystemInfo.deviceType);
		stringBuilder.AppendLine("deviceUniqueIdentifier=" + SystemInfo.deviceUniqueIdentifier);
		stringBuilder.AppendLine("graphicsDeviceID=" + SystemInfo.graphicsDeviceID);
		stringBuilder.AppendLine("graphicsDeviceName=" + SystemInfo.graphicsDeviceName);
		stringBuilder.AppendLine("graphicsDeviceType=" + SystemInfo.graphicsDeviceType);
		stringBuilder.AppendLine("graphicsDeviceVendor=" + SystemInfo.graphicsDeviceVendor);
		stringBuilder.AppendLine("graphicsDeviceVendorID=" + SystemInfo.graphicsDeviceVendorID);
		stringBuilder.AppendLine("graphicsDeviceVersion=" + SystemInfo.graphicsDeviceVersion);
		stringBuilder.AppendLine("graphicsMemorySize=" + SystemInfo.graphicsMemorySize);
		stringBuilder.AppendLine("graphicsMultiThreaded=" + SystemInfo.graphicsMultiThreaded);
		stringBuilder.AppendLine("graphicsShaderLevel=" + SystemInfo.graphicsShaderLevel);
		stringBuilder.AppendLine("maxTextureSize=" + SystemInfo.maxTextureSize);
		stringBuilder.AppendLine("operatingSystem=" + SystemInfo.operatingSystem);
		stringBuilder.AppendLine("processorCount=" + SystemInfo.processorCount);
		stringBuilder.AppendLine("processorFrequency=" + SystemInfo.processorFrequency);
		stringBuilder.AppendLine("processorType=" + SystemInfo.processorType);
		stringBuilder.AppendLine("systemMemorySize=" + SystemInfo.systemMemorySize);
		return Encoding.ASCII.GetBytes(stringBuilder.ToString());
	}
}
