using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(Text))]
	public class MonoNumberGrow : MonoBehaviour
	{
		public float duration = 1f;

		public bool play;

		public float valueBefore;

		public float valueAfter;

		public bool isInt;

		private Text _text;

		private float _value;

		private float _timer;

		private void Awake()
		{
			play = false;
			_text = GetComponent<Text>();
		}

		private void Update()
		{
			if (play)
			{
				_timer += Time.deltaTime;
				_value = Mathf.Lerp(valueBefore, valueAfter, _timer / duration);
				if (Mathf.Approximately(_value, valueAfter))
				{
					play = false;
					_value = valueAfter;
				}
				if (isInt)
				{
					_text.text = ((!(valueAfter > valueBefore)) ? Mathf.CeilToInt(_value).ToString() : Mathf.FloorToInt(_value).ToString());
				}
				else
				{
					_text.text = _value.ToString();
				}
			}
		}

		public void Play(float valueBefore, float valueAfter, float duration, bool isInt = true)
		{
			if (valueBefore != valueAfter)
			{
				play = true;
				this.isInt = isInt;
				this.valueBefore = valueBefore;
				this.valueAfter = valueAfter;
				this.duration = duration;
				_timer = 0f;
				_value = valueBefore;
				_text.text = _value.ToString();
			}
		}
	}
}
