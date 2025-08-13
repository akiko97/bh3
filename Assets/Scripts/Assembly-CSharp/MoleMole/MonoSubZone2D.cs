using UnityEngine;

namespace MoleMole
{
	public class MonoSubZone2D : MonoBehaviour
	{
		public virtual bool Contain(Vector3 pos)
		{
			return false;
		}
	}
}
