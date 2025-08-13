using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoUIFadeEffect : MonoBehaviour
	{
		[SerializeField]
		public AnimationCurve AlphaCurve;

		private float _duration;

		private float _time;

		private Image _image;

		public void Init(float duration)
		{
			_duration = duration;
			_time = 0f;
		}

		private void Start()
		{
			_image = base.transform.Find("Panel").GetComponent<Image>();
		}

		private void Update()
		{
			_time += Time.deltaTime;
			float time = _time / _duration;
			if (_time <= 1f)
			{
				Color color = _image.color;
				color.a = AlphaCurve.Evaluate(time);
				_image.color = color;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
