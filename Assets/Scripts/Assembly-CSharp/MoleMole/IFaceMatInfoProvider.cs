namespace MoleMole
{
	public interface IFaceMatInfoProvider
	{
		int capacity { get; }

		FaceMatInfo GetFaceMatInfo(int index);

		string[] GetMatInfoNames();
	}
}
