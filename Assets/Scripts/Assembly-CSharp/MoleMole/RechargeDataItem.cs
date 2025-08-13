using proto;

namespace MoleMole
{
	public class RechargeDataItem
	{
		public string productID;

		public string productName;

		public string formattedPrice = string.Empty;

		public ProductType productType;

		public int payHardCoin;

		public int freeHardCoin;

		public int serverPrice;

		public int leftBuyTimes;

		public int cardDailyHardCoin;

		public int cardLeftDays;

		public RechargeDataItem(RechargeDataItem rechargeItem)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			productID = rechargeItem.productID;
			productName = rechargeItem.productName;
			formattedPrice = rechargeItem.formattedPrice;
			productType = rechargeItem.productType;
			payHardCoin = rechargeItem.payHardCoin;
			freeHardCoin = rechargeItem.freeHardCoin;
			serverPrice = rechargeItem.serverPrice;
			leftBuyTimes = rechargeItem.leftBuyTimes;
			cardDailyHardCoin = rechargeItem.cardDailyHardCoin;
			cardLeftDays = rechargeItem.cardLeftDays;
		}

		public RechargeDataItem(Product product)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			productID = product.name;
			productName = product.desc;
			formattedPrice = "¥" + (float)product.price / 100f;
			productType = product.type;
			payHardCoin = (int)product.pay_hcoin;
			freeHardCoin = (int)product.free_hcoin;
			serverPrice = (int)product.price;
			leftBuyTimes = (int)product.left_buy_times;
			cardDailyHardCoin = (int)product.card_daily_hcoin;
			cardLeftDays = (int)product.card_left_days;
		}

		public void UpdateFromProduct(Product product)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			productName = product.desc;
			productType = product.type;
			payHardCoin = (int)product.pay_hcoin;
			freeHardCoin = (int)product.free_hcoin;
			serverPrice = (int)product.price;
			leftBuyTimes = (int)product.left_buy_times;
			cardDailyHardCoin = (int)product.card_daily_hcoin;
			cardLeftDays = (int)product.card_left_days;
		}

		public bool CanPurchase()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Invalid comparison between Unknown and I4
			if ((int)productType == 2 && leftBuyTimes <= 0)
			{
				return false;
			}
			if ((int)productType == 3 && cardLeftDays >= 180)
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			return string.Format("<RechargeDataItem>\nID: {0}\nname: {1}\nformattedPrice: {2}\ntype: {3}\npayHCoin: {4}\nfreeHCoin: {5}\nserverPrice: {6}\nleftBuyTimes: {7}\ncardDailyHCoin: {8}\ncardLeftDays: {9}", productID, productName, formattedPrice, productType, payHardCoin, freeHardCoin, serverPrice, leftBuyTimes, cardDailyHardCoin, cardLeftDays);
		}
	}
}
