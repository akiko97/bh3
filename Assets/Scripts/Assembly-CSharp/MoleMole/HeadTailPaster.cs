using UnityEngine;

namespace MoleMole
{
	public class HeadTailPaster : Paster
	{
		[Header("Head Target Transform")]
		public Transform HeadTransform;

		[Header("Tail Target Transform")]
		public Transform TailTransform;

		private float _origSize;

		protected override void Start()
		{
			base.Start();
			_origSize = Size;
		}

		protected override void Update()
		{
			Vector3 position = (HeadTransform.position + TailTransform.position) * 0.5f;
			Vector3 normalized = (HeadTransform.position - TailTransform.position).normalized;
			float num = ((!(position.y < 1f)) ? position.y : 1f);
			Size = _origSize * Mathf.Lerp(1f, 0.4f, (num - 1f) / 3f);
			float y = position.y;
			position.y = 0f;
			normalized.y = 0f;
			_pasterTrsf.position = position;
			_pasterTrsf.forward = normalized;
			_pasterTrsf.localScale = base.transform.localScale * Size;
			float value = Mathf.Clamp01(1f - (y - FalloffStartDistance) / FalloffEndDistance);
			_material.SetFloat("_Falloff", value);
		}
	}
}
