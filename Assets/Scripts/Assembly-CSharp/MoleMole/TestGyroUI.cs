using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class TestGyroUI : MonoBehaviour
	{
		private Gyroscope _gyro;

		public Text gravityText;

		public Text rotationRateText;

		public Text attitudeText;

		public void Start()
		{
			_gyro = Input.gyro;
			_gyro.enabled = GraphicsSettingData.IsEnableGyroscope();
		}

		public void Update()
		{
			if (_gyro != null)
			{
				gravityText.text = _gyro.gravity.ToString();
				rotationRateText.text = _gyro.rotationRate.ToString();
				attitudeText.text = _gyro.attitude.ToString() + "\n" + _gyro.attitude.eulerAngles.ToString();
			}
		}
	}
}
