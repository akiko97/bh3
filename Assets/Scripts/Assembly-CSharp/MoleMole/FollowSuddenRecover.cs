namespace MoleMole
{
	public class FollowSuddenRecover : BaseFollowShortState
	{
		public FollowSuddenRecover(MainCameraFollowState followState)
			: base(followState)
		{
			base.isSkippingBaseState = true;
		}

		public override void PostUpdate()
		{
			if (_owner.recoverState.active)
			{
				_owner.recoverState.CancelAndJumpToOriginalState();
			}
			_owner.needLerpForwardThisFrame = false;
			_owner.needLerpPositionThisFrame = false;
			End();
		}
	}
}
