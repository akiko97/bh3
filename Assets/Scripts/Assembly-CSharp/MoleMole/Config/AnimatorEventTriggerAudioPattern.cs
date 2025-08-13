namespace MoleMole.Config
{
	public class AnimatorEventTriggerAudioPattern : AnimatorEvent
	{
		public string AudioPatternName;

		public bool onlyLocalAvatar;

		public override void HandleAnimatorEvent(BaseMonoAnimatorEntity entity)
		{
			bool flag = true;
			if (onlyLocalAvatar)
			{
				BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
				if (localAvatar.GetRuntimeID() != entity.GetRuntimeID())
				{
					flag = false;
				}
			}
			if (flag)
			{
				entity.TriggerAudioPattern(AudioPatternName);
			}
		}
	}
}
