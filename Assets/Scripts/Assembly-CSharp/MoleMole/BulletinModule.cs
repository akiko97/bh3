using System;
using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class BulletinModule : BaseModule
	{
		private const uint SHOW_TYPE_EVENT = 0u;

		private const uint SHOW_TYPE_SYSTEM = 1u;

		private Dictionary<uint, Bulletin> _allBulletinDict;

		public DateTime LastCheckBulletinTime;

		public List<Bulletin> EventBulletinList { get; private set; }

		public List<Bulletin> SystemBulletinList { get; private set; }

		public BulletinModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			EventBulletinList = new List<Bulletin>();
			SystemBulletinList = new List<Bulletin>();
			_allBulletinDict = new Dictionary<uint, Bulletin>();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 138:
				return OnGetBulletinRsp(pkt.getData<GetBulletinRsp>());
			case 187:
				return OnUrgencyMsgNotify(pkt.getData<UrgencyMsgNotify>());
			default:
				return false;
			}
		}

		private bool OnGetBulletinRsp(GetBulletinRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				EventBulletinList.Clear();
				SystemBulletinList.Clear();
				_allBulletinDict.Clear();
				foreach (Bulletin item in rsp.bulletin_list)
				{
					if (item.type == 0)
					{
						EventBulletinList.Add(item);
					}
					else if (item.type == 1)
					{
						SystemBulletinList.Add(item);
					}
					_allBulletinDict[item.id] = item;
				}
			}
			else
			{
				string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
			}
			return false;
		}

		private bool OnUrgencyMsgNotify(UrgencyMsgNotify rsp)
		{
			if (Singleton<MainUIManager>.Instance != null)
			{
				Singleton<MainUIManager>.Instance.ShowWidget(new AnnouncementDialogContext(rsp.msg));
			}
			return false;
		}

		public Bulletin TryGetBulletinByID(uint id)
		{
			Bulletin value;
			_allBulletinDict.TryGetValue(id, out value);
			return value;
		}

		public void SetBulletinsOldByShowType(uint type)
		{
			HashSet<uint> oldBulletinIDSet = Singleton<MiHoYoGameData>.Instance.LocalData.OldBulletinIDSet;
			switch (type)
			{
			case 0u:
				foreach (Bulletin eventBulletin in EventBulletinList)
				{
					if (!oldBulletinIDSet.Contains(eventBulletin.id))
					{
						oldBulletinIDSet.Add(eventBulletin.id);
					}
				}
				break;
			case 1u:
				foreach (Bulletin systemBulletin in SystemBulletinList)
				{
					if (!oldBulletinIDSet.Contains(systemBulletin.id))
					{
						oldBulletinIDSet.Add(systemBulletin.id);
					}
				}
				break;
			}
			Singleton<MiHoYoGameData>.Instance.Save();
		}

		public bool HasNewBulletinsByType(uint type)
		{
			HashSet<uint> cacheSet = Singleton<MiHoYoGameData>.Instance.LocalData.OldBulletinIDSet;
			switch (type)
			{
			case 0u:
				return EventBulletinList.Exists((Bulletin x) => !cacheSet.Contains(x.id));
			case 1u:
				return SystemBulletinList.Exists((Bulletin x) => !cacheSet.Contains(x.id));
			default:
				return false;
			}
		}

		public bool HasNewBulletins()
		{
			return HasNewBulletinsByType(0u) || HasNewBulletinsByType(1u);
		}
	}
}
