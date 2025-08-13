namespace MoleMole
{
	public class SDKPayResult : PayResult
	{
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
