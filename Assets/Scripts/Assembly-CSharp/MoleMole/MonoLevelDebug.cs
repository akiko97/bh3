using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoLevelDebug : MonoBehaviour
	{
		private const float BEGIN_POS_X = 330f;

		private const float BEGIN_POS_Y = -60f;

		private const float POS_Y_OFFSET = 60f;

		public string luaName = "Level0.lua";

		public Transform scrollArea;

		public List<MonoLevelDebugToggle> toggleList;

		public bool useDynamicLevel;

		public Image dynamicLvImg;

		private void Start()
		{
			toggleList = new List<MonoLevelDebugToggle>();
			int num = 0;
			string[] array = DesignDataTemp.LEVEL_LUA_ENTRY_FILE_NAMES["Common"];
			foreach (string text in array)
			{
				Transform transform = Object.Instantiate(Miscs.LoadResource<GameObject>("UI/Menus/Widget/DevLevel/DebugLevelToggle")).transform;
				transform.SetParent(scrollArea, false);
				RectTransform rectTransform = (RectTransform)transform;
				rectTransform.anchoredPosition = new Vector3(330f, -60f - (float)num * 60f, 0f);
				MonoLevelDebugToggle component = transform.GetComponent<MonoLevelDebugToggle>();
				component.luaName = text;
				component.luaNameText.text = text;
				component.levelDebug = this;
				toggleList.Add(component);
				num++;
			}
			RectTransform rectTransform2 = (RectTransform)scrollArea;
			rectTransform2.sizeDelta = new Vector2(0f, (float)num * 60f);
		}

		public void OnClickDebugButton()
		{
			base.transform.gameObject.SetActive(!base.transform.gameObject.activeSelf);
		}

		public void OnClickLevelButton()
		{
			Singleton<LevelScoreManager>.Create();
			Singleton<LevelScoreManager>.Instance.SetDebugLevelBeginIntent(luaName);
			Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
			ChapterSelectPageContext chapterSelectPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext as ChapterSelectPageContext;
			if (chapterSelectPageContext != null)
			{
				chapterSelectPageContext.OnDoLevelBegin();
			}
			Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", true, true);
		}

		public void Refresh(MonoLevelDebugToggle theToggle)
		{
			for (int i = 0; i < toggleList.Count; i++)
			{
				if (toggleList[i] != theToggle)
				{
					toggleList[i].toggle.isOn = false;
				}
			}
		}

		public void OnClickDynamicLvButton()
		{
			useDynamicLevel = !useDynamicLevel;
			if (useDynamicLevel)
			{
				dynamicLvImg.color = Color.red;
			}
			else
			{
				dynamicLvImg.color = Color.white;
			}
		}
	}
}
