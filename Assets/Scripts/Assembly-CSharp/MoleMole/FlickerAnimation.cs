using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Renderer))]
	public class FlickerAnimation : MonoBehaviour
	{
		public float playbackSpeed = 1f;

		public float scaler = 1f;

		public AnimationCurve frameOverTime = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public int materialId;

		public int divider = 3;

		public bool isFlicker;

		public bool isLoop = true;

		private float emissionScaler = 1f;

		private float _time;

		private Material _material;

		public new string name = "_EmissionScaler";

		private int tick;

		private void Start()
		{
			Preparation();
		}

		private void Preparation()
		{
			_material = GetComponent<Renderer>().materials[materialId];
			emissionScaler = _material.GetFloat(name);
			if (isLoop)
			{
				frameOverTime.preWrapMode = WrapMode.Loop;
				frameOverTime.postWrapMode = WrapMode.Loop;
			}
		}

		public void Update()
		{
			float value;
			if (isFlicker)
			{
				value = ((tick % divider != 1) ? emissionScaler : (emissionScaler * scaler));
				_material.SetFloat(name, value);
				tick++;
				return;
			}
			_time += Time.deltaTime * playbackSpeed;
			_time += 1f / 60f * playbackSpeed;
			if (!isLoop && _time > 1f)
			{
				_time = 1f;
			}
			value = emissionScaler + frameOverTime.Evaluate(_time) * scaler;
			_material.SetFloat(name, value);
		}

		private void OnDestroy()
		{
			if (_material != null)
			{
				Object.DestroyImmediate(_material);
			}
		}
	}
}
