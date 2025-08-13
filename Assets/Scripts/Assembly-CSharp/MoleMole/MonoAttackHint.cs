using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MonoAttackHint : MonoAuxObject
	{
		private enum OuterState
		{
			Glow = 0,
			Idle = 1
		}

		private enum InnerState
		{
			Hide = 0,
			Inflate = 1,
			FadeOut = 2
		}

		private const int MAX_FLAG_COUNT = 6;

		private const float OUTER_FADE_IN_DURATION = 0.2f;

		public Transform outerHint;

		public Transform innerHint;

		public Transform flag;

		public float flagBorderOffset = 0.1f;

		public float flagGap = 0.8f;

		public float fadeOutTime = 0.3f;

		public AnimationCurve inflateCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		public AnimationCurve outerFadeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		public AnimationCurve fadeOutCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		private BaseMonoAnimatorEntity _owner;

		private ConfigMonsterAttackHint _config;

		private float _timer;

		private float _innerMinScale;

		private float _innerMaxScale;

		private Material _sharedFlagMaterial;

		private MaterialPropertyBlockFader _mpb_flagFader;

		private MaterialPropertyBlockFader _mpb_outerFader;

		private MaterialPropertyBlockFader _mpb_innerFader;

		private OuterState _outerState;

		private InnerState _innerState;

		private void Awake()
		{
			_sharedFlagMaterial = flag.GetComponent<Renderer>().sharedMaterial;
			flag.gameObject.SetActive(false);
			innerHint.SetLocalPositionY(0.0001f);
			_mpb_flagFader = new MaterialPropertyBlockFader(flag.GetComponent<Renderer>(), "_TintColor");
			_mpb_outerFader = new MaterialPropertyBlockFader(outerHint.GetComponent<Renderer>(), "_TintColor");
			_mpb_innerFader = new MaterialPropertyBlockFader(innerHint.GetComponent<Renderer>(), "_TintColor");
			outerHint.SetLocalPositionY(0.02f);
			innerHint.SetLocalPositionY(0.06f);
		}

		public void Init(BaseMonoAnimatorEntity owner, BaseMonoAnimatorEntity target, ConfigMonsterAttackHint config)
		{
			_timer = 0f;
			_owner = owner;
			_config = config;
			_outerState = OuterState.Glow;
			_innerState = InnerState.Hide;
			if (config is RectAttackHint)
			{
				RectAttackHint rectAttackHint = (RectAttackHint)config;
				if (config.OffsetBase == HintOffsetBase.Target && target != null)
				{
					base.transform.position = target.XZPosition + target.FaceDirection * (rectAttackHint.OffsetZ + 0.5f * rectAttackHint.Distance);
				}
				else if (config.OffsetBase == HintOffsetBase.Owner)
				{
					base.transform.position = owner.XZPosition + owner.FaceDirection * (rectAttackHint.OffsetZ + 0.5f * rectAttackHint.Distance);
				}
				base.transform.forward = owner.FaceDirection;
				base.transform.SetLocalScaleX(rectAttackHint.Width);
				base.transform.SetLocalScaleZ(rectAttackHint.Distance);
				_innerMinScale = 0f;
				_innerMaxScale = 1f;
			}
			else if (config is CircleAttackHint)
			{
				CircleAttackHint circleAttackHint = (CircleAttackHint)config;
				if (config.OffsetBase == HintOffsetBase.Target && target != null)
				{
					base.transform.position = target.XZPosition + target.FaceDirection * circleAttackHint.OffsetZ;
				}
				else if (config.OffsetBase == HintOffsetBase.Owner)
				{
					base.transform.position = owner.XZPosition + owner.FaceDirection * circleAttackHint.OffsetZ;
				}
				base.transform.forward = owner.FaceDirection;
				Transform obj = innerHint;
				Vector3 localScale = Vector3.one * circleAttackHint.Radius;
				outerHint.localScale = localScale;
				obj.localScale = localScale;
				_innerMinScale = 0f;
				_innerMaxScale = circleAttackHint.Radius;
				outerHint.GetComponent<MeshRenderer>().material.SetFloat("_FanAngle", (float)Math.PI * 2f);
				innerHint.GetComponent<MeshRenderer>().material.SetFloat("_FanAngle", (float)Math.PI * 2f);
				GameObject gameObject = new GameObject("Flags");
				gameObject.transform.SetParent(base.transform);
				Vector3 zero = Vector3.zero;
				zero.y = 0.04f;
				gameObject.transform.localPosition = zero;
				int num = Mathf.Min(Mathf.FloorToInt(circleAttackHint.Radius * 7f / flagGap), 6);
				float num2 = (float)Math.PI * 2f / (float)num * 57.29578f;
				for (int i = 0; i < num; i++)
				{
					Transform transform = UnityEngine.Object.Instantiate(this.flag);
					transform.SetParent(gameObject.transform, false);
					transform.gameObject.SetActive(true);
					transform.localRotation = Quaternion.AngleAxis(num2 * (float)i, Vector3.up);
					Vector3 localPosition = transform.localRotation * (Vector3.forward * (circleAttackHint.Radius - flagBorderOffset));
					transform.localPosition = localPosition;
					transform.GetComponent<Renderer>().sharedMaterial = _sharedFlagMaterial;
				}
			}
			else
			{
				if (!(config is SectorAttackHint))
				{
					return;
				}
				SectorAttackHint sectorAttackHint = (SectorAttackHint)config;
				if (config.OffsetBase == HintOffsetBase.Target && target != null)
				{
					base.transform.position = target.XZPosition + target.FaceDirection * sectorAttackHint.OffsetZ;
				}
				else if (config.OffsetBase == HintOffsetBase.Owner)
				{
					base.transform.position = owner.XZPosition + owner.FaceDirection * sectorAttackHint.OffsetZ;
				}
				base.transform.forward = owner.FaceDirection;
				Transform obj2 = innerHint;
				Vector3 localScale = Vector3.one * sectorAttackHint.Radius;
				outerHint.localScale = localScale;
				obj2.localScale = localScale;
				_innerMinScale = 0f;
				_innerMaxScale = sectorAttackHint.Radius;
				outerHint.GetComponent<MeshRenderer>().material.SetFloat("_FanAngle", (float)Math.PI / 180f * sectorAttackHint.Angle);
				innerHint.GetComponent<MeshRenderer>().material.SetFloat("_FanAngle", (float)Math.PI / 180f * sectorAttackHint.Angle);
				GameObject gameObject2 = new GameObject("Flags");
				gameObject2.transform.SetParent(base.transform);
				Vector3 zero2 = Vector3.zero;
				zero2.y = 0.04f;
				gameObject2.transform.localPosition = zero2;
				gameObject2.transform.forward = owner.FaceDirection;
				int num3 = Mathf.Min(Mathf.FloorToInt(sectorAttackHint.Radius * 7f * ((float)Math.PI / 180f) * sectorAttackHint.Angle / (float)Math.PI / 2f / flagGap), 6);
				float num4 = (float)Math.PI / 180f * sectorAttackHint.Angle / (float)num3 * 57.29578f;
				bool flag = num3 % 2 == 0;
				for (int j = 0; j < num3; j++)
				{
					Transform transform2 = UnityEngine.Object.Instantiate(this.flag);
					transform2.SetParent(gameObject2.transform, false);
					transform2.gameObject.SetActive(true);
					if (flag)
					{
						transform2.localRotation = ((j % 2 != 0) ? Quaternion.AngleAxis((0f - num4) * ((float)(j / 2) + 0.5f), Vector3.up) : Quaternion.AngleAxis(num4 * ((float)(j / 2) + 0.5f), Vector3.up));
					}
					else if (j == 0)
					{
						transform2.localRotation = Quaternion.AngleAxis(0f, Vector3.up);
					}
					else
					{
						transform2.localRotation = ((j % 2 != 0) ? Quaternion.AngleAxis((0f - num4) * (float)j, Vector3.up) : Quaternion.AngleAxis(num4 * (float)j, Vector3.up));
					}
					Vector3 localPosition2 = transform2.localRotation * (Vector3.forward * (sectorAttackHint.Radius - flagBorderOffset));
					transform2.localPosition = localPosition2;
					transform2.GetComponent<Renderer>().sharedMaterial = _sharedFlagMaterial;
				}
			}
		}

		private void Start()
		{
			innerHint.localScale = Vector3.zero;
			_mpb_flagFader.LerpAlpha(0f);
			_mpb_outerFader.LerpAlpha(0f);
		}

		private void Update()
		{
			if (_outerState == OuterState.Glow)
			{
				float t = _timer / 0.2f;
				_mpb_flagFader.LerpAlpha(t);
				_mpb_outerFader.LerpAlpha(t);
				if (_timer > 0.2f)
				{
					_outerState = OuterState.Idle;
				}
			}
			else if (_outerState != OuterState.Idle)
			{
			}
			if (_innerState == InnerState.Hide)
			{
				if (_timer > _config.InnerStartDelay)
				{
					_innerState = InnerState.Inflate;
				}
			}
			else if (_innerState == InnerState.Inflate)
			{
				float num = (_timer - _config.InnerStartDelay) / _config.InnerInflateDuration;
				if (num > 1f)
				{
					innerHint.localScale = Vector3.one * _innerMaxScale;
					_innerState = InnerState.FadeOut;
				}
				else
				{
					_mpb_flagFader.LerpAlpha(outerFadeCurve.Evaluate(num));
					_mpb_outerFader.LerpAlpha(outerFadeCurve.Evaluate(num));
					innerHint.localScale = Vector3.one * Mathf.Lerp(_innerMinScale, _innerMaxScale, inflateCurve.Evaluate(num));
				}
			}
			else if (_innerState == InnerState.FadeOut)
			{
				float num2 = (_timer - _config.InnerStartDelay - _config.InnerInflateDuration) / fadeOutTime;
				if (num2 > 1f)
				{
					SetDestroy();
				}
				else
				{
					_mpb_innerFader.LerpAlpha(fadeOutCurve.Evaluate(num2));
				}
			}
			_timer += Time.deltaTime * _owner.TimeScale;
		}
	}
}
