using UnityEngine;

namespace MoleMole
{
	public interface IRetreatable
	{
		Vector3 XZPosition { get; }

		uint GetRuntimeID();

		bool IsToBeRemove();

		bool IsActive();

		void SetOverrideVelocity(Vector3 speed);

		void SetNeedOverrideVelocity(bool needOverrideVelocity);

		int PushProperty(string propertyKey, float value);

		void PopProperty(string propertyKey, int stackIx);

		string GetCurrentNamedState();

		float GetCurrentNormalizedTime();
	}
}
