using System.Collections.Generic;
using MoleMole;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/RectMask")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class RectMask : MonoBehaviour
	{
		private class MatEntry
		{
			public Material baseMat;

			public Material customMat;

			public int count;
		}

		[SerializeField]
		private float _leftTransition;

		[SerializeField]
		private float _bottomTransition;

		[SerializeField]
		private float _rightTransition;

		[SerializeField]
		private float _topTransition;

		private static readonly string KEYWORD_TOGGLE = "RECT_MASK";

		private static readonly string PROPERTY_TOGGLE = "_RectMask";

		private static readonly string PROPERTY_RECT = "_RMRect";

		private static readonly string PROPERTY_TRANSITION_WIDTH = "_RMTransitWidth";

		private int _propertyIdToggle;

		private int _propertyIdRect;

		private int _propertyIdTransitionWidth;

		private static readonly string DEFAULT_SHADER_NAME = "UI/Default";

		private static readonly string RECTMASK_DEFAULT_SHADER_NAME = "miHoYo/UI/Default";

		private static Shader RECTMASK_DEFAULT_SHADER;

		private List<MatEntry> _matList = new List<MatEntry>();

		private RectTransform _transform;

		private Canvas _rootCanvas;

		private HashSet<MaskableGraphic> _graphicSet;

		private bool _isMaterialDirty;

		private bool _isGraphicDirty;

		public float leftTransition
		{
			get
			{
				return _leftTransition;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetStruct(ref _leftTransition, value))
				{
					SetMaterialDirty();
				}
			}
		}

		public float bottomTransition
		{
			get
			{
				return _bottomTransition;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetStruct(ref _bottomTransition, value))
				{
					SetMaterialDirty();
				}
			}
		}

		public float rightTransition
		{
			get
			{
				return _rightTransition;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetStruct(ref _rightTransition, value))
				{
					SetMaterialDirty();
				}
			}
		}

		public float topTransition
		{
			get
			{
				return _topTransition;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetStruct(ref _topTransition, value))
				{
					SetMaterialDirty();
				}
			}
		}

		private Shader _rectmaskDefaultShader
		{
			get
			{
				if (RECTMASK_DEFAULT_SHADER == null)
				{
					RECTMASK_DEFAULT_SHADER = Shader.Find(RECTMASK_DEFAULT_SHADER_NAME);
					if (RECTMASK_DEFAULT_SHADER == null || !RECTMASK_DEFAULT_SHADER.isSupported)
					{
						Debug.LogError(string.Format("Shader '{0}' fail to load", RECTMASK_DEFAULT_SHADER_NAME));
						base.enabled = false;
					}
				}
				return RECTMASK_DEFAULT_SHADER;
			}
		}

		private Vector4 _rect
		{
			get
			{
				Vector3[] array = new Vector3[4];
				_transform.GetWorldCorners(array);
				if (_rootCanvas.renderMode == RenderMode.ScreenSpaceCamera)
				{
					Camera worldCamera = _rootCanvas.worldCamera;
					array[0] = worldCamera.WorldToViewportPoint(array[0]);
					array[2] = worldCamera.WorldToViewportPoint(array[2]);
				}
				return new Vector4(array[0].x, array[0].y, array[2].x, array[2].y);
			}
		}

		private Vector4 _transitions
		{
			get
			{
				Vector4 result = new Vector4(_leftTransition, _bottomTransition, _rightTransition, _topTransition);
				if (_rootCanvas.renderMode == RenderMode.ScreenSpaceCamera)
				{
					Camera worldCamera = _rootCanvas.worldCamera;
					result.x /= worldCamera.pixelWidth;
					result.z /= worldCamera.pixelWidth;
					result.y /= worldCamera.pixelHeight;
					result.w /= worldCamera.pixelHeight;
				}
				return result;
			}
		}

		private bool HasMaterial(Material customMat)
		{
			for (int i = 0; i < _matList.Count; i++)
			{
				MatEntry matEntry = _matList[i];
				if (matEntry.customMat == customMat)
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateAllMaterial()
		{
			for (int i = 0; i < _matList.Count; i++)
			{
				MatEntry matEntry = _matList[i];
				matEntry.customMat.SetVector(_propertyIdRect, _rect);
				matEntry.customMat.SetVector(_propertyIdTransitionWidth, _transitions);
			}
		}

		private Material AddMaterial(Material baseMat)
		{
			if (baseMat.shader.name != DEFAULT_SHADER_NAME)
			{
				if (!baseMat.HasProperty(_propertyIdToggle))
				{
					Debug.LogWarning("Material " + baseMat.name + " doesn't have " + PROPERTY_TOGGLE + " property", baseMat);
					return baseMat;
				}
				if (!baseMat.HasProperty(_propertyIdRect))
				{
					Debug.LogWarning("Material " + baseMat.name + " doesn't have " + PROPERTY_RECT + " property", baseMat);
					return baseMat;
				}
				if (!baseMat.HasProperty(_propertyIdTransitionWidth))
				{
					Debug.LogWarning("Material " + baseMat.name + " doesn't have " + PROPERTY_TRANSITION_WIDTH + " property", baseMat);
					return baseMat;
				}
			}
			for (int i = 0; i < _matList.Count; i++)
			{
				MatEntry matEntry = _matList[i];
				if (matEntry.baseMat == baseMat)
				{
					matEntry.count++;
					return matEntry.customMat;
				}
			}
			MatEntry matEntry2 = new MatEntry();
			matEntry2.count = 1;
			matEntry2.baseMat = baseMat;
			matEntry2.customMat = new Material(baseMat);
			matEntry2.customMat.name = string.Format("{0} (RectMask Instance)", baseMat.name);
			if (matEntry2.customMat.shader.name == DEFAULT_SHADER_NAME)
			{
				matEntry2.customMat.shader = _rectmaskDefaultShader;
			}
			matEntry2.customMat.SetInt(_propertyIdToggle, 1);
			matEntry2.customMat.EnableKeyword(KEYWORD_TOGGLE);
			matEntry2.customMat.SetVector(_propertyIdRect, _rect);
			matEntry2.customMat.SetVector(_propertyIdTransitionWidth, _transitions);
			_matList.Add(matEntry2);
			return matEntry2.customMat;
		}

		private Material RemoveMaterial(Material customMat)
		{
			if (customMat == null)
			{
				return null;
			}
			for (int i = 0; i < _matList.Count; i++)
			{
				MatEntry matEntry = _matList[i];
				if (!(matEntry.customMat != customMat))
				{
					Material result = null;
					if (--matEntry.count == 0)
					{
						Object.Destroy(matEntry.customMat);
						result = matEntry.baseMat;
						matEntry.baseMat = null;
						_matList.RemoveAt(i);
					}
					return result;
				}
			}
			return null;
		}

		private void ClearAllMaterial()
		{
			for (int i = 0; i < _matList.Count; i++)
			{
				MatEntry matEntry = _matList[i];
				Object.Destroy(matEntry.customMat);
				matEntry.baseMat = null;
			}
			_matList.Clear();
		}

		private void Awake()
		{
			_propertyIdToggle = Shader.PropertyToID(PROPERTY_TOGGLE);
			_propertyIdRect = Shader.PropertyToID(PROPERTY_RECT);
			_propertyIdTransitionWidth = Shader.PropertyToID(PROPERTY_TRANSITION_WIDTH);
			_graphicSet = new HashSet<MaskableGraphic>();
			_transform = GetComponent<RectTransform>();
			RectTransform.reapplyDrivenProperties += OnTransformReapplyDrivenProperties;
			_rootCanvas = Singleton<MainUIManager>.Instance.SceneCanvas.GetComponent<Canvas>();
			if (_rootCanvas == null)
			{
				Debug.LogError("Cannot find root Canvas.", this);
				base.enabled = false;
			}
		}

		private void UpdateMask()
		{
			if (_isGraphicDirty)
			{
				ClearAllGraphics();
				SetupAllGraphics();
				_isGraphicDirty = false;
				_isMaterialDirty = false;
			}
			else if (_isMaterialDirty)
			{
				UpdateAllMaterial();
				SetAllGraphicMaterials();
				_isMaterialDirty = false;
			}
		}

		private void Update()
		{
			UpdateMask();
		}

		private void LateUpdate()
		{
			UpdateMask();
		}

		private void OnEnable()
		{
			SetGraphicDirty();
		}

		private void OnDisable()
		{
			ClearAllGraphics();
		}

		public void SetGraphicDirty()
		{
			_isGraphicDirty = true;
		}

		public void SetMaterialDirty()
		{
			_isMaterialDirty = true;
		}

		public void OnTransformReapplyDrivenProperties(RectTransform driven)
		{
			if (driven == _transform)
			{
				SetMaterialDirty();
			}
		}

		private void SetupAllGraphics()
		{
			List<MaskableGraphic> list = UnityEngine.UI.ListPool<MaskableGraphic>.Get();
			Transform obj = base.transform;
			List<MaskableGraphic> result = list;
			obj.GetComponentsInChildren(true, result);
			for (int i = 0; i < list.Count; i++)
			{
				AddGraphic(list[i]);
			}
			UnityEngine.UI.ListPool<MaskableGraphic>.Release(list);
		}

		private void SetAllGraphicMaterials()
		{
			foreach (MaskableGraphic item in _graphicSet)
			{
				if (item != null)
				{
					SetGraphicMaterial(item);
				}
			}
		}

		private void ClearAllGraphics()
		{
			foreach (MaskableGraphic item in _graphicSet)
			{
				if (item != null)
				{
					RestoreGraphic(item);
				}
			}
			_graphicSet.Clear();
			_matList.Clear();
		}

		public void AddGraphic(MaskableGraphic graphic)
		{
			if (!_graphicSet.Contains(graphic))
			{
				bool isMaterialDirty = _isMaterialDirty;
				_graphicSet.Add(graphic);
				SetupGraphic(graphic);
				_isMaterialDirty = isMaterialDirty;
			}
		}

		public void DeleteGraphic(MaskableGraphic graphic)
		{
			if (_graphicSet.Contains(graphic))
			{
				RestoreGraphic(graphic);
				_graphicSet.Remove(graphic);
			}
		}

		public void RestoreGraphic(MaskableGraphic graphic)
		{
			graphic.UnregisterDirtyMaterialCallback(SetAllGraphicMaterials);
			RestoreGraphicMaterial(graphic);
		}

		private void SetupGraphic(MaskableGraphic graphic)
		{
			graphic.RegisterDirtyMaterialCallback(SetAllGraphicMaterials);
			SetGraphicMaterial(graphic);
		}

		private void SetGraphicMaterial(MaskableGraphic graphic)
		{
			Material material = graphic.material;
			if (!HasMaterial(material))
			{
				Material material2 = AddMaterial(material);
				if (material2 != material)
				{
					graphic.material = material2;
				}
			}
		}

		private void RestoreGraphicMaterial(MaskableGraphic graphic)
		{
			graphic.material = RemoveMaterial(graphic.material);
		}
	}
}
