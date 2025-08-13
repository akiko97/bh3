using System;
using System.Collections.Generic;
using UnityEngine;
using proto;
using Avatar = proto.Avatar;

namespace MoleMole
{
	public class GalTouchModule : BaseModule
	{
		public delegate void GalTouchInfoChangedHandler(int oldGoodFeel, int oldHeartLevel, int newGoodFeel, int newHeartLevel, GoodFeelLimitType limitType);

		private List<GalTouchInfoItem> _galTouchInfoItems;

		private GalTouchInfoItem _currentGalTouchInfo;

		private int _maxDialyAddGoodFeel;

		private bool _waitingForResponse;

		public event Action<int, int> CurrentAvatarChanged;

		public event GalTouchInfoChangedHandler GalTouchInfoChanged;

		public event Action<int, int> GalAddBuff;

		public GalTouchModule()
		{
			_galTouchInfoItems = new List<GalTouchInfoItem>();
			Singleton<NotifyManager>.Instance.RegisterModule(this);
		}

		public override bool OnPacket(NetPacketV1 packet)
		{
			switch (packet.getCmdId())
			{
			case 25:
				return OnGetAvatarDataRsp(packet.getData<GetAvatarDataRsp>());
			case 111:
				return OnGetConfigDataRsp(packet.getData<GetConfigRsp>());
			case 155:
				return OnAddGoodfeelRsp(packet.getData<AddGoodfeelRsp>());
			default:
				return false;
			}
		}

		public void ChangeAvatar(int id)
		{
			if (_galTouchInfoItems == null)
			{
				Debug.LogError("Gal Touch Module Not Setup");
			}
			else
			{
				if (_currentGalTouchInfo != null && _currentGalTouchInfo.avatarId == id)
				{
					return;
				}
				GalTouchInfoItem galTouchInfoItem = null;
				int i = 0;
				for (int count = _galTouchInfoItems.Count; i < count; i++)
				{
					if (_galTouchInfoItems[i].avatarId == id)
					{
						galTouchInfoItem = _galTouchInfoItems[i];
						break;
					}
				}
				if (galTouchInfoItem == null)
				{
					Debug.LogError("Invalid Avatar Id");
					return;
				}
				if (this.CurrentAvatarChanged != null)
				{
					this.CurrentAvatarChanged((_currentGalTouchInfo != null) ? _currentGalTouchInfo.avatarId : (-1), galTouchInfoItem.avatarId);
				}
				_currentGalTouchInfo = galTouchInfoItem;
				Singleton<MiHoYoGameData>.Instance.LocalData.LastGalAvatarId = _currentGalTouchInfo.avatarId;
				Singleton<MiHoYoGameData>.Instance.Save();
			}
		}

		public void IncreaseTouchGoodFeel(int amount)
		{
			if (_currentGalTouchInfo == null || _waitingForResponse)
			{
				return;
			}
			GoodFeelLimitType limitType = GoodFeelLimitType.None;
			bool flag = amount <= 0;
			amount = Mathf.Clamp(amount, -_currentGalTouchInfo.touchGoodFeel, GetTodayRemainGoodFeel());
			if (_currentGalTouchInfo.heartLevel > 4)
			{
				if (this.GalTouchInfoChanged != null)
				{
					this.GalTouchInfoChanged(_currentGalTouchInfo.touchGoodFeel + _currentGalTouchInfo.battleGoodFeel, _currentGalTouchInfo.heartLevel, _currentGalTouchInfo.touchGoodFeel + _currentGalTouchInfo.battleGoodFeel, _currentGalTouchInfo.heartLevel, GoodFeelLimitType.ReachMax);
				}
				return;
			}
			int num = GalTouchData.QueryLevelUpFeelNeedTouch(_currentGalTouchInfo.heartLevel);
			int num2 = GalTouchData.QueryLevelUpFeelNeedBattle(_currentGalTouchInfo.heartLevel);
			if (_currentGalTouchInfo.battleGoodFeel >= num2 && !IsLimitedByMission(_currentGalTouchInfo))
			{
				int total = 0;
				int feel = 0;
				int level = 1;
				HeartLevelAndGoodFeelToTotal(_currentGalTouchInfo.touchGoodFeel + _currentGalTouchInfo.battleGoodFeel, _currentGalTouchInfo.heartLevel, out total);
				int total2 = total + amount;
				TotalToHeartLevelAndGoodFeel(total2, out feel, out level);
				if (_currentGalTouchInfo.heartLevel != level)
				{
					total2 = Mathf.Min(total2, GetLevelTotalTouchLimit(_currentGalTouchInfo.heartLevel + 1));
					TotalToHeartLevelAndGoodFeel(total2, out feel, out level);
				}
				HeartLevelAndGoodFeelToTotal(feel, level, out total2);
				_currentGalTouchInfo.todayAddedFeel += total2 - total;
				if (amount == 0 && GetTodayRemainGoodFeel() == 0)
				{
					limitType = GoodFeelLimitType.DialyGoodFeel;
				}
				if (this.GalTouchInfoChanged != null)
				{
					this.GalTouchInfoChanged(_currentGalTouchInfo.touchGoodFeel + _currentGalTouchInfo.battleGoodFeel, _currentGalTouchInfo.heartLevel, feel, level, limitType);
				}
				if (level != _currentGalTouchInfo.heartLevel)
				{
					_currentGalTouchInfo.battleGoodFeel = 0;
				}
				_currentGalTouchInfo.touchGoodFeel = feel - _currentGalTouchInfo.battleGoodFeel;
				_currentGalTouchInfo.heartLevel = level;
				Singleton<NetworkManager>.Instance.RequestGalAddGoodFeel(_currentGalTouchInfo.avatarId, total2 - total, 1u);
				_waitingForResponse = true;
				return;
			}
			int num3 = Mathf.Clamp(_currentGalTouchInfo.touchGoodFeel + amount, 0, num - 1);
			amount = num3 - _currentGalTouchInfo.touchGoodFeel;
			if (amount == 0 && !flag)
			{
				if (GetTodayRemainGoodFeel() == 0)
				{
					limitType = GoodFeelLimitType.DialyGoodFeel;
				}
				else if (_currentGalTouchInfo.battleGoodFeel < num2)
				{
					limitType = GoodFeelLimitType.Battle;
				}
				else if (IsLimitedByMission(_currentGalTouchInfo))
				{
					limitType = GoodFeelLimitType.Mission;
				}
			}
			else
			{
				limitType = GoodFeelLimitType.None;
			}
			if (this.GalTouchInfoChanged != null)
			{
				this.GalTouchInfoChanged(_currentGalTouchInfo.touchGoodFeel + _currentGalTouchInfo.battleGoodFeel, _currentGalTouchInfo.heartLevel, num3 + _currentGalTouchInfo.battleGoodFeel, _currentGalTouchInfo.heartLevel, limitType);
			}
			Singleton<NetworkManager>.Instance.RequestGalAddGoodFeel(_currentGalTouchInfo.avatarId, amount, 1u);
			_currentGalTouchInfo.todayAddedFeel += amount;
			_waitingForResponse = true;
			_currentGalTouchInfo.touchGoodFeel = num3;
		}

		public void IncreaseBattleGoodFeel(int avatarId, int amount)
		{
			GalTouchInfoItem galTouchInfoItem = null;
			int i = 0;
			for (int count = _galTouchInfoItems.Count; i < count; i++)
			{
				if (_galTouchInfoItems[i].avatarId == avatarId)
				{
					galTouchInfoItem = _galTouchInfoItems[i];
					break;
				}
			}
			if (galTouchInfoItem != null)
			{
				amount = Mathf.Clamp(amount, -galTouchInfoItem.battleGoodFeel, GetTodayRemainGoodFeel());
				int b = GalTouchData.QueryLevelUpFeelNeedBattle(galTouchInfoItem.heartLevel);
				int num = Mathf.Min(galTouchInfoItem.battleGoodFeel + amount, b);
				amount = num - galTouchInfoItem.battleGoodFeel;
				Singleton<NetworkManager>.Instance.RequestGalAddGoodFeel(galTouchInfoItem.avatarId, amount, 2u);
				galTouchInfoItem.todayAddedFeel += amount;
				galTouchInfoItem.battleGoodFeel = num;
			}
		}

		public void AddBuff(int avatarId, int buffId)
		{
			int i = 0;
			for (int count = _galTouchInfoItems.Count; i < count; i++)
			{
				if (_galTouchInfoItems[i].avatarId == avatarId)
				{
					_galTouchInfoItems[i].buffId = buffId;
					_galTouchInfoItems[i].buffRestTime = 5;
					if (this.GalAddBuff != null)
					{
						this.GalAddBuff(avatarId, buffId);
					}
				}
			}
		}

		public int GetAvatarGalTouchBuffId(int avatarId)
		{
			int i = 0;
			for (int count = _galTouchInfoItems.Count; i < count; i++)
			{
				if (_galTouchInfoItems[i].avatarId == avatarId)
				{
					return _galTouchInfoItems[i].buffId;
				}
			}
			return 0;
		}

		public int GetAvatarGalTouchBuffRestTime(int avatarId)
		{
			int i = 0;
			for (int count = _galTouchInfoItems.Count; i < count; i++)
			{
				if (_galTouchInfoItems[i].avatarId == avatarId)
				{
					return _galTouchInfoItems[i].buffRestTime;
				}
			}
			return 0;
		}

		public int UseBuff(int avatarId)
		{
			int result = 0;
			int i = 0;
			for (int count = _galTouchInfoItems.Count; i < count; i++)
			{
				if (_galTouchInfoItems[i].avatarId != avatarId)
				{
					continue;
				}
				if (_galTouchInfoItems[i].buffId > 0 && _galTouchInfoItems[i].buffRestTime > 0)
				{
					result = _galTouchInfoItems[i].buffId;
					_galTouchInfoItems[i].buffRestTime--;
					if (_galTouchInfoItems[i].buffRestTime <= 0)
					{
						_galTouchInfoItems[i].buffId = 0;
					}
				}
				break;
			}
			return result;
		}

		public int GetReadyToTouchAvatarID()
		{
			return Singleton<MiHoYoGameData>.Instance.LocalData.LastGalAvatarId;
		}

		public int GetCurrentTouchAvatarID()
		{
			return (_currentGalTouchInfo != null) ? _currentGalTouchInfo.avatarId : 0;
		}

		public int GetCharacterHeartLevel(int id)
		{
			int i = 0;
			for (int count = _galTouchInfoItems.Count; i < count; i++)
			{
				GalTouchInfoItem galTouchInfoItem = _galTouchInfoItems[i];
				if (galTouchInfoItem.avatarId == id)
				{
					return galTouchInfoItem.heartLevel;
				}
			}
			return 0;
		}

		public int GetCharacterHeartLevel()
		{
			if (_currentGalTouchInfo == null)
			{
				return 0;
			}
			return _currentGalTouchInfo.heartLevel;
		}

		public int GetCharacterTouchGoodFeel(int id)
		{
			int i = 0;
			for (int count = _galTouchInfoItems.Count; i < count; i++)
			{
				GalTouchInfoItem galTouchInfoItem = _galTouchInfoItems[i];
				if (galTouchInfoItem.avatarId == id)
				{
					return galTouchInfoItem.touchGoodFeel;
				}
			}
			return 0;
		}

		public int GetCharacterTouchGoodFeel()
		{
			if (_currentGalTouchInfo == null)
			{
				return 0;
			}
			return _currentGalTouchInfo.touchGoodFeel;
		}

		public int GetCharacterBattleGoodFeel(int id)
		{
			int i = 0;
			for (int count = _galTouchInfoItems.Count; i < count; i++)
			{
				GalTouchInfoItem galTouchInfoItem = _galTouchInfoItems[i];
				if (galTouchInfoItem.avatarId == id)
				{
					return galTouchInfoItem.battleGoodFeel;
				}
			}
			return 0;
		}

		public int GetCharacterBattleGoodFeel()
		{
			if (_currentGalTouchInfo == null)
			{
				return 0;
			}
			return _currentGalTouchInfo.battleGoodFeel;
		}

		public int GetCharacterTodayAddedFeel(int id)
		{
			int i = 0;
			for (int count = _galTouchInfoItems.Count; i < count; i++)
			{
				GalTouchInfoItem galTouchInfoItem = _galTouchInfoItems[i];
				if (galTouchInfoItem.avatarId == id)
				{
					return galTouchInfoItem.todayAddedFeel;
				}
			}
			return 0;
		}

		public int GetCharacterTodayAddedFeel()
		{
			if (_currentGalTouchInfo == null)
			{
				return 0;
			}
			return _currentGalTouchInfo.todayAddedFeel;
		}

		public void SetHeartLevelAndGoodFeel(int id, int heartLevel, int touchGoodFeel, int battleGoodFeel)
		{
			int i = 0;
			for (int count = _galTouchInfoItems.Count; i < count; i++)
			{
				GalTouchInfoItem galTouchInfoItem = _galTouchInfoItems[i];
				if (galTouchInfoItem.avatarId == id)
				{
					DoSetHeartLevelAndGoodFeel(galTouchInfoItem, heartLevel, touchGoodFeel, battleGoodFeel);
					break;
				}
			}
		}

		public void SetHeartLevelAndGoodFeel(int heartLevel, int touchGoodFeel, int battleGoodFeel)
		{
			DoSetHeartLevelAndGoodFeel(_currentGalTouchInfo, heartLevel, touchGoodFeel, battleGoodFeel);
		}

		public int GetTodayRemainGoodFeel()
		{
			int num = 0;
			int i = 0;
			for (int count = _galTouchInfoItems.Count; i < count; i++)
			{
				num += _galTouchInfoItems[i].todayAddedFeel;
			}
			int num2 = _maxDialyAddGoodFeel - num;
			if (num2 < 0)
			{
				num2 = 0;
			}
			return num2;
		}

		private void DoSetHeartLevelAndGoodFeel(GalTouchInfoItem item, int heartLevel, int touchGoodFeel, int battleGoodFeel)
		{
			if (item != null)
			{
				int num = touchGoodFeel + battleGoodFeel;
				heartLevel = Mathf.Clamp(heartLevel, 0, 5);
				int num2 = GalTouchData.QueryLevelUpFeelNeed(heartLevel);
				if (num2 != 0)
				{
					num = Mathf.Clamp(num, 0, num2 - 1);
				}
				item.heartLevel = heartLevel;
				item.touchGoodFeel = Mathf.Min(touchGoodFeel, num);
				item.battleGoodFeel = Mathf.Max(num - item.touchGoodFeel, 0);
			}
		}

		private bool OnGetAvatarDataRsp(GetAvatarDataRsp rsp)
		{
			List<Avatar> avatar_list = rsp.avatar_list;
			int i = 0;
			for (int count = avatar_list.Count; i < count; i++)
			{
				Avatar val = avatar_list[i];
				if (!val.touch_goodfeelSpecified || !val.stage_goodfeelSpecified)
				{
					continue;
				}
				GalTouchInfoItem galTouchInfoItem = null;
				int j = 0;
				for (int count2 = _galTouchInfoItems.Count; j < count2; j++)
				{
					if (_galTouchInfoItems[j].avatarId == (int)val.avatar_id)
					{
						galTouchInfoItem = _galTouchInfoItems[j];
						break;
					}
				}
				if (galTouchInfoItem == null)
				{
					galTouchInfoItem = new GalTouchInfoItem();
					galTouchInfoItem.avatarId = (int)val.avatar_id;
					_galTouchInfoItems.Add(galTouchInfoItem);
				}
				int feel = 0;
				int level = 1;
				TotalToHeartLevelAndGoodFeel((int)(val.touch_goodfeel + val.stage_goodfeel), out feel, out level);
				int feel2 = 0;
				TotalToHeartLevelAndGoodFeelBattle((int)val.stage_goodfeel, level, out feel2);
				int touchGoodFeel = feel - feel2;
				galTouchInfoItem.heartLevel = level;
				galTouchInfoItem.touchGoodFeel = touchGoodFeel;
				galTouchInfoItem.battleGoodFeel = feel2;
				if (val.today_has_add_goodfeelSpecified)
				{
					galTouchInfoItem.todayAddedFeel = (int)val.today_has_add_goodfeel;
				}
			}
			return false;
		}

		private bool OnGetConfigDataRsp(GetConfigRsp rsp)
		{
			if (rsp.avatar_max_add_goodfeelSpecified)
			{
				_maxDialyAddGoodFeel = (int)rsp.avatar_max_add_goodfeel;
			}
			return false;
		}

		private bool OnAddGoodfeelRsp(AddGoodfeelRsp rsp)
		{
			_waitingForResponse = false;
			return false;
		}

		private void TotalToHeartLevelAndGoodFeel(int total, out int feel, out int level)
		{
			total = ((total >= 0) ? total : 0);
			int num = 1;
			while (true)
			{
				if (num == 5)
				{
					total = GalTouchData.QueryLevelUpFeelNeed(4);
					break;
				}
				int num2 = GalTouchData.QueryLevelUpFeelNeed(num);
				if (total >= num2)
				{
					total -= num2;
					num++;
					continue;
				}
				break;
			}
			feel = total;
			level = num;
		}

		private void TotalToHeartLevelAndGoodFeelBattle(int total, int level, out int feel)
		{
			total = ((total >= 0) ? total : 0);
			for (int i = 1; i < level; i++)
			{
				total -= GalTouchData.QueryLevelUpFeelNeedBattle(i);
			}
			feel = ((total >= 0) ? total : 0);
		}

		private void HeartLevelAndGoodFeelToTotal(int feel, int level, out int total)
		{
			int num = 0;
			level = Mathf.Clamp(level, 1, 5);
			for (int i = 2; i <= level; i++)
			{
				num += GalTouchData.QueryLevelUpFeelNeed(i - 1);
			}
			if (level != 5)
			{
				num += feel;
			}
			total = num;
		}

		private void HeartLevelAndGoodFeelToTotalTouch(int feel, int level, out int total)
		{
			int num = 0;
			level = Mathf.Clamp(level, 1, 5);
			for (int i = 2; i <= level; i++)
			{
				num += GalTouchData.QueryLevelUpFeelNeedTouch(i - 1);
			}
			if (level != 5)
			{
				num += feel;
			}
			total = num;
		}

		private void HeartLevelAndGoodFeelToTotalBattle(int feel, int level, out int total)
		{
			int num = 0;
			level = Mathf.Clamp(level, 1, 5);
			for (int i = 2; i <= level; i++)
			{
				num += GalTouchData.QueryLevelUpFeelNeedBattle(i - 1);
			}
			if (level != 5)
			{
				num += feel;
			}
			total = num;
		}

		private int GetLevelTotalTouchLimit(int level)
		{
			int num = 0;
			level = Mathf.Clamp(level, 1, 5);
			for (int i = 1; i < level; i++)
			{
				num += GalTouchData.QueryLevelUpFeelNeed(i);
			}
			if (level != 5)
			{
				num += GalTouchData.QueryLevelUpFeelNeedTouch(level);
			}
			return num;
		}

		private bool IsLimitedByMission(GalTouchInfoItem item)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Invalid comparison between Unknown and I4
			TouchMissionItem touchMissionItem = GalTouchData.GetTouchMissionItem(item.avatarId, item.heartLevel);
			if (touchMissionItem == null)
			{
				return false;
			}
			MissionDataItem missionDataItem = Singleton<MissionModule>.Instance.GetMissionDataItem(touchMissionItem.missionId);
			if (missionDataItem == null)
			{
				return true;
			}
			return (int)missionDataItem.status == 2;
		}

		public bool IsCurrentAvatarLimitedByMission()
		{
			return IsLimitedByMission(_currentGalTouchInfo);
		}
	}
}
