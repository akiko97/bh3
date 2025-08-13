using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoStigmataAffixSkillPanel : MonoBehaviour
	{
		public void SetupView(StigmataDataItem stigmataData, List<StigmataDataItem.AffixSkillData> affixSkills)
		{
			if (!stigmataData.IsAffixIdentify)
			{
				if (base.transform.Find("Name") != null)
				{
					base.transform.Find("Name").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_AffixTab_NotIdentifyName");
				}
				Transform transform = base.transform.Find("AffixDesc_0");
				transform.gameObject.SetActive(true);
				transform.GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_AffixTab_NotIdentifyDesc");
				Transform transform2 = base.transform.Find("AffixDesc_1");
				transform2.gameObject.SetActive(false);
				return;
			}
			if (base.transform.Find("Name") != null)
			{
				base.transform.Find("Name").GetComponent<Text>().text = stigmataData.GetAffixName();
			}
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform transform3 = base.transform.Find("AffixDesc_" + i);
				if (!(transform3 == null))
				{
					if (i < affixSkills.Count)
					{
						transform3.gameObject.SetActive(true);
						EquipSkillDataItem skill = affixSkills[i].skill;
						transform3.GetComponent<Text>().text = skill.GetSkillDisplay(stigmataData.level);
					}
					else
					{
						transform3.gameObject.SetActive(false);
					}
				}
			}
		}
	}
}
