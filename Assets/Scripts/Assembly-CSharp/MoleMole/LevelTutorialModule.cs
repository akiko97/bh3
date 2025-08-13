using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class LevelTutorialModule : BaseModule
	{
		public static int BASE_LEVEL_TUTORIAL_ID = 10000;

		private List<int> _finishTutorialList;

		public LevelTutorialModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			_finishTutorialList = new List<int>();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 128:
				return OnGetFinishGuideDataRsp(pkt.getData<GetFinishGuideDataRsp>());
			case 130:
				return OnFinishGuideReportRsp(pkt.getData<FinishGuideReportRsp>());
			default:
				return false;
			}
		}

		private bool OnGetFinishGuideDataRsp(GetFinishGuideDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				foreach (uint item in rsp.guide_id_list)
				{
					UpdateFinishTutorialID((int)item);
				}
			}
			return false;
		}

		private bool OnFinishGuideReportRsp(FinishGuideReportRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				List<uint> guide_id_list = rsp.guide_id_list;
				foreach (uint item in guide_id_list)
				{
					UpdateFinishTutorialID((int)item);
				}
			}
			return false;
		}

		private bool IsLevelTutorialID(int tutorialID)
		{
			return tutorialID > BASE_LEVEL_TUTORIAL_ID;
		}

		private void UpdateFinishTutorialID(int tutorialID)
		{
			if (!_finishTutorialList.Contains(tutorialID) && IsLevelTutorialID(tutorialID))
			{
				_finishTutorialList.Add(tutorialID);
			}
		}

		public List<int> GetUnFinishedTutorialIDList(int levelID)
		{
			List<int> list = new List<int>();
			if (levelID != 0)
			{
				List<LevelTutorialMetaData> list2 = LevelTutorialMetaDataReader.GetItemList().FindAll((LevelTutorialMetaData x) => !_finishTutorialList.Contains(x.tutorialId));
				foreach (LevelTutorialMetaData item in list2)
				{
					if (!list.Contains(item.tutorialId) && item.levelId == levelID)
					{
						list.Add(item.tutorialId);
					}
				}
			}
			return list;
		}

		public void MarkTutorialIDFinish(int tutorialID)
		{
			Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint)tutorialID, true);
			UpdateFinishTutorialID(tutorialID);
		}

		public bool IsTutorialIDFinish(int tutorialID)
		{
			return _finishTutorialList.Contains(tutorialID);
		}
	}
}
