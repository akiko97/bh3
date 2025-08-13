using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(ImageSmoothMask))]
	public class ImageForSmoothMask : Image
	{
		private ImageSmoothMask _mask;

		protected override void OnEnable()
		{
			_mask = GetComponent<ImageSmoothMask>();
			base.OnEnable();
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (_mask.coverRatio < 0.01f)
			{
				return;
			}
			if (_mask.coverRatio > 0.99f)
			{
				base.OnPopulateMesh(vh);
			}
			else if (Application.isPlaying)
			{
				List<UIVertex[]> list = _mask.GenerateUIQuads();
				for (int i = 0; i < list.Count; i++)
				{
					vh.AddUIVertexQuad(list[i]);
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			base.sprite = null;
		}
	}
}
