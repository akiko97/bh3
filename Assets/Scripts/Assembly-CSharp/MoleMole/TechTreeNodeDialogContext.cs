using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class TechTreeNodeDialogContext : BaseSequenceDialogContext
	{
		private CabinTechTreeNode _data;

		public TechTreeNodeDialogContext(CabinTechTreeNode data)
		{
			config = new ContextPattern
			{
				contextName = "TechTreeNodeDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/TechTreeNodeDialog"
			};
			_data = data;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, OnBGClick);
			BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), Close);
			BindViewCallback(base.view.transform.Find("Dialog/Content/ActivateBtn").GetComponent<Button>(), OnActivate);
		}

		protected override bool SetupView()
		{
			InitView();
			base.view.transform.Find("Dialog/Content/Info/Title").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(_data._metaData.Title);
			SetupDesc();
			base.view.transform.Find("Dialog/Content/NodeIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_data._metaData.Icon);
			Transform transform = base.view.transform.Find("Dialog/Content/Info/Error/BG/Line1");
			Transform transform2 = base.view.transform.Find("Dialog/Content/Info/Error/BG/Line2");
			if (_data._status == TechTreeNodeStatus.Lock)
			{
				base.view.transform.Find("Dialog/Content/Info/Error").gameObject.SetActive(true);
				List<TechTreeNodeLockInfo> lockInfo = _data.GetLockInfo();
				for (int i = 0; i < lockInfo.Count; i++)
				{
					Transform transform3 = ((i != 0) ? transform2 : transform);
					transform3.gameObject.SetActive(true);
					TechTreeNodeLockInfo techTreeNodeLockInfo = lockInfo[i];
					if (techTreeNodeLockInfo._lockType == TechTreeNodeLock.CabinLevel)
					{
						string cabinName = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType)_data._metaData.Cabin).GetCabinName();
						transform3.GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_TechTreeNodeLevelLack", cabinName, techTreeNodeLockInfo._needLevel.ToString());
					}
					else if (techTreeNodeLockInfo._lockType == TechTreeNodeLock.AvatarLevel || techTreeNodeLockInfo._lockType == TechTreeNodeLock.AvatarUnlock)
					{
						AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(_data._metaData.UnlockAvatarID);
						transform3.GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_TechTreeNodeLevelLack", avatarByID.ShortName, techTreeNodeLockInfo._needLevel.ToString());
						base.view.transform.Find("Dialog/Content/AvatarIcon").gameObject.SetActive(true);
						base.view.transform.Find("Dialog/Content/AvatarIcon/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_AvatarLevelLack");
						base.view.transform.Find("Dialog/Content/AvatarIcon/Image").GetComponent<Image>().sprite = UIUtil.GetAvatarCardIcon(_data._metaData.UnlockAvatarID);
					}
				}
			}
			else if (_data._status == TechTreeNodeStatus.Unlock_Ban_Active)
			{
				base.view.transform.Find("Dialog/Content/Info/Error").gameObject.SetActive(true);
				transform.gameObject.SetActive(true);
				transform.GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_NeedActiveNeibour");
			}
			else if (_data._status == TechTreeNodeStatus.Unlock_Ready_Active)
			{
				int leftPowerCost = Singleton<IslandModule>.Instance.GetLeftPowerCost();
				if (_data._metaData.PowerCost > leftPowerCost)
				{
					base.view.transform.Find("Dialog/Content/Info/Error").gameObject.SetActive(true);
					transform.gameObject.SetActive(true);
					transform.GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_PowerLack");
				}
				else
				{
					base.view.transform.Find("Dialog/Content/ActivateBtn").GetComponent<Button>().interactable = true;
				}
			}
			else if (_data._status == TechTreeNodeStatus.Active)
			{
				base.view.transform.Find("Dialog/Content/LabelActive").gameObject.SetActive(true);
				base.view.transform.Find("Dialog/Content/ActivateBtn").gameObject.SetActive(false);
			}
			base.view.transform.Find("Dialog/Content/PowerInfo/Num").GetComponent<Text>().text = _data._metaData.PowerCost.ToString();
			return false;
		}

		private void InitView()
		{
			base.view.transform.Find("Dialog/Content/Info/Error").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/AvatarIcon").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/LabelActive").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/ActivateBtn").gameObject.SetActive(true);
			base.view.transform.Find("Dialog/Content/ActivateBtn").GetComponent<Button>().interactable = false;
			base.view.transform.Find("Dialog/Content/Info/Error/BG/Line1").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/Info/Error/BG/Line2").gameObject.SetActive(false);
		}

		private void SetupDesc()
		{
			Text component = base.view.transform.Find("Dialog/Content/Info/Desc/Text").GetComponent<Text>();
			if (_data._metaData.AbilityType == 1)
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc, (float)_data._metaData.Argument2 / 100f);
			}
			else if (_data._metaData.AbilityType == 2)
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc, _data._metaData.Argument1);
			}
			else if (_data._metaData.AbilityType == 3)
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc, _data._metaData.Argument1);
			}
			else if (_data._metaData.AbilityType == 4)
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc, _data._metaData.Argument1, MiscData.Config.VentureDifficultyDesc[_data._metaData.Argument2]);
			}
			else if (_data._metaData.AbilityType == 5)
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc, _data._metaData.Argument1);
			}
			else if (_data._metaData.AbilityType == 6)
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc);
			}
			else if (_data._metaData.AbilityType == 7)
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc, _data._metaData.Argument1);
			}
			else if (_data._metaData.AbilityType == 8)
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc, _data._metaData.Argument1);
			}
			else if (_data._metaData.AbilityType == 9)
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc, (float)_data._metaData.Argument1 / 100f);
			}
			else if (_data._metaData.AbilityType == 10)
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc, (float)_data._metaData.Argument1 / 100f);
			}
			else if (_data._metaData.AbilityType == 11)
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc, _data._metaData.Argument1);
			}
			else if (_data._metaData.AbilityType == 14)
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc, _data._metaData.Argument1);
			}
			else if (_data._metaData.AbilityType == 13)
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc, _data._metaData.Argument1);
			}
			else
			{
				component.text = LocalizationGeneralLogic.GetText(_data._metaData.Desc);
			}
		}

		private void OnActivate()
		{
			Singleton<NetworkManager>.Instance.RequestAddCabinTech((CabinType)_data._metaData.Cabin, _data._metaData.X, _data._metaData.Y);
			Destroy();
		}

		private void OnBGClick(BaseEventData evtData = null)
		{
			Destroy();
		}

		private void Close()
		{
			Destroy();
		}
	}
}
