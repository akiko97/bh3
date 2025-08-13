using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MoleMole
{
	public class AssetBundleManager
	{
		private Dictionary<BundleType, Dictionary<string, AssetBundleInfo>> _fileName2AssetBundleDict;

		private Dictionary<BundleType, Dictionary<string, string>> _assetName2FileNameDict;

		private Dictionary<BundleType, Dictionary<string, string>> _resPath2AssetNameDict;

		public string remoteAssetBundleUrl;

		public MonoAssetBundleLoader Loader { get; private set; }

		private AssetBundleManager()
		{
			_fileName2AssetBundleDict = new Dictionary<BundleType, Dictionary<string, AssetBundleInfo>>();
			_fileName2AssetBundleDict.Add(BundleType.DATA_FILE, new Dictionary<string, AssetBundleInfo>());
			_fileName2AssetBundleDict.Add(BundleType.RESOURCE_FILE, new Dictionary<string, AssetBundleInfo>());
			_assetName2FileNameDict = new Dictionary<BundleType, Dictionary<string, string>>();
			_assetName2FileNameDict.Add(BundleType.DATA_FILE, new Dictionary<string, string>());
			_assetName2FileNameDict.Add(BundleType.RESOURCE_FILE, new Dictionary<string, string>());
			_resPath2AssetNameDict = new Dictionary<BundleType, Dictionary<string, string>>();
			_resPath2AssetNameDict.Add(BundleType.DATA_FILE, new Dictionary<string, string>());
			_resPath2AssetNameDict.Add(BundleType.RESOURCE_FILE, new Dictionary<string, string>());
			InitAssetBundleLoader();
		}

		private void InitAssetBundleLoader()
		{
			if (Loader == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "AssetBundleLoader";
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				Loader = gameObject.AddComponent<MonoAssetBundleLoader>();
			}
		}

		public void Destroy()
		{
			foreach (BundleType key in _fileName2AssetBundleDict.Keys)
			{
				ClearAssetBundle(key);
			}
			if (Loader != null)
			{
				UnityEngine.Object.Destroy(Loader.gameObject);
			}
		}

		public void SetAssetBundleDict(BundleType bundleType, Dictionary<string, AssetBundleInfo> fileName2AssetBundleDict)
		{
			_fileName2AssetBundleDict[bundleType] = fileName2AssetBundleDict;
			foreach (KeyValuePair<string, AssetBundleInfo> item in fileName2AssetBundleDict)
			{
				foreach (string item2 in item.Value.AssetPathSet)
				{
					_assetName2FileNameDict[bundleType][item2] = item.Key;
					string resourcePath = AssetBundleUtility.GetResourcePath(item2);
					_resPath2AssetNameDict[bundleType][resourcePath] = item2;
				}
			}
		}

		public void MergeAssetBundleDictOnRequire(BundleType bundleType, Dictionary<string, AssetBundleInfo> fileName2AssetBundleDict)
		{
			foreach (string key in fileName2AssetBundleDict.Keys)
			{
				if (_fileName2AssetBundleDict[bundleType].ContainsKey(key) || fileName2AssetBundleDict[key].FileDownloadMode != DownloadMode.ON_REQUIRE)
				{
					continue;
				}
				_fileName2AssetBundleDict[bundleType][key] = fileName2AssetBundleDict[key];
				foreach (string item in _fileName2AssetBundleDict[bundleType][key].AssetPathSet)
				{
					_assetName2FileNameDict[bundleType][item] = key;
					string resourcePath = AssetBundleUtility.GetResourcePath(item);
					_resPath2AssetNameDict[bundleType][resourcePath] = item;
				}
			}
		}

		public Dictionary<string, AssetBundleInfo> GetAssetBundleInfoDict(BundleType bundleType)
		{
			return _fileName2AssetBundleDict[bundleType];
		}

		public AssetBundleInfo GetAssetBundleInfoByFileName(BundleType bundleType, string fileName)
		{
			if (_fileName2AssetBundleDict[bundleType].ContainsKey(fileName))
			{
				return _fileName2AssetBundleDict[bundleType][fileName];
			}
			return null;
		}

		public string GetAssetNameByResPath(BundleType bundleType, string resPath)
		{
			if (_resPath2AssetNameDict[bundleType].ContainsKey(resPath))
			{
				return _resPath2AssetNameDict[bundleType][resPath];
			}
			return null;
		}

		public string GetFileNameByAssetName(BundleType bundleType, string assetName)
		{
			if (_assetName2FileNameDict[bundleType].ContainsKey(assetName))
			{
				return _assetName2FileNameDict[bundleType][assetName];
			}
			return null;
		}

		public AssetBundleInfo GetAssetBundleInfoByAssetName(BundleType bundleType, string assetName)
		{
			string fileNameByAssetName = GetFileNameByAssetName(bundleType, assetName);
			if (fileNameByAssetName != null)
			{
				return GetAssetBundleInfoByFileName(bundleType, fileNameByAssetName);
			}
			return null;
		}

		public AssetBundleInfo GetAssetBundleInfoByResPath(BundleType bundleType, string resPath)
		{
			string assetNameByResPath = Singleton<AssetBundleManager>.Instance.GetAssetNameByResPath(bundleType, resPath);
			return (!string.IsNullOrEmpty(assetNameByResPath)) ? Singleton<AssetBundleManager>.Instance.GetAssetBundleInfoByAssetName(BundleType.RESOURCE_FILE, assetNameByResPath) : null;
		}

		public string GetDataVersion()
		{
			AssetBundleInfo assetBundleInfoByFileName = GetAssetBundleInfoByFileName(BundleType.DATA_FILE, "Data/all");
			if (assetBundleInfoByFileName != null)
			{
				return assetBundleInfoByFileName.FileCrc;
			}
			return "NoData";
		}

		public string GetAssetBoundleStatusFilePath()
		{
			string text = "/data/build.status";
			return remoteAssetBundleUrl + text;
		}

		public string GetEventAssetBoundleStatusFilePath()
		{
			string text = "/event/build.status";
			return remoteAssetBundleUrl + text;
		}

		public T LoadData<T>(string resPath) where T : UnityEngine.Object
		{
			if (!GlobalVars.DataUseAssetBundle)
			{
				return Resources.Load<T>(resPath);
			}
			return Load<T>(BundleType.DATA_FILE, resPath);
		}

		public T LoadRes<T>(string resPath) where T : UnityEngine.Object
		{
			if (!GlobalVars.ResourceUseAssetBundle)
			{
				return Resources.Load<T>(resPath);
			}
			return Load<T>(BundleType.RESOURCE_FILE, resPath);
		}

		private T Load<T>(BundleType bundleType, string resPath) where T : UnityEngine.Object
		{
			string assetNameByResPath = GetAssetNameByResPath(bundleType, resPath);
			AssetBundleInfo assetBundleInfo = ((!string.IsNullOrEmpty(assetNameByResPath)) ? GetAssetBundleInfoByAssetName(bundleType, assetNameByResPath) : null);
			if (assetBundleInfo == null || !assetBundleInfo.IsDownloaded())
			{
				return Resources.Load<T>(resPath);
			}
			T result = (T)null;
			switch ((int)((GlobalVars.UseSpliteResources ? 1 : 0) * 3 + assetBundleInfo.FileDownloadMode))
			{
			case 0:
				result = Resources.Load<T>(resPath);
				break;
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
				result = assetBundleInfo.Load<T>(assetNameByResPath);
				break;
			}
			return result;
		}

		public AsyncAssetRequst LoadDataAsync(string resPath)
		{
			if (!GlobalVars.DataUseAssetBundle)
			{
				ResourceRequest operation = Resources.LoadAsync(resPath);
				return new AsyncAssetRequst(operation);
			}
			return LoadAsync(BundleType.DATA_FILE, resPath);
		}

		public AsyncAssetRequst LoadResAsync(string resPath)
		{
			if (!GlobalVars.ResourceUseAssetBundle)
			{
				ResourceRequest operation = Resources.LoadAsync(resPath);
				return new AsyncAssetRequst(operation);
			}
			return LoadAsync(BundleType.RESOURCE_FILE, resPath);
		}

		private AsyncAssetRequst LoadAsync(BundleType bundleType, string resPath)
		{
			string assetNameByResPath = GetAssetNameByResPath(bundleType, resPath);
			AssetBundleInfo assetBundleInfo = ((!string.IsNullOrEmpty(assetNameByResPath)) ? GetAssetBundleInfoByAssetName(bundleType, assetNameByResPath) : null);
			if (assetBundleInfo == null || !assetBundleInfo.IsDownloaded())
			{
				ResourceRequest operation = Resources.LoadAsync(resPath);
				return new AsyncAssetRequst(operation);
			}
			AsyncAssetRequst result = null;
			switch ((int)((GlobalVars.UseSpliteResources ? 1 : 0) * 3 + assetBundleInfo.FileDownloadMode))
			{
			case 0:
			{
				ResourceRequest operation3 = Resources.LoadAsync(resPath);
				result = new AsyncAssetRequst(operation3);
				break;
			}
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			{
				AssetBundleRequest operation2 = assetBundleInfo.LoadAsync(assetNameByResPath);
				result = new AsyncAssetRequst(operation2);
				break;
			}
			}
			return result;
		}

		public void UnloadUnusedAssetBundle(BundleType bundleType)
		{
			foreach (AssetBundleInfo value in _fileName2AssetBundleDict[bundleType].Values)
			{
				if (value != null && value.IsLoaded() && value.FileUnloadMode == UnloadMode.MANUAL_UNLOAD)
				{
					value.Unload(false);
				}
			}
		}

		public void ClearAssetBundle(BundleType bundleType)
		{
			foreach (AssetBundleInfo value in _fileName2AssetBundleDict[bundleType].Values)
			{
				if (value != null)
				{
					value.Unload(true);
				}
			}
			_fileName2AssetBundleDict[bundleType].Clear();
			_assetName2FileNameDict[bundleType].Clear();
			_resPath2AssetNameDict[bundleType].Clear();
		}

		public static void RemoveAllAssetBundle(BundleType bundleType)
		{
			string path = AssetBundleUtility.LocalAssetBundleDirectory(bundleType);
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
		}

		public void CheckSVNVersion()
		{
			if (GlobalVars.DataUseAssetBundle && !(TimeUtil.Now < Loader.checkSVNVersionDate.AddSeconds(MiscData.Config.BasicConfig.CheckAssetBoundleIntervalSecond)))
			{
				Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(GetAssetBoundleStatusFilePath(), GetAssetBoundleStatusCallBack, null));
			}
		}

		private void GetAssetBoundleStatusCallBack(string version)
		{
			string text = version.Trim();
			Loader.checkSVNVersionDate = TimeUtil.Now;
			if (string.IsNullOrEmpty(Loader.assetBoundleSVNVersion))
			{
				Loader.assetBoundleSVNVersion = text;
			}
			else if (Loader.assetBoundleSVNVersion != text)
			{
				Loader.assetBoundleSVNVersion = text;
				Singleton<MainUIManager>.Instance.ShowDialog(new HintWithConfirmDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_NeedToRestartGame"), null, GeneralLogicManager.RestartGame, LocalizationGeneralLogic.GetText("Menu_Tips")));
			}
		}

		public void UpdateEventSVNVersion(Action OnChangedCallback = null)
		{
			if (!GlobalVars.ResourceUseAssetBundle)
			{
				return;
			}
			Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(GetEventAssetBoundleStatusFilePath(), delegate(string version)
			{
				string text = version.Trim();
				if (Loader.eventAssetBoundleSVNVersion != text)
				{
					Loader.eventAssetBoundleSVNVersion = text;
					if (OnChangedCallback != null)
					{
						OnChangedCallback();
					}
				}
			}, null));
		}
	}
}
