using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoTechTreeUI : MonoBehaviour
	{
		private int _row;

		private int _column;

		[SerializeField]
		private GameObject _nodePrefab;

		[SerializeField]
		private RectTransform _content;

		private Vector3 _nodeScale = new Vector3(1f, 1f, 1f);

		private List<MonoTechNodeUI> _nodeUIObjList = new List<MonoTechNodeUI>();

		private CabinTechTree _tree;

		private List<CabinTechTreeNode> _nodeDataList = new List<CabinTechTreeNode>();

		private int _x_meta_to_ui;

		private int _y_meta_to_ui;

		private Vector2 _originNormalizedPositin;

		public bool HasChildren()
		{
			return _nodeUIObjList.Count > 0;
		}

		public void ClearNodes()
		{
			foreach (MonoTechNodeUI nodeUIObj in _nodeUIObjList)
			{
				Object.DestroyImmediate(nodeUIObj.gameObject);
			}
			_nodeUIObjList.Clear();
		}

		public void InitNodes(CabinTechTree tree)
		{
			_tree = tree;
			_nodeDataList = _tree.GetNodeList();
			GetTreeArea();
			_content.GetComponent<GridLayoutGroup>().constraintCount = _row;
			_originNormalizedPositin = new Vector2(0f, 0f);
			for (int i = 0; i < _row; i++)
			{
				for (int j = 0; j < _column; j++)
				{
					GameObject gameObject = Object.Instantiate(_nodePrefab);
					gameObject.transform.SetParent(_content.transform);
					gameObject.GetComponent<RectTransform>().localScale = _nodeScale;
					MonoTechNodeUI component = gameObject.GetComponent<MonoTechNodeUI>();
					CabinTechTreeNode dataByUI = GetDataByUI(j, i);
					if (dataByUI != null)
					{
						if (dataByUI._metaData.X == 0 && dataByUI._metaData.Y == 0)
						{
							_originNormalizedPositin.x = (float)j / (float)_row;
							_originNormalizedPositin.y = 1f - (float)i / (float)_column;
						}
						gameObject.name = string.Format("TechTreeNode_{0}", dataByUI._metaData.ID);
					}
					component.Init(dataByUI, j, i);
					component.RefreshStatus();
					_nodeUIObjList.Add(component);
				}
			}
		}

		public void RefreshUI()
		{
			foreach (MonoTechNodeUI nodeUIObj in _nodeUIObjList)
			{
				nodeUIObj.RefreshStatus();
			}
		}

		private void GetTreeArea()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < _nodeDataList.Count; i++)
			{
				CabinTechTreeNode cabinTechTreeNode = _nodeDataList[i];
				if (i == 0)
				{
					num3 = cabinTechTreeNode._metaData.X;
					num4 = cabinTechTreeNode._metaData.X;
					num = cabinTechTreeNode._metaData.Y;
					num2 = cabinTechTreeNode._metaData.Y;
				}
				num4 = ((cabinTechTreeNode._metaData.X <= num4) ? num4 : cabinTechTreeNode._metaData.X);
				num3 = ((cabinTechTreeNode._metaData.X >= num3) ? num3 : cabinTechTreeNode._metaData.X);
				num2 = ((cabinTechTreeNode._metaData.Y <= num2) ? num2 : cabinTechTreeNode._metaData.Y);
				num = ((cabinTechTreeNode._metaData.Y >= num) ? num : cabinTechTreeNode._metaData.Y);
			}
			_row = num2 - num + 1;
			_column = num4 - num3 + 1;
			_x_meta_to_ui = ((num3 >= 0) ? num3 : (-num3));
			_y_meta_to_ui = ((num >= 0) ? num : (-num));
		}

		private void Meta2UI(int x, int y, ref int x1, ref int y1)
		{
			x1 = x + _x_meta_to_ui;
			y1 = y + _y_meta_to_ui;
		}

		private void UI2Meta(int x, int y, ref int x1, ref int y1)
		{
			x1 = x - _x_meta_to_ui;
			y1 = y - _y_meta_to_ui;
		}

		private CabinTechTreeNode GetDataByUI(int x, int y)
		{
			int x2 = 0;
			int y2 = 0;
			UI2Meta(x, y, ref x2, ref y2);
			return _tree.GetNode(x2, y2);
		}

		public void SetOriginPosition()
		{
			base.transform.GetComponent<ScrollRect>().normalizedPosition = _originNormalizedPositin;
		}
	}
}
