namespace MoleMole
{
	public class WeixinPayResult : PayResult
	{
		private string _result;

		public WeixinPayResult(string rawResult)
		{
			if (!string.IsNullOrEmpty(rawResult))
			{
				_result = rawResult;
				if (_result == "Succ")
				{
					payRetCode = PayRetcode.SUCCESS;
				}
				else if (_result == "ErrUserCancel")
				{
					payRetCode = PayRetcode.CANCELED;
				}
				else
				{
					payRetCode = PayRetcode.FAILED;
				}
			}
		}

		public override string GetResultShowText()
		{
			if (payRetCode == PayRetcode.FAILED)
			{
				if (_result == "ErrWXAppNotInstalled")
				{
					return LocalizationGeneralLogic.GetText("IAP_ErrWXAppNotInstalled");
				}
				if (_result == "ErrWXAppNotSupportAPI")
				{
					return LocalizationGeneralLogic.GetText("IAP_ErrWXAppNotSupportAPI");
				}
				if (_result == "ErrFailCreateWeiXinOrder")
				{
					return LocalizationGeneralLogic.GetText("IAP_FailCreateWeiXinOrder");
				}
				return LocalizationGeneralLogic.GetText("IAPTransitionFailed");
			}
			return base.GetResultShowText();
		}
	}
}
