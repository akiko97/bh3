using System;

namespace MoleMole
{
	public class LDWaitComboBreak : BaseLDEvent
	{
		public LDWaitComboBreak(double runtimeID)
		{
			LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
			levelActor.onLevelComboChanged = (Action<int, int>)Delegate.Combine(levelActor.onLevelComboChanged, new Action<int, int>(UpdateAttackSpeedByCombo));
		}

		private void UpdateAttackSpeedByCombo(int from, int to)
		{
			if (to < from)
			{
				LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
				levelActor.onLevelComboChanged = (Action<int, int>)Delegate.Remove(levelActor.onLevelComboChanged, new Action<int, int>(UpdateAttackSpeedByCombo));
				Done();
			}
		}
	}
}
