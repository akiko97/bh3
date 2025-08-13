using System;
using UnityEngine;

namespace MoleMole.MainMenu
{
	[Serializable]
	public class LightningType
	{
		[Serializable]
		public class LightningMaterial
		{
			public Vector3 pivot;

			public float texOffsetX;
		}

		public string Name;

		public bool Enable = true;

		[Range(0f, 1f)]
		public float EmitRate;

		[Range(0f, 100f)]
		public float EmitRateDeviation;

		public Bounds EmitArea;

		[Tooltip("Relative y location of a cloud from which a lightning start")]
		[Range(0f, 1f)]
		public float StartLocation;

		[Range(0f, 1f)]
		public float StartLifttime;

		[Range(0f, 1f)]
		public float StartLifttimeDeviation;

		[Range(0f, 1f)]
		public float VisibleLightningRatio;

		public float Size;

		[Range(0f, 10f)]
		public float MinIntensity = 1f;

		[Range(0f, 10f)]
		public float MaxIntensity = 1f;

		public AnimationCurve IntensityCurve;

		[Range(0f, 100f)]
		public float CloudLitIntensinty = 1f;

		[Range(0f, 100f)]
		[Tooltip("Without lightning")]
		public float CloudLitIntensinty2 = 1f;

		public GameObject Prefab;

		public LightningMaterial[] Materials;

		private Lightning[] _buffer = new Lightning[10];

		private float _RemainEmitCount;

		public static readonly int MaxFlashCount = 4;

		public static readonly int MaxFlashPoint = 6;

		public static int FlashPointId;

		public MaterialPropertyBlock DumbMPB;

		public void Init(CloudEmitter cloudEmitter)
		{
			DumbMPB = new MaterialPropertyBlock();
			DumbMPB.SetFloat("_TexOffsetX", 0f);
			DumbMPB.SetFloat("_TexScaleX", 0.25f);
			DumbMPB.SetFloat("_EmissionScaler", 1f);
			for (int i = 0; i < _buffer.Length; i++)
			{
				_buffer[i] = new Lightning();
				_buffer[i].Init(Prefab, cloudEmitter.transform, VisibleLightningRatio > 0.01f);
			}
		}

		public int GetEmitCount(float deltaTime)
		{
			_RemainEmitCount += EmitRate * (1f + (UnityEngine.Random.value * 2f - 1f) * EmitRateDeviation) * deltaTime;
			int num = Mathf.FloorToInt(_RemainEmitCount);
			if (num < 1)
			{
				return 0;
			}
			_RemainEmitCount -= num;
			return num;
		}

		public void Emit(Vector3 position, Vector3 velocity)
		{
			Lightning lightning = null;
			Lightning[] buffer = _buffer;
			foreach (Lightning lightning2 in buffer)
			{
				if (lightning2 == null)
				{
					if (Application.isPlaying)
					{
						Debug.LogError("Missing lightning object");
						return;
					}
					CloudEmitter cloudEmitter = UnityEngine.Object.FindObjectOfType<CloudEmitter>();
					cloudEmitter.Reset();
					return;
				}
				if (!lightning2.Active)
				{
					lightning = lightning2;
					break;
				}
			}
			if (lightning != null)
			{
				bool lightningVisible = UnityEngine.Random.value < VisibleLightningRatio;
				lightning.Emit(position, velocity, lightningVisible, this);
			}
		}

		public static void PreUpdate(Material cloudMaterial)
		{
			FlashPointId = 0;
			for (int i = 0; i < MaxFlashPoint; i++)
			{
				cloudMaterial.SetVector("_FlashPoint0" + i, Vector4.zero);
			}
		}

		public void Update(float deltaTime, float realDeltaTime, Material cloudMaterial)
		{
			Lightning[] buffer = _buffer;
			foreach (Lightning lightning in buffer)
			{
				if (lightning == null)
				{
					if (!Application.isPlaying)
					{
						CloudEmitter cloudEmitter = UnityEngine.Object.FindObjectOfType<CloudEmitter>();
						cloudEmitter.Reset();
						break;
					}
					Debug.LogError("Missing lightning object");
				}
				if (lightning.Active)
				{
					lightning.Update(deltaTime, realDeltaTime, this, ref FlashPointId, cloudMaterial);
				}
			}
		}
	}
}
