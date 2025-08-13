using System;
using System.Collections;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

namespace MoleMole
{
	public class MonoVideoPlayer : MonoBehaviour
	{
		public enum VideoControlType
		{
			Unload = 0,
			Play = 1,
			Load = 2
		}

		private MediaPlayer _mediaPlayer;

		private VideoDisplayDialogContext _currentDisplayDialog;

		private DisplayUGUI _videoDisplayUgui;

		private CgDataItem _currentCgDataItem;

		private bool _videoLoaded;

		private bool _videoStarted;

		private bool _endDestroyDisplay;

		public Action<CgDataItem> OnVideoEnd;

		public Action<CgDataItem> OnVideoBegin;

		private VideoControlType _currentControlType;

		public void Awake()
		{
			_mediaPlayer = base.transform.Find("MediaPlayer").GetComponent<MediaPlayer>();
			_mediaPlayer.Events.AddListener(OnVideoEvent);
			_mediaPlayer.gameObject.SetActive(false);
		}

		private void Start()
		{
			_currentCgDataItem = null;
			OnVideoBegin = null;
			OnVideoEnd = null;
			_videoLoaded = false;
			_videoStarted = false;
			_endDestroyDisplay = true;
		}

		public void LoadOrPlayVideo(CgDataItem cgDataItem, Action OverrideSkipCallback = null, Action<CgDataItem> OnVideoBeginCallback = null, Action<CgDataItem> OnVideoEndCallback = null, VideoControlType controlType = VideoControlType.Play, bool withSkipBtn = true, bool endDestroyDisplay = true)
		{
			if (_currentControlType == VideoControlType.Unload)
			{
				_currentDisplayDialog = new VideoDisplayDialogContext(cgDataItem, this, withSkipBtn);
				_currentDisplayDialog.SkipVideoConfirmCallback = ((OverrideSkipCallback == null) ? new Action(StartSkipWithFade) : OverrideSkipCallback);
				Singleton<MainUIManager>.Instance.ShowDialog(_currentDisplayDialog);
				_videoDisplayUgui = _currentDisplayDialog.view.GetComponentInChildren<DisplayUGUI>();
				if (_videoDisplayUgui != null)
				{
					_videoDisplayUgui.CurrentMediaPlayer = _mediaPlayer;
					_videoDisplayUgui.gameObject.SetActive(false);
				}
				_currentControlType = controlType;
				_mediaPlayer.gameObject.SetActive(true);
				_mediaPlayer.Stop();
				_mediaPlayer.CloseVideo();
				string path = string.Format("Video/{0}.mp4", cgDataItem.cgPath);
				if (!_mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, path, false))
				{
					_mediaPlayer.Events.RemoveAllListeners();
					_videoLoaded = false;
					_mediaPlayer.gameObject.SetActive(false);
				}
				_videoLoaded = true;
				_endDestroyDisplay = endDestroyDisplay;
				_currentCgDataItem = cgDataItem;
				OnVideoBegin = (Action<CgDataItem>)Delegate.Combine(OnVideoBegin, OnVideoBeginCallback);
				OnVideoEnd = (Action<CgDataItem>)Delegate.Combine(OnVideoEnd, OnVideoEndCallback);
			}
			else if (_currentControlType == VideoControlType.Load && controlType == VideoControlType.Play && _videoLoaded)
			{
				OnVideoStarted();
			}
		}

		private void Update()
		{
		}

		private void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, RenderHeads.Media.AVProVideo.ErrorCode ec)
		{
			switch (et)
			{
			case MediaPlayerEvent.EventType.ReadyToPlay:
				if (_currentControlType == VideoControlType.Play && !_videoStarted)
				{
					OnVideoStarted();
				}
				break;
			case MediaPlayerEvent.EventType.FirstFrameReady:
				StartCoroutine(WaitActiveUgui());
				break;
			case MediaPlayerEvent.EventType.FinishedPlaying:
			case MediaPlayerEvent.EventType.Error:
				OnVideoEnded();
				break;
			case MediaPlayerEvent.EventType.Started:
			case MediaPlayerEvent.EventType.Closing:
				break;
			}
		}

		private IEnumerator WaitActiveUgui()
		{
			if (!(_videoDisplayUgui == null))
			{
				_videoDisplayUgui.color = new Color(0f, 0f, 0f, 0f);
				yield return new WaitForSeconds(0.1f);
				_videoDisplayUgui.gameObject.SetActive(true);
				_videoDisplayUgui.color = Color.white;
			}
		}

		private void OnVideoStarted()
		{
			_currentControlType = VideoControlType.Play;
			SetGameAudioEnabled(false);
			_mediaPlayer.Play();
			_videoStarted = true;
			if (_currentCgDataItem != null && OnVideoBegin != null)
			{
				OnVideoBegin(_currentCgDataItem);
			}
			Screen.sleepTimeout = -1;
		}

		private void OnVideoEnded()
		{
			SetGameAudioEnabled(true);
			_currentControlType = VideoControlType.Unload;
			_videoLoaded = false;
			_videoStarted = false;
			_mediaPlayer.gameObject.SetActive(false);
			if (_currentCgDataItem != null && OnVideoEnd != null)
			{
				OnVideoEnd(_currentCgDataItem);
			}
			if (_endDestroyDisplay && _currentDisplayDialog != null)
			{
				_currentDisplayDialog.Destroy();
			}
			Screen.sleepTimeout = -2;
		}

		public void DestroyCurrentDisplayDialog()
		{
			if (_currentDisplayDialog != null)
			{
				_currentDisplayDialog.Destroy();
				_currentDisplayDialog = null;
			}
		}

		public void SkipCgDisplay()
		{
			if (_mediaPlayer != null)
			{
				_mediaPlayer.CloseVideo();
				OnVideoEnded();
			}
		}

		public void StartSkipWithFade()
		{
			StartCoroutine(fadeSkipIter());
		}

		private IEnumerator fadeSkipIter()
		{
			if (_mediaPlayer == null || _videoDisplayUgui == null || !_videoDisplayUgui.gameObject.activeSelf)
			{
				yield break;
			}
			float fadeRatio = 1f;
			while (fadeRatio > 0f)
			{
				if (!_videoDisplayUgui.gameObject.activeSelf)
				{
					yield break;
				}
				Color preColor = _videoDisplayUgui.color;
				_videoDisplayUgui.color = new Color(preColor.r, preColor.g, preColor.b, fadeRatio);
				fadeRatio -= Time.deltaTime * 2f;
				yield return null;
			}
			SkipCgDisplay();
		}

		private void SetGameAudioEnabled(bool enabled)
		{
			if (enabled)
			{
				Singleton<WwiseAudioManager>.Instance.Post("UI_CG_Exit");
			}
			else
			{
				Singleton<WwiseAudioManager>.Instance.Post("UI_CG_Enter_Long");
			}
		}
	}
}
