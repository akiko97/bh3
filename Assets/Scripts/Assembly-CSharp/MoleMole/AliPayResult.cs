namespace MoleMole
{
	public class AliPayResult : PayResult
	{
		private string resultStatus;

		private string result;

		private string memo;

		public AliPayResult(string rawResult)
		{
			if (string.IsNullOrEmpty(rawResult))
			{
				return;
			}
			string[] array = rawResult.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.StartsWith("resultStatus"))
				{
					resultStatus = GetValue(text, "resultStatus");
				}
				else if (text.StartsWith("result"))
				{
					result = GetValue(text, "result");
				}
				else if (text.StartsWith("memo"))
				{
					memo = GetValue(text, "memo");
				}
			}
			if (resultStatus == "9000")
			{
				payRetCode = PayRetcode.SUCCESS;
			}
			else if (resultStatus == "8000")
			{
				payRetCode = PayRetcode.CONFIRMING;
			}
			else if (resultStatus == "6001")
			{
				payRetCode = PayRetcode.CANCELED;
			}
			else
			{
				payRetCode = PayRetcode.FAILED;
			}
		}

		private string GetValue(string content, string key)
		{
			string text = key + "={";
			int num = content.IndexOf(text) + text.Length;
			int length = content.LastIndexOf("}") - num;
			return content.Substring(num, length);
		}

		public string GetResultStatus()
		{
			return resultStatus;
		}

		public string GetMemo()
		{
			return memo;
		}

		public string GetResult()
		{
			return result;
		}

		public override string GetResultShowText()
		{
			if (payRetCode == PayRetcode.FAILED)
			{
				return LocalizationGeneralLogic.GetText("IAPTransitionFailed");
			}
			return base.GetResultShowText();
		}
	}
}
