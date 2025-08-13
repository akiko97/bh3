using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailRendererSortLayer : MonoBehaviour
{
	public string sortingOrderName = "Default";

	public int sortingOrder;

	private void Start()
	{
		TrailRenderer component = GetComponent<TrailRenderer>();
		component.sortingLayerName = sortingOrderName;
		component.sortingOrder = sortingOrder;
	}
}
