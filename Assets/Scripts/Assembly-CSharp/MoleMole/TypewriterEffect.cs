using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MoleMole
{
	[RequireComponent(typeof(Text))]
	[AddComponentMenu("Typewriter Effect")]
	public class TypewriterEffect : MonoBehaviour
	{
		public UnityEvent myEvent;

		public int charsPerSecond;

		private bool isActive;

		private float timer;

		private string words;

		private Text mText;

		private void Start()
		{
			if (myEvent == null)
			{
				myEvent = new UnityEvent();
			}
			words = GetComponent<Text>().text;
			GetComponent<Text>().text = string.Empty;
			timer = 0f;
			isActive = true;
			charsPerSecond = Mathf.Max(1, charsPerSecond);
			mText = GetComponent<Text>();
		}

		private void ReloadText()
		{
			words = GetComponent<Text>().text;
			mText = GetComponent<Text>();
			timer = 0f;
		}

		public void OnStart()
		{
			ReloadText();
			isActive = true;
		}

		public void OnEnable()
		{
			ReloadText();
			isActive = true;
		}

		public void RestartRead()
		{
			ReloadText();
			isActive = true;
		}

		public void Finish()
		{
			OnFinish();
		}

		private void OnStartWriter()
		{
			if (isActive)
			{
				try
				{
					mText.text = words.Substring(0, (int)((float)charsPerSecond * timer));
					timer += Time.deltaTime;
				}
				catch (Exception)
				{
					OnFinish();
				}
			}
		}

		private void OnFinish()
		{
			isActive = false;
			timer = 0f;
			GetComponent<Text>().text = words;
			try
			{
				myEvent.Invoke();
			}
			catch (Exception)
			{
			}
		}

		private void Update()
		{
			OnStartWriter();
		}
	}
}
