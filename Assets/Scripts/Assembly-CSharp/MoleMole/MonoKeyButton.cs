using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoKeyButton : MonoBehaviour
	{
		public string KeyButtonCode;

		public Button button;

		private void Start()
		{
		}

		private void DoSomeThing()
		{
			button.onClick.Invoke();
		}

		private void Update()
		{
			if (GlobalVars.KEYBOARD_FUNCTION_BUTTON_CONTROL && button.interactable)
			{
			}
		}
	}
}
