using System;
using UnityEngine;

namespace MoleMole
{
	[ExecuteInEditMode]
	public class MonoItemAttributeDiff : MonoBehaviour
	{
		public bool showKeepIcon;

		private AvatarDataItem _avatarData;

		private StorageDataItemBase _itemBefore;

		private StorageDataItemBase _itemAfter;

		private Action<Transform, float, float> _setupAttr;

		public void SetupView(AvatarDataItem avatarData, StorageDataItemBase itemBefore, StorageDataItemBase itemAfter, Action<Transform, float, float> setupAttr)
		{
			_avatarData = avatarData;
			_itemBefore = itemBefore;
			_itemAfter = itemAfter;
			_setupAttr = setupAttr;
			SetupStatus();
		}

		private void SetupStatus()
		{
			float arg = 0f;
			float arg2 = 0f;
			if (_itemBefore != null)
			{
				arg = ((!(_itemBefore is StigmataDataItem)) ? _itemBefore.GetHPAdd() : (_itemBefore as StigmataDataItem).GetHPAddWithAffix(_avatarData));
			}
			if (_itemAfter != null)
			{
				arg2 = ((!(_itemAfter is StigmataDataItem)) ? _itemAfter.GetHPAdd() : (_itemAfter as StigmataDataItem).GetHPAddWithAffix(_avatarData));
			}
			if (_setupAttr != null)
			{
				_setupAttr(base.transform.Find("HP"), arg, arg2);
			}
			float arg3 = 0f;
			float arg4 = 0f;
			if (_itemBefore != null)
			{
				arg3 = ((!(_itemBefore is StigmataDataItem)) ? _itemBefore.GetSPAdd() : (_itemBefore as StigmataDataItem).GetSPAddWithAffix(_avatarData));
			}
			if (_itemAfter != null)
			{
				arg4 = ((!(_itemAfter is StigmataDataItem)) ? _itemAfter.GetSPAdd() : (_itemAfter as StigmataDataItem).GetSPAddWithAffix(_avatarData));
			}
			if (_setupAttr != null)
			{
				_setupAttr(base.transform.Find("SP"), arg3, arg4);
			}
			float arg5 = 0f;
			float arg6 = 0f;
			if (_itemBefore != null)
			{
				arg5 = ((!(_itemBefore is StigmataDataItem)) ? _itemBefore.GetAttackAdd() : (_itemBefore as StigmataDataItem).GetAttackAddWithAffix(_avatarData));
			}
			if (_itemAfter != null)
			{
				arg6 = ((!(_itemAfter is StigmataDataItem)) ? _itemAfter.GetAttackAdd() : (_itemAfter as StigmataDataItem).GetAttackAddWithAffix(_avatarData));
			}
			if (_setupAttr != null)
			{
				_setupAttr(base.transform.Find("ATK"), arg5, arg6);
			}
			float arg7 = 0f;
			float arg8 = 0f;
			if (_itemBefore != null)
			{
				arg7 = ((!(_itemBefore is StigmataDataItem)) ? _itemBefore.GetDefenceAdd() : (_itemBefore as StigmataDataItem).GetDefenceAddWithAffix(_avatarData));
			}
			if (_itemAfter != null)
			{
				arg8 = ((!(_itemAfter is StigmataDataItem)) ? _itemAfter.GetDefenceAdd() : (_itemAfter as StigmataDataItem).GetDefenceAddWithAffix(_avatarData));
			}
			if (_setupAttr != null)
			{
				_setupAttr(base.transform.Find("DEF"), arg7, arg8);
			}
			float arg9 = 0f;
			float arg10 = 0f;
			if (_itemBefore != null)
			{
				arg9 = ((!(_itemBefore is StigmataDataItem)) ? _itemBefore.GetCriticalAdd() : (_itemBefore as StigmataDataItem).GetCriticalAddWithAffix(_avatarData));
			}
			if (_itemAfter != null)
			{
				arg10 = ((!(_itemAfter is StigmataDataItem)) ? _itemAfter.GetCriticalAdd() : (_itemAfter as StigmataDataItem).GetCriticalAddWithAffix(_avatarData));
			}
			if (_setupAttr != null)
			{
				_setupAttr(base.transform.Find("CRT"), arg9, arg10);
			}
		}
	}
}
