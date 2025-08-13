using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class AvatarSubSkillDialogContext : BaseDialogContext
	{
		public readonly AvatarDataItem avatarData;

		public readonly AvatarSkillDataItem skillData;

		public readonly AvatarSubSkillDataItem subSkillData;

		private List<NeedItemData> _showItemList;

		public AvatarSubSkillDialogContext(AvatarDataItem avatarData, AvatarSkillDataItem skillData, AvatarSubSkillDataItem subSkillData)
		{
			config = new ContextPattern
			{
				contextName = "AvatarSubSkillDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/AvatarSubSkillDialog"
			};
			this.avatarData = avatarData;
			this.skillData = skillData;
			this.subSkillData = subSkillData;
		}

		public override bool OnPacket(NetPacketV1 pkt)
		{
			ushort cmdId = pkt.getCmdId();
			if (cmdId == 51)
			{
				return OnAvatarSubSkillLevelUpRsp(pkt.getData<AvatarSubSkillLevelUpRsp>());
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/LeftBtn").GetComponent<Button>(), OnLeftBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/RightBtn").GetComponent<Button>(), OnRightBtnClick);
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			Transform transform = base.view.transform.Find("Dialog/Content/DoubleButton/LeftBtn");
			bool flag = !subSkillData.UnLocked && subSkillData.CanTry;
			string textID = ((!flag) ? "Menu_Cancel" : "Menu_TrySkill");
			transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID);
			Transform transform2 = base.view.transform.Find("Dialog/Content/DoubleButton/RightBtn");
			string textID2 = ((!subSkillData.UnLocked) ? "Menu_Action_Unlock" : "Menu_LevelUp");
			transform2.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID2);
			if (!flag)
			{
				transform.gameObject.SetActive(false);
				transform2.GetComponent<RectTransform>().SetLocalPositionX(0f);
			}
			base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/NameRow/NameText").GetComponent<Text>().text = subSkillData.Name;
			Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(subSkillData.IconPath);
			base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/Icon/Image").GetComponent<Image>().sprite = spriteByPrefab;
			Text component = base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/NameRow/AddPtText").GetComponent<Text>();
			component.gameObject.SetActive(subSkillData.UnLocked);
			if (subSkillData.level == subSkillData.MaxLv)
			{
				component.text = "MAX";
			}
			else
			{
				component.text = ((subSkillData.level <= 0) ? string.Empty : string.Format("+{0}", subSkillData.level));
			}
			base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/RemainSkillPoint/Num").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.skillPoint.ToString();
			SetupDesc();
			SetupConsume();
			SetupMaterials();
			SetupLackInfo();
			return false;
		}

		public void OnRightBtnClick()
		{
			Singleton<NetworkManager>.Instance.RequestAvatarSubSkillLevelUp(avatarData.avatarID, skillData.skillID, subSkillData.subSkillID);
		}

		public void OnLeftBtnClick()
		{
			if (subSkillData.CanTry && !subSkillData.UnLocked)
			{
				Singleton<LevelScoreManager>.Create();
				Singleton<LevelScoreManager>.Instance.SetTryLevelBeginIntent(avatarData.avatarID, "Lua/Levels/Common/LevelInfinityTest.lua", skillData.skillID, subSkillData.subSkillID);
				Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", true, true);
			}
			else
			{
				Close();
			}
		}

		public void Close()
		{
			Destroy();
		}

		public void OnBGClick(BaseEventData evtData = null)
		{
			Close();
		}

		private bool OnAvatarSubSkillLevelUpRsp(AvatarSubSkillLevelUpRsp rsp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Invalid comparison between Unknown and I4
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Invalid comparison between Unknown and I4
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			if ((int)rsp.retcode == 0)
			{
				Dictionary<int, SubSkillStatus> subSkillStatusDict = Singleton<MiHoYoGameData>.Instance.LocalData.SubSkillStatusDict;
				SubSkillStatus status = subSkillData.Status;
				if (status == SubSkillStatus.CanUnlock || status == SubSkillStatus.CanUpLevel)
				{
					subSkillStatusDict[subSkillData.subSkillID] = subSkillData.Status;
				}
				else
				{
					subSkillStatusDict.Remove(subSkillData.subSkillID);
				}
				Singleton<MiHoYoGameData>.Instance.Save();
				Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SubSkillStatusCacheUpdate));
				Close();
			}
			else
			{
				string empty = string.Empty;
				if ((int)rsp.retcode == 5)
				{
					int num = ((!subSkillData.UnLocked) ? subSkillData.UnlockLv : subSkillData.LvUpNeedAvatarLevel);
					empty = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode, num);
				}
				else if ((int)rsp.retcode == 6)
				{
					int index = ((!subSkillData.UnLocked) ? subSkillData.UnlockStar : subSkillData.GetUpLevelStarNeed());
					string text = MiscData.Config.AvatarStarName[index];
					empty = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode, text);
				}
				else
				{
					empty = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.retcode);
				}
				Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(empty));
			}
			return false;
		}

		private void SetupLackInfo()
		{
			bool flag = ((!subSkillData.UnLocked) ? (avatarData.level < subSkillData.UnlockLv) : (avatarData.level < subSkillData.LvUpNeedAvatarLevel));
			bool flag2 = ((!subSkillData.UnLocked) ? (avatarData.star < subSkillData.UnlockStar) : (avatarData.star < subSkillData.GetUpLevelStarNeed()));
			Transform transform = base.view.transform.Find("Dialog/Content/VerticalLayout/LevelLack");
			Transform transform2 = base.view.transform.Find("Dialog/Content/VerticalLayout/StarLack");
			base.view.transform.Find("Dialog/Content/DoubleButton/RightBtn").GetComponent<Button>().interactable = !flag && !flag2;
			if (flag2)
			{
				transform2.gameObject.SetActive(true);
				transform.gameObject.SetActive(false);
				transform2.Find("UnLockStar").GetComponent<MonoAvatarStar>().SetupView((!subSkillData.UnLocked) ? subSkillData.UnlockStar : subSkillData.GetUpLevelStarNeed());
			}
			else if (flag)
			{
				transform2.gameObject.SetActive(false);
				transform.gameObject.SetActive(true);
				transform.Find("LvNeed").GetComponent<Text>().text = ((!subSkillData.UnLocked) ? subSkillData.UnlockLv : subSkillData.LvUpNeedAvatarLevel).ToString();
			}
			else
			{
				transform2.gameObject.SetActive(false);
				transform.gameObject.SetActive(false);
			}
		}

		private void SetupDesc()
		{
			base.view.transform.Find("Dialog/Content/VerticalLayout/DescText").GetComponent<Text>().text = ((!subSkillData.UnLocked) ? subSkillData.Info : (LocalizationGeneralLogic.GetText("Menu_Desc_AfterLvUp") + subSkillData.NextLevelInfo));
		}

		private void SetupConsume()
		{
			bool flag = ((!subSkillData.UnLocked) ? (avatarData.level < subSkillData.UnlockLv) : (avatarData.level < subSkillData.LvUpNeedAvatarLevel));
			bool flag2 = ((!subSkillData.UnLocked) ? (avatarData.star < subSkillData.UnlockStar) : (avatarData.star < subSkillData.GetUpLevelStarNeed()));
			if (flag || flag2)
			{
				base.view.transform.Find("Dialog/Content/VerticalLayout/Consume").gameObject.SetActive(false);
				return;
			}
			base.view.transform.Find("Dialog/Content/VerticalLayout/Consume").gameObject.SetActive(true);
			base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/Label/Unlock").gameObject.SetActive(!subSkillData.UnLocked);
			base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/Label/LvUp").gameObject.SetActive(subSkillData.UnLocked);
			int num = ((!subSkillData.UnLocked) ? subSkillData.UnlockPoint : subSkillData.LvUpPoint);
			int num2 = ((!subSkillData.UnLocked) ? subSkillData.UnlockSCoin : subSkillData.LvUpSCoin);
			base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/SkillPointNum").GetComponent<Text>().text = num.ToString();
			if (num > Singleton<PlayerModule>.Instance.playerData.skillPoint)
			{
				base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/SkillPointNum").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
			}
			else
			{
				base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/SkillPointNum").GetComponent<Text>().color = MiscData.GetColor("TotalWhite");
			}
			base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/SCoinNum").GetComponent<Text>().text = num2.ToString();
			if (num2 > Singleton<PlayerModule>.Instance.playerData.scoin)
			{
				base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/SCoinNum").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
			}
			else
			{
				base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/SCoinNum").GetComponent<Text>().color = MiscData.GetColor("TotalWhite");
			}
		}

		private void SetupMaterials()
		{
			_showItemList = ((!subSkillData.UnLocked) ? subSkillData.UnlockNeedItemList : subSkillData.LvUpNeedItemList);
			if (_showItemList == null || _showItemList.Count <= 0)
			{
				base.view.transform.Find("Dialog/Content/VerticalLayout/Materials").gameObject.SetActive(false);
				return;
			}
			foreach (NeedItemData showItem in _showItemList)
			{
				showItem.enough = Singleton<StorageModule>.Instance.TryGetStorageDataItemByMetaId(showItem.itemMetaID, showItem.itemNum).Count > 0;
			}
			base.view.transform.Find("Dialog/Content/VerticalLayout/Materials/Materials").GetComponent<MonoGridScroller>().Init(OnChange, _showItemList.Count);
		}

		private void OnChange(Transform trans, int index)
		{
			MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
			component.SetupView(_showItemList[index].itemData);
			component.transform.Find("NotEnough").gameObject.SetActive(!_showItemList[index].enough);
			component.SetClickCallback(OnItemButonClick);
		}

		private void OnItemButonClick(StorageDataItemBase item, bool selected)
		{
			if ((subSkillData.UnLocked && subSkillData.GetLvUpNeedItemDataByID(item.ID).enough) || (!subSkillData.UnLocked && subSkillData.GetUnlockNeedItemDataByID(item.ID).enough))
			{
				UIUtil.ShowItemDetail(item, true);
			}
			else
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new DropLinkDialogContext(item as MaterialDataItem));
			}
		}
	}
}
