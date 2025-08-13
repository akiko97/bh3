using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class AkMemBankLoader : MonoBehaviour
{
	private const int WaitMs = 50;

	private const long AK_BANK_PLATFORM_DATA_ALIGNMENT = 16L;

	private const long AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK = 15L;

	public string bankName = string.Empty;

	public bool isLocalizedBank;

	private WWW ms_www;

	private GCHandle ms_pinnedArray;

	private IntPtr ms_pInMemoryBankPtr = IntPtr.Zero;

	[HideInInspector]
	public uint ms_bankID;

	private string m_bankPath;

	private void Start()
	{
		if (isLocalizedBank)
		{
			LoadLocalizedBank(bankName);
		}
		else
		{
			LoadNonLocalizedBank(bankName);
		}
	}

	public void LoadNonLocalizedBank(string in_bankFilename)
	{
		string in_bankPath = "file://" + Path.Combine(AkBasePathGetter.GetPlatformBasePath(), in_bankFilename);
		DoLoadBank(in_bankPath);
	}

	public void LoadLocalizedBank(string in_bankFilename)
	{
		string in_bankPath = "file://" + Path.Combine(Path.Combine(AkBasePathGetter.GetPlatformBasePath(), AkInitializer.GetCurrentLanguage()), in_bankFilename);
		DoLoadBank(in_bankPath);
	}

	private IEnumerator LoadFile()
	{
		ms_www = new WWW(m_bankPath);
		yield return ms_www;
		uint in_uInMemoryBankSize = 0u;
		try
		{
			ms_pinnedArray = GCHandle.Alloc(ms_www.bytes, GCHandleType.Pinned);
			ms_pInMemoryBankPtr = ms_pinnedArray.AddrOfPinnedObject();
			in_uInMemoryBankSize = (uint)ms_www.bytes.Length;
			if ((ms_pInMemoryBankPtr.ToInt64() & 0xF) != 0L)
			{
				byte[] alignedBytes = new byte[(long)ms_www.bytes.Length + 16L];
				GCHandle new_pinnedArray = GCHandle.Alloc(alignedBytes, GCHandleType.Pinned);
				IntPtr new_pInMemoryBankPtr = new_pinnedArray.AddrOfPinnedObject();
				int alignedOffset = 0;
				if ((new_pInMemoryBankPtr.ToInt64() & 0xF) != 0L)
				{
					long alignedPtr = (new_pInMemoryBankPtr.ToInt64() + 15) & -16;
					alignedOffset = (int)(alignedPtr - new_pInMemoryBankPtr.ToInt64());
					new_pInMemoryBankPtr = new IntPtr(alignedPtr);
				}
				Array.Copy(ms_www.bytes, 0, alignedBytes, alignedOffset, ms_www.bytes.Length);
				ms_pInMemoryBankPtr = new_pInMemoryBankPtr;
				ms_pinnedArray.Free();
				ms_pinnedArray = new_pinnedArray;
			}
		}
		catch
		{
			yield break;
		}
		AKRESULT result = AkSoundEngine.LoadBank(ms_pInMemoryBankPtr, in_uInMemoryBankSize, out ms_bankID);
		if (result != AKRESULT.AK_Success)
		{
			Debug.LogError("WwiseUnity: AkMemBankLoader: bank loading failed with result " + result);
		}
	}

	private void DoLoadBank(string in_bankPath)
	{
		m_bankPath = in_bankPath;
		StartCoroutine(LoadFile());
	}

	private void OnDestroy()
	{
		if (ms_pInMemoryBankPtr != IntPtr.Zero)
		{
			AKRESULT aKRESULT = AkSoundEngine.UnloadBank(ms_bankID, ms_pInMemoryBankPtr);
			if (aKRESULT == AKRESULT.AK_Success)
			{
				ms_pinnedArray.Free();
			}
		}
	}
}
