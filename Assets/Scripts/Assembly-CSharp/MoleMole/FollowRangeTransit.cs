using UnityEngine;

namespace MoleMole
{
	public class FollowRangeTransit : BaseFollowShortState
	{
		private const float RANGE_TRANSIT_LINEAR_LERP_TIME = 0.7f;

		private MainCameraFollowState.RangeState _rangeState;

		private float _origAnchorRadius;

		private float _targetAnchorRadius;

		private float _origAnchorElevation;

		private float _targetAnchorElevation;

		private float _timer;

		public FollowRangeTransit(MainCameraFollowState followState)
			: base(followState)
		{
			base.isSkippingBaseState = false;
		}

		public void SetRange(MainCameraFollowState.RangeState rangeState)
		{
			_rangeState = rangeState;
		}

		public override void Enter()
		{
			_timer = 0f;
			_origAnchorRadius = _owner.anchorRadius;
			_origAnchorElevation = _owner.anchorElevation;
			if (_rangeState == MainCameraFollowState.RangeState.Near)
			{
				_targetAnchorRadius = 6f;
				_targetAnchorElevation = 3.5f;
			}
			else if (_rangeState == MainCameraFollowState.RangeState.Far)
			{
				_targetAnchorRadius = 7f;
				_targetAnchorElevation = 7f;
			}
			else if (_rangeState == MainCameraFollowState.RangeState.Furter)
			{
				_targetAnchorRadius = 8.5f;
				_targetAnchorElevation = 7f;
			}
			else if (_rangeState == MainCameraFollowState.RangeState.High)
			{
				_targetAnchorElevation = 10f;
			}
			else if (_rangeState == MainCameraFollowState.RangeState.Higher)
			{
				_targetAnchorElevation = 15f;
			}
			_owner.recoverState.SetupRecoverRadius(_targetAnchorRadius);
			_owner.recoverState.SetupRecoverElevation(_targetAnchorElevation);
		}

		public override void Update()
		{
			_timer += Time.deltaTime * _owner.mainCamera.TimeScale;
			if (_timer < 0.7f)
			{
				_owner.anchorRadius = Mathf.Lerp(_origAnchorRadius, _targetAnchorRadius, _timer / 0.7f);
				_owner.anchorElevation = Mathf.Lerp(_origAnchorElevation, _targetAnchorElevation, _timer / 0.7f);
			}
			else
			{
				End();
			}
		}

		public override void Exit()
		{
			_owner.recoverState.TryRecover();
		}
	}
}
