using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(Renderer))]
	public class UVSheetAnimation : MonoBehaviour
	{
		public enum OffsetMode
		{
			Auto = 0,
			Manual = 1
		}

		[Header("Common Option")]
		public OffsetMode offsetMode;

		[Tooltip("The material id of this renderer to apply on")]
		public int materialId;

		public float playbackSpeed = 1f;

		[Tooltip("If do interpolation between frames")]
		public bool doInterpolation;

		public bool isLoop = true;

		public AnimationCurve frameOverTime = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[Header("Auto Mode")]
		public int tiles_X = 1;

		public int tiles_Y = 1;

		[Tooltip("Only select tiles in this list")]
		public int[] tileFilterList;

		[Tooltip("If need to scale uv to fit a frame")]
		public bool needScale;

		[Header("Manual Mode")]
		public Vector2[] offsetList;

		private float _time;

		private int _totalFrames = 1;

		private int _selectedTotalFrames = 1;

		private float _step_X;

		private float _step_Y;

		private Material _material;

		private void Start()
		{
			Preparation();
		}

		private void OnEnable()
		{
			Preparation();
		}

		private void OnDisable()
		{
		}

		private void Preparation()
		{
			Material[] materials = GetComponent<Renderer>().materials;
			_material = materials[materialId];
			_totalFrames = tiles_X * tiles_Y;
			_step_X = 1f / (float)tiles_X;
			_step_Y = 1f / (float)tiles_Y;
			if (isLoop)
			{
				frameOverTime.preWrapMode = WrapMode.Loop;
				frameOverTime.postWrapMode = WrapMode.Loop;
			}
			if (tileFilterList.Length == 0)
			{
				tileFilterList = new int[_totalFrames];
				for (int i = 0; i < _totalFrames; i++)
				{
					tileFilterList[i] = i;
				}
			}
			_selectedTotalFrames = tileFilterList.Length;
		}

		public void Update()
		{
			if (Application.isPlaying)
			{
				_time += Time.deltaTime * playbackSpeed;
			}
			else
			{
				_time += 1f / 60f * playbackSpeed;
			}
			if (!isLoop && _time > 1f)
			{
				_time = 1f;
			}
			float time = frameOverTime.Evaluate(_time);
			Play(time);
		}

		private Vector2 OffsetOfFrame(int frame)
		{
			if (offsetMode == OffsetMode.Auto)
			{
				Vector2 result = default(Vector2);
				frame %= _selectedTotalFrames;
				frame = tileFilterList[frame];
				result.x = (float)(frame % tiles_X) * _step_X;
				result.y = (float)(tiles_Y - 1 - frame / tiles_X) * _step_Y;
				return result;
			}
			frame %= offsetList.Length;
			return offsetList[frame];
		}

		private void Play(float time)
		{
			time = Mathf.Clamp01(time);
			float num = time * (float)_selectedTotalFrames;
			int num2 = (int)num;
			if (needScale)
			{
				_material.SetTextureScale("_MainTex", new Vector2(_step_X, _step_Y));
			}
			Vector2 vector = OffsetOfFrame(num2);
			_material.SetTextureOffset("_MainTex", vector);
			if (doInterpolation)
			{
				_material.SetVector("_nextFrameOffset", OffsetOfFrame(num2 + 1) - vector);
				_material.SetFloat("_frameInterpolationFactor", num - (float)num2);
			}
			else
			{
				_material.SetVector("_nextFrameOffset", vector);
				_material.SetFloat("_frameInterpolationFactor", 0f);
			}
		}
	}
}
