using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoMonsterToggle : MonoBehaviour
	{
		public enum ToggleColumn
		{
			CATEGORY = 0,
			NAME = 1,
			TYPE = 2
		}

		public MonoDebugPanel debugPanel;

		public string toggleValue;

		public ToggleColumn column;

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void SetMonsterToggleValue(MonoDebugPanel debugPanel, string value, ToggleColumn column)
		{
			this.debugPanel = debugPanel;
			toggleValue = value;
			this.column = column;
		}

		public void OnToggleValueChanged(bool isOn)
		{
			if (isOn)
			{
				if (column == ToggleColumn.CATEGORY)
				{
					debugPanel.OnMonsterCategoryToggleValueChanged(base.gameObject.GetComponent<Toggle>());
				}
				else if (column == ToggleColumn.NAME)
				{
					debugPanel.OnMonsterNameToggleValueChanged(base.gameObject.GetComponent<Toggle>());
				}
				else if (column == ToggleColumn.TYPE)
				{
					debugPanel.OnMonsterTypeToggleValueChanged(base.gameObject.GetComponent<Toggle>());
				}
			}
		}
	}
}
