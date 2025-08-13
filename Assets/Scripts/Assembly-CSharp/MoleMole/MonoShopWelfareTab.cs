using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoShopWelfareTab : MonoBehaviour
	{
		private Transform _scrollViewTrans;

		private List<WelfareDataItem> _welfareDataItemList;

		private Action _onGetBtnClick;

		public void SetupView(Action onGetBtnClick = null)
		{
			_onGetBtnClick = onGetBtnClick;
			_scrollViewTrans = base.transform.Find("ScrollView");
			_welfareDataItemList = Singleton<ShopWelfareModule>.Instance.GetWelfareDataItemList();
			MonoGridScroller component = _scrollViewTrans.GetComponent<MonoGridScroller>();
			component.Init(OnScrollChange, _welfareDataItemList.Count, new Vector2(0f, 1f));
			if (_scrollViewTrans.gameObject.activeInHierarchy)
			{
				MonoScrollerFadeManager component2 = _scrollViewTrans.GetComponent<MonoScrollerFadeManager>();
				component2.Init(component.GetItemDict(), null, IsWelfareDataItemEqual);
				component2.Play();
			}
		}

		private void OnScrollChange(Transform trans, int index)
		{
			WelfareDataItem welfareDataItem = _welfareDataItemList[index];
			trans.GetComponent<MonoWelfareItem>().SetupView(welfareDataItem, _onGetBtnClick);
		}

		private bool IsWelfareDataItemEqual(RectTransform dataNew, RectTransform dataOld)
		{
			if (dataNew == null || dataOld == null)
			{
				return false;
			}
			MonoWelfareItem component = dataOld.GetComponent<MonoWelfareItem>();
			MonoWelfareItem component2 = dataNew.GetComponent<MonoWelfareItem>();
			return component2.GetWelfareDataItem().vipLevel == component.GetWelfareDataItem().vipLevel;
		}
	}
}
