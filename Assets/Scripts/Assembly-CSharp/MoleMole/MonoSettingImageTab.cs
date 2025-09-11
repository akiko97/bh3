using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoSettingImageTab : MonoBehaviour
	{
		private float _contrastDelta;

		private int _contrastShowMinValue = -20;

		private int _contrastShowMaxValue = 20;

		private Slider _contrastSlider;

		public void SetupView()
		{
			base.transform.Find("Content/RT/3dModel").GetComponent<MonoGammaSettingRenderImage>().SetupView();
			GraphicsSettingData.ApplyPersonalContrastDelta();
			_contrastSlider = base.transform.Find("Content/Contrast/Slider").GetComponent<Slider>();
			RecoverOriginState();
		}

		public bool CheckNeedSave()
		{
			return !GraphicsSettingData.IsEqualToPersonalContrastDelta(_contrastDelta);
		}

		public void OnNoSaveBtnClick()
		{
			RecoverOriginState();
		}

		public void OnSaveBtnClick()
		{
			GraphicsSettingData.SavePersonalContrastDelta(_contrastDelta);
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_SettingSaveSuccess")));
		}

		public void OnContrastDeltaChanged()
		{
			_contrastDelta = (_contrastSlider.value - (float)_contrastShowMinValue) / 20f - 1f;
			GraphicsSettingUtil.SetPostFXContrast(_contrastDelta);
			ShowContrast((int)_contrastSlider.value);
		}

		private void RecoverOriginState()
		{
			GraphicsSettingData.ApplyPersonalContrastDelta();
			GraphicsSettingData.CopyPersonalContrastDelta(ref _contrastDelta);
			int num = (int)((float)(_contrastShowMaxValue - _contrastShowMinValue) / 2f * (_contrastDelta + 1f) + (float)_contrastShowMinValue);
			_contrastSlider.value = num;
			ShowContrast(num);
		}

		private void ShowContrast(int showValue)
		{
			if (showValue == _contrastShowMinValue)
			{
				_contrastSlider.transform.Find("Fill Area/Fill").GetComponent<Image>().enabled = false;
			}
			else
			{
				_contrastSlider.transform.Find("Fill Area/Fill").GetComponent<Image>().enabled = true;
			}
			_contrastSlider.transform.Find("Handle Slide Area/Handle/Popup/PopupSmall/Text").GetComponent<Text>().text = showValue.ToString();
		}
	}
}
