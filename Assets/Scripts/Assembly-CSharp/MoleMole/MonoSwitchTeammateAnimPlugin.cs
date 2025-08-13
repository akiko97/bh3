using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoSwitchTeammateAnimPlugin : MonoBehaviour
	{
		private bool _bSwitchAnim;

		private GameObject _switchAnimObj;

		private Vector2 _animFrom;

		private Vector2 _animTo;

		private float _animDuration = 0.25f;

		private float _animEclapsed;

		private int _animFromIndex;

		private int _animToIndex;

		private List<MonoTeamMember> _memberList;

		public RefreshTeammateUI_Handler _OnRefreshTeammateUI;

		public void RegisterCallback(RefreshTeammateUI_Handler refreshTeammateUIHandler)
		{
			_OnRefreshTeammateUI = refreshTeammateUIHandler;
		}

		public bool IsPlaying()
		{
			return _bSwitchAnim;
		}

		private void Start()
		{
			_memberList = new List<MonoTeamMember>();
			Transform transform = base.transform.Find("TeamPanel/Team");
			for (int i = 1; i <= 3; i++)
			{
				GameObject gameObject = transform.Find(i.ToString()).gameObject;
				MonoTeamMember component = gameObject.GetComponent<MonoTeamMember>();
				_memberList.Add(component);
			}
		}

		public void StartSwitchAnim(int dataIndex, int fromIndex, int toIndex)
		{
			_bSwitchAnim = true;
			_switchAnimObj = CreateAnimIcon(dataIndex);
			_animFrom = GetTeamMember(fromIndex).GetComponent<RectTransform>().anchoredPosition;
			_animTo = GetTeamMember(toIndex).GetComponent<RectTransform>().anchoredPosition;
			_animEclapsed = 0f;
			_animFromIndex = fromIndex;
			_animToIndex = toIndex;
		}

		private void Update()
		{
			if (_bSwitchAnim && _switchAnimObj != null)
			{
				_animEclapsed += Time.deltaTime;
				float num = _animEclapsed / _animDuration;
				if (num < 1f)
				{
					_switchAnimObj.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(_animFrom, _animTo, num);
				}
				else
				{
					OnSwitchAnimEnd();
				}
			}
		}

		private void OnSwitchAnimEnd()
		{
			_switchAnimObj.GetComponent<RectTransform>().anchoredPosition = _animTo;
			_bSwitchAnim = false;
			Object.Destroy(_switchAnimObj);
			_switchAnimObj = null;
			bool bSelfSkill = _animFromIndex == 1 || _animToIndex == 1;
			if (_OnRefreshTeammateUI != null)
			{
				_OnRefreshTeammateUI(_animToIndex, bSelfSkill);
			}
		}

		private GameObject CreateAnimIcon(int dataIndex)
		{
			GameObject gameObject = null;
			gameObject = GetTeamMember(dataIndex).gameObject;
			GameObject gameObject2 = Object.Instantiate(gameObject);
			gameObject2.transform.SetParent(gameObject.transform.parent, false);
			gameObject2.transform.Find("ChangeIcon").gameObject.SetActive(false);
			gameObject2.transform.Find("Btn").gameObject.SetActive(false);
			gameObject2.transform.Find("BG/LeaderTopBound").gameObject.SetActive(false);
			gameObject2.GetComponent<RectTransform>().anchoredPosition = gameObject.GetComponent<RectTransform>().anchoredPosition;
			return gameObject2;
		}

		private MonoTeamMember GetTeamMember(int index)
		{
			foreach (MonoTeamMember member in _memberList)
			{
				if (member.GetIndex() == index)
				{
					return member;
				}
			}
			return null;
		}
	}
}
