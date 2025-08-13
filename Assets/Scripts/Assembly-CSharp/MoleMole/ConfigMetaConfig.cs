using System;
using FullInspector;
using MoleMole.Config;

namespace MoleMole
{
	public class ConfigMetaConfig : BaseScriptableObject
	{
		public string[] effectPatternPathes;

		public string[] stageEntryPathes;

		public string[] auxEntryPathes;

		public string[] dynamicObjectRegistryPathes;

		public string[] renderEntryPathes;

		public string[] weatherEntryPathes;

		public string[] animatorEventPatternPathes;

		public string[] touchPatternPathes;

		[NonSerialized]
		public string atmosphereRegistryPath = "Rendering/MainMenuAtmosphereConfig/Data/AtmosphereRegistry";

		[NonSerialized]
		public string allLevelsClearedAtmosphereRegistryPath = "Rendering/MainMenuAtmosphereConfig/Data/AllLevelsClearedAtmosphereRegistry";

		public string[] abilityRegistryPathes;

		public string[] weaponRegistryPathes;

		public string[] equipmentSkillRegistryPathes;

		public string[] galTouchBuffRegistryPathes;

		public string[] propObjectRegistryPathes;

		public ConfigNavMeshScenePath[] scenePaths;

		public string[] graphicsSettingRegistryPathes;

		public string[] cameraCurvePatternPathes;

		public string[] sharedAnimEventGroupPathes;

		public string[] groupAIGridPathes;

		[NonSerialized]
		public string galTouchDataPath = "Data/GalTouch/GalTouchData";

		[NonSerialized]
		public string graphicsVolatileSettingRegistryPath = "Data/_BothLocalAndAssetBundle/GraphicsSettingConfig/VolatileSetting";

		[NonSerialized]
		public string inLevelMiscData = "Data/InLevelMiscData";
	}
}
