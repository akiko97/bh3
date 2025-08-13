using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoGMTalkButton : MonoBehaviour
	{
		public delegate void ButtonCallBack(string command);

		private string _command;

		private ButtonCallBack _buttonCallback;

		public void SetupView(string command, ButtonCallBack buttonCallback = null)
		{
			_command = command;
			_buttonCallback = buttonCallback;
			base.transform.Find("Text").GetComponent<Text>().text = command;
		}

		public void OnButtonCallBack()
		{
			if (_buttonCallback != null)
			{
				_buttonCallback(_command);
			}
		}
	}
}
