using SimpleJSON;

namespace MoleMole
{
	public sealed class StageTypeData
	{
		public uint TypeID { get; private set; }

		public string TypeName { get; private set; }

		public string PerpStagePrefabPath { get; private set; }

		public string OrthStagePrefabPath { get; private set; }

		public StageTypeData(JSONNode aJson)
		{
			TypeID = (uint)aJson["TypeID"].AsInt;
			TypeName = aJson["TypeName"].Value;
			PerpStagePrefabPath = aJson["PerpStagePrefabPath"].Value;
			OrthStagePrefabPath = aJson["OrthStagePrefabPath"].Value;
		}
	}
}
