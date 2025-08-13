using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoNumberJump : MonoBehaviour
	{
		public Text text;

		public int valueChangeTimesPerSecond = 20;

		public int valueDelta = 1;

		public float showTime = 2f;

		public bool replay;

		private int currentValue;

		private bool _isTimeFormat;

		private int _targetValue;

		private bool _isPlaying;

		private float _updateTimer;

		private float _updateInterval;

		private bool _playAlready;

		private void Update()
		{
			if (replay && !_playAlready)
			{
				SetInitValue();
				replay = false;
				_isPlaying = true;
				_playAlready = true;
			}
			if (!_isPlaying)
			{
				return;
			}
			_updateTimer += Time.deltaTime;
			if (_updateTimer > _updateInterval)
			{
				currentValue += valueDelta;
				if (currentValue >= _targetValue)
				{
					currentValue = _targetValue;
					_isPlaying = false;
				}
				ShowCurrentValue();
				_updateTimer = 0f;
			}
		}

		private void SetInitValue()
		{
			int num = valueChangeTimesPerSecond * valueDelta;
			int num2 = _targetValue / num;
			if ((float)num2 > showTime)
			{
				currentValue = Mathf.FloorToInt((float)_targetValue - (float)num * showTime);
			}
			else
			{
				currentValue = 0;
			}
			_updateInterval = 1f / (float)valueChangeTimesPerSecond;
			if (_targetValue < valueDelta * valueChangeTimesPerSecond)
			{
				valueDelta = Mathf.Clamp(_targetValue / valueChangeTimesPerSecond, 1, valueDelta + 1);
			}
			_updateTimer = 0f;
			ShowCurrentValue();
		}

		public void SetTargetValue(int targetValue, bool isTimeFormat = false, bool startPlay = false)
		{
			_targetValue = targetValue;
			_isTimeFormat = isTimeFormat;
			if (startPlay)
			{
				replay = true;
			}
		}

		private void ShowCurrentValue()
		{
			if (_isTimeFormat)
			{
				int num = Mathf.CeilToInt(currentValue) / 60;
				int num2 = Mathf.CeilToInt(currentValue) - 60 * num;
				text.text = string.Format("{0:D2}:{1:D2}", num, num2);
			}
			else
			{
				text.text = currentValue.ToString();
			}
		}
	}
}
