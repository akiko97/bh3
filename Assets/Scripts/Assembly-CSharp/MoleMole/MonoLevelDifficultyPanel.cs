using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoLevelDifficultyPanel : MonoBehaviour
	{
		public GameObject levelDifficultyGO;

		private bool _popUpActive;

		private LevelDiffculty _currentDifficulty = LevelDiffculty.Normal;

		private ChapterDataItem _chapterData;

		private List<LevelDiffculty> _difficultyList;

		public void Init(LevelDiffculty difficulty, ChapterDataItem chapterData)
		{
			_popUpActive = false;
			base.transform.Find("PopUp").gameObject.SetActive(_popUpActive);
			base.transform.Find("Btn").gameObject.SetActive(_popUpActive);
			_currentDifficulty = difficulty;
			_chapterData = chapterData;
			_difficultyList = new List<LevelDiffculty>();
			InitPopUp();
			SetupView();
			FireNotify(true);
		}

		private void InitPopUp()
		{
			_difficultyList = _chapterData.GetLevelDifficultyListInChapter();
			Transform transform = base.transform.Find("PopUp");
			for (int i = 1; i < _difficultyList.Count; i++)
			{
				Transform child;
				if (transform.childCount >= i)
				{
					child = transform.GetChild(i - 1);
				}
				else
				{
					GameObject gameObject = Object.Instantiate(levelDifficultyGO);
					gameObject.name = "difficulty_" + i;
					child = gameObject.transform;
					child.SetParent(transform, false);
				}
				child.GetComponent<MonoLevelDifficultyButton>().SetupClickCallBack(OnDifficultyBtnClick);
			}
			if (transform.childCount > _difficultyList.Count - 1)
			{
				for (int num = transform.childCount - 1; num >= _difficultyList.Count; num--)
				{
					Object.Destroy(transform.GetChild(num));
				}
			}
		}

		public void OnCurrentDifficultyClick()
		{
			if (_difficultyList.Count < 2)
			{
				base.transform.Find("PopUp").gameObject.SetActive(false);
				base.transform.Find("Btn").gameObject.SetActive(false);
				return;
			}
			_popUpActive = !_popUpActive;
			bool flag = UnlockUIDataReaderExtend.UnLockByMission(3) && UnlockUIDataReaderExtend.UnlockByTutorial(3);
			bool flag2 = UnlockUIDataReaderExtend.UnLockByMission(4) && UnlockUIDataReaderExtend.UnlockByTutorial(4);
			if (_difficultyList.Count > 2)
			{
				bool flag3 = _popUpActive && (flag || flag2);
				base.transform.Find("PopUp").gameObject.SetActive(flag3);
				base.transform.Find("Btn").gameObject.SetActive(flag3);
				bool active = flag3 && flag;
				bool active2 = flag3 && flag2;
				if (_difficultyList[0] == LevelDiffculty.Normal)
				{
					base.transform.Find("PopUp/difficulty_1").gameObject.SetActive(active);
					base.transform.Find("PopUp/difficulty_2").gameObject.SetActive(active2);
				}
				else if (_difficultyList[0] == LevelDiffculty.Hard)
				{
					base.transform.Find("PopUp/difficulty_1").gameObject.SetActive(_popUpActive);
					base.transform.Find("PopUp/difficulty_2").gameObject.SetActive(active2);
				}
				else
				{
					base.transform.Find("PopUp/difficulty_1").gameObject.SetActive(_popUpActive);
					base.transform.Find("PopUp/difficulty_2").gameObject.SetActive(active);
				}
			}
			else
			{
				bool active3 = _popUpActive && flag;
				base.transform.Find("PopUp").gameObject.SetActive(active3);
				base.transform.Find("Btn").gameObject.SetActive(active3);
				if (_difficultyList[0] == LevelDiffculty.Normal)
				{
					base.transform.Find("PopUp/difficulty_1").gameObject.SetActive(active3);
					base.transform.Find("PopUp/difficulty_2").gameObject.SetActive(false);
				}
				else
				{
					base.transform.Find("PopUp/difficulty_1").gameObject.SetActive(_popUpActive);
					base.transform.Find("PopUp/difficulty_2").gameObject.SetActive(false);
				}
			}
		}

		public void OnBGClick()
		{
			_popUpActive = false;
			base.transform.Find("PopUp").gameObject.SetActive(_popUpActive);
			base.transform.Find("Btn").gameObject.SetActive(_popUpActive);
		}

		private void SetupView()
		{
			_difficultyList.Sort(LevelDifficultySort);
			SetupDifficultyView(base.transform.Find("Current"), _currentDifficulty);
			bool flag = UnlockUIDataReaderExtend.UnLockByMission(3) && UnlockUIDataReaderExtend.UnlockByTutorial(3);
			bool flag2 = UnlockUIDataReaderExtend.UnLockByMission(4) && UnlockUIDataReaderExtend.UnlockByTutorial(4);
			Transform transform = base.transform.Find("Current/Arrow");
			if (_difficultyList.Count > 2)
			{
				transform.gameObject.SetActive(flag || flag2);
			}
			else if (_difficultyList.Count > 1)
			{
				transform.gameObject.SetActive(flag);
			}
			else
			{
				transform.gameObject.SetActive(false);
			}
			if (_difficultyList.Count > 1)
			{
				Transform transform2 = base.transform.Find("PopUp");
				for (int i = 1; i < _difficultyList.Count; i++)
				{
					LevelDiffculty difficulty = _difficultyList[i];
					Transform child = transform2.GetChild(i - 1);
					SetupDifficultyView(child, difficulty);
					child.GetComponent<MonoLevelDifficultyButton>().SetupDifficulty(difficulty);
				}
			}
		}

		private void FireNotify(bool isBtnClick)
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLevelDifficulty, _currentDifficulty));
		}

		private int LevelDifficultySort(LevelDiffculty lobj, LevelDiffculty robj)
		{
			if (lobj == robj)
			{
				return 0;
			}
			if (lobj == _currentDifficulty)
			{
				return -1;
			}
			if (robj == _currentDifficulty)
			{
				return 1;
			}
			return lobj.CompareTo(robj);
		}

		public void OnDifficultyBtnClick(LevelDiffculty difficulty)
		{
			_currentDifficulty = difficulty;
			SetupView();
			FireNotify(true);
			OnBGClick();
		}

		private void SetupDifficultyView(Transform trans, LevelDiffculty difficulty)
		{
			Color difficultyColor = Miscs.GetDifficultyColor(difficulty);
			string difficultyDesc = Miscs.GetDifficultyDesc(difficulty);
			string difficultyMark = UIUtil.GetDifficultyMark(difficulty);
			trans.Find("Color").GetComponent<Image>().color = difficultyColor;
			trans.Find("Desc").GetComponent<Text>().text = difficultyDesc;
			trans.Find("Icon/Image").GetComponent<Image>().color = difficultyColor;
			trans.Find("Icon/Text").GetComponent<Text>().text = difficultyMark;
		}
	}
}
