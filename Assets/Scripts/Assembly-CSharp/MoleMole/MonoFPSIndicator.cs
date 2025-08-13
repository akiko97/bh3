using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public sealed class MonoFPSIndicator : MonoBehaviour
	{
		public float updateInterval = 0.5f;

		public float fpsMin = float.MaxValue;

		public float fpsMax = float.MinValue;

		public float fpsAvg;

		public string logContext = string.Empty;

		private float _time;

		private int _frames;

		private float _timeleft;

		private float _totalTime;

		private int _totalFrames;

		public void Start()
		{
			_time = 0f;
			_frames = 0;
			_totalTime = 0f;
			_totalFrames = 0;
			if ((bool)GetComponent<GUIText>())
			{
				_timeleft = updateInterval;
			}
		}

		public void Update()
		{
			_timeleft -= Time.deltaTime;
			if (Time.deltaTime > 0f)
			{
				_time += Time.unscaledDeltaTime;
				_totalTime += Time.unscaledDeltaTime;
			}
			_frames++;
			_totalFrames++;
			if (_timeleft <= 0f)
			{
				float num = (float)_frames / _time;
				if (num < fpsMin)
				{
					fpsMin = num;
				}
				if (num > fpsMax)
				{
					fpsMax = num;
				}
				base.transform.Find("Text").GetComponent<Text>().text = string.Empty + num.ToString("f2");
				_timeleft = updateInterval;
				_time = 0f;
				_frames = 0;
			}
		}
	}
}
