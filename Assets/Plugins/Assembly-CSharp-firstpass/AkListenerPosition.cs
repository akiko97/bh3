using System;

public class AkListenerPosition : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public AkVector OrientationFront
	{
		get
		{
			IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkListenerPosition_OrientationFront_get(swigCPtr);
			return (!(intPtr == IntPtr.Zero)) ? new AkVector(intPtr, false) : null;
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkListenerPosition_OrientationFront_set(swigCPtr, AkVector.getCPtr(value));
		}
	}

	public AkVector OrientationTop
	{
		get
		{
			IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkListenerPosition_OrientationTop_get(swigCPtr);
			return (!(intPtr == IntPtr.Zero)) ? new AkVector(intPtr, false) : null;
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkListenerPosition_OrientationTop_set(swigCPtr, AkVector.getCPtr(value));
		}
	}

	public AkVector Position
	{
		get
		{
			IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkListenerPosition_Position_get(swigCPtr);
			return (!(intPtr == IntPtr.Zero)) ? new AkVector(intPtr, false) : null;
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkListenerPosition_Position_set(swigCPtr, AkVector.getCPtr(value));
		}
	}

	internal AkListenerPosition(IntPtr cPtr, bool cMemoryOwn)
	{
		swigCMemOwn = cMemoryOwn;
		swigCPtr = cPtr;
	}

	public AkListenerPosition()
		: this(AkSoundEnginePINVOKE.CSharp_new_AkListenerPosition(), true)
	{
	}

	internal static IntPtr getCPtr(AkListenerPosition obj)
	{
		return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
	}

	~AkListenerPosition()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkListenerPosition(swigCPtr);
				}
				swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}
}
