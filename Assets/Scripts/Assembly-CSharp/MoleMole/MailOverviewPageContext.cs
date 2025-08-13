using System.Collections.Generic;
using System.Linq;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MailOverviewPageContext : BasePageContext
	{
		private List<MailDataItem> _mailList;

		private SequenceDialogManager _dialogManager;

		private MonoGridScroller _scroller;

		private MonoScrollerFadeManager _scrollerFadeInManager;

		private Dictionary<int, RectTransform> _dictBeforeFetch;

		public MailOverviewPageContext()
		{
			config = new ContextPattern
			{
				contextName = "MailOverviewPageContext",
				viewPrefabPath = "UI/Menus/Page/Mail/MailOverviewPage"
			};
			_mailList = new List<MailDataItem>();
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 87 || cmdId == 85)
			{
				SetupView();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("InfoPanel/GetAllBtn").GetComponent<Button>(), OnGetAllBtnClick);
		}

		protected override bool SetupView()
		{
			_mailList = Singleton<MailModule>.Instance.GetAllMails();
			_scroller = base.view.transform.Find("MailListPanel/ScrollView").GetComponent<MonoGridScroller>();
			_scroller.Init(OnScrollerChange, _mailList.Count);
			_scrollerFadeInManager = base.view.transform.Find("MailListPanel/ScrollView").GetComponent<MonoScrollerFadeManager>();
			_scrollerFadeInManager.Init(_scroller.GetItemDict(), _dictBeforeFetch, IsMailEqual);
			_dictBeforeFetch = null;
			_scrollerFadeInManager.Play();
			base.view.transform.Find("InfoPanel/MailNum/Num").GetComponent<Text>().text = _mailList.Count.ToString();
			_dialogManager = new SequenceDialogManager(ClearUnlockAvatarDialogs);
			return false;
		}

		public override void BackPage()
		{
			Singleton<MailModule>.Instance.SetAllMailAsOld();
			base.BackPage();
		}

		public override void Destroy()
		{
			Singleton<MailModule>.Instance.SetAllMailAsOld();
			base.Destroy();
		}

		private void OnGetAllBtnClick()
		{
			if (!Singleton<MailModule>.Instance.HasAttachmentMail())
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_NoAttachmentMailHint")));
			}
			else
			{
				Singleton<NetworkManager>.Instance.RequestGetAllMailAttachment();
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.UnlockAvatar)
			{
				int avatarID = (int)ntf.body;
				AvatarUnlockDialogContext dialogContext = new AvatarUnlockDialogContext(avatarID);
				_dialogManager.AddDialog(dialogContext);
				if (!_dialogManager.IsPlaying())
				{
					_dialogManager.StartShow();
				}
			}
			return false;
		}

		private void OnScrollerChange(Transform trans, int index)
		{
			if (index >= 0 && index < _mailList.Count)
			{
				MailDataItem mailData = _mailList[index];
				trans.GetComponent<MonoMailInfoRow>().SetupView(mailData, OnMailCheckBtnClick, OnMailGetBtnClick);
			}
		}

		private void OnMailCheckBtnClick(MailDataItem mailData)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			Singleton<MailModule>.Instance.SetMailAsOld(mailData);
			if ((int)mailData.type == 3)
			{
				Singleton<MailModule>.Instance.SetMailRead(mailData);
			}
			SetupView();
			Singleton<MainUIManager>.Instance.ShowDialog(new MailDetailDialogContext(mailData));
		}

		private void OnMailGetBtnClick(MailDataItem mailData)
		{
			_dictBeforeFetch = _scroller.GetItemDict().ToDictionary((KeyValuePair<int, RectTransform> entry) => entry.Key, (KeyValuePair<int, RectTransform> entry) => entry.Value);
			Singleton<NetworkManager>.Instance.RequestGetOneMailAttachment(mailData);
		}

		private void ClearUnlockAvatarDialogs()
		{
			_dialogManager.ClearDialogs();
		}

		private bool IsMailEqual(RectTransform mailNew, RectTransform mailOld)
		{
			if (mailNew == null || mailOld == null)
			{
				return false;
			}
			MonoMailInfoRow component = mailOld.GetComponent<MonoMailInfoRow>();
			MonoMailInfoRow component2 = mailNew.GetComponent<MonoMailInfoRow>();
			return component2.GetMailCacheKey() == component.GetMailCacheKey();
		}
	}
}
