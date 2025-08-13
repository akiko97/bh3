using proto;

namespace MoleMole
{
	public class CabinEngineDataItem : CabinDataItemBase
	{
		private static CabinEngineDataItem _instance;

		private CabinEngineDataItem()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			cabinType = (CabinType)1;
			_techTree = null;
			level = 0;
			extendGrade = 1;
		}

		public static CabinEngineDataItem GetInstance()
		{
			if (_instance == null)
			{
				_instance = new CabinEngineDataItem();
			}
			return _instance;
		}
	}
}
