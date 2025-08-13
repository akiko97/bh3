using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoChapterButton : MonoBehaviour
	{
		private ChapterDataItem _chapterData;

		private int _normal_star;

		private int _normal_sum;

		private int _hard_star;

		private int _hard_sum;

		private int _hell_star;

		private int _hell_sum;

		private readonly int _maxStar = 4;

		public bool selected { get; private set; }

		private void Awake()
		{
			base.transform.Find("Button").GetComponent<Button>().onClick.AddListener(OnClick);
		}

		public void SetupView(ChapterDataItem chapterData)
		{
			_chapterData = chapterData;
			base.transform.Find("Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_chapterData.CoverPic);
			base.transform.Find("Icon/Lock").gameObject.SetActive(!_chapterData.Unlocked);
			GameObject gameObject = base.transform.Find("Button").gameObject;
			if (gameObject != null)
			{
				MonoButtonWwiseEvent monoButtonWwiseEvent = gameObject.GetComponent<MonoButtonWwiseEvent>();
				if (monoButtonWwiseEvent == null)
				{
					monoButtonWwiseEvent = gameObject.AddComponent<MonoButtonWwiseEvent>();
				}
				monoButtonWwiseEvent.eventName = ((!_chapterData.Unlocked) ? "UI_Gen_Select_Negative" : "UI_Click");
			}
			InitChapterProgress();
			SetProgress();
		}

		public void OnClick()
		{
			MonoChapterScroller component = base.transform.parent.parent.GetComponent<MonoChapterScroller>();
			if (component.IsCenter(base.transform))
			{
				if (_chapterData.Unlocked)
				{
					Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(_chapterData));
				}
			}
			else
			{
				component.ClickToChangeCenter(base.transform);
			}
		}

		public void UpdateView(bool isSelected)
		{
			selected = isSelected;
			base.transform.Find("Icon/Mask").gameObject.SetActive(!isSelected);
			base.transform.Find("Icon/Lock/BG").gameObject.SetActive(isSelected);
			base.transform.Find("Arrow").gameObject.SetActive(isSelected);
			base.transform.Find("Frame").GetComponent<Image>().color = ((!isSelected) ? MiscData.GetColor("Black") : MiscData.GetColor("Blue"));
			base.transform.Find("Frame/Top").GetComponent<Image>().color = ((!isSelected) ? MiscData.GetColor("Black") : MiscData.GetColor("Blue"));
			base.transform.Find("Frame/Bottom").GetComponent<Image>().color = ((!isSelected) ? MiscData.GetColor("Black") : MiscData.GetColor("Blue"));
			base.transform.Find("Frame/Left").GetComponent<Image>().color = ((!isSelected) ? MiscData.GetColor("Black") : MiscData.GetColor("Blue"));
			base.transform.Find("Frame/Right").GetComponent<Image>().color = ((!isSelected) ? MiscData.GetColor("Black") : MiscData.GetColor("Blue"));
			SetProgress();
		}

		private void SetProgress()
		{
			base.transform.Find("Icon/Progres").gameObject.SetActive(_chapterData.Unlocked);
			if (_chapterData.Unlocked)
			{
				float num = (float)_normal_star / (float)_normal_sum;
				Image component = base.transform.Find("Icon/Progres/Normal/Progress").GetComponent<Image>();
				component.fillAmount = num;
				Text component2 = base.transform.Find("Icon/Progres/Normal/Text").GetComponent<Text>();
				int num2 = ((_normal_star != _normal_sum) ? ((int)(num * 100f)) : 100);
				component2.text = string.Format("{0}%", num2);
				float num3 = (float)_hard_star / (float)_hard_sum;
				Image component3 = base.transform.Find("Icon/Progres/Hard/Progress").GetComponent<Image>();
				component3.fillAmount = num3;
				Text component4 = base.transform.Find("Icon/Progres/Hard/Text").GetComponent<Text>();
				num2 = ((_hard_star != _hard_sum) ? ((int)(num3 * 100f)) : 100);
				component4.text = string.Format("{0}%", num2);
				float num4 = (float)_hell_star / (float)_hell_sum;
				Image component5 = base.transform.Find("Icon/Progres/Torment/Progress").GetComponent<Image>();
				component5.fillAmount = num4;
				Text component6 = base.transform.Find("Icon/Progres/Torment/Text").GetComponent<Text>();
				num2 = ((_hell_star != _hell_sum) ? ((int)(num4 * 100f)) : 100);
				component6.text = string.Format("{0}%", num2);
			}
		}

		private void InitChapterProgress()
		{
			if (!_chapterData.Unlocked)
			{
				return;
			}
			_normal_star = 0;
			List<LevelDataItem> levelList = _chapterData.GetLevelList();
			for (int i = 0; i < levelList.Count; i++)
			{
				LevelDataItem levelDataItem = levelList[i];
				if (levelDataItem.progress > 0)
				{
					_normal_star++;
				}
				for (int j = 0; j < levelDataItem.challengeList.Count; j++)
				{
					if (levelDataItem.challengeList[j].Finished)
					{
						_normal_star++;
					}
				}
			}
			_normal_sum = levelList.Count * _maxStar;
			_hard_star = 0;
			List<LevelDataItem> levelList2 = _chapterData.GetLevelList(LevelDiffculty.Hard);
			for (int k = 0; k < levelList2.Count; k++)
			{
				LevelDataItem levelDataItem2 = levelList2[k];
				if (levelDataItem2.progress > 0)
				{
					_hard_star++;
				}
				for (int l = 0; l < levelDataItem2.challengeList.Count; l++)
				{
					if (levelDataItem2.challengeList[l].Finished)
					{
						_hard_star++;
					}
				}
			}
			_hard_sum = levelList2.Count * _maxStar;
			_hell_star = 0;
			List<LevelDataItem> levelList3 = _chapterData.GetLevelList(LevelDiffculty.Hell);
			for (int m = 0; m < levelList3.Count; m++)
			{
				LevelDataItem levelDataItem3 = levelList3[m];
				if (levelDataItem3.progress > 0)
				{
					_hell_star++;
				}
				for (int n = 0; n < levelDataItem3.challengeList.Count; n++)
				{
					if (levelDataItem3.challengeList[n].Finished)
					{
						_hell_star++;
					}
				}
			}
			_hell_sum = levelList3.Count * _maxStar;
		}
	}
}
