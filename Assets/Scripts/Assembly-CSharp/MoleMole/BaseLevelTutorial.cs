using System;
using System.Collections;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

namespace MoleMole
{
	[fiInspectorOnly]
	public abstract class BaseLevelTutorial
	{
		public enum StepState
		{
			Sleep = 0,
			Active = 1,
			Done = 2
		}

		public readonly int tutorialId;

		protected LevelTutorialHelperPlugin _helper;

		protected LevelTutorialMetaData _metaData;

		protected List<string> _displayList;

		protected int _maxStepCount;

		protected List<StepState> _stepStateList;

		public bool active;

		[ShowInInspector]
		public int step { get; set; }

		public BaseLevelTutorial(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData)
		{
			_helper = helper;
			_metaData = metaData;
			tutorialId = _metaData.tutorialId;
			_stepStateList = new List<StepState>();
			_displayList = new List<string>();
			step = 0;
			_maxStepCount = _metaData.paramList.Count;
			for (int i = 0; i < _maxStepCount; i++)
			{
				_stepStateList.Add(StepState.Sleep);
				_displayList.Add(_metaData.diaplayTarget[i]);
			}
			active = true;
		}

		public virtual void MoveToNextStep()
		{
			if (step < _maxStepCount)
			{
				_stepStateList[step] = StepState.Done;
				step++;
			}
		}

		public virtual bool IsInStep(int stepIndex)
		{
			return step == stepIndex;
		}

		public virtual bool IsAllStepDone()
		{
			bool result = true;
			foreach (StepState stepState in _stepStateList)
			{
				if (stepState != StepState.Done)
				{
					result = false;
					break;
				}
			}
			return result;
		}

		public virtual void ResetToStep(int stepIndex)
		{
			if (stepIndex < _maxStepCount && stepIndex >= 0)
			{
				step = stepIndex;
				for (int i = 0; i < _maxStepCount; i++)
				{
					_stepStateList[i] = ((i < stepIndex) ? StepState.Done : StepState.Sleep);
				}
			}
		}

		public virtual StepState GetStepState(int stepIndex)
		{
			if (stepIndex < _maxStepCount)
			{
				return _stepStateList[stepIndex];
			}
			return StepState.Sleep;
		}

		public StepState GetCurrentStepState()
		{
			if (step < _maxStepCount)
			{
				return _stepStateList[step];
			}
			return StepState.Sleep;
		}

		public virtual void ActiveCurrentStep()
		{
			if (step < _maxStepCount && _stepStateList[step] == StepState.Sleep)
			{
				_stepStateList[step] = StepState.Active;
			}
		}

		public virtual void DoneCurrentStep()
		{
			if (step < _maxStepCount)
			{
				_stepStateList[step] = StepState.Done;
			}
		}

		public virtual string GetDisplayTarget(int stepIndex)
		{
			if (stepIndex >= 0 && stepIndex < _maxStepCount)
			{
				return LocalizationGeneralLogic.GetText(_displayList[stepIndex]);
			}
			return string.Empty;
		}

		public virtual float GetDelayTime(int stepIndex)
		{
			if (stepIndex >= 0 && stepIndex < _maxStepCount)
			{
				return _metaData.paramList[stepIndex];
			}
			return 0f;
		}

		public abstract bool IsFinished();

		public virtual void Core()
		{
		}

		public virtual void OnDecided()
		{
			active = false;
		}

		public virtual void PauseGame()
		{
			Singleton<LevelManager>.Instance.SetTutorialTimeScale(0f);
		}

		public virtual void ResumeGame()
		{
			Singleton<LevelManager>.Instance.SetTutorialTimeScale(1f);
		}

		public virtual void SetControllerEnable(bool enable)
		{
			Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(!enable);
		}

		public virtual void NotifyStep(NotifyTypes notifyType)
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(notifyType, this));
		}

		public virtual void WaitShowTutorialStep(float delay, Action callback)
		{
			if (delay == 0f)
			{
				callback();
			}
			else
			{
				Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(WaitShowStep(delay, callback));
			}
		}

		private IEnumerator WaitShowStep(float delay, Action callback)
		{
			yield return new WaitForSeconds(delay);
			callback();
		}

		public virtual void OnAdded()
		{
		}

		public virtual bool OnEvent(BaseEvent evt)
		{
			return false;
		}

		public virtual bool OnPostEvent(BaseEvent evt)
		{
			return false;
		}

		public virtual bool ListenEvent(BaseEvent evt)
		{
			return false;
		}
	}
}
