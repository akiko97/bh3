using UnityEngine;

public class AkAutoObject
{
	public int m_id;

	public AkAutoObject(GameObject GameObj)
	{
		m_id = GameObj.GetInstanceID();
		AkSoundEngine.RegisterGameObj(GameObj, "AkAutoObject.cs", 1u);
	}

	~AkAutoObject()
	{
		AkSoundEngine.UnregisterGameObjInternal(m_id);
	}
}
