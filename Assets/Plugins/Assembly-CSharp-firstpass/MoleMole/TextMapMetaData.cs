namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class TextMapMetaData : IHashable
	{
		public readonly string ID;

		public readonly string Text;

		public TextMapMetaData(string ID, string Text)
		{
			this.ID = ID;
			this.Text = Text;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(ID, ref lastHash);
			HashUtils.ContentHashOnto(Text, ref lastHash);
		}
	}
}
