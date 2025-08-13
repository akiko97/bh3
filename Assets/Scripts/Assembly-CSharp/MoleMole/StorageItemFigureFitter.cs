using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class StorageItemFigureFitter : MonoBehaviour
	{
		public void SetTheFigureFitTheContent(string imagePath)
		{
			GameObject gameObject = Miscs.LoadResource<GameObject>(imagePath);
			float num = gameObject.GetComponent<SpriteRenderer>().sprite.rect.width;
			float num2 = gameObject.GetComponent<SpriteRenderer>().sprite.rect.height;
			float width = (base.transform as RectTransform).rect.width;
			if (num > width)
			{
				num2 *= width / num;
				num = width;
			}
			base.transform.Find("Image").GetComponent<Image>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
			(base.transform.Find("Image") as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num);
			(base.transform.Find("Image") as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2);
		}
	}
}
