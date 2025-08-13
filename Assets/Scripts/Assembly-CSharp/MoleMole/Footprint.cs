using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class Footprint : MonoBehaviour
	{
		[Serializable]
		public struct Setting
		{
			public string name;

			public string[] avatarTypeNames;

			public Material material;

			public float size;

			public Vector3 offset;
		}

		private class State
		{
			public Setting setting;

			public BaseStepController controller;

			public ParticleSystem particleSystem;

			public BaseStepController.Pattern lastLeftPatten;

			public BaseStepController.Pattern rightLeftPatten;
		}

		public GameObject particleSystemPrefab;

		public MonoZone2D zone;

		public Material material;

		public float size = 1f;

		public Vector3 offset;

		public int maxCount = 1000;

		public float duration;

		public Setting[] avatarSettings;

		private Dictionary<BaseMonoAvatar, State> _avatarStateDict = new Dictionary<BaseMonoAvatar, State>();

		private Setting _defaultSetting;

		private Dictionary<string, Setting> _avatarTypeNameSettingDict = new Dictionary<string, Setting>();

		private List<ParticleSystem> _particleSystemList = new List<ParticleSystem>();

		private Dictionary<Setting, ParticleSystem> _settingParticleSystemDict = new Dictionary<Setting, ParticleSystem>();

		private Transform _lightForwardTransform;

		private Vector3 _lightDir = new Vector3(0f, -1f, 1f);

		private void Awake()
		{
			ParseAvatarSettings();
			CreateParticleSystems();
		}

		private void Update()
		{
			if (_lightForwardTransform == null)
			{
				_lightForwardTransform = Singleton<StageManager>.Instance.GetStageEnv().lightForwardTransform;
				_lightDir = _lightForwardTransform.forward;
			}
			List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
			for (int i = 0; i < allAvatars.Count; i++)
			{
				BaseMonoAvatar baseMonoAvatar = allAvatars[i];
				if (!baseMonoAvatar.IsActive() || (zone != null && !zone.Contain(baseMonoAvatar.XZPosition)))
				{
					continue;
				}
				State state;
				if (_avatarStateDict.ContainsKey(baseMonoAvatar))
				{
					state = _avatarStateDict[baseMonoAvatar];
				}
				else
				{
					state = new State();
					string avatarTypeName = baseMonoAvatar.AvatarTypeName;
					state.setting = ((!_avatarTypeNameSettingDict.ContainsKey(avatarTypeName)) ? _defaultSetting : _avatarTypeNameSettingDict[avatarTypeName]);
					state.particleSystem = _settingParticleSystemDict[state.setting];
					state.controller = baseMonoAvatar.GetComponent<BaseStepController>();
					state.lastLeftPatten = BaseStepController.Pattern.Void;
					state.rightLeftPatten = BaseStepController.Pattern.Void;
					if (state.controller == null)
					{
					}
					_avatarStateDict.Add(baseMonoAvatar, state);
				}
				BaseStepController controller = state.controller;
				if (!(controller == null))
				{
					HandleStep(controller.currentLeftStepParam, ref state.lastLeftPatten, state);
					HandleStep(controller.currentRightStepParam, ref state.rightLeftPatten, state);
				}
			}
		}

		private void ParseAvatarSettings()
		{
			_defaultSetting = default(Setting);
			_defaultSetting.name = "Default";
			_defaultSetting.material = material;
			_defaultSetting.size = size;
			_defaultSetting.offset = offset;
			Setting[] array = avatarSettings;
			for (int i = 0; i < array.Length; i++)
			{
				Setting value = array[i];
				string[] avatarTypeNames = value.avatarTypeNames;
				foreach (string key in avatarTypeNames)
				{
					_avatarTypeNameSettingDict.Add(key, value);
				}
			}
		}

		private void CreateParticleSystem(Setting setting)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(particleSystemPrefab);
			gameObject.name = string.Format("__ParticelSystem_for_setting_{0}", setting.name);
			gameObject.transform.SetParentAndReset(base.transform);
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			SetParticleSystem(component, setting);
			_particleSystemList.Add(component);
			_settingParticleSystemDict.Add(setting, component);
		}

		private void CreateParticleSystems()
		{
			Setting[] array = avatarSettings;
			foreach (Setting setting in array)
			{
				CreateParticleSystem(setting);
			}
			CreateParticleSystem(_defaultSetting);
		}

		private void SetParticleSystem(ParticleSystem ps, Setting setting)
		{
			ps.simulationSpace = ParticleSystemSimulationSpace.World;
			ps.maxParticles = maxCount;
			ParticleSystemRenderer component = ps.GetComponent<ParticleSystemRenderer>();
			component.material = setting.material;
			component.renderMode = ParticleSystemRenderMode.HorizontalBillboard;
			ps.startSpeed = 0f;
			ParticleSystem.EmissionModule emission = ps.emission;
			emission.enabled = false;
		}

		private void HandleStep(BaseStepController.Param param, ref BaseStepController.Pattern lastPatten, State state)
		{
			BaseStepController.Param param2 = param;
			if (lastPatten == BaseStepController.Pattern.Down && param2.pattern == BaseStepController.Pattern.Static)
			{
				AddFootprint(param2.position, param2.toeForwardXZ, state);
			}
			lastPatten = param.pattern;
		}

		private void AddFootprint(Vector3 position, Vector3 direction, State state)
		{
			Setting setting = state.setting;
			ParticleSystem particleSystem = state.particleSystem;
			float num = Mathf.Atan2(direction.x, direction.z) * 57.29578f;
			ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
			Vector3 position2 = position + setting.offset;
			position2.y = 0f;
			emitParams.position = position2;
			emitParams.axisOfRotation = Vector3.up;
			emitParams.rotation = num;
			emitParams.startSize = setting.size;
			emitParams.startLifetime = duration;
			Quaternion quaternion = Quaternion.Euler(0f, num, 0f);
			Vector3 vector = quaternion * -_lightDir;
			vector = (vector + Vector3.one) * 0.5f;
			emitParams.startColor = new Color(vector.x, vector.y, vector.z, 1f);
			particleSystem.Emit(emitParams, 1);
		}
	}
}
