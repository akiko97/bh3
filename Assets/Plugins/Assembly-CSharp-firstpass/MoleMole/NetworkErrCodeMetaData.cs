namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class NetworkErrCodeMetaData : IHashable
	{
		public readonly string errType;

		public readonly string retCode;

		public readonly string textMapID;

		public NetworkErrCodeMetaData(string errType, string retCode, string textMapID)
		{
			this.errType = errType;
			this.retCode = retCode;
			this.textMapID = textMapID;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(errType, ref lastHash);
			HashUtils.ContentHashOnto(retCode, ref lastHash);
			HashUtils.ContentHashOnto(textMapID, ref lastHash);
		}
	}
}
