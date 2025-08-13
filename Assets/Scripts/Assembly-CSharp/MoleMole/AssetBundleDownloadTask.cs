using System;
using System.Collections;
using UnityEngine;

namespace MoleMole
{
	public class AssetBundleDownloadTask
	{
		public Action<long, long, long, float> OnProgress { get; private set; }

		public Action<AssetBundleDownloadTask> OnFinished { get; private set; }

		public AssetBundleInfo AssetBundleInfo { get; private set; }

		public byte[] DownloadedBytes { get; private set; }

		public DownloadStatus CurrentStatus { get; private set; }

		public bool IsSuccess
		{
			get
			{
				return CurrentStatus == DownloadStatus.SUCCESS_DOWNLOADED;
			}
		}

		public bool IsFailed
		{
			get
			{
				return CurrentStatus == DownloadStatus.FAIL_DOWNLOADED;
			}
		}

		public AssetBundleDownloadTask(AssetBundleInfo assetBundleInfo, Action<long, long, long, float> onProgress, Action<AssetBundleDownloadTask> onFinished)
		{
			AssetBundleInfo = assetBundleInfo;
			OnProgress = onProgress;
			OnFinished = onFinished;
			DownloadedBytes = null;
			CurrentStatus = DownloadStatus.WAITING;
		}

		public IEnumerator StartDownload()
		{
			string url = AssetBundleInfo.RemoteFilePath;
			url = url + ((!url.Contains("?")) ? "?" : "&") + "t=" + TimeUtil.Now.ToString("yyyyMMddHHmmss");
			CurrentStatus = DownloadStatus.DOWNLOADING;
			long previousBytesDownloaded = 0L;
			float startTime = Time.realtimeSinceStartup;
			float previousTime = startTime;
			float downloadSpeed = 0f;
			if (OnProgress != null)
			{
				OnProgress(0L, AssetBundleInfo.FileCompressedSize, 0L, downloadSpeed);
			}
			WWW www = new WWW(url);
			while (!www.isDone)
			{
				float deltaTime = Time.realtimeSinceStartup - previousTime;
				if (deltaTime >= 0.5f)
				{
					long currentBytesDownloaded = (long)((float)AssetBundleInfo.FileCompressedSize * www.progress);
					long deltaBytesDonloaded = currentBytesDownloaded - previousBytesDownloaded;
					downloadSpeed = (float)deltaBytesDonloaded / deltaTime;
					previousTime = Time.realtimeSinceStartup;
					previousBytesDownloaded = currentBytesDownloaded;
					if (OnProgress != null)
					{
						OnProgress(currentBytesDownloaded, AssetBundleInfo.FileCompressedSize, deltaBytesDonloaded, downloadSpeed);
					}
				}
				yield return null;
			}
			if (www.error == null)
			{
				CurrentStatus = DownloadStatus.SUCCESS_DOWNLOADED;
				if (OnProgress != null)
				{
					OnProgress(AssetBundleInfo.FileCompressedSize, AssetBundleInfo.FileCompressedSize, AssetBundleInfo.FileCompressedSize - previousBytesDownloaded, 0f);
				}
				DownloadedBytes = www.bytes;
			}
			else
			{
				CurrentStatus = DownloadStatus.FAIL_DOWNLOADED;
				DownloadedBytes = null;
			}
			if (www != null)
			{
				if (www.assetBundle != null)
				{
					www.assetBundle.Unload(true);
				}
				www.Dispose();
				www = null;
			}
			if (OnFinished != null)
			{
				OnFinished(this);
			}
			Singleton<AssetBundleManager>.Instance.Loader.RemoveDownloadTask(this);
		}
	}
}
