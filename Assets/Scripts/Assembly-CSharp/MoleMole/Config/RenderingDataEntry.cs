namespace MoleMole.Config
{
	public class RenderingDataEntry
	{
		private const string RENDERING_CONFIG_PATH_PREFIX = "Rendering/";

		public string name;

		public string dataPath;

		public string GetDataPath()
		{
			return "Rendering/" + dataPath;
		}
	}
}
