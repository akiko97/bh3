using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace MoleMole
{
	public class MutilUpdateCD : Action
	{
		public class CDInfo
		{
			public SharedFloat CD;

			public float defaultTime;

			public bool isRandom;

			public float minRandTime;

			public float maxRandTime;

			public bool keepUpdating;

			[BehaviorDesigner.Runtime.Tasks.Tooltip("if NoUpdate < 0, CD will never update (for unique monster)")]
			public SharedFloat NoUpdate;

			public void InitOnAwake()
			{
				if (isRandom)
				{
					CD.Value = Random.Range(minRandTime, maxRandTime);
				}
				else
				{
					CD.Value = defaultTime;
				}
			}
		}

		public List<CDInfo> cdList;

		private IAIEntity _aiEntity;

		public override void OnAwake()
		{
			BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
			if (component is BaseMonoAvatar)
			{
				_aiEntity = (BaseMonoAvatar)component;
			}
			else if (component is BaseMonoMonster)
			{
				_aiEntity = (BaseMonoMonster)component;
			}
			foreach (CDInfo cd in cdList)
			{
				cd.InitOnAwake();
			}
		}

		public override void OnStart()
		{
		}

		public override TaskStatus OnUpdate()
		{
			foreach (CDInfo cd in cdList)
			{
				if ((cd.keepUpdating || cd.CD.Value >= 0f) && cd.NoUpdate.Value >= 0f)
				{
					cd.CD.Value -= Time.deltaTime * _aiEntity.TimeScale;
				}
			}
			return TaskStatus.Running;
		}
	}
}
