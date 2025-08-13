using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoAvatarSkillStep : MonoBehaviour
	{
		private const string PATTERN = "\\[(\\d+)\\]";

		public Transform textPrefab;

		public Transform iconPrefab;

		private AvatarDataItem _avatarData;

		public void SetupView(AvatarDataItem avatar, string input)
		{
			Clear();
			_avatarData = avatar;
			int num = 0;
			foreach (Match item in Regex.Matches(input, "\\[(\\d+)\\]"))
			{
				if (num < item.Index)
				{
					string text = input.Substring(num, item.Index - num);
					AddText(text);
				}
				num = item.Index + item.Value.Length;
				int skillId = int.Parse(item.Groups[1].Value);
				AddIcon(skillId);
			}
			if (num < input.Length)
			{
				string text2 = input.Substring(num, input.Length - num);
				AddText(text2);
			}
		}

		private void Clear()
		{
			base.transform.DestroyChildren();
		}

		private void AddText(string text)
		{
			Transform transform = base.transform.AddChildFromPrefab(textPrefab, "Text");
			transform.GetComponent<Text>().text = text;
		}

		private void AddIcon(int skillId)
		{
			Transform transform = base.transform.AddChildFromPrefab(iconPrefab, "Icon");
			transform.Find("Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_avatarData.GetAvatarSkillBySkillID(skillId).IconPath);
		}
	}
}
