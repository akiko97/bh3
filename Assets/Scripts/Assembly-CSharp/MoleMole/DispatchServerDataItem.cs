using SimpleJSON;

namespace MoleMole
{
	public class DispatchServerDataItem
	{
		public readonly string host;

		public readonly ushort port;

		public readonly string assetBundleUrl;

		public readonly string accountUrl;

		public readonly bool isReview;

		public readonly bool forbidNewUser;

		public readonly bool forbidRecharge;

		public readonly int rechargeMaxLimit;

		public readonly bool dataUseAssetBundleUseSever;

		public readonly bool resUseAssetBundleUseSever;

		public readonly bool dataUseAssetBundle;

		public readonly bool resUseAssetBundle;

		public readonly string oaServerUrl;

		public readonly bool showVersionText;

		public DispatchServerDataItem(JSONNode json)
		{
			host = json["gateway"]["ip"];
			port = (ushort)json["gateway"]["port"].AsInt;
			assetBundleUrl = json["asset_boundle_url"];
			accountUrl = json["account_url"];
			isReview = !string.IsNullOrEmpty(json["ext"]["is_xxxx"]) && json["ext"]["is_xxxx"].AsInt == 1;
			forbidNewUser = !string.IsNullOrEmpty(json["ext"]["forbid_new_user"]) && json["ext"]["forbid_new_user"].AsInt == 1;
			forbidRecharge = !string.IsNullOrEmpty(json["ext"]["forbid_recharge"]) && json["ext"]["forbid_recharge"].AsInt == 1;
			rechargeMaxLimit = ((!string.IsNullOrEmpty(json["ext"]["recharge_max_limit"])) ? json["ext"]["recharge_max_limit"].AsInt : 0);
			oaServerUrl = json["oaserver_url"];
			dataUseAssetBundleUseSever = !string.IsNullOrEmpty(json["ext"]["data_use_asset_boundle"]);
			resUseAssetBundleUseSever = !string.IsNullOrEmpty(json["ext"]["res_use_asset_boundle"]);
			dataUseAssetBundle = !string.IsNullOrEmpty(json["ext"]["data_use_asset_boundle"]) && json["ext"]["data_use_asset_boundle"].AsInt == 1;
			resUseAssetBundle = !string.IsNullOrEmpty(json["ext"]["res_use_asset_boundle"]) && json["ext"]["res_use_asset_boundle"].AsInt == 1;
			showVersionText = !string.IsNullOrEmpty(json["ext"]["show_version_text"]) && json["ext"]["show_version_text"].AsInt == 1;
		}
	}
}
