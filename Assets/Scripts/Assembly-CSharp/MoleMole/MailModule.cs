using System.Collections.Generic;
using System.Linq;
using proto;

namespace MoleMole
{
	public class MailModule : BaseModule
	{
		private Dictionary<KeyValuePair<MailType, int>, MailDataItem> _mailDict;

		public MailModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			_mailDict = new Dictionary<KeyValuePair<MailType, int>, MailDataItem>();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 85:
				return OnGetMailDataRsp(pkt.getData<GetMailDataRsp>());
			case 87:
				return OnGetMailAttachmentRsp(pkt.getData<GetMailAttachmentRsp>());
			default:
				return false;
			}
		}

		private bool OnGetMailDataRsp(GetMailDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				foreach (Mail item in rsp.mail_list)
				{
					MailDataItem mailDataItem = new MailDataItem(item);
					_mailDict[mailDataItem.GetKeyForMail()] = mailDataItem;
				}
			}
			return false;
		}

		private bool OnGetMailAttachmentRsp(GetMailAttachmentRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				foreach (MailKey item in rsp.succ_mail_key_list)
				{
					_mailDict.Remove(new KeyValuePair<MailType, int>(item.type, (int)item.id));
				}
				Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
			}
			return false;
		}

		public List<MailDataItem> GetAllMails()
		{
			List<MailDataItem> list = _mailDict.Values.ToList();
			list.Sort(MailDataItem.CompareToTimeDesc);
			return list;
		}

		public MailDataItem TryGetMailData(MailType mailType, int mailID)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			MailDataItem value;
			_mailDict.TryGetValue(new KeyValuePair<MailType, int>(mailType, mailID), out value);
			return value;
		}

		public void SetMailRead(MailDataItem mailData)
		{
			if (!IsMailRead(mailData) && _mailDict.ContainsKey(mailData.GetKeyForMail()))
			{
				Singleton<MiHoYoGameData>.Instance.LocalData.ReadMailIdList.Add(mailData.GetKeyForMailCache());
				Singleton<MiHoYoGameData>.Instance.Save();
			}
		}

		public bool IsMailRead(MailDataItem mailData)
		{
			MailCacheKey key = mailData.GetKeyForMailCache();
			List<MailCacheKey> readMailIdList = Singleton<MiHoYoGameData>.Instance.LocalData.ReadMailIdList;
			return readMailIdList.Find((MailCacheKey x) => x.type == key.type && x.id == key.id && x.time == key.time) != null;
		}

		public bool IsMailNew(MailDataItem mail)
		{
			List<MailCacheKey> oldMailCache = Singleton<MiHoYoGameData>.Instance.LocalData.OldMailCache;
			MailCacheKey key = mail.GetKeyForMailCache();
			if (oldMailCache.Find((MailCacheKey x) => x.type == key.type && x.id == key.id && x.time == key.time) == null)
			{
				return true;
			}
			return false;
		}

		public void SetMailAsOld(MailDataItem mail)
		{
			Singleton<MiHoYoGameData>.Instance.LocalData.OldMailCache.Add(mail.GetKeyForMailCache());
			Singleton<MiHoYoGameData>.Instance.Save();
		}

		public bool HasNewMail()
		{
			List<MailCacheKey> oldMailCache = Singleton<MiHoYoGameData>.Instance.LocalData.OldMailCache;
			foreach (KeyValuePair<KeyValuePair<MailType, int>, MailDataItem> item in _mailDict)
			{
				MailCacheKey key = item.Value.GetKeyForMailCache();
				if (oldMailCache.Find((MailCacheKey x) => x.type == key.type && x.id == key.id && x.time == key.time) == null)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasAttachmentMail()
		{
			foreach (KeyValuePair<KeyValuePair<MailType, int>, MailDataItem> item in _mailDict)
			{
				if (item.Value.hasAttachment)
				{
					return true;
				}
			}
			return false;
		}

		public void SetAllMailAsOld()
		{
			foreach (KeyValuePair<KeyValuePair<MailType, int>, MailDataItem> item in _mailDict)
			{
				Singleton<MiHoYoGameData>.Instance.LocalData.OldMailCache.Add(item.Value.GetKeyForMailCache());
			}
			Singleton<MiHoYoGameData>.Instance.Save();
		}
	}
}
