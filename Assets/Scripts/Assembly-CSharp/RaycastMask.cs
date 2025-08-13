using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(Collider2D))]
public class RaycastMask : MonoBehaviour, ICanvasRaycastFilter
{
	private Collider2D myCollider;

	private RectTransform rectTransform;

	private void Awake()
	{
		myCollider = GetComponent<Collider2D>();
		rectTransform = GetComponent<RectTransform>();
	}

	public bool IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
	{
		Vector3 worldPoint = Vector3.zero;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPos, eventCamera, out worldPoint);
		return myCollider.OverlapPoint(worldPoint);
	}
}
