using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole.Config
{
	public class ConfigAnimatorEntity
	{
		[NonSerialized]
		public ConfigCommonEntity CommonConfig;

		public ConfigBindAnimatorStateToParameter[] AnimatorStateParamBinds = ConfigBindAnimatorStateToParameter.EMPTY;

		public ConfigMPArguments MPArguments;

		[NonSerialized]
		public Dictionary<int, AnimatorStateToParameterConfig> StateToParamBindMap;

		public virtual void OnLevelLoaded()
		{
			StateToParamBindMap = new Dictionary<int, AnimatorStateToParameterConfig>();
			ConfigBindAnimatorStateToParameter[] animatorStateParamBinds = AnimatorStateParamBinds;
			foreach (ConfigBindAnimatorStateToParameter configBindAnimatorStateToParameter in animatorStateParamBinds)
			{
				string[] animatorStateNames = configBindAnimatorStateToParameter.AnimatorStateNames;
				foreach (string name in animatorStateNames)
				{
					StateToParamBindMap.Add(Animator.StringToHash(name), configBindAnimatorStateToParameter.ParameterConfig);
				}
			}
		}
	}
}
