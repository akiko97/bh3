using System.Diagnostics;
using UnityEngine;

public static class HPProfile
{
	private static Stopwatch stopWatch = new Stopwatch();

	public static void Begin()
	{
		stopWatch.Reset();
		stopWatch.Start();
	}

	public static void End(string prompt)
	{
		stopWatch.Stop();
		UnityEngine.Debug.Log(string.Format("{0} : {1}", prompt, ((float)stopWatch.ElapsedTicks * 1000f / (float)Stopwatch.Frequency).ToString()));
	}
}
