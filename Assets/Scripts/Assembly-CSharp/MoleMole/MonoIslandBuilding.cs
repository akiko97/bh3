using UnityEngine;

namespace MoleMole
{
	public class MonoIslandBuilding : MonoBehaviour
	{
		[SerializeField]
		private Vector3 _landedOffset;

		[SerializeField]
		private float _landedPitch;

		[SerializeField]
		private Vector3 _focusOffset;

		[SerializeField]
		private float _foucusPitch;

		public float highlight_polygon_offset = -2000f;

		[SerializeField]
		private float _highlight_bloom_factor = 1f;

		[SerializeField]
		private float _normal_bloom_factor = 0.3f;

		private Vector3 _landedPos;

		private Vector3 _focusPos;

		private MonoIslandBuildingsUtil _util;

		private Renderer _mainRenderer;

		private int index_highLightMat = -1;

		private E_AlphaLerpDir _lerpDir;

		private float _startTimeLerp;

		private MonoIslandModel _model;

		private void Awake()
		{
			_util = base.transform.parent.GetComponent<MonoIslandBuildingsUtil>();
		}

		private void Update()
		{
			UpdateHighLightLerp();
		}

		public Vector3 GetLandedOffset()
		{
			return _landedOffset;
		}

		public Vector3 GetLandedPos()
		{
			_landedPos = base.transform.position + _landedOffset;
			return _landedPos;
		}

		public float GetLandedPitch()
		{
			return _landedPitch;
		}

		public Vector3 GetFocusPos()
		{
			_focusPos = base.transform.position + _focusOffset;
			return _focusPos;
		}

		public float GetFocusPitch()
		{
			return _foucusPitch;
		}

		public void UpdateBuildingWhenExtend(string buildingPath)
		{
			Transform transform = base.transform.Find("Building");
			transform.DestroyChildren();
			Transform transform2 = Object.Instantiate(Resources.Load<GameObject>(buildingPath)).transform;
			transform2.SetParent(transform, false);
			_model = transform2.GetComponent<MonoIslandModel>();
			_mainRenderer = _model.GetRenderer();
		}

		public void SetRenderQueue(E_IslandRenderQueue queue)
		{
			_mainRenderer.material.renderQueue = (int)queue;
			Renderer[] renderer_RenderQueue = _model.GetRenderer_RenderQueue();
			for (int i = 0; i < renderer_RenderQueue.Length; i++)
			{
				renderer_RenderQueue[i].material.renderQueue = (int)queue;
			}
		}

		public void AddHighLightMat(Renderer renderer)
		{
			if (renderer == null)
			{
				renderer = _mainRenderer;
			}
			int num = renderer.materials.Length + 1;
			Material[] array = new Material[num];
			for (int i = 0; i < renderer.materials.Length; i++)
			{
				array[i] = renderer.materials[i];
			}
			array[num - 1] = _util._highLightMat;
			renderer.materials = array;
			index_highLightMat = num - 1;
		}

		private void RemoveHighLightMat(Renderer renderer)
		{
			int num = renderer.materials.Length - 1;
			Material[] array = new Material[num];
			for (int i = 0; i < renderer.materials.Length - 1; i++)
			{
				array[i] = renderer.materials[i];
			}
			renderer.materials = array;
			index_highLightMat = -1;
		}

		public void SetHighLightAlpha(float t)
		{
			_mainRenderer.materials[index_highLightMat].SetFloat("_Opaqueness", t);
		}

		public void SetHighLightBloomFactor(float t)
		{
			_mainRenderer.materials[index_highLightMat].SetFloat("_BloomFactor", t);
		}

		public void SetPolygonOffset(float offset)
		{
			_mainRenderer.material.SetFloat("_PolygonOffset", offset);
			Renderer[] renderer_RenderQueue = _model.GetRenderer_RenderQueue();
			for (int i = 0; i < renderer_RenderQueue.Length; i++)
			{
				renderer_RenderQueue[i].material.SetFloat("_PolygonOffset", offset);
			}
		}

		public void TriggerHighLight(E_AlphaLerpDir dir)
		{
			_lerpDir = dir;
			_startTimeLerp = Time.time;
		}

		private void UpdateHighLightLerp()
		{
			if (_lerpDir == E_AlphaLerpDir.None)
			{
				return;
			}
			if (_lerpDir == E_AlphaLerpDir.ToLarge && index_highLightMat < 0)
			{
				AddHighLightMat(_mainRenderer);
			}
			float num = (Time.time - _startTimeLerp) / _util._highLightLerpDuration;
			if (num > 1f)
			{
				float num2 = ((_lerpDir != E_AlphaLerpDir.ToLarge) ? 0f : 1f);
				SetHighLightAlpha(num2);
				float highLightBloomFactor = ((_lerpDir != E_AlphaLerpDir.ToLarge) ? _normal_bloom_factor : _highlight_bloom_factor);
				SetHighLightBloomFactor(highLightBloomFactor);
				float num3 = ((_lerpDir != E_AlphaLerpDir.ToLarge) ? _util._ratioOfOpaquenessToPolygonOffsetBack : _util._ratioOfOpaquenessToPolygonOffsetFront);
				SetPolygonOffset(Mathf.Lerp(0f, highlight_polygon_offset, Mathf.Clamp01(num2 / num3)));
				if (_lerpDir == E_AlphaLerpDir.ToLittle && index_highLightMat > 0)
				{
					RemoveHighLightMat(_mainRenderer);
				}
				_lerpDir = E_AlphaLerpDir.None;
			}
			else
			{
				float num4 = ((_lerpDir != E_AlphaLerpDir.ToLarge) ? (1f - num) : num);
				SetHighLightAlpha(num4);
				float highLightBloomFactor2 = Mathf.Lerp(_normal_bloom_factor, _highlight_bloom_factor, (_lerpDir != E_AlphaLerpDir.ToLarge) ? (1f - num) : num);
				SetHighLightBloomFactor(highLightBloomFactor2);
				float num5 = ((_lerpDir != E_AlphaLerpDir.ToLarge) ? _util._ratioOfOpaquenessToPolygonOffsetBack : _util._ratioOfOpaquenessToPolygonOffsetFront);
				SetPolygonOffset(Mathf.Lerp(0f, highlight_polygon_offset, Mathf.Clamp01(num4 / num5)));
			}
		}

		public MonoIslandModel GetModel()
		{
			return _model;
		}
	}
}
