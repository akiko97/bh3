using System;

namespace MoleMole.Config
{
	public class ConfigAvatarStateMachinePattern : ConfigEntityStateMachinePattern
	{
		public float IdleCD;

		public string SwitchInAnimatorStateName = "SwitchInFast";

		[NonSerialized]
		public int SwitchInAnimatorStateHash;

		public string SwitchOutAnimatorStateName = "SwitchOut";

		[NonSerialized]
		public int SwitchOutAnimatorStateHash;
	}
}
