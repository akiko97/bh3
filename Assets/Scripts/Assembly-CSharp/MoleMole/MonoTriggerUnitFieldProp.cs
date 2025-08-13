using UnityEngine;

namespace MoleMole
{
	public class MonoTriggerUnitFieldProp : MonoTriggerProp
	{
		private int _numberX;

		private int _numberZ;

		private float _xUnitLength;

		private float _zUnitLength;

		private Vector3 _childrenOffset;

		private Vector3 _targetForward;

		protected override void Awake()
		{
			base.Awake();
			_triggerCollider.enabled = false;
		}

		public virtual void EnableProp()
		{
			_triggerCollider.enabled = true;
		}

		public virtual void DisableProp()
		{
			_triggerCollider.enabled = false;
		}

		public virtual void InitUnitFieldPropRange(int numberX, int numberZ)
		{
			_targetForward = base.transform.forward;
			base.transform.forward = Vector3.forward;
			_numberX = numberX;
			_numberZ = numberZ;
			GetUnitLength();
			ResetColliderSize();
			CopyModels();
		}

		private void GetUnitLength()
		{
			_xUnitLength = config.PropArguments.Length;
			_zUnitLength = config.PropArguments.Length;
		}

		private void ResetColliderSize()
		{
			BoxCollider boxCollider = (BoxCollider)_triggerCollider;
			Vector3 center = boxCollider.center;
			boxCollider.size = new Vector3(_xUnitLength * (float)_numberX, boxCollider.size.y, _zUnitLength * (float)_numberZ);
			Vector3 forward = Vector3.forward;
			Vector3 right = Vector3.right;
			_childrenOffset = center + right * (boxCollider.size.x - _xUnitLength) / 2f + forward * (boxCollider.size.z - _zUnitLength) / 2f;
		}

		private void CopyModels()
		{
			Transform child = base.gameObject.transform.GetChild(0);
			Vector3 forward = Vector3.forward;
			Vector3 right = Vector3.right;
			for (int i = 0; i < _numberX; i++)
			{
				for (int j = 0; j < _numberZ; j++)
				{
					if (i != 0 || j != 0)
					{
						Transform transform = Object.Instantiate(child);
						transform.SetParent(base.gameObject.transform);
						transform.localRotation = Quaternion.Euler(Vector3.zero);
						transform.localPosition = right * _xUnitLength * i + forward * _zUnitLength * j;
					}
				}
			}
			foreach (Transform item in base.gameObject.transform)
			{
				item.localPosition += new Vector3(0f - _childrenOffset.x, 0f, 0f - _childrenOffset.z);
			}
			base.transform.forward = _targetForward;
		}
	}
}
