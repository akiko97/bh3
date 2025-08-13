using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoEquipExpGrow : MonoBehaviour
	{
		public MonoMaskSliderGrow slider;

		public string levelAudioName;

		private int levelBefore;

		private int expBefore;

		private int exp;

		private List<float> maxList;

		private int addTime = 1;

		public void PlayEquipExpSliderGrow()
		{
			slider.Play(expBefore, exp, maxList, ShowLevelUpHint);
		}

		public void SetData(int levelBefore, int maxExpBefore, int expBefore, int exp, List<float> maxList)
		{
			this.levelBefore = levelBefore;
			this.expBefore = expBefore;
			this.exp = exp;
			this.maxList = maxList;
			base.transform.Find("LevelLabel").GetComponent<Text>().text = "Lv." + levelBefore;
			base.transform.Find("Exp/NumText").GetComponent<Text>().text = expBefore.ToString();
			base.transform.Find("Exp/MaxNumText").GetComponent<Text>().text = maxExpBefore.ToString();
			base.transform.Find("Exp/TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue(expBefore, maxExpBefore, 0f);
		}

		private void ShowLevelUpHint(Transform sliderTrans)
		{
			base.transform.Find("LevelLabel").GetComponent<Text>().text = "Lv." + (levelBefore + addTime);
			addTime++;
			base.transform.Find("LevelUpHint").GetComponent<Animation>().Play();
			if (!string.IsNullOrEmpty(levelAudioName))
			{
				Singleton<WwiseAudioManager>.Instance.Post(levelAudioName);
			}
		}
	}
}
