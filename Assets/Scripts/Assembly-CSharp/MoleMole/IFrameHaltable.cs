using UnityEngine;

namespace MoleMole
{
	public interface IFrameHaltable
	{
		Vector3 XZPosition { get; }

		FixedStack<float> timeScaleStack { get; }

		uint GetRuntimeID();

		bool IsToBeRemove();

		bool IsActive();

		void FrameHalt(int frameNum);
	}
}
