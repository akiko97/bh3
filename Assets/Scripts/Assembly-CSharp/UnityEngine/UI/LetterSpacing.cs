using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/Letter Spacing", 14)]
	[RequireComponent(typeof(Text))]
	public class LetterSpacing : BaseMeshEffect, ILayoutElement
	{
		[SerializeField]
		private float m_spacing;

		public bool autoFixLine;

		public float spacing
		{
			get
			{
				return m_spacing;
			}
			set
			{
				if (m_spacing != value)
				{
					m_spacing = value;
					if (base.graphic != null)
					{
						base.graphic.SetVerticesDirty();
					}
					LayoutRebuilder.MarkLayoutForRebuild((RectTransform)base.transform);
				}
			}
		}

		private Text text
		{
			get
			{
				return base.gameObject.GetComponent<Text>();
			}
		}

		public float minWidth
		{
			get
			{
				return text.minWidth;
			}
		}

		public float preferredWidth
		{
			get
			{
				string text = this.text.text;
				bool flag = false;
				int num = 0;
				for (int i = 0; i < text.Length; i++)
				{
					switch (text[i])
					{
					case '<':
						flag = true;
						continue;
					case '>':
						flag = false;
						continue;
					}
					if (!flag)
					{
						num++;
					}
				}
				return this.text.preferredWidth + spacing * (float)this.text.fontSize / 100f * (float)num;
			}
		}

		public float flexibleWidth
		{
			get
			{
				return text.flexibleWidth;
			}
		}

		public float minHeight
		{
			get
			{
				return text.minHeight;
			}
		}

		public float preferredHeight
		{
			get
			{
				return text.preferredHeight;
			}
		}

		public float flexibleHeight
		{
			get
			{
				return text.flexibleHeight;
			}
		}

		public int layoutPriority
		{
			get
			{
				return text.layoutPriority;
			}
		}

		protected LetterSpacing()
		{
		}

		protected override void Start()
		{
			base.Start();
			AccommodateText();
		}

		public void CalculateLayoutInputHorizontal()
		{
		}

		public void CalculateLayoutInputVertical()
		{
		}

		public void AccommodateText()
		{
			if (!autoFixLine || string.IsNullOrEmpty(this.text.text))
			{
				return;
			}
			float width = base.transform.GetComponent<RectTransform>().rect.width;
			if (width <= 0f && base.gameObject.activeSelf && this.text.text.Length > 0)
			{
				StartCoroutine(ReAccommodateText());
				return;
			}
			float num = spacing * (float)this.text.fontSize / 100f;
			float num2 = this.text.fontSize;
			string text = this.text.text.Replace(Environment.NewLine, string.Empty);
			text = text.Replace("\n", string.Empty);
			string text2 = text;
			int num3 = 1;
			bool flag = false;
			float num4 = 0f;
			for (int i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
				case '<':
					flag = true;
					continue;
				case '>':
					flag = false;
					continue;
				case '{':
					if (i < text.Length - 2 && text[i + 1] == '{' && text[i + 2] == '{')
					{
						text2 = text2.Insert(i, Environment.NewLine);
						num3++;
						i += 2;
						num4 = 0f;
						continue;
					}
					break;
				}
				if (flag)
				{
					continue;
				}
				num4 += num2 + num;
				if (!(num4 - num > width))
				{
					continue;
				}
				char c = text[i];
				int result;
				if ((c == '.' || int.TryParse(c.ToString(), out result)) && i + 1 < text.Length && (text[i + 1] == '%' || int.TryParse(text[i + 1].ToString(), out result)))
				{
					int num5 = i - 1;
					while (num5 > 0 && int.TryParse(text[num5].ToString(), out result))
					{
						num5--;
					}
					text2 = text2.Insert(num5 + num3, Environment.NewLine);
					num3++;
					num4 = (float)(i - num5) * (num + num2);
				}
				else
				{
					text2 = text2.Insert(i + num3, Environment.NewLine);
					num3++;
					num4 = 0f;
				}
			}
			text2 = text2.Replace("{{{", string.Empty);
			this.text.text = text2;
		}

		private IEnumerator ReAccommodateText()
		{
			yield return new WaitForEndOfFrame();
			AccommodateText();
		}

		public int GetLineCount(string targetText)
		{
			float width = base.transform.GetComponent<RectTransform>().rect.width;
			if (width <= 0f)
			{
				return 1;
			}
			float num = spacing * (float)this.text.fontSize / 100f;
			float num2 = this.text.fontSize;
			string text = targetText.Replace(Environment.NewLine, string.Empty);
			text = text.Replace("\n", string.Empty);
			string text2 = text;
			int num3 = 1;
			bool flag = false;
			float num4 = 0f;
			for (int i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
				case '<':
					flag = true;
					continue;
				case '>':
					flag = false;
					continue;
				case '{':
					if (i < text.Length - 2 && text[i + 1] == '{' && text[i + 2] == '{')
					{
						text2 = text2.Insert(i, Environment.NewLine);
						num3++;
						i += 2;
						num4 = 0f;
						continue;
					}
					break;
				}
				if (flag)
				{
					continue;
				}
				num4 += num2 + num;
				if (!(num4 - num > width))
				{
					continue;
				}
				char c = text[i];
				int result;
				if ((c == '.' || int.TryParse(c.ToString(), out result)) && i + 1 < text.Length && (text[i + 1] == '%' || int.TryParse(text[i + 1].ToString(), out result)))
				{
					int num5 = i - 1;
					while (num5 > 0 && int.TryParse(text[num5].ToString(), out result))
					{
						num5--;
					}
					text2 = text2.Insert(num5 + num3, Environment.NewLine);
					num3++;
					num4 = (float)(i - num5) * (num + num2);
				}
				else
				{
					text2 = text2.Insert(i + num3, Environment.NewLine);
					num3++;
					num4 = 0f;
				}
			}
			text2 = text2.Replace("{{{", string.Empty);
			return (num3 <= 0) ? 1 : num3;
		}

		private string[] GetLines()
		{
			return text.text.Split('\n');
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				List<UIVertex> list = new List<UIVertex>();
				vh.GetUIVertexStream(list);
				ModifyVertices(list);
				vh.Clear();
				vh.AddUIVertexTriangleStream(list);
			}
		}

		public void ModifyVertices(List<UIVertex> verts)
		{
			if (!IsActive())
			{
				return;
			}
			string[] lines = GetLines();
			float num = spacing * (float)this.text.fontSize / 100f;
			float num2 = 0f;
			int num3 = 0;
			bool flag = false;
			switch (this.text.alignment)
			{
			case TextAnchor.UpperLeft:
			case TextAnchor.MiddleLeft:
			case TextAnchor.LowerLeft:
				num2 = 0f;
				break;
			case TextAnchor.UpperCenter:
			case TextAnchor.MiddleCenter:
			case TextAnchor.LowerCenter:
				num2 = 0.5f;
				break;
			case TextAnchor.UpperRight:
			case TextAnchor.MiddleRight:
			case TextAnchor.LowerRight:
				num2 = 1f;
				break;
			}
			foreach (string text in lines)
			{
				float num4 = (float)(text.Length - 1) * num * num2;
				int num5 = 0;
				for (int j = 0; j < text.Length; j++)
				{
					int index = num3 * 6;
					int index2 = num3 * 6 + 1;
					int index3 = num3 * 6 + 2;
					int index4 = num3 * 6 + 3;
					int index5 = num3 * 6 + 4;
					int num6 = num3 * 6 + 5;
					bool flag2 = false;
					switch (this.text.text[num3])
					{
					case '<':
						flag = true;
						flag2 = true;
						break;
					case '>':
						flag = false;
						flag2 = true;
						break;
					}
					if (flag2 || flag)
					{
						num5++;
						num3++;
						continue;
					}
					if (num6 > verts.Count - 1)
					{
						return;
					}
					UIVertex value = verts[index];
					UIVertex value2 = verts[index2];
					UIVertex value3 = verts[index3];
					UIVertex value4 = verts[index4];
					UIVertex value5 = verts[index5];
					UIVertex value6 = verts[num6];
					Vector3 vector = Vector3.right * (num * (float)(j - num5) - num4);
					value.position += vector;
					value2.position += vector;
					value3.position += vector;
					value4.position += vector;
					value5.position += vector;
					value6.position += vector;
					verts[index] = value;
					verts[index2] = value2;
					verts[index3] = value3;
					verts[index4] = value4;
					verts[index5] = value5;
					verts[num6] = value6;
					num3++;
				}
				num3++;
			}
		}
	}
}
