using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoUIAnimationHelper : MonoBehaviour
	{
		[Serializable]
		public class ConfigAnimPattern
		{
			public string name;

			public Animation[] subAnims;
		}

		public ConfigAnimPattern[] patterns;

		private Dictionary<string, Animation[]> _animMap;

		public void Awake()
		{
			_animMap = new Dictionary<string, Animation[]>();
			if (patterns != null)
			{
				ConfigAnimPattern[] array = patterns;
				foreach (ConfigAnimPattern configAnimPattern in array)
				{
					_animMap.Add(configAnimPattern.name, configAnimPattern.subAnims);
				}
			}
		}

		[AnimationCallback]
		public void DestroyContext(string contextName)
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AnimDestroyContext, contextName));
		}

		[AnimationCallback]
		public void AnimCallback(string callBackStr)
		{
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AnimCallBack, callBackStr));
		}

		[AnimationCallback]
		public void PlayPattern(string patternName)
		{
			Animation[] array = _animMap[patternName];
			foreach (Animation animation in array)
			{
				if (animation != null && animation.gameObject.activeSelf)
				{
					animation.Play();
				}
			}
		}
	}
}
