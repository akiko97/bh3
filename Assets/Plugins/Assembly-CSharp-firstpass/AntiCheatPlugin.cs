using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using MoleMole;
using UnityEngine;

public class AntiCheatPlugin : MonoBehaviour
{
	private const string DLL = "hsoda";

	private static string _filename;

	public static bool isEnabled;

	public static bool isCheating;

	private void Awake()
	{
		_filename = Path.Combine(Application.persistentDataPath, "proclist");
		setfilepath(_filename);
	}

	public static void Init(bool enabled, List<string> libList, List<string> procList)
	{
		isEnabled = enabled;
		isCheating = false;
		if (libList.Count <= 0 && procList.Count <= 0)
		{
			return;
		}
		clear();
		foreach (string lib in libList)
		{
			mapsInsert(lib);
		}
		foreach (string proc in procList)
		{
			procInsert(proc);
		}
	}

	public static bool Detect()
	{
		if (isEnabled && !isCheating && (procrun() != 0 || procmaps() != 0))
		{
			isCheating = true;
			writeproc();
			return true;
		}
		return false;
	}

	public static byte[] ReadProcList()
	{
		try
		{
			return File.ReadAllBytes(_filename);
		}
		catch (Exception ex)
		{
			string text = ex.ToString();
			SuperDebug.VeryImportantError(text);
			return Encoding.ASCII.GetBytes(text);
		}
	}

	[DllImport("hsoda")]
	private static extern int anti();

	[DllImport("hsoda")]
	private static extern int procrun();

	[DllImport("hsoda")]
	private static extern int procmaps();

	[DllImport("hsoda")]
	private static extern int writeproc();

	[DllImport("hsoda")]
	private static extern void setfilepath(string path);

	[DllImport("hsoda")]
	private static extern void mapsInsert(string str);

	[DllImport("hsoda")]
	private static extern void procInsert(string str);

	[DllImport("hsoda")]
	private static extern void clear();
}
