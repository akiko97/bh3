using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoBulletinBody : MonoBehaviour
	{
		private const string PATTERN = "<type=\"(.+?)\" (.+?)/>";

		public Transform textPrefab;

		public Transform paragraphPrefab;

		public Transform linkPrefab;

		public void SetupView(string input)
		{
			Clear();
			int num = 0;
			foreach (Match item in Regex.Matches(input, "<type=\"(.+?)\" (.+?)/>"))
			{
				if (num < item.Index)
				{
					string text = input.Substring(num, item.Index - num);
					AddText(text);
				}
				num = item.Index + item.Value.Length;
				switch (item.Groups[1].Value)
				{
				case "p":
					AddParagraph(item.Groups[2].Value);
					break;
				case "webview":
					AddLink(item.Groups[2].Value, true);
					break;
				case "browser":
					AddLink(item.Groups[2].Value, false);
					break;
				}
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

		private void AddParagraph(string str)
		{
			Regex regex = new Regex("text=\"(.*?)\"");
			string value = regex.Match(str).Groups[1].Value;
			Transform transform = base.transform.AddChildFromPrefab(paragraphPrefab, "Paragraph");
			transform.GetComponent<Text>().text = value;
		}

		private void AddLink(string str, bool isWebview)
		{
			Regex regex = new Regex("text=\"(.*?)\"");
			string value = regex.Match(str).Groups[1].Value;
			Regex regex2 = new Regex("href=\"(.*?)\"");
			string value2 = regex2.Match(str).Groups[1].Value;
			string opeUrl = OpeUtil.ConvertEventUrl(value2);
			Transform transform = base.transform.AddChildFromPrefab(linkPrefab, "Link");
			transform.GetComponent<Text>().text = value;
			transform.GetComponent<Button>().onClick.AddListener(delegate
			{
				OpenUrl(opeUrl, isWebview);
			});
		}

		private void OpenUrl(string url, bool isWebview)
		{
			if (isWebview)
			{
				WebViewGeneralLogic.LoadUrl(url);
			}
			else
			{
				Application.OpenURL(url);
			}
		}
	}
}
