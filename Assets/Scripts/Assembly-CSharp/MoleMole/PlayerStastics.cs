namespace MoleMole
{
	public class PlayerStastics
	{
		public SafeInt32 screenRotateTimes = 0;

		public SafeFloat stageTime = 0f;

		public PlayerStastics()
		{
			screenRotateTimes = 0;
			stageTime = 0f;
		}

		public PlayerStastics(float levelTime, int screenRotateTimes)
		{
			stageTime = levelTime;
			this.screenRotateTimes = screenRotateTimes;
		}

		public void ResetPlayerStasticsData()
		{
			screenRotateTimes = 0;
			stageTime = 0f;
		}
	}
}
