using UnityEngine;

namespace MoleMole
{
	public class GalTouchTestAvatar : MonoBehaviour, IBodyPartTouchable
	{
		private GalTouchSystem galTouchSystem;

		public int avatarId = 101;

		public int heartLevel = 1;

		public Renderer leftEyeRenderer;

		public Renderer rightEyeRenderer;

		public Renderer mouthRenderer;

		public TestMatInfoProvider leftEyeProvider;

		public TestMatInfoProvider rightEyeProvider;

		public TestMatInfoProvider mouthProvider;

		public Transform headRoot;

		public GameObject[] switchObjects;

		private void Start()
		{
			galTouchSystem = new GalTouchSystem();
			ResetGalTouchSystem();
			MonoBodyPart[] componentsInChildren = GetComponentsInChildren<MonoBodyPart>();
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				componentsInChildren[i].SetBodyPartTouchable(this);
			}
			galTouchSystem.enable = true;
			galTouchSystem.IdleChanged += OnGalTouchSystemIdleChanged;
		}

		private void Update()
		{
			if (galTouchSystem != null)
			{
				galTouchSystem.Process(Time.deltaTime);
			}
		}

		public void ResetGalTouchSystem()
		{
			galTouchSystem.Init(GetComponent<Animator>(), avatarId, heartLevel, leftEyeRenderer, rightEyeRenderer, mouthRenderer, leftEyeProvider, rightEyeProvider, mouthProvider, headRoot);
			galTouchSystem.enable = true;
		}

		public void BodyPartTouched(BodyPartType type, Vector3 point)
		{
			galTouchSystem.BodyPartTouched(type);
		}

		public void TriggerAudioPattern(string name)
		{
			Singleton<WwiseAudioManager>.Instance.Post(name);
		}

		private void OnGalTouchSystemIdleChanged(bool idle)
		{
			if (idle)
			{
				int i = 0;
				for (int num = switchObjects.Length; i < num; i++)
				{
					switchObjects[i].SetActive(false);
				}
			}
		}

		public void SwitchOn(string name)
		{
			int i = 0;
			for (int num = switchObjects.Length; i < num; i++)
			{
				if (switchObjects[i].name == name)
				{
					switchObjects[i].SetActive(true);
					break;
				}
			}
		}

		public void SwitchOff(string name)
		{
			int i = 0;
			for (int num = switchObjects.Length; i < num; i++)
			{
				if (switchObjects[i].name == name)
				{
					switchObjects[i].SetActive(false);
					break;
				}
			}
		}
	}
}
