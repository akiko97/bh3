using UnityEngine;

namespace MoleMole
{
	public class SizedPaster : Paster
	{
		[Header("Use targetTransform's Y position for Y sizing")]
		public Transform TargetTransform;

		[Header("Min/Max Heights")]
		public float MinHeight = 1f;

		public float MaxHeight = 4f;

		private float _origSize;

		protected override void Start()
		{
			base.transform.forward = Vector3.down;
			base.Start();
			_origSize = Size;
		}

		protected override void Update()
		{
			float num = ((!(TargetTransform.position.y < MinHeight)) ? TargetTransform.position.y : MinHeight);
			Size = _origSize * Mathf.Lerp(1f, 0.4f, (num - MinHeight) / (MaxHeight - MinHeight));
			Vector3 position = TargetTransform.position;
			position.y = num;
			base.transform.position = position;
			base.transform.forward = Vector3.down;
			base.Update();
		}
	}
}
