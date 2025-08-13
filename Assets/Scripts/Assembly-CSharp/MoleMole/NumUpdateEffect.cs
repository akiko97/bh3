using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(Text))]
	[RequireComponent(typeof(Animation))]
	public class NumUpdateEffect : MonoBehaviour
	{
		private Text _textComponent;

		private string _textContent;

		private Animation _ani;

		private void Awake()
		{
			_textComponent = base.transform.GetComponent<Text>();
			_textContent = _textComponent.text;
			_ani = base.transform.GetComponent<Animation>();
		}

		private void Update()
		{
			if (_textContent != _textComponent.text)
			{
				_textContent = _textComponent.text;
				_ani.Play();
			}
		}
	}
}
