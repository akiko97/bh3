using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	[ExecuteInEditMode]
	public class ConfigIntToString : MonoBehaviour
	{
		public FaceAnimationConvertItem[] items;

		private void OnGUI()
		{
			if (GUILayout.Button("Execute"))
			{
				Execute();
			}
		}

		private void Execute()
		{
			int i = 0;
			for (int num = items.Length; i < num; i++)
			{
				ConvertConfig(items[i]);
			}
		}

		private void ConvertConfig(FaceAnimationConvertItem item)
		{
			int i = 0;
			for (int num = item.config.items.Length; i < num; i++)
			{
				FaceAnimationItem faceAnimationItem = item.config.items[i];
				ConvertBlocks(faceAnimationItem.leftEyeBlocks, item.leftEyeProvider.GetMatInfoNames());
				ConvertBlocks(faceAnimationItem.rightEyeBlocks, item.rightEyeProvider.GetMatInfoNames());
				ConvertBlocks(faceAnimationItem.mouthBlocks, item.mouthProvider.GetMatInfoNames());
			}
		}

		private void ConvertBlocks(FaceAnimationFrameBlock[] blocks, string[] names)
		{
		}
	}
}
