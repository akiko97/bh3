using proto;

namespace MoleMole
{
	public class CabinBronyaEnhanceDataItem : CabinAvatarEnhanceDataItem
	{
		private static CabinBronyaEnhanceDataItem _instance;

		private CabinBronyaEnhanceDataItem()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			cabinType = (CabinType)7;
			_techTree = new CabinTechTree(cabinType);
			level = 0;
			extendGrade = 1;
		}

		public static CabinBronyaEnhanceDataItem GetInstance()
		{
			if (_instance == null)
			{
				_instance = new CabinBronyaEnhanceDataItem();
			}
			return _instance;
		}
	}
}
