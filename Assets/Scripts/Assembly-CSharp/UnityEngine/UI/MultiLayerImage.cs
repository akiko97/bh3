using System.Collections.Generic;
using MoleMole;
using UnityEngine.Serialization;
using UnityEngine.Sprites;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/MultiLayerImage")]
	public class MultiLayerImage : MaskableGraphic, ISerializationCallbackReceiver, ILayoutElement, ICanvasRaycastFilter
	{
		[SerializeField]
		[FormerlySerializedAs("m_Frame")]
		private Sprite m_Sprite;

		[SerializeField]
		private Vector2 m_SpriteOffset;

		[SerializeField]
		private Vector2 m_SpriteSize;

		[SerializeField]
		private Color m_SpriteColor = Color.white;

		[SerializeField]
		private Sprite m_Sprite2;

		[SerializeField]
		private Vector2 m_SpriteOffset2;

		[SerializeField]
		private Vector2 m_SpriteSize2;

		[SerializeField]
		private Color m_SpriteColor2 = Color.white;

		[SerializeField]
		private Sprite m_Sprite3;

		[SerializeField]
		private Vector2 m_SpriteOffset3;

		[SerializeField]
		private Vector2 m_SpriteSize3;

		[SerializeField]
		private Color m_SpriteColor3 = Color.white;

		[SerializeField]
		private Sprite m_Sprite4;

		[SerializeField]
		private Vector2 m_SpriteOffset4;

		[SerializeField]
		private Vector2 m_SpriteSize4;

		[SerializeField]
		private Color m_SpriteColor4 = Color.white;

		private static readonly string[] SHADER_NAMES = new string[4] { "miHoYo/UI/Image Multi-Layer 1", "miHoYo/UI/Image Multi-Layer 2", "miHoYo/UI/Image Multi-Layer 3", "miHoYo/UI/Image Multi-Layer 4" };

		private static Material[] DEFAULT_MATERIALS = new Material[4];

		private int m_matId = -1;

		private bool m_IsMatIdChange;

		private Material m_InstancedMaterial;

		private Material m_SourceMaterial;

		private float m_EventAlphaThreshold = 1f;

		public Sprite sprite
		{
			get
			{
				return m_Sprite;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetClass(ref m_Sprite, value))
				{
					SetAllDirty();
				}
			}
		}

		public Vector2 spriteOffset
		{
			get
			{
				return m_SpriteOffset;
			}
			set
			{
				if (m_SpriteOffset != value)
				{
					m_SpriteOffset = value;
					SetAllDirty();
				}
			}
		}

		public Vector2 spriteSize
		{
			get
			{
				return m_SpriteSize;
			}
			set
			{
				if (m_SpriteSize != value)
				{
					m_SpriteSize = value;
					SetAllDirty();
				}
			}
		}

		public Color spriteColor
		{
			get
			{
				return m_SpriteColor;
			}
			set
			{
				if (m_SpriteColor != value)
				{
					m_SpriteColor = value;
					SetMaterialDirty();
				}
			}
		}

		public Sprite sprite2
		{
			get
			{
				return m_Sprite2;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetClass(ref m_Sprite2, value))
				{
					SetAllDirty();
				}
			}
		}

		public Vector2 spriteOffset2
		{
			get
			{
				return m_SpriteOffset2;
			}
			set
			{
				if (m_SpriteOffset2 != value)
				{
					m_SpriteOffset2 = value;
					SetAllDirty();
				}
			}
		}

		public Vector2 spriteSize2
		{
			get
			{
				return m_SpriteSize2;
			}
			set
			{
				if (m_SpriteSize2 != value)
				{
					m_SpriteSize2 = value;
					SetAllDirty();
				}
			}
		}

		public Color spriteColor2
		{
			get
			{
				return m_SpriteColor2;
			}
			set
			{
				if (m_SpriteColor2 != value)
				{
					m_SpriteColor2 = value;
					SetMaterialDirty();
				}
			}
		}

		public Sprite sprite3
		{
			get
			{
				return m_Sprite3;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetClass(ref m_Sprite3, value))
				{
					SetAllDirty();
				}
			}
		}

		public Vector2 spriteOffset3
		{
			get
			{
				return m_SpriteOffset3;
			}
			set
			{
				if (m_SpriteOffset3 != value)
				{
					m_SpriteOffset3 = value;
					SetAllDirty();
				}
			}
		}

		public Vector2 spriteSize3
		{
			get
			{
				return m_SpriteSize3;
			}
			set
			{
				if (m_SpriteSize3 != value)
				{
					m_SpriteSize3 = value;
					SetAllDirty();
				}
			}
		}

		public Color spriteColor3
		{
			get
			{
				return m_SpriteColor3;
			}
			set
			{
				if (m_SpriteColor3 != value)
				{
					m_SpriteColor3 = value;
					SetMaterialDirty();
				}
			}
		}

		public Sprite sprite4
		{
			get
			{
				return m_Sprite4;
			}
			set
			{
				if (UnityEngine.UI.SetPropertyUtility.SetClass(ref m_Sprite4, value))
				{
					SetAllDirty();
				}
			}
		}

		public Vector2 spriteOffset4
		{
			get
			{
				return m_SpriteOffset4;
			}
			set
			{
				if (m_SpriteOffset4 != value)
				{
					m_SpriteOffset4 = value;
					SetAllDirty();
				}
			}
		}

		public Vector2 spriteSize4
		{
			get
			{
				return m_SpriteSize4;
			}
			set
			{
				if (m_SpriteSize4 != value)
				{
					m_SpriteSize4 = value;
					SetAllDirty();
				}
			}
		}

		public Color spriteColor4
		{
			get
			{
				return m_SpriteColor4;
			}
			set
			{
				if (m_SpriteColor4 != value)
				{
					m_SpriteColor4 = value;
					SetMaterialDirty();
				}
			}
		}

		private Material defaultMultiLayerMaterial
		{
			get
			{
				int num = 0;
				if (sprite4 != null)
				{
					num = 3;
				}
				else if (sprite3 != null)
				{
					num = 2;
				}
				else if (sprite2 != null)
				{
					num = 1;
				}
				if (num != m_matId)
				{
					m_matId = num;
					m_IsMatIdChange = true;
				}
				if (DEFAULT_MATERIALS[num] == null)
				{
					Shader shader = Shader.Find(SHADER_NAMES[num]);
					if (shader == null || !shader.isSupported)
					{
						Debug.LogError(string.Format("Shader '{0}' fail to load", SHADER_NAMES[num]));
						base.enabled = false;
					}
					DEFAULT_MATERIALS[num] = new Material(shader);
				}
				return DEFAULT_MATERIALS[num];
			}
		}

		public override Material defaultMaterial
		{
			get
			{
				return defaultMultiLayerMaterial;
			}
		}

		public override Material material
		{
			get
			{
				Material material = ((!(m_Material != null)) ? defaultMaterial : m_Material);
				if (!Application.isPlaying)
				{
					if (m_InstancedMaterial == null || m_IsMatIdChange)
					{
						m_IsMatIdChange = false;
						if (m_InstancedMaterial != null)
						{
							if (Application.isEditor)
							{
								Object.DestroyImmediate(m_InstancedMaterial);
							}
							else
							{
								Object.Destroy(m_InstancedMaterial);
							}
						}
						m_InstancedMaterial = new Material(material);
						m_InstancedMaterial.shaderKeywords = material.shaderKeywords;
					}
					return m_InstancedMaterial;
				}
				if (m_SourceMaterial == null || m_IsMatIdChange)
				{
					m_IsMatIdChange = false;
					m_SourceMaterial = material;
					m_Material = null;
				}
				if (m_Material == null)
				{
					m_Material = new Material(m_SourceMaterial);
					m_Material.shaderKeywords = m_SourceMaterial.shaderKeywords;
				}
				return m_Material;
			}
			set
			{
				if (!(m_Material == value))
				{
					m_Material = value;
					SetMaterialDirty();
				}
			}
		}

		public float eventAlphaThreshold
		{
			get
			{
				return m_EventAlphaThreshold;
			}
			set
			{
				m_EventAlphaThreshold = value;
			}
		}

		public override Texture mainTexture
		{
			get
			{
				if (sprite == null)
				{
					if (material != null && material.mainTexture != null)
					{
						return material.mainTexture;
					}
					return Graphic.s_WhiteTexture;
				}
				return sprite.texture;
			}
		}

		public bool hasBorder
		{
			get
			{
				if (sprite != null)
				{
					return sprite.border.sqrMagnitude > 0f;
				}
				return false;
			}
		}

		public float pixelsPerUnit
		{
			get
			{
				float num = 100f;
				if ((bool)sprite)
				{
					num = sprite.pixelsPerUnit;
				}
				float num2 = 100f;
				if ((bool)base.canvas)
				{
					num2 = base.canvas.referencePixelsPerUnit;
				}
				return num / num2;
			}
		}

		public virtual float minWidth
		{
			get
			{
				return 0f;
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				if (sprite == null)
				{
					return 0f;
				}
				return sprite.rect.size.x / pixelsPerUnit;
			}
		}

		public virtual float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		public virtual float minHeight
		{
			get
			{
				return 0f;
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				if (sprite == null)
				{
					return 0f;
				}
				return sprite.rect.size.y / pixelsPerUnit;
			}
		}

		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		public virtual int layoutPriority
		{
			get
			{
				return 0;
			}
		}

		protected MultiLayerImage()
		{
			base.useLegacyMeshGeneration = false;
		}

		public virtual void OnBeforeSerialize()
		{
		}

		public virtual void OnAfterDeserialize()
		{
		}

		private Rect GetSpriteRect(Vector2 center, Vector2 offset, Vector2 size)
		{
			return new Rect
			{
				size = size,
				center = center + offset
			};
		}

		private Vector4 GetDrawingDimensions(Rect r)
		{
			Vector4 vector = ((!(sprite == null)) ? DataUtility.GetPadding(sprite) : Vector4.zero);
			Vector2 vector2 = ((!(sprite == null)) ? new Vector2(sprite.rect.width, sprite.rect.height) : Vector2.zero);
			int num = Mathf.RoundToInt(vector2.x);
			int num2 = Mathf.RoundToInt(vector2.y);
			Vector4 vector3 = new Vector4(vector.x / (float)num, vector.y / (float)num2, ((float)num - vector.z) / (float)num, ((float)num2 - vector.w) / (float)num2);
			return new Vector4(r.x + r.width * vector3.x, r.y + r.height * vector3.y, r.x + r.width * vector3.z, r.y + r.height * vector3.w);
		}

		public override void SetNativeSize()
		{
			if (sprite != null)
			{
				float x = sprite.rect.width / pixelsPerUnit;
				float y = sprite.rect.height / pixelsPerUnit;
				base.rectTransform.anchorMax = base.rectTransform.anchorMin;
				base.rectTransform.sizeDelta = new Vector2(x, y);
				SetAllDirty();
			}
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (sprite == null)
			{
				base.OnPopulateMesh(toFill);
			}
			else
			{
				GenerateSimpleSprite(toFill);
			}
		}

		private void GenerateSimpleSprite(VertexHelper vh)
		{
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			List<UIQuad> list = new List<UIQuad>();
			if (sprite != null)
			{
				list = AddLayer(sprite, 0, pixelAdjustedRect, spriteOffset, spriteSize, list);
			}
			if (sprite2 != null)
			{
				list = AddLayer(sprite2, 1, pixelAdjustedRect, spriteOffset2, spriteSize2, list);
			}
			if (sprite3 != null)
			{
				list = AddLayer(sprite3, 2, pixelAdjustedRect, spriteOffset3, spriteSize3, list);
			}
			if (sprite4 != null)
			{
				list = AddLayer(sprite4, 3, pixelAdjustedRect, spriteOffset4, spriteSize4, list);
			}
			Color color = base.color;
			vh.Clear();
			UIVertex template = new UIVertex
			{
				color = color
			};
			for (int i = 0; i < list.Count; i++)
			{
				vh.AddUIVertexQuad(list[i].ToUIQuad(template));
			}
		}

		private List<UIQuad> AddLayer(Sprite sprite, int spriteId, Rect trsfRect, Vector2 offset, Vector2 size, List<UIQuad> srcQuads)
		{
			Vector4 vector = ((!(sprite != null)) ? Vector4.zero : DataUtility.GetOuterUV(sprite));
			size.x = ((!(size.x < float.Epsilon)) ? size.x : trsfRect.size.x);
			size.y = ((!(size.y < float.Epsilon)) ? size.y : trsfRect.size.y);
			Vector4 drawingDimensions = GetDrawingDimensions(GetSpriteRect(trsfRect.center, offset, size));
			UIQuad uIQuad = new UIQuad();
			uIQuad.min = new Vector2(drawingDimensions.x, drawingDimensions.y);
			uIQuad.max = new Vector2(drawingDimensions.z, drawingDimensions.w);
			uIQuad.uvMin[spriteId] = new Vector2(vector.x, vector.y);
			uIQuad.uvMax[spriteId] = new Vector2(vector.z, vector.w);
			List<UIQuad> list = new List<UIQuad>();
			List<UIQuad> list2 = new List<UIQuad>();
			list2.Add(uIQuad);
			for (int i = 0; i < srcQuads.Count; i++)
			{
				List<UIQuad> list3 = new List<UIQuad>();
				UIQuad uIQuad2 = srcQuads[i].Split(uIQuad, spriteId, list3);
				if (uIQuad2 != null)
				{
					list.Add(uIQuad2);
				}
				for (int j = 0; j < list3.Count; j++)
				{
					list3[j].uvMin[spriteId] = -Vector2.one;
					list3[j].uvMax[spriteId] = -Vector2.one;
				}
				list.AddRange(list3);
				List<UIQuad> list4 = new List<UIQuad>();
				for (int k = 0; k < list2.Count; k++)
				{
					list2[k].Split(srcQuads[i], spriteId, list4, true);
				}
				list2 = list4;
			}
			list.AddRange(list2);
			return list;
		}

		private Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
		{
			for (int i = 0; i <= 1; i++)
			{
				float num = border[i] + border[i + 2];
				if (rect.size[i] < num && num != 0f)
				{
					float num2 = rect.size[i] / num;
					int index2;
					int index = (index2 = i);
					float num3 = border[index2];
					border[index] = num3 * num2;
					int index3 = (index2 = i + 2);
					num3 = border[index2];
					border[index3] = num3 * num2;
				}
			}
			return border;
		}

		protected override void UpdateMaterial()
		{
			if (IsActive())
			{
				Material material = materialForRendering;
				if (material.HasProperty("_Color0"))
				{
					material.SetColor("_Color0", spriteColor);
				}
				if (material.HasProperty("_Color1"))
				{
					material.SetColor("_Color1", spriteColor2);
				}
				if (material.HasProperty("_Color2"))
				{
					material.SetColor("_Color2", spriteColor3);
				}
				if (material.HasProperty("_Color3"))
				{
					material.SetColor("_Color3", spriteColor4);
				}
				if (sprite != null && material.HasProperty("_Tex0"))
				{
					material.SetTexture("_Tex0", sprite.texture);
				}
				if (sprite2 != null && material.HasProperty("_Tex1"))
				{
					material.SetTexture("_Tex1", sprite2.texture);
				}
				if (sprite3 != null && material.HasProperty("_Tex2"))
				{
					material.SetTexture("_Tex2", sprite3.texture);
				}
				if (sprite4 != null && material.HasProperty("_Tex3"))
				{
					material.SetTexture("_Tex3", sprite4.texture);
				}
				base.canvasRenderer.materialCount = 1;
				base.canvasRenderer.SetMaterial(material, 0);
				base.canvasRenderer.SetTexture(mainTexture);
			}
		}

		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		public virtual void CalculateLayoutInputVertical()
		{
		}

		public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			if (m_EventAlphaThreshold >= 1f)
			{
				return true;
			}
			Sprite sprite = this.sprite;
			if (sprite == null)
			{
				return true;
			}
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out localPoint);
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			localPoint.x += base.rectTransform.pivot.x * pixelAdjustedRect.width;
			localPoint.y += base.rectTransform.pivot.y * pixelAdjustedRect.height;
			localPoint = MapCoordinate(localPoint, pixelAdjustedRect);
			Rect textureRect = sprite.textureRect;
			Vector2 vector = new Vector2(localPoint.x / textureRect.width, localPoint.y / textureRect.height);
			float u = Mathf.Lerp(textureRect.x, textureRect.xMax, vector.x) / (float)sprite.texture.width;
			float v = Mathf.Lerp(textureRect.y, textureRect.yMax, vector.y) / (float)sprite.texture.height;
			try
			{
				return sprite.texture.GetPixelBilinear(u, v).a >= m_EventAlphaThreshold;
			}
			catch (UnityException ex)
			{
				Debug.LogError("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + ex.Message + " Also make sure to disable sprite packing for this sprite.", this);
				return true;
			}
		}

		private Vector2 MapCoordinate(Vector2 local, Rect rect)
		{
			Rect rect2 = sprite.rect;
			return new Vector2(local.x * rect2.width / rect.width, local.y * rect2.height / rect.height);
		}
	}
}
