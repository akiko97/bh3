using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class TabManager
	{
		public delegate void OnSetActive(bool active, GameObject content, Button btn);

		private Dictionary<string, GameObject> _tabContentMap;

		private Dictionary<string, Button> _tabBtnMap;

		private string _showingKey;

		public event OnSetActive onSetActive;

		public TabManager()
		{
			_tabContentMap = new Dictionary<string, GameObject>();
			_tabBtnMap = new Dictionary<string, Button>();
		}

		public void Clear()
		{
			_tabContentMap.Clear();
			_tabBtnMap.Clear();
		}

		public void ShowTab(string searchKey)
		{
			_showingKey = searchKey;
			foreach (string key in _tabContentMap.Keys)
			{
				if (this.onSetActive != null)
				{
					this.onSetActive(searchKey == key, _tabContentMap[key], _tabBtnMap[key]);
				}
			}
		}

		public void SetTab(string key, Button btn, GameObject content)
		{
			_tabContentMap[key] = content;
			_tabBtnMap[key] = btn;
		}

		public string GetShowingTabKey()
		{
			return _showingKey;
		}

		public GameObject GetShowingTabContent()
		{
			return GetTabContent(_showingKey);
		}

		public GameObject GetTabContent(string key)
		{
			return (!_tabContentMap.ContainsKey(key)) ? null : _tabContentMap[key];
		}

		public List<string> GetKeys()
		{
			return new List<string>(_tabBtnMap.Keys);
		}
	}
}
