using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	[AddComponentMenu("Image Effects/PostFX")]
	[RequireComponent(typeof(Camera))]
	[ExecuteInEditMode]
	public class PostFXBase : MonoBehaviour
	{
		public enum SizeDownScaleEnum
		{
			Div_1 = 1,
			Div_2 = 2,
			Div_3 = 3,
			Div_4 = 4,
			Div_5 = 5,
			Div_6 = 6,
			Div_7 = 7,
			Div_8 = 8
		}

		public enum InternalBufferSizeEnum
		{
			SIZE_128 = 128,
			SIZE_256 = 256,
			SIZE_512 = 512,
			NULL = 0
		}

		[NonSerialized]
		public const float defaultGameConstrast = 2f;

		public const float defaultUIConstrast = 2.1f;

		private static readonly string RADIAL_BLUR_SHDER_KEYWORD = "RADIAL_BLUR";

		private static readonly string _DISTORTION_CAMERA_NAME = "DistortionCamera";

		protected static int HASH_SEPIA_COLOR;

		protected static int HASH_MAIN_TEX;

		protected static int HASH_TEXEL_SIZE;

		protected static int HASH_THRESHOLD;

		protected static int HASH_SCALER_UPPER_CASE;

		protected static int HASH_SCALER_LOWER_CASE;

		protected static int HASH_MAIN_TEX_0;

		protected static int HASH_MAIN_TEX_1;

		protected static int HASH_MAIN_TEX_2;

		protected static int HASH_MAIN_TEX_3;

		protected static int HASH_COEFF;

		protected static int HASH_EXPOSURE;

		protected static int HASH_CONSTRAST;

		protected static int HASH_LUM_TRESHOLD;

		protected static int HASH_LUM_SCALER;

		protected static int HASH_DISTORTION_TEX;

		protected static int HASH_RADIAL_BLUR_TEX;

		protected static int HASH_RADIAL_BLUR_PARAM;

		protected static int HASH_SCALE_RG;

		protected static int HASH_DIM;

		protected static int HASH_OFFSET;

		protected static int HASH_LUT_TEX;

		protected static int HASH_FX_ALPHA_INTENSITY;

		protected static int HASH_ALPHA_TEX;

		private static bool _hashInited;

		[HideInInspector]
		public bool originalEnabled;

		[HideInInspector]
		public Shader DistortionApplyShader;

		protected Material DistortionApplyMat;

		[HideInInspector]
		public Shader DistortionMapNormShader;

		[HideInInspector]
		public Shader DrawDepthShader;

		[HideInInspector]
		public Shader DrawAlphaShader;

		[Range(0f, 5f)]
		public float glareThreshold = 0.65f;

		[Range(0f, 5f)]
		public float glareScaler = 1.06f;

		[Range(0f, 1f)]
		public float glareIntensity = 0.75f;

		[Range(0f, 5000f)]
		public float Exposure = 13f;

		[Range(0.3f, 10f)]
		public float constrast = 2f;

		public bool FastMode;

		public bool FXAA;

		public bool FXAAForceHQ;

		public bool HDRBuffer = true;

		public bool WriteAlpha;

		public float fxAlphaIntensity = 0.5f;

		public Color SepiaColor = new Color(0.5f, 0.5f, 0.5f, 0f);

		private Camera alphaCamera;

		[Header("Distortion")]
		public bool UseDistortion = true;

		public bool UseDepthTest = true;

		[HideInInspector]
		public int DistortionMapSizeDown = 2;

		[HideInInspector]
		public Color DistortionMapClearColor = new Color(0.498f, 0.498f, 0f, 0f);

		[HideInInspector]
		public int DistortionMapDepthBit = 16;

		[HideInInspector]
		public RenderTextureFormat DistortionMapFormat;

		public LayerMask DepthCullingMask;

		public LayerMask DistortionCullingMask;

		private Camera __distortionCamera;

		[Header("Radial Blur")]
		public bool DisableRadialBlur;

		public Vector2 RadialBlurCenter = new Vector2(0.5f, 0.5f);

		[Range(0f, 2f)]
		public float RadialBlurScatterScale = 1f;

		[Range(0f, 10f)]
		public float RadialBlurStrenth;

		public SizeDownScaleEnum RadialBlurDownScale = SizeDownScaleEnum.Div_4;

		[HideInInspector]
		public Shader RadialBlurShader;

		protected Material RadialBlurMat;

		protected RenderTextureWrapper radial_blur_buffer;

		protected RenderTextureWrapper radial_blur_buffer_temp;

		[Tooltip("If camera writes the depth texture")]
		[Header("Utility")]
		public bool WriteDepthTexture;

		[Header("Color Grading")]
		public bool UseColorGrading = true;

		public Texture2D sourceLut3D;

		protected Texture2D converted2DLut;

		protected string basedOnTempTex = string.Empty;

		[Range(0f, 1f)]
		[SerializeField]
		protected float avatarShadowAdjust = 0.5f;

		[HideInInspector]
		public Shader DownSample4XShader;

		protected Material DownSample4X;

		[HideInInspector]
		public Shader DownSampleShader;

		protected Material DownSample;

		[HideInInspector]
		public Shader BrightPassExShader;

		protected Material BrightPassEx;

		[HideInInspector]
		public Shader GaussCompositionExShader;

		protected Material GaussCompositionEx;

		[HideInInspector]
		public Shader GlareCompositionExShader;

		protected Material GlareCompositionEx;

		[HideInInspector]
		public Shader MultipleGaussPassFilterShader_128;

		[HideInInspector]
		public Shader MultipleGaussPassFilterShader_256;

		[HideInInspector]
		public Shader MultipleGaussPassFilterShader_512;

		protected Dictionary<InternalBufferSizeEnum, Shader> MultipleGaussPassFilterShaderMap;

		protected Material MultipleGaussPassFilter;

		protected RenderTextureWrapper[] compose_buffer = new RenderTextureWrapper[6];

		protected RenderTextureWrapper[] blur_bufferA = new RenderTextureWrapper[6];

		protected RenderTextureWrapper[] blur_bufferB = new RenderTextureWrapper[6];

		protected RenderTextureWrapper alpha_buffer;

		protected RenderTextureWrapper downsample4X_buffer;

		protected RenderTextureWrapper gauss_buffer;

		protected RenderTextureWrapper distortionMap;

		protected float aspectRatio;

		protected int downsampleLevel;

		protected Vector4 blurCoeff = new Vector4(0.3f, 0.3f, 0.26f, 0.15f);

		protected Vector4 glareCoeff = new Vector4(0f, 0f, 0f, 0f);

		protected float stepx;

		protected float stepy;

		private InternalBufferSizeEnum lastInternalBufferSize;

		public InternalBufferSizeEnum internalBufferSize = InternalBufferSizeEnum.SIZE_128;

		private Shader __multipleGaussPassShader;

		protected RenderTextureFormat internalBufferFormat = RenderTextureFormat.ARGBHalf;

		protected bool supportHDRTextures = true;

		protected bool supportDX11;

		protected bool isSupported = true;

		protected Camera _camera;

		protected Camera _distortionCamera
		{
			get
			{
				if (__distortionCamera == null)
				{
					Camera[] componentsInChildren = GetComponentsInChildren<Camera>(true);
					foreach (Camera camera in componentsInChildren)
					{
						if (camera.name == _DISTORTION_CAMERA_NAME)
						{
							__distortionCamera = camera;
							break;
						}
					}
					if (__distortionCamera == null)
					{
						UseDistortion = false;
					}
				}
				return __distortionCamera;
			}
		}

		public bool UseRadialBlur
		{
			get
			{
				return !DisableRadialBlur && RadialBlurStrenth > float.Epsilon;
			}
		}

		public float AvatarShadowAdjust
		{
			get
			{
				return (!FastMode) ? 0f : avatarShadowAdjust;
			}
		}

		private Shader _multipleGaussPassShader
		{
			get
			{
				if (lastInternalBufferSize != internalBufferSize)
				{
					lastInternalBufferSize = internalBufferSize;
					if (MultipleGaussPassFilterShaderMap != null && MultipleGaussPassFilterShaderMap.ContainsKey(internalBufferSize))
					{
						__multipleGaussPassShader = MultipleGaussPassFilterShaderMap[internalBufferSize];
					}
				}
				return __multipleGaussPassShader;
			}
		}

		public bool SupportHDR
		{
			get
			{
				return supportHDRTextures;
			}
		}

		private static void InitUniformHashes()
		{
			if (!_hashInited)
			{
				HASH_SEPIA_COLOR = Shader.PropertyToID("_SepiaColor");
				HASH_MAIN_TEX = Shader.PropertyToID("_MainTex");
				HASH_TEXEL_SIZE = Shader.PropertyToID("_texelSize");
				HASH_THRESHOLD = Shader.PropertyToID("_Threshhold");
				HASH_SCALER_UPPER_CASE = Shader.PropertyToID("_Scaler");
				HASH_SCALER_LOWER_CASE = Shader.PropertyToID("_scaler");
				HASH_MAIN_TEX_0 = Shader.PropertyToID("_MainTex0");
				HASH_MAIN_TEX_1 = Shader.PropertyToID("_MainTex1");
				HASH_MAIN_TEX_2 = Shader.PropertyToID("_MainTex2");
				HASH_MAIN_TEX_3 = Shader.PropertyToID("_MainTex3");
				HASH_COEFF = Shader.PropertyToID("coeff");
				HASH_EXPOSURE = Shader.PropertyToID("exposure");
				HASH_CONSTRAST = Shader.PropertyToID("constrast");
				HASH_LUM_TRESHOLD = Shader.PropertyToID("lumThreshold");
				HASH_LUM_SCALER = Shader.PropertyToID("lumScaler");
				HASH_DISTORTION_TEX = Shader.PropertyToID("_DistortionTex");
				HASH_RADIAL_BLUR_TEX = Shader.PropertyToID("_RadialBlurTex");
				HASH_RADIAL_BLUR_PARAM = Shader.PropertyToID("_RadialBlurParam");
				HASH_SCALE_RG = Shader.PropertyToID("_ScaleRG");
				HASH_DIM = Shader.PropertyToID("_Dim");
				HASH_OFFSET = Shader.PropertyToID("_Offset");
				HASH_LUT_TEX = Shader.PropertyToID("_LutTex");
				HASH_FX_ALPHA_INTENSITY = Shader.PropertyToID("_fxAlphaIntensity");
				HASH_ALPHA_TEX = Shader.PropertyToID("_AlphaTex");
				_hashInited = true;
			}
		}

		protected static void MyDestroy(UnityEngine.Object obj)
		{
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
			else
			{
				UnityEngine.Object.Destroy(obj);
			}
		}

		protected Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
		{
			if (!s)
			{
				Debug.LogError("Missing shader in " + this);
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
				Debug.LogError(string.Concat("The shader ", s, " on effect ", this, " is not supported on this platform!"));
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

		private bool CheckSupport(bool needDepth)
		{
			isSupported = true;
			supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(internalBufferFormat);
			supportDX11 = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
			if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
			{
				NotSupported();
				return false;
			}
			if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
			{
				NotSupported();
				return false;
			}
			if (needDepth)
			{
				GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
			}
			return true;
		}

		private bool CheckSupport(bool needDepth, bool needHdr)
		{
			if (!CheckSupport(needDepth))
			{
				return false;
			}
			if (needHdr && !supportHDRTextures)
			{
				NotSupported();
				return false;
			}
			return true;
		}

		private bool Dx11Support()
		{
			return supportDX11;
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

		private void CheckShaderExists(Shader s)
		{
			if (!s)
			{
				base.enabled = false;
			}
		}

		private void CheckShaderExists(out Shader s, string name)
		{
			s = Shader.Find(name);
			if (!s)
			{
				base.enabled = false;
			}
		}

		protected void NotSupported()
		{
			base.enabled = false;
			isSupported = false;
		}

		protected virtual void OnDestroy()
		{
			ReleaseMaterials();
			if ((bool)converted2DLut)
			{
				UnityEngine.Object.DestroyImmediate(converted2DLut);
			}
			converted2DLut = null;
		}

		private void SetIdentityLut()
		{
			int num = 16;
			Color[] array = new Color[num * num * num * num];
			float num2 = 1f / (1f * (float)num - 1f);
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num; j++)
				{
					for (int k = 0; k < num; k++)
					{
						for (int l = 0; l < num; l++)
						{
							array[k + i * num + l * num * num + j * num * num * num] = new Color((float)k * num2, (float)l * num2, (float)(j * num + i) / ((float)(num * num) - 1f), 1f);
						}
					}
				}
			}
			if ((bool)converted2DLut)
			{
				UnityEngine.Object.DestroyImmediate(converted2DLut);
			}
			converted2DLut = new Texture2D(num * num, num * num, TextureFormat.ARGB32, false);
			converted2DLut.SetPixels(array);
			converted2DLut.Apply();
			basedOnTempTex = string.Empty;
		}

		private bool ValidDimensions(Texture2D tex2d)
		{
			if (!tex2d)
			{
				return false;
			}
			int height = tex2d.height;
			if (height != Mathf.FloorToInt(Mathf.Sqrt(tex2d.width)))
			{
				return false;
			}
			if (height != 16)
			{
				return false;
			}
			return true;
		}

		private void Convert(Texture2D temp2DTex, string path)
		{
			if ((bool)temp2DTex)
			{
				int num = temp2DTex.width * temp2DTex.height;
				num = temp2DTex.height;
				if (!ValidDimensions(temp2DTex))
				{
					Debug.LogWarning("The given 2D texture " + temp2DTex.name + " cannot be used as a 3D LUT.");
					basedOnTempTex = string.Empty;
					return;
				}
				Color[] pixels = temp2DTex.GetPixels();
				Color[] array = new Color[num * num * num * num];
				for (int i = 0; i < num; i++)
				{
					for (int j = 0; j < num; j++)
					{
						for (int k = 0; k < num; k++)
						{
							for (int l = 0; l < num; l++)
							{
								float num2 = ((float)i + (float)(j * num) * 1f) / (float)num;
								int num3 = Mathf.FloorToInt(num2);
								int num4 = Mathf.Min(num3 + 1, num - 1);
								float t = num2 - (float)num3;
								int num5 = k + (num - l - 1) * num * num;
								Color a = pixels[num5 + num3 * num];
								Color b = pixels[num5 + num4 * num];
								array[k + i * num + l * num * num + j * num * num * num] = Color.Lerp(a, b, t);
							}
						}
					}
				}
				if ((bool)converted2DLut)
				{
					UnityEngine.Object.DestroyImmediate(converted2DLut);
				}
				converted2DLut = new Texture2D(num * num, num * num, TextureFormat.ARGB32, false);
				converted2DLut.SetPixels(array);
				converted2DLut.Apply();
				basedOnTempTex = path;
			}
			else
			{
				Debug.LogError("Couldn't color correct with 2D LUT texture. Image Effect will be disabled.");
			}
		}

		protected bool CheckResources()
		{
			CheckSupport(false);
			DistortionApplyMat = CheckShaderAndCreateMaterial(DistortionApplyShader, DistortionApplyMat);
			DownSample4X = CheckShaderAndCreateMaterial(DownSample4XShader, DownSample4X);
			DownSample = CheckShaderAndCreateMaterial(DownSampleShader, DownSample);
			BrightPassEx = CheckShaderAndCreateMaterial(BrightPassExShader, BrightPassEx);
			GaussCompositionEx = CheckShaderAndCreateMaterial(GaussCompositionExShader, GaussCompositionEx);
			GlareCompositionEx = CheckShaderAndCreateMaterial(GlareCompositionExShader, GlareCompositionEx);
			MultipleGaussPassFilter = CheckShaderAndCreateMaterial(_multipleGaussPassShader, MultipleGaussPassFilter);
			if (UseRadialBlur)
			{
				RadialBlurMat = CheckShaderAndCreateMaterial(RadialBlurShader, RadialBlurMat);
			}
			CheckShaderExists(DistortionMapNormShader);
			CheckShaderExists(DrawDepthShader);
			CheckShaderExists(DrawAlphaShader);
			if (!isSupported)
			{
				ReportAutoDisable();
			}
			return isSupported;
		}

		protected void ReleaseMaterials()
		{
			MyDestroy(DistortionApplyMat);
			MyDestroy(DownSample4X);
			MyDestroy(DownSample);
			MyDestroy(BrightPassEx);
			MyDestroy(GaussCompositionEx);
			MyDestroy(GlareCompositionEx);
			MyDestroy(MultipleGaussPassFilter);
			if (UseRadialBlur)
			{
				MyDestroy(RadialBlurMat);
			}
		}

		protected virtual void OnEnable()
		{
			InitUniformHashes();
			isSupported = true;
			_camera = GetComponent<Camera>();
			PrepareMultipleGaussShaders();
		}

		public void PrepareMultipleGaussShaders()
		{
			MultipleGaussPassFilterShaderMap = new Dictionary<InternalBufferSizeEnum, Shader>();
			MultipleGaussPassFilterShaderMap[InternalBufferSizeEnum.SIZE_128] = MultipleGaussPassFilterShader_128;
			MultipleGaussPassFilterShaderMap[InternalBufferSizeEnum.SIZE_256] = MultipleGaussPassFilterShader_256;
			MultipleGaussPassFilterShaderMap[InternalBufferSizeEnum.SIZE_512] = MultipleGaussPassFilterShader_512;
		}

		protected virtual void OnDisable()
		{
			if (alphaCamera != null)
			{
				UnityEngine.Object.DestroyImmediate(alphaCamera.gameObject);
				alphaCamera = null;
			}
		}

		protected virtual void Update()
		{
			if (internalBufferSize == InternalBufferSizeEnum.SIZE_128)
			{
				blurCoeff = new Vector4(0.3f, 0.3f, 0.26f, 0.15f);
			}
			else if (internalBufferSize == InternalBufferSizeEnum.SIZE_256)
			{
				blurCoeff = new Vector4(0.24f, 0.24f, 0.28f, 0.225f);
			}
			else
			{
				blurCoeff = new Vector4(0.18f, 0.18f, 0.3f, 0.3f);
			}
		}

		protected virtual void CreateDistortionMap(int w, int h)
		{
			distortionMap = GraphicsUtils.GetRenderTexture(w / 2, h / 2, DistortionMapDepthBit, DistortionMapFormat);
		}

		protected void CreateRenderTextures(int w, int h)
		{
			if (WriteAlpha)
			{
				alpha_buffer = GraphicsUtils.GetRenderTexture(w, h, 0, RenderTextureFormat.R8);
			}
			downsample4X_buffer = GraphicsUtils.GetRenderTexture(w / 4, h / 4, 16, internalBufferFormat);
			if (UseDistortion)
			{
				CreateDistortionMap(w, h);
			}
			if (UseRadialBlur)
			{
				int radialBlurDownScale = (int)RadialBlurDownScale;
				radial_blur_buffer = GraphicsUtils.GetRenderTexture(w / radialBlurDownScale, h / radialBlurDownScale, 0, internalBufferFormat);
				radial_blur_buffer_temp = GraphicsUtils.GetRenderTexture(w / radialBlurDownScale, h / radialBlurDownScale, 0, internalBufferFormat);
			}
			int num = (int)internalBufferSize;
			int num2 = (int)internalBufferSize;
			gauss_buffer = GraphicsUtils.GetRenderTexture(num2, num2, 0, RenderTextureFormat.ARGBHalf);
			gauss_buffer.content.filterMode = FilterMode.Bilinear;
			for (int i = 0; i < 6; i++)
			{
				compose_buffer[i] = GraphicsUtils.GetRenderTexture(num, num2, 0, internalBufferFormat);
				blur_bufferA[i] = GraphicsUtils.GetRenderTexture(num, num2, 0, internalBufferFormat);
				blur_bufferB[i] = GraphicsUtils.GetRenderTexture(num, num2, 0, internalBufferFormat);
				num /= 2;
				num2 /= 2;
			}
		}

		protected void ReleaseRenderTextures()
		{
			if (WriteAlpha)
			{
				GraphicsUtils.ReleaseRenderTexture(alpha_buffer);
			}
			GraphicsUtils.ReleaseRenderTexture(downsample4X_buffer);
			GraphicsUtils.ReleaseRenderTexture(gauss_buffer);
			if (UseDistortion)
			{
				GraphicsUtils.ReleaseRenderTexture(distortionMap);
			}
			if (UseRadialBlur)
			{
				GraphicsUtils.ReleaseRenderTexture(radial_blur_buffer);
				GraphicsUtils.ReleaseRenderTexture(radial_blur_buffer_temp);
			}
			for (int i = 0; i < 6; i++)
			{
				GraphicsUtils.ReleaseRenderTexture(compose_buffer[i]);
				GraphicsUtils.ReleaseRenderTexture(blur_bufferA[i]);
				GraphicsUtils.ReleaseRenderTexture(blur_bufferB[i]);
			}
		}

		protected void CreateCamera(Camera srcCam, ref Camera destCam)
		{
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

		protected void PrepareAlpha()
		{
			CreateCamera(_camera, ref alphaCamera);
			alphaCamera.CopyFrom(_camera);
			alphaCamera.targetTexture = alpha_buffer;
			alphaCamera.SetReplacementShader(DrawAlphaShader, "OutlineType");
			alphaCamera.Render();
		}

		protected virtual void PrepareDistortion()
		{
			if (!(_distortionCamera == null))
			{
				_distortionCamera.CopyFrom(_camera);
				_distortionCamera.backgroundColor = DistortionMapClearColor;
				_distortionCamera.SetTargetBuffers(distortionMap.content.colorBuffer, distortionMap.content.depthBuffer);
				if (UseDepthTest)
				{
					_distortionCamera.clearFlags = CameraClearFlags.Color;
					_distortionCamera.cullingMask = DepthCullingMask;
					_distortionCamera.RenderWithShader(DrawDepthShader, string.Empty);
					_distortionCamera.clearFlags = CameraClearFlags.Nothing;
					_distortionCamera.cullingMask = DistortionCullingMask;
					_distortionCamera.RenderWithShader(DistortionMapNormShader, "Distortion");
				}
				else
				{
					_distortionCamera.clearFlags = CameraClearFlags.Color;
					_distortionCamera.cullingMask = DistortionCullingMask;
					_distortionCamera.RenderWithShader(DistortionMapNormShader, "Distortion");
				}
			}
		}

		public virtual void OnPreRender()
		{
			if (WriteDepthTexture)
			{
				_camera.depthTextureMode = DepthTextureMode.Depth;
			}
			else
			{
				_camera.depthTextureMode = DepthTextureMode.None;
			}
		}

		public void DoPostProcess(RenderTexture source, RenderTexture destination)
		{
			if (!CheckResources())
			{
				Graphics.Blit(source, destination);
				return;
			}
			if (converted2DLut == null && UseColorGrading)
			{
				if (sourceLut3D == null)
				{
					SetIdentityLut();
				}
				else
				{
					Convert(sourceLut3D, string.Empty);
				}
			}
			if (FastMode)
			{
				if (SepiaColor.a != 0f)
				{
					GlareCompositionEx.SetVector(HASH_SEPIA_COLOR, SepiaColor);
					GlareCompositionEx.SetTexture(HASH_MAIN_TEX, source);
					Graphics.Blit(null, destination, GlareCompositionEx, 5);
				}
				else
				{
					Graphics.Blit(source, destination);
				}
				return;
			}
			aspectRatio = source.width;
			aspectRatio /= source.height;
			if (HDRBuffer)
			{
				internalBufferFormat = RenderTextureFormat.ARGBHalf;
			}
			else
			{
				internalBufferFormat = RenderTextureFormat.ARGB32;
			}
			CreateRenderTextures(source.width, source.height);
			if (WriteAlpha)
			{
				PrepareAlpha();
			}
			if (UseDistortion)
			{
				PrepareDistortion();
			}
			DownSample4X.SetVector(HASH_TEXEL_SIZE, new Vector2(1f / (float)source.width, 1f / (float)source.height));
			Graphics.Blit(source, downsample4X_buffer, DownSample4X, 0);
			DownSample4X.SetVector(HASH_TEXEL_SIZE, new Vector2(0.5f / (float)downsample4X_buffer.content.width, 0.5f / (float)downsample4X_buffer.content.height));
			Graphics.Blit((RenderTexture)downsample4X_buffer, blur_bufferA[0], DownSample4X, 0);
			extractHL(blur_bufferA[0], compose_buffer[0]);
			blur(compose_buffer[0], blur_bufferA[0], blur_bufferB[0], 0);
			for (int i = 1; i < 4; i++)
			{
				Graphics.Blit((RenderTexture)compose_buffer[i - 1], compose_buffer[i], DownSample, 0);
				blur(compose_buffer[i], blur_bufferA[i], blur_bufferB[i], i);
			}
			ComposeBlur(gauss_buffer);
			if (UseRadialBlur)
			{
				ComposeEffect(source, radial_blur_buffer_temp);
				RadialBlur(radial_blur_buffer_temp, radial_blur_buffer);
			}
			ComposeEffect(source, destination, UseRadialBlur);
			ReleaseRenderTextures();
		}

		protected void extractHL(RenderTexture source, RenderTexture destination)
		{
			BrightPassEx.SetFloat(HASH_THRESHOLD, glareThreshold);
			BrightPassEx.SetFloat(HASH_SCALER_UPPER_CASE, glareScaler);
			destination.DiscardContents();
			Graphics.Blit(source, destination, BrightPassEx, 0);
		}

		protected void blur(RenderTexture source, RenderTexture destination, RenderTexture temp, int level)
		{
			MultipleGaussPassFilter.SetVector(HASH_SCALER_LOWER_CASE, new Vector2((!(1f / aspectRatio > 1f)) ? (1f / aspectRatio) : 1f, 0f));
			Graphics.Blit(source, temp, MultipleGaussPassFilter, level);
			MultipleGaussPassFilter.SetVector(HASH_SCALER_LOWER_CASE, new Vector2(0f, (!(aspectRatio > 1f)) ? aspectRatio : 1f));
			destination.DiscardContents();
			Graphics.Blit(temp, destination, MultipleGaussPassFilter, level);
		}

		protected void ComposeBlur(RenderTexture destination)
		{
			GaussCompositionEx.SetTexture(HASH_MAIN_TEX_0, (RenderTexture)blur_bufferA[0]);
			GaussCompositionEx.SetTexture(HASH_MAIN_TEX_1, (RenderTexture)blur_bufferA[1]);
			GaussCompositionEx.SetTexture(HASH_MAIN_TEX_2, (RenderTexture)blur_bufferA[2]);
			GaussCompositionEx.SetTexture(HASH_MAIN_TEX_3, (RenderTexture)blur_bufferA[3]);
			GaussCompositionEx.SetVector(HASH_COEFF, blurCoeff);
			compose_buffer[0].content.DiscardContents();
			Graphics.Blit(null, destination, GaussCompositionEx, 0);
		}

		protected void RadialBlur(RenderTexture source, RenderTexture destination)
		{
			Vector4 vector = new Vector4(RadialBlurCenter.x, RadialBlurCenter.y, RadialBlurStrenth, RadialBlurScatterScale);
			RadialBlurMat.SetVector(HASH_RADIAL_BLUR_PARAM, vector);
			RadialBlurMat.SetTexture(HASH_MAIN_TEX, source);
			Graphics.Blit(source, destination, RadialBlurMat, 0);
		}

		protected void ComposeEffect(RenderTexture source, RenderTexture destination, bool useRadialBlur_ = false)
		{
			glareCoeff.y = glareIntensity;
			glareCoeff.x = 1f - glareIntensity;
			GlareCompositionEx.SetVector(HASH_COEFF, glareCoeff);
			GlareCompositionEx.SetFloat(HASH_EXPOSURE, Exposure);
			GlareCompositionEx.SetFloat(HASH_CONSTRAST, constrast);
			GlareCompositionEx.SetFloat(HASH_LUM_TRESHOLD, glareThreshold);
			GlareCompositionEx.SetFloat(HASH_LUM_SCALER, glareScaler);
			GlareCompositionEx.SetTexture(HASH_MAIN_TEX, source);
			GlareCompositionEx.SetTexture(HASH_MAIN_TEX_1, (RenderTexture)gauss_buffer);
			if (UseDistortion)
			{
				GlareCompositionEx.SetTexture(HASH_DISTORTION_TEX, (RenderTexture)distortionMap);
			}
			else
			{
				GlareCompositionEx.SetTexture(HASH_DISTORTION_TEX, null);
			}
			if (useRadialBlur_)
			{
				GlareCompositionEx.SetTexture(HASH_RADIAL_BLUR_TEX, (RenderTexture)radial_blur_buffer);
				Vector4 vector = new Vector4(RadialBlurCenter.x, RadialBlurCenter.y, RadialBlurStrenth, RadialBlurScatterScale);
				GlareCompositionEx.SetVector(HASH_RADIAL_BLUR_PARAM, vector);
				GlareCompositionEx.EnableKeyword(RADIAL_BLUR_SHDER_KEYWORD);
			}
			else
			{
				GlareCompositionEx.SetTexture(HASH_RADIAL_BLUR_TEX, null);
				GlareCompositionEx.SetVector(HASH_RADIAL_BLUR_PARAM, Vector4.zero);
				GlareCompositionEx.DisableKeyword(RADIAL_BLUR_SHDER_KEYWORD);
			}
			if (FXAA)
			{
				if (FXAAForceHQ)
				{
					Graphics.Blit(null, destination, GlareCompositionEx, 2);
				}
				else if (SepiaColor.a != 0f)
				{
					GlareCompositionEx.SetVector(HASH_SEPIA_COLOR, SepiaColor);
					Graphics.Blit(null, destination, GlareCompositionEx, 7);
				}
				else
				{
					Graphics.Blit(null, destination, GlareCompositionEx, 1);
				}
			}
			else if (SepiaColor.a != 0f)
			{
				GlareCompositionEx.SetVector(HASH_SEPIA_COLOR, SepiaColor);
				Graphics.Blit(null, destination, GlareCompositionEx, 6);
			}
			else if (UseColorGrading)
			{
				float num = converted2DLut.width;
				float num2 = Mathf.Sqrt(num);
				converted2DLut.wrapMode = TextureWrapMode.Clamp;
				GlareCompositionEx.SetFloat(HASH_SCALE_RG, (num2 - 1f) / num);
				GlareCompositionEx.SetFloat(HASH_DIM, num2);
				GlareCompositionEx.SetFloat(HASH_OFFSET, 1f / (2f * num));
				GlareCompositionEx.SetTexture(HASH_LUT_TEX, converted2DLut);
				Graphics.Blit(null, destination, GlareCompositionEx, 3);
			}
			else if (WriteAlpha)
			{
				GlareCompositionEx.SetFloat(HASH_FX_ALPHA_INTENSITY, fxAlphaIntensity);
				GlareCompositionEx.SetTexture(HASH_ALPHA_TEX, (RenderTexture)alpha_buffer);
				Graphics.Blit(null, destination, GlareCompositionEx, 4);
			}
			else
			{
				Graphics.Blit(null, destination, GlareCompositionEx, 0);
			}
		}
	}
}
