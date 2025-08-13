using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Wwise/AkGameObj")]
public class AkGameObj : MonoBehaviour
{
	private const int ALL_LISTENER_MASK = 255;

	public AkGameObjPositionOffsetData m_positionOffsetData;

	public bool isEnvironmentAware;

	public AkGameObjEnvironmentData m_envData;

	public int listenerMask = 1;

	[SerializeField]
	private bool isStaticObject;

	private AkGameObjPositionData m_posData;

	private Bounds GameObjColliderBounds;

	private void Awake()
	{
		if (!isStaticObject)
		{
			m_posData = new AkGameObjPositionData();
		}
		Collider component = GetComponent<Collider>();
		if (component != null)
		{
			GameObjColliderBounds = component.bounds;
		}
		else
		{
			GameObjColliderBounds = new Bounds(Vector3.zero, Vector3.one);
		}
		AKRESULT aKRESULT = AkSoundEngine.RegisterGameObj(base.gameObject, base.gameObject.name, (uint)(listenerMask & 0xFF));
		if (aKRESULT == AKRESULT.AK_Success)
		{
			Vector3 position = GetPosition();
			AkSoundEngine.SetObjectPosition(base.gameObject, position.x, position.y, position.z, base.transform.forward.x, base.transform.forward.y, base.transform.forward.z);
			if (isEnvironmentAware)
			{
				m_envData = new AkGameObjEnvironmentData();
				AddAuxSend(base.gameObject);
			}
		}
	}

	private void CheckStaticStatus()
	{
	}

	private void OnEnable()
	{
		base.enabled = !isStaticObject;
	}

	private void OnDestroy()
	{
		AkUnityEventHandler[] components = base.gameObject.GetComponents<AkUnityEventHandler>();
		AkUnityEventHandler[] array = components;
		foreach (AkUnityEventHandler akUnityEventHandler in array)
		{
			if (akUnityEventHandler.triggerList.Contains(-358577003))
			{
				akUnityEventHandler.DoDestroy();
			}
		}
		if (AkSoundEngine.IsInitialized())
		{
			AkSoundEngine.UnregisterGameObj(base.gameObject);
		}
	}

	private void Update()
	{
		if (isStaticObject)
		{
			return;
		}
		Vector3 position = GetPosition();
		if (!(m_posData.position == position) || !(m_posData.forward == base.transform.forward))
		{
			m_posData.position = position;
			m_posData.forward = base.transform.forward;
			AkSoundEngine.SetObjectPosition(base.gameObject, position.x, position.y, position.z, base.transform.forward.x, base.transform.forward.y, base.transform.forward.z);
			if (isEnvironmentAware)
			{
				UpdateAuxSend();
			}
		}
	}

	public Vector3 GetPosition()
	{
		if (m_positionOffsetData != null)
		{
			Vector3 vector = base.transform.rotation * m_positionOffsetData.positionOffset;
			return base.transform.position + vector;
		}
		return base.transform.position;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (isEnvironmentAware)
		{
			AddAuxSend(other.gameObject);
		}
	}

	private void AddAuxSend(GameObject in_AuxSendObject)
	{
		AkEnvironmentPortal component = in_AuxSendObject.GetComponent<AkEnvironmentPortal>();
		if (component != null)
		{
			m_envData.activePortals.Add(component);
			for (int i = 0; i < component.environments.Length; i++)
			{
				if (component.environments[i] != null)
				{
					int num = m_envData.activeEnvironments.BinarySearch(component.environments[i], AkEnvironment.s_compareByPriority);
					if (num < 0)
					{
						m_envData.activeEnvironments.Insert(~num, component.environments[i]);
					}
				}
			}
			m_envData.auxSendValues = null;
			UpdateAuxSend();
			return;
		}
		AkEnvironment component2 = in_AuxSendObject.GetComponent<AkEnvironment>();
		if (component2 != null)
		{
			int num2 = m_envData.activeEnvironments.BinarySearch(component2, AkEnvironment.s_compareByPriority);
			if (num2 < 0)
			{
				m_envData.activeEnvironments.Insert(~num2, component2);
				m_envData.auxSendValues = null;
				UpdateAuxSend();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!isEnvironmentAware)
		{
			return;
		}
		AkEnvironmentPortal component = other.gameObject.GetComponent<AkEnvironmentPortal>();
		if (component != null)
		{
			for (int i = 0; i < component.environments.Length; i++)
			{
				if (component.environments[i] != null && !GameObjColliderBounds.Intersects(component.environments[i].GetComponent<Collider>().bounds))
				{
					m_envData.activeEnvironments.Remove(component.environments[i]);
				}
			}
			m_envData.activePortals.Remove(component);
			m_envData.auxSendValues = null;
			UpdateAuxSend();
			return;
		}
		AkEnvironment component2 = other.gameObject.GetComponent<AkEnvironment>();
		if (!(component2 != null))
		{
			return;
		}
		for (int j = 0; j < m_envData.activePortals.Count; j++)
		{
			for (int k = 0; k < m_envData.activePortals[j].environments.Length; k++)
			{
				if (component2 == m_envData.activePortals[j].environments[k])
				{
					m_envData.auxSendValues = null;
					UpdateAuxSend();
					return;
				}
			}
		}
		m_envData.activeEnvironments.Remove(component2);
		m_envData.auxSendValues = null;
		UpdateAuxSend();
	}

	private void UpdateAuxSend()
	{
		if (m_envData.auxSendValues == null)
		{
			m_envData.auxSendValues = new AkAuxSendArray((uint)((m_envData.activeEnvironments.Count >= AkEnvironment.MAX_NB_ENVIRONMENTS) ? AkEnvironment.MAX_NB_ENVIRONMENTS : m_envData.activeEnvironments.Count));
		}
		else
		{
			m_envData.auxSendValues.Reset();
		}
		for (int i = 0; i < m_envData.activePortals.Count; i++)
		{
			for (int j = 0; j < m_envData.activePortals[i].environments.Length; j++)
			{
				AkEnvironment akEnvironment = m_envData.activePortals[i].environments[j];
				if (akEnvironment != null && m_envData.activeEnvironments.BinarySearch(akEnvironment, AkEnvironment.s_compareByPriority) < AkEnvironment.MAX_NB_ENVIRONMENTS)
				{
					m_envData.auxSendValues.Add(akEnvironment.GetAuxBusID(), m_envData.activePortals[i].GetAuxSendValueForPosition(base.transform.position, j));
				}
			}
		}
		if (m_envData.auxSendValues.m_Count < AkEnvironment.MAX_NB_ENVIRONMENTS && m_envData.auxSendValues.m_Count < m_envData.activeEnvironments.Count)
		{
			List<AkEnvironment> list = new List<AkEnvironment>(m_envData.activeEnvironments);
			list.Sort(AkEnvironment.s_compareBySelectionAlgorithm);
			int num = Math.Min(AkEnvironment.MAX_NB_ENVIRONMENTS - (int)m_envData.auxSendValues.m_Count, m_envData.activeEnvironments.Count - (int)m_envData.auxSendValues.m_Count);
			for (int k = 0; k < num; k++)
			{
				if (!m_envData.auxSendValues.Contains(list[k].GetAuxBusID()) && (!list[k].isDefault || k == 0))
				{
					m_envData.auxSendValues.Add(list[k].GetAuxBusID(), list[k].GetAuxSendValueForPosition(base.transform.position));
					if (list[k].excludeOthers)
					{
						break;
					}
				}
			}
		}
		AkSoundEngine.SetGameObjectAuxSendValues(base.gameObject, m_envData.auxSendValues, m_envData.auxSendValues.m_Count);
	}
}
