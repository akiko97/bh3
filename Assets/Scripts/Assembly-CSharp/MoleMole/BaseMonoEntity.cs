using UnityEngine;

namespace MoleMole
{
	public abstract class BaseMonoEntity : MonoBehaviour
	{
		protected uint _runtimeID;

		public abstract Vector3 XZPosition { get; }

		public abstract float TimeScale { get; }

		public uint GetRuntimeID()
		{
			return _runtimeID;
		}

		public abstract bool IsToBeRemove();

		public abstract bool IsActive();

		public abstract Transform GetAttachPoint(string name);

		protected virtual void OnDestroy()
		{
		}
	}
}
