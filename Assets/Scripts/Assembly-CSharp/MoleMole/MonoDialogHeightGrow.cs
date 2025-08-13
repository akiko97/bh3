using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoDialogHeightGrow : MonoBehaviour
	{
		public float speed;

		public MovementDirection direction;

		public float delayTime;

		public RectTransform contentTrans;

		private float _delayTimer;

		private bool _playGrowAnimation;

		private float _targetSize;

		private RectTransform _rectTrans;

		private RectTransform.Axis _axis;

		private int _currentStep;

		private Action _growEnd;

		private void Init()
		{
			_rectTrans = base.transform.GetComponent<RectTransform>();
			_axis = ((direction == MovementDirection.Vertical) ? RectTransform.Axis.Vertical : RectTransform.Axis.Horizontal);
			if (contentTrans == null)
			{
				contentTrans = base.transform.Find("Content").GetComponent<RectTransform>();
				if (!(contentTrans == null))
				{
				}
			}
			_targetSize = contentTrans.rect.height;
			if (direction == MovementDirection.Horizontal)
			{
				_targetSize = contentTrans.rect.width;
			}
			_rectTrans.SetSizeWithCurrentAnchors(_axis, 0f);
			_delayTimer = 0f;
			base.transform.GetComponent<CanvasGroup>().alpha = 0f;
		}

		private void Update()
		{
			if (!_playGrowAnimation)
			{
				return;
			}
			float num = _rectTrans.rect.height;
			if (direction == MovementDirection.Horizontal)
			{
				num = _rectTrans.rect.width;
			}
			if (_currentStep >= 3 && num >= _targetSize)
			{
				_playGrowAnimation = false;
				if (_growEnd != null)
				{
					_growEnd();
				}
			}
			switch (_currentStep)
			{
			case 0:
				Init();
				if (base.transform.GetComponent<ContentSizeFitter>() != null)
				{
					base.transform.GetComponent<ContentSizeFitter>().enabled = false;
				}
				_currentStep++;
				break;
			case 1:
				if (contentTrans.GetComponent<ContentSizeFitter>() != null)
				{
					contentTrans.GetComponent<ContentSizeFitter>().enabled = false;
				}
				if (contentTrans.GetComponent<VerticalLayoutGroup>() != null)
				{
					contentTrans.GetComponent<VerticalLayoutGroup>().enabled = false;
				}
				_currentStep++;
				break;
			case 2:
				foreach (Transform item in contentTrans.transform)
				{
					item.SetLocalPositionX(item.localPosition.x + contentTrans.rect.width);
				}
				_currentStep++;
				break;
			case 3:
				if (_delayTimer < delayTime)
				{
					_delayTimer += Time.deltaTime;
					break;
				}
				base.transform.GetComponent<CanvasGroup>().alpha = 1f;
				_rectTrans.SetSizeWithCurrentAnchors(_axis, num + speed * Time.deltaTime);
				break;
			}
		}

		public void PlayGrow(Action growEnd = null)
		{
			_playGrowAnimation = true;
			_currentStep = 0;
			_growEnd = growEnd;
			Init();
		}
	}
}
