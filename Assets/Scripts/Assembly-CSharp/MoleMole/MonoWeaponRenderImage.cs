using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(RawImage))]
	public class MonoWeaponRenderImage : MonoBehaviour
	{
		public const int WEAPON_LAYER = 26;

		private const string TEXTURE_PATH = "UI/RenderTexture/WeaponRenderTexture";

		private const string CONTAINER_PREFAB_PATH = "UI/Menus/Widget/Storage/Weapon3dModel";

		private const float EVO_OFFSET_Y = 10f;

		private const float EMISSION_SCALE = 0.7f;

		private const float EMISSION_BLOOM_FACTOR_SCALE = 0.7f;

		private WeaponDataItem _weaponData;

		private bool _isStatic;

		private int _index;

		private int _rtWidth = 600;

		private int _rtHeight = 300;

		private RenderTextureFormat _rtFormat;

		private int _rtDepth = 24;

		private GameObject _containerGo;

		private RenderTextureWrapper _renderTexture;

		private Coroutine _setupWeapon3dModelCoroutine;

		private bool _triggerRebindRenderTextureToCamera;

		public void SetupView(WeaponDataItem weaponData, bool isStatic = false, int index = 0)
		{
			_weaponData = weaponData;
			_isStatic = isStatic;
			_index = index;
			CleanUp();
			_setupWeapon3dModelCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(DoSetupWeapon3dModel());
		}

		public void OnDestroy()
		{
			CleanUp();
		}

		public void CleanUp()
		{
			if (_setupWeapon3dModelCoroutine != null)
			{
				if (Singleton<ApplicationManager>.Instance != null)
				{
					Singleton<ApplicationManager>.Instance.StopCoroutine(_setupWeapon3dModelCoroutine);
				}
				_setupWeapon3dModelCoroutine = null;
			}
			if (_containerGo != null)
			{
				Object.Destroy(_containerGo);
				_containerGo = null;
			}
			if (_renderTexture != null)
			{
				GraphicsUtils.ReleaseRenderTexture(_renderTexture);
				_renderTexture = null;
			}
		}

		public void OnDisable()
		{
			if (_containerGo != null)
			{
				_containerGo.gameObject.SetActive(false);
			}
		}

		public void OnEnable()
		{
			if (_containerGo != null)
			{
				_containerGo.gameObject.SetActive(true);
			}
		}

		private void TuneMaterialFloat(Material material, string name, float scale)
		{
			if (material.HasProperty(name))
			{
				material.SetFloat(name, Mathf.Max(1f, material.GetFloat(name) * scale));
			}
		}

		private void SetMaterial(GameObject obj)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				Material[] materials = renderer.materials;
				foreach (Material material in materials)
				{
					bool flag = false;
					if (material.HasProperty("_EmissionOverride"))
					{
						flag = material.GetInt("_EmissionOverride") == 1;
					}
					if (flag)
					{
						if (material.HasProperty("_EOEmissionScaler"))
						{
							material.SetFloat("_EmissionScaler", material.GetFloat("_EOEmissionScaler"));
						}
						if (material.HasProperty("_EOPartialEmissionScaler"))
						{
							material.SetFloat("_PartialEmissionScaler", material.GetFloat("_EOPartialEmissionScaler"));
						}
						if (material.HasProperty("_EOEmissionBloomFactor"))
						{
							material.SetFloat("_EmissionBloomFactor", material.GetFloat("_EOEmissionBloomFactor"));
						}
					}
					else
					{
						TuneMaterialFloat(material, "_EmissionScaler", 0.7f);
						TuneMaterialFloat(material, "_PartialEmissionScaler", 0.7f);
						TuneMaterialFloat(material, "_EmissionBloomFactor", 0.7f);
					}
					if (material.HasProperty("_SPTransition"))
					{
						material.SetFloat("_SPTransition", 0f);
					}
				}
			}
		}

		private IEnumerator DoSetupWeapon3dModel()
		{
			yield return null;
			base.transform.GetComponent<RawImage>().enabled = true;
			float scalerFactor = 1f;
			Canvas canvas = Singleton<MainUIManager>.Instance.SceneCanvas.GetComponent<Canvas>();
			if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
			{
				scalerFactor = canvas.scaleFactor;
			}
			_containerGo = Object.Instantiate(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Storage/Weapon3dModel"));
			Camera camera = _containerGo.transform.Find("WeaponCamera").GetComponent<Camera>();
			_renderTexture = GraphicsUtils.GetRenderTexture((int)((float)_rtWidth * scalerFactor), (int)((float)_rtHeight * scalerFactor), _rtDepth, _rtFormat);
			_renderTexture.onRebindToCameraCallBack = OnRebindToCamera;
			if (_renderTexture.IsValid())
			{
				_renderTexture.BindToCamera(camera);
				_renderTexture.content.filterMode = FilterMode.Bilinear;
				base.transform.GetComponent<RawImage>().texture = (RenderTexture)_renderTexture;
			}
			GameObject weaponGo = Object.Instantiate(Miscs.LoadResource<GameObject>(GetWeaponPrefaPath()));
			SetMaterial(weaponGo);
			weaponGo.transform.SetParent(_containerGo.transform.Find("Weapon"), false);
			weaponGo.SetLayer(26, true);
			SetupWeaponView(weaponGo);
			_containerGo.transform.AddLocalPositionY(10f * (float)_index);
			if (_isStatic)
			{
				camera.Render();
				camera.targetTexture = null;
				_containerGo.SetActive(false);
				Object.Destroy(_containerGo);
			}
		}

		private string GetWeaponPrefaPath()
		{
			return _weaponData.GetPrefabPath();
		}

		private void SetupWeaponView(GameObject weaponGo)
		{
			if ((bool)weaponGo.transform.Find("TransformCopy"))
			{
				Transform transform = weaponGo.transform.Find("TransformCopy");
				weaponGo.transform.localPosition = transform.localPosition;
				weaponGo.transform.localEulerAngles = transform.localEulerAngles;
				weaponGo.transform.localScale = transform.localScale;
			}
			if (weaponGo.transform.Find("ShortSword") != null && weaponGo.transform.Find("LongSword") != null)
			{
				weaponGo.transform.Find("ShortSword").gameObject.SetActive(false);
				weaponGo.transform.Find("LongSword").gameObject.SetActive(true);
			}
		}

		public void Update()
		{
			if (_isStatic && _triggerRebindRenderTextureToCamera && _renderTexture != null && !_renderTexture.IsCreated())
			{
				_triggerRebindRenderTextureToCamera = false;
				CleanUp();
				_setupWeapon3dModelCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(DoSetupWeapon3dModel());
			}
		}

		private void OnRebindToCamera()
		{
			if (_isStatic)
			{
				_triggerRebindRenderTextureToCamera = true;
			}
		}
	}
}
