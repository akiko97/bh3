using System;
using UnityEngine;

[Serializable]
public class FreeCameraCtrl : MonoBehaviour
{
	public float headSpeed;

	public float pitchSpeed;

	public float moveSpeed;

	public float zoomSpeed;

	public float pitchBoundMin;

	public float pitchBoundMax;

	public bool useMoveBounds;

	public float moveBounds;

	public float rotateSmoothing;

	public float moveSmoothing;

	public float fov;

	public float distance;

	private Vector2 euler;

	private float idleTime;

	private Quaternion targetRot;

	private Vector3 targetLookAt;

	private float targetDist;

	private Vector3 distanceVec;

	private Transform target;

	private Rect inputBounds;

	public Rect paramInputBounds;

	public bool usePivotPoint;

	public Vector3 pivotPoint;

	public Transform pivotTransform;

	public FreeCameraCtrl()
	{
		headSpeed = 250f;
		pitchSpeed = 120f;
		moveSpeed = 10f;
		zoomSpeed = 30f;
		pitchBoundMin = -89f;
		pitchBoundMax = 89f;
		useMoveBounds = true;
		moveBounds = 100f;
		rotateSmoothing = 0.5f;
		moveSmoothing = 0.7f;
		fov = 60f;
		distance = 2f;
		distanceVec = new Vector3(0f, 0f, 0f);
		paramInputBounds = new Rect(0f, 0f, 1f, 1f);
		usePivotPoint = true;
		pivotPoint = new Vector3(0f, 2f, 0f);
	}

	public virtual void Start()
	{
		Vector3 eulerAngles = transform.eulerAngles;
		euler.x = eulerAngles.y;
		euler.y = eulerAngles.x;
		euler.y = Mathf.Repeat(euler.y + 180f, 360f) - 180f;
		GameObject gameObject = new GameObject("_FreeCameraTarget");
		gameObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
		target = gameObject.transform;
		if (usePivotPoint)
		{
			target.position = pivotPoint;
			targetDist = (transform.position - target.position).magnitude;
		}
		else if (pivotTransform != null)
		{
			usePivotPoint = true;
			Vector3 v = transform.worldToLocalMatrix.MultiplyPoint3x4(pivotTransform.position);
			v.x = 0f;
			v.y = 0f;
			targetDist = v.z;
			target.position = transform.localToWorldMatrix.MultiplyPoint3x4(v);
		}
		else
		{
			target.position = transform.position + transform.forward * distance;
			targetDist = distance;
		}
		targetRot = transform.rotation;
		targetLookAt = target.position;
		idleTime = 0f;
	}

	public virtual void Update()
	{
		inputBounds.x = (float)GetComponent<Camera>().pixelWidth * paramInputBounds.x;
		inputBounds.y = (float)GetComponent<Camera>().pixelHeight * paramInputBounds.y;
		inputBounds.width = (float)GetComponent<Camera>().pixelWidth * paramInputBounds.width;
		inputBounds.height = (float)GetComponent<Camera>().pixelHeight * paramInputBounds.height;
		float num = 0f;
		float num2 = 0f;
		if (!target || !inputBounds.Contains(Input.mousePosition))
		{
			return;
		}
		if (Input.multiTouchEnabled && Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Began)
		{
			num += Input.GetTouch(0).deltaPosition.x * 0.1f;
			num2 += Input.GetTouch(0).deltaPosition.y * 0.1f;
		}
		bool num3 = Input.GetMouseButton(0);
		if (!num3)
		{
			num3 = Input.touchCount == 1;
		}
		bool flag = num3;
		bool num4 = Input.GetMouseButton(1);
		if (!num4)
		{
			num4 = Input.touchCount == 2;
		}
		bool flag2 = num4;
		bool num5 = Input.GetMouseButton(2);
		if (!num5)
		{
			num5 = Input.touchCount == 3;
		}
		bool flag3 = num5;
		bool flag4 = flag;
		bool num6 = flag3;
		if (!num6)
		{
			num6 = flag;
			if (num6)
			{
				num6 = Input.GetKey(KeyCode.LeftControl);
				if (!num6)
				{
					num6 = Input.GetKey(KeyCode.RightControl);
				}
			}
		}
		bool flag5 = num6;
		bool flag6 = flag2;
		if (flag5)
		{
			num = num * moveSpeed * 0.005f * targetDist;
			num2 = num2 * moveSpeed * 0.005f * targetDist;
			targetLookAt -= transform.up * num2 + transform.right * num;
			if (useMoveBounds)
			{
				targetLookAt.x = Mathf.Clamp(targetLookAt.x, 0f - moveBounds, moveBounds);
				targetLookAt.y = Mathf.Clamp(targetLookAt.y, 0f - moveBounds, moveBounds);
				targetLookAt.z = Mathf.Clamp(targetLookAt.z, 0f - moveBounds, moveBounds);
			}
			idleTime = 0f;
		}
		else if (flag6)
		{
			num2 = num2 * zoomSpeed * 0.005f * targetDist;
			targetDist += num2;
			targetDist = Mathf.Max(0.1f, targetDist);
			fov += num * 1.5f;
			fov = Mathf.Clamp(fov, 3f, 140f);
			idleTime = 0f;
		}
		else if (flag4)
		{
			num = num * headSpeed * 0.02f;
			num2 = num2 * pitchSpeed * 0.02f;
			euler.x += num;
			euler.y -= num2;
			euler.y = ClampAngle(euler.y, pitchBoundMin, pitchBoundMax);
			targetRot = Quaternion.Euler(euler.y, euler.x, 0f);
			idleTime = 0f;
		}
		targetDist -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * 0.5f;
		targetDist = Mathf.Clamp(targetDist, 1f, 100f);
	}

	public virtual void FixedUpdate()
	{
		if (!(idleTime <= 5f))
		{
			euler.x += 0.15f;
			targetRot = Quaternion.Euler(euler.y, euler.x, 0f);
		}
		idleTime += Time.deltaTime;
		distance = moveSmoothing * targetDist + (1f - moveSmoothing) * distance;
		Camera.main.fieldOfView = fov;
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSmoothing);
		target.position = Vector3.Lerp(target.position, targetLookAt, moveSmoothing);
		distanceVec.z = distance;
		transform.position = target.position - transform.rotation * distanceVec;
	}

	public virtual float ClampAngle(float angle, float min, float max)
	{
		if (!(angle >= -360f))
		{
			angle += 360f;
		}
		if (!(angle <= 360f))
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public virtual void Main()
	{
	}
}
