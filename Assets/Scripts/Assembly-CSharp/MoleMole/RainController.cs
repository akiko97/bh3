using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class RainController : MonoBehaviour
	{
		private enum SlowModeState
		{
			IN = 0,
			OUT = 1,
			ENTERING = 2,
			LEAVING = 3
		}

		[Range(0f, 1f)]
		public float timeScale;

		public AnimationCurve timeScaleRemapCurve;

		[Range(0f, 1f)]
		public float slowModeTimeScale;

		public float slowModeEnterDuration = 0.6f;

		public float slowModeLeaveDuration = 0.6f;

		public Rain rain;

		public GameObject splash;

		private ParticleSystem __splashParticleSystem;

		private float __lastSystemTimeScale;

		private static readonly float MAX_ACCEPTED_SYSTEM_TIMESCALE_DELTA = 2f;

		private SlowModeState _slowModeState = SlowModeState.OUT;

		private float _transTimer;

		private float _startTimeScale;

		private float _currentTimeScale;

		private ParticleSystem _splashParticleSystem
		{
			get
			{
				if (__splashParticleSystem == null)
				{
					__splashParticleSystem = splash.GetComponent<ParticleSystem>();
				}
				return __splashParticleSystem;
			}
		}

		private float _systemTimeScale
		{
			get
			{
				__lastSystemTimeScale = ((!(Mathf.Abs(Time.timeScale - __lastSystemTimeScale) < MAX_ACCEPTED_SYSTEM_TIMESCALE_DELTA)) ? __lastSystemTimeScale : Time.timeScale);
				return __lastSystemTimeScale;
			}
		}

		private void Update()
		{
			SetupTimeScale();
			MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
			Vector3 followCenterXZPosition = mainCamera.followState.followCenterXZPosition;
			followCenterXZPosition.y = 0f;
			base.transform.position = followCenterXZPosition;
		}

		public void Init()
		{
			__lastSystemTimeScale = Time.timeScale;
			rain.Init();
		}

		public void EnterSlowMode(float startTimeScale)
		{
			if (_slowModeState == SlowModeState.OUT)
			{
				_slowModeState = SlowModeState.ENTERING;
				_transTimer = 0f;
				_startTimeScale = startTimeScale;
			}
		}

		public void LeaveSlowMode()
		{
			if (_slowModeState == SlowModeState.IN)
			{
				_slowModeState = SlowModeState.LEAVING;
				_transTimer = 0f;
			}
		}

		public void SetRain(ConfigRain config)
		{
			rain.SetUp(config);
			ParticleSystem.EmissionModule emission = _splashParticleSystem.emission;
			emission.rate = new ParticleSystem.MinMaxCurve(config.splashDensity * rain.area);
			Material material = _splashParticleSystem.GetComponent<Renderer>().material;
			Color color = material.GetColor("_TintColor");
			material.SetColor("_TintColor", new Color(color.r, color.g, color.b, config.splashOpaqueness * 0.5f));
		}

		private void SetupTimeScale()
		{
			UpdateSlowMode();
			float num = timeScaleRemapCurve.Evaluate(_currentTimeScale * _systemTimeScale);
			num /= Mathf.Max(0.001f, Time.timeScale);
			rain.timeScale = num;
			_splashParticleSystem.playbackSpeed = num;
		}

		private void UpdateSlowMode()
		{
			if (_slowModeState == SlowModeState.IN)
			{
				_currentTimeScale = slowModeTimeScale;
			}
			else if (_slowModeState == SlowModeState.OUT)
			{
				_currentTimeScale = timeScale;
			}
			else if (_slowModeState == SlowModeState.ENTERING)
			{
				_transTimer += Time.unscaledDeltaTime;
				_currentTimeScale = Mathf.Lerp(_startTimeScale, slowModeTimeScale, Mathf.Clamp01(_transTimer / slowModeEnterDuration));
				if (_transTimer > slowModeEnterDuration)
				{
					_slowModeState = SlowModeState.IN;
				}
			}
			else if (_slowModeState == SlowModeState.LEAVING)
			{
				_transTimer += Time.unscaledDeltaTime;
				_currentTimeScale = Mathf.Lerp(slowModeTimeScale, timeScale, Mathf.Clamp01(_transTimer / slowModeLeaveDuration));
				if (_transTimer > slowModeLeaveDuration)
				{
					_slowModeState = SlowModeState.OUT;
				}
			}
			if (_slowModeState != SlowModeState.OUT)
			{
				_currentTimeScale /= _systemTimeScale;
			}
		}
	}
}
