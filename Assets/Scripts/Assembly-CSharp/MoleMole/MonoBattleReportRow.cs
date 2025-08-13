using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoBattleReportRow : MonoBehaviour
	{
		private string _targetName;

		private string _userName;

		private string _content;

		private string _contentNoColor;

		private EndlessMainPageContext.ViewStatus _viewStatus;

		public void SetupView(EndlessWarInfo battleInfo, EndlessMainPageContext.ViewStatus viewStatus)
		{
			_viewStatus = viewStatus;
			EndlessToolDataItem endlessToolDataItem = new EndlessToolDataItem((int)battleInfo.item_id);
			_targetName = ((!battleInfo.target_uidSpecified) ? string.Empty : GetPlayerName((int)battleInfo.target_uid));
			_userName = GetPlayerName((int)battleInfo.uid);
			if (endlessToolDataItem.ApplyToSelf)
			{
				_content = LocalizationGeneralLogic.GetText(endlessToolDataItem.ReportTextMapId, _userName);
			}
			else
			{
				_content = LocalizationGeneralLogic.GetText(endlessToolDataItem.ReportTextMapId, _userName, _targetName);
			}
			_contentNoColor = GetNoColorText(_content);
			base.transform.Find("Text").GetComponent<Text>().text = _content;
		}

		private string GetNoColorText(string input)
		{
			string text = Regex.Replace(input, "<color=#.+?>", string.Empty);
			return text.Replace("</color>", string.Empty);
		}

		public void SetFullColorText()
		{
			base.transform.Find("Text").GetComponent<Text>().text = _content;
		}

		public void SetNoColorText()
		{
			base.transform.Find("Text").GetComponent<Text>().text = _contentNoColor;
		}

		private string GetPlayerName(int uid)
		{
			if (_viewStatus == EndlessMainPageContext.ViewStatus.ShowCurrentGroup)
			{
				return UIUtil.GetPlayerNickname(Singleton<EndlessModule>.Instance.GetPlayerBriefData(uid));
			}
			return UIUtil.GetPlayerNickname(Singleton<EndlessModule>.Instance.GetTopGroupPlayerBriefData(uid));
		}
	}
}
