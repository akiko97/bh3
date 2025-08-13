using FullInspector;

namespace MoleMole
{
	[fiInspectorOnly]
	public class BaseFollowShortState : State<MainCameraFollowState>
	{
		public bool isInteruptable { get; protected set; }

		public bool isSkippingBaseState { get; protected set; }

		public BaseFollowShortState(MainCameraFollowState followState)
			: base(followState)
		{
		}

		public virtual void PostUpdate()
		{
		}

		protected void End()
		{
			_owner.RemoveShortState();
		}
	}
}
