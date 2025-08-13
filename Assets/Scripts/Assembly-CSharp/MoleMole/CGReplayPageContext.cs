using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class CGReplayPageContext : BasePageContext
	{
		private MonoGridScroller _cgScroller;

		private MonoScrollerFadeManager _fadeMgr;

		private Dictionary<int, RectTransform> _dictBeforeFetch;

		public CGReplayPageContext()
		{
			config = new ContextPattern
			{
				contextName = "CGReplayPageContext",
				viewPrefabPath = "UI/Menus/Page/CGReplay/CGReplay"
			};
		}

		protected override void BindViewCallbacks()
		{
		}

		public override bool OnNotify(Notify ntf)
		{
			return false;
		}

		protected override bool SetupView()
		{
			if (base.view == null || base.view.transform == null)
			{
				return false;
			}
			FetchWidget();
			SetupAchieveInfoScroller();
			BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
			if (mainCanvas != null)
			{
				MonoMainCanvas monoMainCanvas = mainCanvas as MonoMainCanvas;
				if (monoMainCanvas != null)
				{
					MonoVideoPlayer videoPlayer = monoMainCanvas.VideoPlayer;
					videoPlayer.OnVideoBegin = (Action<CgDataItem>)Delegate.Combine(videoPlayer.OnVideoBegin, new Action<CgDataItem>(OnVideoBegin));
					MonoVideoPlayer videoPlayer2 = monoMainCanvas.VideoPlayer;
					videoPlayer2.OnVideoEnd = (Action<CgDataItem>)Delegate.Combine(videoPlayer2.OnVideoEnd, new Action<CgDataItem>(OnVideoEnd));
				}
			}
			return false;
		}

		public override void Destroy()
		{
			base.Destroy();
			BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
			if (mainCanvas != null)
			{
				MonoMainCanvas monoMainCanvas = mainCanvas as MonoMainCanvas;
				if (monoMainCanvas != null)
				{
					MonoVideoPlayer videoPlayer = monoMainCanvas.VideoPlayer;
					videoPlayer.OnVideoEnd = (Action<CgDataItem>)Delegate.Remove(videoPlayer.OnVideoEnd, new Action<CgDataItem>(OnVideoBegin));
					MonoVideoPlayer videoPlayer2 = monoMainCanvas.VideoPlayer;
					videoPlayer2.OnVideoEnd = (Action<CgDataItem>)Delegate.Remove(videoPlayer2.OnVideoEnd, new Action<CgDataItem>(OnVideoEnd));
				}
			}
		}

		private void FetchWidget()
		{
			_cgScroller = base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoGridScroller>();
		}

		private void SetupAchieveInfoScroller()
		{
			List<CgDataItem> cgDataItemList = Singleton<CGModule>.Instance.GetCgDataItemList();
			if (_cgScroller == null)
			{
				return;
			}
			_cgScroller.Init(delegate(Transform trans, int index)
			{
				MonoCgIconButton component = trans.GetComponent<MonoCgIconButton>();
				if (!(component == null))
				{
					component.SetupView(cgDataItemList[index]);
					component.SetClickCallback(OnCgIconButtonClicked);
				}
			}, cgDataItemList.Count);
			_fadeMgr = base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_fadeMgr.Init(_cgScroller.GetItemDict(), _dictBeforeFetch, IsMissionEqual);
			_fadeMgr.Play();
			_dictBeforeFetch = null;
		}

		private void OnCgIconButtonClicked(CgDataItem data)
		{
			BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
			if (mainCanvas is MonoMainCanvas)
			{
				(mainCanvas as MonoMainCanvas).PlayVideo(data);
			}
		}

		private bool IsMissionEqual(RectTransform missionNew, RectTransform missionOld)
		{
			if (missionNew == null || missionOld == null)
			{
				return false;
			}
			MonoCgIconButton component = missionOld.GetComponent<MonoCgIconButton>();
			MonoCgIconButton component2 = missionNew.GetComponent<MonoCgIconButton>();
			return component2._item.cgID == component._item.cgID;
		}

		private void OnVideoBegin(CgDataItem cgDataItem)
		{
			BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
			if (mainCanvas != null)
			{
				MonoMainCanvas monoMainCanvas = mainCanvas as MonoMainCanvas;
				if (monoMainCanvas != null)
				{
					SetStarEffectActive(false);
				}
			}
		}

		private void OnVideoEnd(CgDataItem cgDataItem)
		{
			BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
			if (mainCanvas != null)
			{
				MonoMainCanvas monoMainCanvas = mainCanvas as MonoMainCanvas;
				if (monoMainCanvas != null)
				{
					SetStarEffectActive(true);
				}
			}
		}

		private void SetStarEffectActive(bool active)
		{
			base.view.transform.Find("MovingStars").gameObject.SetActive(active);
		}
	}
}
