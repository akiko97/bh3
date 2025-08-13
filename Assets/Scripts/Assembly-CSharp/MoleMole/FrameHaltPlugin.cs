using UnityEngine;

namespace MoleMole
{
	public sealed class FrameHaltPlugin : BaseEntityFuncPlugin
	{
		public const int EXECUTION_ORDER = 95;

		private const float FRAME_HALT_SPEED = 0f;

		private float _haltTime;

		private bool _halting;

		private IFrameHaltable _frameHaltEntity;

		public FrameHaltPlugin(BaseMonoEntity entity)
			: base(entity)
		{
			_frameHaltEntity = (IFrameHaltable)entity;
		}

		public void FrameHalt(int frameNum)
		{
			float num = (float)frameNum * (1f / 60f);
			if (_halting)
			{
				_haltTime = Mathf.Max(_haltTime, num);
				return;
			}
			_haltTime = num;
			_halting = true;
			_frameHaltEntity.timeScaleStack.Push(5, 0f);
		}

		public override void FixedCore()
		{
		}

		public override void Core()
		{
			if (_halting)
			{
				_haltTime -= Time.unscaledDeltaTime * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
				if (_haltTime < 0f)
				{
					_halting = false;
					_frameHaltEntity.timeScaleStack.Pop(5);
				}
			}
		}

		public override bool IsActive()
		{
			return _haltTime > 0f;
		}
	}
}
