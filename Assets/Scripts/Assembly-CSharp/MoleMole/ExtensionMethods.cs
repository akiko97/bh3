using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MoleMole
{
	public static class ExtensionMethods
	{
		public static void SetPositionX(this Transform transform, float x)
		{
			Vector3 position = transform.position;
			position.x = x;
			transform.position = position;
		}

		public static void SetPositionY(this Transform transform, float y)
		{
			Vector3 position = transform.position;
			position.y = y;
			transform.position = position;
		}

		public static void SetPositionZ(this Transform transform, float z)
		{
			Vector3 position = transform.position;
			position.z = z;
			transform.position = position;
		}

		public static void AddPositionX(this Transform transform, float x)
		{
			Vector3 position = transform.position;
			position.x += x;
			transform.position = position;
		}

		public static void AddPositionY(this Transform transform, float y)
		{
			Vector3 position = transform.position;
			position.y += y;
			transform.position = position;
		}

		public static void AddPositionZ(this Transform transform, float z)
		{
			Vector3 position = transform.position;
			position.z += z;
			transform.position = position;
		}

		public static void SubPositionX(this Transform transform, float x)
		{
			transform.AddPositionX(0f - x);
		}

		public static void SubPositionY(this Transform transform, float y)
		{
			transform.AddPositionY(0f - y);
		}

		public static void SubPositionZ(this Transform transform, float z)
		{
			transform.AddPositionZ(0f - z);
		}

		public static void SetPlatformPosition(this Transform transform, Vector3 pos)
		{
			Vector3 position = transform.position;
			position.x = pos.x;
			position.z = pos.z;
			transform.position = position;
		}

		public static Transform AddChildFromPrefab(this Transform trans, Transform prefab, string name = null)
		{
			Transform transform = UnityEngine.Object.Instantiate(prefab);
			transform.SetParent(trans, false);
			if (name != null)
			{
				transform.gameObject.name = name;
			}
			return transform;
		}

		public static void DestroyChildren(this Transform trans)
		{
			foreach (Transform tran in trans)
			{
				UnityEngine.Object.Destroy(tran.gameObject);
			}
		}

		public static void SetParentAndReset(this Transform trans, Transform parent)
		{
			trans.SetParent(parent, false);
			trans.localPosition = Vector3.zero;
			trans.localRotation = Quaternion.identity;
		}

		public static GameObject SetLayer(this GameObject go, int layer, bool setChildren = false)
		{
			go.layer = layer;
			if (setChildren)
			{
				Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					renderer.gameObject.layer = go.layer;
				}
			}
			return go;
		}

		public static bool HasDirectChild(this Transform parent, Transform child)
		{
			for (int i = 0; i < parent.childCount; i++)
			{
				if (parent.GetChild(i) == child)
				{
					return true;
				}
			}
			return false;
		}

		public static void SetLocalPositionX(this Transform transform, float x)
		{
			Vector3 localPosition = transform.localPosition;
			localPosition.x = x;
			transform.localPosition = localPosition;
		}

		public static void SetLocalPositionY(this Transform transform, float y)
		{
			Vector3 localPosition = transform.localPosition;
			localPosition.y = y;
			transform.localPosition = localPosition;
		}

		public static void SetLocalPositionZ(this Transform transform, float z)
		{
			Vector3 localPosition = transform.localPosition;
			localPosition.z = z;
			transform.localPosition = localPosition;
		}

		public static void AddLocalPositionX(this Transform transform, float x)
		{
			Vector3 localPosition = transform.localPosition;
			localPosition.x += x;
			transform.localPosition = localPosition;
		}

		public static void AddLocalPositionY(this Transform transform, float y)
		{
			Vector3 localPosition = transform.localPosition;
			localPosition.y += y;
			transform.localPosition = localPosition;
		}

		public static void AddLocalPositionZ(this Transform transform, float z)
		{
			Vector3 localPosition = transform.localPosition;
			localPosition.z += z;
			transform.localPosition = localPosition;
		}

		public static void SubLocalPositionX(this Transform transform, float x)
		{
			transform.AddLocalPositionX(0f - x);
		}

		public static void SubLocalPositionY(this Transform transform, float y)
		{
			transform.AddLocalPositionY(0f - y);
		}

		public static void SubLocalPositionZ(this Transform transform, float z)
		{
			transform.AddLocalPositionZ(0f - z);
		}

		public static void SetLocalScaleX(this Transform transform, float x)
		{
			Vector3 localScale = transform.localScale;
			localScale.x = x;
			transform.localScale = localScale;
		}

		public static void SetLocalScaleY(this Transform transform, float y)
		{
			Vector3 localScale = transform.localScale;
			localScale.y = y;
			transform.localScale = localScale;
		}

		public static void SetLocalScaleZ(this Transform transform, float z)
		{
			Vector3 localScale = transform.localScale;
			localScale.z = z;
			transform.localScale = localScale;
		}

		public static void AddLocalScaleX(this Transform transform, float x)
		{
			Vector3 localScale = transform.localScale;
			localScale.x += x;
			transform.localScale = localScale;
		}

		public static void AddLocalScaleY(this Transform transform, float y)
		{
			Vector3 localScale = transform.localScale;
			localScale.y += y;
			transform.localScale = localScale;
		}

		public static void AddLocalScaleZ(this Transform transform, float z)
		{
			Vector3 localScale = transform.localScale;
			localScale.z += z;
			transform.localScale = localScale;
		}

		public static void SubLocalScaleX(this Transform transform, float x)
		{
			transform.AddLocalScaleX(0f - x);
		}

		public static void SubLocalScaleY(this Transform transform, float y)
		{
			transform.AddLocalScaleY(0f - y);
		}

		public static void SubLocalScaleZ(this Transform transform, float z)
		{
			transform.AddLocalScaleZ(0f - z);
		}

		public static void SetLocalEulerAnglesX(this Transform transform, float x)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			localEulerAngles.x = x;
			transform.localEulerAngles = localEulerAngles;
		}

		public static void SetLocalEulerAnglesY(this Transform transform, float y)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			localEulerAngles.y = y;
			transform.localEulerAngles = localEulerAngles;
		}

		public static void SetLocalEulerAnglesZ(this Transform transform, float z)
		{
			if (!(transform == null))
			{
				Vector3 localEulerAngles = transform.localEulerAngles;
				localEulerAngles.z = z;
				transform.localEulerAngles = localEulerAngles;
			}
		}

		public static void AddLocalEulerAnglesX(this Transform transform, float x)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			localEulerAngles.x += x;
			transform.localEulerAngles = localEulerAngles;
		}

		public static void AddLocalEulerAnglesY(this Transform transform, float y)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			localEulerAngles.y += y;
			transform.localEulerAngles = localEulerAngles;
		}

		public static void AddLocalEulerAnglesZ(this Transform transform, float z)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			localEulerAngles.z += z;
			transform.localEulerAngles = localEulerAngles;
		}

		public static void SubLocalEulerAnglesX(this Transform transform, float x)
		{
			transform.AddLocalEulerAnglesX(0f - x);
		}

		public static void SubLocalEulerAnglesY(this Transform transform, float y)
		{
			transform.AddLocalEulerAnglesY(0f - y);
		}

		public static void SubLocalEulerAnglesZ(this Transform transform, float z)
		{
			transform.AddLocalEulerAnglesZ(0f - z);
		}

		public static void SetAlpha(this Material material, float a)
		{
			material.color = new Color(material.color.r, material.color.g, material.color.b, a);
		}

		public static void SetStartAlpha(this ParticleSystem particleSystem, float a)
		{
			particleSystem.startColor = new Color(particleSystem.startColor.r, particleSystem.startColor.g, particleSystem.startColor.b, a);
		}

		public static void RemoveAllNulls<T>(this List<T> ls)
		{
			for (int i = 0; i < ls.Count; i++)
			{
				if (ls[i] == null)
				{
					ls.RemoveAt(i);
					i--;
				}
			}
		}

		public static void RemoveNullKeys<T1, T2>(this Dictionary<T1, T2> dict) where T1 : Component
		{
			List<T1> list = new List<T1>();
			foreach (T1 key in dict.Keys)
			{
				if (key == null)
				{
					list.Add(key);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				dict.Remove(list[i]);
			}
		}

		public static int SeekAddPosition<T>(this List<T> ls) where T : class
		{
			for (int i = 0; i < ls.Count; i++)
			{
				if (ls[i] == null)
				{
					return i;
				}
			}
			ls.Add((T)null);
			return ls.Count - 1;
		}

		public static int SeekAddPosition(this List<int> ls)
		{
			for (int i = 0; i < ls.Count; i++)
			{
				if (ls[i] == -1)
				{
					return i;
				}
			}
			ls.Add(-1);
			return ls.Count - 1;
		}

		public static void ExpandToInclude<T>(this List<T> ls, int ix) where T : class
		{
			int num = ix - ls.Count + 1;
			while (num-- > 0)
			{
				ls.Add((T)null);
			}
		}

		public static bool HasParameter(this Animator animator, string parameterKey)
		{
			for (int i = 0; i < animator.parameters.Length; i++)
			{
				AnimatorControllerParameter animatorControllerParameter = animator.parameters[i];
				if (parameterKey == animatorControllerParameter.name)
				{
					return true;
				}
			}
			return false;
		}

		public static T ToEnum<T>(this string value, [Optional] T defaultValue)
		{
			try
			{
				return (T)Enum.Parse(typeof(T), value);
			}
			catch
			{
				return defaultValue;
			}
		}

		public static bool ContainsLayer(this LayerMask layerMask, int layer)
		{
			return (layerMask.value & (1 << layer)) != 0;
		}

		public static bool ContainsState(this AbilityState abilityState, AbilityState targetState)
		{
			return (abilityState & targetState) != 0;
		}

		public static bool ContainsTag(this AttackResult.AttackCategoryTag tag, AttackResult.AttackCategoryTag targetTag)
		{
			return (tag & targetTag) != 0;
		}

		public static bool IsRemoteSend(this IdentityRemoteMode mode)
		{
			return mode == IdentityRemoteMode.SendAndNoReceive || mode == IdentityRemoteMode.SendAndReceive;
		}

		public static bool IsRemoteReceive(this IdentityRemoteMode mode)
		{
			return mode == IdentityRemoteMode.ReceiveAndNoSend || mode == IdentityRemoteMode.SendAndReceive;
		}
	}
}
