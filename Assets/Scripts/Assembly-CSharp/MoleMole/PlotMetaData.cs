namespace MoleMole
{
	public class PlotMetaData
	{
		public readonly int plotID;

		public readonly int levelID;

		public readonly int startDialogID;

		public readonly int endDialogID;

		public PlotMetaData(int plotID, int levelID, int startDialogID, int endDialogID)
		{
			this.plotID = plotID;
			this.levelID = levelID;
			this.startDialogID = startDialogID;
			this.endDialogID = endDialogID;
		}
	}
}
