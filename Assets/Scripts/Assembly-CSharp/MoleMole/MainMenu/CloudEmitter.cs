using System.Collections.Generic;
using UnityEngine;

namespace MoleMole.MainMenu
{
	[RequireComponent(typeof(ParticleSystem))]
	public class CloudEmitter : MonoBehaviour
	{
		private class CollidedParticle
		{
			public Vector3 RealPosition;

			public Vector3 RealVelocity;
		}

		[Header("Control")]
		[Range(0f, 100f)]
		public float PlaybackSpeed = 1f;

		[Header("Clouds")]
		public float Speed = 30f;

		public float AspectRatio = 1f;

		public Color BrightColor = Color.white;

		public Color DarkColor = Color.gray;

		public Color SecondDarkColor = Color.gray * 0.5f;

		public Color RimColor = Color.yellow;

		public Color FlashColor = Color.white;

		[Tooltip("X: constant Y: linear Z: quadratic")]
		public Vector3 FlashAttenuationFactors = new Vector3(1f, 1f, 0.001f);

		[Header("Emitters")]
		public Vector3 EmitterSize = new Vector3(400f, 100f, 1000f);

		public ParticleSystemSimulationSpace SimulationSpace;

		[Header("Material")]
		public Material ParticleMaterial;

		public int MaterialTileGridX = 1;

		public int MaterialTileGridY = 1;

		[Header("The box collider need to be axis aligned")]
		public BoxCollider ShipCollider;

		public float ShipColliderSkin = 5f;

		public float ShipFrontDistanceForSight = 2000f;

		private Rect _shipColliderRect;

		private float _shipColliderZ;

		[Header("Cloud scenes")]
		public int CloudSceneId;

		public CloudScene[] CloudScenes;

		private int _materialCount = 1;

		private int _maxCloudCount = 1000;

		private ParticleSystem _particleSystem;

		private ParticleSystemRenderer _renderer;

		private Material _material;

		private int _frameCount;

		private ParticleSystem.Particle[] _particles;

		private int _particleCount;

		private Dictionary<int, CollidedParticle> _collidedParticleMap = new Dictionary<int, CollidedParticle>();

		private int _nextParticleId;

		private bool _isPaused;

		private bool _needRestoreLastState;

		private float _lastTime;

		private float _lastDeltaTime;

		private int _lastFrameCount = -1;

		private Camera _camera
		{
			get
			{
				return Camera.main;
			}
		}

		private float _deltaTime
		{
			get
			{
				if (_particleSystem.isPaused)
				{
					return 0f;
				}
				float num = 0f;
				if (Application.isPlaying)
				{
					num = Time.deltaTime;
				}
				else if (_lastFrameCount == _frameCount)
				{
					num = _lastDeltaTime;
				}
				else
				{
					num = Time.realtimeSinceStartup - _lastTime;
					_lastTime = Time.realtimeSinceStartup;
				}
				_lastFrameCount = _frameCount;
				if (num > 1f / 30f)
				{
					num = 1f / 30f;
				}
				_lastDeltaTime = num;
				return num * PlaybackSpeed;
			}
		}

		private void Awake()
		{
			Init();
		}

		private void Init()
		{
			if (!Application.isPlaying)
			{
				_lastTime = Time.realtimeSinceStartup;
			}
			if (ShipCollider != null)
			{
				Vector3 center = ShipCollider.center;
				Vector3 size = ShipCollider.size;
				center = ShipCollider.transform.TransformPoint(center);
				center = base.transform.worldToLocalMatrix.MultiplyPoint(center);
				_shipColliderZ = center.z - size.z / 2f;
				_shipColliderRect = default(Rect);
				_shipColliderRect.size = size;
				_shipColliderRect.center = new Vector2(center.x, center.y);
			}
			InitParticleSystem();
			InitCloudScene();
		}

		public void OnEnable()
		{
			RestoreLastState();
		}

		public void OnDisable()
		{
			_needRestoreLastState = true;
		}

		public void Reset()
		{
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>(true);
			foreach (Transform transform in componentsInChildren)
			{
				if (!(transform == base.transform))
				{
					Object.Destroy(transform.gameObject);
				}
			}
			Init();
			SimulateInStart();
		}

		public void Pause()
		{
			_isPaused = true;
			_particleSystem.Pause();
		}

		public void Play()
		{
			_isPaused = false;
			_particleSystem.Play();
		}

		public bool IsPlaying()
		{
			return !_isPaused;
		}

		private void Update()
		{
			_frameCount++;
			Emit(_deltaTime);
			GetParticle();
			if (ShipCollider != null && _camera != null)
			{
				HandleCollision(_deltaTime);
			}
			UpdateLightning(_deltaTime);
			SetParticle();
		}

		private void SetMaterial()
		{
			_material.SetFloat("_EmitterWidth", EmitterSize.x);
			_material.SetFloat("_AspectRatio", AspectRatio);
			_material.SetVector("_TileSize", new Vector4(MaterialTileGridX, MaterialTileGridY, 0f, 0f));
			_material.SetColor("_BrightColor", BrightColor);
			_material.SetColor("_DarkColor", DarkColor);
			_material.SetColor("_SecondDarkColor", SecondDarkColor);
			_material.SetColor("_RimColor", RimColor);
			_material.SetColor("_FlashColor", FlashColor);
			_material.SetVector("_FlashAttenFactors", new Vector4(FlashAttenuationFactors.x, FlashAttenuationFactors.y, FlashAttenuationFactors.z));
		}

		private void InitParticleSystem()
		{
			_particleSystem = GetComponent<ParticleSystem>();
			_renderer = GetComponent<ParticleSystemRenderer>();
			_renderer.renderMode = ParticleSystemRenderMode.Mesh;
			_renderer.mesh = MeshGenerator.BillboardQuad();
			_material = new Material(ParticleMaterial);
			_material.hideFlags = HideFlags.DontSave;
			_renderer.material = _material;
			_materialCount = MaterialTileGridX * MaterialTileGridY;
			_particles = new ParticleSystem.Particle[_particleSystem.maxParticles];
			SetParticleSystem();
		}

		private void SetParticleSystem()
		{
			SetParticleSystemParams();
			SetMaterial();
		}

		private void InitCloudScene()
		{
			CloudScene[] cloudScenes = CloudScenes;
			foreach (CloudScene cloudScene in cloudScenes)
			{
				cloudScene.Init(this);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Vector3 center = new Vector3(0f, 0f, EmitterSize.z / 2f);
			Gizmos.DrawWireCube(center, EmitterSize);
		}

		private void SetParticleSystemParams()
		{
			_particleSystem.startSpeed = Speed;
			_particleSystem.startLifetime = EmitterSize.z / Speed;
			_particleSystem.simulationSpace = SimulationSpace;
			_particleSystem.playbackSpeed = PlaybackSpeed;
			_particleSystem.maxParticles = _maxCloudCount;
		}

		public void SetupCloudConfig(ConfigAtmosphereCommon commonConfig, ConfigCloudStyle config)
		{
			bool flag = false;
			int num = -1;
			string scneneName = commonConfig.ScneneName;
			for (int i = 0; i < CloudScenes.Length; i++)
			{
				if (CloudScenes[i].Name == scneneName)
				{
					num = i;
					break;
				}
			}
			flag = num != CloudSceneId;
			CloudSceneId = num;
			PlaybackSpeed = commonConfig.PlaybackSpeed;
			BrightColor = config.BrightColor;
			DarkColor = config.DarkColor;
			SecondDarkColor = config.SecondDarkColor;
			RimColor = config.RimColor;
			FlashColor = config.FlashColor;
			FlashAttenuationFactors = config.FlashAttenuationFactors;
			if (flag)
			{
				Reset();
			}
			else
			{
				SetParticleSystem();
			}
		}

		private float ParticleSize(ref ParticleSystem.Particle particle)
		{
			return (float)(int)particle.startColor.g * 2f / 255f * EmitterSize.x;
		}

		private int GetParticleId()
		{
			if (_collidedParticleMap.ContainsKey(_nextParticleId))
			{
				_collidedParticleMap.Remove(_nextParticleId);
			}
			int nextParticleId = _nextParticleId;
			_nextParticleId++;
			if (_nextParticleId > _maxCloudCount * 10)
			{
				_nextParticleId = 0;
			}
			return nextParticleId;
		}

		private void EmitCloudType(CloudType cloudType, float deltaTime)
		{
			if (cloudType.MaterialIds.Length == 0)
			{
				return;
			}
			int emitCount = cloudType.GetEmitCount(deltaTime);
			if (emitCount != 0)
			{
				int[] materialIds = cloudType.MaterialIds;
				Vector3 vector = default(Vector3);
				Color color = new Color(0f, 0f, 1f, 1f);
				Vector3 velocity = Vector3.forward * _particleSystem.startSpeed;
				Vector3 vector2 = EmitterSize / 2f;
				for (int i = 0; i < emitCount; i++)
				{
					int num = Random.Range(0, materialIds.Length);
					color.r = (float)materialIds[num] / (float)_materialCount;
					color.r += 0.5f / (float)_materialCount;
					float randomSize = cloudType.GetRandomSize();
					color.g = randomSize / 2f;
					vector = cloudType.GetRandomPosition();
					vector = vector * 2f - Vector3.one;
					vector.x *= vector2.x;
					vector.x = 0f - vector.x;
					vector.y *= vector2.y;
					vector.z = 0f;
					_particleSystem.Emit(new ParticleSystem.EmitParams
					{
						position = vector,
						velocity = velocity,
						startSize = GetParticleId(),
						startLifetime = _particleSystem.startLifetime,
						startColor = color
					}, 1);
				}
			}
		}

		private void EmitCompoundCloudType(CompoundCloudType compoundCloudType, float deltaTime)
		{
			if (compoundCloudType.CloudTypes.Length == 0)
			{
				return;
			}
			int emitCount = compoundCloudType.GetEmitCount(deltaTime);
			if (emitCount == 0)
			{
				return;
			}
			CloudType[] cloudTypes = compoundCloudType.CloudTypes;
			Vector3 vector = compoundCloudType.GetRandomSize() / 2f;
			Vector3 vector2 = compoundCloudType.GetRandomPosition();
			vector2 = vector2 * 2f - Vector3.one;
			vector2.x = 0f - vector2.x;
			vector2.x *= EmitterSize.x / 2f;
			vector2.y *= EmitterSize.y / 2f;
			Color color = new Color(0f, 0f, 1f, 1f);
			Vector3 velocity = Vector3.forward * _particleSystem.startSpeed;
			for (int i = 0; i < emitCount; i++)
			{
				CloudType[] array = cloudTypes;
				foreach (CloudType cloudType in array)
				{
					for (int k = 0; k < cloudType.EmitCount; k++)
					{
						int[] materialIds = cloudType.MaterialIds;
						Vector3 vector3 = default(Vector3);
						int num = Random.Range(0, materialIds.Length);
						color.r = (float)materialIds[num] / (float)_materialCount;
						color.r += 0.5f / (float)_materialCount;
						float randomSize = cloudType.GetRandomSize();
						color.g = randomSize / 2f;
						vector3 = cloudType.GetRandomPosition();
						vector3 = vector3 * 2f - Vector3.one;
						vector3.x *= vector.x;
						vector3.x = 0f - vector3.x;
						vector3.y *= vector.y;
						vector3 += vector2;
						vector3.z += vector.z * 2f * Random.value;
						_particleSystem.Emit(new ParticleSystem.EmitParams
						{
							position = vector3,
							velocity = velocity,
							startSize = GetParticleId(),
							startLifetime = _particleSystem.startLifetime,
							startColor = color
						}, 1);
					}
				}
			}
		}

		private void EmitLayerCloudType(LayerCloudType layerCloudType, float deltaTime)
		{
			if (layerCloudType.MaterialIds.Length != 0 && layerCloudType.IsEmit(deltaTime))
			{
				int[] materialIds = layerCloudType.MaterialIds;
				Vector3 vector = default(Vector3);
				float num = 0f;
				if (!layerCloudType.IsOdd())
				{
					num = (0f - layerCloudType.InterleavedOffset) * layerCloudType.GetRandomSize();
				}
				Color color = new Color(0f, 0f, 1f, 1f);
				Vector3 velocity = Vector3.forward * _particleSystem.startSpeed;
				Vector3 vector2 = EmitterSize / 2f;
				while (num < 1f)
				{
					int num2 = Random.Range(0, materialIds.Length);
					color.r = (float)materialIds[num2] / (float)_materialCount;
					color.r += 0.5f / (float)_materialCount;
					float randomSize = layerCloudType.GetRandomSize();
					color.g = randomSize / 2f;
					num = (vector.x = num + randomSize / 2f) + layerCloudType.GetRandomGap() * randomSize;
					vector.y = layerCloudType.GetRandomPositionY();
					vector = vector * 2f - Vector3.one;
					vector.x *= vector2.x;
					vector.x = 0f - vector.x;
					vector.y *= vector2.y;
					vector.z = Random.value * 10f;
					_particleSystem.Emit(new ParticleSystem.EmitParams
					{
						position = vector,
						velocity = velocity,
						startSize = GetParticleId(),
						startLifetime = _particleSystem.startLifetime * Random.Range(0.99f, 1.01f),
						startColor = color
					}, 1);
				}
			}
		}

		private void EmitCloudScene(int cloudSceneId, float deltaTime)
		{
			if (cloudSceneId < 0 || cloudSceneId >= CloudScenes.Length)
			{
				return;
			}
			CloudType[] cloudTypes = CloudScenes[cloudSceneId].CloudTypes;
			foreach (CloudType cloudType in cloudTypes)
			{
				if (cloudType.Enable)
				{
					EmitCloudType(cloudType, deltaTime);
				}
			}
			CompoundCloudType[] compoundCloudTypes = CloudScenes[cloudSceneId].CompoundCloudTypes;
			foreach (CompoundCloudType compoundCloudType in compoundCloudTypes)
			{
				if (compoundCloudType.Enable)
				{
					EmitCompoundCloudType(compoundCloudType, deltaTime);
				}
			}
			LayerCloudType[] layerCloudTypes = CloudScenes[cloudSceneId].LayerCloudTypes;
			foreach (LayerCloudType layerCloudType in layerCloudTypes)
			{
				if (layerCloudType.Enable)
				{
					EmitLayerCloudType(layerCloudType, deltaTime);
				}
			}
		}

		private void Emit(float deltaTime)
		{
			EmitCloudScene(CloudSceneId, deltaTime);
		}

		private void SimulateInStart()
		{
			float num = 1f / 30f;
			int num2 = Mathf.FloorToInt(_particleSystem.startLifetime / num);
			_particleSystem.playbackSpeed = 1f;
			for (int i = 0; i < num2; i++)
			{
				GetParticle();
				if (ShipCollider != null && _camera != null)
				{
					HandleCollisionForGoodSight(num);
				}
				SetParticle();
				Emit(num);
				_particleSystem.Simulate(num, false, false);
			}
			_particleSystem.playbackSpeed = PlaybackSpeed;
			_particleSystem.Play();
		}

		private void RestoreLastState()
		{
			if (_needRestoreLastState)
			{
				_needRestoreLastState = false;
				_particleSystem.Clear();
				_particleSystem.Emit(_particleCount);
				SetParticle();
			}
		}

		private void EmitLightningType(LightningType lightningType, float deltaTime)
		{
			int emitCount = lightningType.GetEmitCount(deltaTime);
			Bounds emitArea = lightningType.EmitArea;
			Vector3 min = emitArea.min;
			Vector3 max = emitArea.max;
			for (int i = 0; i < 3; i++)
			{
				int index2;
				int index = (index2 = i);
				float num = min[index2];
				min[index] = num * EmitterSize[i];
			}
			for (int j = 0; j < 3; j++)
			{
				int index2;
				int index3 = (index2 = j);
				float num = max[index2];
				max[index3] = num * EmitterSize[j];
			}
			emitArea.min = min;
			emitArea.max = max;
			while (emitCount-- != 0)
			{
				int num2 = -1;
				int num3 = 100;
				while (num2 == -1 && num3-- != 0)
				{
					int num4 = Random.Range(0, _particleCount);
					if (_collidedParticleMap.ContainsKey((int)_particles[num4].startSize) || !emitArea.Contains(_particles[num4].position))
					{
						continue;
					}
					num2 = num4;
					break;
				}
				if (num2 == -1)
				{
					break;
				}
				float num5 = ParticleSize(ref _particles[num2]) * AspectRatio;
				Vector3 position = _particles[num2].position;
				position.y -= num5 * (lightningType.StartLocation - 0.5f);
				lightningType.Emit(position, _particles[num2].velocity);
			}
		}

		private void UpdateLightningType(LightningType lightningType, float deltaTime)
		{
			float num = deltaTime;
			if (PlaybackSpeed > 0.1f)
			{
				num /= PlaybackSpeed;
			}
			lightningType.Update(deltaTime, num, _material);
		}

		private void UpdateLightning(float deltaTime)
		{
			LightningType.PreUpdate(_material);
			LightningType[] lightningTypes = CloudScenes[CloudSceneId].LightningTypes;
			foreach (LightningType lightningType in lightningTypes)
			{
				if (lightningType.Enable)
				{
					EmitLightningType(lightningType, deltaTime);
					UpdateLightningType(lightningType, deltaTime);
				}
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

		public Vector3 LocalToCamera(Vector3 point)
		{
			Vector3 v = base.transform.TransformPoint(point);
			return _camera.worldToCameraMatrix.MultiplyPoint(v);
		}

		public Vector3 CameraToLocal(Vector3 point)
		{
			Vector3 v = _camera.cameraToWorldMatrix.MultiplyPoint(point);
			return base.transform.worldToLocalMatrix.MultiplyPoint(v);
		}

		private void HandleCollision(float deltaTime)
		{
			for (int i = 0; i < _particleCount; i++)
			{
				if (_particles[i].position.z < _shipColliderZ - ShipColliderSkin)
				{
					continue;
				}
				CollidedParticle collidedParticle = null;
				if (_collidedParticleMap.ContainsKey((int)_particles[i].startSize))
				{
					collidedParticle = _collidedParticleMap[(int)_particles[i].startSize];
				}
				else
				{
					Rect other = default(Rect);
					float num = ParticleSize(ref _particles[i]);
					other.size = new Vector2(num, num * AspectRatio);
					other.center = _particles[i].position;
					if (_shipColliderRect.Overlaps(other))
					{
						collidedParticle = new CollidedParticle();
						collidedParticle.RealPosition = _particles[i].position;
						collidedParticle.RealVelocity = _particles[i].velocity;
						_collidedParticleMap.Add((int)_particles[i].startSize, collidedParticle);
						_particles[i].velocity = Vector3.zero;
						_particles[i].remainingLifetime = 1f;
					}
				}
				if (collidedParticle != null)
				{
					collidedParticle.RealPosition += collidedParticle.RealVelocity * deltaTime;
					Vector3 point = LocalToCamera(collidedParticle.RealPosition);
					if (point.z > -0.1f)
					{
						_particles[i].remainingLifetime = 0f;
						_collidedParticleMap.Remove((int)_particles[i].startSize);
						continue;
					}
					float num2 = LocalToCamera(_particles[i].position).z / point.z;
					point.x *= num2;
					point.y *= num2;
					Vector3 position = CameraToLocal(point);
					position.z = _particles[i].position.z;
					_particles[i].position = position;
					Color color = _particles[i].startColor;
					color.b = Mathf.Max(1f / num2, 0.003921569f);
					color.a = 1f / num2;
					_particles[i].startColor = color;
					_particles[i].remainingLifetime = 1f;
				}
			}
		}

		private void HandleCollisionForGoodSight(float deltaTime)
		{
			for (int i = 0; i < _particleCount; i++)
			{
				if (_particles[i].position.z > _shipColliderZ - ShipFrontDistanceForSight)
				{
					Rect other = default(Rect);
					float num = ParticleSize(ref _particles[i]);
					other.size = new Vector2(num, num * AspectRatio);
					other.size *= 0.8f;
					other.center = _particles[i].position;
					Rect shipColliderRect = _shipColliderRect;
					shipColliderRect.size *= _particles[i].position.z / _shipColliderZ;
					if (_shipColliderRect.Overlaps(other))
					{
						_particles[i].remainingLifetime = 0f;
					}
				}
			}
		}
	}
}
