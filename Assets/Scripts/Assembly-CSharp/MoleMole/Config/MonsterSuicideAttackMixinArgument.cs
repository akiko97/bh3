namespace MoleMole.Config
{
	public class MonsterSuicideAttackMixinArgument : IMixinArgument
	{
		public float SuicideCountDown;

		public bool SuicideOnTouch;

		public string OnTouchTriggerID;

		public float BeapInterval = 1f;
	}
}
