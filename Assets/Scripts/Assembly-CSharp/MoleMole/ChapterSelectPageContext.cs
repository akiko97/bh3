using System;
using System.Collections.Generic;
using System.Linq;
using MoleMole.Config;
using proto;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MoleMole
{
    // Token: 0x020008AA RID: 2218
    public class ChapterSelectPageContext : BasePageContext
    {
        // Token: 0x0600372B RID: 14123 RVA: 0x0012A024 File Offset: 0x00128224
        public ChapterSelectPageContext(ChapterDataItem chapter = null)
        {
            this.config = new ContextPattern
            {
                contextName = "ChapterSelectPageContext",
                viewPrefabPath = "UI/Menus/Page/Map/ChapterSelectPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            this.findViewSavedInScene = true;
            this.InitData(chapter);
            this.SetNewUnlockLevelData();
        }

        // Token: 0x0600372C RID: 14124 RVA: 0x0012A080 File Offset: 0x00128280
        public ChapterSelectPageContext(WeekDayActivityDataItem weekDayActivity)
        {
            this.config = new ContextPattern
            {
                contextName = "ChapterSelectPageContext",
                viewPrefabPath = "UI/Menus/Page/Map/ChapterSelectPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            this.findViewSavedInScene = true;
            this._chapterType = StageType.STAGE_WEEK_DAY;
            this._weekDayActivityData = weekDayActivity;
        }

        // Token: 0x0600372D RID: 14125 RVA: 0x0012A0E0 File Offset: 0x001282E0
        public ChapterSelectPageContext(LevelDataItem levelData)
        {
            this.config = new ContextPattern
            {
                contextName = "ChapterSelectPageContext",
                viewPrefabPath = "UI/Menus/Page/Map/ChapterSelectPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            this.findViewSavedInScene = true;
            this._toShowLevelData = levelData;
            this._justShowLevelDetail = this._toShowLevelData != null;
            this._chapterType = levelData.LevelType;
            switch (this._chapterType)
            {
                case StageType.STAGE_STORY:
                    this.chapter = Singleton<LevelModule>.Instance.GetChapterById(levelData.ChapterID);
                    this.difficulty = levelData.Diffculty;
                    this._showActIndex = new ActDataItem(levelData.ActID).actIndex;
                    break;
                case StageType.STAGE_WEEK_DAY:
                case StageType.STAGE_BEFALL:
                    this._weekDayActivityData = Singleton<LevelModule>.Instance.GetWeekDayActivityByID(levelData.ActID);
                    break;
            }
        }

        // Token: 0x0600372E RID: 14126 RVA: 0x0012A1CC File Offset: 0x001283CC
        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.SetLevelDifficulty)
            {
                return this.OnSetupDifficultyNotify((LevelDiffculty)((int)ntf.body));
            }
            if (ntf.type == NotifyTypes.StageEnd)
            {
                return this.OnStageEndNotify((bool)ntf.body);
            }
            if (ntf.type == NotifyTypes.RefreshChapterSelectPage)
            {
                return this.OnRefreshChapterSelectPage();
            }
            return ntf.type == NotifyTypes.ActivtyShopScheduleChange && this.ShowActivityShopEntry();
        }

        // Token: 0x0600372F RID: 14127 RVA: 0x0012A240 File Offset: 0x00128440
        public override bool OnPacket(NetPacketV1 pkt)
        {
            ushort cmdId = pkt.getCmdId();
            if (cmdId == 46)
            {
                return this.OnStageEndRsp(pkt.getData<StageEndRsp>());
            }
            return cmdId == 126 && this.OnGetWeekDayActivityDataRsp(pkt.getData<GetWeekDayActivityDataRsp>());
        }

        // Token: 0x06003730 RID: 14128 RVA: 0x0012A280 File Offset: 0x00128480
        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("WorldMapBtn").GetComponent<Button>(), new UnityAction(this.OnWorldMapBtnClick));
            base.BindViewCallback(base.view.transform.Find("Title/DescPanel/IconEx/Btn").GetComponent<Button>(), new UnityAction(this.JumpToNuclearActivity));
            base.BindViewCallback(base.view.transform.Find("EventShopBtn").GetComponent<Button>(), new UnityAction(this.OnActivityShopBtnClick));
        }

        // Token: 0x06003731 RID: 14129 RVA: 0x0012A314 File Offset: 0x00128514
        protected override bool SetupView()
        {
            if (Mathf.Approximately(this._actScrollerLerpSpeed, 0f))
            {
                this._actScrollerLerpSpeed = base.view.transform.Find("ActPanel/ScrollerView").GetComponent<MonoActScroller>().lerpSpeed;
                this._actScrollerStopLerpThreshold = base.view.transform.Find("ActPanel/ScrollerView").GetComponent<MonoActScroller>().stopLerpThreshold;
            }
            switch (this._chapterType)
            {
                case StageType.STAGE_STORY:
                    base.view.transform.Find("Path/2").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_Story", new object[0]) + " >";
                    base.view.transform.Find("Path/3").GetComponent<Text>().text = this.chapter.Title;
                    base.view.transform.Find("Title/DescPanel/Desc").GetComponent<Text>().text = this.chapter.Title;
                    base.view.transform.Find("Title/DescPanel/Desc").GetComponent<TypewriterEffect>().RestartRead();
                    base.view.transform.Find("DifficultySelect").gameObject.SetActive(true);
                    base.view.transform.Find("DifficultySelect").GetComponent<MonoLevelDifficultyPanel>().Init(this.difficulty, this.chapter);
                    base.view.transform.Find("InfoPanel").gameObject.SetActive(false);
                    base.view.transform.Find("Title/DescPanel/IconEx").gameObject.SetActive(false);
                    base.view.transform.Find("EventShopBtn").gameObject.SetActive(false);
                    this.SetupChallengeNum();
                    if (!this._justShowLevelDetail)
                    {
                        this.SaveChapterStatus();
                    }
                    break;
                case StageType.STAGE_WEEK_DAY:
                case StageType.STAGE_BEFALL:
                    this.SetupActivityView();
                    break;
            }
            base.view.transform.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn", false, null);
            base.view.transform.Find("ChallengeNum").gameObject.SetActive(this._chapterType == StageType.STAGE_STORY);
            this.SetRectMaskDirty();
            Singleton<LevelModule>.Instance.RetrySendLevelEndReq();
            return false;
        }

        // Token: 0x06003732 RID: 14130 RVA: 0x00023538 File Offset: 0x00021738
        private bool OnStageEndRsp(StageEndRsp rsp)
        {
            Singleton<LevelModule>.Instance.HandleStageEndRspForRetry(rsp);
            this.SetupView();
            return false;
        }

        // Token: 0x06003733 RID: 14131 RVA: 0x0012A578 File Offset: 0x00128778
        public override void BackPage()
        {
            if (this._levelDetailContext != null && !this._justShowLevelDetail)
            {
                this._levelDetailContext.Destroy();
                this._levelDetailContext = null;
                base.view.transform.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn", false, null);
            }
            else
            {
                base.BackPage();
            }
        }

        // Token: 0x06003734 RID: 14132 RVA: 0x0002354D File Offset: 0x0002174D
        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            base.view.transform.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn", false, null);
        }

        // Token: 0x06003735 RID: 14133 RVA: 0x00023571 File Offset: 0x00021771
        public void OnDoLevelBegin()
        {
            if (this._levelDetailContext != null)
            {
                this._levelDetailContext.Destroy();
                this._levelDetailContext = null;
            }
            this._toShowLevelData = null;
            this._justShowLevelDetail = false;
        }

        // Token: 0x06003736 RID: 14134 RVA: 0x0012A5D4 File Offset: 0x001287D4
        private void OnWorldMapBtnClick()
        {
            switch (this._chapterType)
            {
                case StageType.STAGE_STORY:
                    Singleton<MainUIManager>.Instance.ShowPage(new ChapterOverviewPageContext(this.chapter), UIType.Page);
                    break;
                case StageType.STAGE_WEEK_DAY:
                case StageType.STAGE_BEFALL:
                    Singleton<MainUIManager>.Instance.ShowPage(new ChapterOverviewPageContext(this._weekDayActivityData), UIType.Page);
                    break;
            }
        }

        // Token: 0x06003737 RID: 14135 RVA: 0x0002359E File Offset: 0x0002179E
        private bool OnGetWeekDayActivityDataRsp(GetWeekDayActivityDataRsp rsp)
        {
            if (this._weekDayActivityData != null)
            {
                this.SetupActivityView();
            }
            return false;
        }

        // Token: 0x06003738 RID: 14136 RVA: 0x0012A638 File Offset: 0x00128838
        private bool OnSetupDifficultyNotify(LevelDiffculty difficulty)
        {
            if (this._chapterType != StageType.STAGE_STORY)
            {
                return false;
            }
            this.difficulty = difficulty;
            this.CheckActIndex();
            this.SetupLevels();
            this.SetupChallengeNum();
            if (this._justShowLevelDetail)
            {
                this.OnLevelClick(this._toShowLevelData);
            }
            this.SetRectMaskDirty();
            return false;
        }

        // Token: 0x06003739 RID: 14137 RVA: 0x0012A68C File Offset: 0x0012888C
        private bool OnStageEndNotify(bool shouldCreateNuclearActivityTips)
        {
            this.SetNewUnlockLevelData();
            if (this._chapterType == StageType.STAGE_STORY && this._newUnlockLevelDataList.Count > 0)
            {
                MonoActScroller component = base.view.transform.Find("ActPanel/ScrollerView").GetComponent<MonoActScroller>();
                component.onLerpEndCallBack = new Action(this.OnActLerpOnWhenNeedPlayNewUnlockLevel);
                component.lerpSpeed /= this._actScrollerSpeedDownRatio;
                component.stopLerpThreshold *= this._actScrollerSpeedDownRatio * 2f;
                int actIndex = new ActDataItem(this._newUnlockLevelDataList[0].ActID).actIndex;
                this._shouldChangeActIndex = actIndex != this._showActIndex;
                if (actIndex != this._showActIndex)
                {
                    if (this._newUnlockLevelActDelayTimer != null)
                    {
                        this._newUnlockLevelActDelayTimer.Destroy();
                    }
                    this._newUnlockLevelActDelayTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.4f, 0f);
                    this._newUnlockLevelActDelayTimer.timeUpCallback = new Action(this.ActDelayTimerTimeUpCallBack);
                    this._needLerpAfterInitLevels = true;
                }
                else
                {
                    this._needLerpAfterInitLevels = false;
                    this.ActDelayTimerTimeUpCallBack();
                }
            }
            if (shouldCreateNuclearActivityTips)
            {
                this.CreateNuclearActivityTips();
            }
            this.SetRectMaskDirty();
            return false;
        }

        // Token: 0x0600373A RID: 14138 RVA: 0x000235B2 File Offset: 0x000217B2
        private bool OnRefreshChapterSelectPage()
        {
            this.SetupView();
            return false;
        }

        // Token: 0x0600373B RID: 14139 RVA: 0x0012A7C8 File Offset: 0x001289C8
        private void CreateNuclearActivityTips()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralConfirmDialogContext
            {
                type = GeneralConfirmDialogContext.ButtonType.SingleButton,
                desc = LocalizationGeneralLogic.GetText("Menu_NuclearOpen", new object[0]),
                buttonCallBack = delegate (bool confirmed)
                {
                    if (confirmed)
                    {
                        this.JumpToNuclearActivity();
                    }
                }
            }, UIType.Any);
        }

        // Token: 0x0600373C RID: 14140 RVA: 0x0012A818 File Offset: 0x00128A18
        private void JumpToNuclearActivity()
        {
            if (this._weekDayActivityData == null)
            {
                return;
            }
            SeriesDataItem weekDaySeriesByActivityID = Singleton<LevelModule>.Instance.GetWeekDaySeriesByActivityID(this._weekDayActivityData.GetActivityID());
            WeekDayActivityDataItem weekDayActivityDataItem = weekDaySeriesByActivityID.weekActivityList.Find((WeekDayActivityDataItem x) => x.GetActivityType() == ActivityType.ACTIVITY_NUCLEAR && x.GetStatus() == ActivityDataItemBase.Status.InProgress);
            if (weekDayActivityDataItem != null)
            {
                this._weekDayActivityData = weekDayActivityDataItem;
                this.SetupView();
            }
        }

        // Token: 0x0600373D RID: 14141 RVA: 0x0012A884 File Offset: 0x00128A84
        private void OnActivityShopBtnClick()
        {
            StoreDataItem storeDataByType = Singleton<StoreModule>.Instance.GetStoreDataByType(UIShopType.SHOP_ACTIVITY);
            if (storeDataByType != null && storeDataByType.isOpen)
            {
                Singleton<MainUIManager>.Instance.ShowPage(new ShopPageContext(UIShopType.SHOP_ACTIVITY), UIType.Page);
            }
        }

        // Token: 0x0600373E RID: 14142 RVA: 0x0012A8C0 File Offset: 0x00128AC0
        private void SetNewUnlockLevelData()
        {
            if (this._chapterType == StageType.STAGE_STORY)
            {
                this._newUnlockLevelDataList = new List<LevelDataItem>();
                List<LevelDataItem> levelList = this.chapter.GetLevelList(this.difficulty);
                levelList.Sort((LevelDataItem lo, LevelDataItem ro) => lo.levelId - ro.levelId);
                foreach (LevelDataItem levelDataItem in levelList)
                {
                    if (levelDataItem.levelId != 10101 && levelDataItem.status != StageStatus.STAGE_LOCKED && Singleton<MiHoYoGameData>.Instance.LocalData.NeedPlayLevelAnimationSet.Contains(levelDataItem.levelId))
                    {
                        this._newUnlockLevelDataList.Add(levelDataItem);
                    }
                }
            }
        }

        // Token: 0x0600373F RID: 14143 RVA: 0x0012A9A4 File Offset: 0x00128BA4
        private void OnLevelClick(LevelDataItem levelData)
        {
            if (levelData.status == StageStatus.STAGE_LOCKED)
            {
                return;
            }
            LevelDetailDialogContextV2 levelDetailDialogContextV = new LevelDetailDialogContextV2(levelData, levelData.Diffculty);
            Singleton<MainUIManager>.Instance.ShowDialog(levelDetailDialogContextV, UIType.SpecialDialog);
            this._levelDetailContext = levelDetailDialogContextV;
            if (levelData.LevelType == StageType.STAGE_STORY)
            {
                this._showActIndex = new ActDataItem(levelData.ActID).actIndex;
            }
            levelData.isNewLevel = false;
            if (levelData.LevelType == StageType.STAGE_STORY)
            {
                Singleton<MiHoYoGameData>.Instance.LocalData.NeedPlayLevelAnimationSet.Remove(levelData.levelId);
                Singleton<MiHoYoGameData>.Instance.Save();
            }
        }

        // Token: 0x06003740 RID: 14144 RVA: 0x0012AA38 File Offset: 0x00128C38
        private void SaveChapterStatus()
        {
            if (this._chapterType == StageType.STAGE_STORY)
            {
                Singleton<MiHoYoGameData>.Instance.LocalData.LastChapterID = this.chapter.chapterId;
                Singleton<MiHoYoGameData>.Instance.LocalData.LastActIndex = this._actScroller.centerIndex;
                Singleton<MiHoYoGameData>.Instance.LocalData.LastDifficulty = this.difficulty;
                Singleton<MiHoYoGameData>.Instance.Save();
            }
            else if (this._chapterType == StageType.STAGE_WEEK_DAY || this._chapterType == StageType.STAGE_BEFALL || this._chapterType == StageType.STAGE_NUCLEAR)
            {
                this._weekDayActivityData = this._actScroller.GetCenterTransform().GetComponent<MonoActButton>().GetWeekDayActivityData();
                this.ShowActivityShopEntry();
            }
        }

        // Token: 0x06003741 RID: 14145 RVA: 0x0012AAF0 File Offset: 0x00128CF0
        private void ClearChildItems(Transform target)
        {
            if (target == null)
            {
                return;
            }
            foreach (object obj in target)
            {
                Transform transform = (Transform)obj;
                if (!(transform == null))
                {
                    MonoItemStatus component = transform.GetComponent<MonoItemStatus>();
                    if (component != null)
                    {
                        component.isValid = false;
                    }
                    global::UnityEngine.Object.Destroy(transform.gameObject);
                }
            }
        }

        // Token: 0x06003742 RID: 14146 RVA: 0x000235BC File Offset: 0x000217BC
        private void ClearLevelsContent()
        {
            this.ClearChildItems(base.view.transform.Find("LevelPanel/ScrollerView/Content"));
            this.ClearChildItems(base.view.transform.Find("ActPanel/ScrollerView/Content"));
        }

        // Token: 0x06003743 RID: 14147 RVA: 0x0012AB8C File Offset: 0x00128D8C
        private void SetupLevels()
        {
            this.levelTransDict = new Dictionary<LevelDataItem, Transform>();
            Transform transform = base.view.transform.Find("LevelPanel/ScrollerView");
            Transform transform2 = base.view.transform.Find("ActPanel/ScrollerView");
            MonoActScroller component = transform2.GetComponent<MonoActScroller>();
            this._actScroller = component;
            this.ClearLevelsContent();
            Dictionary<int, List<LevelDataItem>> showLevelOfActs = this.GetShowLevelOfActs();
            if (showLevelOfActs.Count <= 0)
            {
                transform.gameObject.SetActive(false);
                transform2.gameObject.SetActive(false);
                return;
            }
            transform.gameObject.SetActive(true);
            transform2.gameObject.SetActive(true);
            foreach (LevelDataItem levelDataItem in this.chapter.GetAllLevelList())
            {
                if (Singleton<LevelModule>.Instance.GetLevelDropItemIDList(levelDataItem.levelId) == null)
                {
                    Singleton<NetworkManager>.Instance.RequestChapterDropList(this.chapter);
                    break;
                }
            }
            int totalFinishedChanllengeNum = this.chapter.GetTotalFinishedChanllengeNum(this.difficulty);
            List<int> list = showLevelOfActs.Keys.ToList<int>();
            list.Sort();
            foreach (int num in list)
            {
                ActDataItem actDataItem = new ActDataItem(num);
                Transform transform3 = global::UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Map/Act", BundleType.RESOURCE_FILE)).transform;
                transform3.SetParent(transform2.Find("Content"), false);
                List<LevelDataItem> list2 = showLevelOfActs[num];
                transform3.GetComponent<MonoActButton>().SetupActView(actDataItem, list2, transform, new LevelBtnClickCallBack(this.OnLevelClick), base.view.transform.Find("ActivityBG"), this.levelTransDict, totalFinishedChanllengeNum);
            }
            if (this._newUnlockLevelDataList != null)
            {
                transform2.GetComponent<MonoLevelScroller>().onLerpEndCallBack = new Action(this.OnActLerpOnWhenNeedPlayNewUnlockLevel);
            }
            transform.GetComponent<MonoLevelScroller>().InitLevelPanels(this._showActIndex, list.Count, null, this._needLerpAfterInitLevels);
            transform2.GetComponent<MonoActScroller>().InitActs(this._showActIndex, list.Count, new Action(this.SaveChapterStatus), this._needLerpAfterInitLevels);
            this._needLerpAfterInitLevels = false;
        }

        // Token: 0x06003744 RID: 14148 RVA: 0x0012ADF4 File Offset: 0x00128FF4
        private Dictionary<int, List<LevelDataItem>> GetShowLevelOfActs()
        {
            Dictionary<int, List<LevelDataItem>> levelOfActs = this.chapter.GetLevelOfActs(this.difficulty);
            foreach (int num in levelOfActs.Keys.ToArray<int>())
            {
                ActDataItem actDataItem = new ActDataItem(num);
                if (actDataItem.actType == ActDataItem.ActType.Extra)
                {
                    if (levelOfActs[num].TrueForAll((LevelDataItem x) => x.status == StageStatus.STAGE_LOCKED))
                    {
                        levelOfActs.Remove(num);
                    }
                }
            }
            return levelOfActs;
        }

        // Token: 0x06003745 RID: 14149 RVA: 0x000235BC File Offset: 0x000217BC
        private void ClearActivityContent()
        {
            this.ClearChildItems(base.view.transform.Find("LevelPanel/ScrollerView/Content"));
            this.ClearChildItems(base.view.transform.Find("ActPanel/ScrollerView/Content"));
        }

        // Token: 0x06003746 RID: 14150 RVA: 0x0012AE84 File Offset: 0x00129084
        private void SetupActivityView()
        {
            this.levelTransDict = new Dictionary<LevelDataItem, Transform>();
            bool flag = false;
            SeriesDataItem weekDaySeriesByActivityID = Singleton<LevelModule>.Instance.GetWeekDaySeriesByActivityID(this._weekDayActivityData.GetActivityID());
            List<WeekDayActivityDataItem> showActivityListBySeries = this.GetShowActivityListBySeries(weekDaySeriesByActivityID, out flag);
            for (int i = 0; i < showActivityListBySeries.Count; i++)
            {
                if (showActivityListBySeries[i] == this._weekDayActivityData)
                {
                    this._showActIndex = i;
                    break;
                }
            }
            this._showActIndex = Mathf.Clamp(this._showActIndex, 0, showActivityListBySeries.Count - 1);
            base.view.transform.Find("Path/2").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_Event", new object[0]) + " >";
            base.view.transform.Find("Path/3").GetComponent<Text>().text = weekDaySeriesByActivityID.title;
            base.view.transform.Find("DifficultySelect").gameObject.SetActive(false);
            base.view.transform.Find("InfoPanel").gameObject.SetActive(true);
            base.view.transform.Find("Title/DescPanel/Desc").GetComponent<Text>().text = weekDaySeriesByActivityID.title;
            base.view.transform.Find("Title/DescPanel/Desc").GetComponent<TypewriterEffect>().RestartRead();
            base.view.transform.Find("Title/DescPanel/IconEx").gameObject.SetActive(flag);
            base.view.transform.Find("ActivityBG/ExBG").gameObject.SetActive(false);
            Transform transform = base.view.transform.Find("LevelPanel/ScrollerView");
            Transform transform2 = base.view.transform.Find("ActPanel/ScrollerView");
            MonoActScroller component = transform2.GetComponent<MonoActScroller>();
            this._actScroller = component;
            this.ClearActivityContent();
            if (showActivityListBySeries.Count <= 0)
            {
                transform.gameObject.SetActive(false);
                transform2.gameObject.SetActive(false);
                return;
            }
            transform.gameObject.SetActive(true);
            transform2.gameObject.SetActive(true);
            foreach (WeekDayActivityDataItem weekDayActivityDataItem in showActivityListBySeries)
            {
                Transform transform3 = global::UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Map/Act", BundleType.RESOURCE_FILE)).transform;
                transform3.SetParent(transform2.Find("Content"), false);
                List<LevelDataItem> weekDayActivityLevelsByID = Singleton<LevelModule>.Instance.GetWeekDayActivityLevelsByID(weekDayActivityDataItem.GetActivityID());
                transform3.GetComponent<MonoActButton>().SetupActivityView(weekDayActivityDataItem, base.view.transform.Find("InfoPanel").GetComponent<MonoActivityInfoPanel>(), weekDayActivityLevelsByID, transform, new LevelBtnClickCallBack(this.OnLevelClick), base.view.transform.Find("ActivityBG"), this.levelTransDict);
            }
            transform.GetComponent<MonoLevelScroller>().InitLevelPanels(this._showActIndex, showActivityListBySeries.Count, null, false);
            transform2.GetComponent<MonoActScroller>().InitActs(this._showActIndex, showActivityListBySeries.Count, new Action(this.SaveChapterStatus), false);
            if (this._justShowLevelDetail)
            {
                this.OnLevelClick(this._toShowLevelData);
            }
            this.SetRectMaskDirty();
            this.ShowActivityShopEntry();
        }

        // Token: 0x06003747 RID: 14151 RVA: 0x0012B1F0 File Offset: 0x001293F0
        private void InitData(ChapterDataItem chapter)
        {
            this._chapterType = StageType.STAGE_STORY;
            if (chapter == null)
            {
                int lastChapterID = Singleton<MiHoYoGameData>.Instance.LocalData.LastChapterID;
                this.chapter = Singleton<LevelModule>.Instance.GetChapterById(lastChapterID);
                this.difficulty = Singleton<MiHoYoGameData>.Instance.LocalData.LastDifficulty;
                this._showActIndex = Singleton<MiHoYoGameData>.Instance.LocalData.LastActIndex;
            }
            else
            {
                this.chapter = chapter;
                this.difficulty = Singleton<MiHoYoGameData>.Instance.LocalData.LastDifficulty;
                if (this.chapter.chapterId != Singleton<MiHoYoGameData>.Instance.LocalData.LastChapterID)
                {
                    this._showActIndex = 0;
                }
                else
                {
                    this._showActIndex = Singleton<MiHoYoGameData>.Instance.LocalData.LastActIndex;
                }
            }
        }

        // Token: 0x06003748 RID: 14152 RVA: 0x0012B2B8 File Offset: 0x001294B8
        private void CheckActIndex()
        {
            if (this._chapterType != StageType.STAGE_STORY)
            {
                return;
            }
            if (this.chapter.chapterId != Singleton<MiHoYoGameData>.Instance.LocalData.LastChapterID || this.difficulty != Singleton<MiHoYoGameData>.Instance.LocalData.LastDifficulty)
            {
                this._showActIndex = 0;
            }
        }

        // Token: 0x06003749 RID: 14153 RVA: 0x0012B314 File Offset: 0x00129514
        private void OnActLerpOnWhenNeedPlayNewUnlockLevel()
        {
            MonoActScroller component = base.view.transform.Find("ActPanel/ScrollerView").GetComponent<MonoActScroller>();
            component.lerpSpeed = this._actScrollerLerpSpeed;
            component.stopLerpThreshold = this._actScrollerStopLerpThreshold;
            if (this._chapterType != StageType.STAGE_STORY)
            {
                return;
            }
            if (this._newUnlockLevelDataList == null || this._newUnlockLevelDataList.Count < 1)
            {
                return;
            }
            if (this._newUnlockLevelAnimationTimer != null)
            {
                this._newUnlockLevelAnimationTimer.Destroy();
            }
            if (this._shouldChangeActIndex)
            {
                this.PlayNewUnlockLevelAnimation();
                this._shouldChangeActIndex = false;
            }
            else
            {
                this._newUnlockLevelAnimationTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.5f, 0f);
                this._newUnlockLevelAnimationTimer.timeUpCallback = new Action(this.PlayNewUnlockLevelAnimation);
                this._newUnlockLevelAnimationTimer.StartRun(false);
            }
        }

        // Token: 0x0600374A RID: 14154 RVA: 0x0012B3F4 File Offset: 0x001295F4
        private void PlayNewUnlockLevelAnimation()
        {
            if (this._newUnlockLevelDataList != null)
            {
                foreach (LevelDataItem levelDataItem in this._newUnlockLevelDataList)
                {
                    if (this.levelTransDict != null && this.levelTransDict.ContainsKey(levelDataItem) && this.levelTransDict[levelDataItem] != null)
                    {
                        MonoLevelView component = this.levelTransDict[levelDataItem].GetComponent<MonoLevelView>();
                        if (component)
                        {
                            component.PlayNewUnlockAnimation(0.5f);
                        }
                        Singleton<MiHoYoGameData>.Instance.LocalData.NeedPlayLevelAnimationSet.Remove(levelDataItem.levelId);
                    }
                }
                Singleton<MiHoYoGameData>.Instance.Save();
                this._newUnlockLevelDataList.Clear();
            }
            base.view.transform.Find("ActPanel/ScrollerView").GetComponent<MonoLevelScroller>().onLerpEndCallBack = null;
        }

        // Token: 0x0600374B RID: 14155 RVA: 0x0012B500 File Offset: 0x00129700
        private void ActDelayTimerTimeUpCallBack()
        {
            if (this._newUnlockLevelDataList.Count < 1)
            {
                return;
            }
            this._showActIndex = new ActDataItem(this._newUnlockLevelDataList[0].ActID).actIndex;
            this.SetupLevels();
            this.SetRectMaskDirty();
        }

        // Token: 0x0600374C RID: 14156 RVA: 0x0012B54C File Offset: 0x0012974C
        private void SetupChallengeNum()
        {
            base.view.transform.Find("ChallengeNum/Num").GetComponent<Text>().text = "x" + this.chapter.GetTotalFinishedChanllengeNum(this.difficulty);
        }

        // Token: 0x0600374D RID: 14157 RVA: 0x0012B598 File Offset: 0x00129798
        private List<WeekDayActivityDataItem> GetShowActivityListBySeries(SeriesDataItem seriesData, out bool hasInProgressNuclearActivity)
        {
            hasInProgressNuclearActivity = false;
            List<WeekDayActivityDataItem> list = new List<WeekDayActivityDataItem>();
            if (seriesData.weekActivityList == null)
            {
                return list;
            }
            int i = 0;
            int count = seriesData.weekActivityList.Count;
            while (i < count)
            {
                if (seriesData.weekActivityList[i].GetActivityType() != ActivityType.ACTIVITY_NUCLEAR)
                {
                    goto IL_005F;
                }
                if (seriesData.weekActivityList[i].GetStatus() == ActivityDataItemBase.Status.InProgress)
                {
                    hasInProgressNuclearActivity = true;
                    goto IL_005F;
                }
            IL_0071:
                i++;
                continue;
            IL_005F:
                list.Add(seriesData.weekActivityList[i]);
                goto IL_0071;
            }
            list.Sort((WeekDayActivityDataItem lob, WeekDayActivityDataItem robj) => lob.GetActivityID() - robj.GetActivityID());
            return list;
        }

        // Token: 0x0600374E RID: 14158 RVA: 0x0012B648 File Offset: 0x00129848
        public override void Destroy()
        {
            if (this._newUnlockLevelActDelayTimer != null)
            {
                this._newUnlockLevelActDelayTimer.Destroy();
            }
            if (this._newUnlockLevelAnimationTimer != null)
            {
                this._newUnlockLevelAnimationTimer.Destroy();
            }
            if (base.view != null)
            {
                this.ClearLevelsContent();
                this.ClearActivityContent();
            }
            base.Destroy();
        }

        // Token: 0x0600374F RID: 14159 RVA: 0x000235F4 File Offset: 0x000217F4
        private void SetRectMaskDirty()
        {
            base.view.transform.Find("LevelPanel/ScrollerView").GetComponent<RectMask>().SetGraphicDirty();
            base.view.transform.Find("ActPanel/ScrollerView").GetComponent<RectMask>().SetGraphicDirty();
        }

        // Token: 0x06003750 RID: 14160 RVA: 0x0012B6A4 File Offset: 0x001298A4
        private bool ShowActivityShopEntry()
        {
            if (this._weekDayActivityData == null)
            {
                return false;
            }
            bool flag = false;
            StoreDataItem storeDataByType = Singleton<StoreModule>.Instance.GetStoreDataByType(UIShopType.SHOP_ACTIVITY);
            if (storeDataByType != null && storeDataByType.isOpen)
            {
                flag = true;
            }
            base.view.transform.Find("EventShopBtn").gameObject.SetActive(this._weekDayActivityData.ShowActivityShopEntry() && flag);
            return false;
        }

        // Token: 0x04002C0D RID: 11277
        private const string LEVEL_PANEL_PREFAB_PATH = "UI/Menus/Widget/Map/LevelPanel";

        // Token: 0x04002C0E RID: 11278
        private const string ACT_PREFAB_PATH = "UI/Menus/Widget/Map/Act";

        // Token: 0x04002C0F RID: 11279
        private const string PAGE_FADE_IN_ANI_STR = "PageFadeIn";

        // Token: 0x04002C10 RID: 11280
        private const int FIRST_LEVEL_ID = 10101;

        // Token: 0x04002C11 RID: 11281
        private const float TIMER_SPAN = 0.5f;

        // Token: 0x04002C12 RID: 11282
        private const float ACT_DELAY_TIMER_SPAN = 0.4f;

        // Token: 0x04002C13 RID: 11283
        private StageType _chapterType;

        // Token: 0x04002C14 RID: 11284
        private ChapterDataItem chapter;

        // Token: 0x04002C15 RID: 11285
        private LevelDiffculty difficulty;

        // Token: 0x04002C16 RID: 11286
        private bool _difficultyPopUpActive;

        // Token: 0x04002C17 RID: 11287
        private LevelDetailDialogContextV2 _levelDetailContext;

        // Token: 0x04002C18 RID: 11288
        private MonoActScroller _actScroller;

        // Token: 0x04002C19 RID: 11289
        private int _showActIndex;

        // Token: 0x04002C1A RID: 11290
        private bool _justShowLevelDetail;

        // Token: 0x04002C1B RID: 11291
        private LevelDataItem _toShowLevelData;

        // Token: 0x04002C1C RID: 11292
        private WeekDayActivityDataItem _weekDayActivityData;

        // Token: 0x04002C1D RID: 11293
        public Dictionary<LevelDataItem, Transform> levelTransDict;

        // Token: 0x04002C1E RID: 11294
        private List<LevelDataItem> _newUnlockLevelDataList;

        // Token: 0x04002C1F RID: 11295
        private CanvasTimer _newUnlockLevelAnimationTimer;

        // Token: 0x04002C20 RID: 11296
        private CanvasTimer _newUnlockLevelActDelayTimer;

        // Token: 0x04002C21 RID: 11297
        private float _actScrollerLerpSpeed;

        // Token: 0x04002C22 RID: 11298
        private float _actScrollerStopLerpThreshold;

        // Token: 0x04002C23 RID: 11299
        private float _actScrollerSpeedDownRatio = 5f;

        // Token: 0x04002C24 RID: 11300
        private bool _shouldChangeActIndex;

        // Token: 0x04002C25 RID: 11301
        private bool _needLerpAfterInitLevels;
    }
}
