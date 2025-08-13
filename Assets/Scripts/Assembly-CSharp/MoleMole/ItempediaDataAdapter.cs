namespace MoleMole
{
	public class ItempediaDataAdapter
	{
		private object _itemData;

		private StorageDataItemBase _dummyStorageItemData;

		public int ID
		{
			get
			{
				if (_itemData is WeaponMetaData)
				{
					return ((WeaponMetaData)_itemData).ID;
				}
				if (_itemData is StigmataMetaData)
				{
					return ((StigmataMetaData)_itemData).ID;
				}
				if (_itemData is ItemMetaData)
				{
					return ((ItemMetaData)_itemData).ID;
				}
				return -1;
			}
		}

		public string name
		{
			get
			{
				if (_itemData is WeaponMetaData)
				{
					return ((WeaponMetaData)_itemData).name;
				}
				if (_itemData is StigmataMetaData)
				{
					return ((StigmataMetaData)_itemData).name;
				}
				if (_itemData is ItemMetaData)
				{
					return ((ItemMetaData)_itemData).name;
				}
				return null;
			}
		}

		public string iconPath
		{
			get
			{
				if (_itemData is WeaponMetaData)
				{
					return ((WeaponMetaData)_itemData).iconPath;
				}
				if (_itemData is StigmataMetaData)
				{
					return ((StigmataMetaData)_itemData).iconPath;
				}
				if (_itemData is ItemMetaData)
				{
					return ((ItemMetaData)_itemData).iconPath;
				}
				return null;
			}
		}

		public int rarity
		{
			get
			{
				if (_itemData is WeaponMetaData)
				{
					return ((WeaponMetaData)_itemData).rarity;
				}
				if (_itemData is StigmataMetaData)
				{
					return ((StigmataMetaData)_itemData).rarity;
				}
				if (_itemData is ItemMetaData)
				{
					return ((ItemMetaData)_itemData).rarity;
				}
				return -1;
			}
		}

		public int maxRarity
		{
			get
			{
				if (_itemData is WeaponMetaData)
				{
					return ((WeaponMetaData)_itemData).maxRarity;
				}
				if (_itemData is StigmataMetaData)
				{
					return ((StigmataMetaData)_itemData).maxRarity;
				}
				if (_itemData is ItemMetaData)
				{
					return ((ItemMetaData)_itemData).rarity;
				}
				return -1;
			}
		}

		public int level
		{
			get
			{
				if (_itemData is WeaponMetaData)
				{
					return ((WeaponMetaData)_itemData).maxLv;
				}
				if (_itemData is StigmataMetaData)
				{
					return ((StigmataMetaData)_itemData).maxLv;
				}
				if (_itemData is ItemMetaData)
				{
					return ((ItemMetaData)_itemData).maxLv;
				}
				return -1;
			}
		}

		public int baseType
		{
			get
			{
				if (_itemData is WeaponMetaData)
				{
					return ((WeaponMetaData)_itemData).baseType;
				}
				if (_itemData is StigmataMetaData)
				{
					return ((StigmataMetaData)_itemData).baseType;
				}
				if (_itemData is ItemMetaData)
				{
					return ((ItemMetaData)_itemData).BaseType;
				}
				return -1;
			}
		}

		public int cost
		{
			get
			{
				if (_itemData is WeaponMetaData)
				{
					return ((WeaponMetaData)_itemData).cost;
				}
				if (_itemData is StigmataMetaData)
				{
					return ((StigmataMetaData)_itemData).cost;
				}
				if (_itemData is ItemMetaData)
				{
					return ((ItemMetaData)_itemData).cost;
				}
				return -1;
			}
		}

		public int suite
		{
			get
			{
				if (_itemData is StigmataMetaData)
				{
					return ((StigmataMetaData)_itemData).setID;
				}
				return -1;
			}
		}

		public ItempediaDataAdapter(object data)
		{
			_itemData = data;
		}

		public StorageDataItemBase GetDummyStorageItemData()
		{
			if (_dummyStorageItemData == null)
			{
				if (_itemData is WeaponMetaData)
				{
					WeaponMetaData weaponMetaData = (WeaponMetaData)_itemData;
					_dummyStorageItemData = new WeaponDataItem(0, weaponMetaData);
					_dummyStorageItemData.level = weaponMetaData.maxLv;
				}
				else if (_itemData is StigmataMetaData)
				{
					StigmataMetaData stigmataMetaData = (StigmataMetaData)_itemData;
					_dummyStorageItemData = new StigmataDataItem(0, stigmataMetaData);
					_dummyStorageItemData.level = stigmataMetaData.maxLv;
					((StigmataDataItem)_dummyStorageItemData).SetAffixSkill(true, 0, 0);
				}
				else if (_itemData is ItemMetaData)
				{
					_dummyStorageItemData = new MaterialDataItem((ItemMetaData)_itemData);
				}
			}
			return _dummyStorageItemData;
		}

		public static int CompareToRarityDesc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
		{
			if (lobj.rarity != robj.rarity)
			{
				return robj.rarity - lobj.rarity;
			}
			if (lobj.level != robj.level)
			{
				return robj.level - lobj.level;
			}
			return robj.ID - lobj.ID;
		}

		public static int CompareToRarityAsc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
		{
			if (lobj.rarity != robj.rarity)
			{
				return lobj.rarity - robj.rarity;
			}
			if (lobj.level != robj.level)
			{
				return lobj.level - robj.level;
			}
			return lobj.ID - robj.ID;
		}

		public static int CompareToLevelDesc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
		{
			if (lobj.level != robj.level)
			{
				return robj.level - lobj.level;
			}
			if (lobj.rarity != robj.rarity)
			{
				return robj.rarity - lobj.rarity;
			}
			return robj.ID - lobj.ID;
		}

		public static int CompareToLevelAsc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
		{
			if (lobj.level != robj.level)
			{
				return lobj.level - robj.level;
			}
			if (lobj.rarity != robj.rarity)
			{
				return lobj.rarity - robj.rarity;
			}
			return lobj.ID - robj.ID;
		}

		public static int CompareToBaseTypeDesc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
		{
			if (lobj.baseType != robj.baseType)
			{
				return robj.baseType - lobj.baseType;
			}
			return CompareToRarityDesc(lobj, robj);
		}

		public static int CompareToBaseTypeAsc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
		{
			if (lobj.baseType != robj.baseType)
			{
				return lobj.baseType - robj.baseType;
			}
			return CompareToRarityAsc(lobj, robj);
		}

		public static int CompareToCostDesc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
		{
			if (lobj.cost != robj.cost)
			{
				return robj.cost - lobj.cost;
			}
			return CompareToRarityDesc(lobj, robj);
		}

		public static int CompareToCostAsc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
		{
			if (lobj.cost != robj.cost)
			{
				return lobj.cost - robj.cost;
			}
			return CompareToRarityAsc(lobj, robj);
		}

		public static int CompareToSuiteDesc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
		{
			if (lobj.suite != robj.suite)
			{
				if (lobj.suite == 0)
				{
					return 1;
				}
				if (robj.suite == 0)
				{
					return -1;
				}
				return robj.suite - lobj.suite;
			}
			return CompareToRarityDesc(lobj, robj);
		}

		public static int CompareToSuiteAsc(ItempediaDataAdapter lobj, ItempediaDataAdapter robj)
		{
			if (lobj.suite != robj.suite)
			{
				if (lobj.suite == 0)
				{
					return 1;
				}
				if (robj.suite == 0)
				{
					return -1;
				}
				return lobj.suite - robj.suite;
			}
			return CompareToRarityAsc(lobj, robj);
		}
	}
}
