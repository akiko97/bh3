using UnityEngine;
using UnityEngine.EventSystems;

namespace MoleMole
{
	public class MonoReportList : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		private const float FADE_OUT_TIME = 3f;

		private float _timer;

		private bool _isFullColor;

		public void Init()
		{
			DoSetFullColor(false);
		}

		public void Update()
		{
			if (_timer >= 0f)
			{
				_timer -= Time.unscaledDeltaTime;
				if (_timer < 0f)
				{
					SetFullColor(false);
				}
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			SetFullColor(true);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_timer = 3f;
		}

		public void SetFullColor(bool fullColor)
		{
			if (_isFullColor != fullColor)
			{
				_isFullColor = fullColor;
				DoSetFullColor(_isFullColor);
			}
		}

		private void DoSetFullColor(bool fullColor)
		{
			MonoBattleReportRow[] componentsInChildren = base.transform.GetComponentsInChildren<MonoBattleReportRow>(true);
			foreach (MonoBattleReportRow monoBattleReportRow in componentsInChildren)
			{
				if (fullColor)
				{
					monoBattleReportRow.SetFullColorText();
				}
				else
				{
					monoBattleReportRow.SetNoColorText();
				}
			}
		}
	}
}
