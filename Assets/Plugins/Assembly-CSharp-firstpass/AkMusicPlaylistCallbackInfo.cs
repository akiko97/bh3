using System;

public class AkMusicPlaylistCallbackInfo : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public uint playlistID
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_playlistID_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_playlistID_set(swigCPtr, value);
		}
	}

	public uint uNumPlaylistItems
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uNumPlaylistItems_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uNumPlaylistItems_set(swigCPtr, value);
		}
	}

	public uint uPlaylistSelection
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uPlaylistSelection_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uPlaylistSelection_set(swigCPtr, value);
		}
	}

	public uint uPlaylistItemDone
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uPlaylistItemDone_get(swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uPlaylistItemDone_set(swigCPtr, value);
		}
	}

	internal AkMusicPlaylistCallbackInfo(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = cPtr;
	}

	public AkMusicPlaylistCallbackInfo()
		: this(AkSoundEnginePINVOKE.CSharp_new_AkMusicPlaylistCallbackInfo(), true)
	{
	}

	internal static IntPtr getCPtr(AkMusicPlaylistCallbackInfo obj)
	{
		return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
	}

	~AkMusicPlaylistCallbackInfo()
	{
		Dispose();
	}

	public virtual void Dispose()
	{
		lock (this)
		{
			if (swigCPtr != IntPtr.Zero)
			{
				if (swigCMemOwn)
				{
					swigCMemOwn = false;
					AkSoundEnginePINVOKE.CSharp_delete_AkMusicPlaylistCallbackInfo(swigCPtr);
				}
				swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}
}
