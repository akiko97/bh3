using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoLevelDifficultyButton : MonoBehaviour
	{
		private LevelDiffculty _difficulty;

		private Action<LevelDiffculty> _clickCallBack;

		public void SetupClickCallBack(Action<LevelDiffculty> callBack)
		{
			_clickCallBack = callBack;
			base.transform.Find("Btn").GetComponent<Button>().onClick.RemoveAllListeners();
			base.transform.Find("Btn").GetComponent<Button>().onClick.AddListener(OnClick);
		}

		public void SetupDifficulty(LevelDiffculty difficulty)
		{
			_difficulty = difficulty;
		}

		private void OnClick()
		{
			if (_clickCallBack != null)
			{
				_clickCallBack(_difficulty);
			}
		}
	}
}
