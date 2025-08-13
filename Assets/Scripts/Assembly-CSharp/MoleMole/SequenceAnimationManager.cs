using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class SequenceAnimationManager
	{
		private List<MonoAnimationinSequence> _animationList;

		private int _index;

		private Action _onAllAnimationEnd;

		private Action<Transform> _onEveryAnimation;

		private CanvasTimer _startDelayTimer;

		private int _lastIndex;

		private bool _lockUI;

		public bool IsPlaying;

		public SequenceAnimationManager(Action allAnimationEndCallBack = null, Action<Transform> onEveryAnimation = null)
		{
			_animationList = new List<MonoAnimationinSequence>();
			_index = 0;
			_lastIndex = -1;
			_onAllAnimationEnd = allAnimationEndCallBack;
			_onEveryAnimation = onEveryAnimation;
		}

		public void ClearAnimations()
		{
			_animationList.Clear();
		}

		public void AddAnimation(MonoAnimationinSequence animation, Action<Transform> endCallBack = null)
		{
			animation.SetAnimationEndCallBack(OnAnimationEnd, endCallBack);
			animation.TryResetToAnimationFirstFrame();
			_animationList.Add(animation);
		}

		public void StartPlay(float startDelay = 0f, bool lockUI = true)
		{
			_lockUI = lockUI;
			if (Mathf.Approximately(startDelay, 0f))
			{
				DoStarPlay();
				return;
			}
			_startDelayTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(startDelay, 0f);
			_startDelayTimer.timeUpCallback = DoStarPlay;
		}

		private void DoStarPlay()
		{
			IsPlaying = true;
			Singleton<MainUIManager>.Instance.LockUI(_lockUI);
			_index = 0;
			_lastIndex = -1;
			PlayNext();
		}

		public void AddAllChildrenInTransform(Transform trans)
		{
			foreach (Transform tran in trans)
			{
				MonoAnimationinSequence component = tran.GetComponent<MonoAnimationinSequence>();
				if (component != null)
				{
					AddAnimation(component);
				}
			}
		}

		private void OnAnimationEnd()
		{
			_lastIndex = _index;
			_index++;
			PlayNext();
		}

		private void PlayNext()
		{
			if (_lastIndex > 0 && _onEveryAnimation != null)
			{
				_onEveryAnimation(_animationList[_lastIndex].transform);
			}
			while (_index < _animationList.Count && (_animationList[_index] == null || !_animationList[_index].gameObject.activeSelf))
			{
				_index++;
			}
			if (_index < _animationList.Count && _animationList[_index] != null && _animationList[_index].gameObject.activeSelf)
			{
				_animationList[_index].Play();
				return;
			}
			Singleton<MainUIManager>.Instance.LockUI(false);
			if (_onAllAnimationEnd != null)
			{
				_onAllAnimationEnd();
			}
			IsPlaying = false;
		}
	}
}
