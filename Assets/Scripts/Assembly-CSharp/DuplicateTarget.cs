using UnityEngine;

public class DuplicateTarget : MonoBehaviour
{
	public GameObject target;

	public int count;

	private void Start()
	{
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = Object.Instantiate(target);
			gameObject.transform.parent = base.gameObject.transform;
		}
	}
}
