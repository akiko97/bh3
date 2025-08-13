using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MoleMole
{
	public class MonoBenchmarkSwitches : MonoBehaviour
	{
		public class BenchmarkSwitch
		{
			public string name;

			public Action<bool> toggler;

			protected bool _toggled;

			public BenchmarkSwitch(string name)
			{
				this.name = name;
			}

			public BenchmarkSwitch(string name, Action<bool> toggler)
			{
				this.name = name;
				this.toggler = toggler;
			}

			public virtual void SetEnabled()
			{
				toggler(true);
				_toggled = true;
			}

			public virtual void DrawWidgets()
			{
				if (GUILayout.Button(string.Format("{0}: {1}", name, (!_toggled) ? "<color=red>Disabled</color>" : "Enabled"), GUILayout.Height(50f)))
				{
					_toggled = !_toggled;
					toggler(_toggled);
				}
			}
		}

		public class StageBenchmarkSwitch : BenchmarkSwitch
		{
			private GameObject _stageGO;

			public StageBenchmarkSwitch()
				: base("Stage")
			{
				toggler = SetStageEnable;
			}

			private void SetStageEnable(bool enabled)
			{
				if (_stageGO == null)
				{
					_stageGO = UnityEngine.Object.FindObjectOfType<MonoBasePerpStage>().gameObject;
				}
				_stageGO.SetActive(enabled);
			}
		}

		public class ResolutionBenchmarkSwitch : BenchmarkSwitch
		{
			private Resolution _orig;

			private float[] _scaleList = new float[6] { 1f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f };

			private int _ix;

			public ResolutionBenchmarkSwitch()
				: base("Res Scale")
			{
				_orig = Screen.currentResolution;
			}

			public override void SetEnabled()
			{
				SetResolution();
			}

			public override void DrawWidgets()
			{
				if (GUILayout.Button("Resolution: " + _scaleList[_ix], GUILayout.Height(50f)))
				{
					_ix = (_ix + 1) % _scaleList.Length;
					SetResolution();
				}
			}

			private void SetResolution()
			{
				float num = _scaleList[_ix];
				Screen.SetResolution((int)((float)_orig.width * num), (int)((float)_orig.height * num), Screen.fullScreen);
			}
		}

		public class DofRefSizeBenchmarkSwitch : BenchmarkSwitch
		{
			private Resolution _orig;

			private int[] _sizeList = new int[6] { 1080, 1008, 936, 864, 792, 720 };

			private int _ix;

			public DofRefSizeBenchmarkSwitch()
				: base("Dof Size")
			{
			}

			public override void SetEnabled()
			{
				SetSize();
			}

			public override void DrawWidgets()
			{
				if (GUILayout.Button("Dof Size: " + _sizeList[_ix], GUILayout.Height(50f)))
				{
					_ix = (_ix + 1) % _sizeList.Length;
					SetSize();
				}
			}

			private void SetSize()
			{
				MainMenuCamera mainMenuCamera = UnityEngine.Object.FindObjectOfType<MainMenuCamera>();
				if (mainMenuCamera != null)
				{
					mainMenuCamera.ReferencedBufferHeight = _sizeList[_ix];
				}
			}
		}

		public class MaterialShaderBenchmarkSwitch : BenchmarkSwitch
		{
			public MaterialShaderBenchmarkSwitch()
				: base("Use Simple Shader")
			{
				toggler = SetSimpleShader;
			}

			private void SetSimpleShader(bool enabled)
			{
				if (enabled || !_toggled)
				{
					return;
				}
				Shader shader = Shader.Find("Unlit/Texture");
				Renderer[] array = UnityEngine.Object.FindObjectsOfType<Renderer>();
				foreach (Renderer renderer in array)
				{
					Material[] sharedMaterials = renderer.sharedMaterials;
					foreach (Material material in sharedMaterials)
					{
						material.shader = shader;
					}
				}
			}

			public override void DrawWidgets()
			{
				if (GUILayout.Button((!_toggled) ? "<color=red>Use Simple Shader</color>" : "Use Simple Shader", GUILayout.Height(50f)))
				{
					toggler(false);
					_toggled = false;
				}
			}
		}

		public class MaterialTextureBenchmarkSwitch : BenchmarkSwitch
		{
			private Texture2D[] _texes;

			public MaterialTextureBenchmarkSwitch(Texture2D[] simpleTexes)
				: base("Use Simple Texture")
			{
				toggler = SetSimpleTexture;
				_texes = simpleTexes;
			}

			private void SetSimpleTexture(bool enabled)
			{
				if (enabled || !_toggled)
				{
					return;
				}
				Shader shader = Shader.Find("Unlit/Texture");
				int num = 0;
				Renderer[] array = UnityEngine.Object.FindObjectsOfType<Renderer>();
				foreach (Renderer renderer in array)
				{
					Material[] sharedMaterials = renderer.sharedMaterials;
					foreach (Material material in sharedMaterials)
					{
						material.shader = shader;
						material.mainTexture = _texes[num % _texes.Length];
						num++;
					}
				}
			}

			public override void DrawWidgets()
			{
				if (GUILayout.Button((!_toggled) ? "<color=red>Use Simple Shader&Tex</color>" : "Use Simple Shader&Tex", GUILayout.Height(50f)))
				{
					toggler(false);
					_toggled = false;
				}
			}
		}

		public class ResScaleBenchmarkSwitch : BenchmarkSwitch
		{
			private PostFXWithResScale.CAMERA_RES_SCALE[] _scaleList = new PostFXWithResScale.CAMERA_RES_SCALE[7]
			{
				PostFXWithResScale.CAMERA_RES_SCALE.RES_100,
				PostFXWithResScale.CAMERA_RES_SCALE.RES_90,
				PostFXWithResScale.CAMERA_RES_SCALE.RES_80,
				PostFXWithResScale.CAMERA_RES_SCALE.RES_70,
				PostFXWithResScale.CAMERA_RES_SCALE.RES_60,
				PostFXWithResScale.CAMERA_RES_SCALE.RES_50,
				PostFXWithResScale.CAMERA_RES_SCALE.RES_25
			};

			private int _ix;

			public ResScaleBenchmarkSwitch()
				: base("Res Scale")
			{
			}

			public override void SetEnabled()
			{
				SetScale();
			}

			public override void DrawWidgets()
			{
				if (GUILayout.Button("Res Scale: " + _scaleList[_ix], GUILayout.Height(50f)))
				{
					_ix = (_ix + 1) % _scaleList.Length;
					SetScale();
				}
			}

			private void SetScale()
			{
				PostFXWithResScale postFXWithResScale = UnityEngine.Object.FindObjectOfType<PostFXWithResScale>();
				if (postFXWithResScale != null)
				{
					postFXWithResScale.CameraResScale = _scaleList[_ix];
				}
			}
		}

		public class RadialBlurResBenchmarkSwitch : BenchmarkSwitch
		{
			private PostFXBase.SizeDownScaleEnum[] _scaleList = new PostFXBase.SizeDownScaleEnum[8]
			{
				PostFXBase.SizeDownScaleEnum.Div_5,
				PostFXBase.SizeDownScaleEnum.Div_6,
				PostFXBase.SizeDownScaleEnum.Div_7,
				PostFXBase.SizeDownScaleEnum.Div_8,
				PostFXBase.SizeDownScaleEnum.Div_1,
				PostFXBase.SizeDownScaleEnum.Div_2,
				PostFXBase.SizeDownScaleEnum.Div_3,
				PostFXBase.SizeDownScaleEnum.Div_4
			};

			private int _ix;

			public RadialBlurResBenchmarkSwitch()
				: base("RB Res Scale")
			{
			}

			public override void SetEnabled()
			{
				SetScale();
			}

			public override void DrawWidgets()
			{
				if (GUILayout.Button("Res Scale: " + _scaleList[_ix], GUILayout.Height(50f)))
				{
					_ix = (_ix + 1) % _scaleList.Length;
					SetScale();
				}
			}

			private void SetScale()
			{
				PostFX postFX = UnityEngine.Object.FindObjectOfType<PostFX>();
				if (postFX != null)
				{
					postFX.RadialBlurDownScale = _scaleList[_ix];
				}
			}
		}

		public class BufferSizeBenchmarkSwitch : BenchmarkSwitch
		{
			private int _ix;

			private PostFXBase.InternalBufferSizeEnum[] _bufferSizes = new PostFXBase.InternalBufferSizeEnum[3]
			{
				PostFXBase.InternalBufferSizeEnum.SIZE_128,
				PostFXBase.InternalBufferSizeEnum.SIZE_256,
				PostFXBase.InternalBufferSizeEnum.SIZE_512
			};

			public BufferSizeBenchmarkSwitch()
				: base("PostFX Buffer Size")
			{
			}

			public override void SetEnabled()
			{
				SetSize();
			}

			public override void DrawWidgets()
			{
				if (GUILayout.Button("Buffer Size: " + _bufferSizes[_ix], GUILayout.Height(50f)))
				{
					_ix = (_ix + 1) % _bufferSizes.Length;
					SetSize();
				}
			}

			private void SetSize()
			{
				PostFXBase postFXBase = UnityEngine.Object.FindObjectOfType<PostFXBase>();
				if (!(postFXBase == null))
				{
					postFXBase.internalBufferSize = _bufferSizes[_ix];
				}
			}
		}

		public class UseResScaleFXSwitch : BenchmarkSwitch
		{
			private List<BenchmarkSwitch> _switches;

			private MonoBenchmarkSwitches _bench;

			public UseResScaleFXSwitch(List<BenchmarkSwitch> switches, MonoBenchmarkSwitches bench)
				: base("Use Res Scale")
			{
				_switches = switches;
				_bench = bench;
			}

			public override void SetEnabled()
			{
			}

			public override void DrawWidgets()
			{
				if (GUILayout.Button((!_toggled) ? "Use Res Scale" : "<color=red>Use Res Scale</color>", GUILayout.Height(50f)) && !_toggled)
				{
					_toggled = true;
					_bench.StartCoroutine(ChangePostFXIter());
				}
			}

			private IEnumerator ChangePostFXIter()
			{
				yield return new WaitForEndOfFrame();
				PostFXBase postFX = UnityEngine.Object.FindObjectOfType<PostFXBase>();
				if (postFX == null)
				{
					yield break;
				}
				GameObject postFXGo = postFX.gameObject;
				Shader distortionApply = postFX.DistortionApplyShader;
				Shader distortionNorm = postFX.DistortionMapNormShader;
				Shader drawDepth = postFX.DrawDepthShader;
				Shader drawAlpha = postFX.DrawAlphaShader;
				Shader down4x = postFX.DownSample4XShader;
				Shader down = postFX.DownSampleShader;
				Shader bright = postFX.BrightPassExShader;
				Shader gaussComp = postFX.GaussCompositionExShader;
				Shader glareCompo = postFX.GlareCompositionExShader;
				Shader mp1 = postFX.MultipleGaussPassFilterShader_128;
				Shader mp2 = postFX.MultipleGaussPassFilterShader_256;
				Shader mp3 = postFX.MultipleGaussPassFilterShader_512;
				UnityEngine.Object.DestroyImmediate(postFX);
				yield return null;
				PostFXWithResScale resScale = postFXGo.AddComponent<PostFXWithResScale>();
				resScale.DistortionApplyShader = distortionApply;
				resScale.DistortionMapNormShader = distortionNorm;
				resScale.DrawDepthShader = drawDepth;
				resScale.DrawAlphaShader = drawAlpha;
				resScale.DownSample4XShader = down4x;
				resScale.DownSampleShader = down;
				resScale.BrightPassExShader = bright;
				resScale.GaussCompositionExShader = gaussComp;
				resScale.GlareCompositionExShader = glareCompo;
				resScale.MultipleGaussPassFilterShader_128 = mp1;
				resScale.MultipleGaussPassFilterShader_256 = mp2;
				resScale.MultipleGaussPassFilterShader_512 = mp3;
				resScale.enabled = false;
				yield return null;
				resScale.enabled = true;
				foreach (BenchmarkSwitch swc in _switches)
				{
					if (swc is ResScaleBenchmarkSwitch)
					{
						yield break;
					}
				}
				_switches.Add(new ResScaleBenchmarkSwitch());
			}
		}

		public class NoFPSLimitSwitch : BenchmarkSwitch
		{
			public NoFPSLimitSwitch()
				: base("No FPS Limit")
			{
				toggler = NoLimitToggle;
			}

			private void NoLimitToggle(bool enabled)
			{
				if (!enabled && _toggled)
				{
					Application.targetFrameRate = 99999999;
					QualitySettings.vSyncCount = 0;
				}
			}

			public override void DrawWidgets()
			{
				if (GUILayout.Button((!_toggled) ? "<color=red>No FPS Limit</color>" : "No FPS Limit", GUILayout.Height(50f)))
				{
					toggler(false);
					_toggled = false;
				}
			}
		}

		private const int BUTTONS_PER_COLUMN = 8;

		protected bool _toggled;

		private GUIStyle _style;

		private Texture2D[] _simpleTextures;

		private bool _isDisableEventSystem;

		protected List<BenchmarkSwitch> _switches;

		private void Start()
		{
			_switches = new List<BenchmarkSwitch>();
			_switches.Add(new BenchmarkSwitch("Post FX", delegate(bool enabled)
			{
				GraphicsSettingUtil.EnablePostFX(enabled);
			}));
			_switches.Add(new BenchmarkSwitch("HDR", GraphicsSettingUtil.EnableHDR));
			_switches.Add(new BenchmarkSwitch("FXAA", GraphicsSettingUtil.EnableFXAA));
			_switches.Add(new BenchmarkSwitch("Distortion", GraphicsSettingUtil.EnableDistortion));
			_switches.Add(new BenchmarkSwitch("Not Fast Mode", delegate(bool enabled)
			{
				PostFXBase postFXBase = UnityEngine.Object.FindObjectOfType<PostFXBase>();
				if (postFXBase != null)
				{
					postFXBase.FastMode = !enabled;
				}
			}));
			_switches.Add(new BufferSizeBenchmarkSwitch());
			_switches.Add(new BenchmarkSwitch("ColorGrading", GraphicsSettingUtil.EnableColorGrading));
			_switches.Add(new BenchmarkSwitch("Reflection", GraphicsSettingUtil.EnableReflection));
			_switches.Add(new BenchmarkSwitch("DynBone", GraphicsSettingUtil.EnableDynamicBone));
			_switches.Add(new BenchmarkSwitch("60FPS", delegate(bool enabled)
			{
				if (enabled)
				{
					Application.targetFrameRate = 60;
				}
				else
				{
					Application.targetFrameRate = 30;
				}
			}));
			_switches.Add(new BenchmarkSwitch("UI Camera", MakeUICameraEnabledToggler()));
			_switches.Add(new BenchmarkSwitch("UI Camera Clear", MakeUICameraClearToggler()));
			_switches.Add(new BenchmarkSwitch("UI EventSystem", MakeEventSystemEnabledToggler()));
			_switches.Add(new BenchmarkSwitch("DamageText", MakeDamageTextToggler()));
			_switches.Add(new BenchmarkSwitch("InlevelLock", MakeInlevelLockTogger()));
			if (UnityEngine.Object.FindObjectOfType<MonoBasePerpStage>() != null)
			{
				_switches.Add(new StageBenchmarkSwitch());
			}
			_switches.Add(new BenchmarkSwitch("SkinMeshRenderer", delegate(bool enabled)
			{
				SkinnedMeshRenderer[] array = UnityEngine.Object.FindObjectsOfType<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
				{
					skinnedMeshRenderer.enabled = enabled;
				}
			}));
			_switches.Add(new BenchmarkSwitch("MeshRenderer", delegate(bool enabled)
			{
				MeshRenderer[] array = UnityEngine.Object.FindObjectsOfType<MeshRenderer>();
				foreach (MeshRenderer meshRenderer in array)
				{
					meshRenderer.enabled = enabled;
				}
			}));
			_switches.Add(new BenchmarkSwitch("ParticleSystem", delegate(bool enabled)
			{
				ParticleSystem[] array = UnityEngine.Object.FindObjectsOfType<ParticleSystem>();
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.gameObject.SetActive(enabled);
				}
				if (Singleton<EffectManager>.Instance != null)
				{
					Singleton<EffectManager>.Instance.mute = !enabled;
				}
			}));
			_switches.Add(new BenchmarkSwitch("Collider", delegate(bool enabled)
			{
				Collider[] array = UnityEngine.Object.FindObjectsOfType<Collider>();
				foreach (Collider collider in array)
				{
					collider.enabled = enabled;
				}
			}));
			_switches.Add(new BenchmarkSwitch("Animator", delegate(bool enabled)
			{
				Animator[] array = UnityEngine.Object.FindObjectsOfType<Animator>();
				foreach (Animator animator in array)
				{
					animator.enabled = enabled;
					if (!enabled)
					{
						Rigidbody component = animator.GetComponent<Rigidbody>();
						component.velocity = Vector3.zero;
						component.angularVelocity = Vector3.zero;
					}
				}
			}));
			if (UnityEngine.Object.FindObjectOfType<BehaviorTree>() != null)
			{
				_switches.Add(new BenchmarkSwitch("AI", delegate(bool enabled)
				{
					if (!enabled)
					{
						BehaviorManager.instance.UpdateInterval = UpdateIntervalType.Manual;
					}
					else
					{
						BehaviorManager.instance.UpdateInterval = UpdateIntervalType.EveryFrame;
					}
				}));
			}
			if (Singleton<EffectManager>.Instance != null)
			{
				_switches.Add(new BenchmarkSwitch("Effects", delegate(bool enabled)
				{
					Singleton<EffectManager>.Instance.mute = !enabled;
				}));
			}
			_switches.Add(new BenchmarkSwitch("FixedUpdates", delegate(bool enabled)
			{
				Time.fixedDeltaTime = ((!enabled) ? 1f : 0.02f);
			}));
			_switches.Add(new ResolutionBenchmarkSwitch());
			_switches.Add(new DofRefSizeBenchmarkSwitch());
			_switches.Add(new BenchmarkSwitch("Light", MakeComponentEnabledToggler<Light>()));
			_switches.Add(new BenchmarkSwitch("Quality Bone", delegate(bool enabled)
			{
				SkinnedMeshRenderer[] array = UnityEngine.Object.FindObjectsOfType<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
				{
					if (enabled)
					{
						skinnedMeshRenderer.quality = SkinQuality.Auto;
					}
					else
					{
						skinnedMeshRenderer.quality = SkinQuality.Bone1;
					}
				}
			}));
			_switches.Add(new MaterialShaderBenchmarkSwitch());
			_switches.Add(new MaterialTextureBenchmarkSwitch(_simpleTextures));
			if (UnityEngine.Object.FindObjectOfType<PostFXWithResScale>() != null)
			{
				_switches.Add(new ResScaleBenchmarkSwitch());
			}
			_switches.Add(new UseResScaleFXSwitch(_switches, this));
			_switches.Add(new NoFPSLimitSwitch());
			_switches.Add(new BenchmarkSwitch("Radial Blur", delegate(bool enabled)
			{
				PostFX postFX = UnityEngine.Object.FindObjectOfType<PostFX>();
				if (postFX != null)
				{
					if (enabled)
					{
						postFX.DisableRadialBlur = false;
						postFX.RadialBlurCenter = new Vector2(0.5f, 0.5f);
						postFX.RadialBlurScatterScale = 0.3f;
						postFX.RadialBlurStrenth = 2f;
					}
					else
					{
						postFX.RadialBlurStrenth = 0f;
					}
				}
			}));
			if (UnityEngine.Object.FindObjectOfType<PostFX>() != null)
			{
				_switches.Add(new RadialBlurResBenchmarkSwitch());
			}
			foreach (BenchmarkSwitch @switch in _switches)
			{
				@switch.SetEnabled();
			}
		}

		private Action<bool> MakeComponentEnabledToggler<T>() where T : Behaviour
		{
			return delegate(bool enabled)
			{
				T[] array = UnityEngine.Object.FindObjectsOfType<T>();
				for (int i = 0; i < array.Length; i++)
				{
					T val = array[i];
					val.enabled = enabled;
				}
			};
		}

		private Action<bool> MakeComponentEnabledToggler<T1, T2>() where T1 : Behaviour where T2 : Behaviour
		{
			return delegate(bool enabled)
			{
				T1[] array = UnityEngine.Object.FindObjectsOfType<T1>();
				for (int i = 0; i < array.Length; i++)
				{
					T1 val = array[i];
					T2 component = val.GetComponent<T2>();
					component.enabled = enabled;
				}
			};
		}

		protected Action<bool> MakeUICameraEnabledToggler()
		{
			return delegate(bool enabled)
			{
				Camera[] array = UnityEngine.Object.FindObjectsOfType<Camera>();
				foreach (Camera camera in array)
				{
					if (camera.gameObject.name == "UICamera")
					{
						camera.enabled = enabled;
					}
				}
				MonoInLevelUICamera[] array2 = UnityEngine.Object.FindObjectsOfType<MonoInLevelUICamera>();
				foreach (MonoInLevelUICamera monoInLevelUICamera in array2)
				{
					monoInLevelUICamera.GetComponent<Camera>().enabled = enabled;
				}
			};
		}

		protected Action<bool> MakeDamageTextToggler()
		{
			return delegate(bool enabled)
			{
				GlobalVars.muteDamageText = !enabled;
			};
		}

		protected Action<bool> MakeInlevelLockTogger()
		{
			return delegate(bool enabled)
			{
				GlobalVars.muteInlevelLock = !enabled;
			};
		}

		protected Action<bool> MakeUICameraClearToggler()
		{
			return delegate(bool enabled)
			{
				Camera[] array = UnityEngine.Object.FindObjectsOfType<Camera>();
				foreach (Camera camera in array)
				{
					if (camera.gameObject.name == "UICamera")
					{
						camera.clearFlags = ((!enabled) ? CameraClearFlags.Nothing : CameraClearFlags.Depth);
					}
				}
				MonoInLevelUICamera[] array2 = UnityEngine.Object.FindObjectsOfType<MonoInLevelUICamera>();
				foreach (MonoInLevelUICamera monoInLevelUICamera in array2)
				{
					monoInLevelUICamera.GetComponent<Camera>().clearFlags = ((!enabled) ? CameraClearFlags.Nothing : CameraClearFlags.Depth);
				}
			};
		}

		protected void AddUIPageTogglers(Dictionary<string, string[]> pageDict)
		{
			BaseMonoCanvas baseMonoCanvas = UnityEngine.Object.FindObjectOfType<BaseMonoCanvas>();
			if (baseMonoCanvas == null)
			{
				return;
			}
			foreach (KeyValuePair<string, string[]> item in pageDict)
			{
				Transform[] trsfs = new Transform[item.Value.Length];
				for (int i = 0; i < trsfs.Length; i++)
				{
					trsfs[i] = baseMonoCanvas.transform.Find(item.Value[i]);
				}
				_switches.Add(new BenchmarkSwitch("UI " + item.Key, delegate(bool enabled)
				{
					Transform[] array = trsfs;
					foreach (Transform transform in array)
					{
						if (transform != null)
						{
							transform.gameObject.SetActive(enabled);
						}
					}
				}));
			}
		}

		private Action<bool> MakeEventSystemEnabledToggler()
		{
			return delegate(bool enabled)
			{
				if (!enabled)
				{
					_isDisableEventSystem = true;
				}
				EventSystem[] array = UnityEngine.Object.FindObjectsOfType<EventSystem>();
				foreach (EventSystem eventSystem in array)
				{
					eventSystem.enabled = enabled;
				}
			};
		}

		private void Awake()
		{
			_simpleTextures = new Texture2D[4];
			Color[] array = new Color[4]
			{
				Color.red,
				Color.green,
				Color.blue,
				Color.yellow
			};
			for (int i = 0; i < _simpleTextures.Length; i++)
			{
				_simpleTextures[i] = new Texture2D(16, 16);
				Color[] array2 = new Color[256];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = array[i];
				}
				_simpleTextures[i].SetPixels(array2);
			}
		}

		private void OnEnable()
		{
			_style = new GUIStyle();
			Texture2D texture2D = new Texture2D(16, 16);
			Color[] array = new Color[256];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Color.gray;
			}
			texture2D.SetPixels(array);
			_style.normal.background = texture2D;
		}

		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(_style.normal.background);
		}

		private void OnGUI()
		{
			if (_toggled)
			{
				WidgetSwitches();
				if (GUI.Button(new Rect((float)Screen.width * 0.85f, (float)Screen.height * 0.85f, 60f, 60f), "CLOSE"))
				{
					_toggled = false;
					if (!_isDisableEventSystem)
					{
						SetEventSystemEnable(true);
					}
				}
				if (GUI.Button(new Rect((float)Screen.width * 0.93f, (float)Screen.height * 0.85f, 60f, 60f), "Return"))
				{
					MonoDevLevel monoDevLevel = UnityEngine.Object.FindObjectOfType<MonoDevLevel>();
					if (monoDevLevel != null)
					{
						monoDevLevel.gameObject.SetActive(false);
					}
					Canvas[] array = UnityEngine.Object.FindObjectsOfType<Canvas>();
					Canvas[] array2 = array;
					foreach (Canvas canvas in array2)
					{
						canvas.gameObject.SetActive(false);
					}
					if (Singleton<AvatarManager>.Instance != null)
					{
						Singleton<AvatarManager>.Instance.RemoveAllAvatars();
					}
					if (Singleton<MonsterManager>.Instance != null)
					{
						Singleton<MonsterManager>.Instance.RemoveAllMonsters();
					}
					if (Singleton<PropObjectManager>.Instance != null)
					{
						Singleton<PropObjectManager>.Instance.RemoveAllPropObjects();
					}
					if (Singleton<DynamicObjectManager>.Instance != null)
					{
						Singleton<DynamicObjectManager>.Instance.RemoveAllDynamicObjects();
					}
					if (Singleton<CameraManager>.Instance != null)
					{
						Singleton<CameraManager>.Instance.RemoveAllCameras();
					}
					GeneralLogicManager.DestroyAll();
					if (Singleton<LevelScoreManager>.Instance != null)
					{
						Singleton<LevelScoreManager>.Destroy();
					}
					SceneManager.LoadScene("DevLevelDeploy");
					Resources.UnloadUnusedAssets();
					GC.Collect();
					GC.WaitForPendingFinalizers();
				}
				GUI.Label(new Rect(20f, (float)Screen.height * 0.85f, 200f, 60f), "GraphicAPI: " + SystemInfo.graphicsDeviceType);
			}
			else if (GUI.Button(new Rect((float)Screen.width * 0.5f, (float)Screen.height * 0.15f, 80f, 30f), "Benchmark"))
			{
				_toggled = true;
				SetEventSystemEnable(false);
			}
		}

		protected void WidgetSwitches()
		{
			GUI.color = Color.white;
			GUI.backgroundColor = Color.gray;
			bool flag = false;
			for (int i = 0; i < _switches.Count; i++)
			{
				if (i % 8 == 0)
				{
					GUILayout.BeginArea(new Rect(Mathf.Floor(i / 8) * 240f, 10f, 250f, Screen.height - 20), _style);
					flag = true;
				}
				_switches[i].DrawWidgets();
				if (i % 8 == 7)
				{
					GUILayout.EndArea();
					flag = false;
				}
			}
			if (flag)
			{
				GUILayout.EndArea();
			}
		}

		protected void SetEventSystemEnable(bool enabled)
		{
			EventSystem eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
			if (eventSystem != null)
			{
				eventSystem.enabled = enabled;
			}
		}
	}
}
