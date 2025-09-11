using System;

namespace MoleMole
{
	public class AccountManager
	{
		public readonly ConfigAccount accountConfig;

		public readonly TheBaseAccountManager manager;

		public OpeUtil.ApkCommentInfo apkCommentInfo;

		public string apkSignature;

		private AccountManager()
		{
			accountConfig = new ConfigAccount();
			switch (accountConfig.accountBranch)
			{
			case ConfigAccount.AccountBranch.Original:
				manager = new TheOriginalAccountManager();
				break;
			case ConfigAccount.AccountBranch.UC:
				manager = new TheUCAccountManager();
				break;
			case ConfigAccount.AccountBranch.QIHOO:
				manager = new TheQihooAccountManager();
				break;
			case ConfigAccount.AccountBranch.OPPO:
				manager = new TheOppoAccountManager();
				break;
			case ConfigAccount.AccountBranch.VIVO:
				manager = new TheVivoAccountManager();
				break;
			case ConfigAccount.AccountBranch.HUAWEI:
				manager = new TheHuaweiAccountManager();
				break;
			case ConfigAccount.AccountBranch.XIAOMI:
				manager = new TheXiaoMiAccountManager();
				break;
			case ConfigAccount.AccountBranch.TENCENT:
				manager = new TheTencentAccountManager();
				break;
			case ConfigAccount.AccountBranch.GIONEE:
				manager = new TheAmigoAccountManager();
				break;
			case ConfigAccount.AccountBranch.LENOVO:
				manager = new TheLenovoAccountManager();
				break;
			case ConfigAccount.AccountBranch.BAIDU:
				manager = new TheBaiduAccountManager();
				break;
			case ConfigAccount.AccountBranch.COOLPAD:
				manager = new TheCoolpadAccountManager();
				break;
			case ConfigAccount.AccountBranch.WANDOUJIA:
				manager = new TheWandouAccountManager();
				break;
			case ConfigAccount.AccountBranch.MEIZU:
				manager = new TheMeizuAccountManager();
				break;
			case ConfigAccount.AccountBranch.BILIBILI:
				manager = new TheBiliAccountManager();
				break;
			default:
				throw new Exception("Invalid Type or State!: " + accountConfig.accountBranch);
			}
		}

		public bool AllowTryUserLogin()
		{
			return accountConfig.accountBranch == ConfigAccount.AccountBranch.Original;
		}

		public void SetupApkCommentInfo()
		{
			//apkCommentInfo = null;
			//apkCommentInfo = OpeUtil.GetApkComment();
		}
	}
}
