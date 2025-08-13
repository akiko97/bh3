using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoDropNewItemColor : MonoBehaviour
	{
		public void SetColorBlack()
		{
			Color color = MiscData.GetColor("TotalBlack");
			TrySetImageColorWithChildren(color);
			TrySetRawImageColorWithChildren(color);
		}

		public void SetColorWhite()
		{
			Color color = MiscData.GetColor("TotalWhite");
			TrySetImageColorWithChildren(color);
			TrySetRawImageColorWithChildren(color);
		}

		private void TrySetImageColorWithChildren(Color color)
		{
			Image component = GetComponent<Image>();
			if (component != null)
			{
				component.color = color;
			}
			Image[] componentsInChildren = GetComponentsInChildren<Image>();
			if (componentsInChildren != null)
			{
				int i = 0;
				for (int num = componentsInChildren.Length; i < num; i++)
				{
					componentsInChildren[i].color = color;
				}
			}
		}

		private void TrySetRawImageColorWithChildren(Color color)
		{
			RawImage component = GetComponent<RawImage>();
			if (component != null)
			{
				component.color = color;
			}
			RawImage[] componentsInChildren = GetComponentsInChildren<RawImage>();
			if (componentsInChildren != null)
			{
				int i = 0;
				for (int num = componentsInChildren.Length; i < num; i++)
				{
					componentsInChildren[i].color = color;
				}
			}
		}
	}
}
