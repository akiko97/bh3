using System.Collections.Generic;

namespace MoleMole
{
	public class EquipSetDataItem
	{
		public readonly int ID;

		public readonly int ownNum;

		private EquipmentSetMetaData _metaData;

		public SortedDictionary<int, EquipSkillDataItem> EquipSkillDict { get; private set; }

		public string EquipSetName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.setName);
			}
		}

		public string EquipSetDesc
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.setDesc);
			}
		}

		public EquipSetDataItem(int id, int ownNum = 0)
		{
			ID = id;
			this.ownNum = ownNum;
			_metaData = EquipmentSetMetaDataReader.GetEquipmentSetMetaDataByKey(id);
			EquipSkillDict = new SortedDictionary<int, EquipSkillDataItem>();
			if (_metaData != null)
			{
				if (_metaData.prop1ID != 0)
				{
					EquipSkillDict.Add(_metaData.spellEffectNum1, new EquipSkillDataItem(_metaData.prop1ID, _metaData.prop1Param1, _metaData.prop1Param2, _metaData.prop1Param3, _metaData.prop1Param1Add, _metaData.prop1Param2Add, _metaData.prop1Param3Add));
				}
				if (_metaData.prop2ID != 0)
				{
					EquipSkillDict.Add(_metaData.spellEffectNum2, new EquipSkillDataItem(_metaData.prop2ID, _metaData.prop2Param1, _metaData.prop2Param2, _metaData.prop2Param3, _metaData.prop2Param1Add, _metaData.prop2Param2Add, _metaData.prop2Param3Add));
				}
				if (_metaData.prop3ID != 0)
				{
					EquipSkillDict.Add(_metaData.spellEffectNum3, new EquipSkillDataItem(_metaData.prop3ID, _metaData.prop3Param1, _metaData.prop3Param2, _metaData.prop3Param3, _metaData.prop3Param1Add, _metaData.prop3Param2Add, _metaData.prop3Param3Add));
				}
			}
		}

		public Dictionary<int, EquipSkillDataItem> GetOwnSetSkills()
		{
			Dictionary<int, EquipSkillDataItem> dictionary = new Dictionary<int, EquipSkillDataItem>();
			foreach (KeyValuePair<int, EquipSkillDataItem> item in EquipSkillDict)
			{
				if (item.Key <= ownNum)
				{
					dictionary.Add(item.Key, item.Value);
				}
			}
			return dictionary;
		}
	}
}
