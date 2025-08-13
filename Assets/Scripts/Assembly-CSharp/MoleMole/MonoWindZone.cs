using UnityEngine;

namespace MoleMole
{
	public class MonoWindZone : MonoBehaviour
	{
		public float LeavesMagnitude = 0.05f;

		public float LeavesFrequency = 0.05f;

		public float RotateMagnitude = 0.05f;

		public float RotateFrequency = 0.07f;

		public float BushesMagnitude = 0.1f;

		public float BushesFrequency = 0.05f;

		[Header("Objects to effect")]
		public GameObject TrunksObject;

		public GameObject LeavesRootObject;

		private Transform _trsf;

		private float _time;

		private float _TimeScale
		{
			get
			{
				LevelManager instance = Singleton<LevelManager>.Instance;
				if (instance == null)
				{
					return 1f;
				}
				return instance.levelEntity.TimeScale;
			}
		}

		public void Awake()
		{
			_trsf = base.transform;
			_time = 0f;
		}

		public void Init()
		{
			TreesPreprocessor treesPreprocessor = new TreesPreprocessor(TrunksObject, LeavesRootObject);
			treesPreprocessor.Process();
		}

		private void Update()
		{
			_time += Time.deltaTime * _TimeScale;
			Vector3 forward = _trsf.forward;
			Shader.SetGlobalVector("_miHoYo_Wind", new Vector4(forward.x, forward.y, forward.z, _time));
			Shader.SetGlobalVector("_miHoYo_WindParams1", new Vector4(LeavesMagnitude, LeavesFrequency, RotateMagnitude, RotateFrequency));
			Shader.SetGlobalVector("_miHoYo_WindParams2", new Vector4(BushesMagnitude, BushesFrequency));
		}
	}
}
