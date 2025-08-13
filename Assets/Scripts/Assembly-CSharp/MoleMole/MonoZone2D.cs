using UnityEngine;

namespace MoleMole
{
	public class MonoZone2D : MonoBehaviour
	{
		private MonoSubZone2D[] _zones;

		private void Awake()
		{
			Init();
		}

		public void Init()
		{
			_zones = GetComponentsInChildren<MonoSubZone2D>(true);
		}

		public void Reset()
		{
			Init();
		}

		public bool Contain(Vector3 pos)
		{
			for (int i = 0; i < _zones.Length; i++)
			{
				if (_zones[i].Contain(pos))
				{
					return true;
				}
			}
			return false;
		}
	}
}
