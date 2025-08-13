using System.Collections.Generic;
using proto;

namespace MoleMole
{
	public class CabinTechTreeNode
	{
		public CabinTechTreeMetaData _metaData;

		private OnTechTreeNodeActive _activeHandler;

		public TechTreeNodeStatus _status_before_reset;

		private TechTreeNodeStatus _internal_status;

		private CabinTechTree _tree;

		private List<TechTreeNodeLockInfo> _lockInfoList = new List<TechTreeNodeLockInfo>();

		public TechTreeNodeStatus _status
		{
			get
			{
				return _internal_status;
			}
			set
			{
				if (_status_before_reset == TechTreeNodeStatus.Unlock_Ready_Active && _internal_status != TechTreeNodeStatus.Active && value == TechTreeNodeStatus.Active && _activeHandler != null)
				{
					_activeHandler();
				}
				_internal_status = value;
			}
		}

		public string IconPath
		{
			get
			{
				return _metaData.Icon;
			}
		}

		public CabinTechTreeNode(CabinTechTree tree)
		{
			_tree = tree;
			_status = TechTreeNodeStatus.Lock;
		}

		public void RegisterCallback(OnTechTreeNodeActive activeHandler)
		{
			_activeHandler = activeHandler;
		}

		public void UnRegisterCallback()
		{
			_activeHandler = null;
		}

		public List<TechTreeNodeLockInfo> GetLockInfo()
		{
			_lockInfoList.Clear();
			if (_metaData.UnlockLevel > 0)
			{
				int level = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType)_metaData.Cabin).level;
				if (level < _metaData.UnlockLevel)
				{
					TechTreeNodeLockInfo item = new TechTreeNodeLockInfo
					{
						_lockType = TechTreeNodeLock.CabinLevel,
						_needLevel = _metaData.UnlockLevel
					};
					_lockInfoList.Add(item);
				}
			}
			if (_metaData.UnlockAvatarID > 0)
			{
				AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(_metaData.UnlockAvatarID);
				if (!avatarByID.UnLocked)
				{
					TechTreeNodeLockInfo item2 = new TechTreeNodeLockInfo
					{
						_lockType = TechTreeNodeLock.AvatarUnlock,
						_needLevel = _metaData.UnlockAvatarLevel
					};
					_lockInfoList.Add(item2);
				}
				else if (avatarByID.level < _metaData.UnlockAvatarLevel)
				{
					TechTreeNodeLockInfo item3 = new TechTreeNodeLockInfo
					{
						_lockType = TechTreeNodeLock.AvatarLevel,
						_needLevel = _metaData.UnlockAvatarLevel
					};
					_lockInfoList.Add(item3);
				}
			}
			return _lockInfoList;
		}

		public TechTreeNodeStatus GetStatus()
		{
			if (_status == TechTreeNodeStatus.Active)
			{
				return _status;
			}
			if (_metaData.UnlockLevel > 0)
			{
				int level = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType)_metaData.Cabin).level;
				if (level < _metaData.UnlockLevel)
				{
					return TechTreeNodeStatus.Lock;
				}
			}
			if (_metaData.UnlockAvatarID > 0)
			{
				AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(_metaData.UnlockAvatarID);
				if (!avatarByID.UnLocked || avatarByID.level < _metaData.UnlockAvatarLevel)
				{
					return TechTreeNodeStatus.Lock;
				}
			}
			if (_metaData.X == 0 && _metaData.Y == 0)
			{
				return TechTreeNodeStatus.Unlock_Ready_Active;
			}
			bool flag = false;
			List<CabinTechTreeNode> neibours = _tree.GetNeibours(this);
			foreach (CabinTechTreeNode item in neibours)
			{
				if (item._status == TechTreeNodeStatus.Active)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				return TechTreeNodeStatus.Unlock_Ready_Active;
			}
			return TechTreeNodeStatus.Unlock_Ban_Active;
		}

		public List<CabinTechTreeNode> GetNeibours()
		{
			return _tree.GetNeibours(this);
		}

		public string GetComment()
		{
			if (_status == TechTreeNodeStatus.Unlock_Ready_Active)
			{
				int leftPowerCost = Singleton<IslandModule>.Instance.GetLeftPowerCost();
				if (leftPowerCost < _metaData.PowerCost)
				{
					return string.Format("lack power, left: {0}, meta: {1}", leftPowerCost, _metaData.PowerCost);
				}
			}
			return string.Empty;
		}
	}
}
