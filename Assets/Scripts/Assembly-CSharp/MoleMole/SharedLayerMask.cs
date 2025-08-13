using BehaviorDesigner.Runtime;
using UnityEngine;

namespace MoleMole
{
	public class SharedLayerMask : SharedVariable<LayerMask>
	{
		public override string ToString()
		{
			return mValue.ToString();
		}

		public static implicit operator SharedLayerMask(LayerMask value)
		{
			SharedLayerMask sharedLayerMask = new SharedLayerMask();
			sharedLayerMask.mValue = value;
			return sharedLayerMask;
		}
	}
}
