using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class Gradient : BaseMeshEffect
{
	[SerializeField]
	public Color32 topColor = Color.white;

	[SerializeField]
	public Color32 bottomColor = Color.black;

	public override void ModifyMesh(VertexHelper vh)
	{
		int currentVertCount = vh.currentVertCount;
		if (IsActive() && currentVertCount != 0)
		{
			List<UIVertex> list = new List<UIVertex>();
			vh.GetUIVertexStream(list);
			UIVertex vertex = default(UIVertex);
			int count = list.Count;
			int index = Mathf.Clamp(2, 0, count - 1);
			int index2 = Mathf.Clamp(count / 5, 0, count - 1);
			float y = list[index].position.y;
			float y2 = list[index2].position.y;
			float num = y2 - y;
			for (int i = 0; i < currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);
				vertex.color *= Color.Lerp(bottomColor, topColor, (vertex.position.y - y) / num);
				vh.SetUIVertex(vertex, i);
			}
		}
	}
}
