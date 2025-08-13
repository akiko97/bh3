using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class PlayerLevelUpDialogContext : BaseSequenceDialogContext
	{
		private const string NEW_FEATURE_PREFAB_PATH = "UI/Menus/Widget/Map/NewFeature";

		private const float TIMER_SPAN = 2f;

		private SequenceAnimationManager _animationManager;

		private CanvasTimer _timer;

		private int _levelBefore_no_scoremanager;

		private AvatarCardDataItem _avatarData;

		public PlayerLevelUpDialogContext()
		{
			config = new ContextPattern
			{
				contextName = "PlayerLevelUpDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/PlayerLevelUpDialog"
			};
			if (_timer != null)
			{
				_timer.Destroy();
			}
			_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(2f, 0f);
			_timer.timeUpCallback = OnBGClick;
			_timer.StopRun();
			_avatarData = null;
		}

		public void SetLevelBeforeNoScoreManager(int level)
		{
			_levelBefore_no_scoremanager = level;
		}

		public void SetNotifyWhenDestroy(AvatarCardDataItem avatarData)
		{
			_avatarData = avatarData;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Btn").GetComponent<Button>(), OnBGClick);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Btn").gameObject.SetActive(false);
			_animationManager = new SequenceAnimationManager(StartTimer);
			LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
			int num = ((instance == null) ? _levelBefore_no_scoremanager : instance.playerLevelBefore);
			int teamLevel = Singleton<PlayerModule>.Instance.playerData.teamLevel;
			PlayerLevelMetaData playerLevelMetaData = PlayerLevelMetaDataReader.TryGetPlayerLevelMetaDataByKey(num);
			SuperDebug.VeryImportantAssert(playerLevelMetaData != null, string.Format("Cannot get player level data for player level:{0}", num));
			if (playerLevelMetaData == null)
			{
				playerLevelMetaData = PlayerLevelMetaDataReader.TryGetPlayerLevelMetaDataByKey(1);
			}
			base.view.transform.Find("Dialog/Content/LevelInfo/LvBefore/Lv").GetComponent<Text>().text = num.ToString();
			base.view.transform.Find("Dialog/Content/LevelInfo/LvAfter/Lv").GetComponent<Text>().text = teamLevel.ToString();
			int maxStamina = Singleton<PlayerModule>.Instance.playerData.MaxStamina;
			int stamina = playerLevelMetaData.stamina;
			Transform transform = base.view.transform.Find("Dialog/Content/MaxStamina");
			if (maxStamina > stamina)
			{
				transform.gameObject.SetActive(true);
				transform.Find("Num").GetComponent<Text>().text = maxStamina.ToString();
				transform.Find("AddNum").GetComponent<Text>().text = "+" + (maxStamina - stamina);
			}
			else
			{
				transform.gameObject.SetActive(false);
			}
			int avatarLevelLimit = Singleton<PlayerModule>.Instance.playerData.AvatarLevelLimit;
			int avatarLevelLimit2 = playerLevelMetaData.avatarLevelLimit;
			Transform transform2 = base.view.transform.Find("Dialog/Content/MaxAvatarLevel");
			if (avatarLevelLimit > avatarLevelLimit2)
			{
				transform2.gameObject.SetActive(true);
				transform2.Find("Num").GetComponent<Text>().text = avatarLevelLimit.ToString();
				transform2.Find("AddNum").GetComponent<Text>().text = "+" + (avatarLevelLimit - avatarLevelLimit2);
			}
			else
			{
				transform2.gameObject.SetActive(false);
			}
			RectTransform component = base.view.transform.Find("Dialog").GetComponent<RectTransform>();
			List<string> newFeatures = MiscData.GetNewFeatures(num, teamLevel);
			for (int i = 0; i < newFeatures.Count; i++)
			{
				string text = LocalizationGeneralLogic.GetText(newFeatures[i]);
				GameObject gameObject = Object.Instantiate(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Map/NewFeature"));
				gameObject.transform.SetParent(component.Find("Content"), false);
				gameObject.transform.Find("FeatureName").GetComponent<Text>().text = text;
			}
			_animationManager.AddAllChildrenInTransform(base.view.transform.Find("Dialog/Content"));
			base.view.transform.Find("Dialog").GetComponent<MonoDialogHeightGrow>().PlayGrow(OnDialogBGGrowEnd);
			return false;
		}

		private void OnBGClick()
		{
			Destroy();
		}

		public override void Destroy()
		{
			if (_avatarData != null)
			{
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MissionRewardAvatarGot, _avatarData));
			}
			_timer.Destroy();
			base.Destroy();
		}

		private void OnDialogBGGrowEnd()
		{
			_animationManager.StartPlay();
			base.view.transform.Find("Btn").gameObject.SetActive(true);
		}

		private void StartTimer()
		{
			_timer.StartRun();
		}
	}
}
