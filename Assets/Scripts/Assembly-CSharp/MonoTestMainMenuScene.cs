using MoleMole;
using UnityEngine;

public class MonoTestMainMenuScene : MonoBehaviour
{
	public RenderGroup[] renderGroups;

	private bool _toggled;

	private GUIStyle _style;

	private void OnEnable()
	{
		_style = new GUIStyle();
		Texture2D texture2D = new Texture2D(16, 16);
		Color[] array = new Color[256];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Color.gray;
		}
		texture2D.SetPixels(array);
		_style.normal.background = texture2D;
		Application.targetFrameRate = 60;
	}

	private void OnGUI()
	{
		if (_toggled)
		{
			GUI.color = Color.white;
			GUI.backgroundColor = Color.gray;
			GUILayout.BeginArea(new Rect(10f, 10f, 250f, Screen.height - 20), _style);
			for (int i = 0; i < renderGroups.Length; i++)
			{
				RenderGroup renderGroup = renderGroups[i];
				GUILayout.BeginHorizontal();
				GUILayout.Label(renderGroup.name);
				if (GUILayout.Button("Toggle", GUILayout.Width(100f), GUILayout.Height(40f)))
				{
					for (int j = 0; j < renderGroup.gameObjects.Length; j++)
					{
						renderGroup.gameObjects[j].SetActive(!renderGroup.gameObjects[j].activeSelf);
					}
				}
				GUILayout.EndHorizontal();
			}
			_toggled = !GUILayout.Button("Close", GUILayout.Height(50f));
			GUILayout.EndArea();
			GUILayout.BeginArea(new Rect(260f, 10f, 250f, Screen.height - 20), _style);
			GUILayout.Label(string.Format("FPS: {0}", 1f / Time.smoothDeltaTime));
			if (GUILayout.Button("PostFX Toggle", GUILayout.Height(50f)))
			{
				PostFXWithResScale postFXWithResScale = Object.FindObjectOfType<PostFXWithResScale>();
				if (postFXWithResScale != null)
				{
					postFXWithResScale.enabled = !postFXWithResScale.enabled;
				}
			}
			if (GUILayout.Button("HDR & HDR Buffer", GUILayout.Height(50f)))
			{
				PostFXWithResScale postFXWithResScale2 = Object.FindObjectOfType<PostFXWithResScale>();
				if (postFXWithResScale2 != null)
				{
					bool hdr = Camera.main.hdr;
					Camera.main.hdr = !hdr;
					postFXWithResScale2.HDRBuffer = !hdr;
				}
			}
			if (GUILayout.Button("FXAA", GUILayout.Height(50f)))
			{
				PostFXWithResScale postFXWithResScale3 = Object.FindObjectOfType<PostFXWithResScale>();
				if (postFXWithResScale3 != null)
				{
					postFXWithResScale3.FXAA = !postFXWithResScale3.FXAA;
				}
			}
			if (GUILayout.Button("Distortion Map", GUILayout.Height(50f)))
			{
				PostFXWithResScale postFXWithResScale4 = Object.FindObjectOfType<PostFXWithResScale>();
				if (postFXWithResScale4 != null)
				{
					postFXWithResScale4.UseDistortion = !postFXWithResScale4.UseDistortion;
				}
			}
			if (GUILayout.Button("Distortion Apply", GUILayout.Height(50f)))
			{
				PostFXWithResScale postFXWithResScale5 = Object.FindObjectOfType<PostFXWithResScale>();
				if (postFXWithResScale5 != null)
				{
					postFXWithResScale5.UseDistortion = !postFXWithResScale5.UseDistortion;
				}
			}
			if (GUILayout.Button("Use Distortion Depth Test", GUILayout.Height(50f)))
			{
				PostFXWithResScale postFXWithResScale6 = Object.FindObjectOfType<PostFXWithResScale>();
				if (postFXWithResScale6 != null)
				{
					postFXWithResScale6.UseDepthTest = !postFXWithResScale6.UseDepthTest;
				}
			}
			GUILayout.EndArea();
		}
		else
		{
			_toggled = GUI.Button(new Rect(10f, 10f, 150f, 50f), "Render Scene");
		}
	}
}
