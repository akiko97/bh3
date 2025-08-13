using MoleMole.Config;
using UnityEngine.UI;

namespace MoleMole
{
	public class AvatarLevelUpDialogContext : BaseSequenceDialogContext
	{
		private const float TIMER_SPAN = 2f;

		private CanvasTimer _timer;

		private uint _level;

		private uint _levelBefore;

		public AvatarLevelUpDialogContext(uint level, uint levelBefore)
		{
			config = new ContextPattern
			{
				contextName = "AvatarLevelUpDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/AvatarLevelUpDialog"
			};
			_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(2f, 0f);
			_timer.timeUpCallback = Destroy;
			_timer.StopRun();
			_level = level;
			_levelBefore = levelBefore;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Btn").GetComponent<Button>(), OnBGClick);
		}

		protected override bool SetupView()
		{
			_timer.StartRun();
			base.view.transform.Find("Dialog/Content/Level").GetComponent<Text>().text = string.Format("Lv.{0}", _level);
			base.view.transform.Find("Dialog/Content/LevelPast").GetComponent<Text>().text = string.Format("Lv.{0}", _levelBefore);
			return false;
		}

		private void OnBGClick()
		{
		}

		public override void Destroy()
		{
			_timer.Destroy();
			base.Destroy();
		}
	}
}
