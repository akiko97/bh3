using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Camera))]
	public class DrawShadow : MonoBehaviour
	{
		public enum BlurLevel
		{
			Lv1 = 1,
			Lv2 = 2,
			Lv3 = 3,
			Lv4 = 4
		}

		[HideInInspector]
		public Shader MultipleGaussPassFilterShader;

		private Material MultipleGaussPassFilter;

		[HideInInspector]
		public Shader MultipleGaussPassFilterShaderHQ;

		private Material MultipleGaussPassFilterHQ;

		private static readonly float LowQualityBlurShaderBaseImageSize = 64f;

		private static readonly float HighQualityBlurShaderBaseImageSize = 512f;

		[HideInInspector]
		public Shader DrawShadowShader;

		public int CullingLayer = 256;

		public int ReplacementLayer = 23;

		public bool useBlur = true;

		public bool HighQuality;

		[Header("Only work in high quality mode")]
		public BlurLevel blurLevel = BlurLevel.Lv4;

		private RenderTextureWrapper temp_buffer;

		private RenderTextureWrapper[] temp_buffers;

		private float aspectRatio;

		private RenderTextureFormat internalBufferFormat;

		private RenderTextureFormat internalBufferFormatHQ;

		protected bool isSupported = true;

		protected List<Renderer> _renderers = new List<Renderer>();

		protected List<int> _oldLayers = new List<int>();

		public void Awake()
		{
			Renderer[] componentsInChildren = base.transform.root.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				int layer = componentsInChildren[i].gameObject.layer;
				if (((1 << layer) & CullingLayer) != 0)
				{
					_renderers.Add(componentsInChildren[i]);
					_oldLayers.Add(layer);
				}
			}
		}

		private void OnPreCull()
		{
			for (int i = 0; i < _renderers.Count; i++)
			{
				_renderers[i].gameObject.layer = ReplacementLayer;
			}
		}

		private void OnPostRender()
		{
			for (int i = 0; i < _renderers.Count; i++)
			{
				_renderers[i].gameObject.layer = _oldLayers[i];
			}
		}

		private Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
		{
			if (!s)
			{
				base.enabled = false;
				return null;
			}
			if (s.isSupported && (bool)m2Create && m2Create.shader == s)
			{
				return m2Create;
			}
			if (!s.isSupported)
			{
				NotSupported();
				return null;
			}
			m2Create = new Material(s);
			m2Create.hideFlags = HideFlags.DontSave;
			if ((bool)m2Create)
			{
				return m2Create;
			}
			return null;
		}

		private Material CreateMaterial(Shader s, Material m2Create)
		{
			if (!s)
			{
				return null;
			}
			if ((bool)m2Create && m2Create.shader == s && s.isSupported)
			{
				return m2Create;
			}
			if (!s.isSupported)
			{
				return null;
			}
			m2Create = new Material(s);
			m2Create.hideFlags = HideFlags.DontSave;
			if ((bool)m2Create)
			{
				return m2Create;
			}
			return null;
		}

		private void ReportAutoDisable()
		{
			Debug.LogWarning(string.Concat("The image effect ", this, " has been disabled as it's not supported on the current platform."));
		}

		private bool CheckShader(Shader s)
		{
			if (!s.isSupported)
			{
				NotSupported();
				return false;
			}
			return false;
		}

		private void NotSupported()
		{
			base.enabled = false;
			isSupported = false;
		}

		private bool CheckResources()
		{
			MultipleGaussPassFilter = CheckShaderAndCreateMaterial(MultipleGaussPassFilterShader, MultipleGaussPassFilter);
			if (HighQuality)
			{
				MultipleGaussPassFilterHQ = CheckShaderAndCreateMaterial(MultipleGaussPassFilterShaderHQ, MultipleGaussPassFilterHQ);
			}
			if (!isSupported)
			{
				ReportAutoDisable();
			}
			return isSupported;
		}

		private void OnEnable()
		{
			isSupported = true;
		}

		private void CreateRenderTextures(int w, int h)
		{
			if (!HighQuality)
			{
				temp_buffer = GraphicsUtils.GetRenderTexture(w, h, 0, internalBufferFormat);
				return;
			}
			temp_buffer = GraphicsUtils.GetRenderTexture(w, h, 0, internalBufferFormatHQ);
			temp_buffers = new RenderTextureWrapper[(int)blurLevel];
			for (int i = 0; i < (int)blurLevel; i++)
			{
				temp_buffers[i] = GraphicsUtils.GetRenderTexture(w, h, 0, internalBufferFormatHQ);
			}
		}

		private void ReleaseRenderTextures()
		{
			if (temp_buffer != null)
			{
				GraphicsUtils.ReleaseRenderTexture(temp_buffer);
				temp_buffer = null;
			}
			if (!HighQuality)
			{
				return;
			}
			for (int i = 0; i < (int)blurLevel; i++)
			{
				if (temp_buffers[i] != null)
				{
					GraphicsUtils.ReleaseRenderTexture(temp_buffers[i]);
					temp_buffers[i] = null;
				}
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (destination == null)
			{
				return;
			}
			if (!CheckResources())
			{
				Graphics.Blit(source, destination);
				return;
			}
			aspectRatio = source.width;
			aspectRatio /= source.height;
			CreateRenderTextures(source.width, source.height);
			if (useBlur)
			{
				DoBlur(source, destination);
			}
			else
			{
				Graphics.Blit(source, destination);
			}
			ReleaseRenderTextures();
		}

		private void DoBlur(RenderTexture source, RenderTexture destination)
		{
			if (!HighQuality)
			{
				float offsetScale = LowQualityBlurShaderBaseImageSize / (float)source.width;
				blur(source, destination, temp_buffer, MultipleGaussPassFilter, 0, offsetScale);
				return;
			}
			float num = HighQualityBlurShaderBaseImageSize / (float)source.width;
			blur(source, temp_buffers[0], temp_buffer, MultipleGaussPassFilterHQ, 0, num);
			for (int i = 1; i < (int)blurLevel; i++)
			{
				num /= 2f;
				blur(temp_buffers[i - 1], temp_buffers[i], temp_buffer, MultipleGaussPassFilterHQ, i, num);
			}
			Graphics.Blit((RenderTexture)temp_buffers[(int)(blurLevel - 1)], destination);
		}

		private void blur(RenderTexture source, RenderTexture destination, RenderTexture temp, Material material, int level, float offsetScale = 1f)
		{
			material.SetVector("_scaler", new Vector2((!(1f / aspectRatio > 1f)) ? (1f / aspectRatio) : 1f, 0f) * offsetScale);
			Graphics.Blit(source, temp, material, level);
			material.SetVector("_scaler", new Vector2(0f, (!(aspectRatio > 1f)) ? aspectRatio : 1f) * offsetScale);
			destination.DiscardContents();
			Graphics.Blit(temp, destination, material, level);
		}
	}
}
