using System.Collections;
using UnityEngine;

namespace CinemaDirector
{
	public class CutsceneTrigger : MonoBehaviour
	{
		public StartMethod StartMethod;

		public Cutscene Cutscene;

		public GameObject TriggerObject;

		public string SkipButtonName = "Jump";

		public float Delay;

		private bool hasTriggered;

		private void Awake()
		{
			if (Cutscene != null)
			{
				Cutscene.Optimize();
			}
		}

		private void Start()
		{
			if (StartMethod == StartMethod.OnStart && Cutscene != null)
			{
				hasTriggered = true;
				StartCoroutine(PlayCutscene());
			}
		}

		private IEnumerator PlayCutscene()
		{
			yield return new WaitForSeconds(Delay);
			Cutscene.Play();
		}

		private void Update()
		{
			if ((SkipButtonName != null || SkipButtonName != string.Empty) && Input.GetButtonDown(SkipButtonName) && Cutscene != null && Cutscene.State == Cutscene.CutsceneState.Playing)
			{
				Cutscene.Skip();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!hasTriggered && other.gameObject == TriggerObject)
			{
				hasTriggered = true;
				Cutscene.Play();
			}
		}
	}
}
