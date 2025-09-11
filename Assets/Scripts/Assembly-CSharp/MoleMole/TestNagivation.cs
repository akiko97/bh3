using UnityEngine;

namespace MoleMole
{
	public class TestNagivation : MonoBehaviour
	{
		public Transform goal;

		private void Start()
		{
			UnityEngine.AI.NavMeshAgent component = GetComponent<UnityEngine.AI.NavMeshAgent>();
			component.destination = goal.position;
		}

		private void Update()
		{
		}
	}
}
