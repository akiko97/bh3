using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public interface IAttacker
	{
		Transform transform { get; }

		Vector3 XZPosition { get; }

		Vector3 FaceDirection { get; }

		BaseMonoEntity AttackTarget { get; }

		event AnimatedHitBoxCreatedHandler onAnimatedHitBoxCreatedCallBack;

		uint GetRuntimeID();

		bool IsToBeRemove();

		Transform GetAttachPoint(string name);

		bool IsActive();

		void FrameHalt(int frameNum);

		float Evaluate(DynamicFloat target);

		int Evaluate(DynamicInt target);

		void onAnimatedHitBoxCreated(MonoAnimatedHitboxDetect hitBox, ConfigEntityAttackPattern attackPattern);
	}
}
