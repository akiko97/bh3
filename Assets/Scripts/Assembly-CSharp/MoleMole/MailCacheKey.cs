using System;
using proto;

namespace MoleMole
{
	public class MailCacheKey
	{
		public MailType type = (MailType)3;

		public int id;

		public DateTime time = DateTime.Now;

		public MailCacheKey(MailType type, int id, DateTime time)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			this.type = type;
			this.id = id;
			this.time = time;
		}

		public MailCacheKey()
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)

	}
}
