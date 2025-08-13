using UnityEngine;

namespace MoleMole
{
	public class Avatar3dModelDataItem
	{
		public readonly AvatarDataItem avatar;

		public readonly Vector3 pos;

		public readonly Vector3 eulerAngles;

		public readonly bool showLockViewIfLock;

		public Avatar3dModelDataItem(AvatarDataItem avatar, Vector3 pos, Vector3 eulerAngles, bool showLockViewIfLock = false)
		{
			this.avatar = avatar;
			this.pos = pos;
			this.eulerAngles = eulerAngles;
			this.showLockViewIfLock = showLockViewIfLock;
		}
	}
}
