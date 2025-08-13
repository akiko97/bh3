using System;
using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginOverrideHandler : BaseMonoEffectPlugin
	{
		[Serializable]
		public class RendererMaterialOverride
		{
			public string materialOverrideKey;

			public Renderer renderer;

			[NonSerialized]
			public Material originalMaterial;

			public static RendererMaterialOverride[] EMPTY = new RendererMaterialOverride[0];
		}

		[Serializable]
		public class RendererColorOverride
		{
			public string colorOverrideKey;

			public string colorPropertyKey = "_TintColor";

			public Renderer renderer;

			[NonSerialized]
			public Color originalColor;

			public static RendererColorOverride[] EMPTY = new RendererColorOverride[0];
		}

		[Serializable]
		public class RendererFloatOverride
		{
			public string floatOverrideKey;

			public string floatPropertyKey;

			public Renderer renderer;

			[NonSerialized]
			public float originalValue;

			public static RendererFloatOverride[] EMPTY = new RendererFloatOverride[0];
		}

		[Serializable]
		public class ParticleSystemStartColorOverride
		{
			public enum StartColorType
			{
				ConstColor1 = 0,
				RandomBetweenColor12 = 1
			}

			public string colorOverrideKey1;

			public string colorOverrideKey2;

			public ParticleSystem particleSystem;

			public StartColorType type;

			[NonSerialized]
			public Color originalStartColor;

			public static ParticleSystemStartColorOverride[] EMPTY = new ParticleSystemStartColorOverride[0];
		}

		private static MaterialPropertyBlock _emptyBlock = new MaterialPropertyBlock();

		[HideInInspector]
		public RendererMaterialOverride[] rendererOverrides = RendererMaterialOverride.EMPTY;

		[HideInInspector]
		public string[] effectOverlays = Miscs.EMPTY_STRINGS;

		[HideInInspector]
		public RendererColorOverride[] rendererColorOverrides = RendererColorOverride.EMPTY;

		[HideInInspector]
		public RendererFloatOverride[] rendererFloatOverrides = RendererFloatOverride.EMPTY;

		[HideInInspector]
		public ParticleSystemStartColorOverride[] particleSystemStartColorOverrides = ParticleSystemStartColorOverride.EMPTY;

		public override void Setup()
		{
			base.Setup();
			_emptyBlock.Clear();
			for (int i = 0; i < rendererColorOverrides.Length; i++)
			{
				RendererColorOverride rendererColorOverride = rendererColorOverrides[i];
				if (rendererColorOverride == null)
				{
					continue;
				}
				if (rendererColorOverride.renderer is ParticleSystemRenderer)
				{
					if (GraphicsUtils.IsInstancedMaterial(rendererColorOverride.renderer.sharedMaterial))
					{
						rendererColorOverride.renderer.sharedMaterial.SetColor(rendererColorOverride.colorPropertyKey, rendererColorOverride.originalColor);
					}
				}
				else
				{
					rendererColorOverride.renderer.SetPropertyBlock(_emptyBlock);
				}
			}
			for (int j = 0; j < rendererFloatOverrides.Length; j++)
			{
				RendererFloatOverride rendererFloatOverride = rendererFloatOverrides[j];
				if (rendererFloatOverride == null)
				{
					continue;
				}
				if (rendererFloatOverride.renderer is ParticleSystemRenderer)
				{
					if (GraphicsUtils.IsInstancedMaterial(rendererFloatOverride.renderer.sharedMaterial))
					{
						rendererFloatOverride.renderer.sharedMaterial.SetFloat(rendererFloatOverride.floatPropertyKey, rendererFloatOverride.originalValue);
					}
				}
				else
				{
					rendererFloatOverride.renderer.SetPropertyBlock(_emptyBlock);
				}
			}
			for (int k = 0; k < particleSystemStartColorOverrides.Length; k++)
			{
				ParticleSystemStartColorOverride particleSystemStartColorOverride = particleSystemStartColorOverrides[k];
				particleSystemStartColorOverride.particleSystem.startColor = particleSystemStartColorOverride.originalStartColor;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			for (int i = 0; i < rendererOverrides.Length; i++)
			{
				RendererMaterialOverride rendererMaterialOverride = rendererOverrides[i];
				rendererMaterialOverride.originalMaterial = rendererMaterialOverride.renderer.sharedMaterial;
			}
			for (int j = 0; j < rendererColorOverrides.Length; j++)
			{
				RendererColorOverride rendererColorOverride = rendererColorOverrides[j];
				rendererColorOverride.originalColor = rendererColorOverride.renderer.sharedMaterial.GetColor(rendererColorOverride.colorPropertyKey);
			}
			for (int k = 0; k < rendererFloatOverrides.Length; k++)
			{
				RendererFloatOverride rendererFloatOverride = rendererFloatOverrides[k];
				rendererFloatOverride.originalValue = rendererFloatOverride.renderer.sharedMaterial.GetFloat(rendererFloatOverride.floatPropertyKey);
			}
			for (int l = 0; l < particleSystemStartColorOverrides.Length; l++)
			{
				ParticleSystemStartColorOverride particleSystemStartColorOverride = particleSystemStartColorOverrides[l];
				particleSystemStartColorOverride.originalStartColor = particleSystemStartColorOverride.particleSystem.startColor;
			}
		}

		public void HandleEffectOverride(MonoEffectOverride effectOverride)
		{
			for (int i = 0; i < rendererOverrides.Length; i++)
			{
				RendererMaterialOverride rendererMaterialOverride = rendererOverrides[i];
				Material value;
				if (effectOverride.materialOverrides.TryGetValue(rendererMaterialOverride.materialOverrideKey, out value))
				{
					if (rendererMaterialOverride.renderer is ParticleSystemRenderer)
					{
						GraphicsUtils.CreateAndAssignInstancedMaterial(rendererMaterialOverride.renderer, value);
					}
					else
					{
						rendererMaterialOverride.renderer.sharedMaterial = value;
					}
				}
				else if (rendererMaterialOverride.renderer is ParticleSystemRenderer)
				{
					if (GraphicsUtils.IsInstancedMaterial(rendererMaterialOverride.renderer.sharedMaterial))
					{
						GraphicsUtils.CreateAndAssignInstancedMaterial(rendererMaterialOverride.renderer, rendererMaterialOverride.originalMaterial);
					}
					else
					{
						rendererMaterialOverride.renderer.sharedMaterial = rendererMaterialOverride.originalMaterial;
					}
				}
				else
				{
					rendererMaterialOverride.renderer.sharedMaterial = rendererMaterialOverride.originalMaterial;
				}
			}
			for (int j = 0; j < effectOverlays.Length; j++)
			{
				string value2;
				if (effectOverride.effectOverlays.TryGetValue(effectOverlays[j], out value2))
				{
					Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(value2, base.transform.position, base.transform.forward, base.transform.localScale, _effect.owner);
				}
			}
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			for (int k = 0; k < rendererColorOverrides.Length; k++)
			{
				RendererColorOverride rendererColorOverride = rendererColorOverrides[k];
				Color value3;
				if (!effectOverride.colorOverrides.TryGetValue(rendererColorOverride.colorOverrideKey, out value3))
				{
					continue;
				}
				if (rendererColorOverride.renderer is ParticleSystemRenderer)
				{
					if (!GraphicsUtils.IsInstancedMaterial(rendererColorOverride.renderer.sharedMaterial))
					{
						GraphicsUtils.CreateAndAssignInstancedMaterial(rendererColorOverride.renderer, rendererColorOverride.renderer.sharedMaterial);
					}
					rendererColorOverride.renderer.sharedMaterial.SetColor(rendererColorOverride.colorPropertyKey, value3);
				}
				else
				{
					materialPropertyBlock.Clear();
					rendererColorOverride.renderer.GetPropertyBlock(materialPropertyBlock);
					materialPropertyBlock.SetColor(rendererColorOverride.colorPropertyKey, effectOverride.colorOverrides[rendererColorOverride.colorOverrideKey]);
					rendererColorOverride.renderer.SetPropertyBlock(materialPropertyBlock);
				}
			}
			for (int l = 0; l < rendererFloatOverrides.Length; l++)
			{
				RendererFloatOverride rendererFloatOverride = rendererFloatOverrides[l];
				float value4;
				if (!effectOverride.floatOverrides.TryGetValue(rendererFloatOverride.floatOverrideKey, out value4))
				{
					continue;
				}
				if (rendererFloatOverride.renderer is ParticleSystemRenderer)
				{
					if (!GraphicsUtils.IsInstancedMaterial(rendererFloatOverride.renderer.sharedMaterial))
					{
						GraphicsUtils.CreateAndAssignInstancedMaterial(rendererFloatOverride.renderer, rendererFloatOverride.renderer.sharedMaterial);
					}
					rendererFloatOverride.renderer.sharedMaterial.SetFloat(rendererFloatOverride.floatPropertyKey, value4);
				}
				else
				{
					materialPropertyBlock.Clear();
					rendererFloatOverride.renderer.GetPropertyBlock(materialPropertyBlock);
					materialPropertyBlock.SetFloat(rendererFloatOverride.floatPropertyKey, effectOverride.floatOverrides[rendererFloatOverride.floatOverrideKey]);
					rendererFloatOverride.renderer.SetPropertyBlock(materialPropertyBlock);
				}
			}
			for (int m = 0; m < particleSystemStartColorOverrides.Length; m++)
			{
				ParticleSystemStartColorOverride particleSystemStartColorOverride = particleSystemStartColorOverrides[m];
				Color value6;
				Color value7;
				if (particleSystemStartColorOverride.type == ParticleSystemStartColorOverride.StartColorType.ConstColor1)
				{
					Color value5;
					if (effectOverride.colorOverrides.TryGetValue(particleSystemStartColorOverride.colorOverrideKey1, out value5))
					{
						particleSystemStartColorOverride.particleSystem.startColor = value5;
					}
				}
				else if (particleSystemStartColorOverride.type == ParticleSystemStartColorOverride.StartColorType.RandomBetweenColor12 && effectOverride.colorOverrides.TryGetValue(particleSystemStartColorOverride.colorOverrideKey1, out value6) && effectOverride.colorOverrides.TryGetValue(particleSystemStartColorOverride.colorOverrideKey2, out value7))
				{
					particleSystemStartColorOverride.particleSystem.startColor = Color.Lerp(value6, value7, UnityEngine.Random.value);
				}
			}
		}

		public override bool IsToBeRemove()
		{
			return false;
		}

		public override void SetDestroy()
		{
		}

		private void OnDestroy()
		{
			for (int i = 0; i < rendererOverrides.Length; i++)
			{
				RendererMaterialOverride rendererMaterialOverride = rendererOverrides[i];
				GraphicsUtils.TryCleanRendererInstancedMaterial(rendererMaterialOverride.renderer);
			}
			for (int j = 0; j < rendererColorOverrides.Length; j++)
			{
				RendererColorOverride rendererColorOverride = rendererColorOverrides[j];
				if (rendererColorOverride != null)
				{
					GraphicsUtils.TryCleanRendererInstancedMaterial(rendererColorOverride.renderer);
				}
			}
			for (int k = 0; k < rendererFloatOverrides.Length; k++)
			{
				RendererFloatOverride rendererFloatOverride = rendererFloatOverrides[k];
				if (rendererFloatOverride != null)
				{
					GraphicsUtils.TryCleanRendererInstancedMaterial(rendererFloatOverride.renderer);
				}
			}
		}
	}
}
