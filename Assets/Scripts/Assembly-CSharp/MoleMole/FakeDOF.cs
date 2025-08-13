using System;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Camera))]
	public class FakeDOF : PostFXFramework
	{
		public enum BokehQualityEnum
		{
			Normal = 0,
			High = 1
		}

		[HideInInspector]
		public Shader DownSampleShader;

		[HideInInspector]
		public Shader HexBlurShader;

		[HideInInspector]
		public Shader BackgroundShader;

		private Material _downSampleMat;

		private Material _hexBlurMat;

		private Material _backgroundMat;

		public BokehQualityEnum bokehQuality;

		private static readonly float _maxBlurFactor = 10f;

		private static readonly float _minBlurFactor = 1f;

		[Range(0f, 10f)]
		public float backgroundBlurFactor;

		public float bokehRotateSpeed;

		[Range(0f, 5f)]
		public float bokehIntensity;

		public float separateDistance = 5f;

		private Vector4[] _dofHexagon = new Vector4[7];

		private Vector4[] _dofVector = new Vector4[7];

		private RenderTextureFormat bufferFormat = RenderTextureFormat.ARGBHalf;

		private RenderTextureWrapper _farCameraBuffer;

		private RenderTextureWrapper _farBackgroundBuffer1;

		private RenderTextureWrapper _farBackgroundBuffer2;

		private Camera _camera;

		private Camera _farCamera;

		private PostFXWithResScale _postFXWithRescale;

		private int _width;

		private int _height;

		private float _origFarPlane;

		private Renderer _backgroundRenderer;

		private void Init()
		{
			_camera = GetComponent<Camera>();
			_origFarPlane = _camera.farClipPlane;
			_postFXWithRescale = GetComponent<PostFXWithResScale>();
			GenerateDOFHexagon();
			CreateMaterials();
			CreateBackgourQuad();
			CreateFarCamera();
		}

		private void Awake()
		{
			Init();
		}

		private void OnPreRender()
		{
			_width = ((!(_postFXWithRescale == null)) ? _postFXWithRescale.InnerCameraWidth : Screen.width);
			_height = ((!(_postFXWithRescale == null)) ? _postFXWithRescale.InnerCameraHeight : Screen.height);
			DoProcess();
		}

		private void OnPostRender()
		{
			ReleaseBuffers();
		}

		private void TurnOnDOF()
		{
			_camera.farClipPlane = separateDistance;
		}

		private void TurnOffDOF()
		{
			_camera.farClipPlane = _origFarPlane;
		}

		private void DoProcess()
		{
			if (backgroundBlurFactor > _minBlurFactor)
			{
				TurnOnDOF();
				CreateBuffers();
				Vector3 localPosition = _backgroundRenderer.transform.localPosition;
				localPosition.z = separateDistance - 0.01f;
				_backgroundRenderer.transform.localPosition = localPosition;
				DrawBackgroud();
				RenderTexture tex = ApplyDOF(_farCameraBuffer);
				Shader.SetGlobalTexture("_miHoYo_Background", tex);
			}
			else
			{
				TurnOffDOF();
			}
		}

		private void OnDisable()
		{
			TurnOffDOF();
		}

		private void CreateMaterials()
		{
			_downSampleMat = CheckShaderAndCreateMaterial(DownSampleShader, _downSampleMat);
			_hexBlurMat = CheckShaderAndCreateMaterial(HexBlurShader, _hexBlurMat);
			_backgroundMat = CheckShaderAndCreateMaterial(BackgroundShader, _backgroundMat);
		}

		private void CreateBuffers()
		{
			int num = _width / 2;
			int num2 = _height / 2;
			if (_farCameraBuffer == null)
			{
				_farCameraBuffer = GraphicsUtils.GetRenderTexture(num, num2, 16, bufferFormat);
			}
			if (_farBackgroundBuffer1 == null)
			{
				_farBackgroundBuffer1 = GraphicsUtils.GetRenderTexture(num, num2, 0, bufferFormat);
			}
			if (_farBackgroundBuffer2 == null)
			{
				_farBackgroundBuffer2 = GraphicsUtils.GetRenderTexture(num / 2, num2 / 2, 0, bufferFormat);
			}
		}

		private void ReleaseBuffers()
		{
			if (_farCameraBuffer != null)
			{
				GraphicsUtils.ReleaseRenderTexture(_farCameraBuffer);
				_farCameraBuffer = null;
			}
			if (_farBackgroundBuffer1 != null)
			{
				GraphicsUtils.ReleaseRenderTexture(_farBackgroundBuffer1);
				_farBackgroundBuffer1 = null;
			}
			if (_farBackgroundBuffer2 != null)
			{
				GraphicsUtils.ReleaseRenderTexture(_farBackgroundBuffer2);
				_farBackgroundBuffer2 = null;
			}
		}

		private void CreateFarCamera()
		{
			CreateCamera(_camera, ref _farCamera, "DofFarCamera");
			_farCamera.farClipPlane = _origFarPlane;
		}

		private void CreateBackgourQuad()
		{
			if (_backgroundRenderer == null)
			{
				GameObject gameObject = new GameObject("_DOFBackground", typeof(MeshRenderer));
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				gameObject.transform.SetParentAndReset(base.transform);
				MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
				meshFilter.mesh = MeshGenerator.Quad();
				_backgroundRenderer = gameObject.GetComponent<Renderer>();
				_backgroundRenderer.material = _backgroundMat;
			}
		}

		private void GenerateDOFHexagon()
		{
			_dofHexagon[0] = new Vector4(0f, 0f, 0f, 0f);
			for (int i = 0; i < 6; i++)
			{
				float num = Mathf.Sin((float)Math.PI / 3f * (float)i);
				float num2 = Mathf.Cos((float)Math.PI / 3f * (float)i);
				float num3 = 0f;
				float num4 = 1f;
				float x = num2 * num3 - num * num4;
				float y = num * num3 + num2 * num4;
				_dofHexagon[i + 1] = new Vector4(x, y, 0f, 0f);
			}
		}

		private void ApplyHexBlur(RenderTexture source, RenderTexture destination, float blurFactor, float bokehIntensity, float tapScale, int pass = 0)
		{
			float num = 1f / (float)source.width;
			float num2 = 1f / (float)source.height;
			Vector4 vector = new Vector4(blurFactor * num, blurFactor * num2, 0f, bokehIntensity);
			_hexBlurMat.SetVector("blurScale", vector);
			float num3 = Mathf.Sin(backgroundBlurFactor * bokehRotateSpeed);
			float num4 = Mathf.Cos(backgroundBlurFactor * bokehRotateSpeed);
			for (int i = 1; i < 7; i++)
			{
				_dofVector[i - 1] = new Vector4(num4 * _dofHexagon[i].x - num3 * _dofHexagon[i].y, num3 * _dofHexagon[i].x + num4 * _dofHexagon[i].y, 0f, 0f) * tapScale;
			}
			for (int j = 0; j < 6; j++)
			{
				_hexBlurMat.SetVector("dofScatter" + (j + 1), _dofVector[j]);
			}
			_hexBlurMat.mainTexture = source;
			destination.DiscardContents();
			Graphics.Blit(source, destination, _hexBlurMat, pass);
		}

		private RenderTexture ApplyDOF(RenderTexture texture)
		{
			int width = _width;
			int height = _height;
			RenderTexture renderTexture;
			RenderTexture renderTexture2;
			int num;
			if (backgroundBlurFactor < _maxBlurFactor / 2f)
			{
				renderTexture = _farCameraBuffer;
				renderTexture2 = _farBackgroundBuffer1;
				num = 2;
			}
			else
			{
				renderTexture = _farBackgroundBuffer1;
				Graphics.Blit((RenderTexture)_farCameraBuffer, renderTexture, _downSampleMat, 0);
				renderTexture2 = _farBackgroundBuffer2;
				num = 2;
			}
			if (bokehQuality == BokehQualityEnum.Normal)
			{
				RenderTextureWrapper renderTexture3 = GraphicsUtils.GetRenderTexture(width / num, height / num, 0, bufferFormat);
				ApplyHexBlur(renderTexture, renderTexture3, backgroundBlurFactor, bokehIntensity, 1f / (float)num);
				ApplyHexBlur(renderTexture3, renderTexture2, backgroundBlurFactor, bokehIntensity, 2f / (float)num);
				GraphicsUtils.ReleaseRenderTexture(renderTexture3);
			}
			else if (bokehQuality == BokehQualityEnum.High)
			{
				RenderTextureWrapper renderTexture4 = GraphicsUtils.GetRenderTexture(width / num, height / num, 0, bufferFormat);
				RenderTextureWrapper renderTexture5 = GraphicsUtils.GetRenderTexture(width / num, height / num, 0, bufferFormat);
				ApplyHexBlur(renderTexture, renderTexture4, backgroundBlurFactor, bokehIntensity, 0.5f / (float)num);
				ApplyHexBlur(renderTexture4, renderTexture5, backgroundBlurFactor, bokehIntensity, 1f / (float)num);
				ApplyHexBlur(renderTexture5, renderTexture2, backgroundBlurFactor, bokehIntensity, 2f / (float)num);
				GraphicsUtils.ReleaseRenderTexture(renderTexture4);
				GraphicsUtils.ReleaseRenderTexture(renderTexture5);
			}
			return renderTexture2;
		}

		private void DrawBackgroud()
		{
			_farCamera.CopyFrom(_camera);
			_farCamera.clearFlags = CameraClearFlags.Color;
			_farCamera.nearClipPlane = separateDistance;
			_farCamera.farClipPlane = _origFarPlane;
			_farCamera.targetTexture = _farCameraBuffer;
			_farCamera.Render();
		}
	}
}
