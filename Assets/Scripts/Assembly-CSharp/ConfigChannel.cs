using System.Collections.Generic;
using MoleMole;

public class ConfigChannel
{
	public string ChannelName;

	public string BundleIdentifier;

	public string ProductName;

	public string PreDefines;

	public List<string> DispatchUrls;

	public int VersionCode;

	public bool DataUseAssetBundle;

	public bool EventUseAssetBundle;

	public ConfigAccount.AccountBranch AccountBranch;

	public ConfigAccount.PaymentBranch PaymentBranch;
}
