using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class MonoStigmataFigure : MonoBehaviour
	{
		private StigmataDataItem _stigmata;

		public void SetupView(StigmataDataItem stigmata)
		{
			_stigmata = stigmata;
			SetupPrefIntoContainer(base.transform.Find("PrefContainer"), stigmata);
		}

		public void SetupViewWithIdentifyStatus(StigmataDataItem stigmata)
		{
			_stigmata = stigmata;
			SetupViewWithIdentifyStatus(stigmata, false);
		}

		public void SetupViewWithIdentifyStatus(StigmataDataItem stigmata, bool forceLock)
		{
			_stigmata = stigmata;
			Transform transform = base.transform.Find("PrefContainer");
			Transform transform2 = base.transform.Find("Mask/PrefContainer");
			Transform transform3 = base.transform.Find("Mask");
			SetupPrefIntoContainer(transform, stigmata);
			if (stigmata.IsAffixIdentify && !forceLock)
			{
				SetImageAttrForAllChildren(transform, null, Color.white);
				transform.gameObject.SetActive(true);
				transform3.gameObject.SetActive(false);
				return;
			}
			transform.gameObject.SetActive(false);
			Material mat = Miscs.LoadResource<Material>("Material/ImageColorize");
			SetImageAttrForAllChildren(transform, mat, Color.white);
			transform3.gameObject.SetActive(true);
			SetupPrefIntoContainer(transform2, stigmata);
			Material mat2 = Miscs.LoadResource<Material>("Material/ImageMonoColor");
			SetImageAttrForAllChildren(transform2, mat2, MiscData.GetColor("DarkBlue"));
		}

		private void SetupPrefIntoContainer(Transform containerTrans, StigmataDataItem stigmata)
		{
			containerTrans.DestroyChildren();
			GameObject gameObject = Object.Instantiate(Miscs.LoadResource<GameObject>(stigmata.GetImagePath()));
			gameObject.transform.SetParent(containerTrans, false);
			gameObject.gameObject.SetActive(true);
			gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		}

		private void SetImageAttrForAllChildren(Transform trans, Material mat, Color color)
		{
			Image[] componentsInChildren = trans.GetComponentsInChildren<Image>();
			if (componentsInChildren != null)
			{
				int i = 0;
				for (int num = componentsInChildren.Length; i < num; i++)
				{
					componentsInChildren[i].material = mat;
					componentsInChildren[i].color = color;
				}
			}
		}

		public void OnStigmataFigureClick()
		{
			Singleton<MainUIManager>.Instance.ShowDialog(new DropNewItemDialogContext(_stigmata, false, true));
		}
	}
}
