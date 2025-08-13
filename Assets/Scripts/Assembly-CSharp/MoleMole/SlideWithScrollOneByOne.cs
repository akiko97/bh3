using UnityEngine;

namespace MoleMole
{
	public class SlideWithScrollOneByOne : MonoBehaviour
	{
		private const float TRANSIT_LINEAR_LERP_TIME = 1f;

		public MyScrollRect scrollRect;

		private float _orginPosition;

		private float _targetPosition;

		private float _timer;

		private RectTransform _rectTrans;

		private Transform _contentTrans;

		private float _step;

		public SetAvatar3DModel SetUpAvatar;

		private void Start()
		{
			_timer = 0f;
			scrollRect.horizontalNormalizedPosition = 0f;
			_orginPosition = scrollRect.horizontalNormalizedPosition;
			_targetPosition = _orginPosition;
			_rectTrans = base.transform as RectTransform;
			_contentTrans = base.transform.Find("Content");
			_step = 1f / (float)_contentTrans.childCount;
			OnScrollView();
		}

		private void Update()
		{
			if (_orginPosition != _targetPosition)
			{
				_timer += Time.deltaTime;
				if (_timer < 1f)
				{
					scrollRect.horizontalNormalizedPosition = Mathf.Lerp(_orginPosition, _targetPosition, _timer / 1f);
				}
				else
				{
					scrollRect.horizontalNormalizedPosition = _targetPosition;
					_orginPosition = _targetPosition;
					_timer = 0f;
					if (SetUpAvatar != null)
					{
						_step = 1f / (float)_contentTrans.childCount;
						SetUpAvatar(Mathf.FloorToInt(_targetPosition / _step));
					}
				}
			}
			OnScrollView();
		}

		public void SetTargetPosition(float targetPosition)
		{
			_orginPosition = scrollRect.horizontalNormalizedPosition;
			_targetPosition = targetPosition;
		}

		public void Next()
		{
			_step = 1f / (float)_contentTrans.childCount;
			if (_orginPosition + _step > 1f)
			{
				_targetPosition = 0f;
			}
			else
			{
				_targetPosition = _orginPosition + _step;
			}
		}

		public void Last()
		{
			_step = 1f / (float)_contentTrans.childCount;
			if (_orginPosition - _step < 0f)
			{
				_targetPosition = 1f;
			}
			else
			{
				_targetPosition = _orginPosition - _step;
			}
		}

		private void OnScrollView()
		{
			Vector2 position = _rectTrans.TransformPoint(_rectTrans.rect.position);
			Vector2 size = _rectTrans.TransformVector(_rectTrans.rect.size);
			Rect rect = new Rect(position, size);
			Transform transform = base.transform.Find("Content");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				RectTransform rectTransform = child as RectTransform;
				Vector2 position2 = rectTransform.TransformPoint(rectTransform.rect.position);
				Vector2 size2 = rectTransform.TransformVector(rectTransform.rect.size);
				Rect other = new Rect(position2, size2);
				bool active = rect.Overlaps(other, true);
				child.gameObject.SetActive(active);
			}
		}
	}
}
