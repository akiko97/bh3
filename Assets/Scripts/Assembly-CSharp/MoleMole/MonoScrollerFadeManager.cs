using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoScrollerFadeManager : MonoBehaviour
	{
		private List<RectTransform> _itemList;

		private ScrollRect _scroller;

		[SerializeField]
		private float FadeInDuration = 0.5f;

		[SerializeField]
		private float NextItemFadeInDelay = 0.3f;

		[SerializeField]
		private float FirstDelay = 0.1f;

		public void Init(Dictionary<int, RectTransform> itemDict, Dictionary<int, RectTransform> oldItemList, Func<RectTransform, RectTransform, bool> isEqual)
		{
			_scroller = GetComponent<ScrollRect>();
			SortedDictionary<int, RectTransform> sortedDictionary = new SortedDictionary<int, RectTransform>();
			foreach (KeyValuePair<int, RectTransform> item in itemDict)
			{
				sortedDictionary.Add(item.Key, item.Value);
			}
			if (oldItemList == null)
			{
				_itemList = sortedDictionary.Values.ToList();
				return;
			}
			sortedDictionary.Clear();
			foreach (KeyValuePair<int, RectTransform> item2 in itemDict)
			{
				bool flag = true;
				if (oldItemList.ContainsValue(item2.Value))
				{
					flag = !oldItemList.ContainsKey(item2.Key) || !isEqual(item2.Value, oldItemList[item2.Key]);
				}
				if (flag)
				{
					sortedDictionary.Add(item2.Key, item2.Value);
				}
			}
			_itemList = sortedDictionary.Values.ToList();
		}

		public void Play()
		{
			StartCoroutine(PlayAll());
		}

		public void Reset()
		{
			if (!(_scroller != null) || _scroller.enabled)
			{
				return;
			}
			foreach (RectTransform item in _itemList)
			{
				CanvasGroup component = item.GetComponent<CanvasGroup>();
				component.alpha = 1f;
			}
			EndFadeIn();
		}

		private IEnumerator PlayAll()
		{
			BeginFadeIn();
			yield return new WaitForSeconds(FirstDelay);
			int num = 0;
			int itemIndex = 0;
			while (itemIndex < _itemList.Count)
			{
				List<RectTransform> transList = new List<RectTransform>();
				for (; itemIndex < _itemList.Count && _itemList[itemIndex] == null; itemIndex++)
				{
				}
				if (itemIndex >= _itemList.Count)
				{
					EndFadeIn();
					continue;
				}
				float prePositionY = _itemList[itemIndex].localPosition.y;
				for (; itemIndex < _itemList.Count; itemIndex++)
				{
					if (_itemList[itemIndex] == null)
					{
						itemIndex++;
						break;
					}
					if (_itemList[itemIndex].localPosition.y == prePositionY)
					{
						transList.Add(_itemList[itemIndex]);
						continue;
					}
					break;
				}
				StartCoroutine(PlayStep(transList, num, itemIndex));
				num++;
			}
			EndFadeIn();
		}

		private IEnumerator PlayStep(List<RectTransform> tranList, int num, int lastItemIndex)
		{
			float delay = (float)num * NextItemFadeInDelay;
			yield return new WaitForSeconds(delay);
			if (tranList == null || tranList.Count < 1)
			{
				if (lastItemIndex >= _itemList.Count - 1)
				{
					EndFadeIn();
				}
				yield break;
			}
			float timer = 0f;
			List<CanvasGroup> itemCanvasGroupList = new List<CanvasGroup>();
			foreach (RectTransform rectTrans in tranList)
			{
				if (rectTrans != null)
				{
					itemCanvasGroupList.Add(rectTrans.GetComponent<CanvasGroup>());
				}
			}
			while (timer <= FadeInDuration)
			{
				timer += Time.deltaTime;
				foreach (CanvasGroup canvasGroup in itemCanvasGroupList)
				{
					if (canvasGroup == null)
					{
						if (lastItemIndex >= _itemList.Count - 1)
						{
							EndFadeIn();
						}
						yield break;
					}
					canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / FadeInDuration);
				}
				yield return null;
			}
			foreach (CanvasGroup canvasGroup2 in itemCanvasGroupList)
			{
				if (canvasGroup2 == null)
				{
					if (lastItemIndex >= _itemList.Count - 1)
					{
						EndFadeIn();
					}
					yield break;
				}
				canvasGroup2.alpha = 1f;
			}
			if (lastItemIndex >= _itemList.Count - 1)
			{
				EndFadeIn();
			}
		}

		private void BeginFadeIn()
		{
			_scroller.enabled = false;
			foreach (RectTransform item in _itemList)
			{
				CanvasGroup component = item.GetComponent<CanvasGroup>();
				component.alpha = 0f;
			}
		}

		private void EndFadeIn()
		{
			if (_scroller == null)
			{
				_scroller = GetComponent<ScrollRect>();
			}
			_scroller.enabled = true;
		}
	}
}
