using UnityEngine;

namespace MoleMole
{
	public class RewardUIData
	{
		public ResourceType rewardType;

		public int value;

		public int level;

		public int itemID;

		public string iconPath;

		public string imagePath;

		public string valueLabelTextID;

		public string nameTextID;

		public string descTextID;

		public static string ITEM_ICON_PREFAB_PATH = "ItemIconPrefabPath";

		public static string ITEM_ICON_TEXT_ID = "ItemIconTextID";

		public RewardUIData(ResourceType type, int value, string textID, string descTextID, int itemID = 0, int level = 0)
		{
			rewardType = type;
			this.value = value;
			iconPath = UIUtil.GetResourceIconPath(type, itemID);
			valueLabelTextID = textID;
			this.descTextID = descTextID;
			this.itemID = itemID;
			this.level = level;
		}

		public static RewardUIData GetPlayerExpData(int value = 0)
		{
			RewardUIData rewardUIData = new RewardUIData(ResourceType.PlayerExp, value, "RewardName_Exp", "MaterialDetail_Exp");
			rewardUIData.nameTextID = "Menu_Exp";
			rewardUIData.imagePath = "SpriteOutput/MaterialFigures/IconExp";
			return rewardUIData;
		}

		public static RewardUIData GetFriendPointData(int value = 0)
		{
			RewardUIData rewardUIData = new RewardUIData(ResourceType.FriendPoint, value, "RewardName_FriendPoint", "MaterialDetail_Fpoint");
			rewardUIData.nameTextID = "Menu_FriendPoint";
			rewardUIData.imagePath = "SpriteOutput/MaterialFigures/IconFP";
			return rewardUIData;
		}

		public static RewardUIData GetHcoinData(int value = 0)
		{
			RewardUIData rewardUIData = new RewardUIData(ResourceType.Hcoin, value, "RewardName_Hcoin", "MaterialDetail_Hcoin");
			rewardUIData.nameTextID = "Menu_Hcoin";
			rewardUIData.imagePath = "SpriteOutput/MaterialFigures/IconHC";
			return rewardUIData;
		}

		public static RewardUIData GetScoinData(int value = 0)
		{
			RewardUIData rewardUIData = new RewardUIData(ResourceType.Scoin, value, "RewardName_Scoin", "MaterialDetail_Scoin");
			rewardUIData.nameTextID = "Menu_Scoin";
			rewardUIData.imagePath = "SpriteOutput/MaterialFigures/IconSC";
			return rewardUIData;
		}

		public static RewardUIData GetSkillPointData(int value = 0)
		{
			RewardUIData rewardUIData = new RewardUIData(ResourceType.SkillPoint, value, "RewardName_SkillPoint", string.Empty);
			rewardUIData.nameTextID = "Menu_SkillPtNum";
			rewardUIData.imagePath = "SpriteOutput/MaterialFigures/IconST";
			return rewardUIData;
		}

		public static RewardUIData GetStaminaData(int value = 0)
		{
			RewardUIData rewardUIData = new RewardUIData(ResourceType.Stamina, value, "RewardName_Stamina", "MaterialDetail_Stamina");
			rewardUIData.nameTextID = "Menu_Stamina";
			rewardUIData.imagePath = "SpriteOutput/MaterialFigures/IconST";
			return rewardUIData;
		}

		public Sprite GetIconSprite()
		{
			if (!string.IsNullOrEmpty(iconPath))
			{
				return Miscs.GetSpriteByPrefab(iconPath);
			}
			return null;
		}

		public Sprite GetImageSprite()
		{
			if (!string.IsNullOrEmpty(imagePath))
			{
				return Miscs.GetSpriteByPrefab(imagePath);
			}
			return null;
		}
	}
}
