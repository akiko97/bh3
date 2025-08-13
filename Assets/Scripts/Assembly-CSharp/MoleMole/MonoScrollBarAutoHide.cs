using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(Scrollbar))]
	[RequireComponent(typeof(CanvasGroup))]
	public class MonoScrollBarAutoHide : MonoBehaviour
	{
		private bool _isFadingOut;

		private CanvasGroup _canvasGroup;

		private float _fadeOutTimer;

		public float fadeOutTimeSpan = 0.5f;

		public bool hidebyDefault;

		private void Awake()
		{
			_isFadingOut = false;
			_canvasGroup = GetComponent<CanvasGroup>();
			_canvasGroup.alpha = 0f;
			_fadeOutTimer = 0f;
		}

		private void Update()
		{
			if (_isFadingOut)
			{
				_fadeOutTimer += Time.deltaTime;
				if (_fadeOutTimer < fadeOutTimeSpan)
				{
					_canvasGroup.alpha = 1f - _fadeOutTimer / fadeOutTimeSpan;
					return;
				}
				_canvasGroup.alpha = 0f;
				_isFadingOut = false;
			}
		}

		public void UpdateStatus(float velocity)
		{
			if (Mathf.Abs(velocity) > 2f)
			{
				_canvasGroup.alpha = 1f;
				_isFadingOut = false;
			}
			else if (!_isFadingOut)
			{
				_isFadingOut = true;
				_fadeOutTimer = 0f;
			}
		}

		public void UpdateStatus(bool visible)
		{
			if (_canvasGroup == null)
			{
				_canvasGroup = GetComponent<CanvasGroup>();
			}
			_canvasGroup.alpha = ((!visible) ? 0f : 1f);
		}
	}
}
