using UnityEngine;

namespace MoleMole
{
	public class MonoInLevelShaderContainer : MonoBehaviour
	{
		[Header("shaders need to be warm up in level, and dontdestroyonload, because they are small in memory")]
		public Shader[] shaders;
	}
}
