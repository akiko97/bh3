using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class GalTouchFaceTestDrive : MonoBehaviour
	{
		public FaceAnimation _faceAnimation;

		public Renderer leftEye;

		public Renderer rightEye;

		public Renderer mouth;

		public TestMatInfoProvider leftEyeProvider;

		public TestMatInfoProvider rightEyeProvider;

		public TestMatInfoProvider mouthProvider;

		private string animationName = string.Empty;

		private FacePartControl leftEyeControl;

		private FacePartControl rightEyeControl;

		private FacePartControl mouthControl;

		private int leftEyeIndex;

		private int rightEyeIndex;

		private int mouthIndex;

		private void Awake()
		{
			_faceAnimation = new FaceAnimation();
			ConfigFaceAnimation config = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Mei");
			leftEyeControl = new FacePartControl();
			leftEyeControl.Init(leftEyeProvider, leftEye);
			rightEyeControl = new FacePartControl();
			rightEyeControl.Init(rightEyeProvider, rightEye);
			mouthControl = new FacePartControl();
			mouthControl.Init(mouthProvider, mouth);
			_faceAnimation.Setup(config, leftEyeControl, rightEyeControl, mouthControl);
		}

		private void Update()
		{
			_faceAnimation.Process(Time.deltaTime);
		}

		private void OnGUI()
		{
			animationName = GUILayout.TextField(animationName);
			if (GUILayout.Button("Play"))
			{
				_faceAnimation.PlayFaceAnimation(animationName);
			}
			GUILayout.BeginHorizontal();
			GUILayout.Label("Left : ");
			int i = 0;
			for (int maxIndex = leftEyeControl.GetMaxIndex(); i < maxIndex; i++)
			{
				if (GUILayout.Button(i.ToString()))
				{
					leftEyeControl.SetFacePartIndex(i);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Right : ");
			int j = 0;
			for (int maxIndex2 = rightEyeControl.GetMaxIndex(); j < maxIndex2; j++)
			{
				if (GUILayout.Button(j.ToString()))
				{
					rightEyeControl.SetFacePartIndex(j);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Mouth : ");
			int k = 0;
			for (int maxIndex3 = mouthControl.GetMaxIndex(); k < maxIndex3; k++)
			{
				if (GUILayout.Button(k.ToString()))
				{
					mouthControl.SetFacePartIndex(k);
				}
			}
			GUILayout.EndHorizontal();
		}
	}
}
