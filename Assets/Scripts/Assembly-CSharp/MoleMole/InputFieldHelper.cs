using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class InputFieldHelper : MonoBehaviour
	{
		public int mCharacterlimit;

		public void OnEndEdit(InputField vInput)
		{
			if (mCharacterlimit > 0)
			{
				string text = vInput.text.Trim();
				int length = Mathf.Min(mCharacterlimit, text.Length);
				text = text.Substring(0, length);
				vInput.text = text;
			}
		}
	}
}
