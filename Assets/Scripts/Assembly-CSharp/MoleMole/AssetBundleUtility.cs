using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MoleMole
{
	public class AssetBundleUtility
	{
		public static byte BYTE_SALT = 165;

		public static string RSA_SALT = "<RSAKeyValue><Modulus>rXoKWm82JSX4UYihkt2FSjrp3pZqTxt6AyJ0ZexHssStYesCFuUOmDBrk0nxPTY2r7oB4ZC9tDhHzmA66Me56wkD47Z3fCEBfLFxmEVdUCvM1RIFdQxQCB7CMaFWXHoVfBhNcD60OtXD71vFusBLioa6HDHbKk8LdgWdV10OWaE=</Modulus><Exponent>EQ==</Exponent><P>16GiwrgCGvcYbgSZOBJRx4G9kioGgexLSyW62iK4EuT0Xu9xyflBDaC4yooFkxrflqEAIiEfTqNGlYeJks+5qw==</P><Q>zfQY4dWi/Dlo38y6xvX4pUEAj1hbeFo/Qiy7H00P089W0KC6Mdi+GY4UuRGJtgX7UZfGQdHRj8mBjijFyhUl4w==</Q><DP>cihlOejyDkaUdnrntEXvD0Svp7vlU9dzJ8iuNz+OoJdUMkKHiQt8yvq8Lv3Gt0p2Xs20xsY9wDhSi2Xfa9diSw==</DP><DQ>GDrVwDdAWeii7SclCFksT61LXCiDO1XpUxRSP+ryzZ/sGIthMwpwt7ZcynqIrAC0J7eAvHMJmHIPPeat24oEdQ==</DQ><InverseQ>P4/vgq1XF77N8K/OxTbcjWFCC1d+v3W5xWQJbmU3KfVF2wOStZeILT2X12s7AHD+uUfN9O/xdEBIeqcSLVxWjw==</InverseQ><D>o0WvZCxvMgWeatrybBvIvlWQ0X6CLFYYe2u42GXpILkbp3PFuzHvnkuwip/yG35RllS2efGjfHE0hgA3cazrNgM6gBDcFa7iznviIiQTySxFuzy3mXpjSQFaGgdvmuUQLgg5qahcdGgT455Fzo5GSu+IyTpD+dNoKy79NLTbvjE=</D></RSAKeyValue>";

		public static byte[] ENCRYPTED_KEY;

		private static string DATA_PATH = Application.dataPath;

		private static string PERSISTENT_DATA_PATH = Application.persistentDataPath;

		private static bool LOCAL_TEST = false;

		public static AssetBundleInfo[] GetVersionAssetBundleInfo(BundleType bundleType)
		{
			AssetBundleInfo[] array = null;
			if (bundleType == BundleType.DATA_FILE)
			{
				return new AssetBundleInfo[1]
				{
					new AssetBundleInfo("DataVersion", 100L, string.Empty, null, null, UnloadMode.MANUAL_UNLOAD, DownloadMode.IMMEDIATELY, bundleType, false, string.Empty)
				};
			}
			return new AssetBundleInfo[2]
			{
				new AssetBundleInfo("ResourceVersion", 100L, string.Empty, null, null, UnloadMode.MANUAL_UNLOAD, DownloadMode.IMMEDIATELY, bundleType, false, "event"),
				new AssetBundleInfo("ResourceVersion", 100L, string.Empty, null, null, UnloadMode.MANUAL_UNLOAD, DownloadMode.IMMEDIATELY, bundleType, false, string.Empty)
			};
		}

		public static string LocalAssetBundleDirectory(BundleType bundleType)
		{
			string result = string.Empty;
			switch (bundleType)
			{
			case BundleType.DATA_FILE:
				result = PERSISTENT_DATA_PATH + "/Data/";
				break;
			case BundleType.RESOURCE_FILE:
				result = PERSISTENT_DATA_PATH + "/Resources/";
				break;
			}
			return result;
		}

		public static string RemoteAssetBundleDirctory(BundleType bundleType, string serverAddr, string remoteDir)
		{
			string empty = string.Empty;
			switch (bundleType)
			{
			case BundleType.DATA_FILE:
				empty = "data";
				break;
			case BundleType.RESOURCE_FILE:
				empty = "resource";
				break;
			default:
				throw new Exception("Invalid Type or State!");
			}
			string text = ((!string.IsNullOrEmpty(remoteDir)) ? remoteDir : empty);
			if (LOCAL_TEST)
			{
				return "file://" + DATA_PATH + "/../Packages/" + text + "/editor_compressed/";
			}
			string text2 = "android_compressed";
			return serverAddr + "/" + text + "/" + text2 + "/";
		}

		public static bool ValidateAndSaveAssetBundle(BundleType bundleType, AssetBundleDownloadTask task)
		{
			string localFilePath = task.AssetBundleInfo.LocalFilePath;
			try
			{
				CreateParentDirectory(localFilePath);
				DeleteFile(localFilePath);
				byte[] downloadedBytes = task.DownloadedBytes;
				AssetBundleInfo assetBundleInfo = task.AssetBundleInfo;
				if (GlobalVars.DataUseAssetBundle)
				{
					string text = CalculateFileCrc(localFilePath, downloadedBytes);
					if (text != assetBundleInfo.FileCrc)
					{
						throw new Exception(string.Format("CRC Mismatch. Local:{0}, Remote:{1}. Retry downloading.", text, assetBundleInfo.FileCrc));
					}
				}
				File.WriteAllBytes(localFilePath, downloadedBytes);
				return true;
			}
			catch (Exception)
			{
				if (File.Exists(localFilePath))
				{
					File.Delete(localFilePath);
				}
				return false;
			}
		}

		public static void MyAESDecrypted(ref byte[] bytes)
		{
			try
			{
				byte[] bytes2 = new byte[ENCRYPTED_KEY.Length];
				Array.Copy(ENCRYPTED_KEY, 0, bytes2, 0, ENCRYPTED_KEY.Length);
				SecurityUtil.RSADecrypted(ref bytes2, RSA_SALT);
				byte[] array = new byte[32];
				byte[] array2 = new byte[16];
				Array.Copy(bytes2, 0, array, 0, 32);
				Array.Copy(bytes2, 32, array2, 0, 16);
				SecurityUtil.AESDecrypted(ref bytes, array, array2);
				ClearBuffer(bytes2);
				ClearBuffer(array);
				ClearBuffer(array2);
			}
			catch (Exception)
			{
			}
		}

		public static string CalculateFileCrc(string filePath, byte[] fileBytes)
		{
			try
			{
				byte[] bytes = new byte[ENCRYPTED_KEY.Length];
				Array.Copy(ENCRYPTED_KEY, 0, bytes, 0, ENCRYPTED_KEY.Length);
				SecurityUtil.RSADecrypted(ref bytes, RSA_SALT);
				byte[] array = new byte[8];
				Array.Copy(bytes, 48, array, 0, 8);
				string result = SecurityUtil.CalculateFileHash(fileBytes, array);
				ClearBuffer(bytes);
				ClearBuffer(array);
				return result;
			}
			catch (Exception)
			{
			}
			return string.Empty;
		}

		private static void ClearBuffer(byte[] buffer)
		{
			if (buffer != null)
			{
				for (int i = 0; i < buffer.Length; i++)
				{
					buffer[i] = 0;
				}
				buffer = null;
			}
		}

		public static void RebuildDirectory(string path)
		{
			string path2 = path.Substring(0, path.LastIndexOf('/'));
			if (Directory.Exists(path2))
			{
				Directory.Delete(path2, true);
			}
			if (!Directory.Exists(path2))
			{
				Directory.CreateDirectory(path2);
			}
		}

		public static void CreateParentDirectory(string path)
		{
			string path2 = path.Substring(0, path.LastIndexOf('/'));
			if (!Directory.Exists(path2))
			{
				Directory.CreateDirectory(path2);
			}
		}

		public static void DeleteFile(string path)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		public static List<string> GetFileList(string path)
		{
			List<string> list = new List<string>();
			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
				foreach (string text in files)
				{
					if (!text.EndsWith(".DS_Store") && !text.EndsWith(".meta") && !text.EndsWith(".bat") && !text.EndsWith(".mask"))
					{
						list.Add(text.Replace('\\', '/'));
					}
				}
			}
			else
			{
				list.Add(path.Replace('\\', '/'));
			}
			return list;
		}

		public static string GetResourcePath(string path)
		{
			string text = path.Replace("Assets/Resources/", string.Empty);
			if (text.Contains("."))
			{
				text = text.Substring(0, text.LastIndexOf('.'));
			}
			return text;
		}

		public static string GetResourcePathForEditor(string path)
		{
			string text = path.Replace(Application.dataPath + "/Resources/", string.Empty);
			if (text.Contains("."))
			{
				text = text.Substring(0, text.LastIndexOf('.'));
			}
			return text;
		}

		public static string GetAssetPath(string path)
		{
			return path.Replace(Application.dataPath + "/", string.Empty);
		}
	}
}
