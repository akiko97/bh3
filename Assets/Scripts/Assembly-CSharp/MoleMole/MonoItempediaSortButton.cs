using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(Button))]
	public class MonoItempediaSortButton : MonoBehaviour
	{
		private Action Clicked;

		public void SetClickCallback(Action cb)
		{
			Clicked = cb;
		}

		public void OnClick()
		{
			if (Clicked != null)
			{
				Clicked();
			}
		}

		public void SetupView(bool selected, bool asent)
		{
			Image component = base.transform.GetComponent<Image>();
			bool flag = (component.enabled = selected);
			component.color = ((!flag) ? Color.white : MiscData.GetColor("Yellow"));
			base.transform.Find("Text").GetComponent<Text>().color = ((!flag) ? Color.white : MiscData.GetColor("Black"));
			base.transform.Find("Order").gameObject.SetActive(flag);
			base.transform.Find("Order/UpImg").gameObject.SetActive(asent);
			base.transform.Find("Order/DownImg").gameObject.SetActive(!asent);
		}
	}
}
