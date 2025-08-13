using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(GridLayoutGroup))]
	public class MonoEquipSubStar : MonoBehaviour
	{
		public enum Dircetion
		{
			LeftToRight = 0,
			RightToLeft = 1,
			Center = 2
		}

		public Dircetion dirction;

		public int maxStars = 7;

		public int activeStars;

		private string _activeImageName = "ActiveImage";

		private string _unactiveImageName = "UnactiveImage";

		public void Awake()
		{
			SetupView(activeStars, maxStars);
		}

		public void SetupView(int activeStars, int maxStars)
		{
			this.activeStars = activeStars;
			this.maxStars = maxStars;
			GridLayoutGroup component = GetComponent<GridLayoutGroup>();
			bool flag = false;
			switch (dirction)
			{
			case Dircetion.LeftToRight:
				component.startCorner = GridLayoutGroup.Corner.UpperLeft;
				component.childAlignment = TextAnchor.MiddleLeft;
				break;
			case Dircetion.RightToLeft:
				component.startCorner = GridLayoutGroup.Corner.UpperRight;
				component.childAlignment = TextAnchor.MiddleRight;
				flag = true;
				break;
			case Dircetion.Center:
				component.startCorner = GridLayoutGroup.Corner.UpperLeft;
				component.childAlignment = TextAnchor.MiddleCenter;
				break;
			}
			foreach (Transform item in base.transform)
			{
				int num = int.Parse(item.name);
				bool flag2 = (!flag && num <= maxStars) || (flag && num > base.transform.childCount - maxStars);
				item.gameObject.SetActive(flag2);
				if (flag2)
				{
					bool flag3 = (!flag && num <= activeStars) || (flag && base.transform.childCount - num < activeStars);
					item.Find(_activeImageName).gameObject.SetActive(flag3);
					item.Find(_unactiveImageName).gameObject.SetActive(!flag3);
				}
			}
		}
	}
}
