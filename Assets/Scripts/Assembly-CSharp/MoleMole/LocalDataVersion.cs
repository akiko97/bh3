namespace MoleMole
{
	public class LocalDataVersion
	{
		public static string version;

		public static void LoadFromFile()
		{
			ConfigUserLocalDataVersion configUserLocalDataVersion = ConfigUtil.LoadJSONConfig<ConfigUserLocalDataVersion>("Data/_BothLocalAndAssetBundle/LocalDataVersion");
			version = configUserLocalDataVersion.UserLocalDataVersion;
		}
	}
}
