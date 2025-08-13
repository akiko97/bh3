using UnityEngine;

namespace MoleMole
{
	public interface IWeaponAttacher
	{
		GameObject gameObject { get; }

		Transform GetAttachPoint(string name);

		bool HasAttachPoint(string name);
	}
}
