using System.Collections.Generic;
using UnityEngine;

namespace MoleMole.MainMenu
{
	public class Lightning
	{
		public bool Active;

		public bool LightningVisible;

		public Vector3 StartPosition = Vector3.zero;

		public Vector3 EndPosition = Vector3.zero;

		public Vector3 Velocity = Vector3.zero;

		public int FlashCount;

		private Transform _parentTransform;

		private LightningObject[] _objects = new LightningObject[LightningType.MaxFlashCount];

		public void Init(GameObject prefab, Transform parent, bool needLightning)
		{
			_parentTransform = parent;
			for (int i = 0; i < _objects.Length; i++)
			{
				LightningObject lightningObject = new LightningObject();
				_objects[i] = lightningObject;
				if (needLightning)
				{
					lightningObject.Object = Object.Instantiate(prefab);
					lightningObject.Object.hideFlags = HideFlags.DontSave;
					Transform transform = lightningObject.Object.transform;
					transform.parent = _parentTransform;
					transform.localPosition = Vector3.zero;
					lightningObject.OrigScale = transform.localScale;
				}
				lightningObject.Active = false;
			}
		}

		public void Emit(Vector3 position, Vector3 velocity, bool lightningVisible, LightningType lightningType)
		{
			Active = true;
			LightningVisible = lightningVisible;
			StartPosition = position;
			EndPosition = StartPosition - Vector3.up * lightningType.Size;
			Velocity = velocity;
			FlashCount = Random.Range(1, LightningType.MaxFlashCount);
			float num = 0f;
			HashSet<int> hashSet = new HashSet<int>();
			int num2 = lightningType.Materials.Length;
			for (int i = 0; i < FlashCount; i++)
			{
				LightningObject lightningObject = _objects[i];
				if (lightningVisible)
				{
					if (hashSet.Count >= num2)
					{
						break;
					}
					int num3;
					do
					{
						num3 = Random.Range(0, num2);
					}
					while (hashSet.Contains(num3));
					hashSet.Add(num3);
					lightningObject.Mat = lightningType.Materials[num3];
					lightningObject.UpdatePosition(position);
				}
				lightningObject.Intensity = Random.Range(lightningType.MinIntensity, lightningType.MaxIntensity);
				lightningObject.Size = lightningType.Size;
				float num4 = lightningType.StartLifttime * (1f + (Random.value * 2f - 1f) * lightningType.StartLifttimeDeviation);
				lightningObject.StartLifttime = num4 + num;
				lightningObject.DelayTime = num;
				num += num4;
				lightningObject.Lifttime = 0f;
				lightningObject.Active = true;
			}
		}

		public void Update(float deltaTime, float realDeltaTime, LightningType lightningType, ref int flashPointId, Material cloudMaterial)
		{
			bool active = false;
			float num = 0f;
			for (int i = 0; i < FlashCount; i++)
			{
				LightningObject lightningObject = _objects[i];
				if (lightningObject.Active)
				{
					active = true;
					lightningObject.Lifttime += realDeltaTime;
					if (LightningVisible && lightningObject.Lifttime > lightningObject.DelayTime)
					{
						lightningObject.Show(true);
					}
					if (lightningObject.Lifttime > lightningObject.StartLifttime)
					{
						lightningObject.Active = false;
					}
					float num2 = lightningType.IntensityCurve.Evaluate((lightningObject.Lifttime - lightningObject.DelayTime) / (lightningObject.StartLifttime - lightningObject.DelayTime)) * lightningObject.Intensity;
					num += num2;
					if (LightningVisible)
					{
						lightningObject.Object.transform.localPosition += Velocity * deltaTime;
						lightningType.DumbMPB.SetFloat("_TexOffsetX", lightningObject.Mat.texOffsetX);
						lightningType.DumbMPB.SetFloat("_EmissionScaler", num2);
						Renderer component = lightningObject.Object.GetComponent<Renderer>();
						component.SetPropertyBlock(lightningType.DumbMPB);
					}
				}
			}
			Active = active;
			if (Active)
			{
				float intensity = num * ((!LightningVisible) ? lightningType.CloudLitIntensinty2 : lightningType.CloudLitIntensinty);
				SetFlashPoint(StartPosition, intensity, cloudMaterial, ref flashPointId);
				if (LightningVisible)
				{
					SetFlashPoint(EndPosition, intensity, cloudMaterial, ref flashPointId);
				}
			}
		}

		private void SetFlashPoint(Vector3 position, float intensity, Material cloudMaterial, ref int flashPointId)
		{
			if (flashPointId < LightningType.MaxFlashPoint)
			{
				Vector3 vector = _parentTransform.TransformPoint(position);
				cloudMaterial.SetVector("_FlashPoint0" + flashPointId, new Vector4(vector.x, vector.y, vector.z, intensity));
				flashPointId++;
			}
		}
	}
}
