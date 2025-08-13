using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoComboText : MonoBehaviour
	{
		private readonly string[] _evaluationList = new string[6]
		{
			string.Empty,
			"Good",
			"Great",
			"Terrific",
			"Splendid",
			"Mavelous"
		};

		public void SetupView(int comboBefore, int comboAfter)
		{
			if (comboAfter > 0)
			{
				Text component = base.transform.Find("NumText/Combo/ComboNum").GetComponent<Text>();
				component.text = comboAfter + string.Empty;
				if (comboBefore == 0)
				{
					base.transform.localScale = Vector3.one;
				}
				if (base.transform.Find("NumText").GetComponent<Animation>().isPlaying)
				{
					base.transform.Find("NumText").GetComponent<Animation>().Rewind();
				}
				base.transform.Find("NumText").GetComponent<Animation>().Play();
				int comboEvaluation = GetComboEvaluation(comboAfter);
				string text = _evaluationList[comboEvaluation];
				foreach (Transform item in base.transform.Find("NumText/Evaluation"))
				{
					item.gameObject.SetActive(item.name == text);
				}
				base.transform.Find("NumText/Combo/ComboNumBG").GetComponent<Image>().color = Miscs.ParseColor(MiscData.Config.ComboNumFrameColor[comboEvaluation]);
			}
			else
			{
				base.transform.localScale = Vector3.zero;
			}
		}

		private int GetComboEvaluation(int combo)
		{
			int result = 0;
			for (int num = MiscData.Config.ComboEvaluation.Count - 1; num >= 0; num--)
			{
				if (combo >= MiscData.Config.ComboEvaluation[num])
				{
					result = num + 1;
					break;
				}
			}
			return result;
		}

		public void ActBlingEffect()
		{
			if (base.gameObject.activeInHierarchy && Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning)
			{
				Transform transform = base.transform.Find("NumText/Combo");
				Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Combo_Clear_Resist_Effect", transform.position, transform.forward, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
			}
		}
	}
}
