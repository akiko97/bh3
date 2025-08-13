using System;
using System.Collections;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Camera))]
	public class MainMenuCamera : MonoBehaviour
	{
		public enum BlurTypeEnum
		{
			Gauss = 0,
			Hexagon = 1
		}

		public enum SizeDivEnum
		{
			Div_1 = 1,
			Div_2 = 2,
			Div_3 = 3,
			Div_4 = 4
		}

		public enum BokehQualityEnum
		{
			Normal = 0,
			High = 1
		}

		[HideInInspector]
		public Shader CompositionShader;

		[HideInInspector]
		public Shader GaussShader;

		[HideInInspector]
		public Shader DownSampleShader;

		[HideInInspector]
		public Shader DownSample4xShader;

		[HideInInspector]
		public Shader DownSampleCompShader;

		[HideInInspector]
		public Shader ReflectedShader;

		[HideInInspector]
		public Shader HexBlurShader;

		public Camera FarCamera;

		public float NearFarSeparateDistance = 5f;

		[Header("Background")]
		public GameObject BackgroundQuad;

		private BokehQualityEnum BokehQuality;

		public int ReferencedBufferHeight = 864;

		private float _bufferSizeScale = 1f;

		public AnimationCurve BackgrondBlurCurve;

		private static float maxBlurFactor = 10f;

		[Range(0f, 10f)]
		public float BackgroundBlurFactor;

		private float _startBlurFactor = 1f;

		public float BokehRotateSpeed;

		[Range(0f, 5f)]
		public float BokehIntensity = 1.7f;

		private bool useDOF;

		private Vector4[] dofHexagon = new Vector4[7];

		private Vector4[] dofVector = new Vector4[7];

		private RenderTextureFormat bufferFormat = RenderTextureFormat.ARGBHalf;

		private RenderTextureWrapper _farCameraBuffer;

		private RenderTextureWrapper _farBackgroundBuffer1;

		private RenderTextureWrapper _farBackgroundBuffer2;

		private Material _compositionMat;

		private Material _gaussMat;

		private Material _downSampleMat;

		private Material _downSample4xMat;

		private Material _downSampleCompMat;

		private Material _hexBlurMat;

		private Material _backgroundMat;

		private int _width;

		private int _height;

		private Camera _camera;

		private float _origFarPlane;

		private bool isSupported = true;

		private void Awake()
		{
			Init();
		}

		private void UpdateTargetSize()
		{
			if (_width != _camera.pixelHeight || _height != _camera.pixelHeight)
			{
				_width = _camera.pixelWidth;
				_height = _camera.pixelHeight;
				_backgroundMat.SetVector("_TexelOffset", new Vector4(1f / (float)_width, -1f / (float)_height));
				if (_height < ReferencedBufferHeight)
				{
					_bufferSizeScale = (float)ReferencedBufferHeight / (float)_height;
				}
				else
				{
					_bufferSizeScale = 1f;
				}
			}
		}

		private void Init()
		{
			GenerateDOFHexagon();
			_camera = GetComponent<Camera>();
			_origFarPlane = _camera.farClipPlane;
			_backgroundMat = BackgroundQuad.GetComponent<Renderer>().material;
			UpdateTargetSize();
			CheckResources();
			SetMaterials();
			SetCameras();
		}

		private void OnDestroy()
		{
			ReleaseBuffers();
		}

		private void OnEnable()
		{
			SetDOF();
			DrawBackground();
		}

		private void Update()
		{
			Shader.SetGlobalVector("_miHoYo_CameraRight", base.transform.right);
			UpdateTargetSize();
			SetDOF();
		}

		[AnimationCallback]
		public void OnGameEntryBeforeEnterSpaceshipAnimOver()
		{
			MonoGameEntry monoGameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
			monoGameEntry.OnCameraBeforeEnterSpaceshipAnimOver();
		}

		[AnimationCallback]
		public void OnGameEntryEnterSpaceshipAnimEvent(int phase)
		{
			MonoGameEntry monoGameEntry = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
			monoGameEntry.OnCameraEnterSpaceshipAnimEvent(phase);
		}

		private void SetDOF()
		{
			BackgroundBlurFactor = BackgrondBlurCurve.Evaluate(base.transform.position.z);
			if (!useDOF && BackgroundBlurFactor > _startBlurFactor)
			{
				useDOF = true;
				TurnOnDOF();
			}
			else if (useDOF && BackgroundBlurFactor < _startBlurFactor)
			{
				useDOF = false;
				TurnOffDOF();
			}
		}

		private IEnumerator TurnOnDOFDelaySet()
		{
			yield return new WaitForEndOfFrame();
			useDOF = true;
			_camera.farClipPlane = NearFarSeparateDistance;
			BackgroundQuad.SetActive(true);
		}

		private void TurnOnDOF()
		{
			FarCamera.nearClipPlane = NearFarSeparateDistance;
			StartCoroutine(TurnOnDOFDelaySet());
		}

		private void TurnOffDOF()
		{
			useDOF = false;
			_camera.farClipPlane = _origFarPlane;
			BackgroundQuad.SetActive(false);
		}

		private void GenerateDOFHexagon()
		{
			dofHexagon[0] = new Vector4(0f, 0f, 0f, 0f);
			for (int i = 0; i < 6; i++)
			{
				float num = Mathf.Sin((float)Math.PI / 3f * (float)i);
				float num2 = Mathf.Cos((float)Math.PI / 3f * (float)i);
				float num3 = 0f;
				float num4 = 1f;
				float x = num2 * num3 - num * num4;
				float y = num * num3 + num2 * num4;
				dofHexagon[i + 1] = new Vector4(x, y, 0f, 0f);
			}
		}

		private void ReportAutoDisable()
		{
			Debug.LogWarning(string.Concat("Camera effect of ", this, " has been disabled as it's not supported on the current platform."));
		}

		private bool CheckResources()
		{
			_compositionMat = CheckShaderAndCreateMaterial(CompositionShader, _compositionMat);
			_gaussMat = CheckShaderAndCreateMaterial(GaussShader, _gaussMat);
			_downSampleMat = CheckShaderAndCreateMaterial(DownSampleShader, _downSampleMat);
			_downSample4xMat = CheckShaderAndCreateMaterial(DownSample4xShader, _downSample4xMat);
			_downSampleCompMat = CheckShaderAndCreateMaterial(DownSampleCompShader, _downSampleCompMat);
			_hexBlurMat = CheckShaderAndCreateMaterial(HexBlurShader, _hexBlurMat);
			if (!isSupported)
			{
				ReportAutoDisable();
			}
			return isSupported;
		}

		private void NotSupported()
		{
			base.enabled = false;
			isSupported = false;
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

		private void CreateBuffers()
		{
			int num = (int)((float)_width * _bufferSizeScale) / 2;
			int num2 = (int)((float)_height * _bufferSizeScale) / 2;
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
			_compositionMat.SetTexture("_FarTex", (RenderTexture)_farCameraBuffer);
			FarCamera.targetTexture = _farCameraBuffer;
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

		private void SetMaterials()
		{
			if (_downSample4xMat != null)
			{
				_downSample4xMat.SetVector("twoTexelSize", new Vector2(2f / (float)_width, 2f / (float)_height));
			}
		}

		private void SetCameraParamsWithMainCamera(Camera cam)
		{
			cam.fieldOfView = _camera.fieldOfView;
			cam.nearClipPlane = _camera.nearClipPlane;
			cam.farClipPlane = _camera.farClipPlane;
		}

		private void SetCameras()
		{
			SetCameraParamsWithMainCamera(FarCamera);
			FarCamera.enabled = false;
		}

		private void SetGaussParam(float s, float scale, float width)
		{
			GaussParamGenerator gaussParamGenerator = new GaussParamGenerator(s, width);
			float[] array = new float[8];
			for (int i = 0; i < gaussParamGenerator.Weights.Length; i++)
			{
				array[i] = gaussParamGenerator.Weights[i];
			}
			_gaussMat.SetVector("_weights", new Vector4(array[0], array[1], array[2], array[3]));
			_gaussMat.SetVector("_weights2", new Vector4(array[4], array[5], array[6], array[7]));
			for (int j = 0; j < gaussParamGenerator.Offsets.Length; j++)
			{
				_gaussMat.SetVector("_offset_4_" + (j + 1), gaussParamGenerator.Offsets[j] * scale);
			}
		}

		private void ApplyHexBlur(RenderTexture source, RenderTexture destination, float blurFactor, float bokehIntensity, float tapScale, int pass = 0)
		{
			float num = 1f / (float)source.width;
			float num2 = 1f / (float)source.height;
			Vector4 vector = new Vector4(blurFactor * num, blurFactor * num2, 0f, bokehIntensity);
			_hexBlurMat.SetVector("blurScale", vector);
			float num3 = Mathf.Sin(BackgroundBlurFactor * BokehRotateSpeed);
			float num4 = Mathf.Cos(BackgroundBlurFactor * BokehRotateSpeed);
			for (int i = 1; i < 7; i++)
			{
				dofVector[i - 1] = new Vector4(num4 * dofHexagon[i].x - num3 * dofHexagon[i].y, num3 * dofHexagon[i].x + num4 * dofHexagon[i].y, 0f, 0f) * tapScale;
			}
			for (int j = 0; j < 6; j++)
			{
				_hexBlurMat.SetVector("dofScatter" + (j + 1), dofVector[j]);
			}
			_hexBlurMat.mainTexture = source;
			destination.DiscardContents();
			Graphics.Blit(source, destination, _hexBlurMat, pass);
		}

		private RenderTexture ApplyDOFtoBackground()
		{
			CreateBuffers();
			FarCamera.Render();
			int num = (int)((float)_width * _bufferSizeScale);
			int num2 = (int)((float)_height * _bufferSizeScale);
			RenderTexture renderTexture2;
			int num3;
			if (BackgroundBlurFactor < maxBlurFactor / 2f)
			{
				RenderTexture renderTexture = _farCameraBuffer;
				renderTexture2 = _farBackgroundBuffer1;
				num3 = 2;
			}
			else
			{
				RenderTexture renderTexture = _farBackgroundBuffer1;
				Graphics.Blit((RenderTexture)_farCameraBuffer, renderTexture, _downSampleMat, 0);
				renderTexture2 = _farBackgroundBuffer2;
				num3 = 2;
			}
			if (BokehQuality == BokehQualityEnum.Normal)
			{
				RenderTextureWrapper renderTexture3 = GraphicsUtils.GetRenderTexture(num / num3, num2 / num3, 0, bufferFormat);
				ApplyHexBlur(_farCameraBuffer, renderTexture3, BackgroundBlurFactor, BokehIntensity, 1f / (float)num3);
				ApplyHexBlur(renderTexture3, renderTexture2, BackgroundBlurFactor, BokehIntensity, 2f / (float)num3);
				GraphicsUtils.ReleaseRenderTexture(renderTexture3);
			}
			else if (BokehQuality == BokehQualityEnum.High)
			{
				RenderTextureWrapper renderTexture4 = GraphicsUtils.GetRenderTexture(num / num3, num2 / num3, 0, bufferFormat);
				RenderTextureWrapper renderTexture5 = GraphicsUtils.GetRenderTexture(num / num3, num2 / num3, 0, bufferFormat);
				ApplyHexBlur(_farCameraBuffer, renderTexture4, BackgroundBlurFactor, BokehIntensity, 0.5f / (float)num3);
				ApplyHexBlur(renderTexture4, renderTexture5, BackgroundBlurFactor, BokehIntensity, 1f / (float)num3);
				ApplyHexBlur(renderTexture5, renderTexture2, BackgroundBlurFactor, BokehIntensity, 2f / (float)num3);
				GraphicsUtils.ReleaseRenderTexture(renderTexture4);
				GraphicsUtils.ReleaseRenderTexture(renderTexture5);
			}
			return renderTexture2;
		}

		private void OnPreRender()
		{
			DrawBackground();
		}

		private void OnPostRender()
		{
			ReleaseBuffers();
		}

		private void DrawBackground()
		{
			if (useDOF)
			{
				RenderTexture tex = ApplyDOFtoBackground();
				Shader.SetGlobalTexture("_miHoYo_Background", tex);
			}
		}
	}
}
