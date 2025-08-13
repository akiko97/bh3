using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoLightProbManager : MonoBehaviour
	{
		public LightProbProperties properties;

		private MonoLightProbGroup[] _groups;

		public void Init()
		{
			MonoLightProbGroup[] componentsInChildren = GetComponentsInChildren<MonoLightProbGroup>(true);
			List<MonoLightProbGroup> list = new List<MonoLightProbGroup>();
			MonoLightProbGroup[] array = componentsInChildren;
			foreach (MonoLightProbGroup monoLightProbGroup in array)
			{
				if (Mathf.Abs(monoLightProbGroup.transform.position.y) < 2f)
				{
					monoLightProbGroup.Init();
					list.Add(monoLightProbGroup);
					monoLightProbGroup.gameObject.SetActive(true);
				}
				else
				{
					monoLightProbGroup.gameObject.SetActive(false);
				}
			}
			_groups = list.ToArray();
		}

		public void Reset()
		{
			Init();
		}

		public bool Evaluate(Vector3 pos, ref LightProbProperties ret)
		{
			if (_groups == null)
			{
				return false;
			}
			ret = default(LightProbProperties);
			int num = 0;
			MonoLightProbGroup[] groups = _groups;
			foreach (MonoLightProbGroup monoLightProbGroup in groups)
			{
				LightProbProperties ret2 = default(LightProbProperties);
				if (monoLightProbGroup.Evaluate(pos, ref ret2))
				{
					num++;
					ret += ret2;
				}
			}
			if (num != 0)
			{
				ret /= (float)num;
			}
			else
			{
				ret = properties;
			}
			return true;
		}
	}
}
