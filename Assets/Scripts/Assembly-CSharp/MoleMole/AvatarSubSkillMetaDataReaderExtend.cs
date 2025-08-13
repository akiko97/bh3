using System.Collections.Generic;

namespace MoleMole
{
	public class AvatarSubSkillMetaDataReaderExtend
	{
		private static Dictionary<int, List<int>> _skillMap;

		public static void LoadFromFileAndBuildMap()
		{
			AvatarSubSkillMetaDataReader.LoadFromFile();
			List<AvatarSubSkillMetaData> itemList = AvatarSubSkillMetaDataReader.GetItemList();
			_skillMap = new Dictionary<int, List<int>>();
			foreach (AvatarSubSkillMetaData item in itemList)
			{
				if (!_skillMap.ContainsKey(item.skillId))
				{
					_skillMap.Add(item.skillId, new List<int>());
				}
				_skillMap[item.skillId].Add(item.avatarSubSkillId);
			}
		}

		public static List<int> GetAvatarSubSkillIdList(int skillId)
		{
			if (!_skillMap.ContainsKey(skillId))
			{
				return new List<int>();
			}
			return _skillMap[skillId];
		}
	}
}
