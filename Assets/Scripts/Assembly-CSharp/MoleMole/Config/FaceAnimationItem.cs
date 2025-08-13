using System;

namespace MoleMole.Config
{
	[Serializable]
	public class FaceAnimationItem
	{
		public string name;

		public int length;

		public float timePerFrame;

		public FaceAnimationFrameBlock[] leftEyeBlocks;

		public FaceAnimationFrameBlock[] rightEyeBlocks;

		public FaceAnimationFrameBlock[] mouthBlocks;

		public FaceAnimationFrameBlock[] attachmentBlocks;
	}
}
