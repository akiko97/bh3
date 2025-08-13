using FullInspector;

namespace MoleMole.Config
{
	public class StageEntry
	{
		private const string STAGE_PREFAB_PATH = "Stage/";

		public readonly string TypeName;

		public readonly string PerpStagePrefabPath;

		public readonly string EnvPrefabPath;

		public readonly string LocationPointName;

		public string HideNodeNames;

		public string ShowNodeNames;

		public string HideNodePrefabPaths;

		public string ShowNodePrefabPaths;

		[InspectorNullable]
		public ConfigStageEffectSetting StageEffectSetting;

		public string GetPerpStagePrefabPath()
		{
			return "Stage/" + PerpStagePrefabPath;
		}

		public string GetEnvPrefabPath()
		{
			return "Stage/" + EnvPrefabPath;
		}
	}
}
