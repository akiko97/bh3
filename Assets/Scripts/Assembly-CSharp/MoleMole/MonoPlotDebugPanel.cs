using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoPlotDebugPanel : MonoBehaviour
	{
		private const int DEFAULT_PLOT_ID = 20001;

		private int _currentPlotID = 20001;

		private void Awake()
		{
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void OnShowPlotPanelClick()
		{
			SetupView();
			base.transform.gameObject.SetActive(true);
		}

		public void SetupView()
		{
		}

		public void OnClosePlotBtnClick()
		{
			base.transform.gameObject.SetActive(false);
		}

		public void ShowPlot()
		{
			Text component = base.transform.Find("InputField/Text").GetComponent<Text>();
			if (component != null)
			{
				if (!int.TryParse(component.text, out _currentPlotID) || _currentPlotID < 20001)
				{
					_currentPlotID = 20001;
				}
				PlotMetaData plotMetaData = PlotMetaDataReader.TryGetPlotMetaDataByKey(_currentPlotID);
				if (plotMetaData != null && !Singleton<CameraManager>.Instance.GetMainCamera().storyState.active)
				{
					bool isOn = base.transform.Find("LerpInToggle").GetComponent<Toggle>().isOn;
					bool isOn2 = base.transform.Find("LerpOutToggle").GetComponent<Toggle>().isOn;
					Singleton<CameraManager>.Instance.GetMainCamera().PlayStoryCameraState(_currentPlotID, isOn, isOn2);
					base.transform.gameObject.SetActive(false);
				}
			}
		}
	}
}
