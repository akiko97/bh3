using UnityEngine;

namespace MoleMole.MainMenu
{
	public class LightningObject
	{
		public float Size;

		public LightningType.LightningMaterial Mat;

		public float StartLifttime;

		public float Lifttime;

		public float DelayTime;

		public float Intensity;

		private bool _active;

		public GameObject _object;

		public Vector3 OrigScale;

		public bool Active
		{
			get
			{
				return _active;
			}
			set
			{
				_active = value;
				if (!_active && _object != null)
				{
					_object.SetActive(false);
				}
			}
		}

		public GameObject Object
		{
			get
			{
				if (_object == null)
				{
					Debug.LogError("Missing lightning quad object, please try restarting the particle system");
				}
				return _object;
			}
			set
			{
				_object = value;
			}
		}

		public void Show(bool show)
		{
			Object.SetActive(show);
		}

		public void UpdatePosition(Vector3 position)
		{
			Transform transform = Object.transform;
			transform.localScale = OrigScale * Size;
			transform.localPosition = position - Mat.pivot * Size;
		}
	}
}
