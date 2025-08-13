using MoleMole.Config;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoleMole
{
	public class GeneralHintDialogContext : BaseDialogContext
	{
		private const float TIMER_SPAN = 2f;

		public string desc;

		private CanvasTimer _timer;

		public GeneralHintDialogContext(string desc, float timerSpan = 2f)
		{
			config = new ContextPattern
			{
				contextName = "GeneralHintDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/GeneralHintDialog"
			};
			this.desc = desc;
			if (_timer != null)
			{
				_timer.Destroy();
			}
			_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(timerSpan, 0f);
			_timer.timeUpCallback = OnTimerUp;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/Btn").GetComponent<Button>(), Destroy);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = desc;
			return false;
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		private void OnTimerUp()
		{
			Destroy();
		}

		public override void Destroy()
		{
			_timer.Destroy();
			base.Destroy();
		}
	}
}
