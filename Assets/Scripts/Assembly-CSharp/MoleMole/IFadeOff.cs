using UnityEngine;

namespace MoleMole
{
	public interface IFadeOff
	{
		Vector3 XZPosition { get; }

		uint GetRuntimeID();

		bool IsToBeRemove();

		bool IsActive();

		float GetTargetAlpha();

		Material[] GetAllMaterials();
	}
}
