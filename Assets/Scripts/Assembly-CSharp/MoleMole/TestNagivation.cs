using UnityEngine;

namespace MoleMole
{
	public class TestNagivation : MonoBehaviour
	{
		public Transform goal;

		private void Start()
		{
			NavMeshAgent component = GetComponent<NavMeshAgent>();
			component.destination = goal.position;
		}

		private void Update()
		{
		}
	}
}
