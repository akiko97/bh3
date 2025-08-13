using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoEffect : BaseMonoEffect
	{
		private BaseMonoEffectPlugin[] _effectPlugins;

		private ParticleSystem _mainParticleSystem;

		private ParticleSystem[] _allParticleSystems;

		private ParticleSystemRenderer[] _allParticleSystemRenderers;

		private Animation[] _allAnimations;

		private float _lastTimeScale;

		private bool _isToBeRemoved;

		public bool dontDestroyWhenOwnerEvade;

		[NonSerialized]
		public bool disableGORecursively;

		public bool IgnoreTimescale;

		[NonSerialized]
		public string belongSkillName;

		public BaseMonoEntity owner { get; private set; }

		public ParticleSystem mainParticleSystem
		{
			get
			{
				return _mainParticleSystem;
			}
		}

		public override float TimeScale
		{
			get
			{
				return (!(owner != null)) ? Singleton<LevelManager>.Instance.levelEntity.TimeScale : owner.TimeScale;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_allParticleSystems = GetComponentsInChildren<ParticleSystem>();
			_allParticleSystemRenderers = new ParticleSystemRenderer[_allParticleSystems.Length];
			for (int i = 0; i < _allParticleSystems.Length; i++)
			{
				if (_allParticleSystems[i].loop)
				{
					ParticleSystem.SubEmittersModule subEmitters = _allParticleSystems[i].subEmitters;
					subEmitters.enabled = true;
				}
				_allParticleSystemRenderers[i] = _allParticleSystems[i].GetComponent<ParticleSystemRenderer>();
			}
			_mainParticleSystem = GetComponentInChildren<ParticleSystem>();
			_allAnimations = GetComponentsInChildren<Animation>();
			_effectPlugins = GetComponents<BaseMonoEffectPlugin>();
		}

		private void OnValidate()
		{
			MonoEffect componentInParent = GetComponentInParent<MonoEffect>();
			if (!(componentInParent != null))
			{
			}
		}

		public override void Setup()
		{
			if (_effectPlugins != null)
			{
				int i = 0;
				for (int num = _effectPlugins.Length; i < num; i++)
				{
					_effectPlugins[i].Setup();
				}
			}
			_isToBeRemoved = false;
			_lastTimeScale = TimeScale;
			belongSkillName = null;
			SetParticleAndAnimationPlaySpeed((!IgnoreTimescale) ? _lastTimeScale : 1f);
			int j = 0;
			for (int num2 = _allParticleSystems.Length; j < num2; j++)
			{
				_allParticleSystems[j].gameObject.SetActive(true);
			}
		}

		public void SetOwner(BaseMonoEntity owner)
		{
			this.owner = owner;
		}

		public void SetupPlugin()
		{
			if (GetComponent<MonoEffectPluginFollow>() != null)
			{
				GetComponent<MonoEffectPluginFollow>().SetFollowParentTarget(owner.transform);
			}
			if (GetComponent<MonoEffectPluginSkinMeshShape>() != null)
			{
				GetComponent<MonoEffectPluginSkinMeshShape>().SetupSkinmesh(owner);
			}
			if (GetComponent<MonoEffectPluginDisableOnPropertyBlock>() != null)
			{
				GetComponent<MonoEffectPluginDisableOnPropertyBlock>().SetupRenderer(owner);
			}
			SetupOverride(owner);
		}

		public void SetupPluginFromTo(BaseMonoEntity toEntity)
		{
			if (GetComponent<MonoEffectPluginFollow>() != null)
			{
				GetComponent<MonoEffectPluginFollow>().SetFollowParentTarget(toEntity.transform);
			}
			if (GetComponent<MonoEffectPluginMoveToTarget>() != null)
			{
				GetComponent<MonoEffectPluginMoveToTarget>().SetMoveToTarget(toEntity);
			}
			if (GetComponent<MonoEffectPluginSkinMeshShape>() != null)
			{
				GetComponent<MonoEffectPluginSkinMeshShape>().SetupSkinmesh(owner);
			}
			if (GetComponent<MonoEffectPluginDisableOnPropertyBlock>() != null)
			{
				GetComponent<MonoEffectPluginDisableOnPropertyBlock>().SetupRenderer(owner);
			}
			SetupOverride(owner);
		}

		public void SetupOverride(BaseMonoEntity owner)
		{
			MonoEffectOverride component = owner.GetComponent<MonoEffectOverride>();
			if (!(component != null))
			{
				return;
			}
			MonoEffectPluginTrailSmooth component2 = GetComponent<MonoEffectPluginTrailSmooth>();
			if (component2 != null)
			{
				component2.HandleEffectOverride(component);
			}
			else
			{
				MonoEffectPluginTrailStatic component3 = GetComponent<MonoEffectPluginTrailStatic>();
				if (component3 != null)
				{
					component3.HandleEffectOverride(component);
				}
			}
			MonoEffectPluginOverrideHandler component4 = GetComponent<MonoEffectPluginOverrideHandler>();
			if (component4 != null)
			{
				component4.HandleEffectOverride(component);
			}
		}

		public override bool IsToBeRemove()
		{
			if (_isToBeRemoved)
			{
				return true;
			}
			for (int i = 0; i < _effectPlugins.Length; i++)
			{
				if (_effectPlugins[i].IsToBeRemove())
				{
					return true;
				}
			}
			if (_mainParticleSystem != null && _mainParticleSystem.gameObject.activeInHierarchy)
			{
				for (int j = 0; j < _allParticleSystems.Length; j++)
				{
					if (_allParticleSystems[j].IsAlive(false))
					{
						return false;
					}
				}
				return true;
			}
			return _isToBeRemoved;
		}

		public override void Update()
		{
			base.Update();
			if (IgnoreTimescale)
			{
				float timeScale = Time.timeScale;
				if (_lastTimeScale != timeScale)
				{
					SetParticleAndAnimationPlaySpeed((timeScale != 0f) ? (1f / timeScale) : 0f);
				}
				_lastTimeScale = Time.timeScale;
			}
			else
			{
				float timeScale2 = TimeScale;
				if (_lastTimeScale != timeScale2)
				{
					SetParticleAndAnimationPlaySpeed(timeScale2);
				}
				_lastTimeScale = TimeScale;
			}
		}

		public void Pause()
		{
			SetParticleAndAnimationPaused(true);
		}

		public void Resume()
		{
			SetParticleAndAnimationPaused(false);
		}

		private void SetParticleAndAnimationPaused(bool paused)
		{
			for (int i = 0; i < _allParticleSystems.Length; i++)
			{
				ParticleSystem particleSystem = _allParticleSystems[i];
				if (particleSystem != null)
				{
					ParticleSystem.EmissionModule emission = particleSystem.emission;
					emission.enabled = !paused;
				}
			}
			if (_allAnimations != null)
			{
				for (int j = 0; j < _allAnimations.Length; j++)
				{
					Animation animation = _allAnimations[j];
					if (!(animation == null) && !(animation.clip == null))
					{
						if (paused)
						{
							animation.Stop();
						}
						else
						{
							animation.Play();
						}
					}
				}
			}
			List<MeshRenderer> list = new List<MeshRenderer>();
			base.transform.GetComponentsInChildren(list);
			foreach (MeshRenderer item in list)
			{
				item.enabled = !paused;
			}
		}

		private void SetParticleAndAnimationPlaySpeed(float speed)
		{
			if (_allParticleSystems != null)
			{
				for (int i = 0; i < _allParticleSystems.Length; i++)
				{
					_allParticleSystems[i].playbackSpeed = speed;
				}
			}
			if (_allAnimations == null)
			{
				return;
			}
			for (int j = 0; j < _allAnimations.Length; j++)
			{
				Animation animation = _allAnimations[j];
				if (!(animation == null) && !(animation.clip == null))
				{
					animation[animation.clip.name].speed = speed;
				}
			}
		}

		public void SetDestroy()
		{
			if (dontDestroyWhenOwnerEvade)
			{
				return;
			}
			for (int i = 0; i < _effectPlugins.Length; i++)
			{
				_effectPlugins[i].SetDestroy();
			}
			for (int j = 0; j < _allParticleSystems.Length; j++)
			{
				_allParticleSystems[j].Stop(false);
				if (!_allParticleSystemRenderers[j].isVisible)
				{
					_allParticleSystems[j].Clear();
				}
			}
		}

		public void SetDestroyImmediately()
		{
			for (int i = 0; i < _effectPlugins.Length; i++)
			{
				_effectPlugins[i].SetDestroy();
			}
			for (int j = 0; j < _allParticleSystems.Length; j++)
			{
				_allParticleSystems[j].Stop(false);
				_allParticleSystems[j].Clear();
			}
			_isToBeRemoved = true;
		}

		public override bool IsActive()
		{
			return !_isToBeRemoved;
		}

		private void OnDisable()
		{
			if (disableGORecursively)
			{
				int i = 0;
				for (int num = _allParticleSystems.Length; i < num; i++)
				{
					_allParticleSystems[i].gameObject.SetActive(false);
				}
			}
		}
	}
}
