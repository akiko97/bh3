using System;
using LuaInterface;
using UnityEngine;

namespace MoleMole
{
	public class LetMeCrash : MonoBehaviour
	{
		public static string orUrl = string.Empty;

		public static string orUpload = string.Empty;

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		private unsafe void OnGUI()
		{
			if (GUILayout.Button("Let Me Crash", GUILayout.MinWidth(200f), GUILayout.MinHeight(100f)))
			{
				LuaDLL.lua_call((IntPtr)(void*)null, 1, 1);
			}
			orUrl = DrawField("Url", orUrl);
			orUpload = DrawField("Upload", orUpload);
		}

		private string DrawField(string label, string val)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(label);
			string result = GUILayout.TextField((val != null) ? val : string.Empty);
			GUILayout.EndHorizontal();
			return result;
		}
	}
}
