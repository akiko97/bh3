using System.Collections.Generic;
using MoleMole.Config;

namespace MoleMole
{
	public class FaceAnimation
	{
		private Dictionary<string, FaceAnimationFrameInfo> _frameInfoDict = new Dictionary<string, FaceAnimationFrameInfo>();

		private FaceAnimationFrameInfo _curFrameInfo;

		private float _timer;

		private FacePartControl _leftEye;

		private FacePartControl _rightEye;

		private FacePartControl _mouth;

		private bool _playing;

		private FaceAnimationPlayMode _playMode;

		public bool isPlaying
		{
			get
			{
				return _playing;
			}
		}

		public void Setup(ConfigFaceAnimation config, FacePartControl leftEye, FacePartControl rightEye, FacePartControl mouth)
		{
			_leftEye = leftEye;
			_rightEye = rightEye;
			_mouth = mouth;
			BuildAnimationFrameInfo(config);
		}

		public void PlayFaceAnimation(string name, FaceAnimationPlayMode mode = FaceAnimationPlayMode.Normal)
		{
			if (!string.IsNullOrEmpty(name) && _frameInfoDict.ContainsKey(name))
			{
				_curFrameInfo = _frameInfoDict[name];
				_timer = 0f;
				_playing = true;
				_playMode = mode;
			}
		}

		public void Stop()
		{
			if (_playing)
			{
				_leftEye.Reset();
				_rightEye.Reset();
				_mouth.Reset();
				_playing = false;
			}
		}

		public void PrepareFaceAnmation(string name)
		{
			if (!string.IsNullOrEmpty(name) && _frameInfoDict.ContainsKey(name))
			{
				_curFrameInfo = _frameInfoDict[name];
				_timer = 0f;
				_playing = false;
			}
		}

		public void Process(float dt)
		{
			if (_playing && _curFrameInfo != null)
			{
				_timer += dt;
				SetupFace();
			}
		}

		public void SetTime(float time)
		{
			if (!_playing && _curFrameInfo != null)
			{
				_timer = time;
				SetupFace();
			}
		}

		public void SetTimePerFrame(float time)
		{
			if (_curFrameInfo != null)
			{
				_curFrameInfo.timePerFrame = time;
			}
		}

		private void SetupFace()
		{
			int num = (int)(_timer / _curFrameInfo.timePerFrame);
			if (num >= 0 && num < _curFrameInfo.length)
			{
				_leftEye.SetFacePartIndex(_curFrameInfo.leftEyeFrames[num]);
				_rightEye.SetFacePartIndex(_curFrameInfo.rightEyeFrames[num]);
				_mouth.SetFacePartIndex(_curFrameInfo.mouthFrames[num]);
			}
			else if (_playMode == FaceAnimationPlayMode.Normal)
			{
				Stop();
			}
			else if (_playMode == FaceAnimationPlayMode.Clamp)
			{
				SetLastFrameFace();
			}
			else if (_playMode == FaceAnimationPlayMode.Loop)
			{
				NormalizeTimer();
			}
		}

		private void SetLastFrameFace()
		{
			if (_curFrameInfo != null)
			{
				_leftEye.SetFacePartIndex(_curFrameInfo.leftEyeFrames[_curFrameInfo.length - 1]);
				_rightEye.SetFacePartIndex(_curFrameInfo.rightEyeFrames[_curFrameInfo.length - 1]);
				_mouth.SetFacePartIndex(_curFrameInfo.mouthFrames[_curFrameInfo.length - 1]);
			}
		}

		private void NormalizeTimer()
		{
			if (_curFrameInfo != null)
			{
				float num = _curFrameInfo.timePerFrame * (float)_curFrameInfo.length;
				while (_timer >= num)
				{
					_timer -= num;
				}
			}
		}

		private void BuildAnimationFrameInfo(ConfigFaceAnimation config)
		{
			_frameInfoDict.Clear();
			int i = 0;
			for (int num = config.items.Length; i < num; i++)
			{
				FaceAnimationFrameInfo faceAnimationFrameInfo = new FaceAnimationFrameInfo();
				faceAnimationFrameInfo.name = config.items[i].name;
				faceAnimationFrameInfo.length = config.items[i].length;
				faceAnimationFrameInfo.timePerFrame = config.items[i].timePerFrame;
				faceAnimationFrameInfo.leftEyeFrames = GetFrameInfoFromBlocks(config.items[i].leftEyeBlocks, config.items[i].length, _leftEye.GetFrameNames());
				faceAnimationFrameInfo.rightEyeFrames = GetFrameInfoFromBlocks(config.items[i].rightEyeBlocks, config.items[i].length, _rightEye.GetFrameNames());
				faceAnimationFrameInfo.mouthFrames = GetFrameInfoFromBlocks(config.items[i].mouthBlocks, config.items[i].length, _mouth.GetFrameNames());
				_frameInfoDict[faceAnimationFrameInfo.name] = faceAnimationFrameInfo;
			}
			if (_curFrameInfo != null && _frameInfoDict.ContainsKey(_curFrameInfo.name))
			{
				_curFrameInfo = _frameInfoDict[_curFrameInfo.name];
			}
		}

		private int[] GetFrameInfoFromBlocks(FaceAnimationFrameBlock[] blocks, int length, string[] names)
		{
			int[] array = new int[length];
			int num = 0;
			if (blocks != null)
			{
				int i = 0;
				for (int num2 = blocks.Length; i < num2; i++)
				{
					int num3 = 0;
					int j = 0;
					for (int num4 = names.Length; j < num4; j++)
					{
						if (blocks[i].frameKey == names[j])
						{
							num3 = j;
							break;
						}
					}
					int k = 0;
					for (int frameLength = blocks[i].frameLength; k < frameLength; k++)
					{
						array[num++] = num3;
						if (num >= length)
						{
							break;
						}
					}
					if (num >= length)
					{
						break;
					}
				}
			}
			return array;
		}
	}
}
