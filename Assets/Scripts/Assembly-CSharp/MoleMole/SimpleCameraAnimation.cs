using BehaviorDesigner.Runtime;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Camera))]
	public class SimpleCameraAnimation : MonoBehaviour
	{
		[Range(1f, 3600f)]
		public float duration = 1f;

		public float attackDelay;

		public float lookAtHeight = 1.7f;

		private static readonly float X_AXIS_LEN = 1000f;

		public AnimationCurve yawAngle = AnimationCurve.Linear(0f, 0f, X_AXIS_LEN, 360f);

		public AnimationCurve pitchAngle = AnimationCurve.Linear(0f, 0f, X_AXIS_LEN, 80f);

		public AnimationCurve distance = AnimationCurve.Linear(0f, 5f, X_AXIS_LEN, 5f);

		public AnimationCurve fov = AnimationCurve.Linear(0f, 50f, X_AXIS_LEN, 50f);

		private Camera _camera;

		private PostFX _postFX;

		private Transform _avatarTransform;

		private BehaviorTree _behaviorTree;

		public bool slowMode;

		public bool showEffect = true;

		public bool isPlaying;

		[SerializeField]
		private float _timer;

		private void Awake()
		{
			_camera = GetComponent<Camera>();
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (isPlaying)
			{
				Play(Time.deltaTime);
				if (Input.GetKeyDown(KeyCode.F))
				{
					isPlaying = false;
					EndPlay();
				}
			}
			else if (Input.GetKeyDown(KeyCode.F))
			{
				InitPlay();
			}
		}

		private void Play(float time)
		{
			_timer += time;
			float num = _timer / duration * X_AXIS_LEN;
			if (_timer > attackDelay && _behaviorTree != null)
			{
				_behaviorTree.SetVariableValue("DoAttack", true);
			}
			base.transform.forward = _avatarTransform.forward;
			Vector3 eulerAngles = base.transform.eulerAngles;
			eulerAngles.x += pitchAngle.Evaluate(num);
			eulerAngles.y += yawAngle.Evaluate(num);
			base.transform.eulerAngles = eulerAngles;
			base.transform.position = _avatarTransform.position + Vector3.up * lookAtHeight - base.transform.forward * distance.Evaluate(num);
			_camera.fieldOfView = fov.Evaluate(num);
			if (num > X_AXIS_LEN)
			{
				EndPlay();
			}
		}

		private void InitPlay()
		{
			BaseMonoAvatar baseMonoAvatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
			_avatarTransform = baseMonoAvatar.transform;
			_behaviorTree = baseMonoAvatar.GetComponent<BehaviorTree>();
			if (slowMode)
			{
				Time.timeScale = 0.25f;
			}
			if (!showEffect)
			{
				_postFX = _camera.GetComponent<PostFX>();
				_postFX.UseDistortion = false;
			}
			_timer = 0f;
			isPlaying = true;
		}

		private void EndPlay()
		{
			if (_behaviorTree != null)
			{
				_behaviorTree.SetVariableValue("DoAttack", false);
			}
			if (slowMode)
			{
				Time.timeScale = 1f;
			}
			if (!showEffect)
			{
				_postFX.UseDistortion = true;
			}
			isPlaying = false;
		}
	}
}
