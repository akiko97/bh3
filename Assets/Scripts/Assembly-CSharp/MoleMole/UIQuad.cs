using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class UIQuad
	{
		public Vector2 min;

		public Vector2 max;

		public Vector2[] uvMin;

		public Vector2[] uvMax;

		public static readonly int MAX_UV_COUNT = 4;

		public UIQuad()
		{
			Vector2 vector = -Vector2.one;
			Set(Vector2.zero, Vector2.zero, vector, vector, vector, vector, vector, vector, vector, vector);
		}

		public UIQuad(Vector2 min, Vector2 max, Vector2 uvMin0, Vector2 uvMax0, Vector2 uvMin1, Vector2 uvMax1)
		{
			Vector2 vector = -Vector2.one;
			Set(min, max, uvMin0, uvMax0, uvMin1, uvMax1, vector, vector, vector, vector);
		}

		public UIQuad(Vector2 min, Vector2 max, Vector2 uvMin0, Vector2 uvMax0, Vector2 uvMin1, Vector2 uvMax1, Vector2 uvMin2, Vector2 uvMax2, Vector2 uvMin3, Vector2 uvMax3)
		{
			Set(min, max, uvMin0, uvMax0, uvMin1, uvMax1, uvMin2, uvMax2, uvMin3, uvMax3);
		}

		public UIQuad(UIQuad quad)
		{
			Set(quad.min, quad.max, quad.uvMin[0], quad.uvMax[0], quad.uvMin[1], quad.uvMax[1], quad.uvMin[2], quad.uvMax[2], quad.uvMin[3], quad.uvMax[3]);
		}

		public void Set(Vector2 min, Vector2 max, Vector2 uvMin0, Vector2 uvMax0, Vector2 uvMin1, Vector2 uvMax1, Vector2 uvMin2, Vector2 uvMax2, Vector2 uvMin3, Vector2 uvMax3)
		{
			this.min = min;
			this.max = max;
			uvMin = new Vector2[4] { uvMin0, uvMin1, uvMin2, uvMin3 };
			uvMax = new Vector2[4] { uvMax0, uvMax1, uvMax2, uvMax3 };
		}

		public UIVertex[] ToUIQuad(UIVertex template)
		{
			UIVertex[] array = new UIVertex[4] { template, template, template, template };
			array[0].position = new Vector3(min.x, min.y, 0f);
			array[1].position = new Vector3(min.x, max.y, 0f);
			array[2].position = new Vector3(max.x, max.y, 0f);
			array[3].position = new Vector3(max.x, min.y, 0f);
			array[0].uv0 = new Vector3(uvMin[0].x, uvMin[0].y);
			array[1].uv0 = new Vector3(uvMin[0].x, uvMax[0].y);
			array[2].uv0 = new Vector3(uvMax[0].x, uvMax[0].y);
			array[3].uv0 = new Vector3(uvMax[0].x, uvMin[0].y);
			array[0].uv1 = new Vector3(uvMin[1].x, uvMin[1].y);
			array[1].uv1 = new Vector3(uvMin[1].x, uvMax[1].y);
			array[2].uv1 = new Vector3(uvMax[1].x, uvMax[1].y);
			array[3].uv1 = new Vector3(uvMax[1].x, uvMin[1].y);
			array[0].normal = new Vector3(uvMin[2].x, 0f, uvMin[2].y);
			array[1].normal = new Vector3(uvMin[2].x, 0f, uvMax[2].y);
			array[2].normal = new Vector3(uvMax[2].x, 0f, uvMax[2].y);
			array[3].normal = new Vector3(uvMax[2].x, 0f, uvMin[2].y);
			array[0].tangent = new Vector3(uvMin[3].x, 0f, uvMin[3].y);
			array[1].tangent = new Vector3(uvMin[3].x, 0f, uvMax[3].y);
			array[2].tangent = new Vector3(uvMax[3].x, 0f, uvMax[3].y);
			array[3].tangent = new Vector3(uvMax[3].x, 0f, uvMin[3].y);
			return array;
		}

		public UIQuad Split(UIQuad another, int anotherUIid = 1, List<UIQuad> unOverlappedList = null, bool revertUVid = false)
		{
			UIQuad result = null;
			UIQuad uIQuad = Split(another, anotherUIid, unOverlappedList, 0, revertUVid);
			if (uIQuad != null)
			{
				result = uIQuad.Split(another, anotherUIid, unOverlappedList, 1, revertUVid);
			}
			return result;
		}

		private UIQuad Split(UIQuad another, int anotherUIid, List<UIQuad> unOverlappedList, int dir, bool revertUVid)
		{
			float num = Mathf.Max(min[dir], another.min[dir]);
			float num2 = Mathf.Min(max[dir], another.max[dir]);
			if (min[dir] < num && unOverlappedList != null)
			{
				unOverlappedList.Add(GetLowerPart(num, dir, anotherUIid, revertUVid));
			}
			if (num2 < max[dir] && unOverlappedList != null)
			{
				unOverlappedList.Add(GetHigherPart(num2, dir, anotherUIid, revertUVid));
			}
			UIQuad uIQuad = null;
			if (num < num2)
			{
				uIQuad = GetMiddlePart(num, num2, anotherUIid, dir);
				if (!revertUVid)
				{
					uIQuad.uvMin[anotherUIid][dir] = Mathf.Lerp(another.uvMin[anotherUIid][dir], another.uvMax[anotherUIid][dir], (num - another.min[dir]) / (another.max[dir] - another.min[dir]));
					uIQuad.uvMax[anotherUIid][dir] = Mathf.Lerp(another.uvMin[anotherUIid][dir], another.uvMax[anotherUIid][dir], (num2 - another.min[dir]) / (another.max[dir] - another.min[dir]));
				}
			}
			return uIQuad;
		}

		private UIQuad GetLowerPart(float splitPoint, int dir, int anotherUIid, bool revertUIid)
		{
			splitPoint = Mathf.Min(max[dir], splitPoint);
			UIQuad uIQuad = new UIQuad(this);
			uIQuad.max[dir] = splitPoint;
			float t = (splitPoint - min[dir]) / (max[dir] - min[dir]);
			if (!revertUIid)
			{
				for (int i = 0; i < MAX_UV_COUNT; i++)
				{
					if (i != anotherUIid)
					{
						uIQuad.uvMax[i][dir] = Mathf.Lerp(uvMin[i][dir], uvMax[i][dir], t);
					}
					else
					{
						uIQuad.uvMax[i][dir] = -1f;
					}
				}
			}
			else
			{
				for (int j = 0; j < MAX_UV_COUNT; j++)
				{
					if (j != anotherUIid)
					{
						uIQuad.uvMax[j][dir] = -1f;
					}
					else
					{
						uIQuad.uvMax[j][dir] = Mathf.Lerp(uvMin[j][dir], uvMax[j][dir], t);
					}
				}
			}
			return uIQuad;
		}

		private UIQuad GetHigherPart(float splitPoint, int dir, int anotherUIid, bool revertUIid)
		{
			splitPoint = Mathf.Max(min[dir], splitPoint);
			UIQuad uIQuad = new UIQuad(this);
			uIQuad.min[dir] = splitPoint;
			float t = (splitPoint - min[dir]) / (max[dir] - min[dir]);
			if (!revertUIid)
			{
				for (int i = 0; i < MAX_UV_COUNT; i++)
				{
					if (i != anotherUIid)
					{
						uIQuad.uvMin[i][dir] = Mathf.Lerp(uvMin[i][dir], uvMax[i][dir], t);
					}
					else
					{
						uIQuad.uvMin[i][dir] = -1f;
					}
				}
			}
			else
			{
				for (int j = 0; j < MAX_UV_COUNT; j++)
				{
					if (j != anotherUIid)
					{
						uIQuad.uvMin[j][dir] = -1f;
					}
					else
					{
						uIQuad.uvMin[j][dir] = Mathf.Lerp(uvMin[j][dir], uvMax[j][dir], t);
					}
				}
			}
			return uIQuad;
		}

		private UIQuad GetMiddlePart(float splitPoint1, float splitPoint2, int anotherUIid, int dir)
		{
			UIQuad uIQuad = new UIQuad(this);
			uIQuad.min[dir] = splitPoint1;
			uIQuad.max[dir] = splitPoint2;
			float t = (splitPoint1 - min[dir]) / (max[dir] - min[dir]);
			float t2 = (splitPoint2 - min[dir]) / (max[dir] - min[dir]);
			for (int i = 0; i < MAX_UV_COUNT; i++)
			{
				uIQuad.uvMin[i][dir] = Mathf.Lerp(uvMin[i][dir], uvMax[i][dir], t);
				uIQuad.uvMax[i][dir] = Mathf.Lerp(uvMin[i][dir], uvMax[i][dir], t2);
			}
			return uIQuad;
		}
	}
}
