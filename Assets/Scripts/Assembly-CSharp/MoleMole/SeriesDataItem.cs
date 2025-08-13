using System.Collections.Generic;

namespace MoleMole
{
	public class SeriesDataItem
	{
		private SeriesMetaData _metaData;

		public List<WeekDayActivityDataItem> weekActivityList;

		public int id
		{
			get
			{
				return _metaData.id;
			}
		}

		public string title
		{
			get
			{
				return LocalizationGeneralLogic.GetText(_metaData.title);
			}
		}

		public SeriesDataItem(int seriesID)
		{
			_metaData = SeriesMetaDataReader.GetSeriesMetaDataByKey(seriesID);
			weekActivityList = new List<WeekDayActivityDataItem>();
		}
	}
}
