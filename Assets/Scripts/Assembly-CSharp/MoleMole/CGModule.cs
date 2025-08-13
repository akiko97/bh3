using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class CGModule : BaseModule
	{
		public static int BASE_CG_ID = 40000;

		private List<int> _finishCGList;

		public CGModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			_finishCGList = new List<int>();
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
					UpdateFinishCGID((int)item);
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
					UpdateFinishCGID((int)item);
				}
			}
			return false;
		}

		private bool IsLevelPlotID(int cgID)
		{
			return cgID > BASE_CG_ID;
		}

		private void UpdateFinishCGID(int cgID)
		{
			if (!_finishCGList.Contains(cgID) && IsLevelPlotID(cgID))
			{
				_finishCGList.Add(cgID);
			}
		}

		public bool IsCGFinished(int plotID)
		{
			return _finishCGList.Contains(plotID);
		}

		public List<int> GetUnFinishedCGIDList(int levelID)
		{
			List<int> list = new List<int>();
			if (levelID != 0)
			{
				List<CgMetaData> list2 = CgMetaDataReader.GetItemList().FindAll((CgMetaData x) => !_finishCGList.Contains(x.CgID));
				foreach (CgMetaData item in list2)
				{
					if (!list.Contains(item.CgID) && item.levelID == levelID)
					{
						list.Add(item.CgID);
					}
				}
			}
			return list;
		}

		public List<int> GetAllCGIDList()
		{
			List<int> list = new List<int>();
			List<CgMetaData> itemList = CgMetaDataReader.GetItemList();
			foreach (CgMetaData item in itemList)
			{
				list.Add(item.CgID);
			}
			return list;
		}

		public List<int> GetFinishedCGIDList()
		{
			List<int> list = new List<int>();
			List<CgMetaData> itemList = CgMetaDataReader.GetItemList();
			List<CgMetaData> list2 = itemList.FindAll((CgMetaData x) => _finishCGList.Contains(x.CgID));
			if (list2 != null)
			{
				foreach (CgMetaData item in list2)
				{
					list.Add(item.CgID);
				}
			}
			return list;
		}

		public List<int> GetUnFinishedCGIDList()
		{
			List<int> list = new List<int>();
			List<CgMetaData> list2 = CgMetaDataReader.GetItemList().FindAll((CgMetaData x) => !_finishCGList.Contains(x.CgID));
			foreach (CgMetaData item in list2)
			{
				list.Add(item.CgID);
			}
			return list;
		}

		public void MarkCGIDFinish(int cgID)
		{
			if (!_finishCGList.Contains(cgID))
			{
				Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint)cgID, true);
				UpdateFinishCGID(cgID);
			}
		}

		public CgDataItem GetCgDataItem(int cgId)
		{
			CgMetaData cgMetaData = CgMetaDataReader.TryGetCgMetaDataByKey(cgId);
			if (cgMetaData != null)
			{
				return new CgDataItem(cgMetaData);
			}
			return null;
		}

		public List<CgDataItem> GetCgDataItemList()
		{
			List<CgDataItem> list = new List<CgDataItem>();
			List<CgMetaData> itemList = CgMetaDataReader.GetItemList();
			foreach (CgMetaData item in itemList)
			{
				list.Add(new CgDataItem(item));
			}
			return list;
		}

		public List<CgDataItem> GetFinishedCgDataItemList()
		{
			List<CgDataItem> list = new List<CgDataItem>();
			List<CgMetaData> itemList = CgMetaDataReader.GetItemList();
			foreach (CgMetaData item in itemList)
			{
				if (item != null && _finishCGList.Contains(item.CgID))
				{
					list.Add(new CgDataItem(item));
				}
			}
			return list;
		}
	}
}
