using System.Collections.Generic;

namespace MoleMole
{
	public class AvatarMetaDataReaderExtend
	{
		private static Dictionary<int, AvatarIDs> _avatarIDMap;

		public static void LoadFromFileAndBuildMap()
		{
			AvatarMetaDataReader.LoadFromFile();
			_avatarIDMap = new Dictionary<int, AvatarIDs>();
			foreach (AvatarMetaData item in AvatarMetaDataReader.GetItemList())
			{
				AvatarIDs avatarIDs = new AvatarIDs();
				avatarIDs.avatarID = item.avatarID;
				avatarIDs.avatarCardID = item.avatarCardID;
				avatarIDs.avatarFragmentID = item.avatarFragmentID;
				AvatarIDs value = avatarIDs;
				_avatarIDMap.Add(item.avatarID, value);
				_avatarIDMap.Add(item.avatarCardID, value);
				_avatarIDMap.Add(item.avatarFragmentID, value);
			}
		}

		public static AvatarIDs GetAvatarIDsByKey(int id)
		{
			AvatarIDs value;
			_avatarIDMap.TryGetValue(id, out value);
			return value;
		}
	}
}
