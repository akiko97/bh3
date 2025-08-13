using proto;

namespace MoleMole
{
	public class CabinMeiEnhanceDataItem : CabinAvatarEnhanceDataItem
	{
		private static CabinMeiEnhanceDataItem _instance;

		private CabinMeiEnhanceDataItem()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			cabinType = (CabinType)6;
			_techTree = new CabinTechTree(cabinType);
			level = 0;
			extendGrade = 1;
		}

		public static CabinMeiEnhanceDataItem GetInstance()
		{
			if (_instance == null)
			{
				_instance = new CabinMeiEnhanceDataItem();
			}
			return _instance;
		}
	}
}
