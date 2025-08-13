using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[ExecuteInEditMode]
	public class MonoAttributeDisplay : MonoBehaviour
	{
		private StorageDataItemBase _item;

		private AvatarDataItem _ownerData;

		private AvatarDataItem _avatarData;

		private readonly float[] spacingX = new float[4] { 0f, 0f, 140f, 36f };

		private readonly float posX_two_Column = -23f;

		public void SetupView(StorageDataItemBase item, AvatarDataItem ownerData = null)
		{
			_item = item;
			_ownerData = ownerData;
			SetupStatus();
			SetupGrid();
		}

		public void SetupView(AvatarDataItem avatarData)
		{
			_avatarData = avatarData;
			SetupStatus();
			SetupGrid();
		}

		private void SetupGrid()
		{
			int num = 0;
			foreach (Transform item in base.transform)
			{
				if (item.gameObject.activeSelf)
				{
					num++;
				}
			}
			GridLayoutGroup component = base.transform.GetComponent<GridLayoutGroup>();
			int num2 = ((num <= 3) ? 1 : 2);
			int num3 = Mathf.CeilToInt(1f * (float)num / (float)num2);
			RectTransform component2 = GetComponent<RectTransform>();
			float x = ((num3 != 2) ? 0f : posX_two_Column);
			component2.anchoredPosition = new Vector2(x, component2.anchoredPosition.y);
			component.constraintCount = num2;
			component.spacing = new Vector2(spacingX[num3], component.spacing.y);
		}

		private void SetupStatus()
		{
			if (_item != null)
			{
				StigmataDataItem stigmataDataItem = _item as StigmataDataItem;
				float num = _item.GetHPAdd();
				if (stigmataDataItem != null)
				{
					num = stigmataDataItem.GetHPAddWithAffix(_ownerData);
				}
				base.transform.Find("HP").gameObject.SetActive(num > 0f);
				base.transform.Find("HP/Num").GetComponent<Text>().text = Mathf.FloorToInt(num).ToString();
				float num2 = _item.GetSPAdd();
				if (stigmataDataItem != null)
				{
					num2 = stigmataDataItem.GetSPAddWithAffix(_ownerData);
				}
				base.transform.Find("SP").gameObject.SetActive(num2 > 0f);
				base.transform.Find("SP/Num").GetComponent<Text>().text = Mathf.FloorToInt(num2).ToString();
				float num3 = _item.GetAttackAdd();
				if (stigmataDataItem != null)
				{
					num3 = stigmataDataItem.GetAttackAddWithAffix(_ownerData);
				}
				base.transform.Find("ATK").gameObject.SetActive(num3 > 0f);
				base.transform.Find("ATK/Num").GetComponent<Text>().text = Mathf.FloorToInt(num3).ToString();
				float num4 = _item.GetDefenceAdd();
				if (stigmataDataItem != null)
				{
					num4 = stigmataDataItem.GetDefenceAddWithAffix(_ownerData);
				}
				base.transform.Find("DEF").gameObject.SetActive(num4 > 0f);
				base.transform.Find("DEF/Num").GetComponent<Text>().text = Mathf.FloorToInt(num4).ToString();
				float num5 = _item.GetCriticalAdd();
				if (stigmataDataItem != null)
				{
					num5 = stigmataDataItem.GetCriticalAddWithAffix(_ownerData);
				}
				base.transform.Find("CRT").gameObject.SetActive(num5 > 0f);
				base.transform.Find("CRT/Num").GetComponent<Text>().text = Mathf.FloorToInt(num5).ToString();
				if (base.transform.Find("Cost") != null)
				{
					base.transform.Find("Cost").gameObject.SetActive(_item.GetCost() > 0);
					base.transform.Find("Cost/Num").GetComponent<Text>().text = Mathf.FloorToInt(_item.GetCost()).ToString();
				}
			}
			else if (_avatarData != null)
			{
				base.transform.Find("HP").gameObject.SetActive(_avatarData.FinalHPUI > 0f);
				base.transform.Find("HP/Num").GetComponent<Text>().text = Mathf.FloorToInt(_avatarData.FinalHPUI).ToString();
				base.transform.Find("SP").gameObject.SetActive(_avatarData.FinalSPUI > 0f);
				base.transform.Find("SP/Num").GetComponent<Text>().text = Mathf.FloorToInt(_avatarData.FinalSPUI).ToString();
				base.transform.Find("ATK").gameObject.SetActive(_avatarData.FinalAttackUI > 0f);
				base.transform.Find("ATK/Num").GetComponent<Text>().text = Mathf.FloorToInt(_avatarData.FinalAttackUI).ToString();
				base.transform.Find("DEF").gameObject.SetActive(_avatarData.FinalDefenseUI > 0f);
				base.transform.Find("DEF/Num").GetComponent<Text>().text = Mathf.FloorToInt(_avatarData.FinalDefenseUI).ToString();
				base.transform.Find("CRT").gameObject.SetActive(_avatarData.FinalDefenseUI > 0f);
				base.transform.Find("CRT/Num").GetComponent<Text>().text = Mathf.FloorToInt(_avatarData.FinalCriticalUI).ToString();
				if (base.transform.Find("Cost") != null)
				{
					base.transform.Find("Cost").gameObject.SetActive(_avatarData.GetCurrentCost() > 0);
					base.transform.Find("Cost/Num").GetComponent<Text>().text = Mathf.FloorToInt(_avatarData.GetCurrentCost()).ToString();
				}
			}
		}
	}
}
