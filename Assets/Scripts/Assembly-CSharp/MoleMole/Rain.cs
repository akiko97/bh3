using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(ParticleSystem))]
	public class Rain : MonoBehaviour
	{
		[HideInInspector]
		public float timeScale;

		public float speed = 1f;

		[Header("Sheet animation")]
		public int tiles_X = 1;

		public int tiles_Y = 1;

		public AnimationCurve sheetAnmCurve;

		[Header("Appearance")]
		public float origSpeedStrech = 0.02f;

		public AnimationCurve speedStrechCurve;

		public float origSize = 0.005f;

		public AnimationCurve sizeCurve;

		public float origOpaqueness = 1f;

		public AnimationCurve opaquenessCurve;

		public float radius = 20f;

		[HideInInspector]
		public float area;

		[Range(0f, 1f)]
		public float origAudioVolumn = 1f;

		[Range(-3f, 3f)]
		public float origAudioPitch = 1f;

		private ParticleSystem __particleSystem;

		private ParticleSystemRenderer _particleRenderer;

		private Material _material;

		private ParticleSystem.Particle[] _particles;

		private int _particleCount;

		private ParticleSystem _particleSystem
		{
			get
			{
				if (__particleSystem == null)
				{
					__particleSystem = GetComponent<ParticleSystem>();
				}
				return __particleSystem;
			}
		}

		private void Update()
		{
			UpdateMisc();
			GetParticle();
			float num = timeScale * Time.timeScale;
			SetPlaybackSpeed(num);
			SetSheetAnimation(num);
			SetSize(num);
			SetVelocityScale(num);
			SetOpaqueness(num);
			SetParticle();
			SetSound(num);
		}

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
		}

		public void Init()
		{
			_particleRenderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
			_material = _particleRenderer.material;
			area = (float)Math.PI * radius * radius;
		}

		public void SetUpAudio(ConfigRain config)
		{
		}

		public void SetUp(ConfigRain config)
		{
			ParticleSystem.EmissionModule emission = _particleSystem.emission;
			emission.rate = new ParticleSystem.MinMaxCurve(config.density * area);
			speed = config.speed;
			origSpeedStrech = config.speedStrech;
			origSize = config.size;
			origOpaqueness = config.opaqueness;
			SetUpAudio(config);
		}

		private void UpdateMisc()
		{
			if (Application.isEditor)
			{
				area = (float)Math.PI * radius * radius;
			}
		}

		private void GetParticle()
		{
			if (_particles == null || _particles.Length < _particleSystem.maxParticles)
			{
				_particles = new ParticleSystem.Particle[_particleSystem.maxParticles];
			}
			_particleCount = _particleSystem.GetParticles(_particles);
		}

		private void SetParticle()
		{
			_particleSystem.SetParticles(_particles, _particleCount);
		}

		private void SetSound(float t)
		{
		}

		private void SetPlaybackSpeed(float t)
		{
			_particleSystem.playbackSpeed = timeScale * speed;
		}

		private void SetSheetAnimation(float t)
		{
			int num = tiles_X * tiles_Y;
			int num2 = Mathf.FloorToInt(sheetAnmCurve.Evaluate(t) * (float)num);
			float num3 = 1f / (float)tiles_X;
			float num4 = 1f / (float)tiles_Y;
			float x = (float)(num2 % tiles_X) * num3;
			float y = (float)((num - 1 - num2) / tiles_X) * num4;
			_material.SetTextureScale("_MainTex", new Vector2(num3, num4));
			_material.SetTextureOffset("_MainTex", new Vector2(x, y));
		}

		private void SetSize(float t)
		{
			float startSize = origSize * sizeCurve.Evaluate(t);
			for (int i = 0; i < _particleCount; i++)
			{
				_particles[i].startSize = startSize;
			}
		}

		private void SetVelocityScale(float t)
		{
			_particleRenderer.velocityScale = origSpeedStrech * speedStrechCurve.Evaluate(t);
		}

		private void SetOpaqueness(float t)
		{
			float value = opaquenessCurve.Evaluate(t) * origOpaqueness;
			_material.SetFloat("_Opaqueness", Mathf.Clamp01(value));
		}
	}
}
