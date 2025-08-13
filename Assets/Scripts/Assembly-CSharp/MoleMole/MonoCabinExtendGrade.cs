using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(GridLayoutGroup))]
	public class MonoCabinExtendGrade : MonoBehaviour
	{
		private const int MAX_STAR = 5;

		public bool HideDarkStar = true;

		public int star;

		private Color _darkColor;

		private Color _lightColor;

		public void SetupView(int star)
		{
			this.star = star;
			GridLayoutGroup component = GetComponent<GridLayoutGroup>();
			InitColor();
			component.startCorner = GridLayoutGroup.Corner.UpperLeft;
			component.childAlignment = TextAnchor.MiddleLeft;
			for (int i = 1; i <= 5; i++)
			{
				Transform transform = base.transform.Find(i.ToString());
				Image component2 = transform.GetComponent<Image>();
				if (i <= star)
				{
					component2.color = _lightColor;
					transform.gameObject.SetActive(true);
				}
				else
				{
					component2.color = _darkColor;
					transform.gameObject.SetActive(!HideDarkStar);
				}
			}
		}

		private void InitColor()
		{
			_darkColor = UIUtil.SetupColor("#00009B37");
			_lightColor = UIUtil.SetupColor("#FFFFFFFF");
		}
	}
}
