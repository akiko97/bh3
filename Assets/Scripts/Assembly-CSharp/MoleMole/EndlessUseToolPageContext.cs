using System;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class EndlessUseToolPageContext : BasePageContext
	{
		private const string BATTLE_REPORT_PREFAB_PATH = "UI/Menus/Widget/EndlessActivity/BattleReportRow";

		private const string RANK_ROW_BUTTON_PREFAB_PATH = "UI/Menus/Widget/EndlessActivity/RankInfoButton";

		private const string ENDLESS_ITEM_PREFAB_PATH = "UI/Menus/Widget/EndlessActivity/EndlessItem";

		private EndlessPlayerData _selectPlayer;

		private EndlessItem _selectItem;

		private EndlessToolDataItem _selectToolData;

		private int ANIMATOR_CAN_SELECT_PLAYER_BOOL_ID = Animator.StringToHash("CanSelectPlayer");

		private Animator _animator;

		public EndlessUseToolPageContext()
		{
			config = new ContextPattern
			{
				contextName = "EndlessUseToolPageContext",
				viewPrefabPath = "UI/Menus/Page/EndlessActivity/EndlessUseToolPage"
			};
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			switch (pkt.getCmdId())
			{
			case 140:
				return SetupView();
			case 148:
				return OnUseEndlessItemRsp(pkt.getData<UseEndlessItemRsp>());
			case 151:
				return OnEndlessPlayerDataUpdateNotify(pkt.getData<EndlessPlayerDataUpdateNotify>());
			case 146:
				return OnGetLastEndlessRewardDataRsp(pkt.getData<GetLastEndlessRewardDataRsp>());
			case 152:
				return OnEndlessItemDataUpdateNotify(pkt.getData<EndlessItemDataUpdateNotify>());
			default:
				return false;
			}
		}

		public override bool OnNotify(Notify ntf)
		{
			return false;
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("GroupPanel/Title/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectEndlessToolFirst");
			_animator = base.view.transform.GetComponent<Animator>();
			SetupRank();
			SetupItemList();
			SetRankListTitle();
			CheckSelectItemForPlayerPanel();
			Singleton<EndlessModule>.Instance.CheckAllAvatarHPChanged();
			return false;
		}

		public override void OnLandedFromBackPage()
		{
			base.OnLandedFromBackPage();
			CheckCurrentEndlessDataValid();
		}

		private void OnUseBtnClick()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Invalid comparison between Unknown and I4
			if (_selectToolData.ApplyToSelf || (int)_selectToolData.ToolType == 3)
			{
				SetupUseToolConfirmDialog(_selectToolData);
			}
			else
			{
				_animator.SetBool(ANIMATOR_CAN_SELECT_PLAYER_BOOL_ID, true);
			}
		}

		private void RequestUseTool()
		{
			Singleton<NetworkManager>.Instance.RequestUseEndlessItem(_selectItem.item_id, (_selectPlayer != null) ? ((int)_selectPlayer.uid) : (-1));
			_selectPlayer = null;
		}

		private bool OnEndlessPlayerDataUpdateNotify(EndlessPlayerDataUpdateNotify rsp)
		{
			SetupRank();
			return false;
		}

		private bool OnGetLastEndlessRewardDataRsp(GetLastEndlessRewardDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0 && rsp.reward_list.Count > 0)
			{
				BackPage();
			}
			return false;
		}

		private bool OnEndlessItemDataUpdateNotify(EndlessItemDataUpdateNotify rsp)
		{
			SetupItemList();
			return false;
		}

		private bool OnUseEndlessItemRsp(UseEndlessItemRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				ShowEndlessToolEffect(_selectToolData, rsp);
				_selectItem = null;
				_selectToolData = null;
				SetupView();
			}
			return false;
		}

		private void SetupRank()
		{
			List<int> rankListSorted = Singleton<EndlessModule>.Instance.GetRankListSorted();
			base.view.transform.Find("GroupPanel/RankPanel/RankList").GetComponent<MonoGridScroller>().Init(OnScrollerChange, rankListSorted.Count);
		}

		private void SetupItemList()
		{
			Transform transform = base.view.transform.Find("ItemPanel/ItemList/Content");
			List<EndlessItem> playerEndlessItemList = Singleton<EndlessModule>.Instance.GetPlayerEndlessItemList();
			if (transform.childCount < playerEndlessItemList.Count)
			{
				int num = playerEndlessItemList.Count - transform.childCount;
				for (int i = 0; i < num; i++)
				{
					Transform transform2 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("UI/Menus/Widget/EndlessActivity/EndlessItem")).transform;
					transform2.SetParent(transform, false);
				}
			}
			for (int j = 0; j < transform.childCount; j++)
			{
				Transform child = transform.GetChild(j);
				if (j >= playerEndlessItemList.Count)
				{
					child.gameObject.SetActive(false);
					continue;
				}
				EndlessItem val = playerEndlessItemList[j];
				child.GetComponent<MonoEndlessItemButton>().SetupView(val, val == _selectItem, _selectPlayer, OnItemButtonClick, OnUseBtnClick);
			}
			CheckItemListEmpty();
		}

		private void ShowActivityRewardDialog(GetLastEndlessRewardDataRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				if (rsp.reward_list.Count > 0)
				{
					Singleton<MainUIManager>.Instance.ShowDialog(new MissionRewardGotDialogContext(rsp.reward_list));
				}
				Singleton<MiHoYoGameData>.Instance.LocalData.LastRewardData = null;
				Singleton<MiHoYoGameData>.Instance.Save();
			}
		}

		private void CheckCurrentEndlessDataValid()
		{
			if (TimeUtil.Now > Singleton<EndlessModule>.Instance.SettlementTime)
			{
				Singleton<NetworkManager>.Instance.RequestEndlessData();
				BackPage();
			}
		}

		private void OnRankRowButtonClick(EndlessPlayerData playerEndlessData)
		{
			if (_selectItem == null)
			{
				base.view.transform.Find("GroupPanel/Title/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectEndlessToolFirst");
				return;
			}
			if (_selectPlayer == playerEndlessData)
			{
				_selectPlayer = null;
			}
			else
			{
				_selectPlayer = playerEndlessData;
				SetupUseToolConfirmDialog(_selectToolData);
			}
			SetupRank();
			SetupItemList();
		}

		private void OnItemButtonClick(EndlessItem itemData)
		{
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Invalid comparison between Unknown and I4
			if (_selectItem == itemData)
			{
				_selectItem = null;
				_selectToolData = null;
				base.view.transform.Find("GroupPanel/Title/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectEndlessToolFirst");
				_animator.SetBool(ANIMATOR_CAN_SELECT_PLAYER_BOOL_ID, false);
			}
			else
			{
				_selectItem = itemData;
				_selectToolData = new EndlessToolDataItem((int)itemData.item_id);
				if (_selectToolData.ApplyToSelf || (int)_selectToolData.ToolType == 3)
				{
					_animator.SetBool(ANIMATOR_CAN_SELECT_PLAYER_BOOL_ID, false);
				}
			}
			_selectPlayer = null;
			SetRankListTitle();
			SetupRank();
			SetupItemList();
		}

		private void OnScrollerChange(Transform childTrans, int index)
		{
			List<int> rankListSorted = Singleton<EndlessModule>.Instance.GetRankListSorted();
			EndlessPlayerData playerEndlessData = Singleton<EndlessModule>.Instance.GetPlayerEndlessData(rankListSorted[index]);
			PlayerFriendBriefData playerBriefData = Singleton<EndlessModule>.Instance.GetPlayerBriefData(rankListSorted[index]);
			childTrans.GetComponent<MonoRankButton>().SetupView(index + 1, playerEndlessData, UIUtil.GetPlayerNickname(playerBriefData), _selectPlayer == playerEndlessData, OnRankRowButtonClick, _selectToolData);
		}

		private void SetRankListTitle()
		{
			if (_selectItem == null)
			{
				base.view.transform.Find("GroupPanel/Title/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectEndlessToolFirst");
			}
			else
			{
				base.view.transform.Find("GroupPanel/Title/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_SelectEndlessTarget");
			}
		}

		private void CheckItemListEmpty()
		{
			base.view.transform.Find("ItemPanel/NoItemHint").gameObject.SetActive(Singleton<EndlessModule>.Instance.GetPlayerEndlessItemList().Count <= 0);
		}

		private void CheckSelectItemForPlayerPanel()
		{
			if (_selectItem == null || _selectToolData.ApplyToSelf || _selectPlayer == null)
			{
				_animator.SetBool(ANIMATOR_CAN_SELECT_PLAYER_BOOL_ID, false);
			}
			else
			{
				_animator.SetBool(ANIMATOR_CAN_SELECT_PLAYER_BOOL_ID, true);
			}
		}

		private void ShowEndlessToolEffect(EndlessToolDataItem toolData, UseEndlessItemRsp rsp)
		{
			Singleton<MainUIManager>.Instance.ShowUIEffect(config.contextName, toolData.EffectPrefatPath);
			string text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessUseSuccess", toolData.GetDisplayTitle());
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(text));
		}

		private void SetupUseToolConfirmDialog(EndlessToolDataItem toolData)
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Invalid comparison between Unknown and I4
			string text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessComfirmUse", toolData.GetDisplayTitle());
			if (toolData.ShowIcon)
			{
				if (toolData.ApplyToSelf)
				{
					List<EndlessToolDataItem> appliedToolDataList = Singleton<EndlessModule>.Instance.GetAppliedToolDataList();
					for (int i = 0; i < appliedToolDataList.Count; i++)
					{
						if (appliedToolDataList[i].ID == toolData.ID)
						{
							text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessComfirmReuse", toolData.GetDisplayTitle());
							break;
						}
					}
				}
				else if ((int)toolData.ToolType != 3 && _selectPlayer != null)
				{
					List<EndlessWaitEffectItem> wait_effect_item_list = _selectPlayer.wait_effect_item_list;
					for (int j = 0; j < wait_effect_item_list.Count; j++)
					{
						if (wait_effect_item_list[j].item_id == (uint)toolData.ID)
						{
							text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessComfirmReuse", toolData.GetDisplayTitle());
							break;
						}
					}
				}
			}
			if (Singleton<EndlessModule>.Instance.SelfInvisible())
			{
				text = text + Environment.NewLine + LocalizationGeneralLogic.GetText("Menu_Desc_InvisibleItemWillLoseEffectHint");
			}
			Singleton<MainUIManager>.Instance.ShowDialog(new GeneralDialogContext
			{
				type = GeneralDialogContext.ButtonType.DoubleButton,
				title = LocalizationGeneralLogic.GetText("Menu_Title_Tips"),
				desc = text,
				buttonCallBack = OnConfirmUseTool,
				notDestroyAfterTouchBG = true,
				destroyCallBack = ClearSelectPlayer
			});
		}

		private void OnConfirmUseTool(bool confirmed)
		{
			if (confirmed)
			{
				RequestUseTool();
			}
			else
			{
				ClearSelectPlayer();
			}
		}

		private void ClearSelectPlayer()
		{
			_selectPlayer = null;
			SetupRank();
			CheckSelectItemForPlayerPanel();
		}
	}
}
