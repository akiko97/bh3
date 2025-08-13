using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoFaceEffect : MonoBehaviour
	{
		public FaceEffectItem[] items;

		private void Awake()
		{
			List<FaceEffectItem> list = new List<FaceEffectItem>();
			foreach (Transform item in base.transform)
			{
				FaceEffectItem faceEffectItem = new FaceEffectItem();
				faceEffectItem.name = item.gameObject.name;
				faceEffectItem.effect = item.gameObject;
				list.Add(faceEffectItem);
				faceEffectItem.effect.SetActive(false);
			}
			items = list.ToArray();
		}
	}
}
