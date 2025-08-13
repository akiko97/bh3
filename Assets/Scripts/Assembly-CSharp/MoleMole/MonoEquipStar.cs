using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(GridLayoutGroup))]
	public class MonoEquipStar : MonoBehaviour
	{
		public enum Dircetion
		{
			LeftToRight = 0,
			RightToLeft = 1,
			Center = 2
		}

		private const int MAX_STAR = 7;

		public Dircetion dirction;

		public int star;

		public void Awake()
		{
			SetupView(star);
		}

		public void SetupView(int star)
		{
			this.star = star;
			GridLayoutGroup component = GetComponent<GridLayoutGroup>();
			switch (dirction)
			{
			case Dircetion.LeftToRight:
				component.startCorner = GridLayoutGroup.Corner.UpperLeft;
				component.childAlignment = TextAnchor.MiddleLeft;
				break;
			case Dircetion.RightToLeft:
				component.startCorner = GridLayoutGroup.Corner.UpperRight;
				component.childAlignment = TextAnchor.MiddleRight;
				break;
			case Dircetion.Center:
				component.startCorner = GridLayoutGroup.Corner.UpperLeft;
				component.childAlignment = TextAnchor.MiddleCenter;
				break;
			}
			for (int i = 1; i <= 7; i++)
			{
				base.transform.Find(i.ToString()).gameObject.SetActive(i <= star);
			}
		}
	}
}
