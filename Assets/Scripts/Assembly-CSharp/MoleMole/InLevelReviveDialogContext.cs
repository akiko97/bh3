using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class InLevelReviveDialogContext : BaseDialogContext
	{
		private const int UNLIMIT_REVIVE_TIMES = 65535;

		private LevelScoreManager _levelScoreManager;

		private List<DropItem> _dropItemList;

		public uint avatarRuntimeID;

		public Vector3 revivePosition;

		public bool allTeamDown;

		private MonoGridScroller _dropGridScroller;

		public InLevelReviveDialogContext(uint avatarRuntimeID, Vector3 revivePosition, bool allTeamDown = false)
		{
			config = new ContextPattern
			{
				contextName = "InLevelReviveDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/InLevelReviveDialogV2"
			};
			this.avatarRuntimeID = avatarRuntimeID;
			this.revivePosition = revivePosition;
			this.allTeamDown = allTeamDown;
		}

		public void RefreshView()
		{
			SetupView();
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.AvatarSelectForRevive)
			{
				return OnAvatarSelectForRevive((uint)ntf.body);
			}
			return false;
		}

		protected override bool SetupView()
		{
			_levelScoreManager = Singleton<LevelScoreManager>.Instance;
			SetupCurrentGetItems();
			InitTeam();
			SetupReviveInfo();
			SetupLayout();
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Title/CloseBtn").GetComponent<Button>(), OnBGBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/CurrentGetItems/Items/PrevBtn").GetComponent<Button>(), OnDropItemLeftArrowClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/CurrentGetItems/Items/NextBtn").GetComponent<Button>(), OnDropItemLeftArrowClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/GiveUp").GetComponent<Button>(), OnGiveUpBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/OK").GetComponent<Button>(), OnReviveButtonClick);
		}

		private bool OnAvatarSelectForRevive(uint selectedAvatarRuntimeID)
		{
			SetupTeam(selectedAvatarRuntimeID);
			SetupReviveInfo();
			return false;
		}

		public void OnBGBtnClick()
		{
			if (!allTeamDown)
			{
				Singleton<LevelManager>.Instance.SetPause(false);
				Destroy();
			}
		}

		private void OnDropItemLeftArrowClick()
		{
			Transform transform = base.view.transform.Find("Dialog/Content/CurrentGetItems/Items");
			transform.Find("ScrollView").GetComponent<MonoGridScroller>().ScrollToPreItem();
		}

		private void OnDropItemRightArrowClick()
		{
			Transform transform = base.view.transform.Find("Dialog/Content/CurrentGetItems/Items");
			transform.Find("ScrollView").GetComponent<MonoGridScroller>().ScrollToNextItem();
		}

		private void OnGiveUpBtnClick()
		{
			if (allTeamDown)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new InLevelGiveUpConfirmDialogContext(this, OnGiveUpConfirm));
			}
			else
			{
				OnBGBtnClick();
			}
		}

		private void OnReviveButtonClick()
		{
			int reviveCost = _levelScoreManager.GetReviveCost();
			if (reviveCost > Singleton<PlayerModule>.Instance.playerData.hcoin)
			{
				Singleton<MainUIManager>.Instance.ShowDialog(new InLevelRechargeDialogContext(this));
				return;
			}
			string fullName = Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatarRuntimeID).avatarDataItem.FullName;
			Singleton<MainUIManager>.Instance.ShowDialog(new InLevelReviveConfirmDialogContext(this, reviveCost, fullName));
		}

		public void OnReviveConfirm()
		{
			Singleton<LevelManager>.Instance.SetPause(false);
			Destroy();
		}

		private void SetupCurrentGetItems()
		{
			if (allTeamDown)
			{
				base.view.transform.Find("Dialog/Content/CurrentGetItems/UpContent/Scoin/Num").GetComponent<Text>().text = Mathf.FloorToInt(_levelScoreManager.scoinInside).ToString();
				Transform transform = base.view.transform.Find("Dialog/Content/CurrentGetItems/Items");
				_dropItemList = _levelScoreManager.GetDropListToShow();
				transform.gameObject.SetActive(_dropItemList.Count > 0);
				_dropGridScroller = transform.Find("ScrollView").GetComponent<MonoGridScroller>();
				_dropGridScroller.Init(OnScrollerChange, _dropItemList.Count);
				bool active = _dropItemList.Count > _dropGridScroller.GetMaxItemCountWithouScroll();
				transform.Find("PrevBtn").gameObject.SetActive(active);
				transform.Find("NextBtn").gameObject.SetActive(active);
			}
		}

		private void SetupReviveInfo()
		{
			Transform transform = base.view.transform.Find("Dialog/Content/ReviveConsumePanel");
			int reviveCost = _levelScoreManager.GetReviveCost();
			if (reviveCost > Singleton<PlayerModule>.Instance.playerData.hcoin)
			{
				transform.Find("InfoAvatar").Find("HcoinNum").GetComponent<Text>()
					.color = MiscData.GetColor("WarningRed");
				transform.Find("InfoAvatar").Find("HcoinLabel").GetComponent<Text>()
					.color = MiscData.GetColor("WarningRed");
				base.view.transform.Find("Dialog/Content/ActionBtns/OK/IconRecharge").gameObject.SetActive(true);
				base.view.transform.Find("Dialog/Content/ActionBtns/OK/IconOK").gameObject.SetActive(false);
				base.view.transform.Find("Dialog/Content/ActionBtns/OK/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Tab_Recharge");
			}
			else
			{
				transform.Find("InfoAvatar").Find("HcoinNum").GetComponent<Text>()
					.color = MiscData.GetColor("Blue");
				transform.Find("InfoAvatar").Find("HcoinLabel").GetComponent<Text>()
					.color = MiscData.GetColor("Blue");
				base.view.transform.Find("Dialog/Content/ActionBtns/OK/IconRecharge").gameObject.SetActive(false);
				base.view.transform.Find("Dialog/Content/ActionBtns/OK/IconOK").gameObject.SetActive(true);
				base.view.transform.Find("Dialog/Content/ActionBtns/OK/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_OK");
			}
			transform.Find("InfoAvatar").Find("HcoinNum").GetComponent<Text>()
				.text = reviveCost.ToString();
			transform.Find("InfoAvatar").Find("AvatarFullName").GetComponent<Text>()
				.text = Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatarRuntimeID).avatarDataItem.FullName;
			Transform transform2 = transform.Find("Consume");
			transform2.Find("ReviveTimes").gameObject.SetActive(_levelScoreManager.maxReviveNum != 65535);
			transform2.Find("ReviveTimes/AvaiableTimes").GetComponent<Text>().text = _levelScoreManager.avaiableReviveNum.ToString();
			transform2.Find("ReviveTimes/MaxTimes").GetComponent<Text>().text = _levelScoreManager.maxReviveNum.ToString();
			transform2.Find("Hcoin/Num").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.hcoin.ToString();
			base.view.transform.Find("Dialog/Content/ActionBtns/OK").GetComponent<Button>().interactable = _levelScoreManager.avaiableReviveNum > 0;
		}

		private void OnScrollerChange(Transform trans, int index)
		{
			Vector2 cellSize = _dropGridScroller.grid.GetComponent<GridLayoutGroup>().cellSize;
			trans.SetLocalScaleX(cellSize.x / trans.GetComponent<MonoLevelDropIconButton>().width);
			trans.SetLocalScaleY(cellSize.y / trans.GetComponent<MonoLevelDropIconButton>().height);
			DropItem val = _dropItemList[index];
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int)val.item_id);
			dummyStorageDataItem.level = (int)val.level;
			dummyStorageDataItem.number = (int)val.num;
			trans.GetComponent<CanvasGroup>().alpha = 1f;
			trans.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, null, true);
		}

		private void SetupLayout()
		{
			base.view.transform.Find("Dialog/Content/CurrentGetItems").gameObject.SetActive(allTeamDown);
			base.view.transform.Find("Dialog/Content/Padding").gameObject.SetActive(!allTeamDown);
			base.view.transform.Find("Dialog/Content/ReviveConsumePanel/MiddleLine").gameObject.SetActive(!allTeamDown);
			base.view.transform.Find("Dialog/Content/ReviveConsumePanel/AvatarSelectPanel").gameObject.SetActive(allTeamDown);
			base.view.transform.Find("Dialog/Title/CloseBtn").gameObject.SetActive(!allTeamDown);
		}

		private void InitTeam()
		{
			if (!allTeamDown)
			{
				return;
			}
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			Transform transform = base.view.transform.Find("Dialog/Content/ReviveConsumePanel/AvatarSelectPanel");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (i >= allPlayerAvatars.Count)
				{
					child.gameObject.SetActive(false);
					continue;
				}
				BaseMonoAvatar baseMonoAvatar = allPlayerAvatars[i];
				child.GetComponent<MonoAvatarButton>().InitForReviveButton(baseMonoAvatar);
				child.Find("CDMask").gameObject.SetActive(baseMonoAvatar.GetRuntimeID() != avatarRuntimeID);
			}
		}

		private void SetupTeam(uint selectAvatarRuntimeID)
		{
			avatarRuntimeID = selectAvatarRuntimeID;
			if (!allTeamDown)
			{
				return;
			}
			List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
			Transform transform = base.view.transform.Find("Dialog/Content/ReviveConsumePanel/AvatarSelectPanel");
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (i >= allPlayerAvatars.Count)
				{
					child.gameObject.SetActive(false);
					continue;
				}
				BaseMonoAvatar baseMonoAvatar = allPlayerAvatars[i];
				child.Find("CDMask").gameObject.SetActive(baseMonoAvatar.GetRuntimeID() != avatarRuntimeID);
			}
		}

		private void OnGiveUpConfirm()
		{
			Singleton<LevelManager>.Instance.SetPause(false);
			Destroy();
			Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.EndLose, EvtLevelState.LevelEndReason.EndLoseAllDead));
		}
	}
}
