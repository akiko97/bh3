using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Renderer))]
	public class RotateAnimation : MonoBehaviour
	{
		public Vector3 SpeedVector = new Vector3(0f, 1f, 0f);

		public List<Transform> m_Objects;

		private void Update()
		{
			if (m_Objects.Count == 0)
			{
				base.transform.Rotate(SpeedVector * Time.deltaTime);
				return;
			}
			for (int i = 0; i < m_Objects.Count; i++)
			{
				m_Objects[i].transform.Rotate(SpeedVector * Time.deltaTime);
			}
		}
	}
}
