using UnityEngine;

public class AkBankHandle
{
	private int m_RefCount;

	private uint m_BankID;

	public string bankName;

	public AkCallbackManager.BankCallback bankCallback;

	public int RefCount
	{
		get
		{
			return m_RefCount;
		}
	}

	public AkBankHandle(string name)
	{
		bankName = name;
		bankCallback = null;
	}

	public void LoadBank()
	{
		if (m_RefCount == 0)
		{
			AKRESULT aKRESULT = AkSoundEngine.LoadBank(bankName, -1, out m_BankID);
			if (aKRESULT != AKRESULT.AK_Success)
			{
				Debug.LogWarning("WwiseUnity: Bank " + bankName + " failed to load (" + aKRESULT.ToString() + ")");
			}
		}
		IncRef();
	}

	public void LoadBankAsync(AkCallbackManager.BankCallback callback = null)
	{
		if (m_RefCount == 0)
		{
			bankCallback = callback;
			AkSoundEngine.LoadBank(bankName, AkBankManager.GlobalBankCallback, this, -1, out m_BankID);
		}
		IncRef();
	}

	public void IncRef()
	{
		m_RefCount++;
	}

	public void DecRef()
	{
		m_RefCount--;
		if (m_RefCount == 0)
		{
			AkBankManager.BanksToUnload.Add(m_BankID);
		}
	}
}
