using UnityEngine;

namespace MoleMole
{
	public interface IColorChange
	{
		Vector3 XZPosition { get; }

		Color EmissionColor { get; set; }

		uint GetRuntimeID();

		bool IsToBeRemove();

		bool IsActive();

		Material[] GetAllMaterials();
	}
}
