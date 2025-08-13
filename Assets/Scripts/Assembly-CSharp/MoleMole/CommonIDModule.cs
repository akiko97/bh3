using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class CommonIDModule : BaseModule
	{
		public static int BASE_COMMON_ID = 50000;

		public static int APP_STORE_COMMENT_ID_1 = 5001;

		public static int APP_STORE_COMMENT_ID_2 = 5002;

		private List<int> _finishCommonIDList;

		public CommonIDModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			_finishCommonIDList = new List<int>();
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
					UpdateFinishCommonID((int)item);
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
					UpdateFinishCommonID((int)item);
				}
			}
			return false;
		}

		private void UpdateFinishCommonID(int commonID)
		{
			if (!_finishCommonIDList.Contains(commonID))
			{
				_finishCommonIDList.Add(commonID);
			}
		}

		public bool IsCommonFinished(int commonID)
		{
			return _finishCommonIDList.Contains(commonID);
		}

		public void MarkCommonIDFinish(int commonID)
		{
			if (!_finishCommonIDList.Contains(commonID))
			{
				Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint)commonID, true);
				UpdateFinishCommonID(commonID);
			}
		}
	}
}
