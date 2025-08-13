using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoEquipSkillPanel : MonoBehaviour
	{
		private List<EquipSkillDataItem> _skills;

		public void SetupView(List<EquipSkillDataItem> skills, int equipLevel = 1)
		{
			_skills = skills;
			if (_skills == null)
			{
				return;
			}
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform transform = base.transform.Find("Skill_" + i);
				if (!(transform == null))
				{
					if (i < skills.Count)
					{
						transform.gameObject.SetActive(true);
						EquipSkillDataItem equipSkillDataItem = skills[i];
						transform.Find("Name").GetComponent<Text>().text = equipSkillDataItem.skillName;
						transform.Find("Desc").GetComponent<Text>().text = equipSkillDataItem.GetSkillDisplay(equipLevel);
					}
					else
					{
						transform.gameObject.SetActive(false);
					}
				}
			}
		}
	}
}
