using System.Collections;
using System.IO;
using UnityEngine;

namespace MoleMole
{
	public class ScreenRecorder : MonoBehaviour
	{
		[HideInInspector]
		public float sizeScale = 1f;

		[HideInInspector]
		public string outputPath = "D:/tmp/record";

		public float duration = -1f;

		public KeyCode startKey = KeyCode.Q;

		[Range(10f, 120f)]
		public int frameRate = 30;

		[Range(1f, 4f)]
		public int decimateNumber = 1;

		public bool writeAlpha = true;

		public float updateDeltaTime = 0.2f;

		private Camera _camera;

		private PostFX _postFX;

		private float _frameTime;

		private int _frameCount;

		private int _totalFrameCount;

		private int _recordFrameCount;

		private float _timeScale;

		[SerializeField]
		private bool _isRunning;

		private bool _stateChanged;

		private void Awake()
		{
		}

		private void Update()
		{
			GetInput();
			if (_isRunning)
			{
				if (_stateChanged)
				{
					_stateChanged = false;
					InitRecord();
					return;
				}
				_frameCount++;
				if ((_frameCount - 1) % decimateNumber == 0)
				{
					_recordFrameCount++;
					StartCoroutine(Record());
				}
				if (_totalFrameCount > 0 && _frameCount > _totalFrameCount)
				{
					EndRecord();
				}
			}
			else if (_stateChanged)
			{
				Time.timeScale /= _timeScale;
				_timeScale = 1f;
				Time.captureFramerate = 0;
			}
		}

		public void StartRecord()
		{
			if (!_isRunning)
			{
				_isRunning = true;
				_stateChanged = true;
			}
		}

		public void EndRecord()
		{
			if (_isRunning)
			{
				_isRunning = false;
				_stateChanged = true;
			}
		}

		private void InitRecord()
		{
			_frameTime = 1f / (float)frameRate;
			_frameCount = 0;
			if (duration > 0f)
			{
				_totalFrameCount = (int)(duration / _frameTime);
			}
			else
			{
				_totalFrameCount = -1;
			}
			_recordFrameCount = 0;
			_camera = Camera.main;
			_postFX = _camera.GetComponent<PostFX>();
			Time.captureFramerate = (int)(1f / updateDeltaTime);
			_timeScale = _frameTime / updateDeltaTime;
			Time.timeScale *= _timeScale;
			if (1 == 0)
			{
				EndRecord();
			}
		}

		private bool GetInput()
		{
			if (Input.GetKeyUp(startKey))
			{
				_stateChanged = true;
				_isRunning = !_isRunning;
				return true;
			}
			return false;
		}

		private IEnumerator Record()
		{
			yield return new WaitForEndOfFrame();
			string fileName = string.Format("{0,8:D8}", _recordFrameCount);
			SaveImage(fileName);
			yield return null;
		}

		private void SaveImage(string fileName)
		{
			RenderTextureWrapper renderTexture = GraphicsUtils.GetRenderTexture((int)((float)Screen.width * sizeScale), (int)((float)Screen.height * sizeScale), 0, RenderTextureFormat.ARGB32);
			bool flag = false;
			if (_postFX != null)
			{
				flag = _postFX.WriteAlpha;
				_postFX.WriteAlpha = writeAlpha;
			}
			_camera.targetTexture = renderTexture;
			_camera.Render();
			_camera.targetTexture = null;
			if (_postFX != null)
			{
				_postFX.WriteAlpha = flag;
			}
			RenderTexture.active = renderTexture;
			Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
			texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
			if (!writeAlpha || _postFX == null)
			{
				Color[] pixels = texture2D.GetPixels();
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i].a = 1f;
				}
				texture2D.SetPixels(pixels);
			}
			texture2D.Apply();
			byte[] bytes = texture2D.EncodeToPNG();
			File.WriteAllBytes(string.Format("{0}/{1}.png", outputPath, fileName), bytes);
			Object.Destroy(texture2D);
			GraphicsUtils.ReleaseRenderTexture(renderTexture);
		}
	}
}
