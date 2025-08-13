using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class AchieveUnlockContext : BaseSequenceDialogContext
	{
		private const float TIMER_SPAN = 2f;

		private const float dialogShowDelay = 0.8f;

		private MissionDataItem _missionData;

		private CanvasTimer _timer;

		private SequenceAnimationManager _animationManager;

		public AchieveUnlockContext(int missionId)
		{
			config = new ContextPattern
			{
				contextName = "AchieveUnlockDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/AchieveUnlockDialog"
			};
			_missionData = Singleton<MissionModule>.Instance.GetMissionDataItem(missionId);
			_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(2f, 0f);
			_timer.timeUpCallback = OnBGClick;
			_timer.StopRun();
		}

		protected override void BindViewCallbacks()
		{
		}

		protected override bool SetupView()
		{
			_animationManager = new SequenceAnimationManager(StartTimer);
			Transform transform = base.view.transform.Find("Dialog/Content/AnimMoveIn3/AchieveName");
			if (transform != null && _missionData != null)
			{
				transform.GetComponent<Text>().text = LocalizationGeneralLogic.GetText(_missionData.metaData.title);
			}
			_animationManager.AddAllChildrenInTransform(base.view.transform.Find("Dialog/Content"));
			GameObject gameObject = base.view.transform.Find("Dialog/Content/AnimMoveIn1/AchieveIcon").gameObject;
			if (!string.IsNullOrEmpty(_missionData.metaData.thumb) && gameObject != null)
			{
				GameObject gameObject2 = Resources.Load<GameObject>(_missionData.metaData.thumb);
				if (gameObject2 != null)
				{
					gameObject.transform.Find("BG").GetComponent<Image>().sprite = gameObject2.transform.Find("BG").GetComponent<Image>().sprite;
					gameObject.transform.Find("Icon").GetComponent<Image>().sprite = gameObject2.transform.Find("Icon").GetComponent<Image>().sprite;
				}
			}
			_animationManager.StartPlay(0f, false);
			return false;
		}

		private void OnBGClick()
		{
			Destroy();
		}

		public override void Destroy()
		{
			_timer.Destroy();
			base.Destroy();
		}

		private void StartTimer()
		{
			_timer.StartRun(true);
		}
	}
}
