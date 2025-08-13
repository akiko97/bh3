using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class AchieveUnlockInLevelContext : BaseWidgetContext
	{
		private const float TIMER_SPAN = 2.5f;

		private int _missionId;

		private MissionDataItem _missionData;

		private CanvasTimer _timer;

		public AchieveUnlockInLevelContext()
		{
			config = new ContextPattern
			{
				contextName = "AchieveUnlockInLevelDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/AchieveUnlockInLevelDialog"
			};
			uiType = UIType.SuspendBar;
			_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(2.5f, 0f);
			_timer.timeUpCallback = OnTimeout;
			_timer.StopRun();
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.MissionUpdated)
			{
				return OnMissionUpdated((uint)ntf.body);
			}
			return false;
		}

		private void OnTimeout()
		{
			base.view.SetActive(false);
			_timer.StopRun();
		}

		public override void Destroy()
		{
			_timer.Destroy();
			base.Destroy();
		}

		private bool SetupByMissionInfo(int missionId)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Invalid comparison between Unknown and I4
			_missionId = missionId;
			_missionData = Singleton<MissionModule>.Instance.GetMissionDataItem(_missionId);
			if (_missionData == null || (int)_missionData.status != 3)
			{
				return false;
			}
			LinearMissionData linearMissionData = LinearMissionDataReader.TryGetLinearMissionDataByKey(missionId);
			if (linearMissionData == null || linearMissionData.IsAchievement == 0)
			{
				return false;
			}
			Transform transform = base.view.transform.Find("Dialog/AchieveName");
			if (transform != null)
			{
				transform.GetComponent<Text>().text = LocalizationGeneralLogic.GetText(_missionData.metaData.title);
			}
			return true;
		}

		public bool OnMissionUpdated(uint id)
		{
			if (SetupByMissionInfo((int)id))
			{
				base.view.SetActive(true);
				_timer.StartRun(true);
			}
			return false;
		}
	}
}
