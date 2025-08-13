using MoleMole.MPProtocol;

namespace MoleMole
{
	public class MPLevelManager : LevelManager
	{
		public LevelIdentity levelIdentity;

		public MPMode mpMode;

		public MPLevelManager()
		{
			Singleton<MPManager>.Create();
		}

		protected override void CreateInLevelManagers()
		{
			Singleton<RuntimeIDManager>.Create();
			Singleton<StageManager>.Create();
			Singleton<AvatarManager>.Create();
			Singleton<CameraManager>.Create();
			Singleton<MonsterManager>.Create();
			Singleton<PropObjectManager>.Create();
			Singleton<DynamicObjectManager>.Create();
			Singleton<MPEventManager>.Create();
			Singleton<EventManager>.CreateByInstance(Singleton<MPEventManager>.Instance);
			Singleton<LevelDesignManager>.Create();
			Singleton<AuxObjectManager>.Create();
			Singleton<DetourManager>.Create();
			Singleton<ShaderDataManager>.Create();
			Singleton<CinemaDataManager>.Create();
			gameMode = new NetworkedMP_Default_GameMode();
		}

		public override void InitAtAwake()
		{
			Singleton<MPManager>.Instance.InitAtAwake();
			base.InitAtAwake();
		}

		public override void InitAtStart()
		{
			Singleton<MPManager>.Instance.InitAtStart();
			base.InitAtStart();
		}

		public override void Core()
		{
			Singleton<MPManager>.Instance.Core();
			base.Core();
			Singleton<MPManager>.Instance.PostCore();
		}

		public override void Destroy()
		{
			Singleton<MPManager>.Instance.Destroy();
			Singleton<MPManager>.Destroy();
			base.Destroy();
		}
	}
}
