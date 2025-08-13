using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;
using Material = UnityEngine.Material;

namespace MoleMole
{
	public class EndlessFloorEndPageContext : BasePageContext
	{
		private const float WAIT_TIME = 1f;

		private const string DROP_ITEM_SCALE_07 = "DropItemScale07";

		private const string MATERIAL_GRAY_SCALE_PATH = "Material/ImageGrayscale";

		private SequenceAnimationManager _animationManager;

		public readonly bool isSuccess;

		public readonly EvtLevelState.LevelEndReason endReason;

		private SequenceAnimationManager _dropPanelBGAnimationManager;

		private SequenceAnimationManager _dropItemAnimationManager;

		private List<DropItem> _dropItemList;

		private MonoGridScroller _dropScroller;

		private EndlessStageBeginRsp _stageBeginRsp;

		private EndlessGroupMetaData _groupMetaData;

		private LoadingWheelWidgetContext _loadingWheelDialogContext;

		private bool _hasAvatarDie;

		public EndlessFloorEndPageContext(bool isSuccess)
		{
			config = new ContextPattern
			{
				contextName = "EndlessFloorEndPageContext",
				viewPrefabPath = "UI/Menus/Page/EndlessActivity/EndlessFloorEndPage"
			};
			this.isSuccess = isSuccess;
			if (Singleton<LevelScoreManager>.Instance != null)
			{
				Singleton<LevelScoreManager>.Instance.isLevelSuccess = isSuccess;
			}
		}

		public EndlessFloorEndPageContext(EvtLevelState.LevelEndReason reason)
		{
			config = new ContextPattern
			{
				contextName = "EndlessFloorEndPageContext",
				viewPrefabPath = "UI/Menus/Page/EndlessActivity/EndlessFloorEndPage"
			};
			endReason = reason;
			isSuccess = reason == EvtLevelState.LevelEndReason.EndWin;
			if (Singleton<LevelScoreManager>.Instance != null)
			{
				Singleton<LevelScoreManager>.Instance.isLevelSuccess = isSuccess;
			}
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Actions/ExitBtn/Button").GetComponent<Button>(), Exit);
			BindViewCallback(base.view.transform.Find("Actions/ContinueBtn/Button").GetComponent<Button>(), OnContinueBtnClick);
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 144:
				return OnStageEndRsp(pkt.getData<EndlessStageEndRsp>());
			case 142:
				return OnStageBeginRsp(pkt.getData<EndlessStageBeginRsp>());
			default:
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.AvatarDie)
			{
				OnAvatarDieNotify();
			}
			return false;
		}

		protected override bool SetupView()
		{
			Singleton<LevelScoreManager>.Instance.HandleLevelEnd(endReason);
			base.view.transform.Find("Actions/ContinueBtn").gameObject.SetActive(isSuccess && Singleton<EndlessModule>.Instance.CurrentFinishProgress < Singleton<PlayerModule>.Instance.playerData.endlessMaxProgress);
			base.view.transform.Find("GroupPanel").gameObject.SetActive(false);
			base.view.transform.Find("Actions").gameObject.SetActive(false);
			SyncRequestLevelEnd();
			return false;
		}

		public override void Destroy()
		{
			if (_loadingWheelDialogContext != null)
			{
				_loadingWheelDialogContext.Finish();
			}
			base.Destroy();
		}

		private void Exit()
		{
			if (BehaviorManager.instance != null && BehaviorManager.instance.gameObject != null)
			{
				Object.DestroyImmediate(BehaviorManager.instance.gameObject);
			}
			Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship");
		}

		private void OnContinueBtnClick()
		{
			if (Singleton<LevelScoreManager>.Instance != null)
			{
				Singleton<LevelScoreManager>.Destroy();
			}
			RequestToEnterLevel();
		}

		private bool OnAvatarDieNotify()
		{
			_hasAvatarDie = true;
			return false;
		}

		private bool OnStageEndRsp(EndlessStageEndRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				base.view.transform.Find("Actions/ContinueBtn").gameObject.SetActive(isSuccess && rsp.progress < Singleton<PlayerModule>.Instance.playerData.endlessMaxProgress && Singleton<PlayerModule>.Instance.playerData.GetMemberList((StageType)4).Count > 0);
				DoSetupView();
			}
			else
			{
				string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput));
				Exit();
			}
			return false;
		}

		public bool OnStageBeginRsp(EndlessStageBeginRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				_stageBeginRsp = rsp;
				DoBeginLevel();
			}
			else
			{
				ResetWaitPacketData();
				GeneralDialogContext generalDialogContext = new GeneralDialogContext();
				generalDialogContext.type = GeneralDialogContext.ButtonType.SingleButton;
				generalDialogContext.title = LocalizationGeneralLogic.GetText("Menu_Title_Tips");
				GeneralDialogContext generalDialogContext2 = generalDialogContext;
				generalDialogContext2.desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				Singleton<MainUIManager>.Instance.ShowDialog(generalDialogContext2);
			}
			return false;
		}

		private void SyncRequestLevelEnd()
		{
			LoadingWheelWidgetContext loadingWheelWidgetContext = new LoadingWheelWidgetContext(144);
			loadingWheelWidgetContext.ignoreMaxWaitTime = true;
			Singleton<MainUIManager>.Instance.ShowWidget(loadingWheelWidgetContext);
			if (!Singleton<LevelScoreManager>.Instance.RequestLevelEnd())
			{
				loadingWheelWidgetContext.Finish();
			}
		}

		private void DoSetupView()
		{
			base.view.transform.Find("GroupPanel").gameObject.SetActive(true);
			InitAnimationAndDialogManager();
			Transform transform = base.view.transform.Find("GroupPanel/GroupInfotPanel");
			transform.Find("GroupName").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey(Singleton<EndlessModule>.Instance.currentGroupLevel).groupName);
			Color color = Miscs.ParseColor(MiscData.Config.EndlessGroupUnSelectColor[Singleton<EndlessModule>.Instance.currentGroupLevel]);
			Color color2 = Miscs.ParseColor(MiscData.Config.EndlessGroupBGColor[Singleton<EndlessModule>.Instance.currentGroupLevel]);
			transform.Find("GroupName").GetComponent<Text>().color = color;
			base.view.transform.Find("BG/GroupColor").GetComponent<Image>().color = color2;
			int currentFinishProgress = Singleton<EndlessModule>.Instance.CurrentFinishProgress;
			transform.Find("FloorNum").GetComponent<Text>().text = ((currentFinishProgress != 0) ? currentFinishProgress.ToString() : "-");
			base.view.transform.Find("GroupPanel/Result/Win").gameObject.SetActive(isSuccess);
			base.view.transform.Find("GroupPanel/Result/Lost").gameObject.SetActive(!isSuccess);
			if (!isSuccess)
			{
				base.view.transform.Find("BG/TriangleFill").GetComponent<Image>().material = Miscs.LoadResource<Material>("Material/ImageGrayscale");
				base.view.transform.Find("BG").GetComponent<Image>().material = Miscs.LoadResource<Material>("Material/ImageGrayscale");
			}
			else
			{
				base.view.transform.Find("BG/TriangleFill").GetComponent<Image>().material = null;
				base.view.transform.Find("BG").GetComponent<Image>().material = null;
			}
			SetupReward();
			_dropItemAnimationManager.StartPlay();
		}

		private void SetupReward()
		{
			if (isSuccess)
			{
				_dropItemList = Singleton<LevelScoreManager>.Instance.GetTotalDropList();
				base.view.transform.Find("GroupPanel/Drops/ScrollView").GetComponent<MonoGridScroller>().Init(delegate(Transform trans, int index)
				{
					OnScrollerChange(trans, index);
				}, _dropItemList.Count);
				_dropItemAnimationManager.AddAllChildrenInTransform(base.view.transform.Find("GroupPanel/Drops/ScrollView/Content"));
				base.view.transform.Find("GroupPanel/Drops/Nothing").gameObject.SetActive(_dropItemList.Count <= 0);
			}
		}

		private void OnScrollerChange(Transform trans, int index)
		{
			Vector2 cellSize = _dropScroller.grid.GetComponent<GridLayoutGroup>().cellSize;
			trans.SetLocalScaleX(cellSize.x / trans.GetComponent<MonoLevelDropIconButton>().width);
			trans.SetLocalScaleY(cellSize.y / trans.GetComponent<MonoLevelDropIconButton>().height);
			DropItem val = _dropItemList[index];
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)val.item_id);
			dummyStorageDataItem.level = (int)val.level;
			dummyStorageDataItem.number = (int)val.num;
			trans.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, null, true, true);
			trans.GetComponent<MonoAnimationinSequence>().animationName = "DropItemScale07";
		}

		private void InitAnimationAndDialogManager()
		{
			_dropItemAnimationManager = new SequenceAnimationManager(OnDropItemAnimationEnd);
			_dropScroller = base.view.transform.Find("GroupPanel/Drops/ScrollView").GetComponent<MonoGridScroller>();
		}

		private void OnDropNewItemDialogsEnd()
		{
			_dropItemAnimationManager.StartPlay();
		}

		private void OnDropItemAnimationEnd()
		{
			foreach (Transform item in base.view.transform.Find("GroupPanel/Drops/ScrollView/Content"))
			{
				item.SetLocalScaleX(1f);
				item.SetLocalScaleY(1f);
				item.GetComponent<CanvasGroup>().alpha = 1f;
			}
			base.view.transform.Find("Actions").gameObject.SetActive(true);
			if (_hasAvatarDie)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_EndlessAvatarDie")));
			}
		}

		private void RequestToEnterLevel()
		{
			_loadingWheelDialogContext = new LoadingWheelWidgetContext();
			_loadingWheelDialogContext.ignoreMaxWaitTime = true;
			Singleton<MainUIManager>.Instance.ShowWidget(_loadingWheelDialogContext);
			Singleton<NetworkManager>.Instance.RequestEndlessLevelBeginReq();
		}

		private void DoBeginLevel()
		{
			Singleton<LevelScoreManager>.Create();
			int num = (int)((!_stageBeginRsp.progressSpecified) ? 1 : (_stageBeginRsp.progress + 1));
			_groupMetaData = EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey(Singleton<EndlessModule>.Instance.currentGroupLevel);
			int hardLevel = Mathf.FloorToInt(_groupMetaData.baseHardLevel + (float)(num - 1) * _groupMetaData.deltaHardLevel);
			List<string> list = new List<string>();
			foreach (uint item in _stageBeginRsp.effect_item_id_list)
			{
				EndlessToolDataItem endlessToolDataItem = new EndlessToolDataItem((int)item);
				if (endlessToolDataItem.ParamString != null)
				{
					list.Add(endlessToolDataItem.ParamString);
				}
			}
			Singleton<LevelScoreManager>.Instance.SetEndlessLevelBeginIntent(num, hardLevel, list, _stageBeginRsp, MiscData.Config.BasicConfig.EndlessInitTimer);
			ResetWaitPacketData();
			Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", false, true);
		}

		private void ResetWaitPacketData()
		{
			_stageBeginRsp = null;
			if (_loadingWheelDialogContext != null)
			{
				_loadingWheelDialogContext.Finish();
			}
		}
	}
}
