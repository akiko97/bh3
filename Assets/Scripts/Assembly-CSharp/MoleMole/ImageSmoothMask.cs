using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(CanvasRenderer))]
	public class ImageSmoothMask : MonoBehaviour
	{
		private class Quad
		{
			public Vector2 min;

			public Vector2 max;

			public Vector2 uvMin0;

			public Vector2 uvMax0;

			public Vector2 uvMin1;

			public Vector2 uvMax1;

			public Quad(Vector2 min, Vector2 max, Vector2 uvMin0, Vector2 uvMax0, Vector2 uvMin1, Vector2 uvMax1)
			{
				this.min = min;
				this.max = max;
				this.uvMin0 = uvMin0;
				this.uvMax0 = uvMax0;
				this.uvMin1 = uvMin1;
				this.uvMax1 = uvMax1;
			}

			public Quad(Quad quad)
			{
				min = quad.min;
				max = quad.max;
				uvMin0 = quad.uvMin0;
				uvMax0 = quad.uvMax0;
				uvMin1 = quad.uvMin1;
				uvMax1 = quad.uvMax1;
			}

			public UIVertex[] ToUIQuad(UIVertex template)
			{
				UIVertex[] array = new UIVertex[4] { template, template, template, template };
				array[0].position = new Vector3(min.x, min.y, 0f);
				array[1].position = new Vector3(min.x, max.y, 0f);
				array[2].position = new Vector3(max.x, max.y, 0f);
				array[3].position = new Vector3(max.x, min.y, 0f);
				array[0].uv0 = new Vector3(uvMin0.x, uvMin0.y, 0f);
				array[1].uv0 = new Vector3(uvMin0.x, uvMax0.y, 0f);
				array[2].uv0 = new Vector3(uvMax0.x, uvMax0.y, 0f);
				array[3].uv0 = new Vector3(uvMax0.x, uvMin0.y, 0f);
				array[0].uv1 = new Vector3(uvMin1.x, uvMin1.y, 0f);
				array[1].uv1 = new Vector3(uvMin1.x, uvMax1.y, 0f);
				array[2].uv1 = new Vector3(uvMax1.x, uvMax1.y, 0f);
				array[3].uv1 = new Vector3(uvMax1.x, uvMin1.y, 0f);
				return array;
			}

			public Quad Split(Quad another, List<Quad> unOverlappedList = null)
			{
				Quad result = null;
				Quad quad = Split(another, unOverlappedList, 0);
				if (quad != null)
				{
					result = quad.Split(another, unOverlappedList, 1);
				}
				return result;
			}

			public Quad GridSplit(Quad another, List<Quad> unOverlappedList)
			{
				Quad result = null;
				List<Quad> list = new List<Quad>();
				Quad quad = Split(another, list, 0);
				if (quad != null)
				{
					result = quad.Split(another, unOverlappedList, 1);
				}
				foreach (Quad item in list)
				{
					quad = item.Split(another, unOverlappedList, 1);
					if (quad != null)
					{
						unOverlappedList.Add(quad);
					}
				}
				return result;
			}

			private Quad Split(Quad another, List<Quad> unOverlappedList, int dir)
			{
				float num = Mathf.Max(min[dir], another.min[dir]);
				float num2 = Mathf.Min(max[dir], another.max[dir]);
				if (min[dir] < num && unOverlappedList != null)
				{
					unOverlappedList.Add(GetLowerPart(num, another.uvMin1[dir], dir));
				}
				if (num2 < max[dir] && unOverlappedList != null)
				{
					unOverlappedList.Add(GetHigherPart(num2, another.uvMax1[dir], dir));
				}
				Quad quad = null;
				if (num < num2)
				{
					quad = GetMiddlePart(num, num2, dir);
					quad.uvMin1[dir] = Mathf.Lerp(another.uvMin1[dir], another.uvMax1[dir], (num - another.min[dir]) / (another.max[dir] - another.min[dir]));
					quad.uvMax1[dir] = Mathf.Lerp(another.uvMin1[dir], another.uvMax1[dir], (num2 - another.min[dir]) / (another.max[dir] - another.min[dir]));
				}
				return quad;
			}

			private Quad GetLowerPart(float splitPoint, float uv1, int dir)
			{
				splitPoint = Mathf.Min(max[dir], splitPoint);
				Quad quad = new Quad(this);
				quad.max[dir] = splitPoint;
				float t = (splitPoint - min[dir]) / (max[dir] - min[dir]);
				quad.uvMax0[dir] = Mathf.Lerp(uvMin0[dir], uvMax0[dir], t);
				quad.uvMax1[dir] = uv1;
				return quad;
			}

			private Quad GetHigherPart(float splitPoint, float uv1, int dir)
			{
				splitPoint = Mathf.Max(min[dir], splitPoint);
				Quad quad = new Quad(this);
				quad.min[dir] = splitPoint;
				float t = (splitPoint - min[dir]) / (max[dir] - min[dir]);
				quad.uvMin0[dir] = Mathf.Lerp(uvMin0[dir], uvMax0[dir], t);
				quad.uvMin1[dir] = uv1;
				return quad;
			}

			private Quad GetMiddlePart(float splitPoint1, float splitPoint2, int dir)
			{
				Quad quad = new Quad(this);
				quad.min[dir] = splitPoint1;
				quad.max[dir] = splitPoint2;
				float t = (splitPoint1 - min[dir]) / (max[dir] - min[dir]);
				float t2 = (splitPoint2 - min[dir]) / (max[dir] - min[dir]);
				quad.uvMin0[dir] = Mathf.Lerp(uvMin0[dir], uvMax0[dir], t);
				quad.uvMin1[dir] = Mathf.Lerp(uvMin1[dir], uvMax1[dir], t);
				quad.uvMax0[dir] = Mathf.Lerp(uvMin0[dir], uvMax0[dir], t2);
				quad.uvMax1[dir] = Mathf.Lerp(uvMin1[dir], uvMax1[dir], t2);
				return quad;
			}
		}

		public Shader maskShader;

		public Image maskImage;

		private ImageForSmoothMask _image;

		private Material _material;

		public MonoMaskSlider maskSlider;

		[HideInInspector]
		public float coverRatio = 0.5f;

		private void CreateMaterial()
		{
			_material = new Material(maskShader);
		}

		private void Init()
		{
			maskImage.enabled = false;
			_image = GetComponent<ImageForSmoothMask>();
			CreateMaterial();
			_material.SetTexture("_MaskTex", maskImage.mainTexture);
			_image.material = _material;
			CheckValid();
			if (maskSlider != null)
			{
				MonoMaskSlider monoMaskSlider = maskSlider;
				monoMaskSlider.onValueChanged = (Action<float, float>)Delegate.Combine(monoMaskSlider.onValueChanged, new Action<float, float>(OnMaskSliderValueChanged));
			}
			ResetRender();
		}

		private void OnMaskSliderValueChanged(float fromRatio, float toRatio)
		{
			if (fromRatio != toRatio)
			{
				coverRatio = toRatio;
				ResetRender();
			}
		}

		private void Awake()
		{
			Init();
		}

		private void OnDestroy()
		{
			if (_material != null)
			{
				UnityEngine.Object.DestroyImmediate(_material);
			}
			maskShader = null;
			if (maskImage != null)
			{
				maskImage.sprite = null;
			}
			maskImage = null;
			maskSlider = null;
		}

		private void ResetRender()
		{
			_image.SetAllDirty();
		}

		private void CheckValid()
		{
			CheckImage(_image);
			CheckImage(maskImage);
		}

		private void CheckImage(Image image)
		{
			if (image.type != Image.Type.Simple && image.type != Image.Type.Sliced)
			{
			}
		}

		private static Vector2[] GetMinMaxUV(Sprite sprite)
		{
			Vector2[] array = new Vector2[2];
			Vector2[] uv = sprite.uv;
			array[0] = Vector2.one * float.PositiveInfinity;
			array[1] = Vector2.one * float.NegativeInfinity;
			Vector2[] array2 = uv;
			foreach (Vector2 rhs in array2)
			{
				array[0] = Vector2.Min(array[0], rhs);
				array[1] = Vector2.Max(array[1], rhs);
			}
			return array;
		}

		public List<UIVertex[]> GenerateUIQuads()
		{
			List<UIVertex[]> list = new List<UIVertex[]>();
			List<Quad> imageQuades = GetImageQuades(_image, base.transform.worldToLocalMatrix);
			List<Quad> imageQuades2 = GetImageQuades(maskImage, base.transform.worldToLocalMatrix);
			List<Quad> list2 = new List<Quad>();
			foreach (Quad item in imageQuades2)
			{
				Quad quad = null;
				foreach (Quad item2 in imageQuades)
				{
					quad = item2.Split(item);
					if (quad != null)
					{
						list2.Add(quad);
					}
				}
			}
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = _image.color;
			simpleVert.normal.x = 1f;
			foreach (Quad item3 in list2)
			{
				list.Add(item3.ToUIQuad(simpleVert));
			}
			return list;
		}

		private static List<Quad> GetImageQuades(Image image, Matrix4x4 mat)
		{
			List<Quad> list = new List<Quad>();
			Vector3[] array = new Vector3[4];
			image.rectTransform.GetWorldCorners(array);
			array[0] = mat.MultiplyPoint(array[0]);
			array[2] = mat.MultiplyPoint(array[2]);
			Vector2[] minMaxUV = GetMinMaxUV(image.sprite);
			Quad quad = new Quad(array[0], array[2], minMaxUV[0], minMaxUV[1], minMaxUV[0], minMaxUV[1]);
			if (image.type == Image.Type.Simple)
			{
				list.Add(quad);
			}
			else if (image.type == Image.Type.Sliced)
			{
				Vector4 border = image.sprite.border;
				Rect rect = image.sprite.rect;
				Quad quad2 = new Quad(quad);
				quad2.min += new Vector2(border.x, border.y);
				quad2.max -= new Vector2(border.z, border.w);
				quad2.uvMin0 = new Vector2(Mathf.Lerp(minMaxUV[0].x, minMaxUV[1].x, border.x / rect.width), Mathf.Lerp(minMaxUV[0].y, minMaxUV[1].y, border.y / rect.height));
				quad2.uvMin1 = quad2.uvMin0;
				quad2.uvMax0 = new Vector2(Mathf.Lerp(minMaxUV[1].x, minMaxUV[0].x, border.z / rect.width), Mathf.Lerp(minMaxUV[1].y, minMaxUV[0].y, border.w / rect.height));
				quad2.uvMax1 = quad2.uvMax0;
				if (image.fillCenter)
				{
					list.Add(quad2);
				}
				List<Quad> list2 = new List<Quad>();
				quad.GridSplit(quad2, list2);
				foreach (Quad item in list2)
				{
					item.uvMin0 = item.uvMin1;
					item.uvMax0 = item.uvMax1;
				}
				list.AddRange(list2);
			}
			return list;
		}
	}
}
