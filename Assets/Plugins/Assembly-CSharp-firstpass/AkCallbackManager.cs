using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class AkCallbackManager
{
	public class EventCallbackPackage
	{
		public object m_Cookie;

		public EventCallback m_Callback;

		public bool m_bNotifyEndOfEvent;

		public uint m_playingID;

		public static EventCallbackPackage Create(EventCallback in_cb, object in_cookie, ref uint io_Flags)
		{
			if (io_Flags == 0 || in_cb == null)
			{
				io_Flags = 0u;
				return null;
			}
			EventCallbackPackage eventCallbackPackage = new EventCallbackPackage();
			eventCallbackPackage.m_Callback = in_cb;
			eventCallbackPackage.m_Cookie = in_cookie;
			eventCallbackPackage.m_bNotifyEndOfEvent = (io_Flags & 1) != 0;
			io_Flags |= 1u;
			m_mapEventCallbacks[eventCallbackPackage.GetHashCode()] = eventCallbackPackage;
			m_LastAddedEventPackage = eventCallbackPackage;
			return eventCallbackPackage;
		}
	}

	public class BankCallbackPackage
	{
		public object m_Cookie;

		public BankCallback m_Callback;

		public BankCallbackPackage(BankCallback in_cb, object in_cookie)
		{
			m_Callback = in_cb;
			m_Cookie = in_cookie;
			m_mapBankCallbacks[GetHashCode()] = this;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct AkCommonCallback
	{
		public IntPtr pPackage;

		public IntPtr pNext;

		public AkCallbackType eType;
	}

	public struct AkEventCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint eventID;
	}

	public struct AkDynamicSequenceItemCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint audioNodeID;

		public IntPtr pCustomInfo;
	}

	public struct AkMidiEventCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint eventID;

		public byte byType;

		public byte byChan;

		public byte byParam1;

		public byte byParam2;

		public byte byOnOffNote;

		public byte byVelocity;

		public byte byCc;

		public byte byCcValue;

		public byte byValueLsb;

		public byte byValueMsb;

		public byte byAftertouchNote;

		public byte byNoteAftertouchValue;

		public byte byChanAftertouchValue;

		public byte byProgramNum;
	}

	public struct AkMarkerCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint eventID;

		public uint uIdentifier;

		public uint uPosition;

		public string strLabel;
	}

	public struct AkDurationCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint eventID;

		public float fDuration;

		public float fEstimatedDuration;

		public uint audioNodeID;
	}

	public class AkMusicSyncCallbackInfoBase
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public AkCallbackType musicSyncType;

		public float fBeatDuration;

		public float fBarDuration;

		public float fGridDuration;

		public float fGridOffset;
	}

	public class AkMusicSyncCallbackInfo : AkMusicSyncCallbackInfoBase
	{
		public string pszUserCueName;
	}

	public struct AkMonitoringMsg
	{
		public ErrorCode errorCode;

		public ErrorLevel errorLevel;

		public uint playingID;

		public IntPtr gameObjID;

		public string msg;
	}

	public struct AkBankInfo
	{
		public uint bankID;

		public IntPtr inMemoryBankPtr;

		public AKRESULT eLoadResult;

		public uint memPoolId;
	}

	public delegate void EventCallback(object in_cookie, AkCallbackType in_type, object in_info);

	public delegate void MonitoringCallback(ErrorCode in_errorCode, ErrorLevel in_errorLevel, uint in_playingID, IntPtr in_gameObjID, string in_msg);

	public delegate void BankCallback(uint in_bankID, IntPtr in_InMemoryBankPtr, AKRESULT in_eLoadResult, uint in_memPoolId, object in_Cookie);

	private static Dictionary<int, EventCallbackPackage> m_mapEventCallbacks = new Dictionary<int, EventCallbackPackage>();

	private static Dictionary<int, BankCallbackPackage> m_mapBankCallbacks = new Dictionary<int, BankCallbackPackage>();

	private static EventCallbackPackage m_LastAddedEventPackage = null;

	private static IntPtr m_pNotifMem;

	private static MonitoringCallback m_MonitoringCB;

	private static byte[] floatMarshalBuffer = new byte[4];

	public static void RemoveEventCallback(uint in_playingID)
	{
		foreach (KeyValuePair<int, EventCallbackPackage> mapEventCallback in m_mapEventCallbacks)
		{
			if (mapEventCallback.Value.m_playingID == in_playingID)
			{
				m_mapEventCallbacks.Remove(mapEventCallback.Key);
				break;
			}
		}
	}

	public static List<int> RemoveEventCallbackCookie(object in_cookie)
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, EventCallbackPackage> mapEventCallback in m_mapEventCallbacks)
		{
			if (mapEventCallback.Value.m_Cookie == in_cookie)
			{
				list.Add(mapEventCallback.Key);
			}
		}
		foreach (int item in list)
		{
			m_mapEventCallbacks.Remove(item);
		}
		return list;
	}

	public static List<int> RemoveBankCallback(object in_cookie)
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, BankCallbackPackage> mapBankCallback in m_mapBankCallbacks)
		{
			if (mapBankCallback.Value.m_Cookie == in_cookie)
			{
				list.Add(mapBankCallback.Key);
			}
		}
		foreach (int item in list)
		{
			m_mapEventCallbacks.Remove(item);
		}
		return list;
	}

	public static void SetLastAddedPlayingID(uint in_playingID)
	{
		if (m_LastAddedEventPackage != null && m_LastAddedEventPackage.m_playingID == 0)
		{
			m_LastAddedEventPackage.m_playingID = in_playingID;
		}
	}

	public static AKRESULT Init()
	{
		m_pNotifMem = Marshal.AllocHGlobal(4096);
		return AkCallbackSerializer.Init(m_pNotifMem, 4096u);
	}

	public static void Term()
	{
		AkCallbackSerializer.Term();
		Marshal.FreeHGlobal(m_pNotifMem);
		m_pNotifMem = IntPtr.Zero;
	}

	public static void SetMonitoringCallback(ErrorLevel in_Level, MonitoringCallback in_CB)
	{
		AkCallbackSerializer.SetLocalOutput((uint)in_Level);
		m_MonitoringCB = in_CB;
	}

	public static void PostCallbacks()
	{
		if (m_pNotifMem == IntPtr.Zero)
		{
			return;
		}
		IntPtr pData = AkCallbackSerializer.Lock();
		if (pData == IntPtr.Zero)
		{
			AkCallbackSerializer.Unlock();
			return;
		}
		AkCommonCallback akCommonCallback = default(AkCommonCallback);
		akCommonCallback.eType = (AkCallbackType)0;
		akCommonCallback.pPackage = IntPtr.Zero;
		akCommonCallback.pNext = IntPtr.Zero;
		IntPtr intPtr = pData;
		akCommonCallback = new AkCommonCallback
		{
			pPackage = Marshal.ReadIntPtr(pData)
		};
		GotoEndOfCurrentStructMember_IntPtr(ref pData);
		akCommonCallback.pNext = Marshal.ReadIntPtr(pData);
		GotoEndOfCurrentStructMember_IntPtr(ref pData);
		akCommonCallback.eType = (AkCallbackType)Marshal.ReadInt32(pData);
		GotoEndOfCurrentStructMember_EnumType<AkCallbackType>(ref pData);
		EventCallbackPackage eventPkg = null;
		BankCallbackPackage bankPkg = null;
		if (!SafeExtractCallbackPackages(akCommonCallback, out eventPkg, out bankPkg))
		{
			AkCallbackSerializer.Unlock();
			return;
		}
		pData = intPtr;
		while (true)
		{
			pData = (IntPtr)(pData.ToInt64() + Marshal.SizeOf(typeof(AkCommonCallback)));
			if (akCommonCallback.eType == AkCallbackType.AK_Monitoring)
			{
				AkMonitoringMsg akMonitoringMsg = new AkMonitoringMsg
				{
					errorCode = (ErrorCode)Marshal.ReadInt32(pData)
				};
				GotoEndOfCurrentStructMember_ValueType<int>(ref pData);
				akMonitoringMsg.errorLevel = (ErrorLevel)Marshal.ReadInt32(pData);
				GotoEndOfCurrentStructMember_ValueType<int>(ref pData);
				akMonitoringMsg.playingID = (uint)Marshal.ReadInt32(pData);
				GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
				akMonitoringMsg.gameObjID = Marshal.ReadIntPtr(pData);
				GotoEndOfCurrentStructMember_IntPtr(ref pData);
				akMonitoringMsg.msg = SafeMarshalString(pData);
				if (m_MonitoringCB != null)
				{
					m_MonitoringCB(akMonitoringMsg.errorCode, akMonitoringMsg.errorLevel, akMonitoringMsg.playingID, akMonitoringMsg.gameObjID, akMonitoringMsg.msg);
				}
			}
			else if (akCommonCallback.eType == AkCallbackType.AK_Bank)
			{
				AkBankInfo akBankInfo = new AkBankInfo
				{
					bankID = (uint)Marshal.ReadInt32(pData)
				};
				GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
				akBankInfo.inMemoryBankPtr = Marshal.ReadIntPtr(pData);
				GotoEndOfCurrentStructMember_ValueType<IntPtr>(ref pData);
				akBankInfo.eLoadResult = (AKRESULT)Marshal.ReadInt32(pData);
				GotoEndOfCurrentStructMember_EnumType<AKRESULT>(ref pData);
				akBankInfo.memPoolId = (uint)Marshal.ReadInt32(pData);
				GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
				if (bankPkg != null && bankPkg.m_Callback != null)
				{
					bankPkg.m_Callback(akBankInfo.bankID, akBankInfo.inMemoryBankPtr, akBankInfo.eLoadResult, akBankInfo.memPoolId, bankPkg.m_Cookie);
				}
			}
			else
			{
				switch (akCommonCallback.eType)
				{
				case AkCallbackType.AK_EndOfEvent:
				{
					AkEventCallbackInfo akEventCallbackInfo = new AkEventCallbackInfo
					{
						pCookie = Marshal.ReadIntPtr(pData)
					};
					GotoEndOfCurrentStructMember_IntPtr(ref pData);
					akEventCallbackInfo.gameObjID = Marshal.ReadIntPtr(pData);
					GotoEndOfCurrentStructMember_IntPtr(ref pData);
					akEventCallbackInfo.playingID = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					akEventCallbackInfo.eventID = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					if (eventPkg.m_bNotifyEndOfEvent)
					{
						eventPkg.m_Callback(eventPkg.m_Cookie, akCommonCallback.eType, akEventCallbackInfo);
					}
					m_mapEventCallbacks.Remove(eventPkg.GetHashCode());
					break;
				}
				case AkCallbackType.AK_EndOfDynamicSequenceItem:
				{
					AkDynamicSequenceItemCallbackInfo akDynamicSequenceItemCallbackInfo = new AkDynamicSequenceItemCallbackInfo
					{
						pCookie = Marshal.ReadIntPtr(pData)
					};
					GotoEndOfCurrentStructMember_IntPtr(ref pData);
					akDynamicSequenceItemCallbackInfo.playingID = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					akDynamicSequenceItemCallbackInfo.audioNodeID = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					akDynamicSequenceItemCallbackInfo.pCustomInfo = Marshal.ReadIntPtr(pData);
					GotoEndOfCurrentStructMember_IntPtr(ref pData);
					eventPkg.m_Callback(eventPkg.m_Cookie, akCommonCallback.eType, akDynamicSequenceItemCallbackInfo);
					break;
				}
				case AkCallbackType.AK_MidiEvent:
				{
					AkMidiEventCallbackInfo akMidiEventCallbackInfo = new AkMidiEventCallbackInfo
					{
						pCookie = Marshal.ReadIntPtr(pData)
					};
					GotoEndOfCurrentStructMember_IntPtr(ref pData);
					akMidiEventCallbackInfo.gameObjID = Marshal.ReadIntPtr(pData);
					GotoEndOfCurrentStructMember_IntPtr(ref pData);
					akMidiEventCallbackInfo.playingID = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					akMidiEventCallbackInfo.eventID = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					akMidiEventCallbackInfo.byType = Marshal.ReadByte(pData);
					GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
					akMidiEventCallbackInfo.byChan = Marshal.ReadByte(pData);
					GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
					switch (akMidiEventCallbackInfo.byType)
					{
					case 128:
					case 144:
						akMidiEventCallbackInfo.byOnOffNote = Marshal.ReadByte(pData);
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						akMidiEventCallbackInfo.byVelocity = Marshal.ReadByte(pData);
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						break;
					case 160:
						akMidiEventCallbackInfo.byAftertouchNote = Marshal.ReadByte(pData);
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						akMidiEventCallbackInfo.byNoteAftertouchValue = Marshal.ReadByte(pData);
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						break;
					case 176:
						akMidiEventCallbackInfo.byCc = Marshal.ReadByte(pData);
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						akMidiEventCallbackInfo.byCcValue = Marshal.ReadByte(pData);
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						break;
					case 192:
						akMidiEventCallbackInfo.byProgramNum = Marshal.ReadByte(pData);
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						break;
					case 208:
						akMidiEventCallbackInfo.byChanAftertouchValue = Marshal.ReadByte(pData);
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						break;
					case 224:
						akMidiEventCallbackInfo.byValueLsb = Marshal.ReadByte(pData);
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						akMidiEventCallbackInfo.byValueMsb = Marshal.ReadByte(pData);
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						break;
					default:
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						GotoEndOfCurrentStructMember_ValueType<byte>(ref pData);
						break;
					}
					eventPkg.m_Callback(eventPkg.m_Cookie, akCommonCallback.eType, akMidiEventCallbackInfo);
					break;
				}
				case AkCallbackType.AK_Marker:
				{
					AkMarkerCallbackInfo akMarkerCallbackInfo = new AkMarkerCallbackInfo
					{
						pCookie = Marshal.ReadIntPtr(pData)
					};
					GotoEndOfCurrentStructMember_IntPtr(ref pData);
					akMarkerCallbackInfo.gameObjID = Marshal.ReadIntPtr(pData);
					GotoEndOfCurrentStructMember_IntPtr(ref pData);
					akMarkerCallbackInfo.playingID = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					akMarkerCallbackInfo.eventID = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					akMarkerCallbackInfo.uIdentifier = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					akMarkerCallbackInfo.uPosition = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					akMarkerCallbackInfo.strLabel = SafeMarshalMarkerString(pData);
					eventPkg.m_Callback(eventPkg.m_Cookie, akCommonCallback.eType, akMarkerCallbackInfo);
					break;
				}
				case AkCallbackType.AK_Duration:
				{
					AkDurationCallbackInfo akDurationCallbackInfo = new AkDurationCallbackInfo
					{
						pCookie = Marshal.ReadIntPtr(pData)
					};
					GotoEndOfCurrentStructMember_IntPtr(ref pData);
					akDurationCallbackInfo.gameObjID = Marshal.ReadIntPtr(pData);
					GotoEndOfCurrentStructMember_IntPtr(ref pData);
					akDurationCallbackInfo.playingID = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					akDurationCallbackInfo.eventID = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					akDurationCallbackInfo.fDuration = MarshalFloat32(pData);
					GotoEndOfCurrentStructMember_ValueType<float>(ref pData);
					akDurationCallbackInfo.fEstimatedDuration = MarshalFloat32(pData);
					GotoEndOfCurrentStructMember_ValueType<float>(ref pData);
					akDurationCallbackInfo.audioNodeID = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					eventPkg.m_Callback(eventPkg.m_Cookie, akCommonCallback.eType, akDurationCallbackInfo);
					break;
				}
				case AkCallbackType.AK_MusicPlayStarted:
				case AkCallbackType.AK_MusicSyncBeat:
				case AkCallbackType.AK_MusicSyncBar:
				case AkCallbackType.AK_MusicSyncEntry:
				case AkCallbackType.AK_MusicSyncExit:
				case AkCallbackType.AK_MusicSyncGrid:
				case AkCallbackType.AK_MusicSyncUserCue:
				case AkCallbackType.AK_MusicSyncPoint:
				{
					AkMusicSyncCallbackInfo akMusicSyncCallbackInfo = new AkMusicSyncCallbackInfo();
					akMusicSyncCallbackInfo.pCookie = Marshal.ReadIntPtr(pData);
					GotoEndOfCurrentStructMember_IntPtr(ref pData);
					akMusicSyncCallbackInfo.gameObjID = Marshal.ReadIntPtr(pData);
					GotoEndOfCurrentStructMember_IntPtr(ref pData);
					akMusicSyncCallbackInfo.playingID = (uint)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_ValueType<uint>(ref pData);
					akMusicSyncCallbackInfo.musicSyncType = (AkCallbackType)Marshal.ReadInt32(pData);
					GotoEndOfCurrentStructMember_EnumType<AkCallbackType>(ref pData);
					akMusicSyncCallbackInfo.fBeatDuration = MarshalFloat32(pData);
					GotoEndOfCurrentStructMember_ValueType<float>(ref pData);
					akMusicSyncCallbackInfo.fBarDuration = MarshalFloat32(pData);
					GotoEndOfCurrentStructMember_ValueType<float>(ref pData);
					akMusicSyncCallbackInfo.fGridDuration = MarshalFloat32(pData);
					GotoEndOfCurrentStructMember_ValueType<float>(ref pData);
					akMusicSyncCallbackInfo.fGridOffset = MarshalFloat32(pData);
					GotoEndOfCurrentStructMember_ValueType<float>(ref pData);
					akMusicSyncCallbackInfo.pszUserCueName = Marshal.PtrToStringAnsi(pData);
					eventPkg.m_Callback(eventPkg.m_Cookie, akCommonCallback.eType, akMusicSyncCallbackInfo);
					break;
				}
				default:
				{
					string message = string.Format("WwiseUnity: PostCallbacks aborted due to error: Undefined callback type found. Callback object possibly corrupted.");
					Debug.LogError(message);
					AkCallbackSerializer.Unlock();
					return;
				}
				}
			}
			if (akCommonCallback.pNext == IntPtr.Zero)
			{
				break;
			}
			pData = akCommonCallback.pNext;
			intPtr = pData;
			akCommonCallback = new AkCommonCallback
			{
				pPackage = Marshal.ReadIntPtr(pData)
			};
			GotoEndOfCurrentStructMember_IntPtr(ref pData);
			akCommonCallback.pNext = Marshal.ReadIntPtr(pData);
			GotoEndOfCurrentStructMember_IntPtr(ref pData);
			akCommonCallback.eType = (AkCallbackType)Marshal.ReadInt32(pData);
			GotoEndOfCurrentStructMember_EnumType<AkCallbackType>(ref pData);
			eventPkg = null;
			bankPkg = null;
			if (!SafeExtractCallbackPackages(akCommonCallback, out eventPkg, out bankPkg))
			{
				AkCallbackSerializer.Unlock();
				return;
			}
			pData = intPtr;
		}
		AkCallbackSerializer.Unlock();
	}

	private static bool SafeExtractCallbackPackages(AkCommonCallback commonCB, out EventCallbackPackage eventPkg, out BankCallbackPackage bankPkg)
	{
		eventPkg = null;
		bankPkg = null;
		if (commonCB.eType == AkCallbackType.AK_AudioInterruption || commonCB.eType == AkCallbackType.AK_AudioSourceChange || commonCB.eType == AkCallbackType.AK_Monitoring)
		{
			return true;
		}
		if (m_mapEventCallbacks.TryGetValue((int)commonCB.pPackage, out eventPkg))
		{
			return true;
		}
		if (m_mapBankCallbacks.TryGetValue((int)commonCB.pPackage, out bankPkg))
		{
			m_mapBankCallbacks.Remove((int)commonCB.pPackage);
			return true;
		}
		return false;
	}

	private static string SafeMarshalString(IntPtr pData)
	{
		return Marshal.PtrToStringAnsi(pData);
	}

	private static string SafeMarshalMarkerString(IntPtr pData)
	{
		return Marshal.PtrToStringAnsi(pData);
	}

	private static void GotoEndOfCurrentStructMember_ValueType<T>(ref IntPtr pData)
	{
		pData = (IntPtr)(pData.ToInt64() + Marshal.SizeOf(typeof(T)));
	}

	private static void GotoEndOfCurrentStructMember_IntPtr(ref IntPtr pData)
	{
		pData = (IntPtr)(pData.ToInt64() + IntPtr.Size);
	}

	private static void GotoEndOfCurrentStructMember_EnumType<T>(ref IntPtr pData)
	{
		pData = (IntPtr)(pData.ToInt64() + Marshal.SizeOf(Enum.GetUnderlyingType(typeof(T))));
	}

	private static float MarshalFloat32(IntPtr pData)
	{
		floatMarshalBuffer[0] = Marshal.ReadByte(pData, 0);
		floatMarshalBuffer[1] = Marshal.ReadByte(pData, 1);
		floatMarshalBuffer[2] = Marshal.ReadByte(pData, 2);
		floatMarshalBuffer[3] = Marshal.ReadByte(pData, 3);
		return BitConverter.ToSingle(floatMarshalBuffer, 0);
	}
}
