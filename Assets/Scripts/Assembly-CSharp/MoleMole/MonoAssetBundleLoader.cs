using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace MoleMole
{
	public class MonoAssetBundleLoader : MonoBehaviour
	{
		private List<AssetBundleDownloadTask> _downloadTaskList;

		private Dictionary<string, AssetBundleDownloadTask> _downloadTaskDict;

		private BackGroundWorker _zipBackGroundWorker;

		public DateTime checkSVNVersionDate;

		public string assetBoundleSVNVersion;

		public string eventAssetBoundleSVNVersion;

		private List<AssetBundleInfo> _downloadAssetBundleList;

		public void Awake()
		{
			_downloadTaskList = new List<AssetBundleDownloadTask>();
			_downloadTaskDict = new Dictionary<string, AssetBundleDownloadTask>();
			_zipBackGroundWorker = new BackGroundWorker();
		}

		public bool AddDownloadTask(AssetBundleDownloadTask downloadTask)
		{
			if (downloadTask == null)
			{
				return false;
			}
			string remoteFilePath = downloadTask.AssetBundleInfo.RemoteFilePath;
			if (_downloadTaskDict.ContainsKey(remoteFilePath))
			{
				return false;
			}
			_downloadTaskDict.Add(remoteFilePath, downloadTask);
			_downloadTaskList.Add(downloadTask);
			StartCoroutine(downloadTask.StartDownload());
			return true;
		}

		public void RemoveDownloadTask(AssetBundleDownloadTask downloadTask)
		{
			if (downloadTask == null)
			{
				return;
			}
			string remoteFilePath = downloadTask.AssetBundleInfo.RemoteFilePath;
			if (_downloadTaskDict.ContainsKey(remoteFilePath))
			{
				_downloadTaskDict.Remove(remoteFilePath);
			}
			for (int i = 0; i < _downloadTaskList.Count; i++)
			{
				if (_downloadTaskList[i] == downloadTask)
				{
					_downloadTaskList.RemoveAt(i);
					break;
				}
			}
		}

		public void LoadVersionFile(BundleType bundleType, Action<long, long, long, float> onProgress, Action<bool> onFinished)
		{
			AssetBundleInfo[] versionAssetBundleInfo = AssetBundleUtility.GetVersionAssetBundleInfo(bundleType);
			if (versionAssetBundleInfo == null || versionAssetBundleInfo.Length <= 0)
			{
				return;
			}
			Dictionary<string, AssetBundleInfo> fileName2AssetBundleDict = new Dictionary<string, AssetBundleInfo>();
			AssetBundleDownloadTask[] array = new AssetBundleDownloadTask[versionAssetBundleInfo.Length];
			for (int num = versionAssetBundleInfo.Length - 1; num >= 0; num--)
			{
				if (num == versionAssetBundleInfo.Length - 1)
				{
					array[num] = new AssetBundleDownloadTask(versionAssetBundleInfo[num], onProgress, delegate(AssetBundleDownloadTask task)
					{
						try
						{
							if (task.IsSuccess)
							{
								ParseVersionFile(bundleType, task, ref fileName2AssetBundleDict);
								Singleton<AssetBundleManager>.Instance.ClearAssetBundle(bundleType);
								Singleton<AssetBundleManager>.Instance.SetAssetBundleDict(bundleType, fileName2AssetBundleDict);
								MakeDownloadList(bundleType);
								onFinished(true);
							}
							else if (task.IsFailed)
							{
								onFinished(false);
							}
						}
						catch (Exception)
						{
							onFinished(false);
						}
					});
				}
				else
				{
					AssetBundleDownloadTask nextTask = array[num + 1];
					array[num] = new AssetBundleDownloadTask(versionAssetBundleInfo[num], onProgress, delegate(AssetBundleDownloadTask task)
					{
						try
						{
							if (task.IsSuccess)
							{
								ParseVersionFile(bundleType, task, ref fileName2AssetBundleDict);
								AddDownloadTask(nextTask);
							}
							else if (task.IsFailed)
							{
								onFinished(false);
							}
						}
						catch (Exception)
						{
							onFinished(false);
						}
					});
				}
			}
			AddDownloadTask(array[0]);
		}

		private void ParseVersionFile(BundleType bundleType, AssetBundleDownloadTask task, ref Dictionary<string, AssetBundleInfo> fileNames)
		{
			byte[] downloadedBytes = task.DownloadedBytes;
			for (int i = 0; i < downloadedBytes.Length; i++)
			{
				downloadedBytes[i] = (byte)(AssetBundleUtility.BYTE_SALT ^ downloadedBytes[i]);
			}
			AssetBundle assetBundle = AssetBundle.LoadFromMemory(downloadedBytes);
			TextAsset textAsset = assetBundle.LoadAsset("PackageVersion") as TextAsset;
			int num = 0;
			string[] array = textAsset.text.Split("\n"[0]);
			if (array.Length > 0 && bundleType == BundleType.DATA_FILE)
			{
				AssetBundleUtility.ENCRYPTED_KEY = GetEncryptedKeyBytes(array[0]);
				num = 1;
			}
			for (int j = num; j < array.Length; j++)
			{
				AssetBundleInfo assetBundleInfo = AssetBundleInfo.FromString(array[j]);
				if (assetBundleInfo != null)
				{
					fileNames[assetBundleInfo.FileName] = assetBundleInfo;
				}
			}
			assetBundle.Unload(true);
		}

		private byte[] GetEncryptedKeyBytes(string keys)
		{
			byte[] array = new byte[keys.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = byte.Parse(keys.Substring(i * 2, 2), NumberStyles.HexNumber);
			}
			return array;
		}

		public void MakeDownloadList(BundleType bundleType)
		{
			string text = AssetBundleUtility.LocalAssetBundleDirectory(bundleType);
			AssetBundleUtility.CreateParentDirectory(text);
			string[] files = Directory.GetFiles(text, "*.unity3d", SearchOption.AllDirectories);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 0; i < files.Length; i++)
			{
				string key = files[i].Substring(text.Length, files[i].LastIndexOf("_") - text.Length).Replace('\\', '/');
				string value = files[i].Substring(files[i].LastIndexOf("_") + 1, files[i].LastIndexOf(".") - files[i].LastIndexOf("_") - 1);
				if (dictionary.ContainsKey(key))
				{
					File.Delete(files[i].Replace('\\', '/'));
				}
				else
				{
					dictionary.Add(key, value);
				}
			}
			Dictionary<string, AssetBundleInfo> assetBundleInfoDict = Singleton<AssetBundleManager>.Instance.GetAssetBundleInfoDict(bundleType);
			HashSet<string> hashSet = new HashSet<string>();
			hashSet.UnionWith(dictionary.Keys);
			hashSet.UnionWith(assetBundleInfoDict.Keys);
			List<string> list = new List<string>();
			List<AssetBundleInfo> list2 = new List<AssetBundleInfo>();
			foreach (string item in hashSet)
			{
				bool flag = dictionary.ContainsKey(item);
				if (!assetBundleInfoDict.ContainsKey(item))
				{
					list.Add(text + item + "_" + dictionary[item] + ".unity3d");
					continue;
				}
				if (!flag)
				{
					switch ((int)((GlobalVars.UseSpliteResources ? 1 : 0) * 3 + assetBundleInfoDict[item].FileDownloadMode))
					{
					case 1:
					case 3:
					case 4:
						list2.Add(assetBundleInfoDict[item]);
						break;
					}
					continue;
				}
				switch ((int)((GlobalVars.UseSpliteResources ? 1 : 0) * 3 + assetBundleInfoDict[item].FileDownloadMode))
				{
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
					if (dictionary[item] != assetBundleInfoDict[item].FileCrc)
					{
						list.Add(text + item + "_" + dictionary[item] + ".unity3d");
						list2.Add(assetBundleInfoDict[item]);
					}
					break;
				}
			}
			foreach (string item2 in list)
			{
				if (File.Exists(item2))
				{
					File.Delete(item2);
				}
			}
			_downloadAssetBundleList = new List<AssetBundleInfo>();
			List<AssetBundleInfo> list3 = new List<AssetBundleInfo>();
			for (int j = 0; j < list2.Count; j++)
			{
				AssetBundleInfo assetBundleInfo = list2[j];
				if (assetBundleInfo.ParentFileNameSet != null)
				{
					foreach (string item3 in assetBundleInfo.ParentFileNameSet)
					{
						AssetBundleInfo assetBundleInfoByFileName = Singleton<AssetBundleManager>.Instance.GetAssetBundleInfoByFileName(bundleType, item3);
						if (assetBundleInfoByFileName != null && !list3.Contains(assetBundleInfoByFileName))
						{
							list3.Add(assetBundleInfoByFileName);
						}
					}
				}
				_downloadAssetBundleList.Add(assetBundleInfo);
			}
			for (int k = 0; k < list3.Count; k++)
			{
				string fileName = list3[k].FileName;
				bool flag2 = dictionary.ContainsKey(fileName);
				bool flag3 = assetBundleInfoDict.ContainsKey(fileName);
				if ((!flag2 || (flag3 && dictionary[fileName] != assetBundleInfoDict[fileName].FileCrc)) && !_downloadAssetBundleList.Contains(list3[k]))
				{
					_downloadAssetBundleList.Add(list3[k]);
				}
			}
		}

		public long GetDownloadAssetBundleTotalSize()
		{
			long num = 0L;
			if (_downloadAssetBundleList != null)
			{
				for (int i = 0; i < _downloadAssetBundleList.Count; i++)
				{
					num += _downloadAssetBundleList[i].FileCompressedSize;
				}
			}
			return num;
		}

		public IEnumerator StartDownloadAssetBundle(BundleType bundleType, Action<long, long, long, float> onDownloadProgress, Action<int> onZipProgress, Action<bool> onFinished)
		{
			Screen.sleepTimeout = -1;
			if (_downloadAssetBundleList != null)
			{
				long totalAssetBundleBytes = GetDownloadAssetBundleTotalSize();
				long downloadedAssetBundleBytes = 0L;
				ZipStatus zipStatus = ZipStatus.ZIPPING;
				_zipBackGroundWorker.StartBackGroundWork(string.Empty);
				int currentTaskIndex;
				Action<long, long, long, float> onDownloadProgress2 = default(Action<long, long, long, float>);
				BundleType bundleType2 = default(BundleType);
				for (currentTaskIndex = 0; currentTaskIndex < _downloadAssetBundleList.Count; currentTaskIndex++)
				{
					AssetBundleInfo asbInfo = _downloadAssetBundleList[currentTaskIndex];
					DownloadStatus downloadStatus = DownloadStatus.DOWNLOADING;
					AddDownloadTask(new AssetBundleDownloadTask(asbInfo, delegate(long current, long total, long delta, float speed)
					{
						onDownloadProgress2(downloadedAssetBundleBytes + current, totalAssetBundleBytes, delta, speed);
					}, delegate(AssetBundleDownloadTask downloadTask)
					{
						downloadStatus = downloadTask.CurrentStatus;
						if (downloadStatus == DownloadStatus.SUCCESS_DOWNLOADED)
						{
							_zipBackGroundWorker.AddBackGroundWork(delegate
							{
								if (!AssetBundleUtility.ValidateAndSaveAssetBundle(bundleType2, downloadTask))
								{
									zipStatus = ZipStatus.FAIL_ZIPPED;
									_zipBackGroundWorker.StopBackGroundWork();
								}
							});
							downloadedAssetBundleBytes += downloadTask.AssetBundleInfo.FileCompressedSize;
						}
					}));
					while (downloadStatus == DownloadStatus.DOWNLOADING && zipStatus == ZipStatus.ZIPPING)
					{
						yield return null;
					}
					if (downloadStatus != DownloadStatus.SUCCESS_DOWNLOADED || zipStatus != ZipStatus.ZIPPING)
					{
						break;
					}
				}
				while (zipStatus == ZipStatus.ZIPPING && _zipBackGroundWorker.RemainCount > 0)
				{
					if (onZipProgress != null)
					{
						onZipProgress(_zipBackGroundWorker.RemainCount);
					}
					yield return null;
				}
				switch (zipStatus)
				{
				case ZipStatus.ZIPPING:
					zipStatus = ZipStatus.SUCCESS_ZIPPED;
					_zipBackGroundWorker.StopBackGroundWork();
					break;
				}
				if (onFinished != null)
				{
					onFinished(currentTaskIndex >= _downloadAssetBundleList.Count && zipStatus == ZipStatus.SUCCESS_ZIPPED);
				}
				_downloadAssetBundleList.Clear();
				_downloadAssetBundleList = null;
			}
			Screen.sleepTimeout = -2;
		}

		public void TryStartDownloadOneAssetBundle(string resPath, Action<long, long, long, float> onDownloadProgress, Action<bool> onFinished)
		{
			AssetBundleInfo assetBundleInfoByResPath = Singleton<AssetBundleManager>.Instance.GetAssetBundleInfoByResPath(BundleType.RESOURCE_FILE, resPath);
			if (assetBundleInfoByResPath == null)
			{
				Singleton<AssetBundleManager>.Instance.UpdateEventSVNVersion(delegate
				{
					CheckEventVersionAndDownloadOneAssetBundle(resPath, onDownloadProgress, onFinished);
				});
			}
			else
			{
				DoStartDownloadOneAssetBundle(assetBundleInfoByResPath, onDownloadProgress, onFinished);
			}
		}

		private void CheckEventVersionAndDownloadOneAssetBundle(string resPath, Action<long, long, long, float> onDownloadProgress, Action<bool> onFinished)
		{
			AssetBundleInfo assetBundleInfo = new AssetBundleInfo("ResourceVersion", 100L, string.Empty, null, null, UnloadMode.MANUAL_UNLOAD, DownloadMode.IMMEDIATELY, BundleType.RESOURCE_FILE, false, "event");
			AddDownloadTask(new AssetBundleDownloadTask(assetBundleInfo, null, delegate(AssetBundleDownloadTask task)
			{
				try
				{
					if (task.IsSuccess)
					{
						Dictionary<string, AssetBundleInfo> fileNames = new Dictionary<string, AssetBundleInfo>();
						ParseVersionFile(BundleType.RESOURCE_FILE, task, ref fileNames);
						Singleton<AssetBundleManager>.Instance.MergeAssetBundleDictOnRequire(BundleType.RESOURCE_FILE, fileNames);
						AssetBundleInfo assetBundleInfoByResPath = Singleton<AssetBundleManager>.Instance.GetAssetBundleInfoByResPath(BundleType.RESOURCE_FILE, resPath);
						if (assetBundleInfoByResPath != null)
						{
							DoStartDownloadOneAssetBundle(assetBundleInfoByResPath, onDownloadProgress, onFinished);
						}
						else
						{
							onFinished(false);
						}
					}
					else if (task.IsFailed)
					{
						onFinished(false);
					}
				}
				catch (Exception)
				{
					onFinished(false);
				}
			}));
		}

		private void DoStartDownloadOneAssetBundle(AssetBundleInfo asbInfo, Action<long, long, long, float> onDownloadProgress, Action<bool> onFinished)
		{
			AssetBundleDownloadTask downloadTask = new AssetBundleDownloadTask(asbInfo, delegate(long current, long total, long delta, float speed)
			{
				if (onDownloadProgress != null)
				{
					onDownloadProgress(current, total, delta, speed);
				}
			}, delegate(AssetBundleDownloadTask task)
			{
				bool obj = false;
				if (task.CurrentStatus == DownloadStatus.SUCCESS_DOWNLOADED && AssetBundleUtility.ValidateAndSaveAssetBundle(BundleType.RESOURCE_FILE, task))
				{
					obj = true;
				}
				if (onFinished != null)
				{
					onFinished(obj);
				}
			});
			AddDownloadTask(downloadTask);
		}

		public bool IsDownloadCompleted()
		{
			return _downloadAssetBundleList == null || _downloadAssetBundleList.Count == 0;
		}
	}
}
