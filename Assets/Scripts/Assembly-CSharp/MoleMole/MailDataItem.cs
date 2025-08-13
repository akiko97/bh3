using System;
using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class MailDataItem
	{
		public DateTime time;

		protected bool isAttachmentGot;

		public MailAttachment attachment;

		public int ID { get; private set; }

		public MailType type { get; private set; }

		public string title { get; private set; }

		public string content { get; private set; }

		public string sender { get; private set; }

		public bool hasAttachment
		{
			get
			{
				return attachment != null && attachment.itemList.Count > 0;
			}
		}

		public MailDataItem(Mail mail)
		{
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			MailAttachment mailAttachment;
			if (mail == null || mail.attachment == null)
			{
				mailAttachment = null;
			}
			else if ((mail.attachment.item_list == null || mail.attachment.item_list.Count == 0) && mail.attachment.hcoin == 0 && mail.attachment.scoin == 0)
			{
				mailAttachment = null;
			}
			else
			{
				mailAttachment = new MailAttachment();
				if (mail.attachment.hcoin != 0)
				{
					RewardUIData hcoinData = RewardUIData.GetHcoinData((int)mail.attachment.hcoin);
					mailAttachment.itemList.Add(hcoinData);
				}
				if (mail.attachment.scoin != 0)
				{
					RewardUIData scoinData = RewardUIData.GetScoinData((int)mail.attachment.scoin);
					mailAttachment.itemList.Add(scoinData);
				}
				foreach (MailItem item2 in mail.attachment.item_list)
				{
					RewardUIData item = new RewardUIData(ResourceType.Item, (int)item2.num, RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int)item2.item_id, (int)item2.level);
					mailAttachment.itemList.Add(item);
				}
			}
			bool flag = mail.is_attachment_gotSpecified && mail.is_attachment_got;
			Init((int)mail.id, mail.type, mail.title, mail.content, mail.sender, mail.time, mailAttachment, flag);
		}

		private void Init(int id, MailType type, string title, string content, string sender, uint time, MailAttachment attachment = null, bool isAttachmentGot = false)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			ID = id;
			this.type = type;
			this.title = title;
			this.content = content;
			this.sender = sender;
			this.time = Miscs.GetDateTimeFromTimeStamp(time);
			this.attachment = attachment;
			this.isAttachmentGot = isAttachmentGot;
		}

		public static int CompareToTimeDesc(MailDataItem lobj, MailDataItem robj)
		{
			if (lobj.hasAttachment && !robj.hasAttachment)
			{
				return -1;
			}
			if (!lobj.hasAttachment && robj.hasAttachment)
			{
				return 1;
			}
			if (!Singleton<MailModule>.Instance.IsMailRead(lobj) && Singleton<MailModule>.Instance.IsMailRead(robj))
			{
				return -1;
			}
			if (Singleton<MailModule>.Instance.IsMailRead(lobj) && !Singleton<MailModule>.Instance.IsMailRead(robj))
			{
				return 1;
			}
			return robj.time.CompareTo(lobj.time);
		}

		public KeyValuePair<MailType, int> GetKeyForMail()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return new KeyValuePair<MailType, int>(type, ID);
		}

		public MailCacheKey GetKeyForMailCache()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return new MailCacheKey(type, ID, time);
		}
	}
}
