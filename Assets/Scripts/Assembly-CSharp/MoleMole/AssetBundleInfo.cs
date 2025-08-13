using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

namespace MoleMole
{
	public class AssetBundleInfo
	{
		private bool _completeness;

		private AssetBundle _assetBundle;

		private static string FileNamePattern = "N";

		private static string FileCompressedSizePattern = "CS";

		private static string FileCrcPattern = "CRC";

		private static string ParentFileNamePattern = "PN";

		private static string FileUnloadModePattern = "ULM";

		private static string FileDownloadModePattern = "DLM";

		private static string FileBundleTypePattern = "BT";

		private static string AssetPathSetPattern = "APS";

		private static string RemainPattern = "R";

		private static string RemoteDirPattern = "RD";

		public string FileName { get; private set; }

		public long FileCompressedSize { get; private set; }

		public string FileCrc { get; private set; }

		public HashSet<string> ParentFileNameSet { get; private set; }

		public HashSet<string> AssetPathSet { get; private set; }

		public UnloadMode FileUnloadMode { get; private set; }

		public DownloadMode FileDownloadMode { get; private set; }

		public BundleType FileBundleType { get; private set; }

		public bool RemainInInstallPackage { get; private set; }

		public string RemoteDir { get; private set; }

		public string LocalFilePath
		{
			get
			{
				return AssetBundleUtility.LocalAssetBundleDirectory(FileBundleType) + FileName + ((!string.IsNullOrEmpty(FileCrc)) ? ("_" + FileCrc) : string.Empty) + ".unity3d";
			}
		}

		public string RemoteFilePath
		{
			get
			{
				return AssetBundleUtility.RemoteAssetBundleDirctory(FileBundleType, Singleton<AssetBundleManager>.Instance.remoteAssetBundleUrl, RemoteDir) + FileName + ((!string.IsNullOrEmpty(FileCrc)) ? ("_" + FileCrc) : ".unity3d");
			}
		}

		public AssetBundleInfo(string name, long compressedSize, string crc, HashSet<string> parentFileNameSet, HashSet<string> assetPathSet, UnloadMode unloadMode, DownloadMode downloadMode, BundleType bundleType, bool remain, string remoteDir)
		{
			FileName = name;
			FileCompressedSize = compressedSize;
			FileCrc = crc;
			ParentFileNameSet = parentFileNameSet;
			AssetPathSet = assetPathSet;
			FileUnloadMode = unloadMode;
			FileDownloadMode = downloadMode;
			FileBundleType = bundleType;
			RemainInInstallPackage = remain;
			RemoteDir = remoteDir;
			_assetBundle = null;
			_completeness = false;
		}

		public bool IsDownloaded()
		{
			return File.Exists(LocalFilePath);
		}

		public bool IsLoaded()
		{
			return _assetBundle != null;
		}

		public T Load<T>(string name) where T : UnityEngine.Object
		{
			PreLoad();
			if (_assetBundle != null)
			{
				return _assetBundle.LoadAsset<T>(name);
			}
			return (T)null;
		}

		public AssetBundleRequest LoadAsync(string name)
		{
			PreLoad();
			if (_assetBundle != null)
			{
				return _assetBundle.LoadAssetAsync(name);
			}
			return null;
		}

		private void PreLoad()
		{
			if (_assetBundle != null || !File.Exists(LocalFilePath))
			{
				return;
			}
			foreach (string item in ParentFileNameSet)
			{
				if (!string.IsNullOrEmpty(item))
				{
					AssetBundleInfo assetBundleInfoByFileName = Singleton<AssetBundleManager>.Instance.GetAssetBundleInfoByFileName(FileBundleType, item);
					assetBundleInfoByFileName.PreLoad();
				}
			}
			try
			{
				if (FileBundleType == BundleType.DATA_FILE)
				{
					byte[] bytes = File.ReadAllBytes(LocalFilePath);
					if (!_completeness)
					{
						_completeness = true;
						if (AssetBundleUtility.CalculateFileCrc(null, bytes) != FileCrc)
						{
							throw new Exception("File is Not Completeness.");
						}
					}
					AssetBundleUtility.MyAESDecrypted(ref bytes);
					_assetBundle = AssetBundle.LoadFromMemory(bytes);
				}
				else
				{
					_assetBundle = AssetBundle.LoadFromFile(LocalFilePath);
				}
			}
			catch (Exception)
			{
				_assetBundle = null;
			}
			if (_assetBundle == null)
			{
				if (File.Exists(LocalFilePath))
				{
					File.Delete(LocalFilePath);
				}
				Singleton<MainUIManager>.Instance.ShowDialog(new HintWithConfirmDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_AssetBundleError"), null, Application.Quit, LocalizationGeneralLogic.GetText("Menu_Tips")));
			}
		}

		public void Unload(bool forceUnload)
		{
			if (_assetBundle != null)
			{
				_assetBundle.Unload(forceUnload);
				_assetBundle = null;
			}
		}

		public override string ToString()
		{
			JSONClass jSONClass = new JSONClass();
			jSONClass.Add(FileNamePattern, new JSONData(FileName));
			jSONClass.Add(FileCompressedSizePattern, new JSONData((int)FileCompressedSize));
			jSONClass.Add(FileCrcPattern, new JSONData(FileCrc));
			jSONClass.Add(FileUnloadModePattern, new JSONData((int)FileUnloadMode));
			jSONClass.Add(FileDownloadModePattern, new JSONData((int)FileDownloadMode));
			jSONClass.Add(FileBundleTypePattern, new JSONData((int)FileBundleType));
			jSONClass.Add(RemainPattern, new JSONData(RemainInInstallPackage));
			jSONClass.Add(RemoteDirPattern, new JSONData(RemoteDir));
			JSONArray jSONArray = new JSONArray();
			foreach (string item in AssetPathSet)
			{
				jSONArray.Add(new JSONData(item));
			}
			jSONClass.Add(AssetPathSetPattern, jSONArray);
			JSONArray jSONArray2 = new JSONArray();
			foreach (string item2 in ParentFileNameSet)
			{
				jSONArray2.Add(new JSONData(item2));
			}
			jSONClass.Add(ParentFileNamePattern, jSONArray2);
			return jSONClass.ToString();
		}

		public static AssetBundleInfo FromString(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}
			try
			{
				JSONNode jSONNode = JSON.Parse(str);
				string name = jSONNode[FileNamePattern];
				long compressedSize = jSONNode[FileCompressedSizePattern].AsInt;
				string crc = jSONNode[FileCrcPattern];
				UnloadMode asInt = (UnloadMode)jSONNode[FileUnloadModePattern].AsInt;
				DownloadMode asInt2 = (DownloadMode)jSONNode[FileDownloadModePattern].AsInt;
				BundleType asInt3 = (BundleType)jSONNode[FileBundleTypePattern].AsInt;
				bool asBool = jSONNode[RemainPattern].AsBool;
				string remoteDir = jSONNode[RemoteDirPattern];
				HashSet<string> hashSet = new HashSet<string>();
				JSONArray asArray = jSONNode[AssetPathSetPattern].AsArray;
				for (int i = 0; i < asArray.Count; i++)
				{
					hashSet.Add(asArray[i]);
				}
				HashSet<string> hashSet2 = new HashSet<string>();
				JSONArray asArray2 = jSONNode[ParentFileNamePattern].AsArray;
				for (int j = 0; j < asArray2.Count; j++)
				{
					hashSet2.Add(asArray2[j]);
				}
				return new AssetBundleInfo(name, compressedSize, crc, hashSet2, hashSet, asInt, asInt2, asInt3, asBool, remoteDir);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
