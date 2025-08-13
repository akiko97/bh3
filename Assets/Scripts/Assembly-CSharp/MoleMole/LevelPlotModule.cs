using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class LevelPlotModule : BaseModule
	{
		public static int BASE_LEVEL_PLOT_ID = 20000;

		private List<int> _finishPlotList;

		public LevelPlotModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			_finishPlotList = new List<int>();
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
					UpdateFinishPlotID((int)item);
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
					UpdateFinishPlotID((int)item);
				}
			}
			return false;
		}

		private bool IsLevelPlotID(int tutorialID)
		{
			return tutorialID > BASE_LEVEL_PLOT_ID;
		}

		private void UpdateFinishPlotID(int tutorialID)
		{
			if (!_finishPlotList.Contains(tutorialID) && IsLevelPlotID(tutorialID))
			{
				_finishPlotList.Add(tutorialID);
			}
		}

		public bool IsPlotFinished(int plotID)
		{
			return _finishPlotList.Contains(plotID);
		}

		public List<int> GetUnFinishedPlotIDList(int levelID)
		{
			List<int> list = new List<int>();
			if (levelID != 0)
			{
				List<PlotMetaData> list2 = PlotMetaDataReader.GetItemList().FindAll((PlotMetaData x) => !_finishPlotList.Contains(x.plotID));
				foreach (PlotMetaData item in list2)
				{
					if (!list.Contains(item.plotID) && item.levelID == levelID)
					{
						list.Add(item.plotID);
					}
				}
			}
			return list;
		}

		public void MarkPlotIDFinish(int tutorialID)
		{
			Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint)tutorialID, true);
			UpdateFinishPlotID(tutorialID);
		}
	}
}
