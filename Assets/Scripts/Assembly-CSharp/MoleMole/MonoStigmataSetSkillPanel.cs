using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoStigmataSetSkillPanel : MonoBehaviour
	{
		public void SetupView(StigmataDataItem stigmataData, SortedDictionary<int, EquipSkillDataItem> setSkills)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				int num = i + 2;
				Transform transform = base.transform.Find("SetSkill_" + num);
				if (!(transform == null))
				{
					EquipSkillDataItem value;
					setSkills.TryGetValue(num, out value);
					if (value == null)
					{
						transform.gameObject.SetActive(false);
					}
					else
					{
						transform.Find("Desc").GetComponent<Text>().text = value.GetSkillDisplay();
					}
				}
			}
		}
	}
}
