using MoleMole.Config;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class ResetLevelDialogContext : BaseDialogContext
	{
		private readonly LevelDataItem _levelData;

		private LevelDetailDialogContextV2 _parentDialog;

		public ResetLevelDialogContext(LevelDataItem levelData, LevelDetailDialogContextV2 parentDialog)
		{
			config = new ContextPattern
			{
				contextName = "ResetLevelDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/ResetLevelDialog"
			};
			_levelData = levelData;
			_parentDialog = parentDialog;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/GiveUp").GetComponent<Button>(), OnGiveUpButtonCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/OK").GetComponent<Button>(), OnOKButtonCallBack);
			BindViewCallback(base.view.transform.Find("Dialog/Title/CloseBtn").GetComponent<Button>(), OnCloseButtonCallBack);
		}

		protected override bool SetupView()
		{
			bool flag = LevelResetCostMetaDataReader.TryGetLevelResetCostMetaDataByKey(_levelData.resetTimes + 1) == null;
			int num = ((!flag) ? _levelData.GetHCoinSpentToResetLevel(_levelData.resetTimes + 1) : 0);
			string stageName = _levelData.StageName;
			int maxResetTimes = _levelData.MaxResetTimes;
			int num2 = _levelData.MaxResetTimes - _levelData.resetTimes;
			int hcoin = Singleton<PlayerModule>.Instance.playerData.hcoin;
			base.view.transform.Find("Dialog/Content/ReviveConsumePanel/InfoAvatar/HCoinNumText").GetComponent<Text>().text = ((!flag) ? num.ToString() : "??");
			base.view.transform.Find("Dialog/Content/ReviveConsumePanel/InfoAvatar/LevelName").GetComponent<Text>().text = stageName;
			base.view.transform.Find("Dialog/Content/ReviveConsumePanel/Consume/ReviveTimes/AvailableTimes").GetComponent<Text>().text = num2.ToString();
			base.view.transform.Find("Dialog/Content/ReviveConsumePanel/Consume/ReviveTimes/AvailableTimes/MaxTimes").GetComponent<Text>().text = maxResetTimes.ToString();
			base.view.transform.Find("Dialog/Content/ReviveConsumePanel/Consume/Hcoin/Num").GetComponent<Text>().text = hcoin.ToString();
			return false;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 109)
			{
				return OnStageEnterTimesRsp(pkt.getData<ResetStageEnterTimesRsp>());
			}
			return false;
		}

		private bool OnStageEnterTimesRsp(ResetStageEnterTimesRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Invalid comparison between Unknown and I4
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 2)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.DoubleButton,
					title = LocalizationGeneralLogic.GetText("Menu_GoToRecharge"),
					desc = LocalizationGeneralLogic.GetText("Menu_GoToRechargeDesc"),
					okBtnText = LocalizationGeneralLogic.GetText("Menu_GoToRecharge"),
					cancelBtnText = LocalizationGeneralLogic.GetText("Menu_GiveUpRecharge"),
					buttonCallBack = delegate(bool confirmed)
					{
						if (confirmed)
						{
							Singleton<MainUIManager>.Instance.ShowPage(new RechargePageContext());
						}
					}
				});
			}
			else if ((int)rsp.retcode == 3)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_ResetStageEnterFail"),
					desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)
				});
			}
			else if ((int)rsp.retcode == 0)
			{
				_parentDialog.RefreshChallengeNumber();
			}
			Destroy();
			return false;
		}

		public void OnGiveUpButtonCallBack()
		{
			Destroy();
		}

		public void OnOKButtonCallBack()
		{
			Singleton<NetworkManager>.Instance.RequestStageEnterTimes((uint)_levelData.levelId);
		}

		public void OnCloseButtonCallBack()
		{
			Destroy();
		}
	}
}
