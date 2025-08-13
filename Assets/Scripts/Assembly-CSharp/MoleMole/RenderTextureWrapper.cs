using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class RenderTextureWrapper
	{
		public struct Param
		{
			public int width;

			public int height;

			public int depth;

			public RenderTextureFormat format;

			public RenderTextureReadWrite readWrite;
		}

		private static readonly Dictionary<RenderTextureFormat, RenderTextureFormat[]> _alternativeFormatDict = new Dictionary<RenderTextureFormat, RenderTextureFormat[]>
		{
			{
				RenderTextureFormat.ARGBHalf,
				new RenderTextureFormat[1]
			},
			{
				RenderTextureFormat.R8,
				new RenderTextureFormat[1]
			}
		};

		private RenderTexture _renderTexture;

		private Param _param;

		private List<Camera> _cameraList = new List<Camera>();

		public Action onRebindToCameraCallBack;

		public RenderTexture content
		{
			get
			{
				return _renderTexture;
			}
		}

		public Param param
		{
			get
			{
				return _param;
			}
		}

		public int width
		{
			get
			{
				return _renderTexture.width;
			}
		}

		public int height
		{
			get
			{
				return _renderTexture.height;
			}
		}

		public bool IsValid()
		{
			return _renderTexture != null;
		}

		public void Create(Param param)
		{
			_param = param;
			if (_renderTexture != null)
			{
				RenderTexture.ReleaseTemporary(_renderTexture);
				_renderTexture = null;
			}
			if (GraphicsUtils.isDisableRenderTexture)
			{
				return;
			}
			if (!SystemInfo.SupportsRenderTextureFormat(param.format))
			{
				if (param.format == RenderTextureFormat.ARGBHalf)
				{
					param.format = RenderTextureFormat.ARGB32;
				}
				if (_alternativeFormatDict.ContainsKey(param.format))
				{
					RenderTextureFormat[] array = _alternativeFormatDict[param.format];
					for (int i = 0; i < array.Length; i++)
					{
						if (SystemInfo.SupportsRenderTextureFormat(array[i]))
						{
							param.format = array[i];
						}
					}
				}
			}
			_renderTexture = RenderTexture.GetTemporary(param.width, param.height, param.depth, param.format, param.readWrite);
			if (!(_renderTexture == null))
			{
			}
		}

		public void __Release()
		{
			if (_renderTexture != null)
			{
				RenderTexture.ReleaseTemporary(_renderTexture);
				_renderTexture = null;
			}
			for (int i = 0; i < _cameraList.Count; i++)
			{
				if (_cameraList[i] != null)
				{
					_cameraList[i].targetTexture = null;
				}
			}
			_cameraList.Clear();
		}

		public bool IsCreated()
		{
			return _renderTexture.IsCreated();
		}

		public bool BindToCamera(Camera camera)
		{
			if (camera == null || !IsValid())
			{
				return false;
			}
			camera.targetTexture = _renderTexture;
			if (!_cameraList.Contains(camera))
			{
				_cameraList.Add(camera);
			}
			return true;
		}

		public bool UnbindFromCamera(Camera camera)
		{
			if (camera == null || camera.targetTexture != _renderTexture)
			{
				return false;
			}
			camera.targetTexture = null;
			_cameraList.Remove(camera);
			return true;
		}

		public void RebindToCamera()
		{
			for (int i = 0; i < _cameraList.Count; i++)
			{
				if (_cameraList[i] != null)
				{
					_cameraList[i].targetTexture = _renderTexture;
				}
			}
			if (onRebindToCameraCallBack != null)
			{
				onRebindToCameraCallBack();
			}
		}

		public static implicit operator RenderTexture(RenderTextureWrapper wrapper)
		{
			return wrapper.content;
		}
	}
}
