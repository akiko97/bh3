using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class StigmataMetaData : IHashable
	{
		public readonly int ID;

		public readonly string name;

		public readonly int rarity;

		public readonly int maxRarity;

		public readonly int subRarity;

		public readonly int subMaxRarity;

		public readonly int cost;

		public readonly int powerType;

		public readonly int maxLv;

		public readonly int expType;

		public readonly float sellPriceBase;

		public readonly float sellPriceAdd;

		public readonly float gearExpProvideBase;

		public readonly float gearExpPorvideAdd;

		public readonly string equipmentType;

		public readonly int baseType;

		public readonly string displayTitle;

		public readonly string displayDescription;

		public readonly int displayNumber;

		public readonly string iconPath;

		public readonly string imagePath;

		public readonly float HPBase;

		public readonly float HPAdd;

		public readonly float SPBase;

		public readonly float SPAdd;

		public readonly float attackBase;

		public readonly float attackAdd;

		public readonly float defenceBase;

		public readonly float defenceAdd;

		public readonly float criticalBase;

		public readonly float criticalAdd;

		public readonly float durabilityMax;

		public readonly float durabilityDecrease;

		public readonly float repairPriceBase;

		public readonly float repairPriceAdd;

		public readonly List<string> evoMaterial;

		public readonly int evoID;

		public readonly int prop1ID;

		public readonly float prop1Param1;

		public readonly float prop1Param2;

		public readonly float prop1Param3;

		public readonly float prop1Param1Add;

		public readonly float prop1Param2Add;

		public readonly float prop1Param3Add;

		public readonly int prop2ID;

		public readonly float prop2Param1;

		public readonly float prop2Param2;

		public readonly float prop2Param3;

		public readonly float prop2Param1Add;

		public readonly float prop2Param2Add;

		public readonly float prop2Param3Add;

		public readonly int prop3ID;

		public readonly float prop3Param1;

		public readonly float prop3Param2;

		public readonly float prop3Param3;

		public readonly float prop3Param1Add;

		public readonly float prop3Param2Add;

		public readonly float prop3Param3Add;

		public readonly int setID;

		public readonly string smallIcon;

		public readonly string tattooPath;

		public readonly float offsetX;

		public readonly float offsetY;

		public readonly float scale;

		public readonly int affixDrop;

		public readonly int affixGacha;

		public readonly int affixRefine;

		public readonly int affixDefault;

		public readonly int canRefine;

		public StigmataMetaData(int ID, string name, int rarity, int maxRarity, int subRarity, int subMaxRarity, int cost, int powerType, int maxLv, int expType, float sellPriceBase, float sellPriceAdd, float gearExpProvideBase, float gearExpPorvideAdd, string equipmentType, int baseType, string displayTitle, string displayDescription, int displayNumber, string iconPath, string imagePath, float HPBase, float HPAdd, float SPBase, float SPAdd, float attackBase, float attackAdd, float defenceBase, float defenceAdd, float criticalBase, float criticalAdd, float durabilityMax, float durabilityDecrease, float repairPriceBase, float repairPriceAdd, List<string> evoMaterial, int evoID, int prop1ID, float prop1Param1, float prop1Param2, float prop1Param3, float prop1Param1Add, float prop1Param2Add, float prop1Param3Add, int prop2ID, float prop2Param1, float prop2Param2, float prop2Param3, float prop2Param1Add, float prop2Param2Add, float prop2Param3Add, int prop3ID, float prop3Param1, float prop3Param2, float prop3Param3, float prop3Param1Add, float prop3Param2Add, float prop3Param3Add, int setID, string smallIcon, string tattooPath, float offsetX, float offsetY, float scale, int affixDrop, int affixGacha, int affixRefine, int affixDefault, int canRefine)
		{
			this.ID = ID;
			this.name = name;
			this.rarity = rarity;
			this.maxRarity = maxRarity;
			this.subRarity = subRarity;
			this.subMaxRarity = subMaxRarity;
			this.cost = cost;
			this.powerType = powerType;
			this.maxLv = maxLv;
			this.expType = expType;
			this.sellPriceBase = sellPriceBase;
			this.sellPriceAdd = sellPriceAdd;
			this.gearExpProvideBase = gearExpProvideBase;
			this.gearExpPorvideAdd = gearExpPorvideAdd;
			this.equipmentType = equipmentType;
			this.baseType = baseType;
			this.displayTitle = displayTitle;
			this.displayDescription = displayDescription;
			this.displayNumber = displayNumber;
			this.iconPath = iconPath;
			this.imagePath = imagePath;
			this.HPBase = HPBase;
			this.HPAdd = HPAdd;
			this.SPBase = SPBase;
			this.SPAdd = SPAdd;
			this.attackBase = attackBase;
			this.attackAdd = attackAdd;
			this.defenceBase = defenceBase;
			this.defenceAdd = defenceAdd;
			this.criticalBase = criticalBase;
			this.criticalAdd = criticalAdd;
			this.durabilityMax = durabilityMax;
			this.durabilityDecrease = durabilityDecrease;
			this.repairPriceBase = repairPriceBase;
			this.repairPriceAdd = repairPriceAdd;
			this.evoMaterial = evoMaterial;
			this.evoID = evoID;
			this.prop1ID = prop1ID;
			this.prop1Param1 = prop1Param1;
			this.prop1Param2 = prop1Param2;
			this.prop1Param3 = prop1Param3;
			this.prop1Param1Add = prop1Param1Add;
			this.prop1Param2Add = prop1Param2Add;
			this.prop1Param3Add = prop1Param3Add;
			this.prop2ID = prop2ID;
			this.prop2Param1 = prop2Param1;
			this.prop2Param2 = prop2Param2;
			this.prop2Param3 = prop2Param3;
			this.prop2Param1Add = prop2Param1Add;
			this.prop2Param2Add = prop2Param2Add;
			this.prop2Param3Add = prop2Param3Add;
			this.prop3ID = prop3ID;
			this.prop3Param1 = prop3Param1;
			this.prop3Param2 = prop3Param2;
			this.prop3Param3 = prop3Param3;
			this.prop3Param1Add = prop3Param1Add;
			this.prop3Param2Add = prop3Param2Add;
			this.prop3Param3Add = prop3Param3Add;
			this.setID = setID;
			this.smallIcon = smallIcon;
			this.tattooPath = tattooPath;
			this.offsetX = offsetX;
			this.offsetY = offsetY;
			this.scale = scale;
			this.affixDrop = affixDrop;
			this.affixGacha = affixGacha;
			this.affixRefine = affixRefine;
			this.affixDefault = affixDefault;
			this.canRefine = canRefine;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(ID, ref lastHash);
			HashUtils.ContentHashOnto(name, ref lastHash);
			HashUtils.ContentHashOnto(rarity, ref lastHash);
			HashUtils.ContentHashOnto(maxRarity, ref lastHash);
			HashUtils.ContentHashOnto(subRarity, ref lastHash);
			HashUtils.ContentHashOnto(subMaxRarity, ref lastHash);
			HashUtils.ContentHashOnto(cost, ref lastHash);
			HashUtils.ContentHashOnto(powerType, ref lastHash);
			HashUtils.ContentHashOnto(maxLv, ref lastHash);
			HashUtils.ContentHashOnto(expType, ref lastHash);
			HashUtils.ContentHashOnto(sellPriceBase, ref lastHash);
			HashUtils.ContentHashOnto(sellPriceAdd, ref lastHash);
			HashUtils.ContentHashOnto(gearExpProvideBase, ref lastHash);
			HashUtils.ContentHashOnto(gearExpPorvideAdd, ref lastHash);
			HashUtils.ContentHashOnto(equipmentType, ref lastHash);
			HashUtils.ContentHashOnto(baseType, ref lastHash);
			HashUtils.ContentHashOnto(displayTitle, ref lastHash);
			HashUtils.ContentHashOnto(displayDescription, ref lastHash);
			HashUtils.ContentHashOnto(displayNumber, ref lastHash);
			HashUtils.ContentHashOnto(iconPath, ref lastHash);
			HashUtils.ContentHashOnto(imagePath, ref lastHash);
			HashUtils.ContentHashOnto(HPBase, ref lastHash);
			HashUtils.ContentHashOnto(HPAdd, ref lastHash);
			HashUtils.ContentHashOnto(SPBase, ref lastHash);
			HashUtils.ContentHashOnto(SPAdd, ref lastHash);
			HashUtils.ContentHashOnto(attackBase, ref lastHash);
			HashUtils.ContentHashOnto(attackAdd, ref lastHash);
			HashUtils.ContentHashOnto(defenceBase, ref lastHash);
			HashUtils.ContentHashOnto(defenceAdd, ref lastHash);
			HashUtils.ContentHashOnto(criticalBase, ref lastHash);
			HashUtils.ContentHashOnto(criticalAdd, ref lastHash);
			HashUtils.ContentHashOnto(durabilityMax, ref lastHash);
			HashUtils.ContentHashOnto(durabilityDecrease, ref lastHash);
			HashUtils.ContentHashOnto(repairPriceBase, ref lastHash);
			HashUtils.ContentHashOnto(repairPriceAdd, ref lastHash);
			if (evoMaterial != null)
			{
				foreach (string item in evoMaterial)
				{
					HashUtils.ContentHashOnto(item, ref lastHash);
				}
			}
			HashUtils.ContentHashOnto(evoID, ref lastHash);
			HashUtils.ContentHashOnto(prop1ID, ref lastHash);
			HashUtils.ContentHashOnto(prop1Param1, ref lastHash);
			HashUtils.ContentHashOnto(prop1Param2, ref lastHash);
			HashUtils.ContentHashOnto(prop1Param3, ref lastHash);
			HashUtils.ContentHashOnto(prop1Param1Add, ref lastHash);
			HashUtils.ContentHashOnto(prop1Param2Add, ref lastHash);
			HashUtils.ContentHashOnto(prop1Param3Add, ref lastHash);
			HashUtils.ContentHashOnto(prop2ID, ref lastHash);
			HashUtils.ContentHashOnto(prop2Param1, ref lastHash);
			HashUtils.ContentHashOnto(prop2Param2, ref lastHash);
			HashUtils.ContentHashOnto(prop2Param3, ref lastHash);
			HashUtils.ContentHashOnto(prop2Param1Add, ref lastHash);
			HashUtils.ContentHashOnto(prop2Param2Add, ref lastHash);
			HashUtils.ContentHashOnto(prop2Param3Add, ref lastHash);
			HashUtils.ContentHashOnto(prop3ID, ref lastHash);
			HashUtils.ContentHashOnto(prop3Param1, ref lastHash);
			HashUtils.ContentHashOnto(prop3Param2, ref lastHash);
			HashUtils.ContentHashOnto(prop3Param3, ref lastHash);
			HashUtils.ContentHashOnto(prop3Param1Add, ref lastHash);
			HashUtils.ContentHashOnto(prop3Param2Add, ref lastHash);
			HashUtils.ContentHashOnto(prop3Param3Add, ref lastHash);
			HashUtils.ContentHashOnto(setID, ref lastHash);
			HashUtils.ContentHashOnto(smallIcon, ref lastHash);
			HashUtils.ContentHashOnto(tattooPath, ref lastHash);
			HashUtils.ContentHashOnto(offsetX, ref lastHash);
			HashUtils.ContentHashOnto(offsetY, ref lastHash);
			HashUtils.ContentHashOnto(scale, ref lastHash);
			HashUtils.ContentHashOnto(affixDrop, ref lastHash);
			HashUtils.ContentHashOnto(affixGacha, ref lastHash);
			HashUtils.ContentHashOnto(affixRefine, ref lastHash);
			HashUtils.ContentHashOnto(affixDefault, ref lastHash);
			HashUtils.ContentHashOnto(canRefine, ref lastHash);
		}
	}
}
