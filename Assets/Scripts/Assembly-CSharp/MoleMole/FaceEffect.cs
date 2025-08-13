using UnityEngine;

namespace MoleMole
{
	public class FaceEffect
	{
		private MonoFaceEffect _effect;

		public void Init(MonoFaceEffect effect)
		{
			_effect = effect;
		}

		public void Uninit()
		{
			if (_effect != null)
			{
				Object.Destroy(_effect.gameObject);
				_effect = null;
			}
		}

		public void ShowEffect(string name)
		{
			GameObject effectByName = GetEffectByName(name);
			if (effectByName != null)
			{
				effectByName.SetActive(true);
			}
		}

		public void HideEffect(string name)
		{
			GameObject effectByName = GetEffectByName(name);
			if (effectByName != null)
			{
				effectByName.SetActive(false);
			}
		}

		public void HideAll()
		{
			int i = 0;
			for (int num = _effect.items.Length; i < num; i++)
			{
				_effect.items[i].effect.SetActive(false);
			}
		}

		private GameObject GetEffectByName(string name)
		{
			int i = 0;
			for (int num = _effect.items.Length; i < num; i++)
			{
				if (_effect.items[i].name == name)
				{
					return _effect.items[i].effect;
				}
			}
			return null;
		}
	}
}
