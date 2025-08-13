using System;
using System.Collections;
using UnityEngine;

namespace MoleMole.MainMenu
{
	public class MainMenuStage : MonoBehaviour
	{
		[HideInInspector]
		public bool IsUpdateAtmosphereAuto = true;

		[HideInInspector]
		public bool ForceUpdateAtmosphere = true;

		[HideInInspector]
		public bool UpdateAtmosphereWithTransition = true;

		public GameObject BackgroundQuad;

		public float BackgroundDist = 6000f;

		public float BackgroundExtendAngle = 4f;

		private float _virtualDayTime = -1f;

		private Renderer _skyRenderer;

		private CloudEmitter _cloudEmitter;

		private MaterialPropertyBlock _skyMPB;

		private MaterialPropertyBlock _bgMPB;

		private RenderTextureWrapper _bgRenderTexture;

		private Camera _bgCamera;

		private static readonly string _bgLayerName = "Water";

		private static readonly float _bgFarClip = 10000f;

		private ConfigAtmosphere _atmosphereConfig;

		private int _atmosphereConfigSeriesId;

		private ConfigAtmosphereSeries _atmosphereConfigSeries;

		[HideInInspector]
		public string AtmosphereConfigSeriesPath;

		[HideInInspector]
		public bool IsInTransition;

		private int _currentKey;

		private int _nextKey;

		private float _transitionTime;

		private float _remainTransitionTime;

		private bool _needUpdateAtmosphere = true;

		private bool _isSomethingWrong;

		public float DayTime
		{
			get
			{
				if (_virtualDayTime > -0.9f)
				{
					return _virtualDayTime;
				}
				DateTime now = TimeUtil.Now;
				return (float)now.Hour + (float)now.Minute / 60f + (float)now.Second / 3600f + (float)now.Millisecond / 3600000f;
			}
			set
			{
				_virtualDayTime = value;
			}
		}

		public ConfigAtmosphere AtmosphereConfig
		{
			get
			{
				return _atmosphereConfig;
			}
		}

		public ConfigAtmosphereSeries AtmosphereConfigSeries
		{
			get
			{
				return _atmosphereConfigSeries;
			}
		}

		public string CloudSceneName
		{
			get
			{
				return _atmosphereConfigSeries.Common.ScneneName;
			}
		}

		private void Start()
		{
			Init();
		}

		private void OnEnable()
		{
			Init();
		}

		private void OnDestroy()
		{
			ReleaseBackgroundRenderTexture();
		}

		private void Init()
		{
			try
			{
				GameObject gameObject = base.transform.Find("Sky").gameObject;
				_skyRenderer = gameObject.GetComponent<Renderer>();
				_skyMPB = new MaterialPropertyBlock();
				_cloudEmitter = GetComponentInChildren<CloudEmitter>(true);
				if (GlobalDataManager.metaConfig == null)
				{
					GlobalDataManager.Refresh();
				}
				IsUpdateAtmosphereAuto = true;
				UpdateAtmosphere();
				_isSomethingWrong = false;
			}
			catch
			{
				_isSomethingWrong = true;
				throw;
			}
		}

		private IEnumerator TransitAtmosphere()
		{
			while (IsInTransition)
			{
				_needUpdateAtmosphere = true;
				float t = Time.deltaTime / _remainTransitionTime;
				_remainTransitionTime -= Time.deltaTime;
				_remainTransitionTime = Mathf.Max(1E-06f, _remainTransitionTime);
				if (t < 1f)
				{
					_atmosphereConfig = ConfigAtmosphere.Lerp(_atmosphereConfig, AtmosphereConfigSeries.Value(_nextKey), t);
					yield return null;
					continue;
				}
				break;
			}
			if (IsInTransition)
			{
				_currentKey = _nextKey;
				_atmosphereConfig = AtmosphereConfigSeries.Value(_currentKey);
				IsInTransition = false;
			}
			yield return null;
		}

		private void EvaluateAtmosphere()
		{
			if (UpdateAtmosphereWithTransition)
			{
				int num = AtmosphereConfigSeries.KeyBeforeTime(DayTime);
				if (num != _currentKey)
				{
					if (!IsInTransition)
					{
						_nextKey = num;
						IsInTransition = true;
						_remainTransitionTime = AtmosphereConfigSeries.Common.TransitionTime;
						StartCoroutine(TransitAtmosphere());
					}
					else if (num != _nextKey)
					{
						_nextKey = num;
						_remainTransitionTime = AtmosphereConfigSeries.Common.TransitionTime;
					}
				}
			}
			else if (!IsInTransition)
			{
				_needUpdateAtmosphere = true;
				_currentKey = _atmosphereConfigSeries.KeyBeforeTime(DayTime);
				_atmosphereConfig = _atmosphereConfigSeries.Evaluate(DayTime, true);
			}
		}

		public void Update()
		{
			if (_isSomethingWrong)
			{
				return;
			}
			try
			{
				if (!GlobalVars.STATIC_CLOUD_MODE)
				{
					UpdateAtmosphere();
					if (_bgRenderTexture != null)
					{
						GraphicsUtils.ReleaseRenderTexture(_bgRenderTexture);
						_bgRenderTexture = null;
					}
					if (_skyRenderer != null)
					{
						_skyRenderer.gameObject.SetActive(true);
					}
					if (_cloudEmitter != null)
					{
						_cloudEmitter.gameObject.SetActive(true);
					}
					if (BackgroundQuad != null)
					{
						BackgroundQuad.SetActive(false);
					}
				}
				else if (_bgRenderTexture == null && _atmosphereConfigSeries != null)
				{
					DrawBackgroundToRenderTexture();
					Renderer component = BackgroundQuad.GetComponent<Renderer>();
					if (_bgMPB == null)
					{
						_bgMPB = new MaterialPropertyBlock();
						if (component != null)
						{
							component.GetPropertyBlock(_bgMPB);
						}
					}
					_bgMPB.SetTexture("_MainTex", (RenderTexture)_bgRenderTexture);
					if (component != null)
					{
						component.SetPropertyBlock(_bgMPB);
					}
					if (_skyRenderer != null)
					{
						_skyRenderer.gameObject.SetActive(false);
					}
					if (_cloudEmitter != null)
					{
						_cloudEmitter.gameObject.SetActive(false);
					}
					if (BackgroundQuad != null)
					{
						BackgroundQuad.SetActive(true);
					}
					SetBackgroundQuad();
				}
				_isSomethingWrong = false;
			}
			catch
			{
				_isSomethingWrong = true;
				throw;
			}
		}

		private void UpdateAtmosphere()
		{
			if (!(AtmosphereConfigSeries == null))
			{
				EvaluateAtmosphere();
				if (ForceUpdateAtmosphere || (_needUpdateAtmosphere && IsUpdateAtmosphereAuto))
				{
					SetupAtmosphere(AtmosphereConfigSeries.Common, _atmosphereConfig);
				}
				ForceUpdateAtmosphere = false;
				_needUpdateAtmosphere = false;
			}
		}

		public void SetupAtmosphere(ConfigAtmosphereCommon commonConfig, ConfigAtmosphere config)
		{
			if (_skyMPB == null)
			{
				Init();
			}
			SetupSky(commonConfig, config.Background);
			_cloudEmitter.SetupCloudConfig(commonConfig, config.CloudStyle);
			SetupIndoor(config.Indoor);
		}

		public void ChooseAtmosphereSeriesDefault()
		{
			string text = "Rendering/MainMenuAtmosphereConfig/Default";
			bool flag = Singleton<PlayerModule>.Instance.playerData.userId != 0;
			string path = ((!flag) ? text : Singleton<MiHoYoGameData>.Instance.LocalData.CurrentWeatherConfigPath);
			ConfigAtmosphereSeries config = ConfigAtmosphereSeries.LoadFromFileAndDetach(path);
			int sceneId = (flag ? Singleton<MiHoYoGameData>.Instance.LocalData.CurrentWeatherSceneID : 0);
			ChooseCloudScene(config, sceneId);
		}

		public void ChooseAtmosphereSeriesRandomly()
		{
			string pathRandomly = AtmosphereSeriesData.GetPathRandomly();
			ConfigAtmosphereSeries configAtmosphereSeries = ConfigAtmosphereSeries.LoadFromFileAndDetach(pathRandomly);
			int sceneIdRandomly = configAtmosphereSeries.GetSceneIdRandomly();
			ChooseCloudScene(configAtmosphereSeries, sceneIdRandomly);
		}

		public void ChooseAtmosphereSeriesNext()
		{
			int nextId = AtmosphereSeriesData.GetNextId(_atmosphereConfigSeriesId);
			string path = AtmosphereSeriesData.GetPath(nextId);
			ConfigAtmosphereSeries configAtmosphereSeries = ConfigAtmosphereSeries.LoadFromFileAndDetach(path);
			int sceneIdRandomly = configAtmosphereSeries.GetSceneIdRandomly();
			ChooseCloudScene(configAtmosphereSeries, sceneIdRandomly);
		}

		public void ChooseCloudSceneNext()
		{
			_atmosphereConfigSeries.Common.UpdateSceneNameNext();
			_needUpdateAtmosphere = true;
			ReleaseBackgroundRenderTexture();
			_cloudEmitter.gameObject.SetActive(true);
			UpdateAtmosphere();
		}

		public void ChooseCloudScene(ConfigAtmosphereSeries config, int sceneId)
		{
			if (!(config == null) && config.IsValid())
			{
				_atmosphereConfigSeries = config;
				AtmosphereConfigSeriesPath = config.Path;
				int num = AtmosphereConfigSeriesPath.LastIndexOf('/');
				string text = AtmosphereConfigSeriesPath.Substring(num + 1);
				_atmosphereConfigSeriesId = AtmosphereSeriesData.GetId(AtmosphereConfigSeriesPath);
				_atmosphereConfigSeries.SetSceneId(sceneId);
				_currentKey = AtmosphereConfigSeries.KeyBeforeTime(DayTime);
				_nextKey = _currentKey;
				_atmosphereConfig = AtmosphereConfigSeries.Value(_currentKey);
				_needUpdateAtmosphere = true;
				IsInTransition = false;
				ReleaseBackgroundRenderTexture();
				_cloudEmitter.gameObject.SetActive(true);
				UpdateAtmosphere();
			}
		}

		private void SetupSky(ConfigAtmosphereCommon commonConfig, ConfigBackground config)
		{
			if (commonConfig.Tex != null)
			{
				_skyMPB.SetTexture("_MainTex", commonConfig.Tex);
				_skyMPB.SetColor("_TexRColor", config.RColor);
				_skyMPB.SetColor("_TexGColor", config.GColor);
				_skyMPB.SetColor("_TexBColor", config.BColor);
				_skyMPB.SetFloat("_TexXLocation", config.XLocation);
				_skyMPB.SetFloat("_TexYLocation", config.YLocation);
				_skyMPB.SetFloat("_TexHigh", config.High);
			}
			if (commonConfig.SecondTex != null)
			{
				_skyMPB.SetTexture("_SecTexture", commonConfig.SecondTex);
				_skyMPB.SetFloat("_SecTexXLocation", config.SecTexXLocation);
				_skyMPB.SetFloat("_SecTexYLocation", config.SecTexYLocation);
				_skyMPB.SetFloat("_SecTexHigh", config.SecTexHigh);
				_skyMPB.SetFloat("_SecTexEmission", config.SecTexEmission);
			}
			_skyMPB.SetColor("_GradBottomColor", config.GradBottomColor);
			_skyMPB.SetColor("_GradTopColor", config.GradTopColor);
			_skyMPB.SetFloat("_GradLocation", config.GradLocation);
			_skyMPB.SetFloat("_GradHigh", config.GradHigh);
			_skyMPB.SetFloat("_BloomFactor", config.BloomFactor);
			_skyRenderer.SetPropertyBlock(_skyMPB);
		}

		private void SetupIndoor(ConfigIndoor indoor)
		{
			Shader.SetGlobalColor("_miHoYo_Indoor_Tint_Color", indoor.TintColor);
		}

		private void ReleaseBackgroundRenderTexture()
		{
			if (_bgRenderTexture != null)
			{
				GraphicsUtils.ReleaseRenderTexture(_bgRenderTexture);
				_bgRenderTexture = null;
			}
		}

		private bool DrawBackgroundToRenderTexture()
		{
			Camera main = Camera.main;
			if (main == null)
			{
				return false;
			}
			int layer = LayerMask.NameToLayer(_bgLayerName);
			GameObject gameObject = _skyRenderer.gameObject;
			int layer2 = gameObject.layer;
			gameObject.layer = layer;
			GameObject gameObject2 = _cloudEmitter.gameObject;
			int layer3 = gameObject2.layer;
			gameObject2.layer = layer;
			gameObject.SetActive(true);
			gameObject2.SetActive(true);
			ReleaseBackgroundRenderTexture();
			_bgRenderTexture = GraphicsUtils.GetRenderTexture(main.pixelWidth, main.pixelHeight, 16, RenderTextureFormat.ARGBHalf);
			_bgRenderTexture.onRebindToCameraCallBack = ReleaseBackgroundRenderTexture;
			if (_bgCamera == null)
			{
				GameObject gameObject3 = new GameObject("Background Camera", typeof(Camera));
				gameObject3.hideFlags = HideFlags.HideAndDontSave;
				_bgCamera = gameObject3.GetComponent<Camera>();
				_bgCamera.enabled = false;
				_bgCamera.CopyFrom(main);
				_bgCamera.farClipPlane = _bgFarClip;
				_bgCamera.cullingMask = LayerMask.GetMask(_bgLayerName);
				_bgCamera.fieldOfView += BackgroundExtendAngle * 2f;
				_bgCamera.transform.position = main.transform.position;
				_bgCamera.transform.rotation = main.transform.rotation;
			}
			_bgCamera.targetTexture = _bgRenderTexture;
			_bgRenderTexture.BindToCamera(_bgCamera);
			_bgCamera.Render();
			_bgRenderTexture.UnbindFromCamera(_bgCamera);
			gameObject.layer = layer2;
			gameObject2.layer = layer3;
			return true;
		}

		private void SetBackgroundQuad()
		{
			if (_bgCamera != null)
			{
				float num = _bgCamera.fieldOfView * ((float)Math.PI / 180f);
				float num2 = BackgroundDist * Mathf.Tan(num * 0.5f) * 2f;
				float x = num2 * _bgCamera.aspect;
				Transform transform = BackgroundQuad.transform;
				transform.localScale = new Vector2(x, num2);
				transform.eulerAngles = _bgCamera.transform.eulerAngles;
				transform.position = transform.forward * BackgroundDist;
			}
		}
	}
}
