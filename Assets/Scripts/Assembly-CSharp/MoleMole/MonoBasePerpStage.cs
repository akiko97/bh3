using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MonoBasePerpStage : MonoBehaviour
	{
		private class RenderingDataTransition
		{
			public ConfigStageRenderingData renderingData;

			public float transitDuration = 0.5f;

			public RenderingDataTransition(ConfigStageRenderingData data, float duration)
			{
				renderingData = data;
				transitDuration = duration;
			}

			public RenderingDataTransition(ConfigStageRenderingData data)
			{
				renderingData = data;
			}
		}

		private enum RenderingDataState
		{
			Idle = 0,
			RenderingTransit = 1
		}

		private class SubWeatherTransition
		{
			public ConfigSubWeatherCollection weather;

			public float transitDuration = 0.5f;

			public int renderingIndexInStack;

			public int stageEffectSettingIndexInStack = -1;

			public SubWeatherTransition(ConfigSubWeatherCollection data, int renderingIdx, int settingIx, float duration)
			{
				weather = data;
				transitDuration = duration;
				renderingIndexInStack = renderingIdx;
				stageEffectSettingIndexInStack = settingIx;
			}

			public SubWeatherTransition(ConfigSubWeatherCollection data, int renderingIdx, float duration)
			{
				weather = data;
				transitDuration = duration;
				renderingIndexInStack = renderingIdx;
			}

			public SubWeatherTransition(ConfigSubWeatherCollection data, int renderingIdx)
			{
				weather = data;
				renderingIndexInStack = renderingIdx;
			}
		}

		private enum WeatherState
		{
			Idle = 0,
			WeatherTransit = 1
		}

		public class ContinueWeatherDataSettings
		{
			public float renderingDataContinueTimer;

			public float weatherContinueTimer;

			public ConfigWeatherData currentWeatherData;

			public ConfigWeatherData continueWeatherData;

			public string continueWeatherName;
		}

		private const int BASE_RENDERING_DATANAME_IX = 0;

		private const int BASE_SUB_WEATHER_IX = 0;

		public Transform windZone;

		protected static string _stageDataPath = "Stage/Data/";

		private ConfigStageRenderingData _currentRendertingData;

		private FixedStack<RenderingDataTransition> _renderingDataStack;

		private RenderingDataState _renderingDataState;

		private int _middleRenderingIndex;

		private bool _isInMiddleRendering;

		private EntityTimer _renderingDataTransitionTimer;

		private ConfigStageRenderingData _initBaseRenderingData;

		private readonly string RAIN_PREFAB_PATH = "Effect/Weather/Rain/Rain";

		private GameObject _rainPrefab;

		private GameObject _rainObject;

		[NonSerialized]
		[HideInInspector]
		public RainController rainController;

		private ConfigSubWeatherCollection _currentSubWeather;

		private ConfigSubWeatherCollection _transitTargetSubWeather;

		private FixedStack<SubWeatherTransition> _subWeatherStack;

		private ConfigSubWeatherCollection _initBaseSubWeather;

		private string _currentBaseWeatherName;

		private WeatherState _weatherState;

		private int _middleSubWeatherIndex;

		private bool _isInMiddleWeather;

		private EntityTimer _weatherTransitionTimer;

		public MonoLightProbManager lightProbManager;

		public MonoLightShadowManager lightMapCorrectManager;

		private Transform _mainCameraTransform;

		public virtual void Awake()
		{
			_renderingDataStack = new FixedStack<RenderingDataTransition>(10, OnRenderingDataChanged);
			_currentRendertingData = null;
			_renderingDataTransitionTimer = new EntityTimer(0f);
			_renderingDataTransitionTimer.SetActive(false);
			_renderingDataState = RenderingDataState.Idle;
			_mainCameraTransform = Camera.main.transform;
			_subWeatherStack = new FixedStack<SubWeatherTransition>(5, OnSubWeatherChanged);
			_rainPrefab = Miscs.LoadResource<GameObject>(RAIN_PREFAB_PATH);
			_weatherState = WeatherState.Idle;
			_weatherTransitionTimer = new EntityTimer(0f);
			_weatherTransitionTimer.SetActive(false);
		}

		private ConfigStageRenderingData GetRenderingDataByName(string name)
		{
			return (!string.IsNullOrEmpty(name)) ? RenderingData.GetRenderingDataConfig<ConfigStageRenderingData>(name) : ConfigStageRenderingData.CreateStageRenderingDataFromMaterials(CollectSharedMaterials());
		}

		private ConfigWeatherData GetWeatherDataByName(string name)
		{
			ConfigWeatherData configWeatherData = ((!string.IsNullOrEmpty(name)) ? WeatherData.GetWeatherDataConfig(name) : new ConfigWeatherData());
			if (configWeatherData.configRenderingData == null)
			{
				if (_currentRendertingData != null)
				{
					configWeatherData.configRenderingData = _currentRendertingData.Clone();
				}
				else
				{
					configWeatherData.configRenderingData = ConfigStageRenderingData.CreateDefault();
				}
			}
			if (configWeatherData.configSubWeathers == null)
			{
				configWeatherData.configSubWeathers = new ConfigSubWeatherCollection();
			}
			return configWeatherData;
		}

		public virtual void Init(StageEntry entry, string weatherName)
		{
			_currentBaseWeatherName = weatherName;
			ConfigWeatherData weatherDataByName = GetWeatherDataByName(weatherName);
			_initBaseRenderingData = weatherDataByName.configRenderingData as ConfigStageRenderingData;
			_renderingDataStack.Push(0, new RenderingDataTransition(_initBaseRenderingData), true);
			_currentRendertingData = (ConfigStageRenderingData)_initBaseRenderingData.Clone();
			_currentRendertingData.ApplyGlobally();
			_initBaseSubWeather = weatherDataByName.configSubWeathers;
			_subWeatherStack.Push(0, new SubWeatherTransition(_initBaseSubWeather, 0), true);
			SetSubWeathersImmediately(_initBaseSubWeather);
			CommonInit(entry);
		}

		public virtual void Init(StageEntry entry, ConfigWeatherData weatherData)
		{
			_currentBaseWeatherName = null;
			_initBaseRenderingData = weatherData.configRenderingData as ConfigStageRenderingData;
			_renderingDataStack.Push(0, new RenderingDataTransition(_initBaseRenderingData), true);
			_currentRendertingData = (ConfigStageRenderingData)_initBaseRenderingData.Clone();
			_currentRendertingData.ApplyGlobally();
			_initBaseSubWeather = weatherData.configSubWeathers;
			_subWeatherStack.Push(0, new SubWeatherTransition(_initBaseSubWeather, 0), true);
			SetSubWeathersImmediately(_initBaseSubWeather);
			CommonInit(entry);
		}

		public virtual void Init(StageEntry entry, ConfigWeatherData fromWeatherData, string toWeatherName, float renderingTimer, float weatherTimer)
		{
			_currentBaseWeatherName = toWeatherName;
			ConfigWeatherData weatherDataByName = GetWeatherDataByName(toWeatherName);
			_currentRendertingData = (ConfigStageRenderingData)fromWeatherData.configRenderingData.Clone();
			_currentRendertingData.ApplyGlobally();
			_initBaseRenderingData = weatherDataByName.configRenderingData as ConfigStageRenderingData;
			_renderingDataStack.Push(0, new RenderingDataTransition(_initBaseRenderingData), true);
			if (renderingTimer > 0f)
			{
				TransitRenderingData(_initBaseRenderingData, renderingTimer);
			}
			else
			{
				SetRenderingDataImmediately(_initBaseRenderingData);
			}
			SetSubWeathersImmediately(fromWeatherData.configSubWeathers);
			_initBaseSubWeather = weatherDataByName.configSubWeathers;
			_subWeatherStack.Push(0, new SubWeatherTransition(_initBaseSubWeather, 0), true);
			if (weatherTimer > 0f)
			{
				TransitSubWeather(_initBaseSubWeather, weatherTimer);
			}
			else
			{
				SetSubWeathersImmediately(_initBaseSubWeather);
			}
			CommonInit(entry);
		}

		public virtual void Init(StageEntry entry, ConfigWeatherData fromWeatherData, ConfigWeatherData toWeatherData, float renderingTimer, float weatherTimer)
		{
			_currentBaseWeatherName = null;
			_currentRendertingData = (ConfigStageRenderingData)fromWeatherData.configRenderingData.Clone();
			_currentRendertingData.ApplyGlobally();
			_initBaseRenderingData = toWeatherData.configRenderingData as ConfigStageRenderingData;
			_renderingDataStack.Push(0, new RenderingDataTransition(_initBaseRenderingData), true);
			if (renderingTimer > 0f)
			{
				TransitRenderingData(_initBaseRenderingData, renderingTimer);
			}
			else
			{
				SetRenderingDataImmediately(_initBaseRenderingData);
			}
			SetSubWeathersImmediately(fromWeatherData.configSubWeathers);
			_initBaseSubWeather = toWeatherData.configSubWeathers;
			_subWeatherStack.Push(0, new SubWeatherTransition(_initBaseSubWeather, 0), true);
			if (weatherTimer > 0f)
			{
				TransitSubWeather(_initBaseSubWeather, weatherTimer);
			}
			else
			{
				SetSubWeathersImmediately(_initBaseSubWeather);
			}
			CommonInit(entry);
		}

		public virtual void Reset(StageEntry entry, ConfigWeatherData weatherData)
		{
			_initBaseRenderingData = weatherData.configRenderingData as ConfigStageRenderingData;
			_renderingDataStack.Set(0, new RenderingDataTransition(_initBaseRenderingData), true);
			SetRenderingDataImmediately(_initBaseRenderingData);
			_currentRendertingData.ApplyGlobally();
			_initBaseSubWeather = weatherData.configSubWeathers;
			_subWeatherStack.Set(0, new SubWeatherTransition(_initBaseSubWeather, 0), true);
			SetSubWeathersImmediately(_initBaseSubWeather);
			CommonInit(entry);
		}

		private void CommonInit(StageEntry stageEntry)
		{
			InitWindZone();
			InitLightProb();
			InitLightMapCorrection();
			Singleton<StageManager>.Instance.SetBaseStageEffectSetting(stageEntry.StageEffectSetting);
			Singleton<StageManager>.Instance.SetBaseStageEffectSetting(_subWeatherStack.value.weather.stageEffectSetting);
			FixedStack<float> auxTimeScaleStack = Singleton<LevelManager>.Instance.levelEntity.auxTimeScaleStack;
			auxTimeScaleStack.onChanged = (Action<float, int, float, int>)Delegate.Combine(auxTimeScaleStack.onChanged, new Action<float, int, float, int>(AuxTimeScaleCallback));
		}

		public virtual void Start()
		{
		}

		private void UpdateRenderingData()
		{
			if (_renderingDataState == RenderingDataState.Idle)
			{
				if (_isInMiddleRendering && _renderingDataStack.GetRealTopIndex() == _middleRenderingIndex)
				{
					PopRenderingData(_middleRenderingIndex);
					_isInMiddleRendering = false;
					_middleRenderingIndex = 0;
				}
			}
			else
			{
				if (_renderingDataState != RenderingDataState.RenderingTransit)
				{
					return;
				}
				_renderingDataTransitionTimer.Core(1f);
				_currentRendertingData.LerpStep(_renderingDataTransitionTimer.GetTimingRatio());
				_currentRendertingData.ApplyGlobally();
				if (_renderingDataTransitionTimer.isTimeUp)
				{
					if (_isInMiddleRendering && _renderingDataStack.GetRealTopIndex() == _middleRenderingIndex)
					{
						PopRenderingData(_middleRenderingIndex);
						_isInMiddleRendering = false;
						_middleRenderingIndex = 0;
					}
					else
					{
						_renderingDataTransitionTimer.Reset(false);
						_renderingDataState = RenderingDataState.Idle;
					}
				}
			}
		}

		private void UpdateSubWeather()
		{
			if (_weatherState == WeatherState.Idle && _isInMiddleWeather && _subWeatherStack.GetRealTopIndex() == _middleSubWeatherIndex)
			{
				PopWeather(_middleSubWeatherIndex);
				_isInMiddleWeather = false;
				_middleSubWeatherIndex = 0;
			}
			if (_weatherState != WeatherState.WeatherTransit)
			{
				return;
			}
			_weatherTransitionTimer.Core(1f);
			ConfigSubWeatherCollection configSubWeatherCollection = ConfigSubWeatherCollection.Lerp(_currentSubWeather, _transitTargetSubWeather, _weatherTransitionTimer.GetTimingRatio());
			SetSubWeathers(configSubWeatherCollection);
			if (_weatherTransitionTimer.isTimeUp)
			{
				if (_isInMiddleWeather && _subWeatherStack.GetRealTopIndex() == _middleSubWeatherIndex)
				{
					PopWeather(_middleSubWeatherIndex);
					_isInMiddleWeather = false;
					_middleSubWeatherIndex = 0;
				}
				else
				{
					_weatherState = WeatherState.Idle;
					_currentSubWeather = configSubWeatherCollection;
				}
			}
		}

		public virtual void Update()
		{
			Shader.SetGlobalVector("_miHoYo_CameraRight", _mainCameraTransform.transform.right);
			UpdateRenderingData();
			UpdateSubWeather();
		}

		private Material[] CollectAndAssignInstancedMaterials()
		{
			Dictionary<Material, Material> dictionary = new Dictionary<Material, Material>();
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(true);
			int num = 1;
			foreach (Renderer renderer in componentsInChildren)
			{
				Material[] sharedMaterials = renderer.sharedMaterials;
				Material[] array = new Material[sharedMaterials.Length];
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					Material material = sharedMaterials[j];
					if (!dictionary.ContainsKey(material))
					{
						Material material2 = new Material(material);
						material2.renderQueue = material.renderQueue;
						material2.name = string.Format("{0} #{1}", material.name, num++);
						dictionary.Add(material, material2);
					}
					array[j] = dictionary[material];
				}
				renderer.materials = array;
			}
			Material[] array2 = new Material[dictionary.Count];
			dictionary.Values.CopyTo(array2, 0);
			return array2;
		}

		private Material[] CollectSharedMaterials()
		{
			HashSet<Material> hashSet = new HashSet<Material>();
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					if (!hashSet.Contains(sharedMaterials[j]))
					{
						hashSet.Add(sharedMaterials[j]);
					}
				}
			}
			Material[] array = new Material[hashSet.Count];
			hashSet.CopyTo(array, 0);
			return array;
		}

		public void SetRenderingDataImmediately(ConfigStageRenderingData targetRenderingData)
		{
			_currentRendertingData = (ConfigStageRenderingData)targetRenderingData.Clone();
			if (_renderingDataState == RenderingDataState.RenderingTransit)
			{
				_renderingDataState = RenderingDataState.Idle;
				_renderingDataTransitionTimer.Reset(false);
			}
		}

		public void TransitRenderingData(ConfigStageRenderingData targetRenderingData, float duration)
		{
			_currentRendertingData.SetupTransition(targetRenderingData);
			_renderingDataState = RenderingDataState.RenderingTransit;
			_renderingDataTransitionTimer.timespan = duration;
			_renderingDataTransitionTimer.Reset(true);
		}

		private void OnRenderingDataChanged(RenderingDataTransition oldTransition, int oldIx, RenderingDataTransition newTransition, int newIx)
		{
			if (oldTransition.renderingData != newTransition.renderingData)
			{
				TransitRenderingData(duration: (oldIx == newIx) ? newTransition.transitDuration : ((oldIx <= newIx) ? newTransition.transitDuration : oldTransition.transitDuration), targetRenderingData: newTransition.renderingData);
			}
		}

		public void SetBaseRenderingData(ConfigStageRenderingData renderingData, float duration)
		{
			_renderingDataStack.Set(0, new RenderingDataTransition(renderingData, duration));
		}

		public void SetBaseRenderingData(string renderingDataName, float duration)
		{
			ConfigStageRenderingData renderingDataByName = GetRenderingDataByName(renderingDataName);
			_renderingDataStack.Set(0, new RenderingDataTransition(renderingDataByName, duration));
		}

		public void ResetBaseRenderingData(float duration)
		{
			_renderingDataStack.Set(0, new RenderingDataTransition(_initBaseRenderingData, duration));
		}

		public bool IsCurrentRenderingDataBase()
		{
			return _renderingDataStack.GetRealTopIndex() == 0;
		}

		public void TrySetMiddleRenderingForBase()
		{
			if (IsCurrentRenderingDataBase() && _renderingDataState == RenderingDataState.RenderingTransit)
			{
				float duration = _renderingDataTransitionTimer.timespan - _renderingDataTransitionTimer.timer;
				_middleRenderingIndex = _renderingDataStack.Push(new RenderingDataTransition(GetCurrentRenderingData(), duration), true);
				_isInMiddleRendering = true;
			}
		}

		public int PushRenderingData(string renderingDataName, float transitDuration)
		{
			TrySetMiddleRenderingForBase();
			ConfigStageRenderingData renderingDataByName = GetRenderingDataByName(renderingDataName);
			return _renderingDataStack.Push(new RenderingDataTransition(renderingDataByName, transitDuration));
		}

		public int PushRenderingData(ConfigStageRenderingData renderingData, float transitDuration)
		{
			TrySetMiddleRenderingForBase();
			return _renderingDataStack.Push(new RenderingDataTransition(renderingData, transitDuration));
		}

		public void PopRenderingData(int stackIx)
		{
			_renderingDataStack.Pop(stackIx);
		}

		public ConfigStageRenderingData GetCurrentBaseRenderingData()
		{
			return _renderingDataStack.Get(0).renderingData;
		}

		public float GetCurrentRenderingDataLeftTimer()
		{
			float result = 0f;
			if (_renderingDataState == RenderingDataState.RenderingTransit)
			{
				result = _renderingDataTransitionTimer.timespan - _renderingDataTransitionTimer.timer;
			}
			return result;
		}

		public ConfigStageRenderingData GetCurrentRenderingData()
		{
			return (ConfigStageRenderingData)_currentRendertingData.Clone();
		}

		public void InitLightProb()
		{
			if (!(lightProbManager == null))
			{
				lightProbManager.Init();
			}
		}

		public void InitLightMapCorrection()
		{
			if (!(lightMapCorrectManager == null))
			{
				lightMapCorrectManager.Init();
			}
		}

		public void InitWindZone()
		{
			if (!(windZone == null))
			{
				MonoWindZone component = windZone.GetComponent<MonoWindZone>();
				component.Init();
			}
		}

		public void TransitWeatherData(ConfigWeatherData targetWeatherData, float renderingDataDuration, float weatherDuration)
		{
			TransitRenderingData(targetWeatherData.configRenderingData as ConfigStageRenderingData, renderingDataDuration);
			TransitSubWeather(targetWeatherData.configSubWeathers, weatherDuration);
		}

		private void OnSubWeatherChanged(SubWeatherTransition oldTransition, int oldIx, SubWeatherTransition newTransition, int newIx)
		{
			TransitSubWeather(transitDuration: (oldIx == newIx) ? newTransition.transitDuration : ((oldIx <= newIx) ? newTransition.transitDuration : oldTransition.transitDuration), subWeather: newTransition.weather);
			Singleton<StageManager>.Instance.SetBaseStageEffectSetting(newTransition.weather.stageEffectSetting);
		}

		private void SetBaseSubWeather(ConfigSubWeatherCollection subWeather, float duration)
		{
			_subWeatherStack.Set(0, new SubWeatherTransition(subWeather, 0, duration));
		}

		public string GetCurrentBaseWeatherName()
		{
			return _currentBaseWeatherName;
		}

		private ConfigSubWeatherCollection GetCurrentBaseSubWeatherData()
		{
			return _subWeatherStack.Get(0).weather;
		}

		public float GetCurrentWeatherDataLeftTimer()
		{
			float result = 0f;
			if (_weatherState == WeatherState.WeatherTransit)
			{
				result = _weatherTransitionTimer.timespan - _weatherTransitionTimer.timer;
			}
			return result;
		}

		public ConfigWeatherData GetCurrentWeatherData()
		{
			ConfigWeatherData configWeatherData = new ConfigWeatherData();
			configWeatherData.configRenderingData = GetCurrentRenderingData();
			configWeatherData.configSubWeathers = GetCurrentSubWeatherData();
			return configWeatherData;
		}

		public ConfigWeatherData GetCurrentBaseWeatherData()
		{
			ConfigWeatherData configWeatherData = new ConfigWeatherData();
			configWeatherData.configRenderingData = GetCurrentBaseRenderingData();
			configWeatherData.configSubWeathers = GetCurrentBaseSubWeatherData();
			return configWeatherData;
		}

		private ConfigSubWeatherCollection GetCurrentSubWeatherData()
		{
			if (_weatherState == WeatherState.Idle)
			{
				return _currentSubWeather.Copy();
			}
			if (_weatherState == WeatherState.WeatherTransit)
			{
				return ConfigSubWeatherCollection.Lerp(_currentSubWeather, _transitTargetSubWeather, _weatherTransitionTimer.GetTimingRatio());
			}
			return null;
		}

		public bool IsCurrentSubWeatherDataBase()
		{
			return _subWeatherStack.GetRealTopIndex() == 0;
		}

		public void TrySetMiddleSubWeatherForBase()
		{
			if (IsCurrentSubWeatherDataBase() && _weatherState == WeatherState.WeatherTransit)
			{
				float duration = _weatherTransitionTimer.timespan - _weatherTransitionTimer.timer;
				_middleSubWeatherIndex = _subWeatherStack.Push(new SubWeatherTransition(GetCurrentSubWeatherData(), _middleRenderingIndex, duration), true);
				_isInMiddleRendering = true;
			}
		}

		private int PushSubWeatherData(ConfigSubWeatherCollection subWeather, int renderingIdx, int settingIx, float transitDuration)
		{
			TrySetMiddleSubWeatherForBase();
			return _subWeatherStack.Push(new SubWeatherTransition(subWeather, renderingIdx, settingIx, transitDuration));
		}

		public int PushWeather(string weatherName, float transitDuration)
		{
			ConfigWeatherData weatherDataByName = GetWeatherDataByName(weatherName);
			int renderingIdx = PushRenderingData(weatherDataByName.configRenderingData as ConfigStageRenderingData, transitDuration);
			int settingIx = -1;
			if (weatherDataByName.configSubWeathers.stageEffectSetting != null)
			{
				settingIx = Singleton<StageManager>.Instance.PushStageSettingData(weatherDataByName.configSubWeathers.stageEffectSetting);
			}
			return PushSubWeatherData(weatherDataByName.configSubWeathers, renderingIdx, settingIx, transitDuration);
		}

		public void PopWeather(int stackIx)
		{
			int renderingIndexInStack = _subWeatherStack.Get(stackIx).renderingIndexInStack;
			if (renderingIndexInStack != -1)
			{
				_renderingDataStack.Pop(renderingIndexInStack);
			}
			int stageEffectSettingIndexInStack = _subWeatherStack.Get(stackIx).stageEffectSettingIndexInStack;
			if (stageEffectSettingIndexInStack != -1)
			{
				Singleton<StageManager>.Instance.PopStageSettingData(stageEffectSettingIndexInStack);
			}
			_subWeatherStack.Pop(stackIx);
		}

		public void SetWeahterImmediately(ConfigWeatherData config)
		{
			if (!Application.isPlaying)
			{
				if (config.configRenderingData != null)
				{
					config.configRenderingData.ApplyGlobally();
				}
			}
			else
			{
				SetRenderingDataImmediately(config.configRenderingData as ConfigStageRenderingData);
				_currentRendertingData.ApplyGlobally();
			}
			SetSubWeathersImmediately(config.configSubWeathers);
		}

		public void SetBaseWeather(string weatherName, float duration)
		{
			_currentBaseWeatherName = weatherName;
			ConfigWeatherData weatherDataByName = GetWeatherDataByName(weatherName);
			SetBaseRenderingData(weatherDataByName.configRenderingData as ConfigStageRenderingData, duration);
			SetBaseSubWeather(weatherDataByName.configSubWeathers, duration);
		}

		public void ResetBaseWeather(float duration)
		{
			ResetBaseRenderingData(duration);
			ResetBaseSubWeather(duration);
		}

		private void ResetBaseSubWeather(float duration)
		{
			_subWeatherStack.Set(0, new SubWeatherTransition(_initBaseSubWeather, 0, duration));
		}

		private void SetSubWeathersImmediately(ConfigSubWeatherCollection subWeather)
		{
			SetSubWeathers(subWeather);
			_currentSubWeather = subWeather;
			if (_weatherState == WeatherState.WeatherTransit)
			{
				_weatherState = WeatherState.Idle;
				_weatherTransitionTimer.Reset(false);
			}
		}

		private void SetSubWeathers(ConfigSubWeatherCollection subWeather)
		{
			SetRain(subWeather.configRain);
		}

		private void TransitSubWeather(ConfigSubWeatherCollection subWeather, float transitDuration)
		{
			if (_weatherState == WeatherState.WeatherTransit)
			{
				_currentSubWeather = ConfigSubWeatherCollection.Lerp(_currentSubWeather, _transitTargetSubWeather, _weatherTransitionTimer.GetTimingRatio());
				_transitTargetSubWeather = subWeather;
			}
			else if (_weatherState == WeatherState.Idle)
			{
				_transitTargetSubWeather = subWeather;
				_weatherState = WeatherState.WeatherTransit;
			}
			ConfigSubWeatherCollection.LerpPreparation(_currentSubWeather, _transitTargetSubWeather);
			_weatherTransitionTimer.timespan = transitDuration;
			_weatherTransitionTimer.Reset(true);
		}

		private void SetRain(ConfigRain config)
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (config == null || config.density < 0.001f)
			{
				if (_rainObject != null)
				{
					UnityEngine.Object.Destroy(_rainObject);
					_rainObject = null;
				}
				return;
			}
			if (_rainObject == null)
			{
				_rainObject = UnityEngine.Object.Instantiate(_rainPrefab);
				_rainObject.transform.SetParent(base.transform);
				rainController = _rainObject.GetComponent<RainController>();
				rainController.Init();
			}
			rainController.SetRain(config);
		}

		private void AuxTimeScaleCallback(float oldValue, int oldIx, float newValue, int newIx)
		{
			if (rainController != null)
			{
				if (newValue < 1f)
				{
					rainController.EnterSlowMode(oldValue);
				}
				else
				{
					rainController.LeaveSlowMode();
				}
			}
		}

		public void OnDestroy()
		{
		}

		public ContinueWeatherDataSettings GetContinueWeatherDataSetup()
		{
			ContinueWeatherDataSettings continueWeatherDataSettings = new ContinueWeatherDataSettings();
			continueWeatherDataSettings.renderingDataContinueTimer = GetCurrentRenderingDataLeftTimer();
			continueWeatherDataSettings.weatherContinueTimer = GetCurrentWeatherDataLeftTimer();
			continueWeatherDataSettings.currentWeatherData = GetCurrentWeatherData();
			continueWeatherDataSettings.continueWeatherData = GetCurrentBaseWeatherData();
			continueWeatherDataSettings.continueWeatherName = GetCurrentBaseWeatherName();
			return continueWeatherDataSettings;
		}

		public void TriggerTint(string renderDataName, float duration, float transitDuration)
		{
			StartCoroutine(TintIter(renderDataName, duration, transitDuration));
		}

		private IEnumerator TintIter(string renderDataName, float duration, float transitDuration)
		{
			int stackIx = PushRenderingData(renderDataName, transitDuration);
			while (duration > 0f)
			{
				duration -= Time.deltaTime * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
				yield return null;
			}
			PopRenderingData(stackIx);
		}
	}
}
