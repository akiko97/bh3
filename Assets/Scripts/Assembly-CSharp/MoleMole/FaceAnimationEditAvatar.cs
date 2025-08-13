using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class FaceAnimationEditAvatar : MonoBehaviour
	{
		private Animator _animator;

		private FaceAnimation _faceAnimation = new FaceAnimation();

		private FacePartControl _leftEye = new FacePartControl();

		private FacePartControl _rightEye = new FacePartControl();

		private FacePartControl _mouth = new FacePartControl();

		public int avatarId = 101;

		public int heartLevel = 1;

		public Renderer leftEyeRenderer;

		public Renderer rightEyeRenderer;

		public Renderer mouthRenderer;

		public TestMatInfoProvider leftEyeProvider;

		public TestMatInfoProvider rightEyeProvider;

		public TestMatInfoProvider mouthProvider;

		public AtlasMatInfoProvider eyeProviderAtlas;

		public AtlasMatInfoProvider mouthProviderAtlas;

		public bool useAtlas;

		private ConfigFaceAnimation _config;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			if (useAtlas)
			{
				_leftEye.Init(eyeProviderAtlas, leftEyeRenderer);
				_rightEye.Init(eyeProviderAtlas, rightEyeRenderer);
				_mouth.Init(mouthProviderAtlas, mouthRenderer);
			}
			else
			{
				_leftEye.Init(leftEyeProvider, leftEyeRenderer);
				_rightEye.Init(rightEyeProvider, rightEyeRenderer);
				_mouth.Init(mouthProvider, mouthRenderer);
			}
		}

		public void SetupFaceAnimation(ConfigFaceAnimation config)
		{
			_faceAnimation.Setup(config, _leftEye, _rightEye, _mouth);
			_config = config;
		}

		public void PlayBodyAnimation(string name)
		{
			if (_animator != null)
			{
				_animator.Play(name);
			}
		}

		public void PlayFaceAnimation(string name)
		{
			if (_faceAnimation != null)
			{
				_faceAnimation.PlayFaceAnimation(name);
			}
		}

		public void PrepareFaceAnimation(string name)
		{
			if (_faceAnimation != null)
			{
				_faceAnimation.PrepareFaceAnmation(name);
			}
		}

		public void SetLeftEyeImage(int index)
		{
			if (_leftEye != null)
			{
				_leftEye.SetFacePartIndex(index);
			}
		}

		public void SetRightEyeImage(int index)
		{
			if (_rightEye != null)
			{
				_rightEye.SetFacePartIndex(index);
			}
		}

		public void SetMouthImage(int index)
		{
			if (_mouth != null)
			{
				_mouth.SetFacePartIndex(index);
			}
		}

		public void SetAnimationTime(float time)
		{
			if (_faceAnimation != null)
			{
				_faceAnimation.SetTime(time);
			}
		}

		public void SetAnimationTimePerFrame(float time)
		{
			if (_faceAnimation != null)
			{
				_faceAnimation.SetTimePerFrame(time);
			}
		}

		public void RebuildFaceAnimation()
		{
			_faceAnimation.Setup(_config, _leftEye, _rightEye, _mouth);
		}

		public void TriggerAudioPattern(string name)
		{
			Singleton<WwiseAudioManager>.Instance.Post(name);
		}
	}
}
