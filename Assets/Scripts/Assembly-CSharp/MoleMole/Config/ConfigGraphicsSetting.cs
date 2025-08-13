using System;
using System.Collections.Generic;

namespace MoleMole.Config
{
	public class ConfigGraphicsSetting
	{
		public int RecommendResolutionX;

		public int RecommendResolutionY;

		[NonSerialized]
		public Dictionary<ResolutionQualityGrade, int> ResolutionPercentage = new Dictionary<ResolutionQualityGrade, int>();

		public ResolutionQualityGrade ResolutionQuality;

		[NonSerialized]
		public Dictionary<PostEffectQualityGrade, int> PostFxGradeBufferSize = new Dictionary<PostEffectQualityGrade, int>();

		public int TargetFrameRate;

		public float ContrastDelta;

		public ConfigGraphicsVolatileSetting VolatileSetting;
	}
}
