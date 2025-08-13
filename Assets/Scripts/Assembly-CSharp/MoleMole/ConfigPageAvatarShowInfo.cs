using System.Collections.Generic;

namespace MoleMole
{
	public class ConfigPageAvatarShowInfo
	{
		public Dictionary<string, ConfigTabAvatarTransformInfo> AvatarTabTransformInfos = new Dictionary<string, ConfigTabAvatarTransformInfo>();

		public bool ShowLockViewIfLock;
	}
}
