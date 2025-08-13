using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoLightShadowManager : MonoBehaviour
	{
		public LightProbProperties properties;

		private MonoLightShadowGroup[] _groups;

		public void Init()
		{
			MonoLightShadowGroup[] componentsInChildren = GetComponentsInChildren<MonoLightShadowGroup>(true);
			List<MonoLightShadowGroup> list = new List<MonoLightShadowGroup>();
			MonoLightShadowGroup[] array = componentsInChildren;
			foreach (MonoLightShadowGroup monoLightShadowGroup in array)
			{
				if (Mathf.Abs(monoLightShadowGroup.transform.position.y) < 0.2f)
				{
					monoLightShadowGroup.Init();
					list.Add(monoLightShadowGroup);
				}
				else
				{
					monoLightShadowGroup.gameObject.SetActive(false);
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
			MonoLightShadowGroup[] groups = _groups;
			foreach (MonoLightShadowGroup monoLightShadowGroup in groups)
			{
				LightProbProperties ret2 = default(LightProbProperties);
				if (monoLightShadowGroup.Evaluate(pos, ref ret2))
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
