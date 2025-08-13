using System.Collections.Generic;
using LuaInterface;

namespace MoleMole
{
	public class LDWaitPointInSight : BaseLDEvent
	{
		private List<MonoSpawnPoint> _pointList;

		public LDWaitPointInSight(LuaTable spawnPoints)
		{
			_pointList = new List<MonoSpawnPoint>();
			foreach (object value in spawnPoints.Values)
			{
				int namedSpawnPointIx = Singleton<StageManager>.Instance.GetStageEnv().GetNamedSpawnPointIx(value as string);
				_pointList.Add(Singleton<StageManager>.Instance.GetStageEnv().spawnPoints[namedSpawnPointIx]);
			}
		}

		public override void Core()
		{
			foreach (MonoSpawnPoint point in _pointList)
			{
				if (Singleton<LevelDesignManager>.Instance.IsPointInCameraFov(point.transform.position))
				{
					Done();
				}
			}
		}
	}
}
