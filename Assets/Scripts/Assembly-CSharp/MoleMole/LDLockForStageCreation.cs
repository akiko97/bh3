using System;
using UnityEngine;

namespace MoleMole
{
	public class LDLockForStageCreation : BaseLDEvent
	{
		private const int WAIT_FRAME_CNT = 3;

		private int _frameIx;

		private bool _needUpdate;

		private bool _stageReady;

		private bool _isBeginStage;

		public LDLockForStageCreation(bool locked, bool isBeginStage)
		{
			if (locked)
			{
				Time.timeScale = 0f;
				Time.fixedDeltaTime = 0f;
				Singleton<EventManager>.Instance.SetPauseDispatching(true);
				Singleton<MonsterManager>.Instance.UnOccupyAllPreloadedMonsters();
				Done();
			}
			else
			{
				Singleton<MonsterManager>.Instance.DestroyUnOccupiedPreloadMonsters();
				Singleton<EffectManager>.Instance.ReloadEffectPool();
			}
			_frameIx = 0;
			_needUpdate = !locked;
			_isBeginStage = isBeginStage;
		}

		public override void Core()
		{
			if (!_needUpdate)
			{
				return;
			}
			if (_frameIx == 0)
			{
				if (!_isBeginStage)
				{
					GC.Collect();
					GC.WaitForPendingFinalizers();
				}
			}
			else if (_frameIx == 1)
			{
				if (!_isBeginStage)
				{
					Resources.UnloadUnusedAssets();
				}
				Singleton<EventManager>.Instance.SetPauseDispatching(false);
			}
			else if (_frameIx >= 3 && _stageReady)
			{
				Time.timeScale = 1f;
				Time.fixedDeltaTime = 0.02f * Time.timeScale;
				Done();
			}
			_frameIx++;
		}

		public override void OnEvent(BaseEvent evt)
		{
			if (evt is EvtStageReady)
			{
				_stageReady = true;
			}
		}
	}
}
