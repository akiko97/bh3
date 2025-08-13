using UnityEngine;

namespace MoleMole
{
	public class MonoStageEnv : MonoBehaviour
	{
		public Transform lightForwardTransform;

		public MonoSpawnPoint[] spawnPoints { get; private set; }

		public void Awake()
		{
			spawnPoints = base.transform.GetComponentsInChildren<MonoSpawnPoint>();
		}

		public int GetNamedSpawnPointIx(string name)
		{
			for (int i = 0; i < spawnPoints.Length; i++)
			{
				if (spawnPoints[i].name == name)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
