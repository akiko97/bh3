using UnityEngine;

namespace MoleMole
{
	public class MonoIslandModel : MonoBehaviour
	{
		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private Renderer[] _extraRenderQueueArray;

		[SerializeField]
		private Transform _lock_disable_mesh;

		[SerializeField]
		private Material _lock_mat;

		[SerializeField]
		private Animation _lock_disable_animation;

		[SerializeField]
		private Transform[] _masked_disable_mesh;

		private Material _normal_mat;

		private CabinStatus _status = CabinStatus.UnLocked;

		public Renderer GetRenderer()
		{
			return _renderer;
		}

		public Renderer[] GetRenderer_RenderQueue()
		{
			return _extraRenderQueueArray;
		}

		public void RefreshLockStyle(CabinStatus _targetStatus)
		{
			if (_status == CabinStatus.UnLocked && _targetStatus == CabinStatus.Locked)
			{
				ToLockGraphic();
				_status = CabinStatus.Locked;
			}
			else if (_status == CabinStatus.Locked && _targetStatus == CabinStatus.UnLocked)
			{
				ToUnLockGraphic();
				_status = CabinStatus.UnLocked;
			}
		}

		private void ToLockGraphic()
		{
			if (_lock_disable_mesh != null)
			{
				_lock_disable_mesh.gameObject.SetActive(false);
			}
			if (_lock_disable_animation != null)
			{
				_lock_disable_animation.enabled = false;
			}
			if (_lock_mat != null)
			{
				_normal_mat = _renderer.material;
				_renderer.material = _lock_mat;
			}
		}

		private void ToUnLockGraphic()
		{
			if (_lock_disable_mesh != null)
			{
				_lock_disable_mesh.gameObject.SetActive(true);
			}
			if (_lock_disable_animation != null)
			{
				_lock_disable_animation.enabled = true;
			}
			if (_lock_mat != null)
			{
				_renderer.material = _normal_mat;
			}
		}

		public void ToMaskedGraphic()
		{
			if (_lock_disable_mesh != null)
			{
				_lock_disable_mesh.gameObject.SetActive(false);
			}
			if (_lock_disable_animation != null)
			{
				_lock_disable_animation.enabled = false;
			}
			Transform[] masked_disable_mesh = _masked_disable_mesh;
			foreach (Transform transform in masked_disable_mesh)
			{
				transform.gameObject.SetActive(false);
			}
		}

		public void ToUnMaskedGraphic()
		{
			if (_status == CabinStatus.UnLocked)
			{
				if (_lock_disable_mesh != null)
				{
					_lock_disable_mesh.gameObject.SetActive(true);
				}
				if (_lock_disable_animation != null)
				{
					_lock_disable_animation.enabled = true;
				}
				Transform[] masked_disable_mesh = _masked_disable_mesh;
				foreach (Transform transform in masked_disable_mesh)
				{
					transform.gameObject.SetActive(true);
				}
			}
		}
	}
}
