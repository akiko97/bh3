using System;
using UnityEngine;

namespace MoleMole
{
	[DisallowMultipleComponent]
	public class ReflectionBase : MonoBehaviour
	{
		public enum TexResScale
		{
			RES_25 = 25,
			RES_33 = 33,
			RES_40 = 40,
			RES_50 = 50,
			RES_75 = 75,
			RES_100 = 100
		}

		[Serializable]
		public class ReflectRenderer
		{
			public Renderer renderer;

			private MaterialPropertyBlock block;

			public void Init()
			{
				block = new MaterialPropertyBlock();
			}

			public void SetMaterialBlock(RenderTexture reflectionTex)
			{
				block.SetTexture("_ReflectionTex", reflectionTex);
				renderer.SetPropertyBlock(block);
			}
		}

		public TexResScale texResScale = TexResScale.RES_33;

		public RenderTextureFormat bufferFormat = RenderTextureFormat.ARGBHalf;

		public LayerMask layers = -33;

		[Range(-10f, 10f)]
		public float reflectClipPlaneOffset;

		public ReflectRenderer[] reflectRendererList;

		public bool FastMode;

		protected string[] reflectShaderNames = new string[5] { "miHoYo/Scene/Metal Wall", "miHoYo/Scene/Metal Floor", "miHoYo/Scene/Metal Floor (With Pow)", "miHoYo/Scene/Water", "miHoYo/Scene/Water Shore" };

		public Shader hexBlurShader;

		private Material _hexBlurMat;

		public bool useBlur;

		public float hexBlurFactor;

		protected static Camera _reflectionCamera;

		protected TexResScale _oldTexResScale;

		protected RenderTextureWrapper _reflectionRenderTex;

		protected bool _insideRendering;

		protected Vector3 _reflectionPlanePosition;

		private bool _oldInvertCulling;

		protected virtual void Awake()
		{
			_insideRendering = false;
			_reflectionPlanePosition = base.transform.position;
			if (reflectRendererList.Length == 0)
			{
				Renderer component = GetComponent<Renderer>();
				if (component != null)
				{
					reflectRendererList = new ReflectRenderer[1];
					reflectRendererList[0] = new ReflectRenderer();
					reflectRendererList[0].renderer = component;
				}
			}
			SetFastMode(!GlobalVars.USE_REFLECTION);
			ReflectRenderer[] array = reflectRendererList;
			foreach (ReflectRenderer reflectRenderer in array)
			{
				reflectRenderer.Init();
			}
		}

		public void SetFastMode(bool isFastMode)
		{
			FastMode = isFastMode;
			string[] array = reflectShaderNames;
			foreach (string text in array)
			{
				Shader shader = Shader.Find(text);
				if ((bool)shader)
				{
					if (FastMode)
					{
						shader.maximumLOD = 100;
					}
					else
					{
						shader.maximumLOD = 600;
					}
				}
			}
		}

		protected virtual void Update()
		{
			_reflectionPlanePosition = base.transform.position;
		}

		protected virtual void LateUpdate()
		{
			if (FastMode || reflectRendererList.Length == 0)
			{
				return;
			}
			Camera main = Camera.main;
			if ((bool)main && !_insideRendering)
			{
				_insideRendering = true;
				DrawReflectionRenderTexture(main);
				if (useBlur)
				{
					ApplyHexBlur();
				}
				ReflectRenderer[] array = reflectRendererList;
				foreach (ReflectRenderer reflectRenderer in array)
				{
					reflectRenderer.SetMaterialBlock(_reflectionRenderTex);
				}
				_insideRendering = false;
			}
		}

		private void ApplyHexBlur()
		{
			if (_hexBlurMat == null)
			{
				_hexBlurMat = new Material(hexBlurShader);
			}
			RenderTextureWrapper renderTexture = GraphicsUtils.GetRenderTexture(_reflectionRenderTex.width, _reflectionRenderTex.height, 0, bufferFormat);
			ReflectionTool.ApplyHexBlur(_hexBlurMat, _reflectionRenderTex, renderTexture, hexBlurFactor, 0f, 0.5f, 1);
			ReflectionTool.ApplyHexBlur(_hexBlurMat, renderTexture, _reflectionRenderTex, hexBlurFactor, 0f, 0.5f, 1);
			GraphicsUtils.ReleaseRenderTexture(renderTexture);
		}

		private void DrawReflectionRenderTexture(Camera cam)
		{
			Vector3 reflectionPlanePosition = _reflectionPlanePosition;
			Vector3 up = base.transform.up;
			CreateObjects(cam, ref _reflectionRenderTex, ref _reflectionCamera);
			_reflectionCamera.CopyFrom(cam);
			float w = 0f - Vector3.Dot(up, reflectionPlanePosition);
			Matrix4x4 matrix4x = ReflectionTool.CalculateReflectionMatrix(plane: new Vector4(up.x, up.y, up.z, w), reflectionMat: Matrix4x4.zero);
			Vector3 position = cam.transform.position;
			Vector3 position2 = matrix4x.MultiplyPoint(position);
			_reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * matrix4x;
			Vector4 clipPlane = ReflectionTool.CameraSpacePlane(_reflectionCamera, reflectionPlanePosition, up, 1f, reflectClipPlaneOffset);
			Matrix4x4 projectionMatrix = cam.projectionMatrix;
			projectionMatrix = ReflectionTool.CalculateObliqueMatrix(projectionMatrix, clipPlane, -1f);
			_reflectionCamera.projectionMatrix = projectionMatrix;
			_reflectionCamera.cullingMask = -17 & layers.value;
			_reflectionCamera.targetTexture = _reflectionRenderTex;
			_oldInvertCulling = GL.invertCulling;
			GL.invertCulling = true;
			_reflectionCamera.transform.position = position2;
			Vector3 eulerAngles = cam.transform.eulerAngles;
			_reflectionCamera.transform.eulerAngles = new Vector3(0f, eulerAngles.y, eulerAngles.z);
			_reflectionCamera.Render();
			_reflectionCamera.transform.position = position;
			GL.invertCulling = _oldInvertCulling;
		}

		protected virtual void OnDisable()
		{
			if (_reflectionRenderTex != null)
			{
				GraphicsUtils.ReleaseRenderTexture(_reflectionRenderTex);
				_reflectionRenderTex = null;
			}
			if ((bool)_reflectionCamera)
			{
				UnityEngine.Object.DestroyImmediate(_reflectionCamera.gameObject);
				_reflectionCamera = null;
			}
		}

		protected virtual void CreateObjects(Camera srcCam, ref RenderTextureWrapper renderTex, ref Camera destCam)
		{
			if (renderTex == null || _oldTexResScale != texResScale)
			{
				if (renderTex != null)
				{
					GraphicsUtils.ReleaseRenderTexture(renderTex);
				}
				renderTex = GraphicsUtils.GetRenderTexture(srcCam.pixelWidth * (int)texResScale / 100, srcCam.pixelHeight * (int)texResScale / 100, 16, bufferFormat);
				renderTex.content.name = "__RefRenderTexture" + renderTex.content.GetInstanceID();
				renderTex.content.hideFlags = HideFlags.DontSave;
				_oldTexResScale = texResScale;
			}
			if (!destCam)
			{
				GameObject gameObject = new GameObject("__RefCamera for " + srcCam.GetInstanceID(), typeof(Camera));
				destCam = gameObject.GetComponent<Camera>();
				destCam.enabled = false;
				destCam.transform.position = base.transform.position;
				destCam.transform.rotation = base.transform.rotation;
				gameObject.hideFlags = HideFlags.HideAndDontSave;
			}
		}
	}
}
