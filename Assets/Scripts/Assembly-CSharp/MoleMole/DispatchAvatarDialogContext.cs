using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class DispatchAvatarDialogContext : BaseDialogContext
	{
		private List<AvatarDataItem> showAvatarList;

		public int selectedAvatarID;

		public int teamEditIndex;

		public VentureDataItem ventureData;

		public DispatchAvatarDialogContext(VentureDataItem ventureData, int index)
		{
			config = new ContextPattern
			{
				contextName = "DispatchAvatarDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/CabinDispatchAvatarDialog"
			};
			this.ventureData = ventureData;
			teamEditIndex = index;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.SelectAvtarIconChange)
			{
				return UpdateSelectedAvatar((int)ntf.body);
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/ListPanel/NextBtn").GetComponent<Button>(), OnNextBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ListPanel/PrevBtn").GetComponent<Button>(), OnPrevBtnClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), Close);
		}

		protected override bool SetupView()
		{
			if (Singleton<AvatarModule>.Instance.UserAvatarList.Count == 0)
			{
				return false;
			}
			SetupAvatarListPanel();
			if (selectedAvatarID == 0)
			{
				selectedAvatarID = showAvatarList[0].avatarID;
				List<int> memberIdList = ventureData.selectedAvatarList;
				AvatarDataItem avatarDataItem = showAvatarList.Find((AvatarDataItem x) => x.UnLocked && !memberIdList.Contains(x.avatarID) && !Singleton<IslandModule>.Instance.IsAvatarDispatched(x.avatarID) && !MiscData.Config.AvatarClassDoNotShow.Contains(x.ClassId));
				if (avatarDataItem != null)
				{
					selectedAvatarID = avatarDataItem.avatarID;
				}
			}
			SetupSelectedAvatarInfo();
			return false;
		}

		public void Close()
		{
			Destroy();
		}

		public void OnNextBtnClick()
		{
			base.view.transform.Find("Dialog/Content/ListPanel/ScrollView").GetComponent<MonoGridScroller>().ScrollToNextPage();
		}

		public void OnPrevBtnClick()
		{
			base.view.transform.Find("Dialog/Content/ListPanel/ScrollView").GetComponent<MonoGridScroller>().ScrollToPrevPage();
		}

		public void OnInVentureDispatchBtnClick()
		{
			if (teamEditIndex > ventureData.selectedAvatarList.Count)
			{
				ventureData.selectedAvatarList.Add(selectedAvatarID);
			}
			else
			{
				ventureData.selectedAvatarList[teamEditIndex - 1] = selectedAvatarID;
			}
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DispatchAvatarChanged));
			Destroy();
		}

		public void OnOutVentureDispatchClick()
		{
			ventureData.selectedAvatarList.RemoveAt(teamEditIndex - 1);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DispatchAvatarChanged));
			Destroy();
		}

		private void SetupAvatarListPanel()
		{
			showAvatarList = Singleton<AvatarModule>.Instance.UserAvatarList.FindAll((AvatarDataItem x) => x.UnLocked);
			showAvatarList.Sort(CompareByDefault);
			base.view.transform.Find("Dialog/Content/ListPanel/ScrollView").GetComponent<MonoGridScroller>().Init(OnChange, showAvatarList.Count);
			base.view.transform.Find("Dialog/Content/ListPanel/ScrollView/Content").GetComponent<Animation>().Play();
		}

		private void OnChange(Transform trans, int index)
		{
			bool isSelected = showAvatarList[index].avatarID == selectedAvatarID;
			AvatarDataItem avatarDataItem = showAvatarList[index];
			trans.GetComponent<MonoAvatarIcon>().SetupView(avatarDataItem, isSelected);
		}

		private bool UpdateSelectedAvatar(int avatarId)
		{
			selectedAvatarID = avatarId;
			SetupSelectedAvatarInfo();
			return false;
		}

		private void SetupSelectedAvatarInfo()
		{
			base.view.transform.Find("Dialog/Content/ListPanel/ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
			AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(selectedAvatarID);
			Transform transform = base.view.transform.Find("Dialog/Content/InfoPanel");
			SetupClassName(transform.Find("Info_1/ClassName"), avatarByID);
			transform.Find("Info_1/NameText").GetComponent<Text>().text = avatarByID.ShortName;
			transform.Find("Info_1/SmallIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(avatarByID.AttributeIconPath);
			transform.Find("Info_1/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(avatarByID.star);
			transform.Find("Info_2/LevelText").GetComponent<Text>().text = "LV." + avatarByID.level;
			transform.Find("Info_2/Combat/NumText").GetComponent<Text>().text = Mathf.FloorToInt(avatarByID.CombatNum).ToString();
			SetupVentureDispatchBtn();
		}

		private void SetupClassName(Transform parent, AvatarDataItem avatarSelected)
		{
			parent.Find("FirstName").GetComponent<Text>().text = avatarSelected.ClassFirstName;
			parent.Find("FirstName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnFirstName;
			parent.Find("LastName").GetComponent<Text>().text = avatarSelected.ClassLastName;
			parent.Find("LastName/EnText").GetComponent<Text>().text = avatarSelected.ClassEnLastName;
		}

		private void SetupVentureDispatchBtn()
		{
			List<int> selectedAvatarList = ventureData.selectedAvatarList;
			bool flag = selectedAvatarID != 0 && selectedAvatarList.Contains(selectedAvatarID);
			Button component = base.view.transform.Find("Dialog/Content/InfoPanel/Btn").GetComponent<Button>();
			if (teamEditIndex <= selectedAvatarList.Count && selectedAvatarID == selectedAvatarList[teamEditIndex - 1])
			{
				BindViewCallback(component, OnOutVentureDispatchClick);
				component.interactable = true;
				string textID = "Menu_GetOutTeam";
				component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID);
			}
			else
			{
				BindViewCallback(component, OnInVentureDispatchBtnClick);
				component.interactable = !flag;
				string textID2 = ((!flag) ? "Menu_Action_DispatchAvatar" : "Menu_AlreadyInTeam");
				component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID2);
			}
			if (Singleton<IslandModule>.Instance.IsAvatarDispatched(selectedAvatarID))
			{
				component.interactable = false;
				component.transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_AvatarAlreadyDispatched");
			}
		}

		private int CompareByDefault(AvatarDataItem lemb, AvatarDataItem remb)
		{
			if (!lemb.UnLocked && remb.UnLocked)
			{
				return 1;
			}
			if (!remb.UnLocked && lemb.UnLocked)
			{
				return -1;
			}
			return (!lemb.UnLocked) ? CompareByFragment(lemb, remb) : CompareByStar(lemb, remb);
		}

		private int CompareByFragment(AvatarDataItem lemb, AvatarDataItem remb)
		{
			int num = -(lemb.fragment - remb.fragment);
			if (num != 0)
			{
				return num;
			}
			return CompareByID(lemb, remb);
		}

		private int CompareByStar(AvatarDataItem lemb, AvatarDataItem remb)
		{
			List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList((StageType)1);
			int num = memberList.IndexOf(lemb.avatarID);
			int num2 = memberList.IndexOf(remb.avatarID);
			if (num == -1 && num2 >= 0)
			{
				return 1;
			}
			if (num2 == -1 && num >= 0)
			{
				return -1;
			}
			if (num >= 0 && num2 >= 0)
			{
				return num - num2;
			}
			int num3 = -(lemb.star - remb.star);
			if (num3 != 0)
			{
				return num3;
			}
			return CompareByID(lemb, remb);
		}

		private int CompareByID(AvatarDataItem lemb, AvatarDataItem remb)
		{
			return lemb.avatarID - remb.avatarID;
		}
	}
}
