using proto;

namespace MoleMole
{
	public class CabinKianaEnhanceDataItem : CabinAvatarEnhanceDataItem
	{
		private static CabinKianaEnhanceDataItem _instance;

		private CabinKianaEnhanceDataItem()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			cabinType = (CabinType)2;
			_techTree = new CabinTechTree(cabinType);
			level = 0;
			extendGrade = 1;
		}

		public static CabinKianaEnhanceDataItem GetInstance()
		{
			if (_instance == null)
			{
				_instance = new CabinKianaEnhanceDataItem();
			}
			return _instance;
		}
	}
}
