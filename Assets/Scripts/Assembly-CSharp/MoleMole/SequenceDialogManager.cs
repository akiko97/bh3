using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class SequenceDialogManager
	{
		private List<BaseSequenceDialogContext> _dialogList;

		private int _index;

		private Action _onAllDialogDestroy;

		private bool _bIsPlaying;

		private CanvasTimer _startDelayTimer;

		public SequenceDialogManager(Action onAllDialogDestroyCallBack = null)
		{
			_dialogList = new List<BaseSequenceDialogContext>();
			_index = 0;
			_onAllDialogDestroy = onAllDialogDestroyCallBack;
			_bIsPlaying = false;
		}

		public void ClearDialogs()
		{
			_dialogList.Clear();
		}

		public void AddDialog(BaseSequenceDialogContext dialogContext)
		{
			dialogContext.SetDestroyCallBack(OnDialogDestroy);
			_dialogList.Add(dialogContext);
		}

		public void AddAsFirstDialog(BaseSequenceDialogContext dialogContext)
		{
			dialogContext.SetDestroyCallBack(OnDialogDestroy);
			_dialogList.Insert(0, dialogContext);
		}

		public void StartShow(float startDelay = 0f)
		{
			if (Mathf.Approximately(startDelay, 0f))
			{
				DoStartShow();
			}
			else if (Singleton<MainUIManager>.Instance.SceneCanvas != null)
			{
				_startDelayTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(startDelay, 0f);
				_startDelayTimer.timeUpCallback = DoStartShow;
			}
		}

		private void DoStartShow()
		{
			_index = 0;
			_bIsPlaying = true;
			ShowNext();
		}

		public bool IsPlaying()
		{
			return _bIsPlaying;
		}

		public int GetDialogNum()
		{
			return _dialogList.Count;
		}

		public BaseSequenceDialogContext GetDialog(int index)
		{
			if (index >= 0 && index < _dialogList.Count)
			{
				return _dialogList[index];
			}
			return null;
		}

		private void OnDialogDestroy()
		{
			_index++;
			ShowNext();
		}

		private void ShowNext()
		{
			if (_index < _dialogList.Count)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(_dialogList[_index]);
				return;
			}
			_bIsPlaying = false;
			if (_onAllDialogDestroy != null)
			{
				_onAllDialogDestroy();
			}
		}
	}
}
