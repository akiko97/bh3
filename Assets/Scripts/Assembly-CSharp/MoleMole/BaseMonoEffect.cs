using System;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseMonoEffect : BaseMonoEntity
	{
		protected Transform _effectTrans;

		public Vector3 OffsetVec3;

		public float RotateX;

		[NonSerialized]
		public bool isFromEffectPool;

		public string EffectTypeName { get; private set; }

		public uint EffectTypeID { get; private set; }

		public override Vector3 XZPosition
		{
			get
			{
				return new Vector3(_effectTrans.position.x, 0f, _effectTrans.position.z);
			}
		}

		public Vector3 FaceDirection
		{
			get
			{
				return _effectTrans.forward;
			}
			protected set
			{
				value.Normalize();
				_effectTrans.forward = value;
			}
		}

		protected virtual void Awake()
		{
		}

		public virtual void Init(string effectPath, uint runtimeID, Vector3 initPos, Vector3 faceDir, Vector3 initScale, bool isFromEffectPool)
		{
			EffectTypeName = effectPath;
			_runtimeID = runtimeID;
			_effectTrans = base.gameObject.transform;
			float num = Vector3.Angle(faceDir, Vector3.forward);
			if (!((double)Vector3.Cross(faceDir, Vector3.forward).y < 0.0))
			{
				num = 0f - num;
			}
			initPos += Quaternion.AngleAxis(num, Vector3.up) * Vector3.Scale(OffsetVec3, initScale);
			FaceDirection = faceDir;
			_effectTrans.position = initPos;
			_effectTrans.localEulerAngles = new Vector3(RotateX, _effectTrans.localEulerAngles.y, _effectTrans.localEulerAngles.z);
			_effectTrans.localScale = initScale;
			this.isFromEffectPool = isFromEffectPool;
		}

		public virtual void Setup()
		{
		}

		public virtual void Start()
		{
		}

		public virtual void Update()
		{
		}

		public override Transform GetAttachPoint(string name)
		{
			return base.transform;
		}

		protected override void OnDestroy()
		{
		}
	}
}
