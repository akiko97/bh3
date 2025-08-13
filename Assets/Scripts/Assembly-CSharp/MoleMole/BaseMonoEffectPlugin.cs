using UnityEngine;

namespace MoleMole
{
	[RequireComponent(typeof(MonoEffect))]
	public abstract class BaseMonoEffectPlugin : MonoBehaviour
	{
		protected MonoEffect _effect;

		protected virtual void Awake()
		{
			_effect = GetComponent<MonoEffect>();
		}

		protected virtual void OnValidate()
		{
			MonoEffect componentInParent = GetComponentInParent<MonoEffect>();
			if (!(componentInParent != null))
			{
			}
		}

		public virtual void Setup()
		{
		}

		public abstract bool IsToBeRemove();

		public abstract void SetDestroy();
	}
}
