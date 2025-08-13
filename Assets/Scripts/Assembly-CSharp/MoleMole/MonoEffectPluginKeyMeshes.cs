using UnityEngine;

namespace MoleMole
{
	public class MonoEffectPluginKeyMeshes : BaseMonoEffectPlugin
	{
		[Header("Drag consequent key transforms into this.")]
		public Transform[] keyedTransforms;

		[Header("How many frames does the keyed transform last")]
		public int[] keyedFrames;

		private float[] _keyedTime;

		private float _timer;

		private int _curKey;

		protected override void Awake()
		{
			base.Awake();
			_timer = 0f;
			float num = 0f;
			_keyedTime = new float[keyedFrames.Length];
			for (int i = 0; i < _keyedTime.Length; i++)
			{
				num += (float)keyedFrames[i] * (1f / 60f);
				_keyedTime[i] = num;
			}
		}

		public override void Setup()
		{
			for (int i = 0; i < keyedTransforms.Length; i++)
			{
				keyedTransforms[i].gameObject.SetActive(false);
			}
			_curKey = 0;
			keyedTransforms[_curKey].gameObject.SetActive(true);
			_timer = 0f;
		}

		protected void Update()
		{
			if (_curKey >= _keyedTime.Length)
			{
				return;
			}
			_timer += Time.deltaTime * _effect.TimeScale;
			int curKey = _curKey;
			while (_timer > _keyedTime[_curKey])
			{
				_curKey++;
				if (_curKey == _keyedTime.Length)
				{
					keyedTransforms[_keyedTime.Length - 1].gameObject.SetActive(false);
					return;
				}
			}
			if (curKey != _curKey)
			{
				keyedTransforms[curKey].gameObject.SetActive(false);
				keyedTransforms[_curKey].gameObject.SetActive(true);
			}
		}

		public override bool IsToBeRemove()
		{
			return _curKey >= _keyedTime.Length;
		}

		public override void SetDestroy()
		{
			keyedTransforms[_curKey].gameObject.SetActive(false);
			_curKey = _keyedTime.Length;
		}
	}
}
