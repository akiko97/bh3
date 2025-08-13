using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class IslandModule : BaseModule
	{
		private Dictionary<CabinType, CabinDataItemBase> _cabinDict;

		private Dictionary<int, VentureDataItem> _ventureDict;

		private int _ventureRefreshTimes;

		private HashSet<int> _dispatchAvatarIdSet;

		private bool _venture_inprogress_background;

		private DateTime _venture_endtime_background = DateTime.MaxValue;

		public int VentureRefreshTimes
		{
			get
			{
				return _ventureRefreshTimes;
			}
		}

		public IslandModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			InitCabinDict();
			_ventureDict = new Dictionary<int, VentureDataItem>();
			_ventureRefreshTimes = 0;
			_dispatchAvatarIdSet = new HashSet<int>();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 157:
				return OnGetIsLandRsp(pkt.getData<GetIslandRsp>());
			case 165:
				return OnAddCabinTechRsp(pkt.getData<AddCabinTechRsp>());
			case 169:
				return OnGetIslandVentureRsp(pkt.getData<GetIslandVentureRsp>());
			case 170:
				return OnDelIslandVentureNotify(pkt.getData<DelIslandVentureNotify>());
			case 184:
				return OnGetCollectCabinRsp(pkt.getData<GetCollectCabinRsp>());
			default:
				return false;
			}
		}

		private bool OnGetIsLandRsp(GetIslandRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				foreach (Cabin item in rsp.cabin_list)
				{
					CabinType val = (CabinType)item.type;
					if (_cabinDict.ContainsKey(val))
					{
						CabinDataItemBase cabinDataItemBase = _cabinDict[val];
						cabinDataItemBase.level = (int)item.level;
						cabinDataItemBase.extendGrade = (int)item.extend_grade;
						cabinDataItemBase.levelUpEndTime = ((!item.level_up_end_timeSpecified) ? TimeUtil.Now.AddDays(-1.0) : Miscs.GetDateTimeFromTimeStamp(item.level_up_end_time));
						cabinDataItemBase.SetupMateData();
						if (cabinDataItemBase.HasTechTree())
						{
							cabinDataItemBase._techTree.OnReceiveActiveNodes(item.tech_list);
						}
						if (cabinDataItemBase is CabinAvatarEnhanceDataItem)
						{
							(cabinDataItemBase as CabinAvatarEnhanceDataItem).SetAvatarClassType(val);
						}
					}
				}
				Singleton<MiHoYoGameData>.Instance.Save();
				Singleton<NetworkManager>.Instance.RequestGetCollectCabin();
			}
			return false;
		}

		private bool OnAddCabinTechRsp(AddCabinTechRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
			}
			return false;
		}

		private bool OnGetIslandVentureRsp(GetIslandVentureRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				if (rsp.is_all)
				{
					_ventureDict.Clear();
					_dispatchAvatarIdSet.Clear();
				}
				foreach (IslandVenture item in rsp.venture_list)
				{
					int ventureId = (int)item.id;
					if (!_ventureDict.ContainsKey(ventureId))
					{
						VentureDataItem value = new VentureDataItem(ventureId);
						_ventureDict[ventureId] = value;
					}
					_ventureDict[ventureId].SetEndTime(item.end_time);
					_dispatchAvatarIdSet.RemoveWhere((int x) => _ventureDict[ventureId].dispatchAvatarIdList.Contains(x));
					_ventureDict[ventureId].SetDispatchAvatarList(item.avatar_id);
					foreach (uint item2 in item.avatar_id)
					{
						_dispatchAvatarIdSet.Add((int)item2);
					}
				}
				_ventureRefreshTimes = (int)rsp.refresh_times;
			}
			return false;
		}

		private bool OnDelIslandVentureNotify(DelIslandVentureNotify rsp)
		{
			uint id;
			foreach (uint item in rsp.venture_id_list)
			{
				id = item;
				_dispatchAvatarIdSet.RemoveWhere((int x) => _ventureDict[(int)id].dispatchAvatarIdList.Contains(x));
				_ventureDict.Remove((int)id);
			}
			return false;
		}

		private bool OnDispatchIslandVentureRsp(DispatchIslandVentureRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode != 0)
			{
			}
			return false;
		}

		private bool OnGetCollectCabinRsp(GetCollectCabinRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				CabinCollectDataItem cabinCollectDataItem = _cabinDict[(CabinType)3] as CabinCollectDataItem;
				cabinCollectDataItem.currentScoinAmount = (int)rsp.add_scoin;
				cabinCollectDataItem.dropItems = rsp.drop_item_list;
				cabinCollectDataItem.nextScoinUpdateTime = Miscs.GetDateTimeFromTimeStamp(rsp.next_add_time);
				cabinCollectDataItem.canUpdateScoinLate = rsp.next_add_timeSpecified;
			}
			return false;
		}

		private void InitCabinDict()
		{
			_cabinDict = new Dictionary<CabinType, CabinDataItemBase>();
			_cabinDict[(CabinType)1] = CabinEngineDataItem.GetInstance();
			_cabinDict[(CabinType)3] = CabinCollectDataItem.GetInstance();
			_cabinDict[(CabinType)4] = CabinMiscDataItem.GetInstance();
			_cabinDict[(CabinType)5] = CabinVentureDataItem.GetInstance();
			_cabinDict[(CabinType)2] = CabinKianaEnhanceDataItem.GetInstance();
			_cabinDict[(CabinType)6] = CabinMeiEnhanceDataItem.GetInstance();
			_cabinDict[(CabinType)7] = CabinBronyaEnhanceDataItem.GetInstance();
			foreach (CabinDataItemBase value in _cabinDict.Values)
			{
				value.level = 0;
				value.extendGrade = 1;
			}
		}

		public List<CabinDataItemBase> GetUnlockCabinDataList()
		{
			return _cabinDict.Values.Where((CabinDataItemBase x) => x.status == CabinStatus.UnLocked).ToList();
		}

		public CabinDataItemBase GetCabinDataByType(CabinType cabinType)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return _cabinDict[cabinType];
		}

		public CabinAvatarEnhanceDataItem GetAvatarEnhanceCabinByClass(int classID)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Invalid comparison between Unknown and I4
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Invalid comparison between Unknown and I4
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Invalid comparison between Unknown and I4
			CabinAvatarEnhanceDataItem cabinAvatarEnhanceDataItem = _cabinDict[(CabinType)2] as CabinAvatarEnhanceDataItem;
			if ((int)cabinAvatarEnhanceDataItem._classType == classID)
			{
				return cabinAvatarEnhanceDataItem;
			}
			cabinAvatarEnhanceDataItem = _cabinDict[(CabinType)6] as CabinAvatarEnhanceDataItem;
			if ((int)cabinAvatarEnhanceDataItem._classType == classID)
			{
				return cabinAvatarEnhanceDataItem;
			}
			cabinAvatarEnhanceDataItem = _cabinDict[(CabinType)7] as CabinAvatarEnhanceDataItem;
			if ((int)cabinAvatarEnhanceDataItem._classType == classID)
			{
				return cabinAvatarEnhanceDataItem;
			}
			return null;
		}

		public void InitTechTree()
		{
			_cabinDict[(CabinType)3]._techTree.InitMetaData();
			_cabinDict[(CabinType)4]._techTree.InitMetaData();
			_cabinDict[(CabinType)5]._techTree.InitMetaData();
			_cabinDict[(CabinType)2]._techTree.InitMetaData();
			_cabinDict[(CabinType)6]._techTree.InitMetaData();
			_cabinDict[(CabinType)7]._techTree.InitMetaData();
		}

		public int GetFinishLevelUpNowHcoinCost(int timeRemain)
		{
			float num = 0f;
			List<CabinLevelUpTimePriceMetaData> itemList = CabinLevelUpTimePriceMetaDataReader.GetItemList();
			foreach (CabinLevelUpTimePriceMetaData item in itemList)
			{
				if (timeRemain > item.timeMax)
				{
					num += (float)item.price;
					continue;
				}
				int num2 = timeRemain - item.timeMin + 1;
				num += (float)num2 * (float)item.price / (float)(item.timeMax - item.timeMin + 1);
				break;
			}
			return Mathf.CeilToInt(num);
		}

		public int GetMaxPowerCost()
		{
			int level = _cabinDict[(CabinType)1].level;
			return CabinPowerCostMetaDataReader.GetCabinPowerCostMetaDataByKey(level).MaxPowerCost;
		}

		public int GetNextLevelMaxPowerCost()
		{
			int level = _cabinDict[(CabinType)1].level + 1;
			CabinPowerCostMetaData cabinPowerCostMetaDataByKey = CabinPowerCostMetaDataReader.GetCabinPowerCostMetaDataByKey(level);
			if (cabinPowerCostMetaDataByKey != null)
			{
				return CabinPowerCostMetaDataReader.GetCabinPowerCostMetaDataByKey(level).MaxPowerCost;
			}
			return -1;
		}

		public int GetUsedPowerCost()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			foreach (int value in Enum.GetValues(typeof(CabinType)))
			{
				CabinType key = (CabinType)value;
				num += _cabinDict[key].GetUsedPower();
			}
			return num;
		}

		public int GetLeftPowerCost()
		{
			return GetMaxPowerCost() - GetUsedPowerCost();
		}

		public bool HasCabinLevelUpInProgress()
		{
			foreach (KeyValuePair<CabinType, CabinDataItemBase> item in _cabinDict)
			{
				if (item.Value.levelUpEndTime > TimeUtil.Now)
				{
					return true;
				}
			}
			return false;
		}

		public List<CabinDataItemBase> GetCabinList()
		{
			return _cabinDict.Values.ToList();
		}

		public List<VentureDataItem> GetVentureList()
		{
			List<VentureDataItem> list = _ventureDict.Values.ToList();
			list.Sort((VentureDataItem left, VentureDataItem right) => left.StaminaCost - right.StaminaCost);
			return list;
		}

		public int GetVentureInProgressNum()
		{
			int num = 0;
			foreach (KeyValuePair<int, VentureDataItem> item in _ventureDict)
			{
				if (item.Value.status == VentureDataItem.VentureStatus.InProgress || item.Value.status == VentureDataItem.VentureStatus.Done)
				{
					num++;
				}
			}
			return num;
		}

		public int GetVentureDoneNum()
		{
			int num = 0;
			foreach (KeyValuePair<int, VentureDataItem> item in _ventureDict)
			{
				if (item.Value.status == VentureDataItem.VentureStatus.Done)
				{
					num++;
				}
			}
			return num;
		}

		public bool HasCabinNeedToShowLevelUp()
		{
			foreach (KeyValuePair<CabinType, CabinDataItemBase> item in _cabinDict)
			{
				if (item.Value.NeedToShowLevelUpComplete())
				{
					return true;
				}
			}
			return false;
		}

		public void RegisterVentureInProgress()
		{
			_venture_inprogress_background = false;
			_venture_endtime_background = DateTime.MaxValue;
			foreach (KeyValuePair<int, VentureDataItem> item in _ventureDict)
			{
				if (item.Value.status == VentureDataItem.VentureStatus.InProgress && item.Value.endTime < _venture_endtime_background)
				{
					_venture_endtime_background = item.Value.endTime;
					_venture_inprogress_background = true;
				}
			}
		}

		public void UnRegisterVentureInProgress()
		{
			_venture_inprogress_background = false;
			_venture_endtime_background = DateTime.MaxValue;
		}

		public bool RefreshVentureBackground()
		{
			if (_venture_inprogress_background)
			{
				return TimeUtil.Now > _venture_endtime_background;
			}
			return false;
		}

		public bool IsAvatarDispatched(int avatarId)
		{
			return _dispatchAvatarIdSet.Contains(avatarId);
		}

		public int GetMaxFriendAdd()
		{
			return _cabinDict[(CabinType)4]._techTree.GetAbilitySum((CabinTechEffectType)5, 1);
		}

		public int GetSkillPointAdd()
		{
			return _cabinDict[(CabinType)4]._techTree.GetAbilitySum((CabinTechEffectType)11, 1);
		}

		public bool IsDropMaterials()
		{
			return _cabinDict[(CabinType)3]._techTree.AbilityUnLock((CabinTechEffectType)12);
		}

		public int GetDropMaterialPackageNum()
		{
			if (IsDropMaterials())
			{
				int abilitySum = _cabinDict[(CabinType)3]._techTree.GetAbilitySum((CabinTechEffectType)13, 1);
				return abilitySum + 1;
			}
			return 0;
		}

		public void OnPlayerLevelChanged(int newLevel, int oldLevel)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<CabinType, bool> cabinNeedToShowNewUnlockDict = Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowNewUnlockDict;
			foreach (KeyValuePair<CabinType, CabinDataItemBase> item in _cabinDict)
			{
				if (!cabinNeedToShowNewUnlockDict.ContainsKey(item.Key) && item.Value.GetUnlockPlayerLevel() > oldLevel && item.Value.GetUnlockPlayerLevel() <= newLevel)
				{
					cabinNeedToShowNewUnlockDict.Add(item.Key, true);
				}
			}
		}
	}
}
