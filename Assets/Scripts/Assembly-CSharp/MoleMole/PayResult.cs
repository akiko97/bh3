namespace MoleMole
{
	public class PayResult
	{
		public enum PayRetcode
		{
			SUCCESS = 0,
			CONFIRMING = 1,
			CANCELED = 2,
			FAILED = 3
		}

		public PayRetcode payRetCode;

		public PayResult()
		{
		}

		public PayResult(PayRetcode payRetCode)
		{
			this.payRetCode = payRetCode;
		}

		public virtual string GetResultShowText()
		{
			return string.Empty;
		}
	}
}
