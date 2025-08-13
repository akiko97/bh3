using MoleMole.Config;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class TestUIContext : BaseWidgetContext
	{
		public TestUIContext(GameObject view)
		{
			config = new ContextPattern
			{
				contextName = "TestUIContext"
			};
			base.view = view;
		}

		private void TestCode(MonoTestUI testUI)
		{
			PlayerStatusWidgetContext widget = new PlayerStatusWidgetContext();
			Singleton<MainUIManager>.Instance.ShowWidget(widget);
			Singleton<MainUIManager>.Instance.ShowPage(new MainPageContext());
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 7)
			{
				return OnPlayerLoginRsp(pkt.getData<PlayerLoginRsp>());
			}
			return false;
		}

		private bool OnPlayerLoginRsp(PlayerLoginRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				MonoTestUI monoTestUI = Singleton<MainUIManager>.Instance.SceneCanvas as MonoTestUI;
				if (monoTestUI.avatar3dModelContext != null)
				{
					return false;
				}
				monoTestUI.MainCamera.SetActive(true);
				monoTestUI.MainMenu_SpaceShip.SetActive(true);
				monoTestUI.avatar3dModelContext = new Avatar3dModelContext();
				Singleton<MainUIManager>.Instance.ShowWidget(monoTestUI.avatar3dModelContext, UIType.Root);
				GameObject gameObject = GameObject.Find("MainMenu_SpaceShip");
				GameObject uiMainCamera = GameObject.Find("MainCamera");
				SpaceShipModelContext widget = new SpaceShipModelContext(gameObject, uiMainCamera);
				Singleton<MainUIManager>.Instance.ShowWidget(widget);
				GraphicsSettingData.ApplySettingConfig();
				AudioSettingData.ApplySettingConfig();
				TestCode(monoTestUI);
			}
			else
			{
				GeneralDialogContext generalDialogContext = new GeneralDialogContext();
				generalDialogContext.type = GeneralDialogContext.ButtonType.SingleButton;
				generalDialogContext.title = LocalizationGeneralLogic.GetText("Menu_Title_Tips");
				generalDialogContext.desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				generalDialogContext.notDestroyAfterTouchBG = true;
				GeneralDialogContext dialogContext = generalDialogContext;
				Singleton<MainUIManager>.Instance.ShowDialog(dialogContext);
			}
			return false;
		}
	}
}
