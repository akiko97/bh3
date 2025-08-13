using UnityEngine;

public class MonoBodyPart : MonoBehaviour
{
	public BodyPartType type;

	private IBodyPartTouchable _bodyPartTouchable;

	private Collider _collider;

	private int _preTouchCount;

	public void SetBodyPartTouchable(IBodyPartTouchable touchable)
	{
		_bodyPartTouchable = touchable;
	}

	private void Start()
	{
		_collider = GetComponent<Collider>();
	}

	private void Update()
	{
		if (_collider == null)
		{
			return;
		}
		bool flag = Input.touches.Length > 0 && _preTouchCount == 0;
		Vector3 position = ((!flag) ? Vector3.zero : new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, 0f));
		if (Input.GetMouseButtonDown(0))
		{
			flag = true;
			position = Input.mousePosition;
		}
		if (flag)
		{
			Ray ray = Camera.main.ScreenPointToRay(position);
			RaycastHit hitInfo = default(RaycastHit);
			if (_collider.Raycast(ray, out hitInfo, 9999.9f) && _bodyPartTouchable != null)
			{
				_bodyPartTouchable.BodyPartTouched(type, hitInfo.point);
			}
		}
		_preTouchCount = Input.touches.Length;
	}
}
