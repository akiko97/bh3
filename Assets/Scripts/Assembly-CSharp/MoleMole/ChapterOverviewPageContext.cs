using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class ChapterOverviewPageContext : BasePageContext
	{
		private const string CHAPTER_BUTTON_PREFAB = "UI/Menus/Widget/Map/ChapterButton";

		private const string ACTIVITY_BUTTON_PREFAB = "UI/Menus/Widget/Map/ActivityButton";

		public const string EVENT_TAB = "Event";

		public const string SPECIAL_STORY_TAB = "SpecialStory";

		public const string MAIN_STORY_TAB = "MainStory";

		private string _showingTab;

		private WeekDayActivityDataItem _selectedActivityData;

		private ChapterDataItem _selectedChapterData;

		private bool _noSpecialStory = true;

		public ChapterOverviewPageContext(string tab = "")
		{
			InitChapterOverviewPageContext();
			_showingTab = ((!string.IsNullOrEmpty(tab)) ? tab : "MainStory");
		}

		public ChapterOverviewPageContext(WeekDayActivityDataItem acitivtyData)
		{
			InitChapterOverviewPageContext();
			_selectedActivityData = acitivtyData;
			_showingTab = "Event";
		}

		public ChapterOverviewPageContext(ChapterDataItem chapterData)
		{
			InitChapterOverviewPageContext();
			_selectedChapterData = chapterData;
			_showingTab = "MainStory";
		}

		private void InitChapterOverviewPageContext()
		{
			config = new ContextPattern
			{
				contextName = "ChapterOverviewPageContext",
				viewPrefabPath = "UI/Menus/Page/Map/ChapterOverviewPage",
				cacheType = ViewCacheType.DontCache
			};
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 42:
				return OnGetStageDataRsp(pkt.getData<GetStageDataRsp>());
			case 140:
				return OnGetEndlessDataRsp(pkt.getData<GetEndlessDataRsp>());
			default:
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.RequestEnterEndlessActivity)
			{
				return OnRequestEnterEndlessActivity();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Content/Event/Packed/PackBtn").GetComponent<Button>(), OnEventPackBtnClick);
			BindViewCallback(base.view.transform.Find("Content/SpecialStory/Packed/PackBtn").GetComponent<Button>(), OnSpecialStoryPackBtnClick);
			BindViewCallback(base.view.transform.Find("Content/MainStory/Packed/PackBtn").GetComponent<Button>(), OnMainStoryPackBtnClick);
		}

		protected override bool SetupView()
		{
			if (Singleton<LevelModule>.Instance.AllChapterList.Count == 0)
			{
				return false;
			}
			SetupMainStory();
			SetupSpecialStory();
			SetupEvent();
			Show(_showingTab);
			return false;
		}

		public override void OnLandedFromBackPage()
		{
			Show(_showingTab);
			base.OnLandedFromBackPage();
		}

		public void OnEventPackBtnClick()
		{
			Show("Event");
		}

		public void OnSpecialStoryPackBtnClick()
		{
			if (_noSpecialStory)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_SpecialStoryLock")));
			}
			else
			{
				Show("SpecialStory");
			}
		}

		public void OnMainStoryPackBtnClick()
		{
			Show("MainStory");
		}

		private void Show(string showTab)
		{
			string[] array = new string[3] { "Event", "SpecialStory", "MainStory" };
			string[] array2 = array;
			foreach (string text in array2)
			{
				GameObject gameObject = base.view.transform.Find("Content/" + text).gameObject;
				if (showTab == text)
				{
					gameObject.GetComponent<Animator>().SetTrigger("ExpandTrigger");
				}
				else if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ExpandedAnim"))
				{
					gameObject.GetComponent<Animator>().SetTrigger("PackTrigger");
				}
			}
			_showingTab = showTab;
		}

		public bool OnGetStageDataRsp(GetStageDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				if (_selectedChapterData != null)
				{
					SetupMainStory();
				}
				else if (_selectedActivityData != null)
				{
					SetupEvent();
				}
				else
				{
					Singleton<MainUIManager>.Instance.ShowPage(new ChapterOverviewPageContext(string.Empty));
				}
			}
			return false;
		}

		private bool OnGetEndlessDataRsp(GetEndlessDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				if (rsp.cur_progress_avatar_id_list.Count > 0 || rsp.cur_progress_item_id_list.Count > 0)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
					{
						title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
						desc = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessGoToBattleDirrectly"),
						buttonCallBack = delegate(bool confirmed)
						{
							if (confirmed)
							{
								Singleton<MainUIManager>.Instance.ShowPage(new EndlessMainPageContext());
								Singleton<MainUIManager>.Instance.ShowPage(new EndlessPreparePageContext(true));
							}
						}
					});
				}
				else
				{
					Singleton<MainUIManager>.Instance.ShowPage(new EndlessMainPageContext());
				}
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)));
			}
			return false;
		}

		private bool OnRequestEnterEndlessActivity()
		{
			if (Singleton<EndlessModule>.Instance == null)
			{
				Singleton<EndlessModule>.Create();
			}
			Singleton<NetworkManager>.Instance.RequestEndlessData();
			Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(140));
			return false;
		}

		private void SetupMainStory()
		{
			MonoChapterScroller component = base.view.transform.Find("Content/MainStory/Expanded/ScrollerView").GetComponent<MonoChapterScroller>();
			component.transform.Find("Content").GetComponent<GridLayoutGroup>().enabled = true;
			List<ChapterDataItem> list = Singleton<LevelModule>.Instance.AllChapterList.FindAll((ChapterDataItem x) => x.ChapterType == ChapterDataItem.ChpaterType.MainStory);
			base.view.transform.Find("Content/MainStory/Packed/PackBtn").GetComponent<Button>().interactable = list.Count > 0;
			SetupMainStoryContent(component, list);
		}

		private void SetupSpecialStory()
		{
			MonoChapterScroller component = base.view.transform.Find("Content/SpecialStory/Expanded/ScrollerView").GetComponent<MonoChapterScroller>();
			List<ChapterDataItem> list = Singleton<LevelModule>.Instance.AllChapterList.FindAll((ChapterDataItem x) => x.ChapterType == ChapterDataItem.ChpaterType.SpecialStory);
			_noSpecialStory = list.Count == 0;
			base.view.transform.Find("Content/SpecialStory/Packed/PackBtn").GetComponent<Button>().interactable = true;
			SetupMainStoryContent(component, list);
		}

		private void SetupEvent()
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Invalid comparison between Unknown and I4
			MonoChapterScroller component = base.view.transform.Find("Content/Event/Expanded/ScrollerView").GetComponent<MonoChapterScroller>();
			component.transform.Find("Content").GetComponent<GridLayoutGroup>().enabled = true;
			List<ActivityDataItemBase> list = new List<ActivityDataItemBase>();
			foreach (WeekDayActivityDataItem allWeekDayActivity in Singleton<LevelModule>.Instance.AllWeekDayActivityList)
			{
				if (allWeekDayActivity.GetStatus() != ActivityDataItemBase.Status.Unavailable && ((int)allWeekDayActivity.GetActivityType() != 3 || allWeekDayActivity.GetStatus() == ActivityDataItemBase.Status.InProgress))
				{
					list.Add(allWeekDayActivity);
				}
			}
			list.Add(EndlessActivityDataItem.GetInstance());
			base.view.transform.Find("Content/Event/Packed/PackBtn").GetComponent<Button>().interactable = list.Count > 0;
			SetupEventContent(component, list);
		}

		private void SetupMainStoryContent(MonoChapterScroller scroller, List<ChapterDataItem> list)
		{
			Transform content = scroller.content;
			content.DestroyChildren();
			if (list.Count == 0)
			{
				return;
			}
			int initCenterIndex = 0;
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Map/ChapterButton"));
				gameObject.name = "ChapterButton_" + (i + 1);
				gameObject.GetComponent<MonoChapterButton>().SetupView(list[i]);
				gameObject.transform.SetParent(content, false);
				if (_selectedChapterData != null)
				{
					if (list[i] == _selectedChapterData)
					{
						initCenterIndex = i;
					}
				}
				else if (list[i].Unlocked)
				{
					initCenterIndex = i;
				}
			}
			scroller.Init(initCenterIndex, list.Count, OnChangeSelectChapter);
		}

		private void SetupEventContent(MonoChapterScroller scroller, List<ActivityDataItemBase> list)
		{
			Transform content = scroller.content;
			content.DestroyChildren();
			if (list.Count == 0)
			{
				return;
			}
			int initCenterIndex = 0;
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Map/ActivityButton"));
				gameObject.name = "ChapterButton_" + (i + 1);
				gameObject.GetComponent<MonoActivityEntryButton>().SetupView(list[i]);
				gameObject.transform.SetParent(content, false);
				if (_selectedActivityData != null)
				{
					if (_selectedActivityData == list[i])
					{
						initCenterIndex = i;
					}
				}
				else if (list[i].GetStatus() == ActivityDataItemBase.Status.InProgress)
				{
					initCenterIndex = i;
				}
			}
			scroller.Init(initCenterIndex, list.Count, OnChangeSelectActivity);
		}

		private void OnChangeSelectChapter(int index)
		{
			List<ChapterDataItem> list = Singleton<LevelModule>.Instance.AllChapterList.FindAll((ChapterDataItem x) => x.ChapterType == ChapterDataItem.ChpaterType.MainStory);
			if (index < list.Count)
			{
				_selectedChapterData = list[index];
				_selectedActivityData = null;
			}
		}

		private void OnChangeSelectActivity(int index)
		{
			List<WeekDayActivityDataItem> list = new List<WeekDayActivityDataItem>();
			foreach (WeekDayActivityDataItem allWeekDayActivity in Singleton<LevelModule>.Instance.AllWeekDayActivityList)
			{
				if (allWeekDayActivity.GetStatus() != ActivityDataItemBase.Status.Unavailable)
				{
					list.Add(allWeekDayActivity);
				}
			}
			if (index < list.Count)
			{
				_selectedActivityData = list[index];
				_selectedChapterData = null;
			}
		}
	}
}
