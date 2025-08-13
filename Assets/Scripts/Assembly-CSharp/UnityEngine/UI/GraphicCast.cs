namespace UnityEngine.UI
{
	public class GraphicCast : Graphic
	{
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}
	}
}
