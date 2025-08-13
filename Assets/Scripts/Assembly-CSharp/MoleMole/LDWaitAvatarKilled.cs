using System.Collections.Generic;

namespace MoleMole
{
	public class LDWaitAvatarKilled : BaseLDEvent
	{
		private List<uint> _playAvatarIDs;

		private int _diedAvatarNum;

		private int _targetNum;

		public LDWaitAvatarKilled(double diedAvatarNum)
		{
			_playAvatarIDs = new List<uint>();
			foreach (BaseMonoAvatar allPlayerAvatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
			{
				_playAvatarIDs.Add(allPlayerAvatar.GetRuntimeID());
				_targetNum = (int)diedAvatarNum;
				_diedAvatarNum = 0;
			}
		}

		public override void OnEvent(BaseEvent evt)
		{
			if (evt is EvtKilled && _playAvatarIDs.Contains(evt.targetID))
			{
				_diedAvatarNum++;
				if (_diedAvatarNum >= _targetNum)
				{
					Done();
				}
			}
		}
	}
}
