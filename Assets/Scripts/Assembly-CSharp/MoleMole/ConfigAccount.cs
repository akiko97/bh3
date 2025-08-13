namespace MoleMole
{
	public class ConfigAccount
	{
		public enum AccountBranch
		{
			Original = 0,
			UC = 1,
			QIHOO = 2,
			OPPO = 3,
			VIVO = 4,
			HUAWEI = 5,
			XIAOMI = 6,
			TENCENT = 7,
			GIONEE = 8,
			LENOVO = 9,
			BAIDU = 10,
			COOLPAD = 11,
			WANDOUJIA = 12,
			MEIZU = 13,
			BILIBILI = 14
		}

		public enum PaymentBranch
		{
			DEFAULT = 0,
			APPSTORE_CN = 1,
			ORIGINAL_ANDROID_PAY = 2
		}

		public AccountBranch accountBranch;

		public PaymentBranch paymentBranch;

		public ConfigAccount()
		{
			ConfigChannel configChannel = ConfigUtil.LoadJSONConfig<ConfigChannel>("DataPersistent/BuildChannel/ChannelConfig");
			accountBranch = configChannel.AccountBranch;
			paymentBranch = configChannel.PaymentBranch;
		}
	}
}
