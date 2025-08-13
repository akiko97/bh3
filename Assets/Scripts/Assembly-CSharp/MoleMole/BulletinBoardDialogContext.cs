using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class BulletinBoardDialogContext : BaseSequenceDialogContext
	{
		public enum ShowType
		{
			ShowEvent = 0,
			ShowSystem = 1
		}

		private ShowType _showType;

		private Dictionary<ShowType, uint> _selectIdDict;

		private List<Bulletin> _showList;

		public BulletinBoardDialogContext(ShowType showType = ShowType.ShowEvent)
		{
			config = new ContextPattern
			{
				contextName = "BulletinBoardDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/BulletinBoard/BulletinBoardDialog"
			};
			_showType = showType;
			_selectIdDict = new Dictionary<ShowType, uint>();
			_selectIdDict.Add(ShowType.ShowEvent, 0u);
			_selectIdDict.Add(ShowType.ShowSystem, 0u);
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 138)
			{
				return OnGetBulletinRsp(pkt.getData<GetBulletinRsp>());
			}
			return false;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.DownloadResAssetSucc)
			{
				return OnDownloadResAssetSucc();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/TabBtns/EventBtn").GetComponent<Button>(), ShowEventBulletinList);
			BindViewCallback(base.view.transform.Find("Dialog/TabBtns/SysBtn").GetComponent<Button>(), ShowSystemBulletinList);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Dialog/Content/OneNotice/ScrollView").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/TitleBtnList/ScrollView").gameObject.SetActive(false);
			ShowBulletinListByType(_showType);
			return false;
		}

		private bool OnGetBulletinRsp(GetBulletinRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				ShowBulletinListByType(_showType);
			}
			return false;
		}

		private bool OnDownloadResAssetSucc()
		{
			ShowBulletinListByType(_showType);
			return false;
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Close();
		}

		public void Close()
		{
			SuperDebug.VeryImportantAssert(base.view != null, "view is null!");
			Destroy();
		}

		private void ShowEventBulletinList()
		{
			ShowBulletinListByType(ShowType.ShowEvent);
		}

		private void ShowSystemBulletinList()
		{
			ShowBulletinListByType(ShowType.ShowSystem);
		}

		private void ShowBulletinListByType(ShowType type)
		{
			_showType = type;
			_showList = ((type != ShowType.ShowEvent) ? Singleton<BulletinModule>.Instance.SystemBulletinList : Singleton<BulletinModule>.Instance.EventBulletinList);
			base.view.transform.Find("Dialog/Content/TitleBtnList/ScrollView").GetComponent<MonoGridScroller>().Init(OnScrollerChange, _showList.Count);
			if (_showList.Count > 0)
			{
				base.view.transform.Find("Dialog/Content/OneNotice/ScrollView").gameObject.SetActive(true);
				base.view.transform.Find("Dialog/Content/TitleBtnList/ScrollView").gameObject.SetActive(true);
				ShowBulletinById((_selectIdDict[_showType] != 0) ? _selectIdDict[_showType] : _showList[0].id);
			}
			else
			{
				base.view.transform.Find("Dialog/Content/OneNotice/ScrollView").gameObject.SetActive(false);
				base.view.transform.Find("Dialog/Content/TitleBtnList/ScrollView").gameObject.SetActive(false);
			}
			SetActiveTabBtn(type == ShowType.ShowEvent, base.view.transform.Find("Dialog/TabBtns/EventBtn").GetComponent<Button>());
			SetActiveTabBtn(type == ShowType.ShowSystem, base.view.transform.Find("Dialog/TabBtns/SysBtn").GetComponent<Button>());
			if (Singleton<BulletinModule>.Instance.HasNewBulletinsByType((uint)_showType))
			{
				Singleton<BulletinModule>.Instance.SetBulletinsOldByShowType((uint)_showType);
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.BulletinPopUpUpdate));
			}
			SetupPopUp();
		}

		private void SetActiveTabBtn(bool active, Button btn)
		{
			btn.GetComponent<Image>().color = ((!active) ? MiscData.GetColor("Blue") : Color.white);
			btn.transform.Find("Text").GetComponent<Text>().color = ((!active) ? Color.white : MiscData.GetColor("Black"));
			btn.interactable = !active;
		}

		private void OnScrollerChange(Transform trans, int index)
		{
			bool isSelected = _showList[index].id == _selectIdDict[_showType];
			trans.GetComponent<MonoBulletinTitleButton>().SetupView(_showList[index], isSelected, ShowBulletinById);
		}

		private void ShowBulletinById(uint id)
		{
			Bulletin val = Singleton<BulletinModule>.Instance.TryGetBulletinByID(id);
			if (val != null && val.type == (uint)_showType)
			{
				_selectIdDict[_showType] = id;
				base.view.transform.Find("Dialog/Content/TitleBtnList/ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
				Transform transform = base.view.transform.Find("Dialog/Content/OneNotice/ScrollView/Content");
				string banner_path = val.banner_path;
				Image component = transform.Find("Image/Pics").GetComponent<Image>();
				bool flag = !string.IsNullOrEmpty(banner_path) && UIUtil.TrySetupEventSprite(component, banner_path);
				transform.Find("Image").gameObject.SetActive(flag);
				if (flag)
				{
					LayoutElement component2 = transform.Find("Image").GetComponent<LayoutElement>();
					Rect rect = transform.Find("Image/Pics").GetComponent<Image>().sprite.rect;
					component2.preferredHeight = rect.height;
					component2.preferredWidth = rect.width;
				}
				transform.Find("Title/Text").GetComponent<Text>().text = val.title;
				string event_date_str = val.event_date_str;
				if (event_date_str == string.Empty)
				{
					transform.Find("Title/Time").gameObject.SetActive(false);
				}
				else
				{
					transform.Find("Title/Time").gameObject.SetActive(true);
					transform.Find("Title/Time").GetComponent<Text>().text = val.event_date_str;
				}
				transform.Find("Body").GetComponent<MonoBulletinBody>().SetupView(UIUtil.ProcessStrWithNewLine(val.content));
				base.view.transform.Find("Dialog/Content/OneNotice/ScrollView").GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
			}
		}

		private void SetupPopUp()
		{
			bool active = Singleton<BulletinModule>.Instance.HasNewBulletinsByType(0u);
			base.view.transform.Find("Dialog/TabBtns/EventBtn/PopUp").gameObject.SetActive(active);
			bool active2 = Singleton<BulletinModule>.Instance.HasNewBulletinsByType(1u);
			base.view.transform.Find("Dialog/TabBtns/SysBtn/PopUp").gameObject.SetActive(active2);
		}
	}
}
