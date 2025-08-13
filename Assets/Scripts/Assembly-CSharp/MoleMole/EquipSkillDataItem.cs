using System.Collections.Generic;

namespace MoleMole
{
	public class EquipSkillDataItem
	{
		private float _param1;

		private float _param1Add;

		private float _param2;

		private float _param2Add;

		private float _param3;

		private float _param3Add;

		private EquipSkillMetaData _metaData;

		public int ID { get; private set; }

		public string skillName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.skillName);
			}
		}

		public EquipSkillDataItem(int id, float param1, float param2, float param3, float param1Add, float param2Add, float param3Add)
		{
			ID = id;
			_metaData = EquipSkillMetaDataReader.GetEquipSkillMetaDataByKey(id);
			if (_metaData == null)
			{
			}
			_param1 = param1;
			_param1Add = param1Add;
			_param2 = param2;
			_param2Add = param2Add;
			_param3 = param3;
			_param3Add = param3Add;
		}

		public string GetSkillDisplay(int equipLevel = 1)
		{
			return LocalizationGeneralLogic.GetTextWithParamArray(_metaData.skillDisplay, MiscData.GetColor("Blue"), GetSkillParamArray(equipLevel));
		}

		public float[] GetSkillParamArray(int equipLevel = 1)
		{
			return new float[3]
			{
				GetSkillParam1(equipLevel),
				GetSkillParam2(equipLevel),
				GetSkillParam3(equipLevel)
			};
		}

		public float GetSkillParam1(int equipLevel)
		{
			if (equipLevel < 1)
			{
				return 0f;
			}
			return _param1 + (float)(equipLevel - 1) * _param1Add;
		}

		public float GetSkillParam2(int equipLevel)
		{
			if (equipLevel < 1)
			{
				return 0f;
			}
			return _param2 + (float)(equipLevel - 1) * _param2Add;
		}

		public float GetSkillParam3(int equipLevel)
		{
			if (equipLevel < 1)
			{
				return 0f;
			}
			return _param3 + (float)(equipLevel - 1) * _param3Add;
		}

		public List<float> GetSkillParamList(int equipLevel)
		{
			if (equipLevel < 1)
			{
				return null;
			}
			List<float> list = new List<float>();
			list.Add(GetSkillParam1(equipLevel));
			list.Add(GetSkillParam2(equipLevel));
			list.Add(GetSkillParam3(equipLevel));
			return list;
		}

		private string GetSkillParaToDisplay(float paraValue)
		{
			return paraValue.ToString();
		}
	}
}
