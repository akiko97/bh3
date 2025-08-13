using UnityEngine;

namespace MoleMole
{
	public abstract class BaseMonoCamera : BaseMonoEntity
	{
		protected Transform _cameraTrans;

		public override float TimeScale
		{
			get
			{
				return 1f;
			}
		}

		public uint CameraType { get; private set; }

		public Vector3 Forward
		{
			get
			{
				return _cameraTrans.forward;
			}
			private set
			{
			}
		}

		public override Vector3 XZPosition
		{
			get
			{
				return new Vector3(_cameraTrans.position.x, 0f, _cameraTrans.position.z);
			}
		}

		public virtual void Awake()
		{
		}

		protected void Init(uint cameraType, uint runtimeID)
		{
			CameraType = cameraType;
			_runtimeID = runtimeID;
			_cameraTrans = base.gameObject.transform;
		}

		public virtual void Start()
		{
		}

		public virtual void Update()
		{
		}

		public override bool IsToBeRemove()
		{
			return false;
		}

		public override bool IsActive()
		{
			return true;
		}

		public override Transform GetAttachPoint(string name)
		{
			return base.transform;
		}
	}
}
