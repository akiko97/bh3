using System.Collections.Generic;
using System.Linq;
using proto;

namespace MoleMole
{
	public class MissionModule : BaseModule
	{
		private Dictionary<int, MissionDataItem> _missionDict;

		private Dictionary<int, int> monsterKilledCount;

		private Dictionary<uint, int> _uniqueMonsterKilledCount;

		private int _enemyKilledCount;

		private Dictionary<string, int> _monsterWithCategoryKilledCount;

		private Dictionary<uint, int> _monsterKilledByAnimEventIDCount;

		private Dictionary<string, int> _monsterKilledByAttackCategoryTagCount;

		private Dictionary<uint, int> _triggerAbilityActionCount;

		public bool missionDataReceived;

		public MissionModule()
		{
			Singleton<NotifyManager>.Instance.RegisterModule(this);
			_missionDict = new Dictionary<int, MissionDataItem>();
			monsterKilledCount = new Dictionary<int, int>();
			_uniqueMonsterKilledCount = new Dictionary<uint, int>();
			_monsterWithCategoryKilledCount = new Dictionary<string, int>();
			_monsterKilledByAnimEventIDCount = new Dictionary<uint, int>();
			_monsterKilledByAttackCategoryTagCount = new Dictionary<string, int>();
			_triggerAbilityActionCount = new Dictionary<uint, int>();
			missionDataReceived = false;
		}

		public Dictionary<int, MissionDataItem> GetMissionDict()
		{
			return _missionDict;
		}

		public MissionDataItem GetMissionDataItem(int missionID)
		{
			MissionDataItem value;
			_missionDict.TryGetValue(missionID, out value);
			return value;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 113:
				return OnGetMissionDataRsp(pkt.getData<GetMissionDataRsp>());
			case 115:
				return OnGetMissionRewardRsp(pkt.getData<GetMissionRewardRsp>());
			case 116:
				return OnDelMissionNotify(pkt.getData<DelMissionNotify>());
			case 118:
				return OnUpdateMissionProgressRsp(pkt.getData<UpdateMissionProgressRsp>());
			default:
				return false;
			}
		}

		public void TryToUpdateKillMonsterMission(int monsterId)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			foreach (MissionDataItem value in _missionDict.Values)
			{
				if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 6 || value.metaData.finishParaInt != monsterId)
				{
					continue;
				}
				if (monsterKilledCount.ContainsKey(monsterId))
				{
					Dictionary<int, int> dictionary2;
					Dictionary<int, int> dictionary = (dictionary2 = monsterKilledCount);
					int key2;
					int key = (key2 = monsterId);
					key2 = dictionary2[key2];
					dictionary[key] = key2 + 1;
				}
				else
				{
					monsterKilledCount[monsterId] = 1;
				}
				if (value.progress + monsterKilledCount[monsterId] >= value.metaData.totalProgress)
				{
					Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)6, (uint)monsterId, string.Empty, (uint)monsterKilledCount[monsterId]);
					monsterKilledCount.Remove(monsterId);
				}
				break;
			}
		}

		public void TryToUpdateKillUniqueMonsterMission(uint uniqueMonsterId)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			foreach (MissionDataItem value in _missionDict.Values)
			{
				if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 7 || value.metaData.finishParaInt != (int)uniqueMonsterId)
				{
					continue;
				}
				if (_uniqueMonsterKilledCount.ContainsKey(uniqueMonsterId))
				{
					Dictionary<uint, int> uniqueMonsterKilledCount;
					Dictionary<uint, int> dictionary = (uniqueMonsterKilledCount = _uniqueMonsterKilledCount);
					uint key2;
					uint key = (key2 = uniqueMonsterId);
					int num = uniqueMonsterKilledCount[key2];
					dictionary[key] = num + 1;
				}
				else
				{
					_uniqueMonsterKilledCount[uniqueMonsterId] = 1;
				}
				if (value.progress + _uniqueMonsterKilledCount[uniqueMonsterId] >= value.metaData.totalProgress)
				{
					Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)7, uniqueMonsterId, string.Empty, (uint)_uniqueMonsterKilledCount[uniqueMonsterId]);
					_uniqueMonsterKilledCount.Remove(uniqueMonsterId);
				}
				break;
			}
		}

		public void TryToUpdateKillAnyEnemy()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			foreach (MissionDataItem value in _missionDict.Values)
			{
				if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 8)
				{
					continue;
				}
				_enemyKilledCount++;
				if (value.progress + _enemyKilledCount >= value.metaData.totalProgress)
				{
					Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)8, 0u, string.Empty, (uint)_enemyKilledCount);
					_enemyKilledCount = 0;
				}
				break;
			}
		}

		public void TryToUpdateKillMonsterWithCategoryName(string categoryName)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			foreach (MissionDataItem value in _missionDict.Values)
			{
				if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 9 || !(value.metaData.finishParaStr == categoryName))
				{
					continue;
				}
				if (_monsterWithCategoryKilledCount.ContainsKey(categoryName))
				{
					Dictionary<string, int> monsterWithCategoryKilledCount;
					Dictionary<string, int> dictionary = (monsterWithCategoryKilledCount = _monsterWithCategoryKilledCount);
					string key2;
					string key = (key2 = categoryName);
					int num = monsterWithCategoryKilledCount[key2];
					dictionary[key] = num + 1;
				}
				else
				{
					_monsterWithCategoryKilledCount[categoryName] = 1;
				}
				if (value.progress + _monsterWithCategoryKilledCount[categoryName] >= value.metaData.totalProgress)
				{
					Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)9, 0u, categoryName, (uint)_monsterWithCategoryKilledCount[categoryName]);
					_monsterWithCategoryKilledCount.Remove(categoryName);
				}
				break;
			}
		}

		public void TryToUpdateTriggerAbilityAction(uint finishParaInt)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			foreach (MissionDataItem value in _missionDict.Values)
			{
				if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 10 || value.metaData.finishParaInt != (int)finishParaInt)
				{
					continue;
				}
				if (_triggerAbilityActionCount.ContainsKey(finishParaInt))
				{
					Dictionary<uint, int> triggerAbilityActionCount;
					Dictionary<uint, int> dictionary = (triggerAbilityActionCount = _triggerAbilityActionCount);
					uint key2;
					uint key = (key2 = finishParaInt);
					int num = triggerAbilityActionCount[key2];
					dictionary[key] = num + 1;
				}
				else
				{
					_triggerAbilityActionCount[finishParaInt] = 1;
				}
				if (value.progress + _triggerAbilityActionCount[finishParaInt] >= value.metaData.totalProgress)
				{
					Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)10, finishParaInt, string.Empty, (uint)_triggerAbilityActionCount[finishParaInt]);
					_triggerAbilityActionCount.Remove(finishParaInt);
				}
				break;
			}
		}

		public void TryToUpdateKillByAnimEventID(uint finishParaInt)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			foreach (MissionDataItem value in _missionDict.Values)
			{
				if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 11 || value.metaData.finishParaInt != finishParaInt)
				{
					continue;
				}
				if (_monsterKilledByAnimEventIDCount.ContainsKey(finishParaInt))
				{
					Dictionary<uint, int> monsterKilledByAnimEventIDCount;
					Dictionary<uint, int> dictionary = (monsterKilledByAnimEventIDCount = _monsterKilledByAnimEventIDCount);
					uint key2;
					uint key = (key2 = finishParaInt);
					int num = monsterKilledByAnimEventIDCount[key2];
					dictionary[key] = num + 1;
				}
				else
				{
					_monsterKilledByAnimEventIDCount[finishParaInt] = 1;
				}
				if (value.progress + _monsterKilledByAnimEventIDCount[finishParaInt] >= value.metaData.totalProgress)
				{
					Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)11, finishParaInt, string.Empty, (uint)_monsterKilledByAnimEventIDCount[finishParaInt]);
					_monsterKilledByAnimEventIDCount.Remove(finishParaInt);
				}
				break;
			}
		}

		public void TryToUpdateKillByAttackCategoryTag(AttackResult.AttackCategoryTag[] categoryTags)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			foreach (MissionDataItem value in _missionDict.Values)
			{
				if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 23)
				{
					continue;
				}
				foreach (AttackResult.AttackCategoryTag attackCategoryTag in categoryTags)
				{
					string text = attackCategoryTag.ToString();
					if (value.metaData.finishParaStr == text)
					{
						if (_monsterKilledByAttackCategoryTagCount.ContainsKey(text))
						{
							Dictionary<string, int> monsterKilledByAttackCategoryTagCount;
							Dictionary<string, int> dictionary = (monsterKilledByAttackCategoryTagCount = _monsterKilledByAttackCategoryTagCount);
							string key2;
							string key = (key2 = text);
							int num = monsterKilledByAttackCategoryTagCount[key2];
							dictionary[key] = num + 1;
						}
						else
						{
							_monsterKilledByAttackCategoryTagCount[text] = 1;
						}
						if (value.progress + _monsterKilledByAttackCategoryTagCount[text] >= value.metaData.totalProgress)
						{
							Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)23, 0u, text, (uint)_monsterKilledByAttackCategoryTagCount[text]);
							_monsterKilledByAttackCategoryTagCount.Remove(text);
						}
						break;
					}
				}
			}
		}

		public void FlushMissionDataToServer()
		{
			FlushMonsterMissionProgressToServer();
			FlushUniqueMonsterMissionProgressToServer();
			FlushKillAnyEnemyProgressToServer();
			FlushKillMonsterWithCategoryProgressToServer();
			FlushTriggerAbilityActionProgressToServer();
			FlushKillWithAnimEventIDProgressToServer();
			FlushKillByAttackCategoryTagProgressToServer();
		}

		private void FlushMonsterMissionProgressToServer()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			foreach (int key in monsterKilledCount.Keys)
			{
				foreach (MissionDataItem value in _missionDict.Values)
				{
					if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 6 || value.metaData.finishParaInt != key)
					{
						continue;
					}
					Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)6, (uint)key, string.Empty, (uint)monsterKilledCount[key]);
					break;
				}
			}
			monsterKilledCount.Clear();
		}

		private void FlushUniqueMonsterMissionProgressToServer()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			foreach (uint key in _uniqueMonsterKilledCount.Keys)
			{
				foreach (MissionDataItem value in _missionDict.Values)
				{
					if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 7 || value.metaData.finishParaInt != (int)key)
					{
						continue;
					}
					Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)7, key, string.Empty, (uint)_uniqueMonsterKilledCount[key]);
					break;
				}
			}
			_uniqueMonsterKilledCount.Clear();
		}

		private void FlushKillAnyEnemyProgressToServer()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			foreach (MissionDataItem value in _missionDict.Values)
			{
				if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 8)
				{
					continue;
				}
				Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)8, 0u, string.Empty, (uint)_enemyKilledCount);
				break;
			}
			_enemyKilledCount = 0;
		}

		private void FlushKillMonsterWithCategoryProgressToServer()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			foreach (string key in _monsterWithCategoryKilledCount.Keys)
			{
				foreach (MissionDataItem value in _missionDict.Values)
				{
					if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 9 || !(value.metaData.finishParaStr == key))
					{
						continue;
					}
					Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)9, 0u, key, (uint)_monsterWithCategoryKilledCount[key]);
					break;
				}
			}
			_monsterWithCategoryKilledCount.Clear();
		}

		private void FlushTriggerAbilityActionProgressToServer()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			foreach (uint key in _triggerAbilityActionCount.Keys)
			{
				foreach (MissionDataItem value in _missionDict.Values)
				{
					if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 10 || value.metaData.finishParaInt != (int)key)
					{
						continue;
					}
					Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)10, key, string.Empty, (uint)_triggerAbilityActionCount[key]);
					break;
				}
			}
			_triggerAbilityActionCount.Clear();
		}

		private void FlushKillWithAnimEventIDProgressToServer()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			foreach (uint key in _monsterKilledByAnimEventIDCount.Keys)
			{
				foreach (MissionDataItem value in _missionDict.Values)
				{
					if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 11 || value.metaData.finishParaInt != (int)key)
					{
						continue;
					}
					Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)11, key, string.Empty, (uint)_monsterKilledByAnimEventIDCount[key]);
					break;
				}
			}
			_monsterKilledByAnimEventIDCount.Clear();
		}

		private void FlushKillByAttackCategoryTagProgressToServer()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			foreach (string key in _monsterKilledByAttackCategoryTagCount.Keys)
			{
				foreach (MissionDataItem value in _missionDict.Values)
				{
					if (value.metaData == null || (int)value.status != 2 || value.metaData.finishWay != 23 || !(value.metaData.finishParaStr == key))
					{
						continue;
					}
					Singleton<NetworkManager>.Instance.RequestUpdateMissionProgress((MissionFinishWay)23, 0u, key, (uint)_monsterKilledByAttackCategoryTagCount[key]);
					break;
				}
			}
			_monsterKilledByAttackCategoryTagCount.Clear();
		}

		private bool OnGetMissionDataRsp(GetMissionDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Invalid comparison between Unknown and I4
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Invalid comparison between Unknown and I4
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Invalid comparison between Unknown and I4
			if ((int)rsp.retcode == 0)
			{
				foreach (Mission item in rsp.mission_list)
				{
					if (_missionDict.ContainsKey((int)item.mission_id))
					{
						MissionDataItem missionDataItem = _missionDict[(int)item.mission_id];
						if (!missionDataItem.IsMissionEqual(item))
						{
							missionDataItem.UpdateFromMission(item);
							Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MissionUpdated, item.mission_id));
						}
					}
					else
					{
						MissionDataItem value = new MissionDataItem(item);
						_missionDict[(int)item.mission_id] = value;
						Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MissionUpdated, item.mission_id));
					}
					if (((int)item.status == 2 || (int)item.status == 3 || (int)item.status == 5) && Singleton<TutorialModule>.Instance != null)
					{
						Singleton<TutorialModule>.Instance.TryToDoTutoialWhenUpdateMissionStatus(item);
					}
				}
				missionDataReceived = true;
			}
			return false;
		}

		private bool OnGetMissionRewardRsp(GetMissionRewardRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MissionRewardGot, rsp));
				Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
			}
			return false;
		}

		private bool OnDelMissionNotify(DelMissionNotify rsp)
		{
			_missionDict.Remove((int)rsp.mission_id);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MissionDeleted, rsp.mission_id));
			return false;
		}

		private bool OnUpdateMissionProgressRsp(UpdateMissionProgressRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
			}
			return false;
		}

		public bool NeedNotify()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Invalid comparison between Unknown and I4
			foreach (MissionDataItem value in _missionDict.Values)
			{
				if ((int)value.status == 3)
				{
					LinearMissionData linearMissionDataByKey = LinearMissionDataReader.GetLinearMissionDataByKey(value.id);
					if (linearMissionDataByKey == null || linearMissionDataByKey.IsAchievement == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public List<MissionDataItem> GetAchievements()
		{
			return _missionDict.Values.ToList().FindAll(delegate(MissionDataItem x)
			{
				LinearMissionData linearMissionDataByKey = LinearMissionDataReader.GetLinearMissionDataByKey(x.id);
				if (linearMissionDataByKey == null)
				{
					return false;
				}
				return (linearMissionDataByKey.IsAchievement != 0) ? true : false;
			});
		}
	}
}
