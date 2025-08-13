namespace MoleMole
{
	public interface IEvtWithRemoteID
	{
		uint GetChannelID();

		uint GetSenderID();

		uint GetRemoteID();
	}
}
