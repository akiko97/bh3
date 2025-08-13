using UnityEngine;
using UnityEngine.EventSystems;

namespace MoleMole
{
	[RequireComponent(typeof(Camera))]
	public class MainMenuCameraNavigater : MonoBehaviour
	{
		public enum State
		{
			Idle = 0,
			Drag = 1,
			Reset = 2
		}

		private const float lowPassFilterFactor = 0.2f;

		private const float RESET_TIME = 0.2f;

		public Transform target;

		public float Distance = 4f;

		public float headSpeed = 250f;

		public float pitchSpeed = 120f;

		public Vector2 angleBounds = new Vector2(5f, 5f);

		public float rotateSmoothing = 0.5f;

		public float restoreSmoothing = 0.2f;

		public Rect paramInputBounds = new Rect(0f, 0f, 1f, 1f);

		public Transform pilot1;

		public Transform pilot2;

		public Transform pilot3;

		public float amplitudeX = 1f;

		public float amplitudeY = 1f;

		public float pilotShakeSpeed = 0.02f;

		private Vector3 pilotPosition1 = new Vector3(0f, 0f, 0f);

		private Vector3 pilotPosition2 = new Vector3(0f, 0f, 0f);

		private Vector3 pilotPosition3 = new Vector3(0f, 0f, 0f);

		private Vector3 pilotOffset1 = new Vector3(0f, 0f, 0f);

		private Vector3 pilotOffset2 = new Vector3(0f, 0f, 0f);

		private Vector3 pilotOffset3 = new Vector3(0f, 0f, 0f);

		private Vector2 euler;

		private Vector2 origEuler;

		private Quaternion origRot;

		private Quaternion targetRot;

		public AnimationCurve PilotShakeX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public AnimationCurve PilotShakeY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public AnimationCurve GyroSensitivityCurve;

		protected Quaternion baseAttitude;

		private Quaternion referenceAttitude = Quaternion.Euler(2f, 180f, 0f);

		private Quaternion referanceRotation = Quaternion.identity;

		public float ParallexRange = 2.5f;

		public float ParallexSensitivity = 0.05f;

		public float ParallexBoundHardness = 0.5f;

		private float _time;

		public State state;

		private float resetTimer;

		private void Start()
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			euler.x = eulerAngles.y;
			euler.y = eulerAngles.x;
			euler.y = Mathf.Repeat(euler.y + 180f, 360f) - 180f;
			origEuler = euler;
			origRot = (targetRot = base.transform.rotation);
			if (target != null)
			{
				base.transform.position = target.position - base.transform.forward * Distance;
			}
			state = State.Idle;
			GameObject gameObject = GameObject.Find("MainMenu_SpaceShip");
			if (pilot1 == null && gameObject != null)
			{
				pilot1 = gameObject.transform.Find("Warship/Warship_ControlDesk01");
			}
			if (pilot2 == null && gameObject != null)
			{
				pilot2 = gameObject.transform.Find("Warship/Warship_ControlDesk02");
			}
			if (pilot3 == null && gameObject != null)
			{
				pilot3 = gameObject.transform.Find("Warship/Warship_ControlDesk03");
			}
			if (pilot1 != null)
			{
				pilotPosition1 = pilot1.transform.position;
			}
			if (pilot2 != null)
			{
				pilotPosition2 = pilot2.transform.position;
			}
			if (pilot3 != null)
			{
				pilotPosition3 = pilot3.transform.position;
			}
			PilotShakeX.preWrapMode = WrapMode.Loop;
			PilotShakeX.postWrapMode = WrapMode.Loop;
			PilotShakeY.preWrapMode = WrapMode.Loop;
			PilotShakeY.postWrapMode = WrapMode.Loop;
			Input.gyro.enabled = GraphicsSettingData.IsEnableGyroscope();
			baseAttitude = Input.gyro.attitude;
		}

		public void OnDrag(BaseEventData evt)
		{
			if (evt is PointerEventData)
			{
				Vector2 delta = (evt as PointerEventData).delta;
				delta.x *= headSpeed * 0.02f;
				delta.y *= pitchSpeed * 0.02f;
				euler.x += delta.x;
				euler.y -= delta.y;
				euler.x = ClampAngle(euler.x, origEuler.x - angleBounds.x, origEuler.x + angleBounds.x);
				euler.y = ClampAngle(euler.y, origEuler.y - angleBounds.y, origEuler.y + angleBounds.y);
				targetRot = Quaternion.Euler(euler.y, euler.x, 0f);
				state = State.Drag;
			}
		}

		public void OnDragEnd(BaseEventData evt)
		{
			if (evt is PointerEventData)
			{
				state = State.Reset;
				resetTimer = 0f;
			}
		}

		private float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
			return Mathf.Clamp(angle, min, max);
		}

		private static Quaternion ConvertRotation(Quaternion q)
		{
			return new Quaternion(q.x, q.y, 0f - q.z, 0f - q.w);
		}

		private void Update()
		{
			if (!(pilot1 == null) && !(pilot2 == null) && !(pilot3 == null))
			{
				_time += Time.deltaTime * pilotShakeSpeed;
				pilotOffset1.y = PilotShakeY.Evaluate(0.7f + _time) * amplitudeY;
				pilotOffset2.y = PilotShakeY.Evaluate(_time * 0.994f) * amplitudeY;
				pilotOffset3.y = PilotShakeY.Evaluate(0.1f + _time * 1.031f) * amplitudeY;
				pilotOffset1.x = PilotShakeX.Evaluate(0.7f + _time * 0.997f) * amplitudeX;
				pilotOffset2.x = PilotShakeX.Evaluate(_time * 0.995f) * amplitudeX;
				pilotOffset3.x = PilotShakeX.Evaluate(0.1f + _time * 1.03f) * amplitudeX;
				pilot1.transform.position = pilotPosition1 + pilotOffset1;
				pilot2.transform.position = pilotPosition2 + pilotOffset2;
				pilot3.transform.position = pilotPosition3 + pilotOffset3;
			}
		}

		private void FixedUpdate()
		{
			MonoGameEntry monoGameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
			if (!(monoGameEntry != null))
			{
				ParallexSensitivity = GyroSensitivityCurve.Evaluate(base.transform.position.z);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, referenceAttitude * ConvertRotation(Quaternion.Inverse(baseAttitude) * referanceRotation * Input.gyro.attitude), ParallexSensitivity);
				Vector3 eulerAngles = base.transform.rotation.eulerAngles;
				eulerAngles.z = 0f;
				base.transform.rotation = Quaternion.Euler(eulerAngles);
				if (Quaternion.Angle(Input.gyro.attitude, baseAttitude) > ParallexRange)
				{
					baseAttitude = Quaternion.Slerp(baseAttitude, Input.gyro.attitude, ParallexBoundHardness);
				}
				if (target != null)
				{
					Vector3 b = target.position - base.transform.forward * Distance;
					base.transform.position = Vector3.Lerp(base.transform.position, b, Time.deltaTime * 5f);
				}
			}
		}
	}
}
