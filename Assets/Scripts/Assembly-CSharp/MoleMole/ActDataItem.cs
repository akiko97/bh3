namespace MoleMole
{
	public class ActDataItem
	{
		public enum ActType
		{
			Normal = 1,
			Extra = 2
		}

		private ActMetaData _metaData;

		public int actId;

		public int chapterId;

		public string levelPanelPath;

		public int actIndex
		{
			get
			{
				return _metaData.numInChapter - 1;
			}
		}

		public string actTitle
		{
			get
			{
				return (_metaData.actType != 2) ? ("Act." + _metaData.numInChapter) : "Act.Extra";
			}
		}

		public string actName
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.actName);
			}
		}

		public string smallImgPath
		{
			get
			{
				return _metaData.smallImgPath;
			}
		}

		public string BGImgPath
		{
			get
			{
				return _metaData.bgImgPath;
			}
		}

		public ActType actType
		{
			get
			{
				return (ActType)_metaData.actType;
			}
		}

		public ActDataItem(int actId)
		{
			_metaData = ActMetaDataReader.GetActMetaDataByKey(actId);
			this.actId = _metaData.actId;
			chapterId = _metaData.chapterId;
			levelPanelPath = _metaData.levelPannelPath;
		}
	}
}
