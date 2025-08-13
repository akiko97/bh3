using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoAvatarButtonContainer : MonoBehaviour
	{
		public enum State
		{
			Idle = 0,
			Animating = 1
		}

		public enum FastForwardType
		{
			Btn_1_ClickWith_2_Avatar = 0,
			Btn_1_ClickWith_3_Avatar = 1,
			Btn_2_ClickWith_3_Avatar = 2
		}

		private const string AVATAR_BUTTON_PREFAB_PATH = "UI/Menus/Widget/InLevel/AvatarButton";

		private const float BUTTON_ANIM_DURATION = 0.25f;

		[NonSerialized]
		public List<MonoAvatarButton> avatarBtnList;

		private uint _localRuntimeIDBefore;

		private uint _localRuntimeIDAfter;

		public State _state;

		private FastForwardType _fastForwardType;

		private Coroutine _buttonAnimateCoroutine;

		public void PlaySwapAvatarAnim(uint localRuntimeIDBefore, uint localRuntimeIDAfter)
		{
			if (localRuntimeIDBefore == localRuntimeIDAfter)
			{
				return;
			}
			if (_state == State.Animating)
			{
				StopCoroutine(_buttonAnimateCoroutine);
				_buttonAnimateCoroutine = null;
				FastForwardBtnAnimation();
			}
			_state = State.Animating;
			_localRuntimeIDBefore = localRuntimeIDBefore;
			_localRuntimeIDAfter = localRuntimeIDAfter;
			MonoAvatarButton monoAvatarButton = avatarBtnList.Find((MonoAvatarButton x) => x.avatarRuntimeID == _localRuntimeIDAfter);
			if (!monoAvatarButton.gameObject.activeInHierarchy)
			{
				OnSwapAvatarAnimEnd();
				return;
			}
			switch (monoAvatarButton.index)
			{
			case 1:
				if (avatarBtnList.Count == 2)
				{
					_buttonAnimateCoroutine = StartCoroutine(PlayClickBtn1With2AvtarAnim());
				}
				else if (avatarBtnList.Count == 3)
				{
					_buttonAnimateCoroutine = StartCoroutine(PlayClickBtn1With3AvatarAnim());
				}
				break;
			case 2:
				_buttonAnimateCoroutine = StartCoroutine(PlayClickBtn2With3AvatarAnim());
				break;
			}
		}

		public void AddAvatarButton(uint runtimeID)
		{
			if (avatarBtnList == null)
			{
				avatarBtnList = new List<MonoAvatarButton>();
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>("UI/Menus/Widget/InLevel/AvatarButton"));
			gameObject.transform.SetParent(base.transform, false);
			MonoAvatarButton component = gameObject.GetComponent<MonoAvatarButton>();
			component.Init(runtimeID);
			avatarBtnList.Add(component);
			ResortAvatarBtns(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
			if (Singleton<AvatarManager>.Instance.IsLocalAvatar(runtimeID))
			{
				StartCoroutine(HideFirstLocalAvatarButton(component));
			}
		}

		private IEnumerator HideFirstLocalAvatarButton(MonoAvatarButton button)
		{
			button.GetComponent<CanvasGroup>().alpha = 0f;
			yield return null;
			button.GetComponent<CanvasGroup>().alpha = 1f;
			button.gameObject.SetActive(false);
		}

		public void SetButtonAvailable(bool available)
		{
			int i = 0;
			for (int count = avatarBtnList.Count; i < count; i++)
			{
				avatarBtnList[i].canChange = available;
			}
		}

		private void ResortAvatarBtns(uint localAvatarID)
		{
			int num = 1;
			foreach (MonoAvatarButton avatarBtn in avatarBtnList)
			{
				int num2 = ((avatarBtn.avatarRuntimeID != localAvatarID) ? num++ : avatarBtnList.Count);
				avatarBtn.transform.name = num2.ToString();
				avatarBtn.transform.SetSiblingIndex(num2 - 1);
				avatarBtn.SetIndex(num2);
			}
			avatarBtnList.Sort((MonoAvatarButton left, MonoAvatarButton right) => left.index - right.index);
		}

		private void SetAvatarButtonActive(uint runtimeID, bool active)
		{
			MonoAvatarButton monoAvatarButton = avatarBtnList.Find((MonoAvatarButton x) => x.avatarRuntimeID == runtimeID);
			monoAvatarButton.gameObject.SetActive(active);
			monoAvatarButton.OnSetActive(active);
		}

		public MonoAvatarButton GetAvatarButtonByRuntimeID(uint runtimeID)
		{
			return avatarBtnList.Find((MonoAvatarButton x) => x.avatarRuntimeID == runtimeID);
		}

		private void OnSwapAvatarAnimEnd()
		{
			SetAvatarButtonActive(_localRuntimeIDBefore, true);
			SetAvatarButtonActive(_localRuntimeIDAfter, false);
			ResortAvatarBtns(_localRuntimeIDAfter);
			_localRuntimeIDBefore = 0u;
			_localRuntimeIDAfter = 0u;
			GetComponent<GridLayoutGroup>().enabled = true;
			_buttonAnimateCoroutine = null;
			_state = State.Idle;
		}

		private void FastForwardBtnAnimation()
		{
			if (_fastForwardType == FastForwardType.Btn_1_ClickWith_2_Avatar)
			{
				avatarBtnList[1].gameObject.SetActive(true);
				StopRewindAndClearAlpha(avatarBtnList[0]);
				StopRewindAndClearAlpha(avatarBtnList[1]);
			}
			else if (_fastForwardType == FastForwardType.Btn_1_ClickWith_3_Avatar)
			{
				avatarBtnList[2].gameObject.SetActive(true);
				StopRewindAndClearAlpha(avatarBtnList[0]);
				StopRewindAndClearAlpha(avatarBtnList[1]);
				StopRewindAndClearAlpha(avatarBtnList[2]);
			}
			else if (_fastForwardType == FastForwardType.Btn_2_ClickWith_3_Avatar)
			{
				avatarBtnList[2].gameObject.SetActive(true);
				StopRewindAndClearAlpha(avatarBtnList[1]);
				StopRewindAndClearAlpha(avatarBtnList[2]);
			}
			OnSwapAvatarAnimEnd();
			SetButtonAvailable(true);
		}

		private void StopRewindAndClearAlpha(MonoAvatarButton button)
		{
			Animation component = button.GetComponent<Animation>();
			component.Stop();
			component.Sample();
			button.GetComponent<CanvasGroup>().alpha = 1f;
		}

		private IEnumerator PlayClickBtn1With2AvtarAnim()
		{
			_fastForwardType = FastForwardType.Btn_1_ClickWith_2_Avatar;
			GetComponent<GridLayoutGroup>().enabled = false;
			yield return null;
			Animation btn1Anim = avatarBtnList[0].GetComponent<Animation>();
			Animation btn2Anim = avatarBtnList[1].GetComponent<Animation>();
			btn2Anim.gameObject.SetActive(true);
			btn1Anim.GetComponent<Animation>().Play("FadeOut");
			btn2Anim.GetComponent<Animation>().Play("FadeInFrom_2");
			SetButtonAvailable(false);
			yield return new WaitForSeconds(0.25f);
			OnSwapAvatarAnimEnd();
			SetButtonAvailable(true);
		}

		private IEnumerator PlayClickBtn1With3AvatarAnim()
		{
			_fastForwardType = FastForwardType.Btn_1_ClickWith_3_Avatar;
			GetComponent<GridLayoutGroup>().enabled = false;
			yield return null;
			Animation btn1Anim = avatarBtnList[0].GetComponent<Animation>();
			Animation btn2Anim = avatarBtnList[1].GetComponent<Animation>();
			Animation btn3Anim = avatarBtnList[2].GetComponent<Animation>();
			btn3Anim.gameObject.SetActive(true);
			btn1Anim.GetComponent<Animation>().Play("FadeOut");
			btn2Anim.GetComponent<Animation>().Play("MoveUpFrom_2");
			btn3Anim.GetComponent<Animation>().Play("FadeInFrom_3");
			SetButtonAvailable(false);
			yield return new WaitForSeconds(0.25f);
			OnSwapAvatarAnimEnd();
			SetButtonAvailable(true);
		}

		private IEnumerator PlayClickBtn2With3AvatarAnim()
		{
			_fastForwardType = FastForwardType.Btn_2_ClickWith_3_Avatar;
			GetComponent<GridLayoutGroup>().enabled = false;
			yield return null;
			Animation btn2Anim = avatarBtnList[1].GetComponent<Animation>();
			Animation btn3Anim = avatarBtnList[2].GetComponent<Animation>();
			btn3Anim.gameObject.SetActive(true);
			btn2Anim.GetComponent<Animation>().Play("FadeOut");
			btn3Anim.GetComponent<Animation>().Play("FadeInFrom_3");
			SetButtonAvailable(false);
			yield return new WaitForSeconds(0.25f);
			OnSwapAvatarAnimEnd();
			SetButtonAvailable(true);
		}

		private void OnEnable()
		{
			if (_state == State.Animating && _buttonAnimateCoroutine != null)
			{
				StopCoroutine(_buttonAnimateCoroutine);
				_buttonAnimateCoroutine = null;
				FastForwardBtnAnimation();
			}
		}
	}
}
