namespace MoleMole.Config
{
	public class ConfigMonsterCommonArguments : ConfigEntityCommonArguments
	{
		public float HP;

		public float Attack;

		public float Defence;

		public float? FadeInHeight;

		public float BePushedSpeedRatio;

		public float BePushedSpeedRatioThrow;

		public float HitboxInactiveDelay = 0.45f;

		public float UseTransparentShaderDistanceThreshold = 2.1f;

		public bool UseSwitchShader = true;

		public bool UseEliteShader = true;
	}
}
