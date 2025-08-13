using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(MonoEffect))]
	public class MonoEffectPluginMoveToTarget : BaseMonoEffectPlugin
	{
		[Header("Velocity")]
		public float Velocity = 20f;

		[Header("Emit By Distance Particle System MUST be put into a child and set here")]
		public GameObject ActivateOnStart;

		[Header("Destroy immediately upon arrival")]
		public bool DestroyImmediatelyUponArrival;

		[Header("To Attach Point")]
		public string ToAttachPoint = "RootNode";

		private Transform _targetTransform;

		private bool _active;

		public void SetMoveToTarget(BaseMonoEntity toEntity)
		{
			_targetTransform = toEntity.GetAttachPoint(ToAttachPoint);
		}

		protected override void Awake()
		{
			base.Awake();
			if (ActivateOnStart != null)
			{
				ActivateOnStart.SetActive(false);
			}
		}

		public override void Setup()
		{
			if (ActivateOnStart != null)
			{
				ActivateOnStart.SetActive(true);
			}
			_active = true;
		}

		private void Update()
		{
			if (!_active)
			{
				return;
			}
			if (_targetTransform == null)
			{
				_effect.SetDestroy();
				_active = false;
				return;
			}
			Vector3 vector = _targetTransform.position - base.transform.position;
			float magnitude = vector.magnitude;
			float num = Velocity * Time.deltaTime * _effect.TimeScale;
			vector.Normalize();
			base.transform.forward = vector;
			if (num > magnitude)
			{
				base.transform.position = _targetTransform.position;
				if (DestroyImmediatelyUponArrival)
				{
					_effect.SetDestroyImmediately();
				}
				else
				{
					_effect.SetDestroy();
				}
				_active = false;
			}
			else
			{
				base.transform.position += vector * num;
			}
		}

		public override bool IsToBeRemove()
		{
			return false;
		}

		public override void SetDestroy()
		{
		}

		private void OnDisable()
		{
			if (ActivateOnStart != null)
			{
				ActivateOnStart.SetActive(false);
			}
		}
	}
}
