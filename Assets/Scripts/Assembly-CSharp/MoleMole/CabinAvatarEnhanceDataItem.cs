using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class CabinAvatarEnhanceDataItem : CabinDataItemBase
	{
		public AvatarClassType _classType;

		public float GetAvatarAttrEnhance(AvatarAttrType attrType)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Invalid comparison between I4 and Unknown
			float num = 0f;
			List<CabinTechTreeNode> activeNodeList = _techTree.GetActiveNodeList();
			foreach (CabinTechTreeNode item in activeNodeList)
			{
				CabinTechTreeMetaData metaData = item._metaData;
				if (metaData.Argument1 == (int)attrType)
				{
					num += (float)metaData.Argument2 / 100f;
				}
			}
			return num;
		}

		public int GetTotalEnhancePoint()
		{
			return _techTree.GetActiveNodeList().Count;
		}

		public void SetAvatarClassType(CabinType cabinType)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Invalid comparison between Unknown and I4
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Invalid comparison between Unknown and I4
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if ((int)cabinType == 2)
			{
				_classType = (AvatarClassType)1;
			}
			else if ((int)cabinType == 6)
			{
				_classType = (AvatarClassType)2;
			}
			else if ((int)cabinType == 7)
			{
				_classType = (AvatarClassType)3;
			}
		}
	}
}
