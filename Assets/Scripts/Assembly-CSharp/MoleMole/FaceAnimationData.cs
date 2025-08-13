using System;
using System.Collections;
using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public static class FaceAnimationData
	{
		private static Dictionary<string, ConfigFaceAnimation> _dictFaceAnimation = new Dictionary<string, ConfigFaceAnimation>();

		public static void ReloadFromFile()
		{
			_dictFaceAnimation.Clear();
			_dictFaceAnimation["Kiana"] = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Kiana");
			_dictFaceAnimation["Mei"] = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Mei");
			_dictFaceAnimation["Bronya"] = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Bronya");
			_dictFaceAnimation["Himeko"] = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Bronya");
		}

		public static IEnumerator ReloadFromFileAsync(float progressSpan = 0f, Action<float> moveOneStepCallback = null)
		{
			float step = progressSpan / 4f;
			_dictFaceAnimation.Clear();
			AsyncAssetRequst asyncReqeust = ConfigUtil.LoadConfigAsync("FaceAnimation/Kiana");
			yield return asyncReqeust.operation;
			if (moveOneStepCallback != null)
			{
				moveOneStepCallback(step);
			}
			_dictFaceAnimation["Kiana"] = (ConfigFaceAnimation)asyncReqeust.asset;
			asyncReqeust = ConfigUtil.LoadConfigAsync("FaceAnimation/Mei");
			yield return asyncReqeust.operation;
			if (moveOneStepCallback != null)
			{
				moveOneStepCallback(step);
			}
			_dictFaceAnimation["Mei"] = (ConfigFaceAnimation)asyncReqeust.asset;
			asyncReqeust = ConfigUtil.LoadConfigAsync("FaceAnimation/Bronya");
			yield return asyncReqeust.operation;
			if (moveOneStepCallback != null)
			{
				moveOneStepCallback(step);
			}
			_dictFaceAnimation["Bronya"] = (ConfigFaceAnimation)asyncReqeust.asset;
			asyncReqeust = ConfigUtil.LoadConfigAsync("FaceAnimation/Bronya");
			yield return asyncReqeust.operation;
			if (moveOneStepCallback != null)
			{
				moveOneStepCallback(step);
			}
			_dictFaceAnimation["Himeko"] = (ConfigFaceAnimation)asyncReqeust.asset;
		}

		public static ConfigFaceAnimation GetFaceAnimation(string name)
		{
			if (_dictFaceAnimation.ContainsKey(name))
			{
				return _dictFaceAnimation[name];
			}
			return null;
		}
	}
}
