using System.IO;
using UnityEngine;

namespace MoleMole
{
	[AddComponentMenu("Image Effects/PostFX")]
	[RequireComponent(typeof(Camera))]
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class PostFXWithResScale : PostFXBase
	{
		public enum CAMERA_RES_SCALE
		{
			RES_25 = 25,
			RES_50 = 50,
			RES_60 = 60,
			RES_70 = 70,
			RES_80 = 80,
			RES_90 = 90,
			RES_100 = 100,
			RES_150 = 150,
			RES_200 = 200,
			RES_400 = 400
		}

		private static readonly string _INNER_CAMERA_NAME = "InnerCamera";

		[Tooltip("Only do res scale but not other post fx functions")]
		public bool OnlyResScale;

		[Tooltip("The ratio(x100) of camera buffer res to screen")]
		public CAMERA_RES_SCALE CameraResScale = CAMERA_RES_SCALE.RES_100;

		[Tooltip("If not 0, set the scaled width directly ignoring the native res")]
		public int CameraResWidth;

		[Tooltip("If not 0, set the scaled height directly ignoring the native res")]
		public int CameraResHeight;

		public Camera[] CameraList;

		public bool distortionUseDepthOfInnerBuffer;

		private RenderTexture[] _cameraBufferList;

		private RenderTextureWrapper __rplCameraBuffer;

		private bool needSave;

		[HideInInspector]
		public int cullingMask;

		private Camera __innerCamera;

		public int InnerCameraWidth
		{
			get
			{
				if (CameraResWidth != 0)
				{
					return CameraResWidth;
				}
				return ((!(_camera.targetTexture == null)) ? _camera.pixelWidth : Screen.width) * (int)CameraResScale / 100;
			}
		}

		public int InnerCameraHeight
		{
			get
			{
				if (CameraResHeight != 0)
				{
					return CameraResHeight;
				}
				return ((!(_camera.targetTexture == null)) ? _camera.pixelHeight : Screen.height) * (int)CameraResScale / 100;
			}
		}

		private RenderTextureWrapper _rplCameraBuffer
		{
			get
			{
				if (__rplCameraBuffer != null && (__rplCameraBuffer.width != InnerCameraWidth || __rplCameraBuffer.height != InnerCameraHeight))
				{
					GraphicsUtils.ReleaseRenderTexture(__rplCameraBuffer);
					__rplCameraBuffer = null;
				}
				if (__rplCameraBuffer == null)
				{
					__rplCameraBuffer = GraphicsUtils.GetRenderTexture(InnerCameraWidth, InnerCameraHeight, 24, internalBufferFormat);
					if (__rplCameraBuffer == null)
					{
						NotSupported();
					}
				}
				return __rplCameraBuffer;
			}
		}

		private Camera _innerCamera
		{
			get
			{
				if (__innerCamera == null)
				{
					Camera[] componentsInChildren = GetComponentsInChildren<Camera>(true);
					foreach (Camera camera in componentsInChildren)
					{
						if (camera.name == _INNER_CAMERA_NAME)
						{
							__innerCamera = camera;
							break;
						}
					}
					if (__innerCamera == null)
					{
						UseDistortion = false;
					}
				}
				return __innerCamera;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			cullingMask = _camera.cullingMask;
			BackupCameraBuffers();
			ReplaceCameraBuffers();
		}

		protected override void OnDisable()
		{
			_camera.cullingMask = cullingMask;
			RestoreCameraBuffers();
			base.OnDisable();
		}

		protected override void OnDestroy()
		{
			if (__rplCameraBuffer != null)
			{
				GraphicsUtils.ReleaseRenderTexture(__rplCameraBuffer);
				__rplCameraBuffer = null;
			}
			base.OnDestroy();
		}

		protected override void Update()
		{
			base.Update();
		}

		protected void BackupCameraBuffers()
		{
			if (CameraList != null)
			{
				_cameraBufferList = new RenderTexture[CameraList.Length];
				for (int i = 0; i < _cameraBufferList.Length; i++)
				{
					_cameraBufferList[i] = CameraList[i].targetTexture;
				}
			}
		}

		protected void ReplaceCameraBuffers()
		{
			if (CameraList != null)
			{
				Camera[] cameraList = CameraList;
				foreach (Camera camera in cameraList)
				{
					camera.targetTexture = _rplCameraBuffer;
				}
			}
		}

		protected void RestoreCameraBuffers()
		{
			if (CameraList == null)
			{
				return;
			}
			for (int i = 0; i < CameraList.Length; i++)
			{
				if (i < _cameraBufferList.Length)
				{
					CameraList[i].targetTexture = _rplCameraBuffer;
				}
				else
				{
					CameraList[i].targetTexture = _cameraBufferList[i];
				}
			}
		}

		public void OnPreCull()
		{
			if (Application.isPlaying)
			{
				_camera.cullingMask = 0;
				_camera.clearFlags = CameraClearFlags.Nothing;
			}
		}

		public override void OnPreRender()
		{
			if (Application.isPlaying)
			{
				_camera.cullingMask = cullingMask;
				_camera.clearFlags = CameraClearFlags.Color;
			}
			_innerCamera.CopyFrom(_camera);
			_innerCamera.targetTexture = _rplCameraBuffer;
			_innerCamera.aspect = _camera.aspect;
			if (WriteDepthTexture)
			{
				_innerCamera.depthTextureMode = DepthTextureMode.Depth;
			}
			else
			{
				_innerCamera.depthTextureMode = DepthTextureMode.None;
			}
			_innerCamera.Render();
			_innerCamera.targetTexture = null;
		}

		public void OnPostRender()
		{
			RenderTexture renderTexture = _rplCameraBuffer;
			RenderTexture targetTexture = _camera.targetTexture;
			if (CameraResScale == CAMERA_RES_SCALE.RES_400 && CameraResWidth == 0 && CameraResHeight == 0)
			{
				RenderTextureWrapper renderTexture2 = GraphicsUtils.GetRenderTexture(renderTexture.width, renderTexture.height, 0, internalBufferFormat);
				if (OnlyResScale)
				{
					Graphics.Blit(renderTexture, renderTexture2);
					CheckResources();
				}
				else
				{
					DoPostProcess(renderTexture, renderTexture2);
				}
				DownSample4X.SetVector(PostFXBase.HASH_TEXEL_SIZE, new Vector2(1f / (float)renderTexture.width, 1f / (float)renderTexture.height));
				Graphics.Blit((RenderTexture)renderTexture2, targetTexture, DownSample4X, 0);
				GraphicsUtils.ReleaseRenderTexture(renderTexture2);
			}
			else if (OnlyResScale)
			{
				Graphics.Blit((RenderTexture)_rplCameraBuffer, targetTexture);
			}
			else
			{
				DoPostProcess(_rplCameraBuffer, targetTexture);
			}
		}

		protected override void CreateDistortionMap(int w, int h)
		{
			if (distortionUseDepthOfInnerBuffer)
			{
				distortionMap = GraphicsUtils.GetRenderTexture(w, h, DistortionMapDepthBit, DistortionMapFormat);
			}
			else
			{
				distortionMap = GraphicsUtils.GetRenderTexture(w / 2, h / 2, DistortionMapDepthBit, DistortionMapFormat);
			}
		}

		protected override void PrepareDistortion()
		{
			base._distortionCamera.CopyFrom(_camera);
			base._distortionCamera.backgroundColor = DistortionMapClearColor;
			if (UseDepthTest)
			{
				if (distortionUseDepthOfInnerBuffer)
				{
					RenderTexture active = RenderTexture.active;
					RenderTexture.active = distortionMap;
					GL.Clear(false, true, base._distortionCamera.backgroundColor);
					RenderTexture.active = active;
					base._distortionCamera.SetTargetBuffers(distortionMap.content.colorBuffer, _rplCameraBuffer.content.depthBuffer);
					base._distortionCamera.clearFlags = CameraClearFlags.Nothing;
					base._distortionCamera.cullingMask = DistortionCullingMask;
					base._distortionCamera.RenderWithShader(DistortionMapNormShader, "Distortion");
				}
				else
				{
					base._distortionCamera.SetTargetBuffers(distortionMap.content.colorBuffer, distortionMap.content.depthBuffer);
					base._distortionCamera.clearFlags = CameraClearFlags.Color;
					base._distortionCamera.cullingMask = DepthCullingMask;
					base._distortionCamera.RenderWithShader(DrawDepthShader, string.Empty);
					base._distortionCamera.clearFlags = CameraClearFlags.Nothing;
					base._distortionCamera.cullingMask = DistortionCullingMask;
					base._distortionCamera.RenderWithShader(DistortionMapNormShader, "Distortion");
				}
			}
			else
			{
				base._distortionCamera.SetTargetBuffers(distortionMap.content.colorBuffer, distortionMap.content.depthBuffer);
				base._distortionCamera.clearFlags = CameraClearFlags.Color;
				base._distortionCamera.cullingMask = DistortionCullingMask;
				base._distortionCamera.RenderWithShader(DistortionMapNormShader, "Distortion");
			}
		}

		public static void CaptureRT(RenderTexture rt)
		{
			Texture2D texture2D = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
			texture2D.ReadPixels(new Rect(0f, 0f, rt.width, rt.height), 0, 0);
			texture2D.Apply();
			byte[] bytes = texture2D.EncodeToPNG();
			File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
			Object.Destroy(texture2D);
		}
	}
}
