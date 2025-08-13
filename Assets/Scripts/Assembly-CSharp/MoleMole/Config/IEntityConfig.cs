namespace MoleMole.Config
{
	public interface IEntityConfig
	{
		ConfigEntityAnimEvent TryGetAnimEvent(string animEventID);
	}
}
