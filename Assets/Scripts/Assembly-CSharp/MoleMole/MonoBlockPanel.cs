using UnityEngine;

namespace MoleMole
{
	public class MonoBlockPanel : MonoBehaviour
	{
		private float INTERVAL_SPAN = 3f;

		private float _intervalTimer;

		private bool _isCounting;

		private void Update()
		{
			if (_isCounting)
			{
				_intervalTimer += Time.deltaTime;
				if (_intervalTimer > INTERVAL_SPAN)
				{
					_intervalTimer = 0f;
					_isCounting = false;
					base.gameObject.SetActive(false);
				}
			}
		}

		public void SetTimeSpanTakeEffect(float timeSpan)
		{
			base.gameObject.SetActive(true);
			INTERVAL_SPAN = timeSpan;
			_intervalTimer = 0f;
			_isCounting = true;
		}
	}
}
