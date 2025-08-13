using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Camera))]
	public class InvertCameraCulling : MonoBehaviour
	{
		private bool _oldCulling;

		public void OnPreRender()
		{
			_oldCulling = GL.invertCulling;
			GL.invertCulling = true;
		}

		public void OnPostRender()
		{
			GL.invertCulling = _oldCulling;
		}
	}
}
