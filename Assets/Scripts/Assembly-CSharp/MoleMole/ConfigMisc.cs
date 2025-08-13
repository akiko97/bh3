using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public class ConfigMisc
	{
		public ConfigTransformInfo DefaultAvatar3DModel;

		public List<ConfigPageAvatarShowInfo> PageAvatarShowInfo;

		public ConfigPlotAvatarCameraPosInfo PlotAvatarCameraPosInfo;

		public ConfigTextMapKey TextMapKey;

		public ConfigPrefabPath PrefabPath;

		public List<string> ItemRarityColorList;

		public List<string> DropItemBracketColorList;

		public List<string> AvatarAttributeColorList;

		public List<string> AvatarAttributeBGSpriteList;

		public ConfigDynamicArguments Color = new ConfigDynamicArguments();

		public ConfigChat ChatConfig;

		public ConfigBasic BasicConfig;

		public ConfigDynamicArguments FeatureUnlockLevel = new ConfigDynamicArguments();

		public List<int> ComboEvaluation;

		public List<string> ComboNumFrameColor;

		public ConfigDynamicArguments EliteAbilityIcon = new ConfigDynamicArguments();

		public ConfigDynamicArguments EliteAbilityText = new ConfigDynamicArguments();

		public List<int> EquipPowerUpBoostRateResult;

		public List<string> GachaTimeTextID;

		public List<string> AvatarClassSkillIconPath;

		public List<string> ActivityStatusImgPath;

		public List<string> StigmataTypeIconPath;

		public List<string> GachaTypeTitleFigures;

		public List<string> MonthTextIDList;

		public List<string> EndlessGroupUnSelectColor;

		public List<string> EndlessGroupBGColor;

		public List<string> EndlessGroupSelectPrefabPath;

		public List<string> EndlessGroupUnselectPrefabPath;

		public List<string> EndlessGroupUnopenPrefabPath;

		public List<string> IslandVentureConditionText;

		public List<string> IslandAvatarEnhanceClassImage;

		public List<string> WeatherBgPath;

		public List<int> AvatarClassDoNotShow;

		public List<string> RarityColor;

		public List<int> EasternerClassIDList;

		public List<string> ItemRarityBGImgPath;

		public List<string> ItemRarityLightImgPath;

		public List<string> VentureDifficultyDesc;

		public List<string> AvatarStarName;

		public ConfigDynamicArguments CurrencyIconPath = new ConfigDynamicArguments();

		public ConfigDynamicArguments GachaTicketIconPath = new ConfigDynamicArguments();

		public List<string> AvatarStarIcons;

		public ConfigAntiCheat AntiCheat;

		public ConfigAntiEmulator AntiEmulator;

		public bool CollectUIStatistics;

		public string DumpFileUploadUrl;

		public bool EnableHashCheck;
	}
}
