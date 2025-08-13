using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using proto;

namespace MoleMole
{
	public class MonoTeamMember : MonoBehaviour, IEventSystemHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
	{
		private const string AVATAR_NULL_BG_PATH = "SpriteOutput/AvatarTachie/BgType4";

		private AvatarDataItem _avatarData;

		private int _index;

		private StageType _levelType;

		private RectTransform _baseRect;

		private Camera _camera;

		private GameObject _objDrag;

		private bool _enableDrag = true;

		private Sprite _oldBGSprite;

		private MonoSwitchTeammateAnimPlugin _animPlugin;

		private List<MonoTeamMember> _otherTeamMembers = new List<MonoTeamMember>();

		public RefreshTeammateUI_Handler _OnRefreshTeammateUI;

		public StartSwitchAnim_Handler _OnStartSwitchAnim;

		public void SetupView(StageType levelType, int index, MonoSwitchTeammateAnimPlugin animPlugin, AvatarDataItem avatarData = null, RectTransform baseRect = null)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Invalid comparison between Unknown and I4
			_levelType = levelType;
			_index = index;
			_avatarData = avatarData;
			_baseRect = baseRect;
			_animPlugin = animPlugin;
			bool flag = _index == 1;
			base.transform.Find("BG/LeaderTopBound").gameObject.SetActive(flag);
			base.transform.Find("BG/MemberTopBound").gameObject.SetActive(!flag);
			base.transform.Find("Content").gameObject.SetActive(avatarData != null);
			if (avatarData != null)
			{
				base.transform.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarAttributeBGSpriteList[_avatarData.Attribute]);
				SetupAvatar();
			}
			else
			{
				base.transform.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/AvatarTachie/BgType4");
			}
			_otherTeamMembers.Clear();
			Transform transform = _baseRect.transform.Find("TeamPanel/Team");
			for (int i = 1; i <= 3; i++)
			{
				if (i != _index)
				{
					MonoTeamMember component = transform.Find(i.ToString()).GetComponent<MonoTeamMember>();
					_otherTeamMembers.Add(component);
				}
			}
			if ((int)levelType == 4)
			{
				base.transform.Find("HPRemain").gameObject.SetActive(avatarData != null);
				if (avatarData != null)
				{
					base.transform.Find("HPRemain").GetComponent<MonoRemainHP>().SetAvatarHPData(Singleton<EndlessModule>.Instance.GetEndlessAvatarHPData(avatarData.avatarID));
				}
			}
			bool upAvatarDispatched = avatarData != null && Singleton<IslandModule>.Instance.IsAvatarDispatched(avatarData.avatarID);
			SetUpAvatarDispatched(upAvatarDispatched);
		}

		public void OnClick()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Invalid comparison between Unknown and I4
			int selectedAvatarID = ((_avatarData != null) ? _avatarData.avatarID : 0);
			Singleton<MainUIManager>.Instance.ShowPage(new AvatarOverviewPageContext
			{
				type = AvatarOverviewPageContext.PageType.TeamEdit,
				selectedAvatarID = selectedAvatarID,
				teamEditIndex = _index,
				levelType = _levelType,
				showAvatarRemainHP = ((int)_levelType == 4)
			});
		}

		private void SetupAvatar()
		{
			base.transform.Find("Content/StarPanel/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(_avatarData.star);
			base.transform.Find("Content/LVNum").GetComponent<Text>().text = _avatarData.level.ToString();
			base.transform.Find("Content/Avatar").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_avatarData.AvatarTachie);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (_enableDrag && Singleton<PlayerModule>.Instance.playerData.HasTeamMember(_levelType, _index) && !_animPlugin.IsPlaying())
			{
				_objDrag = CreateDragIcon();
				base.transform.Find("Content").gameObject.SetActive(false);
				_oldBGSprite = base.transform.Find("BG/BGColor").GetComponent<Image>().sprite;
				base.transform.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/AvatarTachie/BgType4");
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (_enableDrag && !(_objDrag == null))
			{
				Vector2 localPoint;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(_baseRect, eventData.position, GetCamera(), out localPoint);
				_objDrag.GetComponent<RectTransform>().anchoredPosition = localPoint;
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			if (!_enableDrag || _objDrag == null)
			{
				return;
			}
			Object.Destroy(_objDrag);
			_objDrag = null;
			MonoTeamMember monoTeamMember = IsHoverMember(eventData);
			if (monoTeamMember != null && Singleton<PlayerModule>.Instance.playerData.HasTeamMember(_levelType, monoTeamMember.GetIndex()))
			{
				if (_OnStartSwitchAnim != null)
				{
					_OnStartSwitchAnim(monoTeamMember.GetIndex(), monoTeamMember.GetIndex(), _index);
				}
				Singleton<PlayerModule>.Instance.playerData.SwitchTeamMember(_levelType, monoTeamMember.GetIndex(), GetIndex());
				Singleton<NetworkManager>.Instance.NotifyUpdateAvatarTeam(_levelType);
				if (_OnRefreshTeammateUI != null)
				{
					_OnRefreshTeammateUI(monoTeamMember.GetIndex(), false);
				}
			}
			else
			{
				base.transform.Find("BG/BGColor").GetComponent<Image>().sprite = _oldBGSprite;
				base.transform.Find("Content").gameObject.SetActive(true);
			}
		}

		public void RegisterCallback(RefreshTeammateUI_Handler refreshTeammateUIHandler, StartSwitchAnim_Handler startSwitchAnimHandler)
		{
			_OnRefreshTeammateUI = refreshTeammateUIHandler;
			_OnStartSwitchAnim = startSwitchAnimHandler;
		}

		private GameObject CreateDragIcon()
		{
			GameObject gameObject = Object.Instantiate(base.gameObject);
			gameObject.transform.SetParent(Singleton<MainUIManager>.Instance.SceneCanvas.transform, false);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchorMin = new Vector2(0.5f, 0.5f);
			component.anchorMax = new Vector2(0.5f, 0.5f);
			component.pivot = new Vector2(0.5f, 0.5f);
			gameObject.transform.Find("ChangeIcon").gameObject.SetActive(false);
			gameObject.transform.Find("Btn").gameObject.SetActive(false);
			gameObject.transform.Find("BG/LeaderTopBound").gameObject.SetActive(false);
			gameObject.transform.Find("BG/HightLightFrame").gameObject.SetActive(true);
			return gameObject;
		}

		private MonoTeamMember IsHoverMember(PointerEventData eventData)
		{
			foreach (MonoTeamMember otherTeamMember in _otherTeamMembers)
			{
				RectTransform component = otherTeamMember.GetComponent<RectTransform>();
				Vector2 localPoint;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(component, eventData.position, GetCamera(), out localPoint);
				if (InRectTransform(component, localPoint))
				{
					return otherTeamMember;
				}
			}
			return null;
		}

		private bool InRectTransform(RectTransform rect, Vector2 localPos)
		{
			if (localPos.x >= 0f && localPos.x <= rect.sizeDelta.x && Mathf.Abs(localPos.y) <= rect.sizeDelta.y / 2f)
			{
				return true;
			}
			return false;
		}

		private Camera GetCamera()
		{
			if (_camera == null)
			{
				_camera = GameObject.Find("UICamera").GetComponent<Camera>();
			}
			return _camera;
		}

		public int GetIndex()
		{
			return _index;
		}

		private void SetUpAvatarDispatched(bool isDispatched)
		{
			base.transform.Find("Content/Avatar").GetComponent<Image>().color = ((!isDispatched) ? MiscData.GetColor("TotalWhite") : MiscData.GetColor("EndlessEnergyRunout"));
			base.transform.Find("Content/LVLabel").gameObject.SetActive(!isDispatched);
			base.transform.Find("Content/LVNum").gameObject.SetActive(!isDispatched);
			base.transform.Find("Content/Hint").gameObject.SetActive(isDispatched);
		}
	}
}
