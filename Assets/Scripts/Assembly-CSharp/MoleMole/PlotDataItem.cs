namespace MoleMole
{
	public class PlotDataItem
	{
		public int plotID;

		public int levelID;

		public int startDialogID;

		public int endDialogID;

		public PlotDataItem(int plotID, int levelID, int startDialogID, int endDialogID)
		{
			this.plotID = plotID;
			this.levelID = levelID;
			this.startDialogID = startDialogID;
			this.endDialogID = endDialogID;
		}

		public PlotDataItem(PlotMetaData plotMetaData)
		{
			plotID = plotMetaData.plotID;
			levelID = plotMetaData.levelID;
			startDialogID = plotMetaData.startDialogID;
			endDialogID = plotMetaData.endDialogID;
		}

		public override string ToString()
		{
			return string.Format("<PlotDataItem>\nplotID: {0}\nlevelID: {1}\nstartDialogID: {2}\nendDialogID: {3}", plotID, levelID, startDialogID, endDialogID);
		}
	}
}
