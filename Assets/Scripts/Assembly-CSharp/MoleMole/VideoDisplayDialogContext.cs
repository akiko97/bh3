using System;
using System.Collections;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class VideoDisplayDialogContext : BaseDialogContext
	{
		public enum SkipButtonState
		{
			Open = 0,
			Close = 1,
			TransitFromOpen = 2,
			TransitFromClose = 3
		}

		private CgDataItem _currentCgDataItem;

		private MonoVideoPlayer _currentVideoPlayer;

		private GeneralDialogContext _currentGeneralDialog;

		private bool _withSkipBtn;

		private float _originalSkipBtnPosY;

		public Action SkipVideoClickedCallback;

		public Action SkipVideoConfirmCallback;

		public Action SkipVideoCancelCallback;

		private SkipButtonState _skipBtnState = SkipButtonState.Close;

		public VideoDisplayDialogContext(CgDataItem cgDataItem, MonoVideoPlayer videoPlayer, bool withSkipBtn = true)
		{
			config = new ContextPattern
			{
				contextName = "VideoDisplayDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/VideoDisplayDialog",
				ignoreNotify = true
			};
			_currentCgDataItem = cgDataItem;
			_currentVideoPlayer = videoPlayer;
			if (_currentVideoPlayer != null)
			{
				MonoVideoPlayer currentVideoPlayer = _currentVideoPlayer;
				currentVideoPlayer.OnVideoEnd = (Action<CgDataItem>)Delegate.Combine(currentVideoPlayer.OnVideoEnd, new Action<CgDataItem>(OnVideoEndCallback));
			}
			_withSkipBtn = withSkipBtn;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("SkipBtn").GetComponent<Button>(), OnSkipBtnClicked);
			BindViewCallback(base.view.transform.GetComponent<Button>(), OnBgBtnClicked);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("SkipBtn").gameObject.SetActive(false);
			return false;
		}

		public void OnBgBtnClicked()
		{
			if (!_withSkipBtn)
			{
				return;
			}
			Animation component = base.view.transform.GetComponent<Animation>();
			if (!(component == null) && !component.isPlaying)
			{
				if (_skipBtnState == SkipButtonState.Close)
				{
					component.Play("DisplayCgSkipBtn");
					_skipBtnState = SkipButtonState.TransitFromClose;
					StartTransitSkipBtn();
				}
				else if (_skipBtnState == SkipButtonState.Open)
				{
					component.Play("DisappearCgSkipBtn");
					_skipBtnState = SkipButtonState.TransitFromOpen;
					StartTransitSkipBtn();
				}
			}
		}

		private void StartTransitSkipBtn()
		{
			Singleton<ApplicationManager>.Instance.StartCoroutine(StartSkipBtnTransit());
		}

		private IEnumerator StartSkipBtnTransit()
		{
			Animation animation = base.view.transform.GetComponent<Animation>();
			if (!(animation == null))
			{
				while (animation.isPlaying)
				{
					yield return null;
				}
				if (_skipBtnState == SkipButtonState.TransitFromOpen)
				{
					_skipBtnState = SkipButtonState.Close;
				}
				else if (_skipBtnState == SkipButtonState.TransitFromClose)
				{
					_skipBtnState = SkipButtonState.Open;
				}
			}
		}

		public void OnSkipBtnClicked()
		{
			if (SkipVideoClickedCallback != null)
			{
				SkipVideoClickedCallback();
			}
			_currentGeneralDialog = new GeneralDialogContext
			{
				type = GeneralDialogContext.ButtonType.DoubleButton,
				title = LocalizationGeneralLogic.GetText("SkipCG_Confirm_Title"),
				desc = LocalizationGeneralLogic.GetText("SkipCG_Confirm_Content"),
				notDestroyAfterTouchBG = false,
				notDestroyAfterCallback = false,
				buttonCallBack = delegate(bool confirmed)
				{
					if (confirmed)
					{
						if (SkipVideoConfirmCallback != null)
						{
							base.view.transform.Find("SkipBtn").gameObject.SetActive(false);
							_skipBtnState = SkipButtonState.Close;
							SkipVideoConfirmCallback();
						}
					}
					else if (SkipVideoCancelCallback != null)
					{
						SkipVideoCancelCallback();
					}
				}
			};
			Singleton<MainUIManager>.Instance.ShowDialog(_currentGeneralDialog);
		}

		private void OnVideoEndCallback(CgDataItem dataItem)
		{
			if (_currentCgDataItem != null && _currentGeneralDialog != null)
			{
				_currentGeneralDialog.Destroy();
				if (_currentVideoPlayer != null)
				{
					MonoVideoPlayer currentVideoPlayer = _currentVideoPlayer;
					currentVideoPlayer.OnVideoEnd = (Action<CgDataItem>)Delegate.Remove(currentVideoPlayer.OnVideoEnd, new Action<CgDataItem>(OnVideoEndCallback));
				}
			}
		}
	}
}
