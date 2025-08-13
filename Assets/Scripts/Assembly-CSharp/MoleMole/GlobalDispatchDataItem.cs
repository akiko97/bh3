using System.Collections.Generic;
using SimpleJSON;

namespace MoleMole
{
	public class GlobalDispatchDataItem
	{
		public class RegionDataItem
		{
			public string dispatchUrl;

			public string name;
		}

		public readonly List<RegionDataItem> regionList;

		public GlobalDispatchDataItem(JSONNode json)
		{
			regionList = new List<RegionDataItem>();
			foreach (JSONNode child in json["region_list"].Childs)
			{
				regionList.Add(new RegionDataItem
				{
					dispatchUrl = child["dispatch_url"],
					name = child["name"]
				});
			}
		}
	}
}
