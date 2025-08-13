namespace MoleMole
{
	public class MonoMPLevelV1 : MonoTheLevelV1
	{
		protected override void CreateLevelManager()
		{
			Singleton<MPLevelManager>.Create();
			Singleton<LevelManager>.CreateByInstance(Singleton<MPLevelManager>.Instance);
			MonoLevelEntity monoLevelEntity = (Singleton<LevelManager>.Instance.levelEntity = base.gameObject.AddComponent<MonoLevelEntity>());
			monoLevelEntity.Init(562036737u);
			Singleton<LevelManager>.Instance.levelActor = Singleton<EventManager>.Instance.CreateActor<MPLevelActor>(monoLevelEntity);
			Singleton<LevelManager>.Instance.levelActor.PostInit();
		}
	}
}
