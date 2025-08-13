using System.Collections.Generic;

namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class CabinDisjointEquipmentMetaData : IHashable
	{
		public class CabinDisjointOutputItem
		{
			public readonly int ID;

			public readonly int Num;

			public CabinDisjointOutputItem(int ID, int Num)
			{
				this.ID = ID;
				this.Num = Num;
			}

			public CabinDisjointOutputItem(string nodeString)
			{
				char[] seperator = new char[1] { ':' };
				List<string> stringListFromString = CommonUtils.GetStringListFromString(nodeString, seperator);
				ID = int.Parse(stringListFromString[0]);
				Num = int.Parse(stringListFromString[1]);
			}
		}

		public readonly int EquipmentID;

		public readonly int NeedSCoin;

		public readonly List<CabinDisjointOutputItem> Item;

		public CabinDisjointEquipmentMetaData(int EquipmentID, int NeedSCoin, List<CabinDisjointOutputItem> Item)
		{
			this.EquipmentID = EquipmentID;
			this.NeedSCoin = NeedSCoin;
			this.Item = Item;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(EquipmentID, ref lastHash);
			HashUtils.ContentHashOnto(NeedSCoin, ref lastHash);
			if (Item == null)
			{
				return;
			}
			foreach (CabinDisjointOutputItem item in Item)
			{
				HashUtils.ContentHashOnto(item.ID, ref lastHash);
				HashUtils.ContentHashOnto(item.Num, ref lastHash);
			}
		}
	}
}
