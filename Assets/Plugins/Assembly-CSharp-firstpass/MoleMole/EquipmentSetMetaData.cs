namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class EquipmentSetMetaData : IHashable
	{
		public readonly int ID;

		public readonly string setName;

		public readonly string setDesc;

		public readonly int prop1ID;

		public readonly int spellEffectNum1;

		public readonly float prop1Param1;

		public readonly float prop1Param2;

		public readonly float prop1Param3;

		public readonly float prop1Param1Add;

		public readonly float prop1Param2Add;

		public readonly float prop1Param3Add;

		public readonly int prop2ID;

		public readonly int spellEffectNum2;

		public readonly float prop2Param1;

		public readonly float prop2Param2;

		public readonly float prop2Param3;

		public readonly float prop2Param1Add;

		public readonly float prop2Param2Add;

		public readonly float prop2Param3Add;

		public readonly int prop3ID;

		public readonly int spellEffectNum3;

		public readonly float prop3Param1;

		public readonly float prop3Param2;

		public readonly float prop3Param3;

		public readonly float prop3Param1Add;

		public readonly float prop3Param2Add;

		public readonly float prop3Param3Add;

		public EquipmentSetMetaData(int ID, string setName, string setDesc, int prop1ID, int spellEffectNum1, float prop1Param1, float prop1Param2, float prop1Param3, float prop1Param1Add, float prop1Param2Add, float prop1Param3Add, int prop2ID, int spellEffectNum2, float prop2Param1, float prop2Param2, float prop2Param3, float prop2Param1Add, float prop2Param2Add, float prop2Param3Add, int prop3ID, int spellEffectNum3, float prop3Param1, float prop3Param2, float prop3Param3, float prop3Param1Add, float prop3Param2Add, float prop3Param3Add)
		{
			this.ID = ID;
			this.setName = setName;
			this.setDesc = setDesc;
			this.prop1ID = prop1ID;
			this.spellEffectNum1 = spellEffectNum1;
			this.prop1Param1 = prop1Param1;
			this.prop1Param2 = prop1Param2;
			this.prop1Param3 = prop1Param3;
			this.prop1Param1Add = prop1Param1Add;
			this.prop1Param2Add = prop1Param2Add;
			this.prop1Param3Add = prop1Param3Add;
			this.prop2ID = prop2ID;
			this.spellEffectNum2 = spellEffectNum2;
			this.prop2Param1 = prop2Param1;
			this.prop2Param2 = prop2Param2;
			this.prop2Param3 = prop2Param3;
			this.prop2Param1Add = prop2Param1Add;
			this.prop2Param2Add = prop2Param2Add;
			this.prop2Param3Add = prop2Param3Add;
			this.prop3ID = prop3ID;
			this.spellEffectNum3 = spellEffectNum3;
			this.prop3Param1 = prop3Param1;
			this.prop3Param2 = prop3Param2;
			this.prop3Param3 = prop3Param3;
			this.prop3Param1Add = prop3Param1Add;
			this.prop3Param2Add = prop3Param2Add;
			this.prop3Param3Add = prop3Param3Add;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(ID, ref lastHash);
			HashUtils.ContentHashOnto(setName, ref lastHash);
			HashUtils.ContentHashOnto(setDesc, ref lastHash);
			HashUtils.ContentHashOnto(prop1ID, ref lastHash);
			HashUtils.ContentHashOnto(spellEffectNum1, ref lastHash);
			HashUtils.ContentHashOnto(prop1Param1, ref lastHash);
			HashUtils.ContentHashOnto(prop1Param2, ref lastHash);
			HashUtils.ContentHashOnto(prop1Param3, ref lastHash);
			HashUtils.ContentHashOnto(prop1Param1Add, ref lastHash);
			HashUtils.ContentHashOnto(prop1Param2Add, ref lastHash);
			HashUtils.ContentHashOnto(prop1Param3Add, ref lastHash);
			HashUtils.ContentHashOnto(prop2ID, ref lastHash);
			HashUtils.ContentHashOnto(spellEffectNum2, ref lastHash);
			HashUtils.ContentHashOnto(prop2Param1, ref lastHash);
			HashUtils.ContentHashOnto(prop2Param2, ref lastHash);
			HashUtils.ContentHashOnto(prop2Param3, ref lastHash);
			HashUtils.ContentHashOnto(prop2Param1Add, ref lastHash);
			HashUtils.ContentHashOnto(prop2Param2Add, ref lastHash);
			HashUtils.ContentHashOnto(prop2Param3Add, ref lastHash);
			HashUtils.ContentHashOnto(prop3ID, ref lastHash);
			HashUtils.ContentHashOnto(spellEffectNum3, ref lastHash);
			HashUtils.ContentHashOnto(prop3Param1, ref lastHash);
			HashUtils.ContentHashOnto(prop3Param2, ref lastHash);
			HashUtils.ContentHashOnto(prop3Param3, ref lastHash);
			HashUtils.ContentHashOnto(prop3Param1Add, ref lastHash);
			HashUtils.ContentHashOnto(prop3Param2Add, ref lastHash);
			HashUtils.ContentHashOnto(prop3Param3Add, ref lastHash);
		}
	}
}
