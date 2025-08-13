using UnityEngine;

namespace MoleMole
{
	public interface IAIEntity
	{
		Vector3 XZPosition { get; }

		BaseMonoEntity AttackTarget { get; }

		Vector3 FaceDirection { get; }

		float TimeScale { get; }

		Transform transform { get; }

		Vector3 RootNodePosition { get; }

		uint GetRuntimeID();

		bool IsActive();

		IAIController GetActiveAIController();

		float GetProperty(string key);
	}
}
