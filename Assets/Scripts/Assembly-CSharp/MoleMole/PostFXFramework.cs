using UnityEngine;

namespace MoleMole
{
	public abstract class PostFXFramework : MonoBehaviour
	{
		protected bool _isSupported = true;

		protected virtual void OnEnable()
		{
		}

		protected void NotSupported()
		{
			base.enabled = false;
			_isSupported = false;
		}

		protected Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
		{
			if (!s)
			{
				Debug.LogError("Missing shader in " + this);
				base.enabled = false;
				return null;
			}
			if (s.isSupported && (bool)m2Create && m2Create.shader == s)
			{
				return m2Create;
			}
			if (!s.isSupported)
			{
				NotSupported();
				Debug.LogError(string.Concat("The shader ", s, " on effect ", this, " is not supported on this platform!"));
				return null;
			}
			m2Create = new Material(s);
			m2Create.hideFlags = HideFlags.DontSave;
			if ((bool)m2Create)
			{
				return m2Create;
			}
			return null;
		}

		protected void CreateCamera(Camera srcCam, ref Camera destCam, string name = null, HideFlags hideFlags = HideFlags.HideAndDontSave)
		{
			if (srcCam != null && destCam == null)
			{
				string text = ((!string.IsNullOrEmpty(name)) ? name : ("__RefCamera for " + srcCam.GetInstanceID()));
				GameObject gameObject = new GameObject(text, typeof(Camera));
				destCam = gameObject.GetComponent<Camera>();
				destCam.enabled = false;
				gameObject.hideFlags = hideFlags;
			}
		}
	}
}
