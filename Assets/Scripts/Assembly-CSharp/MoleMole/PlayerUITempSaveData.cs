using UnityEngine;

namespace MoleMole
{
	public class PlayerUITempSaveData
	{
		public int lastSelectedAvatarID;

		public Vector2 avatarOverviewPageScrollerPos;

		public bool hasLandedMainPage;

		public bool hasSendVerifyEmailApply;

		public bool hasShowAvatarTurnAroundAnim;

		public bool hasShowedStartUpDialogs;

		public PlayerUITempSaveData()
		{
			lastSelectedAvatarID = 0;
			hasLandedMainPage = false;
			hasSendVerifyEmailApply = false;
			hasShowAvatarTurnAroundAnim = false;
			hasShowedStartUpDialogs = false;
		}
	}
}
