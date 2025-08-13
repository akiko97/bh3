using System.Collections.Generic;
using FullInspector;

namespace MoleMole
{
	public abstract class BaseFollowBaseState : State<MainCameraFollowState>
	{
		[InspectorCollapsedFoldout]
		public HashSet<BaseFollowShortState> maskedShortStates;

		public bool cannotBeSkipped { get; protected set; }

		public BaseFollowBaseState(MainCameraFollowState followState)
			: base(followState)
		{
			maskedShortStates = new HashSet<BaseFollowShortState>();
		}

		public abstract void ResetState();
	}
}
