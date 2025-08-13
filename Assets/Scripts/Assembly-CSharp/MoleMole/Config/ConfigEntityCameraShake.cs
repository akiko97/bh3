namespace MoleMole.Config
{
	public class ConfigEntityCameraShake : IndexedConfig<ConfigEntityCameraShake>
	{
		public bool ShakeOnNotHit;

		public float ShakeRange;

		public float ShakeTime;

		public float? ShakeAngle;

		public int ShakeStepFrame = 1;

		public bool ClearPreviousShake;

		public override int CompareTo(ConfigEntityCameraShake other)
		{
			if (other == null)
			{
				return 1;
			}
			int num = ShakeOnNotHit.CompareTo(other.ShakeOnNotHit);
			if (num != 0)
			{
				return num;
			}
			num = ShakeRange.CompareTo(other.ShakeRange);
			if (num != 0)
			{
				return num;
			}
			num = ShakeTime.CompareTo(other.ShakeTime);
			if (num != 0)
			{
				return num;
			}
			num = ShakeAngle.HasValue.CompareTo(other.ShakeAngle.HasValue);
			if (num != 0)
			{
				return num;
			}
			if (ShakeAngle.HasValue && other.ShakeAngle.HasValue)
			{
				num = ShakeAngle.Value.CompareTo(other.ShakeAngle.Value);
				if (num != 0)
				{
					return num;
				}
			}
			num = ShakeStepFrame.CompareTo(other.ShakeStepFrame);
			if (num != 0)
			{
				return num;
			}
			return ClearPreviousShake.CompareTo(other.ClearPreviousShake);
		}

		public override int ContentHash()
		{
			int lastHash = 0;
			HashUtils.ContentHashOnto(ShakeOnNotHit, ref lastHash);
			HashUtils.ContentHashOnto(ShakeRange, ref lastHash);
			HashUtils.ContentHashOnto(ShakeTime, ref lastHash);
			HashUtils.ContentHashOnto((!ShakeAngle.HasValue) ? 0f : ShakeAngle.Value, ref lastHash);
			HashUtils.ContentHashOnto(ShakeStepFrame, ref lastHash);
			HashUtils.ContentHashOnto(ClearPreviousShake, ref lastHash);
			return lastHash;
		}
	}
}
