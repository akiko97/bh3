using System;
using MoleMole.Config;

namespace MoleMole
{
	[Serializable]
	public class FaceAnimationConvertItem
	{
		public ConfigFaceAnimation config;

		public TestMatInfoProvider leftEyeProvider;

		public TestMatInfoProvider rightEyeProvider;

		public TestMatInfoProvider mouthProvider;
	}
}
