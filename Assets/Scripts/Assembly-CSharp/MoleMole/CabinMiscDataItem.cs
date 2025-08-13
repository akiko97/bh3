using proto;

namespace MoleMole
{
	public class CabinMiscDataItem : CabinDataItemBase
	{
		private static CabinMiscDataItem _instance;

		private CabinMiscDataItem()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			cabinType = (CabinType)4;
			_techTree = new CabinTechTree(cabinType);
			level = 0;
			extendGrade = 1;
		}

		public static CabinMiscDataItem GetInstance()
		{
			if (_instance == null)
			{
				_instance = new CabinMiscDataItem();
			}
			return _instance;
		}
	}
}
