using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[ExecuteInEditMode]
	public class MonoImageFitter : MonoBehaviour
	{
		public Image image;

		public bool fitX = true;

		public bool fitY;

		public void Start()
		{
			FitImageSize();
		}

		public void FitImageSize()
		{
			if (!(image == null) && !(image.sprite == null))
			{
				float num = image.sprite.rect.width;
				float num2 = image.sprite.rect.height;
				Rect rect = GetComponent<RectTransform>().rect;
				if (fitX && num > rect.width)
				{
					num2 *= rect.width / num;
					num = rect.width;
				}
				if (fitY && num2 > rect.height)
				{
					num *= rect.height / num2;
					num2 = rect.height;
				}
				image.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num);
				image.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2);
			}
		}
	}
}
