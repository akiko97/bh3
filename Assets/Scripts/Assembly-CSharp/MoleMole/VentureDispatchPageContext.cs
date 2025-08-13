using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class VentureDispatchPageContext : BasePageContext
	{
		private const int MAX_TEAM_MEMBER_NUM = 3;

		private VentureDataItem _ventureData;

		private bool _isConditionMatch;

		public VentureDispatchPageContext(VentureDataItem ventureData)
		{
			config = new ContextPattern
			{
				contextName = "VentureDispatchPageContext",
				viewPrefabPath = "UI/Menus/Page/Island/VentureDispatchPage"
			};
			_ventureData = ventureData;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 174)
			{
				return OnDispatchIslandVentureRsp(pkt.getData<DispatchIslandVentureRsp>());
			}
			return false;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.DispatchAvatarChanged)
			{
				SetupMyTeam();
				SetupVentureConditaions();
				return false;
			}
			if (ntf.type == NotifyTypes.ShowStaminaExchangeInfo2)
			{
				return ShowStaminaExchangeDialog();
			}
			return false;
		}

		public bool ShowStaminaExchangeDialog()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new StaminaExchangeDialogContext("Menu_Desc_StaminaExchange2"));
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Btn").GetComponent<Button>(), OnOkButtonCallBack);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Cost/Stamina").GetComponent<Text>().text = _ventureData.StaminaCost.ToString();
			SetupMyTeam();
			SetupVentureInfo();
			SetupVentureConditaions();
			SetupVentureRewards();
			return false;
		}

		public override void BackToMainMenuPage()
		{
			Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true, false);
		}

		public void OnOkButtonCallBack()
		{
			bool flag = _ventureData.StaminaCost <= Singleton<PlayerModule>.Instance.playerData.stamina;
			bool flag2 = Singleton<IslandModule>.Instance.GetVentureInProgressNum() < (Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType)5) as CabinVentureDataItem).GetMaxVentureNumInProgress();
			if (_isConditionMatch && flag && flag2)
			{
				Singleton<NetworkManager>.Instance.RequestDispatchIslandVenture(_ventureData.VentureID, _ventureData.selectedAvatarList);
			}
			else if (!_isConditionMatch)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					title = LocalizationGeneralLogic.GetText("Menu_Tips"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_ConditionNotMatchHint")
				});
			}
			else if (!flag2)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					title = LocalizationGeneralLogic.GetText("Menu_Tips"),
					desc = LocalizationGeneralLogic.GetText("Menu_Desc_VentureInProgressExceedLimit"),
					type = GeneralDialogContext.ButtonType.SingleButton
				});
			}
			else if (!flag)
			{
				Singleton<PlayerModule>.Instance.playerData._cacheDataUtil.CheckCacheValidAndGo<PlayerStaminaExchangeInfo>(ECacheData.Stamina, NotifyTypes.ShowStaminaExchangeInfo2);
			}
		}

		public bool OnDispatchIslandVentureRsp(DispatchIslandVentureRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				BackPage();
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
				{
					type = GeneralDialogContext.ButtonType.SingleButton,
					title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
					desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode)
				});
			}
			return false;
		}

		private void SetupMyTeam()
		{
			Transform transform = base.view.transform.Find("TeamPanel/Team");
			_ventureData.CleanUpSelectAvatarList();
			for (int i = 1; i <= 3; i++)
			{
				transform.Find(i.ToString()).GetComponent<MonoVentureDispatchAvatar>().SetupView(i, _ventureData);
			}
		}

		private void SetupVentureInfo()
		{
			base.view.transform.Find("TeamPanel/Info/Title/Desc").GetComponent<Text>().text = _ventureData.VentureName;
			base.view.transform.Find("TeamPanel/Info/Desc/Text").GetComponent<Text>().text = _ventureData.Desc;
			base.view.transform.Find("TeamPanel/Info/Desc/Text").GetComponent<TypewriterEffect>().RestartRead();
			base.view.transform.Find("InfoPanel/Level/Num").GetComponent<Text>().text = _ventureData.Level.ToString();
			Transform transform = base.view.transform.Find("InfoPanel/Difficulty/Icon");
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(i + 1 == _ventureData.Difficulty);
			}
			base.view.transform.Find("Cost/Stamina").GetComponent<Text>().text = _ventureData.StaminaCost.ToString();
		}

		private void SetupVentureConditaions()
		{
			Transform transform = base.view.transform.Find("InfoPanel/RequestPanel/RequestScrollView/Content");
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).GetComponent<MonoVentureConditionRow>().SetupView(i, _ventureData);
			}
			_isConditionMatch = _ventureData.IsConditionAllMatch();
		}

		private void SetupVentureRewards()
		{
			List<int> rewardItemIDListToShow = _ventureData.RewardItemIDListToShow;
			Transform transform = base.view.transform.Find("InfoPanel/DropPanel/Drops/ScollerContent");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (i >= rewardItemIDListToShow.Count)
				{
					child.gameObject.SetActive(false);
					continue;
				}
				StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(rewardItemIDListToShow[i]);
				child.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, OnDropItemButtonClick, false, false, false, true);
			}
		}

		private void OnDropItemButtonClick(StorageDataItemBase dropItemData)
		{
			UIUtil.ShowItemDetail(dropItemData, true);
		}
	}
}
