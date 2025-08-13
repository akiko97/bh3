using MoleMole.Config;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class InLevelReviveConfirmDialogContext : BaseDialogContext
	{
		private InLevelReviveDialogContext _reviveContext;

		private int _hcoinCost;

		private string _avatarFullname;

		private LevelActor _levelActor;

		private LevelScoreManager _levelScoreManager;

		public InLevelReviveConfirmDialogContext(InLevelReviveDialogContext reviveContext, int hcoinCost, string avatarFullName)
		{
			config = new ContextPattern
			{
				contextName = "InLevelReviveConfirmDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/InLevelReviveConfirmDialog"
			};
			_reviveContext = reviveContext;
			_hcoinCost = hcoinCost;
			_avatarFullname = avatarFullName;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 107)
			{
				return OnAvatarReviveRsp(pkt.getData<AvatarReviveRsp>());
			}
			return false;
		}

		protected override bool SetupView()
		{
			_levelScoreManager = Singleton<LevelScoreManager>.Instance;
			_levelActor = Singleton<LevelManager>.Instance.levelActor;
			_reviveContext.view.SetActive(false);
			base.view.transform.Find("Dialog/Content/InfoPanel/InfoAvatar/HcoinNum").GetComponent<Text>().text = _hcoinCost.ToString();
			base.view.transform.Find("Dialog/Content/InfoPanel/InfoAvatar/AvatarFullName").GetComponent<Text>().text = _avatarFullname;
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/OK").GetComponent<Button>(), OnOkBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/GiveUp").GetComponent<Button>(), OnBGBtnClick);
		}

		public bool OnAvatarReviveRsp(AvatarReviveRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				if (rsp.revive_timesSpecified)
				{
					int num = _levelScoreManager.maxReviveNum - (int)rsp.revive_times;
					if (_levelScoreManager.avaiableReviveNum > num)
					{
						_levelScoreManager.avaiableReviveNum = num;
						_levelActor.ReviveAvatarByID(_reviveContext.avatarRuntimeID, _reviveContext.revivePosition);
						Destroy();
						Singleton<WwiseAudioManager>.Instance.Post("BGM_PauseMenu_Off");
						_reviveContext.OnReviveConfirm();
					}
				}
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeFail"),
					desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)
				});
			}
			return false;
		}

		private void OnBGBtnClick()
		{
			_reviveContext.view.SetActive(true);
			Destroy();
		}

		private void OnOkBtnClick()
		{
			LoadingWheelWidgetContext loadingWheelWidgetContext = new LoadingWheelWidgetContext(107);
			loadingWheelWidgetContext.ignoreMaxWaitTime = true;
			Singleton<MainUIManager>.Instance.ShowWidget(loadingWheelWidgetContext);
			Singleton<NetworkManager>.Instance.RequestAvatarRevive();
		}
	}
}
