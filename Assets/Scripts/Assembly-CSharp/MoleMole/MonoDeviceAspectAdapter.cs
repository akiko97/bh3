using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[ExecuteInEditMode]
	public class MonoDeviceAspectAdapter : MonoBehaviour
	{
		public enum ScreenMode
		{
			IPHONE_5_SCREEN_MODE = 0,
			ANDROID_16_10_MODE = 1,
			IPHONE_SCREEN_MODE = 2,
			IPAD_SCREEN_MODE = 3
		}

		public ScreenMode screenMode;

		public FixFunction fixFunction;

		private void OnEnable()
		{
			screenMode = GetScreenMode();
			if (fixFunction != null)
			{
				fixFunction.Invoke();
			}
		}

		private ScreenMode GetScreenMode()
		{
			float num = 1f * (float)Mathf.Max(Screen.width, Screen.height) / (float)Mathf.Min(Screen.width, Screen.height);
			if ((double)num > 1.76)
			{
				return ScreenMode.IPHONE_5_SCREEN_MODE;
			}
			if ((double)num >= 1.6)
			{
				return ScreenMode.ANDROID_16_10_MODE;
			}
			if ((double)num > 1.49)
			{
				return ScreenMode.IPHONE_SCREEN_MODE;
			}
			return ScreenMode.IPAD_SCREEN_MODE;
		}

		public void FixLevelPanel()
		{
			Vector2 cellSize = base.transform.GetComponent<GridLayoutGroup>().cellSize;
			if (screenMode == ScreenMode.ANDROID_16_10_MODE)
			{
				cellSize.y = 700f;
			}
			else if (screenMode == ScreenMode.IPAD_SCREEN_MODE || screenMode == ScreenMode.IPHONE_SCREEN_MODE)
			{
				cellSize.y = 900f;
			}
			base.transform.GetComponent<GridLayoutGroup>().cellSize = cellSize;
		}
	}
}
