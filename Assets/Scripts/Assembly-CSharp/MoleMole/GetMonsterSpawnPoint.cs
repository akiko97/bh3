using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	public class GetMonsterSpawnPoint : Action
	{
		public SharedString SpawnPoint;

		public override TaskStatus OnUpdate()
		{
			if (Singleton<MainUIManager>.Instance != null && Singleton<MainUIManager>.Instance.GetInLevelUICanvas() != null && Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager != null)
			{
				MonoSpawnPoint spawnPoint = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.GetSpawnPoint();
				SpawnPoint.SetValue((!(spawnPoint != null)) ? null : spawnPoint.name);
			}
			return TaskStatus.Success;
		}
	}
}
