namespace MoleMole.Config
{
	public class ConfigEntityCommonArguments
	{
		public EntityNature Nature;

		public EntityClass Class;

		public EntityRoleName RoleName;

		public string DefaultAnimEventPredicate = "NormalMode";

		public float CreatePosYOffset;

		public float CreateCollisionRadius;

		public float CreateCollisionHeight;

		public float CollisionLevel;

		public float CollisionRadius = 0.5f;

		public string[] PreloadEffectPatternGroups = Miscs.EMPTY_STRINGS;

		public string[] RequestSoundBankNames = Miscs.EMPTY_STRINGS;

		public string[] EffectPredicates = Miscs.EMPTY_STRINGS;

		public bool HasLowPrefab;

		public float CameraMinAngleRatio = 1f;
	}
}
