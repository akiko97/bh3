using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoDropLink : MonoBehaviour
	{
		private LevelDataItem _levelData;

		private Action<LevelDataItem> _customeLevelClickCallBack;

		public void SetupView(LevelDataItem levelData, Action<LevelDataItem> customeLevelClickCallBack = null)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Invalid comparison between Unknown and I4
			_levelData = levelData;
			_customeLevelClickCallBack = customeLevelClickCallBack;
			if (levelData == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			base.gameObject.SetActive(true);
			bool flag = (int)levelData.status != 1 && levelData.UnlockPlayerLevel <= Singleton<PlayerModule>.Instance.playerData.teamLevel;
			base.transform.Find("Open").gameObject.SetActive(flag);
			base.transform.Find("Lock").gameObject.SetActive(!flag);
			Text text = ((!flag) ? base.transform.Find("Lock/Text").GetComponent<Text>() : base.transform.Find("Open/Text").GetComponent<Text>());
			text.text = levelData.StageName;
			Button component = base.transform.Find("Open").GetComponent<Button>();
			component.onClick.RemoveAllListeners();
			component.onClick.AddListener(OnDropLinkBtnClick);
		}

		public void OnDropLinkBtnClick()
		{
			if (_levelData != null)
			{
				if (_customeLevelClickCallBack != null)
				{
					_customeLevelClickCallBack(_levelData);
				}
				else if (Singleton<MainUIManager>.Instance.SceneCanvas is MonoMainCanvas)
				{
					ShowChapterSelectPage();
				}
				else
				{
					Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true, true, ShowChapterSelectPage);
				}
			}
		}

		private void ShowChapterSelectPage()
		{
			Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(_levelData));
		}
	}
}
