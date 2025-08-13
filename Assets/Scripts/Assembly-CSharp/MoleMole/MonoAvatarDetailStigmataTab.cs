using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoAvatarDetailStigmataTab : MonoBehaviour
	{
		private FriendDetailDataItem _userData;

		private AvatarDataItem _avatarData;

		private bool _isRemoteAvatar;

		public void SetupView(AvatarDataItem avatarData)
		{
			_isRemoteAvatar = false;
			_avatarData = avatarData;
			SetupSlots();
			SetupSetEffect();
		}

		public void SetupView(FriendDetailDataItem userData)
		{
			_isRemoteAvatar = true;
			_userData = userData;
			_avatarData = _userData.leaderAvatar;
			SetupSlots();
			SetupSetEffect();
		}

		private void SetupSlots()
		{
			Transform transform = base.transform.Find("Slots");
			EquipmentSlot[] array = (EquipmentSlot[])(object)new EquipmentSlot[3]
			{
				(EquipmentSlot)2,
				(EquipmentSlot)3,
				(EquipmentSlot)4
			};
			for (int i = 1; i <= array.Length; i++)
			{
				GameObject gameObject = transform.Find(i.ToString()).gameObject;
				gameObject.GetComponent<MonoAvatarStigmataSlot>().SetupView(_avatarData, array[i - 1], i, _isRemoteAvatar);
			}
		}

		private void SetupSetEffect()
		{
			Transform transform = base.transform.Find("Effect");
			int num = 0;
			Dictionary<int, EquipSkillDataItem> dictionary = null;
			EquipSetDataItem ownEquipSetData = _avatarData.GetOwnEquipSetData();
			if (ownEquipSetData == null)
			{
				num = 0;
			}
			else
			{
				dictionary = ownEquipSetData.GetOwnSetSkills();
				num = dictionary.Count;
			}
			if (num > 0)
			{
				List<string> list = GenerateEffectDesc(dictionary);
				Transform transform2 = transform.Find("SetSkillPanel/ScrollView/Content");
				for (int i = 0; i < transform2.childCount; i++)
				{
					Transform child = transform2.GetChild(i);
					if (i >= list.Count)
					{
						child.Find("Desc").GetComponent<Text>().text = GetGrayDesc(i + 2);
					}
					else
					{
						child.Find("Desc").GetComponent<Text>().text = list[i];
					}
				}
			}
			else
			{
				Transform transform3 = transform.Find("SetSkillPanel/ScrollView/Content");
				for (int j = 0; j < transform3.childCount; j++)
				{
					Transform child2 = transform3.GetChild(j);
					child2.Find("Desc").GetComponent<Text>().text = GetGrayDesc(j + 2);
				}
			}
		}

		private List<string> GenerateEffectDesc(Dictionary<int, EquipSkillDataItem> setSkills)
		{
			List<string> list = new List<string>();
			OrderedDictionary orderedDictionary = new OrderedDictionary();
			foreach (KeyValuePair<int, EquipSkillDataItem> setSkill in setSkills)
			{
				orderedDictionary.Add(setSkill.Key, setSkill.Value);
			}
			foreach (DictionaryEntry item in orderedDictionary)
			{
				list.Add("<color=#FEDF4CFF>【" + item.Key.ToString() + "件】</color><color=#00d7ffFF>" + ((EquipSkillDataItem)item.Value).GetSkillDisplay() + "</color>");
			}
			return list;
		}

		private string GetGrayDesc(int index)
		{
			string text = LocalizationGeneralLogic.GetText(string.Format("EquipmentSkill_NotReady_{0}", index));
			return string.Format("<color=#FEDF4CFF>【{0}件】</color><color=#96b1c0FF>{1}</color>", index, text);
		}
	}
}
