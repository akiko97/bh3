using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginAudio : BaseMonoEffectPlugin
	{
		public EffectAudioItem[] enterPatternName;

		public string[] exitPatternName;

		private float _timer;

		private List<EffectAudioItem> _itemList = new List<EffectAudioItem>();

		public override void Setup()
		{
		}

		private void Update()
		{
			_timer += Time.deltaTime;
			for (int i = 0; i < _itemList.Count; i++)
			{
				if (_timer >= _itemList[i].delayTime)
				{
					if (_effect.owner == null || _effect.owner.GetComponent<Collider>() == null)
					{
						Singleton<WwiseAudioManager>.Instance.Post(_itemList[i].eventName);
					}
					else
					{
						Singleton<WwiseAudioManager>.Instance.Post(_itemList[i].eventName, _effect.owner.gameObject);
					}
					_itemList.RemoveAt(i);
					i--;
				}
			}
		}

		public override bool IsToBeRemove()
		{
			return false;
		}

		public override void SetDestroy()
		{
		}

		private void OnEnable()
		{
			_timer = 0f;
			_itemList.AddRange(enterPatternName);
		}

		private void OnDisable()
		{
			_timer = 0f;
			_itemList.Clear();
			int i = 0;
			for (int num = exitPatternName.Length; i < num; i++)
			{
				if (_effect.owner == null || _effect.owner.GetComponent<Collider>() == null)
				{
					Singleton<WwiseAudioManager>.Instance.Post(exitPatternName[i]);
				}
				else
				{
					Singleton<WwiseAudioManager>.Instance.Post(exitPatternName[i], _effect.owner.gameObject);
				}
			}
		}
	}
}
