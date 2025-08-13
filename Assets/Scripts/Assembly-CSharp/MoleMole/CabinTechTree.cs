using System.Collections.Generic;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class CabinTechTree
	{
		private List<CabinTechTreeNode> _itemList = new List<CabinTechTreeNode>();

		private List<CabinTech> _activeNodeList = new List<CabinTech>();

		private List<CabinTechTreeNode> _return_active_node_list = new List<CabinTechTreeNode>();

		private CabinType _ownerType;

		private bool _bInited;

		public CabinTechTree(CabinType type)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			_ownerType = type;
			_bInited = false;
		}

		public void InitMetaData()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Invalid comparison between I4 and Unknown
			if (_bInited)
			{
				return;
			}
			_bInited = true;
			_itemList.Clear();
			foreach (CabinTechTreeMetaData item in CabinTechTreeMetaDataReader.GetItemList())
			{
				if (item.Cabin == (int)_ownerType)
				{
					CabinTechTreeNode cabinTechTreeNode = new CabinTechTreeNode(this);
					cabinTechTreeNode._metaData = item;
					_itemList.Add(cabinTechTreeNode);
				}
			}
			if (_activeNodeList.Count > 0)
			{
				foreach (CabinTech activeNode in _activeNodeList)
				{
					SetNodeActive(activeNode.pos_x, activeNode.pos_y);
				}
				_activeNodeList.Clear();
			}
			RefreshNodes();
		}

		public void OnReceiveActiveNodes(List<CabinTech> list)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			if (!_bInited)
			{
				foreach (CabinTech item in list)
				{
					CabinTech val = new CabinTech();
					val.pos_x = item.pos_x;
					val.pos_y = item.pos_y;
					_activeNodeList.Add(val);
				}
				return;
			}
			ResetNodes();
			foreach (CabinTech item2 in list)
			{
				SetNodeActive(item2.pos_x, item2.pos_y);
			}
			RefreshNodes();
		}

		public void ResetNodes()
		{
			foreach (CabinTechTreeNode item in _itemList)
			{
				item._status_before_reset = item._status;
				item._status = TechTreeNodeStatus.Lock;
			}
		}

		public void RefreshNodes()
		{
			foreach (CabinTechTreeNode item in _itemList)
			{
				item._status = item.GetStatus();
			}
		}

		public void SetNodeActive(int x, int y)
		{
			foreach (CabinTechTreeNode item in _itemList)
			{
				if (item._metaData.X == x && item._metaData.Y == y)
				{
					item._status = TechTreeNodeStatus.Active;
					break;
				}
			}
		}

		public List<CabinTechTreeNode> GetNeibours(CabinTechTreeNode node)
		{
			List<CabinTechTreeNode> list = new List<CabinTechTreeNode>();
			foreach (CabinTechTreeNode item in _itemList)
			{
				if (item._metaData.X == node._metaData.X)
				{
					int num = item._metaData.Y - node._metaData.Y;
					if (num == 1 || num == -1)
					{
						list.Add(item);
					}
				}
				else if (item._metaData.Y == node._metaData.Y)
				{
					int num2 = item._metaData.X - node._metaData.X;
					if (num2 == 1 || num2 == -1)
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		public int GetPowerUsed()
		{
			int num = 0;
			foreach (CabinTechTreeNode item in _itemList)
			{
				if (item._status == TechTreeNodeStatus.Active)
				{
					num += item._metaData.PowerCost;
				}
			}
			return num;
		}

		public int GetResetScoin()
		{
			int num = 0;
			foreach (CabinTechTreeNode item in _itemList)
			{
				if (item._status == TechTreeNodeStatus.Active)
				{
					num += item._metaData.ResetSCoin;
				}
			}
			return num;
		}

		public void Log()
		{
			foreach (CabinTechTreeNode item in _itemList)
			{
			}
		}

		public List<CabinTechTreeNode> GetNodeList()
		{
			return _itemList;
		}

		public CabinTechTreeNode GetNode(int x, int y)
		{
			foreach (CabinTechTreeNode item in _itemList)
			{
				if (item._metaData.X == x && item._metaData.Y == y)
				{
					return item;
				}
			}
			return null;
		}

		public List<CabinTechTreeNode> GetActiveNodeList()
		{
			_return_active_node_list.Clear();
			foreach (CabinTechTreeNode item in _itemList)
			{
				if (item._status == TechTreeNodeStatus.Active)
				{
					_return_active_node_list.Add(item);
				}
			}
			return _return_active_node_list;
		}

		public bool AbilityUnLock(CabinTechEffectType techType)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Invalid comparison between I4 and Unknown
			foreach (CabinTechTreeNode item in _itemList)
			{
				if (item._status == TechTreeNodeStatus.Active && item._metaData.AbilityType == (int)techType)
				{
					return true;
				}
			}
			return false;
		}

		public int GetAbilitySum(CabinTechEffectType techType, int index)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Invalid comparison between I4 and Unknown
			int num = 0;
			foreach (CabinTechTreeNode item in _itemList)
			{
				if (item._status == TechTreeNodeStatus.Active && item._metaData.AbilityType == (int)techType)
				{
					int num2 = 0;
					switch (index)
					{
					case 1:
						num2 = Mathf.FloorToInt(item._metaData.Argument1);
						break;
					case 2:
						num2 = Mathf.FloorToInt(item._metaData.Argument2);
						break;
					}
					num += num2;
				}
			}
			return num;
		}

		public int GetAvailableNodesDiff(int level)
		{
			int num = 0;
			foreach (CabinTechTreeNode item in _itemList)
			{
				if (item._metaData.UnlockLevel == level)
				{
					num++;
				}
			}
			return num;
		}
	}
}
