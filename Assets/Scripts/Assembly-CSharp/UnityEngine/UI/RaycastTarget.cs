namespace UnityEngine.UI
{
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/RaycastTarget")]
	[RequireComponent(typeof(RectTransform))]
	public class RaycastTarget : Graphic, ICanvasRaycastFilter
	{
		private RectTransform m_RectTransform;

		public new RectTransform rectTransform
		{
			get
			{
				return m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>());
			}
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			toFill.Clear();
		}

		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return true;
		}
	}
}
