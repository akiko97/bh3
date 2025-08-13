using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoFadeInAnimManager : MonoBehaviour
	{
		[Serializable]
		public class FadeInItem
		{
			public enum Type
			{
				Normal = 0,
				FadeOut = 1,
				WithMoveLeft = 2,
				WithMoveRight = 3
			}

			public enum Curve
			{
				Linear = 0,
				Quadratic = 1
			}

			public GameObject gameObject;

			public Type type;

			public Curve curve;

			[HideInInspector]
			public Vector2 anchoredPosition;
		}

		[Serializable]
		public class StepItem
		{
			public FadeInItem[] fadeInList;
		}

		[Serializable]
		public class AnimationItem
		{
			public string name;

			public float ALPHA_FADE_DURATION = 0.2f;

			public float MOVE_FADE_DURATION = 0.2f;

			public float STEP_INTERVAL = 0.05f;

			public float MOVE_DISTANCE = 50f;

			public bool isReverseOrder;

			public Action AnimationAllEndCallBack;

			public StepItem[] stepList;

			[HideInInspector]
			public bool isPlaying;
		}

		public AnimationItem[] animationList = new AnimationItem[0];

		private Dictionary<string, AnimationItem> _animMap;

		public void Awake()
		{
			_animMap = new Dictionary<string, AnimationItem>();
			AnimationItem[] array = animationList;
			foreach (AnimationItem animationItem in array)
			{
				_animMap.Add(animationItem.name, animationItem);
			}
			InitAllFadeInItem();
		}

		public void Play(string name, bool isReverseOrder = false, Action endCallBack = null)
		{
			AnimationItem animationItem = _animMap[name];
			animationItem.isReverseOrder = isReverseOrder;
			animationItem.AnimationAllEndCallBack = endCallBack;
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(PlayAnim(animationItem));
			}
		}

		public bool IsAnimationPlaying(string name)
		{
			AnimationItem animationItem = _animMap[name];
			return animationItem.isPlaying;
		}

		private IEnumerator PlayAnim(AnimationItem anim)
		{
			anim.isPlaying = true;
			if (!anim.isReverseOrder)
			{
				HideAllObjectInAnim(anim);
			}
			for (int i = 0; i < anim.stepList.Length; i++)
			{
				if (base.gameObject.activeInHierarchy)
				{
					StartCoroutine(PlayStep(anim, anim.stepList[(!anim.isReverseOrder) ? i : (anim.stepList.Length - i - 1)]));
				}
				int nextIndex = i + 1;
				if (nextIndex < anim.stepList.Length && CheckNeedWaitNextStep(anim.stepList[(!anim.isReverseOrder) ? nextIndex : (anim.stepList.Length - nextIndex - 1)]))
				{
					yield return new WaitForSeconds(anim.STEP_INTERVAL);
				}
			}
			anim.isPlaying = false;
			anim.isReverseOrder = false;
			if (anim.AnimationAllEndCallBack != null)
			{
				anim.AnimationAllEndCallBack();
				anim.AnimationAllEndCallBack = null;
			}
		}

		private bool CheckNeedWaitNextStep(StepItem step)
		{
			bool result = false;
			FadeInItem[] fadeInList = step.fadeInList;
			foreach (FadeInItem fadeInItem in fadeInList)
			{
				if (fadeInItem.gameObject.activeSelf)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private IEnumerator PlayStep(AnimationItem anim, StepItem step)
		{
			float timer = 0f;
			while (timer <= anim.ALPHA_FADE_DURATION)
			{
				FadeInItem[] fadeInList = step.fadeInList;
				foreach (FadeInItem fadeIn in fadeInList)
				{
					if (fadeIn.gameObject.activeSelf)
					{
						CanvasGroup canvasGroup = fadeIn.gameObject.GetComponent<CanvasGroup>();
						timer += Time.unscaledDeltaTime;
						if (fadeIn.type != FadeInItem.Type.FadeOut)
						{
							float ratio = ((!anim.isReverseOrder) ? (timer / anim.ALPHA_FADE_DURATION) : ((anim.ALPHA_FADE_DURATION - timer) / anim.ALPHA_FADE_DURATION));
							canvasGroup.alpha = Mathf.Lerp(0f, 1f, ratio);
						}
						else
						{
							float ratio2 = ((!anim.isReverseOrder) ? ((anim.ALPHA_FADE_DURATION - timer) / anim.ALPHA_FADE_DURATION) : (timer / anim.ALPHA_FADE_DURATION));
							canvasGroup.alpha = Mathf.Lerp(0f, 1f, ratio2);
						}
						if (fadeIn.type == FadeInItem.Type.WithMoveLeft || fadeIn.type == FadeInItem.Type.WithMoveRight)
						{
							float delta = ((fadeIn.type != FadeInItem.Type.WithMoveLeft) ? (0f - anim.MOVE_DISTANCE) : anim.MOVE_DISTANCE);
							float progress_normalized = ((!anim.isReverseOrder) ? (timer / anim.MOVE_FADE_DURATION) : ((anim.ALPHA_FADE_DURATION - timer) / anim.ALPHA_FADE_DURATION));
							float position_normalized = 0f;
							float x = Mathf.Lerp(t: (fadeIn.curve != FadeInItem.Curve.Quadratic) ? progress_normalized : (2f * progress_normalized - progress_normalized * progress_normalized), a: fadeIn.anchoredPosition.x + delta, b: fadeIn.anchoredPosition.x);
							float y = fadeIn.anchoredPosition.y;
							fadeIn.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
						}
					}
				}
				yield return null;
			}
			FadeInItem[] fadeInList2 = step.fadeInList;
			foreach (FadeInItem fadeIn2 in fadeInList2)
			{
				if (fadeIn2.gameObject.activeSelf)
				{
					CanvasGroup canvasGroup2 = fadeIn2.gameObject.GetComponent<CanvasGroup>();
					canvasGroup2.alpha = ((fadeIn2.type != FadeInItem.Type.FadeOut) ? 1 : 0);
					if (fadeIn2.type == FadeInItem.Type.WithMoveLeft || fadeIn2.type == FadeInItem.Type.WithMoveRight)
					{
						fadeIn2.gameObject.GetComponent<RectTransform>().anchoredPosition = fadeIn2.anchoredPosition;
					}
				}
			}
		}

		private void InitAllFadeInItem()
		{
			AnimationItem[] array = animationList;
			foreach (AnimationItem animationItem in array)
			{
				StepItem[] stepList = animationItem.stepList;
				foreach (StepItem stepItem in stepList)
				{
					FadeInItem[] fadeInList = stepItem.fadeInList;
					foreach (FadeInItem fadeInItem in fadeInList)
					{
						fadeInItem.anchoredPosition = fadeInItem.gameObject.GetComponent<RectTransform>().anchoredPosition;
					}
				}
			}
		}

		private void HideAllObjectInAnim(AnimationItem anim)
		{
			StepItem[] stepList = anim.stepList;
			foreach (StepItem stepItem in stepList)
			{
				FadeInItem[] fadeInList = stepItem.fadeInList;
				foreach (FadeInItem fadeInItem in fadeInList)
				{
					if (fadeInItem.gameObject.activeSelf)
					{
						CanvasGroup component = fadeInItem.gameObject.GetComponent<CanvasGroup>();
						if (fadeInItem.type == FadeInItem.Type.FadeOut)
						{
							component.alpha = 1f;
						}
						else
						{
							component.alpha = 0f;
						}
					}
				}
			}
		}
	}
}
