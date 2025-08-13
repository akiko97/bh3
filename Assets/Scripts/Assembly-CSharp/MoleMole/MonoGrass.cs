using System;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Renderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class MonoGrass : MonoBehaviour
	{
		private enum AvatarState
		{
			Walk = 0,
			Float = 1,
			Landing = 2
		}

		public float avatarWalkImpulseStength = 0.3f;

		public float avatarJumpImpulseStength = 1f;

		public float avatarJumpImpulseDuration = 1f;

		public float avatarFloatHeight = 1f;

		[Range(0f, 1f)]
		public float avatarPositionSmooth = 0.1f;

		public AnimationCurve avatarJumpBlendCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		private Renderer _renderer;

		private MaterialPropertyBlock _mpb;

		private AvatarState _avatarState;

		private float _avatarLandingTimer;

		private Transform _avatarAnchor;

		private Vector3 _avatarPosition;

		private float _avatarHeight;

		private BaseMonoAvatar __avatar;

		private BaseMonoAvatar _avatar
		{
			get
			{
				if (__avatar == null)
				{
					__avatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
					GetAvatarAnchor();
				}
				return __avatar;
			}
			set
			{
				__avatar = value;
				GetAvatarAnchor();
			}
		}

		private void Start()
		{
			_renderer = GetComponent<Renderer>();
			_mpb = new MaterialPropertyBlock();
			_renderer.GetPropertyBlock(_mpb);
			AvatarManager instance = Singleton<AvatarManager>.Instance;
			instance.onLocalAvatarChanged = (Action<BaseMonoAvatar, BaseMonoAvatar>)Delegate.Combine(instance.onLocalAvatarChanged, new Action<BaseMonoAvatar, BaseMonoAvatar>(OnLocalAvatarChanged));
		}

		private void GetAvatarAnchor()
		{
			if (__avatar != null)
			{
				_avatarPosition = __avatar.XZPosition;
				_avatarAnchor = __avatar.GetAttachPoint("LeftFoot");
				if (_avatarAnchor == null)
				{
					_avatarAnchor = __avatar.transform;
				}
				_avatarHeight = _avatarAnchor.position.y;
			}
		}

		private void Update()
		{
			SetAvatarImpulse("_RadialWind1");
		}

		private void SetAvatarImpulse(string propName)
		{
			if (_avatar == null)
			{
				return;
			}
			_avatarPosition = Vector3.Lerp(_avatar.XZPosition, _avatarPosition, avatarPositionSmooth);
			_avatarHeight = Mathf.Lerp(_avatarAnchor.position.y, _avatarHeight, avatarPositionSmooth);
			Vector4 value;
			if (_avatarState == AvatarState.Walk)
			{
				Vector3 avatarPosition = _avatarPosition;
				value = new Vector4(avatarPosition.x, 0f, avatarPosition.z, avatarWalkImpulseStength);
				if (_avatarAnchor.position.y > avatarFloatHeight)
				{
					_avatarState = AvatarState.Float;
				}
			}
			else if (_avatarState == AvatarState.Landing)
			{
				_avatarLandingTimer += Time.deltaTime;
				float num = _avatarLandingTimer / avatarJumpImpulseDuration;
				if (num > 1f)
				{
					_avatarState = AvatarState.Walk;
				}
				num = Mathf.Clamp01(num);
				num = avatarJumpBlendCurve.Evaluate(num);
				float a = Mathf.Lerp(avatarWalkImpulseStength, avatarJumpImpulseStength, num);
				a = Mathf.Lerp(a, 0f, Mathf.Clamp01(_avatarHeight / avatarFloatHeight));
				Vector3 avatarPosition2 = _avatarPosition;
				value = new Vector4(avatarPosition2.x, 0f, avatarPosition2.z, a);
			}
			else
			{
				Vector3 avatarPosition3 = _avatarPosition;
				value = new Vector4(avatarPosition3.x, 0f, avatarPosition3.z, avatarWalkImpulseStength);
				if (_avatarAnchor.position.y < avatarFloatHeight)
				{
					_avatarState = AvatarState.Landing;
					_avatarLandingTimer = 0f;
				}
			}
			_mpb.SetVector(propName, value);
			_renderer.SetPropertyBlock(_mpb);
		}

		private void OnLocalAvatarChanged(BaseMonoAvatar from, BaseMonoAvatar to)
		{
			_avatar = to;
		}
	}
}
