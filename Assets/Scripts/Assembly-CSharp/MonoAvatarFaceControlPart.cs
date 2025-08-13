using UnityEngine;

public class MonoAvatarFaceControlPart : MonoBehaviour
{
	public Renderer leftEyeRenderer;

	public Renderer rightEyeRenderer;

	public Renderer mouthRenderer;

	public Texture2D[] leftEyeTextures;

	public Texture2D[] rightEyeTextures;

	public Texture2D[] mouthTextures;

	private int _currentLeftEyeIndex;

	private int _currentRightEyeIndex;

	private int _currentMouthIndex;

	public float targetLeftEyeIndex;

	public float targetRightEyeIndex;

	public float targetMouthIndex;

	public bool useUpdateTargetIndex;

	private void Start()
	{
		SetLeftEye(0);
		SetRightEye(0);
		SetMouth(0);
	}

	private void Update()
	{
		if (useUpdateTargetIndex)
		{
			SetLeftEye((int)targetLeftEyeIndex);
			SetRightEye((int)targetRightEyeIndex);
			SetMouth((int)targetMouthIndex);
		}
	}

	public void SetLeftEye(int index)
	{
		SetFacePart(leftEyeRenderer, leftEyeTextures, index, ref _currentLeftEyeIndex);
	}

	public void SetRightEye(int index)
	{
		SetFacePart(rightEyeRenderer, rightEyeTextures, index, ref _currentRightEyeIndex);
	}

	public void SetMouth(int index)
	{
		SetFacePart(mouthRenderer, mouthTextures, index, ref _currentMouthIndex);
	}

	private bool SetFacePart(Renderer renderer, Texture2D[] textures, int targetIndex, ref int currentIndex)
	{
		if (targetIndex == currentIndex)
		{
			return true;
		}
		if (renderer == null)
		{
			Debug.LogError("[GalTouch] face renderer not set : " + base.gameObject.name);
			return false;
		}
		if (targetIndex < 0 || targetIndex >= textures.Length)
		{
			Debug.LogError(string.Format("[GalTouch] face frame index({0}) out of range : {1}", targetIndex.ToString(), base.gameObject.name));
			return false;
		}
		renderer.material.mainTexture = textures[targetIndex];
		currentIndex = targetIndex;
		return true;
	}
}
