using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(Text))]
	public class LocalizedText : MonoBehaviour
	{
		[SerializeField]
		private string _textID;

		[SerializeField]
		private string _textPattern;

		private Text _label;

		public string TextID
		{
			get
			{
				return _textID;
			}
			set
			{
				_textID = value;
			}
		}

		public string TextPattern
		{
			get
			{
				return _textPattern;
			}
		}

		public void Start()
		{
			_label = GetComponent<Text>();
			SetupTextID(_textID, _textPattern);
		}

		public void SetupTextID(string textID, string textPattern)
		{
			_label.text = LocalizationGeneralLogic.GetText(textID);
			if (!string.IsNullOrEmpty(textPattern))
			{
				_label.text = textPattern.Replace("#1", _label.text);
			}
		}

		public void SetupTextID(string textID, params object[] replaceParams)
		{
			_label.text = LocalizationGeneralLogic.GetText(textID, replaceParams);
		}
	}
}
