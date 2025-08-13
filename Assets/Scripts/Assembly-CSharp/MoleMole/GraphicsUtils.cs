using UnityEngine;
using UnityEngine.Rendering;

namespace MoleMole
{
	public static class GraphicsUtils
	{
		public static bool isDisableRenderTexture = false;

		private static RenderTextureWrapperPool _renderTextureWrapperPool = new RenderTextureWrapperPool();

		public static void CreateAndAssignInstancedMaterial(Renderer renderer, Material targetMaterial)
		{
			if (IsInstancedMaterial(renderer.sharedMaterial))
			{
				renderer.sharedMaterial.CopyPropertiesFromMaterial(targetMaterial);
				return;
			}
			Material material = new Material(targetMaterial);
			material.name = targetMaterial.name + "#copy#";
			material.shaderKeywords = targetMaterial.shaderKeywords;
			renderer.sharedMaterial = material;
		}

		public static bool IsInstancedMaterial(Material mat)
		{
			return mat != null && mat.name.EndsWith("#copy#");
		}

		public static void TryCleanRendererInstancedMaterial(Renderer renderer)
		{
			if (!(renderer == null) && !(renderer.sharedMaterial == null) && IsInstancedMaterial(renderer.sharedMaterial))
			{
				Object.Destroy(renderer.sharedMaterial);
			}
		}

		public static void WarmupAllShaders()
		{
			Shader.WarmupAllShaders();
		}

		public static void SetShaderBloomMaxBlendParams()
		{
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2)
			{
				Shader.SetGlobalInt("_GlobalBloomMaxBlendSrc", 1);
				Shader.SetGlobalInt("_GlobalBloomMaxBlendDst", 0);
				Shader.SetGlobalInt("_GlobalBloomMaxBlendOp", 0);
			}
			else
			{
				Shader.SetGlobalInt("_GlobalBloomMaxBlendSrc", 1);
				Shader.SetGlobalInt("_GlobalBloomMaxBlendDst", 1);
				Shader.SetGlobalInt("_GlobalBloomMaxBlendOp", 4);
			}
		}

		public static int GetRenderTextureNumber()
		{
			return _renderTextureWrapperPool.GetUsedCount();
		}

		public static RenderTextureWrapper GetRenderTexture(RenderTextureWrapper.Param param)
		{
			RenderTextureWrapper item = _renderTextureWrapperPool.GetItem();
			item.Create(param);
			return item;
		}

		public static RenderTextureWrapper GetRenderTexture(int width, int height, int depth)
		{
			return GetRenderTexture(new RenderTextureWrapper.Param
			{
				width = width,
				height = height,
				depth = depth,
				format = RenderTextureFormat.ARGB32,
				readWrite = RenderTextureReadWrite.Default
			});
		}

		public static RenderTextureWrapper GetRenderTexture(int width, int height, int depth, RenderTextureFormat format)
		{
			return GetRenderTexture(new RenderTextureWrapper.Param
			{
				width = width,
				height = height,
				depth = depth,
				format = format,
				readWrite = RenderTextureReadWrite.Default
			});
		}

		public static RenderTextureWrapper GetRenderTexture(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			return GetRenderTexture(new RenderTextureWrapper.Param
			{
				width = width,
				height = height,
				depth = depth,
				format = format,
				readWrite = readWrite
			});
		}

		public static void ReleaseRenderTexture(RenderTextureWrapper wrapper)
		{
			if (wrapper != null)
			{
				_renderTextureWrapperPool.ReleaseItem(wrapper);
			}
		}

		public static void RebindAllRenderTexturesToCamera()
		{
			for (int i = 0; i < _renderTextureWrapperPool.GetUsedCount(); i++)
			{
				if (!_renderTextureWrapperPool.usedList[i].IsCreated())
				{
					_renderTextureWrapperPool.usedList[i].RebindToCamera();
				}
			}
		}
	}
}
