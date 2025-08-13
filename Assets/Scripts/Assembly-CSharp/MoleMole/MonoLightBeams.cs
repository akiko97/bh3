using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	[ExecuteInEditMode]
	public class MonoLightBeams : MonoBehaviour
	{
		private struct Beams
		{
			public float amplitude;

			public float cycleTime;

			public float phaseTime;

			public Transform transform;
		}

		[Header("Swing amplitude range (degree)")]
		public Vector2 amplitudeRange = new Vector2(30f, 60f);

		[Header("Swing cycle time range")]
		public Vector2 cycleTimeRange = new Vector2(5f, 20f);

		private Beams[] beams;

		private void OnEnable()
		{
			Init();
		}

		private void OnDisable()
		{
		}

		private void OnDestroy()
		{
		}

		private void Init()
		{
			List<Beams> list = new List<Beams>();
			int seed = UnityEngine.Random.seed;
			UnityEngine.Random.seed = 0;
			Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				if (!(transform == base.transform))
				{
					Beams item = default(Beams);
					item.amplitude = UnityEngine.Random.Range(amplitudeRange.x, amplitudeRange.y);
					item.cycleTime = Mathf.Max(UnityEngine.Random.Range(cycleTimeRange.x, cycleTimeRange.y), 0.01f);
					item.phaseTime = UnityEngine.Random.Range(0f, item.cycleTime);
					item.transform = transform;
					list.Add(item);
				}
			}
			UnityEngine.Random.seed = seed;
			beams = list.ToArray();
		}

		private void Update()
		{
			float num = ((!Application.isPlaying) ? (1f / 30f) : Time.deltaTime);
			for (int i = 0; i < beams.Length; i++)
			{
				beams[i].phaseTime += num;
				float f = beams[i].phaseTime * 2f * (float)Math.PI / beams[i].cycleTime;
				float num2 = Mathf.Sin(f);
				beams[i].transform.SetLocalEulerAnglesZ(num2 * beams[0].amplitude);
			}
		}
	}
}
