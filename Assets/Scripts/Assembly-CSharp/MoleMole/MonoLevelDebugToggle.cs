using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoLevelDebugToggle : MonoBehaviour
	{
		public Toggle toggle;

		public string luaName;

		public Text luaNameText;

		public MonoLevelDebug levelDebug;

		public void OnValueChanged()
		{
			if (toggle.isOn)
			{
				levelDebug.luaName = luaName;
				levelDebug.Refresh(this);
			}
		}
	}
}
