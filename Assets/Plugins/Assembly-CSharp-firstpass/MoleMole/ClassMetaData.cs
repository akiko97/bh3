namespace MoleMole
{
	[GeneratePartialHash(CombineGeneratedFile = true)]
	public class ClassMetaData : IHashable
	{
		public readonly int classID;

		public readonly string firstName;

		public readonly string lastName;

		public readonly string enFirstName;

		public readonly string enLastName;

		public ClassMetaData(int classID, string firstName, string lastName, string enFirstName, string enLastName)
		{
			this.classID = classID;
			this.firstName = firstName;
			this.lastName = lastName;
			this.enFirstName = enFirstName;
			this.enLastName = enLastName;
		}

		public void ObjectContentHashOnto(ref int lastHash)
		{
			HashUtils.ContentHashOnto(classID, ref lastHash);
			HashUtils.ContentHashOnto(firstName, ref lastHash);
			HashUtils.ContentHashOnto(lastName, ref lastHash);
			HashUtils.ContentHashOnto(enFirstName, ref lastHash);
			HashUtils.ContentHashOnto(enLastName, ref lastHash);
		}
	}
}
