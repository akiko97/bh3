using FullInspector;
using UnityEngine;

namespace MoleMole.Config
{
	public class ConfigTrailData : BaseScriptableObject
	{
		public TrailData properties;

		public static ConfigTrailData CreateByTrailData(TrailData trailData)
		{
			ConfigTrailData configTrailData = ScriptableObject.CreateInstance<ConfigTrailData>();
			configTrailData.properties = new TrailData(trailData.clockwise, trailData.points, trailData.totalPolar, trailData.startPolarOffset, trailData.radiusX, trailData.radiusY, trailData.liftY, trailData.rotate, trailData.position);
			return configTrailData;
		}
	}
}
